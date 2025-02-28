using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Graphe
    {
        private Dictionary<int, Noeud> noeuds = new();
        private List<Lien> liens = new();
        private Dictionary<int, List<int>> adjacence = new();

        public enum Couleur { Blanc, Jaune, Rouge }
        private Dictionary<int, Couleur> couleurs = new();
        private Dictionary<int, int> dateDecouverte = new();
        private Dictionary<int, int> dateFin = new();
        private Dictionary<int, int> predecesseurs = new();

        public void AjouterNoeud(int id)
        {
            if (!noeuds.ContainsKey(id))
            {
                var noeud = new Noeud(id);
                noeuds[id] = noeud;
                adjacence[id] = new List<int>();
            }
        }
        public void AjouterLien(int id1, int id2)
        {
            AjouterNoeud(id1);
            AjouterNoeud(id2);
            liens.Add(new Lien(noeuds[id1], noeuds[id2]));
            adjacence[id1].Add(id2);
            adjacence[id2].Add(id1);
        }

        public void AfficherGraphe()
        {
            foreach (var lien in liens)
            {
                Console.WriteLine($"{lien.Noeud1.Id} -- {lien.Noeud2.Id}");
            }
        }
        public int[] Tableau_des_noeuds() {
            int taille = noeuds.Values.Count;
            int i = 0;
            int[] Tab_noeud = new int[taille];
            foreach (var noeud in noeuds.Values)
            {
                Tab_noeud[i] = noeud.Id;
                i++;
            }
            TriBulles(Tab_noeud);
            return Tab_noeud;
        }

        public void AfficherNoeuds()
        {
            int taille = noeuds.Values.Count;
            int i = 0;
            int[] Tab_noeud = new int[taille];
            foreach (var noeud in noeuds.Values)
            {
                Tab_noeud[i] = noeud.Id;
                i++;
            }
            TriBulles(Tab_noeud);
            Console.WriteLine("Voici la liste des noeuds du graphe: ");
            foreach (var noeuds in Tab_noeud)
            {
                Console.Write(noeuds.ToString() + ";");
            }
        }

        public static void TriBulles(int[] tab)
        {
            bool resul = true;

            for (int i = 0; i < tab.Length - 1 && resul; i++)
            {
                resul = false;
                for (int j = 0; j < tab.Length - 1 - i; j++)
                {
                    if (tab[j] > tab[j + 1])
                    {
                        // Échanger les éléments
                        int temp = tab[j];
                        tab[j] = tab[j + 1];
                        tab[j + 1] = temp;
                        resul = true;
                    }
                }
            }
        }

        public void GenererDotFile(string dotFile)
        {
            using StreamWriter writer = new(dotFile);
            writer.WriteLine("graph G {");
            foreach (var lien in liens)
            {
                writer.WriteLine($"    {lien.Noeud1.Id} -- {lien.Noeud2.Id};");
            }
            writer.WriteLine("}");
        }
        public bool EstConnexe()
        {
            if (noeuds.Count == 0) return false;
            HashSet<int> visites = new();
            Stack<int> pile = new();
            int premier = noeuds.Keys.First();
            pile.Push(premier);

            while (pile.Count > 0)
            {
                int courant = pile.Pop();
                if (!visites.Contains(courant))
                {
                    visites.Add(courant);
                    foreach (int voisin in adjacence[courant])
                        pile.Push(voisin);
                }
            }
            return visites.Count == noeuds.Count;
        }

        public void DFS_iteratif(int sommetInitial)
        {
            // Initialiser les couleurs, les prédécesseurs et les dates
            Console.WriteLine("Code couleur:\nJaune -> Sommet visité\nRouge -> Sommet sans voisin non visité\nBlanc -> Sommet non visité");
            foreach (var noeud in noeuds.Values)
            {
                couleurs[noeud.Id] = Couleur.Blanc;
                predecesseurs[noeud.Id] = -1;  // Aucun prédécesseur au début
                dateDecouverte[noeud.Id] = -1;
                dateFin[noeud.Id] = -1;
            }

            int date = 0;
            Stack<int> pile = new Stack<int>();
            pile.Push(sommetInitial);
            couleurs[sommetInitial] = Couleur.Jaune;
            dateDecouverte[sommetInitial] = date++;

            while (pile.Count > 0)
            {
                int sommetCourant = pile.Peek();
                bool SuccesseursNonVisites = false;

                // Explorer les voisins
                foreach (int voisin in adjacence[sommetCourant])
                {
                    if (couleurs[voisin] == Couleur.Blanc)
                    {
                        // Empiler le voisin et marquer comme visité
                        pile.Push(voisin);
                        couleurs[voisin] = Couleur.Jaune;
                        predecesseurs[voisin] = sommetCourant;
                        dateDecouverte[voisin] = date++;
                        SuccesseursNonVisites = true;
                        break;
                    }
                }

                // Si tous les voisins sont visités (couleur = rouge), dépiler
                if (!SuccesseursNonVisites)
                {
                    int sommetFini = pile.Pop();
                    couleurs[sommetFini] = Couleur.Rouge;
                    dateFin[sommetFini] = date++;
                }
            }

            Console.WriteLine();
            Console.WriteLine("ID Noeud | Couleur | Date Début | Date Fin");
            Console.WriteLine("------------------------------------------");

            foreach (var noeud in noeuds.Values)
            {

                string val = noeud.Id.ToString() + "       ";
                if (noeud.Id < 10)
                {
                    val += " ";
                }
                string val2 = dateDecouverte[noeud.Id].ToString() + "         ";
                if (dateDecouverte[noeud.Id] < 10)
                {
                    val2 += " ";
                }
                Console.WriteLine(val+ "|" + couleurs[noeud.Id]+"    | "+val2+"| "+dateFin[noeud.Id]);
            }
        }

        // BFS (Parcours en largeur) itératif
        public void BFS_iteratif(int sommetInitial)
        {
            Console.WriteLine("Code couleur:\nJaune -> Sommet visité\nRouge -> Sommet sans voisin non visité\nBlanc -> Sommet non visité");
            foreach (var noeud in noeuds.Values)
            {
                couleurs[noeud.Id] = Couleur.Blanc;
                predecesseurs[noeud.Id] = -1;
                dateDecouverte[noeud.Id] = -1;
            }

            Queue<int> file = new Queue<int>();
            couleurs[sommetInitial] = Couleur.Jaune;
            dateDecouverte[sommetInitial] = 0;
            file.Enqueue(sommetInitial);

            int date = 1;

            // Début du parcours BFS
            while (file.Count > 0)
            {
                int sommetCourant = file.Dequeue();
                Console.WriteLine($"Sommet {sommetCourant} découvert à la date {dateDecouverte[sommetCourant]}");

                // Exploration des voisins du sommet courant
                foreach (int voisin in adjacence[sommetCourant])
                {
                    if (couleurs[voisin] == Couleur.Blanc)
                    {
                        couleurs[voisin] = Couleur.Jaune;
                        predecesseurs[voisin] = sommetCourant;
                        dateDecouverte[voisin] = date++;
                        file.Enqueue(voisin);
                    }
                }

                couleurs[sommetCourant] = Couleur.Rouge;  // Le sommet est complètement exploré
            }

            Console.WriteLine();
            Console.WriteLine("ID Noeud | Couleur | Date Découverte");
            Console.WriteLine("------------------------------------");
            foreach (var noeud in noeuds.Values)
            {
                string val = noeud.Id.ToString()+ "      ";
                if (noeud.Id < 10) {
                    val+= " ";
                }
                Console.WriteLine(val+"| "+ couleurs[noeud.Id]+"    | "+ dateDecouverte[noeud.Id]);
            }
        }
    }

}

