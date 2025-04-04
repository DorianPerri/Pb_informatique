using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Lien
    {
        //attributs
        public int id_station;
        public string nom;
        public int id_precedent;
        public int id_suivant;
        public int temps_deux_stations;
        public int temps_changement;
        public Lien(int id_station, string nom, int id_precedent, int id_suivant, int temps_deux_stations, int temps_changement)
        {
            this.id_station = id_station;
            this.nom = nom;
            this.id_precedent = id_precedent;
            this.id_suivant = id_suivant;
            this.temps_deux_stations = temps_deux_stations;
            this.temps_changement = temps_changement;
        }

        //get/set 
        public int Id_station
        { get { return id_station; } }
        public string Nom
        { get { return nom; } }
        public int Id_precedent
        { get { return id_precedent; } }
        public int Id_suivant
        { get { return id_suivant; } }
        public int Temps_deux_stations
        { get { return temps_deux_stations; } set { temps_deux_stations = value; } }
        public int Temps_changement
        { get { return temps_changement; } }


    }
}
