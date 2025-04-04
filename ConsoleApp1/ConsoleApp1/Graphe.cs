using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    internal class Graphe<T>
    {
        private Dictionary<T, Noeud> noeuds = new();
        private List<Lien> liens = new();
        private Dictionary<T, List<T>> adjacence = new();

        public enum Couleur { Blanc, Jaune, Rouge }
        private Dictionary<T, Couleur> couleurs = new();
        private Dictionary<T, int> dateDecouverte = new();
        private Dictionary<T, int> dateFin = new();
        private Dictionary<T, T?> predecesseurs = new();

        public void AjouterNoeud(T id, Noeud noeud)
        {
            if (!noeuds.ContainsKey(id))
            {
                noeuds[id] = noeud;
                adjacence[id] = new List<T>();
            }
        }

        public void AjouterLien(T id1, T id2, int temps)
        {
            if (!noeuds.ContainsKey(id1) || !noeuds.ContainsKey(id2))
                throw new InvalidOperationException("Les noeuds doivent être ajoutés avant de créer un lien.");

            liens.Add(new Lien(noeuds[id1].Id_station, noeuds[id1].Libelle_station, noeuds[id2].Id_station, id2.GetHashCode(), temps, 0));
            adjacence[id1].Add(id2);
            adjacence[id2].Add(id1);
        }

        public void AfficherGraphe()
        {
            foreach (var lien in liens)
            {
                Console.WriteLine($"{lien.id_station} ({lien.nom}) -- {lien.id_suivant} (Temps: {lien.temps_deux_stations})");
            }
        }

        public bool EstConnexe()
        {
            if (noeuds.Count == 0) return false;
            HashSet<T> visites = new();
            Stack<T> pile = new();
            T premier = noeuds.Keys.First();
            pile.Push(premier);

            while (pile.Count > 0)
            {
                T courant = pile.Pop();
                if (!visites.Contains(courant))
                {
                    visites.Add(courant);
                    foreach (T voisin in adjacence[courant])
                        pile.Push(voisin);
                }
            }
            return visites.Count == noeuds.Count;
        }
    
        public void AjouterNoeud(T id)
            {
                if (!adjacence.ContainsKey(id))
                {
                    adjacence[id] = new List<T>();
                }
            }

            public void AjouterLien(T id1, T id2)
            {
                AjouterNoeud(id1);
                AjouterNoeud(id2);
                adjacence[id1].Add(id2);
                adjacence[id2].Add(id1);
            }

            public void DFS_iteratif(T sommetInitial)
            {
                if (!adjacence.ContainsKey(sommetInitial)) return;

                Console.WriteLine("Code couleur:\nJaune -> Sommet visité\nRouge -> Sommet sans voisin non visité\nBlanc -> Sommet non visité");

                foreach (var noeud in adjacence.Keys)
                {
                    couleurs[noeud] = Couleur.Blanc;
                    predecesseurs[noeud] = default;
                    dateDecouverte[noeud] = -1;
                    dateFin[noeud] = -1;
                }

                int date = 0;
                Stack<T> pile = new();
                pile.Push(sommetInitial);
                couleurs[sommetInitial] = Couleur.Jaune;
                dateDecouverte[sommetInitial] = date++;

                while (pile.Count > 0)
                {
                    T sommetCourant = pile.Peek();
                    bool successeursNonVisites = false;

                    foreach (T voisin in adjacence[sommetCourant])
                    {
                        if (couleurs[voisin] == Couleur.Blanc)
                        {
                            pile.Push(voisin);
                            couleurs[voisin] = Couleur.Jaune;
                            predecesseurs[voisin] = sommetCourant;
                            dateDecouverte[voisin] = date++;
                            successeursNonVisites = true;
                            break;
                        }
                    }

                    if (!successeursNonVisites)
                    {
                        T sommetFini = pile.Pop();
                        couleurs[sommetFini] = Couleur.Rouge;
                        dateFin[sommetFini] = date++;
                    }
                }
            }

            public void BFS_iteratif(T sommetInitial)
            {
                if (!adjacence.ContainsKey(sommetInitial)) return;

                Console.WriteLine("Code couleur:\nJaune -> Sommet visité\nRouge -> Sommet sans voisin non visité\nBlanc -> Sommet non visité");

                foreach (var noeud in adjacence.Keys)
                {
                    couleurs[noeud] = Couleur.Blanc;
                    predecesseurs[noeud] = default;
                    dateDecouverte[noeud] = -1;
                }

                Queue<T> file = new();
                couleurs[sommetInitial] = Couleur.Jaune;
                dateDecouverte[sommetInitial] = 0;
                file.Enqueue(sommetInitial);

                int date = 1;
                while (file.Count > 0)
                {
                    T sommetCourant = file.Dequeue();
                    foreach (T voisin in adjacence[sommetCourant])
                    {
                        if (couleurs[voisin] == Couleur.Blanc)
                        {
                            couleurs[voisin] = Couleur.Jaune;
                            predecesseurs[voisin] = sommetCourant;
                            dateDecouverte[voisin] = date++;
                            file.Enqueue(voisin);
                        }
                    }
                    couleurs[sommetCourant] = Couleur.Rouge;
                }
            }

            public List<T> TrouverCircuit(T sommetDepart)
            {
                if (!adjacence.ContainsKey(sommetDepart)) return new List<T>();

                List<T> circuit = new();
                HashSet<T> visites = new();
                Stack<T> pile = new();
                Dictionary<T, T?> predecesseurs = new();

                pile.Push(sommetDepart);
                visites.Add(sommetDepart);
                predecesseurs[sommetDepart] = default;

                while (pile.Count > 0)
                {
                    T sommetCourant = pile.Peek();
                    bool trouve = false;

                    foreach (T voisin in adjacence[sommetCourant])
                    {
                        if (EqualityComparer<T>.Default.Equals(voisin, sommetDepart) && visites.Count > 1)
                        {
                            circuit.Add(sommetDepart);
                            T? current = sommetCourant;
                            while (current != null && predecesseurs.ContainsKey(current))
                            {
                                circuit.Add(current);
                                current = predecesseurs[current];
                            }
                            circuit.Reverse();
                            return circuit;
                        }

                        if (!visites.Contains(voisin))
                        {
                            pile.Push(voisin);
                            visites.Add(voisin);
                            predecesseurs[voisin] = sommetCourant;
                            trouve = true;
                            break;
                        }
                    }
                    if (!trouve) pile.Pop();
                }
                return new List<T>();
            }
    }

}




