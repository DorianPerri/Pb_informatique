using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Noeud
    {

        public int id_station;
        public string libelle_ligne;
        public string libelle_station;
        public float longitude;
        public float latitude;
        public string commune;
        public int code_commune;

        /// <summary>
        /// Constructeur classique
        /// </summary>
        /// <param name="id_station">identifiant de la station</param>
        /// <param name="libelle_ligne">nom de la ligne de la station</param>
        /// <param name="libelle_station">nom de la station</param>
        /// <param name="longitude">longitude</param>
        /// <param name="latitude">latitude</param>
        /// <param name="commune">comme ou se situe la station</param>
        /// <param name="code_commune">identifiant de la commune</param>
        public Noeud(int id_station, string libelle_ligne, string libelle_station, float longitude, float latitude,
            string commune, int code_commune)
        {
            this.id_station = id_station;
            this.libelle_ligne = libelle_ligne;
            this.libelle_station = libelle_station;
            this.longitude = longitude;
            this.latitude = latitude;
            this.commune = commune;
            this.code_commune = code_commune;
        }


        public int Id_station
        {
            get { return this.id_station; }
        }
        public string Libelle_ligne
        {
            get { return this.libelle_ligne; }
            set { this.libelle_ligne = value; }
        }
        public string Libelle_station
        {
            get { return this.libelle_station; }
            set { this.libelle_station = value; }
        }
        public float Latitude
        {
            get { return this.latitude; }
        }
        public float Longitude
        {
            get { return this.longitude; }
        }
        public string Commune
        {
            get { return this.commune; }
        }
        public int Code_commune
        {
            get { return this.code_commune; }
        }

        public static int ComparerParDistance(Noeud a, Noeud b, Dictionary<Noeud, int> distances)
        {
            return distances[a].CompareTo(distances[b]);
        }

    }



}
