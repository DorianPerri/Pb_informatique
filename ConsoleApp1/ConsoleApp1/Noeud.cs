using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Noeud
    {
        //attributs
        public int id_station;
        public string libelle_ligne;
        public string libelle_station;
        public float longitude;
        public float latitude;
        public string commune;
        public int code_commune;

        //constructeur
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

        //get/set
        public int Id_station
        {
            get { return this.id_station; }
        }
        public string Libelle_ligne
        {
            get { return this.libelle_ligne; }
            set { this.Libelle_ligne = value; }
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
    }


}
