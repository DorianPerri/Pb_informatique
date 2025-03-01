using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using ConsoleApp1;
class Program
{
    static void Main()
    {

        // Création du dossier dans lequel seront enregistré les fichier en png et en dot, le chemin doit changer pour s'adapter à l'utilisateur
        // J'ai mis un chemin en absolue sur mon bureau mais il faudra mettre le bon chemin pour enregistrer les fichier au bon endroit

        // Je récupère le répertoire de base de l'application
        string baseDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
        string Emplacement_fichiers = baseDirectory;
        Directory.CreateDirectory(Emplacement_fichiers);

        // Ici on récupère le chemin pour accéder au fichier soc-karate
        string filePath = Path.Combine(baseDirectory, "soc-karate.mtx");
        string Chemin_dot = Path.Combine(Emplacement_fichiers, "graph.dot");
        string Chemin_Image = Path.Combine(Emplacement_fichiers, "graph.png");

        // Affihage du graphe dans la console
        Graphe graphe = ChargerGrapheDepuisMtx(filePath);
        Console.WriteLine("Voici le graphe:");
        graphe.AfficherGraphe();        
        Console.WriteLine();
        Console.WriteLine("---------------------------------------------------------------------------------------------");

        // Génération du graphe en png
        graphe.GenererDotFile(Chemin_dot);
        RenderGraph(Chemin_dot, Chemin_Image);
        Console.WriteLine("L'image du graphe est générée ici : "+Chemin_Image);
        Console.WriteLine("Le graphe en dot est enregistré ici : " + Chemin_dot);
        Console.WriteLine();
        Console.WriteLine("---------------------------------------------------------------------------------------------");

        // Vérification si le graphe est connexe
        Console.WriteLine("Le graphe est connexe : " + graphe.EstConnexe());
        Console.WriteLine();
        Console.WriteLine("---------------------------------------------------------------------------------------------");        

        // Parcours du graphe        
        int[] Les_noeuds = graphe.Tableau_des_noeuds();
        Random rand = new Random();
        int r = rand.Next(1, Les_noeuds[Les_noeuds.Length-1]);

        // En profondeur        
        Console.WriteLine("Parcours en profondeur:");
        Console.WriteLine();
        graphe.DFS_iteratif(Les_noeuds[r]);
        
        Console.WriteLine();
        Console.WriteLine("---------------------------------------------------------------------------------------------");

        // En largeur
        Console.WriteLine("Parcours en largeur:");
        Console.WriteLine();
        graphe.BFS_iteratif(Les_noeuds[r]);
        Console.WriteLine();
        Console.WriteLine("---------------------------------------------------------------------------------------------");

        // Affichage de la liste des noeuds
        graphe.AfficherNoeuds();
        Console.WriteLine();
        Console.WriteLine("---------------------------------------------------------------------------------------------");

        // Circuit
        Console.Write("Entrez un sommet pour trouver un circuit : ");
        int sommetChoisi;
        while (!int.TryParse(Console.ReadLine(), out sommetChoisi) || !graphe.Tableau_des_noeuds().Contains(sommetChoisi))
        {
            Console.Write("Sommet invalide, veuillez réessayer : ");
        }

        List<int> circuit = graphe.TrouverCircuit(sommetChoisi);
        if (circuit.Count > 0)
        {
            Console.WriteLine("Circuit trouvé : " + string.Join(" -> ", circuit));
        }
        else
        {
            Console.WriteLine("Aucun circuit trouvé à partir du sommet " + sommetChoisi);
        }
        Console.WriteLine();
        Console.WriteLine("---------------------------------------------------------------------------------------------");

        Console.WriteLine("Fin de l'analyse du fichier");        
    }

    static Graphe ChargerGrapheDepuisMtx(string filePath)
    {
        Graphe graphe = new();
        foreach (var line in File.ReadLines(filePath))
        {
            if (line.StartsWith("%")) continue;
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                int node1 = int.Parse(parts[0]);
                int node2 = int.Parse(parts[1]);
                if (node1 != node2) graphe.AjouterLien(node1, node2);
            }
        }
        return graphe;
    }

    static void RenderGraph(string dotFile, string outputImage)
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "dot",
                Arguments = $"-Tpng \"{dotFile}\" -o \"{outputImage}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Erreur Graphviz : {error}");
                }
                else
                {
                    Console.WriteLine("Graphviz a généré l'image avec succès.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'exécution de Graphviz : {ex.Message}");
        }
    }
}
