using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using K4os.Compression.LZ4.Internal;
using Mysqlx.Crud;

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

        /// <summary>
        /// Ajouter un noeud au graphe
        /// </summary>
        /// <param name="id">identifiant de la station à ajouter au graphe</param>
        /// <param name="noeud">noeud à ajouter</param>
        public void AjouterNoeud(T id, Noeud noeud)
        {
            if (!noeuds.ContainsKey(id))
            {
                noeuds[id] = noeud;
                adjacence[id] = new List<T>();
            }
        }

        /// <summary>
        /// Ajouter un lien entre deux noeuds
        /// </summary>
        /// <param name="id1">identifiant de la station de départ</param>
        /// <param name="id2">identifiant de la station d'arrivée</param>
        /// <param name="temps">temps de trajet entre 2 stations</param>
        /// <exception cref="InvalidOperationException">Exception à gérer en cas de problème pour éviter de faire crasher le </exception>
        public void AjouterLien(T id1, T id2, int temps)
        {
            if (!noeuds.ContainsKey(id1) || !noeuds.ContainsKey(id2))
                throw new InvalidOperationException("Les noeuds doivent être ajoutés avant de créer un lien.");

            liens.Add(new Lien(noeuds[id1].Id_station, noeuds[id1].Libelle_station, noeuds[id2].Id_station, id2.GetHashCode(), temps, 0));
            adjacence[id1].Add(id2);
            adjacence[id2].Add(id1);
        }

        /// <summary>
        /// Affiche le graphe dans la console
        /// </summary>
        public void AfficherGraphe()
        {
            foreach (var lien in liens)
            {
                Console.WriteLine($"{lien.id_station} ({lien.nom}) -- {lien.id_suivant} (Temps: {lien.temps_deux_stations})");
            }
        }

        /// <summary>
        /// Vérifier si un graphe est connexe
        /// Si le graphe ne possède aucun lien entre ses noeuds on retourne false
        /// Crée un ensemble pour stocker les noeud déjà visités (HashSet<T> visites)
        /// Crée une pile pour faire un parcours en profondeur (Stack<T> pile)
        /// On démarre le parcours depuis le premier noeud du graphe
        /// On le met sur la pile pour l’explorer ensuite (pile.Push(premier))
        /// On parcours le graphe en profondeur
        /// On retire le sommet qu'on vient de visiter du haut de la pile (pile.Pop)
        /// Si on ne l’a pas encore visité on le marque comme visité et on empile ses voisins pour les visiter ensuite (pile.Push)
        /// Le graphe est connexe si et seulement si : le nombre de nœuds visités = le nombre total de nœuds.
        /// </summary>
        /// <returns>true si le graphe est connexe false sinon</returns>
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
            if (visites.Count == noeuds.Count)
            {
                return true;
            }
            else { 
                return false;
            }
        }    

        /// <summary>
        /// Ajouter un noeud à un graphe à partir de son identifiant seulement
        /// </summary>
        /// <param name="id">identifiant du noeud à ajouter au graphe</param>
        public void AjouterNoeud(T id)
            {
                if (!adjacence.ContainsKey(id))
                {
                    adjacence[id] = new List<T>();
                }
            }

        /// <summary>
        /// Ajouter un lien entre deux noeuds
        /// </summary>
        /// <param name="id1">premier noeud à ajouter un lien</param>
        /// <param name="id2">deuxième noeud auquel ajouter un lien</param>
        public void AjouterLien(T id1, T id2)
            {
                AjouterNoeud(id1);
                AjouterNoeud(id2);
                adjacence[id1].Add(id2);
                adjacence[id2].Add(id1);
            }

        /// <summary>
        /// Parcours le graphe en profondeur (basé sur l'algorithme du cours) et affiche le résultat dans la console
        /// Ajoute des couleurs aux sommets visités (pour le rendu 1)
        /// On vérifie d'abord que le sommet existe dans le graphe (si il est dans la liste d'adjacence), si ce n'est pas le cas on arrête le programme
        /// Pour chaque noeud du graphe, on définit la couleur à Blanc pour dire qu'il n'est pas visité
        /// On initialise les prédécesseurs à default
        /// On initialise les dates de découverte et de fin à -1
        /// On empile le sommet de départ (pile.Push)
        /// Quand un sommet est visité on le met en jaune
        /// On lui attribut une date de découverte (pour pouvoir chronométrer le temps que met l'algorithme à parcourir le graphe)
        /// Ensuite tant qu'il y a des sommet non visité dans la pile on continue 
        /// Pour chaque voisin du sommet courant si il est non visité on l'empile pour l'explorer plus tard, on le marque en jaune, 
        /// on enregistre son prédécesseur, on enregistre sa date de découverte, on indique qu'on a découvert un voisin à explorer
        /// On vérifie quon ai encore un voisin à explorer, si ce n'est pas le cas on retire le sommet de la pile, on le met en rouge (exploré) et on enregistre la date de fin
        /// </summary>
        /// <param name="sommetInitial">Sommet de départ du part</param>
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

        /// <summary>
        /// Parcours en largeur du graphe
        /// Si le sommet n'existe pas dans le graphe on arrête le programme
        /// On définit un code couleur
        /// On met tous les sommets en blanc car non visités
        /// On crée une file pour stocker les noeuds à visiter
        /// On commence par le comment initial qu'on met en jaune
        /// On initilise la date de découverte (pour chronométrer et comparer avec le parcours en profondeur)
        /// Tant qu'il y a des sommets dans la file on continue
        /// On retire le sommet en première position dans la file
        /// On parcours tous ses voisins
        /// Si un voisin n'a pas encore été visité on le marque en jaune et on l'ajoute à la file
        /// Une fois tous les voisins explorés, on marque le sommet en rouge
        /// </summary>
        /// <param name="sommetInitial"></param>
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

        /// <summary>
        /// Permet de trouver un circuit dans un graphe
        /// Si le sommet n'existe pas on arrete le programme
        /// On crée une liste circuit qui va stocker les sommets du circuit
        /// Une liste (HashSet) visite qui servira a éviter de visiter plusieurs fois le même sommet
        /// Une pile pour suivre le chemin
        /// Un dictionnaire qui va stocker les prédecesseurs pour reconstruire le chemin si un cycle est trouvé
        /// On ajoute le sommet de départ à la pile, on le note comme visité et on note qu'il n'a pas de prédecesseur
        /// Tant que la pile n'est pas vidée on la parcours
        /// On regarde le sommet en haut de la pile et on note grâce à 'trouve' si il a des voisins à explorer
        /// Maintenant on parcours ses voisins
        /// Si on tombe sur le sommet de départ alors on aura trouvé un cycle
        /// On reconstruit ensuite le chemin pour stocker le cycle
        /// On n'oublie pas d'inverser la liste pour avoir le cycle dans le bon sens
        /// Si on a un voisin non visité on l'ajoute à la pile, on le marque comme visité et on marque son prédécesseur
        /// Si il n'a pas de voisin on le supprime de la pile
        /// </summary>
        /// <param name="sommetDepart">Sommet de départ du cycle</param>
        /// <returns>Liste des sommets du cycle</returns>
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




