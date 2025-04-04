using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class GraphBuilder
    {
        public Dictionary<int, Station> BuildGraph(string nodesPath, string arcsPath)
        {
            var stations = new Dictionary<int, Station>();

            // Charger les noeuds
            using (var reader = new StreamReader(nodesPath))
            {
                reader.ReadLine(); // Skip header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(';');
                    var station = new Station
                    {
                        Id = int.Parse(line[0]),
                        Name = line[2],
                        Longitude = double.Parse(line[3].Replace(".",",")),
                        Latitude = double.Parse(line[4].Replace(".", ","))
                    };
                    stations[station.Id] = station;
                }
            }

            // Charger les arcs
            using (var reader = new StreamReader(arcsPath))
            {
                reader.ReadLine(); // Skip header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(';');
                    var currentId = int.Parse(line[0]);

                    if (!string.IsNullOrEmpty(line[2])) // Précédent
                    {
                        AddConnection(stations, currentId, int.Parse(line[2]), line[4], line[5]);
                    }

                    if (!string.IsNullOrEmpty(line[3])) // Suivant
                    {
                        AddConnection(stations, currentId, int.Parse(line[3]), line[4], line[5]);
                    }
                }
            }

            return stations;
        }

        private void AddConnection(Dictionary<int, Station> stations, int sourceId, int targetId, string travelTime, string changeTime)
        {
            if (stations.TryGetValue(sourceId, out var source) && stations.ContainsKey(targetId))
            {
                source.Connections.Add(new Connection
                {
                    TargetStationId = targetId,
                    TravelTime = int.Parse(travelTime),
                    ChangeTime = int.Parse(changeTime)
                });
            }
        }
        public void GenerateGraphvizImage(Dictionary<int, Station> graph, string outputPath)
        {
            var dotContent = new System.Text.StringBuilder();
            dotContent.AppendLine("digraph Metro {");
            dotContent.AppendLine("    rankdir=LR; node [shape=box];");

            // Nodes
            foreach (var station in graph.Values)
            {
                dotContent.AppendLine($"    {station.Id} [label=\"{station.Name}\\n({station.Longitude}, {station.Latitude})\"];");
            }

            // Edges
            foreach (var station in graph.Values)
            {
                foreach (var conn in station.Connections)
                {
                    dotContent.AppendLine($"    {station.Id} -> {conn.TargetStationId} [label=\"T: {conn.TravelTime}m\\nC: {conn.ChangeTime}m\"];");
                }
            }

            dotContent.AppendLine("}");

            // Sauvegarder le fichier DOT temporaire
            string dotFilePath = "temp.dot";
            File.WriteAllText(dotFilePath, dotContent.ToString());
            Console.WriteLine($"Fichier DOT créé à l'emplacement : {dotFilePath}");

            // Créer un processus pour appeler Graphviz (dot)
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dot", // Assurez-vous que 'dot' est dans le PATH
                Arguments = $"-Tpng {dotFilePath} -o {outputPath}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,  // Capturer les erreurs
                CreateNoWindow = true
            };

            try
            {
                var process = Process.Start(processStartInfo);
                process.WaitForExit();  // Attendre la fin du processus

                if (process.ExitCode == 0)
                {
                    Console.WriteLine($"Image générée avec succès à {outputPath}");
                }
                else
                {
                    string error = process.StandardError.ReadToEnd();
                    Console.WriteLine("Erreur lors de l'exécution de Graphviz :");
                    Console.WriteLine(error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'appel à Graphviz : {ex.Message}");
            }
        }

        public void SaveGraphToFile(Dictionary<int, Station> graph, string outputPath)
        {
            using (var writer = new StreamWriter(outputPath))
            {
                foreach (var station in graph.Values)
                {
                    writer.WriteLine($"Station {station.Id}: {station.Name}");
                    writer.WriteLine($"Coordonnées: ({station.Longitude}, {station.Latitude})");
                    writer.WriteLine("Connections:");
                    foreach (var conn in station.Connections)
                    {
                        writer.WriteLine($"\t→ Station {conn.TargetStationId} (Travel: {conn.TravelTime} min, Change: {conn.ChangeTime} min)");
                    }
                    writer.WriteLine();
                }
            }
        }
        public Dictionary<int, Station> BuildGraphFromText(string filePath)
        {
            var stations = new Dictionary<int, Station>();
            int stationId = 1;

            using (var reader = new StreamReader(filePath))
            {
                Station currentStation = null;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Trim();

                    // Si on rencontre le début d'une station
                    if (line.StartsWith("Station"))
                    {
                        if (currentStation != null)
                        {
                            stations[currentStation.Id] = currentStation; // Sauver la station précédente
                        }

                        var match = Regex.Match(line, @"Station (\d+): (.+)");
                        if (match.Success)
                        {
                            currentStation = new Station
                            {
                                Id = stationId++,
                                Name = match.Groups[2].Value
                            };
                        }
                    }

                    // Si on rencontre les coordonnées
                    else if (line.StartsWith("Coordonnées:"))
                    {
                        var match = Regex.Match(line, @"Coordonnées: \((\d+,\d+), (\d+,\d+)\)");
                        if (match.Success && currentStation != null)
                        {
                            currentStation.Longitude = double.Parse(match.Groups[1].Value.Replace(".", ","));
                            currentStation.Latitude = double.Parse(match.Groups[2].Value.Replace(".", ","));
                        }
                    }

                    // Si on rencontre les connexions
                    else if (line.StartsWith("Connections:"))
                    {
                        continue; // Nous allons parser les connexions dans la ligne suivante
                    }

                    // Parsing des connexions
                    else if (line.StartsWith("→"))
                    {
                        var match = Regex.Match(line, @"→ Station (\d+) \(Travel: (\d+) min, Change: (\d+) min\)");
                        if (match.Success && currentStation != null)
                        {
                            int targetStationId = int.Parse(match.Groups[1].Value);
                            int travelTime = int.Parse(match.Groups[2].Value);
                            int changeTime = int.Parse(match.Groups[3].Value);

                            currentStation.Connections.Add(new Connection
                            {
                                TargetStationId = targetStationId,
                                TravelTime = travelTime,
                                ChangeTime = changeTime
                            });
                        }
                    }
                }

                // Sauver la dernière station
                if (currentStation != null)
                {
                    stations[currentStation.Id] = currentStation;
                }
            }

            return stations;
        }


    }
}
