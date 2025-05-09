    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Diagnostics;
    using QuickGraph;
    using QuickGraph.Graphviz;
    using QuickGraph.Graphviz.Dot;
    using ConsoleApp1;
    using MySql.Data.MySqlClient;
    using Org.BouncyCastle.Bcpg.OpenPgp;
    using System.Globalization;
    using Org.BouncyCastle.Asn1;
    using System.Numerics;
using System.Diagnostics.Tracing;
using System.Text.Json;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Xml;
using System.Data;
class Program
{
        static void Main()
        {
            Console.Clear();
            Console.WriteLine("Importation des Neouds...");
            Noeud[] infos_noeuds = Noeud_depuis_csv("Noeuds.csv");
            Console.WriteLine($"Length de infos_noeuds : {infos_noeuds.Length}");
            Console.WriteLine("\nImportation des Liens...");
            Lien[] lienscsv = Lien_depuis_csv("Arcs.csv");
            Console.WriteLine("\nCréation de la liste d'adjacence d'affichage...");
            Dictionary<string, string[]> Dico_adjacence_affichage = Dico_adj_affichage(lienscsv, infos_noeuds);
            Console.WriteLine("\nCréation de la liste d'adjacence réelle...");
            Dictionary<Noeud, Noeud[]> Dico_adjacence_reel = Dico_ajd_reel(lienscsv, infos_noeuds);
            

            Console.WriteLine("fin de l'intitialisation");
            Console.WriteLine("Appuyez sur une touche pour continuer");
            Console.ReadLine();
            Console.Clear();


            Console.WriteLine("Connexion au serveur");
            Console.WriteLine("Si votre serveur est localhost, entrez '1', sinon entrez le nom de votre serveur");
            Console.Write("Serveur: ");
            string server = Console.ReadLine();
            Console.Write("Identifiant: ");
            string identifiant = Console.ReadLine();
            Console.Write("Mot de passe: ");
            string motDePasse = Console.ReadLine();

            if (server == "1") {
                server = "localhost";
            }
        
            string connectionString = $"SERVER={server};PORT=3306;DATABASE=Application;UID={identifiant};PASSWORD={motDePasse};";

           try {
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();

                bool continuer = true;

                while (continuer)
                {
                    Console.Clear();
                    Console.WriteLine("1: Créer un utilisateur");
                    Console.WriteLine("2: Vous êtes un client");
                    Console.WriteLine("3: Vous êtes un cuisinier");
                    Console.WriteLine("4: Afficher le graphe");
                    Console.WriteLine("5: Carte du métro avec les couleurs");
                    Console.WriteLine("6: Chercher un PCC");
                    Console.WriteLine("7: Graphe d'interraction entre les utilisateurs");
                    Console.WriteLine("8: Exporter la base de données en XML");
                    Console.WriteLine("Échap: Quitter");

                    ConsoleKeyInfo cki = Console.ReadKey(true);

                    switch (cki.Key)
                    {
                        case ConsoleKey.D1:
                        case ConsoleKey.NumPad1:
                            Console.Clear();
                            Console.WriteLine("Vous allez créer un utilisateur");

                            Console.WriteLine("L'utilisateur est-il un cuisinier ou un client?");
                            string type_utilisateur = Console.ReadLine();
                            type_utilisateur = type_utilisateur.ToLower();

                            if (type_utilisateur != "cuisinier" && type_utilisateur != "client")
                            {
                                while (type_utilisateur != "cuisinier" && type_utilisateur != "client")
                                {
                                    Console.WriteLine("Vous devez indiquer client ou cuisinier");
                                    type_utilisateur = Console.ReadLine();
                                    type_utilisateur = type_utilisateur.ToLower();
                                }
                            }

                            string nom_utilisateur = "";
                            string prenom_utilisateur = "";
                            int nb_commande_utilisateur = 0;

                            string type_de_client = "";
                            DateTime date_inscription_client = DateTime.Today;
                            string nom_referent = "";
                            string mail_referent = "";
                            string tel_referent = "";

                            float note_moyenne = 0;

                            if (type_utilisateur == "client")
                            {
                                Console.WriteLine("Le client est-il un particulier ou une entreprise?");
                                type_de_client = Console.ReadLine();
                                type_de_client = type_de_client.ToLower();

                                if (type_de_client != "particulier" && type_de_client != "entreprise")
                                {
                                    while (type_de_client != "particulier" && type_de_client != "entreprise")
                                    {
                                        Console.WriteLine("Vous devez indiquer particulier ou entreprise");
                                        type_de_client = Console.ReadLine();
                                        type_de_client = type_de_client.ToLower();
                                    }
                                }

                                if (type_de_client == "particulier")
                                {
                                    Console.WriteLine("Quel est le nom du client?");
                                    nom_utilisateur = Console.ReadLine();

                                    Console.WriteLine("Quel est le prénom du client?");
                                    prenom_utilisateur = Console.ReadLine();
                                }
                                if (type_de_client == "entreprise")
                                {
                                    Console.WriteLine("Quel est le nom de l'entreprise?");
                                    nom_utilisateur = Console.ReadLine();

                                    prenom_utilisateur = "entreprise";

                                    Console.WriteLine("Quel est le nom du référent?");
                                    nom_referent = Console.ReadLine();

                                    Console.WriteLine("Quel est le mail du référent?");
                                    mail_referent = Console.ReadLine();

                                    Console.WriteLine("Quel est le numéro de téléphone du référent?");
                                    tel_referent = Console.ReadLine();
                                }
                            }

                            if (type_utilisateur == "cuisinier")
                            {
                                Console.WriteLine("Quel est le nom du cuisinier?");
                                nom_utilisateur = Console.ReadLine();

                                Console.WriteLine("Quel est le prénom du cuisinier?");
                                prenom_utilisateur = Console.ReadLine();
                            }

                            Console.WriteLine("Quel est le nom de sa rue?");
                            string nomRue_utilisateur = Console.ReadLine();

                            Console.WriteLine();
                            int numeroRue_utilisateur = SaisieEntier("Quel est le numéro de sa rue?");
                            Console.WriteLine();
                            int codePostal_utilisateur = SaisieEntier("Quel est son code postal?");
                            Console.WriteLine();
                            Console.WriteLine("Quel est le nom de sa ville?");
                            string ville_utilisateur = Console.ReadLine();
                            Console.WriteLine();
                            int telephone_utilisateur = SaisieEntier("Quel est son numéro de téléphone? (les +33 ne sont pas acceptés)"); ;
                            Console.WriteLine();
                            Console.WriteLine("Quel est son mail?");
                            string mail_utilisateur = Console.ReadLine();
                            Console.WriteLine();
                            bool metro_valide = false;
                            string metro_utilisateur = "";
                            while (metro_valide == false)
                            {
                                Console.WriteLine("Quel est le métro le plus proche de lui? (Saisissez une station valide)");
                                metro_utilisateur = Console.ReadLine();

                                foreach (var station_metro in infos_noeuds)
                                {
                                    if (station_metro.libelle_station == metro_utilisateur)
                                    {
                                        metro_valide = true;
                                    }
                                }
                            }
                            Console.WriteLine();
                            Console.WriteLine("Quel est son mot de passe?");
                            string mdp_utilisateur = Console.ReadLine();

                            string identifiant_utilisateur = "";

                            for (int i = 0; i < 7; i++)
                            {
                                Random random = new Random();
                                int nombreAleatoire = random.Next(9);
                                identifiant_utilisateur += nombreAleatoire;
                            }
                            long identifiant_utilisateur_int = long.Parse(identifiant_utilisateur);

                            MySqlCommand ajouter_utilisateur = connection.CreateCommand();
                            ajouter_utilisateur.CommandText = "INSERT INTO Utilisateur (ID_Client, Nom, Prenom, Rue, Numero, Code_Postal, Ville, Telephone, Email, Metro_Proche, Type_client, Mot_De_Passe) " +
                              "VALUES (@ID_Client, @Nom, @Prenom, @Rue, @Numero, @Code_Postal, @Ville, @Telephone, @Email, @Metro_Proche, @Type_client, @Mot_De_Passe)";

                            ajouter_utilisateur.Parameters.AddWithValue("@ID_Client", identifiant_utilisateur_int);
                            ajouter_utilisateur.Parameters.AddWithValue("@Nom", nom_utilisateur);
                            ajouter_utilisateur.Parameters.AddWithValue("@Prenom", prenom_utilisateur);
                            ajouter_utilisateur.Parameters.AddWithValue("@Rue", nomRue_utilisateur);
                            ajouter_utilisateur.Parameters.AddWithValue("@Numero", numeroRue_utilisateur);
                            ajouter_utilisateur.Parameters.AddWithValue("@Code_Postal", codePostal_utilisateur);
                            ajouter_utilisateur.Parameters.AddWithValue("@Ville", ville_utilisateur);
                            ajouter_utilisateur.Parameters.AddWithValue("@Telephone", telephone_utilisateur);
                            ajouter_utilisateur.Parameters.AddWithValue("@Email", mail_utilisateur);
                            ajouter_utilisateur.Parameters.AddWithValue("@Metro_Proche", metro_utilisateur);
                            ajouter_utilisateur.Parameters.AddWithValue("@Type_client", type_utilisateur);
                            ajouter_utilisateur.Parameters.AddWithValue("@Mot_De_Passe", mdp_utilisateur);

                            MySqlDataReader reader_ajouter_utilisateur;
                            reader_ajouter_utilisateur = ajouter_utilisateur.ExecuteReader();
                            reader_ajouter_utilisateur.Close();

                            if (type_utilisateur == "client")
                            {
                                MySqlCommand command_client = connection.CreateCommand();
                                command_client.CommandText = $"INSERT INTO Client (ID_Client, Date_inscription, Nombre_commande)" +
                                                                "VALUES (@ID_Client, @Date_inscription, @Nombre_commande);";

                                command_client.Parameters.AddWithValue("@ID_Client", identifiant_utilisateur_int);
                                command_client.Parameters.AddWithValue("@Date_inscription", date_inscription_client);
                                command_client.Parameters.AddWithValue("@Nombre_commande", nb_commande_utilisateur);
                                MySqlDataReader reader_client;
                                reader_client = command_client.ExecuteReader();
                                reader_client.Close();

                                if (type_de_client == "entreprise")
                                {
                                    MySqlCommand command_client_entreprise = connection.CreateCommand();
                                    command_client_entreprise.CommandText = $"INSERT INTO Entreprise (ID_Client, Referent, telephone_ref, mail_ref)" +
                                                                            "VALUES (@ID_Client, @Referent, @telephone_ref, @mail_ref);";
                                    command_client_entreprise.Parameters.AddWithValue("@ID_Client", identifiant_utilisateur_int);
                                    command_client_entreprise.Parameters.AddWithValue("@Referent", nom_referent);
                                    command_client_entreprise.Parameters.AddWithValue("@telephone_ref", tel_referent);
                                    command_client_entreprise.Parameters.AddWithValue("@mail_ref", mail_referent);

                                    MySqlDataReader reader_client_entreprise;
                                    reader_client_entreprise = command_client_entreprise.ExecuteReader();
                                    reader_client_entreprise.Close();
                                }
                                if (type_de_client == "particulier")
                                {
                                    MySqlCommand command_client_particulier = connection.CreateCommand();
                                    command_client_particulier.CommandText = $"INSERT INTO Particulier (ID_Client, Prenom) VALUES (@ID_Client, @Prenom);";

                                    command_client_particulier.Parameters.AddWithValue("@ID_Client", identifiant_utilisateur_int);
                                    command_client_particulier.Parameters.AddWithValue("@Prenom", prenom_utilisateur);

                                    MySqlDataReader reader_client_particulier;
                                    reader_client_particulier = command_client_particulier.ExecuteReader();
                                    reader_client_particulier.Close();
                                }
                            }
                            else
                            {
                                MySqlCommand command_cuisinier = connection.CreateCommand();
                                command_cuisinier.CommandText = "INSERT INTO Cuisinier (ID_Client, Prenom_cuisinier, Nombre_commande_livrer, Note_moyenne_cuisinier) " +
                                                                "VALUES (@ID_Client, @Prenom_cuisinier, @Nombre_commande_livrer, @Note_moyenne_cuisinier)";

                                command_cuisinier.Parameters.AddWithValue("@ID_Client", identifiant_utilisateur_int);
                                command_cuisinier.Parameters.AddWithValue("@Prenom_cuisinier", prenom_utilisateur);
                                command_cuisinier.Parameters.AddWithValue("@Nombre_commande_livrer", nb_commande_utilisateur);
                                command_cuisinier.Parameters.AddWithValue("@Note_moyenne_cuisinier", note_moyenne);

                                MySqlDataReader reader_cuisinier;
                                reader_cuisinier = command_cuisinier.ExecuteReader();
                                reader_cuisinier.Close();
                            }
                            Console.WriteLine($"{nom_utilisateur} a été inséré avec succès (identifiant: {identifiant_utilisateur}) !");
                            Pause();
                            break;


                        case ConsoleKey.D2:
                        case ConsoleKey.NumPad2:
                            Console.Clear();
                            Console.WriteLine("Connexion");

                            Console.WriteLine("Quel est votre identifiant?");
                            int id_connection_client = Convert.ToInt32(Console.ReadLine());

                            Console.WriteLine("Quel est votre mot de passe?");
                            string mdp_connexion = Console.ReadLine();

                            MySqlCommand connection_id_requete = connection.CreateCommand();
                            connection_id_requete.CommandText = "SELECT ID_Client FROM Utilisateur WHERE ID_Client = @ID_Client";
                            connection_id_requete.Parameters.AddWithValue("@ID_Client", id_connection_client);

                            MySqlDataReader reader_id_requete = connection_id_requete.ExecuteReader();

                            if (!reader_id_requete.Read())
                            {
                                Console.WriteLine("Échec de connexion.");
                                reader_id_requete.Close();
                            }
                            else
                            {
                                reader_id_requete.Close();

                                MySqlCommand connection_mdp_requete = connection.CreateCommand();
                                connection_mdp_requete.CommandText = "SELECT Mot_De_Passe FROM Utilisateur WHERE ID_Client = @ID_Client AND Type_client = 'client'";
                                connection_mdp_requete.Parameters.AddWithValue("@ID_Client", id_connection_client);

                                MySqlDataReader reader_mdp_requete = connection_mdp_requete.ExecuteReader();

                                if (reader_mdp_requete.Read())
                                {
                                    string mdp_Stocke = reader_mdp_requete.GetString(0);
                                    if (mdp_Stocke == mdp_connexion)
                                    {
                                        reader_mdp_requete.Close();
                                        Console.Clear();
                                        Console.WriteLine("Connexion réussie !\nVeuillez patienter quelques secondes");
                                        Thread.Sleep(2000);
                                        Console.Clear();

                                        MySqlCommand nom_connecte = connection.CreateCommand();
                                        nom_connecte.CommandText = "SELECT nom, prenom FROM Utilisateur WHERE ID_Client = @ID_Client AND Type_client = 'client'";
                                        nom_connecte.Parameters.AddWithValue("@ID_Client", id_connection_client);

                                        MySqlDataReader reader_nom_connecte = nom_connecte.ExecuteReader();

                                        string nom_Affiche = "";
                                        string prenom_Affiche = "";
                                        if (reader_nom_connecte.Read())
                                        {
                                            nom_Affiche = reader_nom_connecte.GetString(0);
                                            prenom_Affiche = reader_nom_connecte.GetString(1);
                                        }
                                        reader_nom_connecte.Close();

                                        ConsoleKeyInfo cki_client;
                                        bool continuer_client = true;

                                        while (continuer_client)
                                        {
                                            Console.Clear();
                                            Console.WriteLine($"{prenom_Affiche} {nom_Affiche}");
                                            Console.WriteLine();
                                            Console.WriteLine("1: Passer commande");
                                            Console.WriteLine("2: Annuler une commande");
                                            Console.WriteLine("3: Voir vos commandes");
                                            Console.WriteLine("4: Evaluer un cuisinier");
                                            Console.WriteLine("5: Voir les notes et avis des cuisinier");

                                            Console.WriteLine("Echap: Retour au menu principal");
                                            cki_client = Console.ReadKey(true);

                                            switch (cki_client.Key)
                                            {
                                                case ConsoleKey.D1:
                                                case ConsoleKey.NumPad1:
                                                    Console.Clear();
                                                    Console.WriteLine("Vous allez passer commande");
                                                    Console.WriteLine();
                                                    Console.WriteLine("Etes-vous végétarien? (oui/non)");
                                                    string vegetarien = (Console.ReadLine()).ToLower();
                                                    string vege = "";
                                                    if (vegetarien == "oui")
                                                    {
                                                        vege = " WHERE Regime='vegetarien'";
                                                    }
                                                    Console.WriteLine();

                                                    Console.WriteLine("Quel est votre prix maximum?");
                                                    float prix_maximum = Convert.ToInt32(Console.ReadLine());
                                                    string prix = "";
                                                    if (vege == "")
                                                    {
                                                        prix = $" WHERE Prix_plat<={prix_maximum}";
                                                    }
                                                    else
                                                    {
                                                        prix = $" AND Prix_plat<={prix_maximum}";
                                                    }
                                                    Console.WriteLine();

                                                    Console.WriteLine("Voulez-vous une cuisine particulière (française, italienne...) (oui/non)");
                                                    string origine = (Console.ReadLine()).ToLower();
                                                    string origine_plat = "";
                                                    if (origine == "oui")
                                                    {
                                                        Console.WriteLine("Quelle type de cuisine ?");
                                                        origine = (Console.ReadLine()).ToLower();
                                                        if (vege == "" || prix == "")
                                                        {
                                                            origine_plat = $" WHERE Nature={origine}";
                                                        }
                                                        else
                                                        {
                                                            origine_plat = $" AND Nature={origine}";
                                                        }
                                                    }
                                                    Console.WriteLine();

                                                    string commande_totale = "SELECT* FROM Plat" + vege + prix + origine_plat + " AND Date_Peremption > CURDATE()";
                                                    Console.WriteLine();

                                                    Console.WriteLine("Recherce de plat en cours");
                                                    Thread.Sleep(2);
                                                    Console.Clear();
                                                    MySqlCommand commander_plat = connection.CreateCommand();
                                                    commander_plat.CommandText = commande_totale;


                                                    MySqlDataReader reader_commande_plat;
                                                    reader_commande_plat = commander_plat.ExecuteReader();

                                                    Console.WriteLine("Voici la liste des plats qui peuvent vous convenir: ");
                                                    Dictionary<int, string> Liste_plat = new Dictionary<int, string>();
                                                    int numero_plat_comande = 1;
                                                    while (reader_commande_plat.Read())
                                                    {
                                                        string currentRowAsString = "";
                                                        for (int i = 0; i < reader_commande_plat.FieldCount; i++)
                                                        {
                                                            string valueAsString = reader_commande_plat.GetValue(i).ToString();
                                                            currentRowAsString += valueAsString + ",  ";
                                                        }
                                                        Liste_plat.Add(numero_plat_comande, currentRowAsString);
                                                        Console.WriteLine(numero_plat_comande + " : " + currentRowAsString);
                                                        numero_plat_comande++;
                                                    }
                                                    Console.WriteLine("Entrez le numéro du plat que vous souhaitez commander");
                                                    reader_commande_plat.Close();


                                                int numero_commande_passee = Convert.ToInt32(Console.ReadLine());
                                                    if (Liste_plat.TryGetValue(numero_commande_passee, out string valeur))
                                                    {
                                                        Console.WriteLine("Vous avez choisi de commander le plat suivant:");
                                                        Console.WriteLine(valeur);

                                                        string[] details_Plat = valeur.Split(", ");
                                                        string Nom_plat_commande = details_Plat[0];

                                                        MySqlCommand Recuperer_adresse = connection.CreateCommand();
                                                        Recuperer_adresse.CommandText = "SELECT* FROM Utilisateur WHERE ID_Client='" + id_connection_client + "'";

                                                        MySqlDataReader reader_Recuperer_adresse;
                                                        reader_Recuperer_adresse = Recuperer_adresse.ExecuteReader();

                                                        string donnes_client = "";
                                                        while (reader_Recuperer_adresse.Read())
                                                        {
                                                            for (int i = 0; i < reader_Recuperer_adresse.FieldCount; i++) 
                                                            {
                                                                string valueAsString = reader_Recuperer_adresse.GetValue(i).ToString();
                                                                donnes_client += valueAsString + ", ";
                                                            }
                                                        }
                                                        string[] detail_client = donnes_client.Split(", ");
                                                        string adresse_client_connecte = $"{detail_client[4]} {detail_client[3]} {detail_client[5]} {detail_client[6]}";
                                                        reader_Recuperer_adresse.Close();

                                                        decimal prixCommande = decimal.Parse(details_Plat[3]);
                                                        int quantite = 1;
                                                        DateTime dateLivraison = DateTime.Today;
                                                        string adresseLivraison = adresse_client_connecte;

                                                        MySqlCommand ajouter_Commande = connection.CreateCommand();
                                                        ajouter_Commande.CommandText = "INSERT INTO Commande (Prix_Commande, Quantite, Date_Commande, Date_Livraison, Status) VALUES (@Prix_Commande, @Quantite, NOW(), @Date_Livraison, 'En attente')";
                                                        ajouter_Commande.Parameters.AddWithValue("@Prix_Commande", prixCommande);
                                                        ajouter_Commande.Parameters.AddWithValue("@Quantite", quantite);
                                                        ajouter_Commande.Parameters.AddWithValue("@Date_Livraison", dateLivraison);

                                                        MySqlDataReader reader_inserer_commande;
                                                        reader_inserer_commande = ajouter_Commande.ExecuteReader();
                                                        reader_inserer_commande.Close();

                                                        long numeroCommande;
                                                        MySqlCommand getID = new MySqlCommand("SELECT LAST_INSERT_ID();", connection);
                                                        numeroCommande = Convert.ToInt64(getID.ExecuteScalar());
                                                        MySqlDataReader reader_getID;
                                                        reader_getID = getID.ExecuteReader();
                                                        reader_getID.Close();

                                                        MySqlCommand ajouter_Passer = connection.CreateCommand();
                                                        ajouter_Passer.CommandText = "INSERT INTO Passer (Numero_Commande, Adresse_livraison, ID_Client) VALUES (@Numero_Commande, @Adresse_Livraison, @ID_Client)";
                                                        ajouter_Passer.Parameters.AddWithValue("@Numero_Commande", numeroCommande);
                                                        ajouter_Passer.Parameters.AddWithValue("@Adresse_Livraison", adresseLivraison);
                                                        ajouter_Passer.Parameters.AddWithValue("@ID_Client", id_connection_client);

                                                        MySqlDataReader reader_ajouter_Passer;
                                                        reader_ajouter_Passer = ajouter_Passer.ExecuteReader();
                                                        reader_ajouter_Passer.Close();

                                                        MySqlCommand ajouter_Contient = connection.CreateCommand();
                                                        ajouter_Contient.CommandText = "INSERT INTO Contient (Numero_Commande, Nom_Plat) VALUES (@Numero_Commande, @Nom_Plat)";
                                                        ajouter_Contient.Parameters.AddWithValue("@Numero_Commande", numeroCommande);
                                                        ajouter_Contient.Parameters.AddWithValue("@Nom_Plat", Nom_plat_commande);

                                                        MySqlDataReader reader_ajouter_Contient;
                                                        reader_ajouter_Contient = ajouter_Contient.ExecuteReader();
                                                        reader_ajouter_Contient.Close();
                                                        
                                                        int idCommande;
                                                        MySqlCommand recupCommande = connection.CreateCommand();
                                                        recupCommande.CommandText = "SELECT LAST_INSERT_ID()";
                                                        idCommande = Convert.ToInt32(recupCommande.ExecuteScalar());

                                                        Console.WriteLine($"Votre commande n°{idCommande} a bien été enregistrée.");

                                                        int idCuisinier = -1;
                                                        using (MySqlCommand recupCuisinier = connection.CreateCommand())
                                                        {
                                                            recupCuisinier.CommandText = @"
                                                                                                    SELECT U.ID_Client 
                                                                                                    FROM Utilisateur U
                                                                                                    JOIN Cuisinier C ON U.ID_Client = C.ID_Client
                                                                                                    JOIN Cuisine Cu ON C.ID_Client = Cu.ID_Client
                                                                                                    WHERE TRIM(LOWER(Cu.Nom_Plat)) = TRIM(LOWER(@Nom_Plat))
                                                                                                    LIMIT 1";
                                                            recupCuisinier.Parameters.AddWithValue("@Nom_Plat", Nom_plat_commande);

                                                            using (MySqlDataReader readerCuisinier = recupCuisinier.ExecuteReader())
                                                            {
                                                                if (readerCuisinier.HasRows)
                                                                {
                                                                    while (readerCuisinier.Read())
                                                                    {
                                                                        idCuisinier = readerCuisinier.GetInt32(0);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        if (idCuisinier == -1)
                                                        {
                                                            Console.WriteLine("Aucun cuisinier trouvé pour ce plat.");
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine($"Le cuisinier avec ID {idCuisinier} va préparer votre plat.");
                                                        }

                                                        MySqlCommand Recuperer_adresse_expedition = connection.CreateCommand();
                                                        Recuperer_adresse_expedition.CommandText = "SELECT* FROM Utilisateur WHERE ID_Client='" + idCuisinier + "'";

                                                        MySqlDataReader reader_Recuperer_adresse_expedition;
                                                        reader_Recuperer_adresse_expedition = Recuperer_adresse_expedition.ExecuteReader();

                                                        string donnes_cuisinier = "";
                                                        while (reader_Recuperer_adresse_expedition.Read())
                                                        {
                                                            for (int i = 0; i < reader_Recuperer_adresse_expedition.FieldCount; i++)
                                                            {
                                                                string valueAsString = reader_Recuperer_adresse_expedition.GetValue(i).ToString();
                                                                donnes_cuisinier += valueAsString + ", ";
                                                            }
                                                        }
                                                        string[] detail_cuisinier = donnes_cuisinier.Split(", ");
                                                        string adresse_expedition = $"{detail_cuisinier[4]} {detail_cuisinier[3]} {detail_cuisinier[5]} {detail_cuisinier[6]}";
                                                        reader_Recuperer_adresse_expedition.Close();

                                                        MySqlCommand ajouter_Preparer = connection.CreateCommand();
                                                        ajouter_Preparer.CommandText = @"
                                                                                            INSERT IGNORE INTO Preparer (Numero_Commande, Adresse_expedition, ID_Client)
                                                                                            VALUES (LAST_INSERT_ID(), @adresseExpedition, @idCuisinier);
                                                                                        ";

                                                        ajouter_Preparer.Parameters.AddWithValue("@adresseExpedition", adresse_expedition);
                                                        ajouter_Preparer.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                                                        ajouter_Preparer.ExecuteNonQuery();
                                                        Console.WriteLine("Commande passée avec succès");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Numéro de commande introuvable");
                                                    }
                                                    Pause();
                                                    break;

                                                case ConsoleKey.D2:
                                                case ConsoleKey.NumPad2:
                                                    Console.Clear();
                                                    Afficher_Commande_client(id_connection_client, connection);

                                                    Console.WriteLine("Entrez le numéro de la commande que vous souhaitez supprimer");

                                                    int numero_commande_a_supprimer = Convert.ToInt32(Console.ReadLine());
                                                    Console.WriteLine("Vous avez choisi de supprimer la commande " + numero_commande_a_supprimer);

                                                    MySqlCommand Supprimer_commande = connection.CreateCommand();
                                                    Supprimer_commande.CommandText = @"DELETE FROM Passer 
                                                                                                       WHERE Numero_Commande = @Numero_Commande AND ID_Client = @ID_Client;
                                                                                                       DELETE FROM Contient WHERE Numero_Commande = @Numero_Commande;
                                                                                                       DELETE FROM Preparer WHERE Numero_Commande = @Numero_Commande;
                                                                                                       DELETE FROM Commande WHERE Numero_Commande = @Numero_Commande;";

                                                    Supprimer_commande.Parameters.AddWithValue("@ID_Client", id_connection_client);
                                                    Supprimer_commande.Parameters.AddWithValue("@Numero_Commande", numero_commande_a_supprimer);

                                                    MySqlDataReader reader_Supprimer_commande_utilisateur = Supprimer_commande.ExecuteReader();
                                                    reader_Supprimer_commande_utilisateur.Close();
                                                    Console.WriteLine($"La commande {numero_commande_a_supprimer} a été supprimée avec succès");

                                                    Pause();
                                                    break;

                                                case ConsoleKey.D3:
                                                case ConsoleKey.NumPad3:
                                                    Console.Clear();
                                                    Afficher_Commande_client(id_connection_client, connection);
                                                    Pause();
                                                    break;

                                                case ConsoleKey.D4:
                                                case ConsoleKey.NumPad4:
                                                    Console.Clear();
                                                    string requete_afficher_cuisinier = @"
                                                                    SELECT DISTINCT 
                                                                        cuis.ID_Client AS IDCuisinier, 
                                                                        util.Prenom, 
                                                                        util.Nom,
                                                                        eval.Note
                                                                    FROM Passer passer
                                                                    INNER JOIN Contient contient ON passer.Numero_Commande = contient.Numero_Commande
                                                                    INNER JOIN Cuisine cuisine ON contient.Nom_Plat = cuisine.Nom_Plat
                                                                    INNER JOIN Cuisinier cuis ON cuisine.ID_Client = cuis.ID_Client
                                                                    INNER JOIN Utilisateur util ON cuis.ID_Client = util.ID_Client
                                                                    LEFT JOIN Evaluer eval ON eval.ID_Client = cuis.ID_Client AND eval.ID_Client_1 = @ClientID
                                                                    WHERE passer.ID_Client = @ClientID";

                                                    MySqlCommand affichage_note = new MySqlCommand(requete_afficher_cuisinier, connection);
                                                    affichage_note.Parameters.AddWithValue("@ClientID", id_connection_client);

                                                    Console.WriteLine("Liste de vos cuisiniers :\n");

                                                    List<int> cuisiniers = new List<int>();
                                                    Dictionary<int, double?> cuisiniers_Avec_Note = new Dictionary<int, double?>();

                                                    MySqlDataReader reader_affichage_note = affichage_note.ExecuteReader();

                                                    while (reader_affichage_note.Read())
                                                    {
                                                        int idCuisinier = reader_affichage_note.GetInt32("IDCuisinier");
                                                        string prenom = reader_affichage_note.GetString("Prenom");
                                                        string nom = reader_affichage_note.GetString("Nom");
                                                        double? note = reader_affichage_note.IsDBNull(reader_affichage_note.GetOrdinal("Note")) ? (double?)null : reader_affichage_note.GetDouble("Note");

                                                        cuisiniers.Add(idCuisinier);
                                                        cuisiniers_Avec_Note[idCuisinier] = note;

                                                        if (note.HasValue)
                                                        {
                                                            Console.WriteLine($"{idCuisinier.ToString().PadRight(7)} | {prenom.PadRight(10)} {nom.PadRight(10)} | Déjà évalué : {note.Value}/5");
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine($"{idCuisinier.ToString().PadRight(7)} | {prenom.PadRight(10)} {nom.PadRight(10)} | Pas encore évalué ");
                                                        }
                                                    }
                                                    reader_affichage_note.Close();

                                                    if (cuisiniers.Count == 0)
                                                    {
                                                        Console.WriteLine("Vous n'avez passé aucune commande.");
                                                        return;
                                                    }

                                                    Console.Write("\nEntrez l'identifiant du cuisinier que vous souhaitez évaluer (ou réévaluer) : ");
                                                    int idCuisinierAEvaluer = int.Parse(Console.ReadLine());

                                                    if (!cuisiniers.Contains(idCuisinierAEvaluer))
                                                    {
                                                        Console.WriteLine("Cuisinier invalide !");
                                                        return;
                                                    }

                                                    if (cuisiniers_Avec_Note[idCuisinierAEvaluer] != null)
                                                    {
                                                        Console.WriteLine("Vous avez déjà évalué ce cuisinier. Si vous continuez, votre évaluation sera remplacée.");
                                                    }

                                                    Console.Write("Entrez une note sur 5 (ex: 4.5) : ");
                                                    double noteNouvelle = double.Parse(Console.ReadLine());

                                                    Console.Write("Laissez un commentaire (optionnel) : ");
                                                    string commentaire = Console.ReadLine();
                                                    
                                                    string MAJ_Evaluation = @"
                                                                                        INSERT INTO Evaluer (ID_Client, ID_Client_1, Note, Commente)
                                                                                        VALUES (@IDCuisinier, @IDClient, @Note, @Commentaire)
                                                                                        ON DUPLICATE KEY UPDATE
                                                                                            Note = @Note,
                                                                                            Commente = @Commentaire";
                                                    MySqlCommand MAJ_Cmd = new MySqlCommand(MAJ_Evaluation, connection);
                                                    MAJ_Cmd.Parameters.AddWithValue("@IDCuisinier", idCuisinierAEvaluer);
                                                    MAJ_Cmd.Parameters.AddWithValue("@IDClient", id_connection_client);
                                                    MAJ_Cmd.Parameters.AddWithValue("@Note", noteNouvelle);
                                                    MAJ_Cmd.Parameters.AddWithValue("@Commentaire", commentaire);

                                                    MAJ_Cmd.ExecuteNonQuery();
                                                    Console.WriteLine("Évaluation enregistrée ou mise à jour avec succès");

                                                    string Afficher_note = "SELECT Note FROM Evaluer WHERE ID_Client = @IDCuisinier";
                                                    MySqlCommand selectCmd = new MySqlCommand(Afficher_note, connection);
                                                    selectCmd.Parameters.AddWithValue("@IDCuisinier", idCuisinierAEvaluer);

                                                    double total_Notes = 0;
                                                    int nombreNotes = 0;

                                                    MySqlDataReader reader_note = selectCmd.ExecuteReader();
                                                    while (reader_note.Read())
                                                    {
                                                        total_Notes += reader_note.GetDouble("Note");
                                                        nombreNotes++;
                                                    }
                                                    reader_note.Close();

                                                    if (nombreNotes > 0)
                                                    {
                                                        double moyenne = total_Notes / nombreNotes;

                                                        string MAJ_Moyenne = @"
                                                                                        UPDATE Cuisinier
                                                                                        SET Note_moyenne_cuisinier = @Moyenne
                                                                                        WHERE ID_Client = @IDCuisinier";
                                                        MySqlCommand update_note = new MySqlCommand(MAJ_Moyenne, connection);
                                                        update_note.Parameters.AddWithValue("@Moyenne", moyenne);
                                                        update_note.Parameters.AddWithValue("@IDCuisinier", idCuisinierAEvaluer);
                                                        MySqlDataReader reader_update_note = update_note.ExecuteReader();
                                                        Console.WriteLine("Merci pour votre avis !");
                                                        reader_update_note.Close();
                                                    }
                                                    Pause();
                                                    break;

                                                case ConsoleKey.D5:
                                                case ConsoleKey.NumPad5:
                                                    Console.Clear();
                                                    Console.WriteLine("Voici la liste des cuisiniers et leur note moyenne!");

                                                    MySqlCommand Afficher_note_cuisinier = connection.CreateCommand();
                                                    Afficher_note_cuisinier.CommandText = @"
                                                                                            SELECT u.ID_Client, u.Nom, u.Prenom, c.Note_moyenne_cuisinier
                                                                                            FROM Cuisinier c
                                                                                            JOIN Utilisateur u ON c.ID_Client = u.ID_Client;
                                                                                        ";                            

                                                    MySqlDataReader reader_Afficher_note_cuisinier = Afficher_note_cuisinier.ExecuteReader();
                                                    while (reader_Afficher_note_cuisinier.Read()) { 
                                                        int id = reader_Afficher_note_cuisinier.GetInt32(0);
                                                        string identite_personne = reader_Afficher_note_cuisinier.GetString(2)+" "+ reader_Afficher_note_cuisinier.GetString(1);
                                                        decimal moyenne = reader_Afficher_note_cuisinier.GetDecimal(3);

                                                        Console.WriteLine($"{id.ToString().PadRight(7)} | {identite_personne.ToString().PadRight(40)} | {moyenne.ToString().PadRight(5)}");
                                                        
                                                    }
                                                    reader_Afficher_note_cuisinier.Close();
                                                    int identifiant_cuisto_com = 1;
                                                    while (identifiant_cuisto_com != 0) {
                                                        identifiant_cuisto_com = SaisieEntier("Saisissez l'identifiant du cuisinier dont vous souhaitez connaitre l'avis (entrez 0 si vous ne souhaitez pas afficher de commentaire)");
                                                        MySqlCommand Afficher_commentaire_cuisinier = connection.CreateCommand();
                                                        Afficher_commentaire_cuisinier.CommandText = @"
                                                                                                       SELECT 
                                                                                                            uc.Nom AS Nom_Client,
                                                                                                            uc.Prenom AS Prenom_Client,
                                                                                                            e.Note,
                                                                                                            e.Commente
                                                                                                       FROM Evaluer e
                                                                                                       JOIN Utilisateur uc ON e.ID_Client_1 = uc.ID_Client
                                                                                                       WHERE e.ID_Client = @ID_Cuisinier;";

                                                        Afficher_commentaire_cuisinier.Parameters.AddWithValue("@ID_Cuisinier", identifiant_cuisto_com);
                                                        MySqlDataReader reader_Afficher_commentaire_cuisinier = Afficher_commentaire_cuisinier.ExecuteReader();
                                                        bool a_Commentaire = false;
                                                        while (reader_Afficher_commentaire_cuisinier.Read())
                                                        {
                                                            a_Commentaire = true;
                                                            string identite = reader_Afficher_commentaire_cuisinier.GetString(1) + " " + reader_Afficher_commentaire_cuisinier.GetString(0);
                                                            decimal note = reader_Afficher_commentaire_cuisinier.GetDecimal(2);
                                                            string comm = reader_Afficher_commentaire_cuisinier.GetString(3);

                                                            Console.WriteLine($"{identite} : {note.ToString()}\n{comm}\n");
                                                            Console.WriteLine();
                                                        }

                                                        if (!a_Commentaire)
                                                        {
                                                            Console.WriteLine("Ce cuisinier n'a reçu aucun commentaire.");
                                                        }
                                                        reader_Afficher_commentaire_cuisinier.Close();
                                                    }                                 
                                                    Pause();
                                                    break;

                                                case ConsoleKey.Escape:
                                                    Console.Clear();
                                                    Console.WriteLine("Retour au menu principal... Appuyez sur une touche");
                                                    continuer_client = false;
                                                    break;


                                                default:
                                                    Console.Clear();
                                                    Console.WriteLine("Option invalide, veuillez choisir une option valide.");
                                                    Pause();
                                                    break;

                                            }
                                        }


                                    }
                                    else
                                    {
                                        Console.WriteLine("Mot de passe incorrect.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Aucun utilisateur trouvé.");
                                }
                                reader_mdp_requete.Close();
                            }
                            Pause();
                            break;

                        case ConsoleKey.D3:
                        case ConsoleKey.NumPad3:
                            Console.Clear();
                            Console.WriteLine("Connexion Cuisinier");

                            Console.WriteLine("Quel est votre identifiant?");
                            int id_connection_cuisinier = Convert.ToInt32(Console.ReadLine());

                            Console.WriteLine("Quel est votre mot de passe?");
                            string mdp_connexion_cuisinier = Console.ReadLine();
                            
                            MySqlCommand test_id = connection.CreateCommand();
                            test_id.CommandText = "SELECT ID_Client FROM Utilisateur WHERE ID_Client = @ID_Client";
                            test_id.Parameters.AddWithValue("@ID_Client", id_connection_cuisinier);

                            MySqlDataReader reader_id = test_id.ExecuteReader();

                            if (!reader_id.Read())
                            {
                                Console.WriteLine("Échec de connexion : Identifiant introuvable.");
                                reader_id.Close();
                            }
                            else
                            {
                                reader_id.Close();
                            
                                MySqlCommand test_mdp = connection.CreateCommand();
                                test_mdp.CommandText = "SELECT Mot_De_Passe FROM Utilisateur WHERE ID_Client = @ID_Client AND Type_client = 'cuisinier'";
                                test_mdp.Parameters.AddWithValue("@ID_Client", id_connection_cuisinier);

                                MySqlDataReader reader_mdp = test_mdp.ExecuteReader();

                                if (reader_mdp.Read())
                                {
                                    string motDePasseStocke = reader_mdp.GetString(0);
                                    if (motDePasseStocke == mdp_connexion_cuisinier)
                                    {
                                        reader_mdp.Close();
                                        Console.Clear();
                                        Console.WriteLine("Connexion réussie !");
                                        Thread.Sleep(2000);
                                        Console.Clear();
                                        
                                        MySqlCommand recup_nom_prenom = connection.CreateCommand();
                                        recup_nom_prenom.CommandText = "SELECT Nom, Prenom FROM Utilisateur WHERE ID_Client = @ID_Client AND Type_client = 'cuisinier'";
                                        recup_nom_prenom.Parameters.AddWithValue("@ID_Client", id_connection_cuisinier);

                                        MySqlDataReader reader_nom_prenom = recup_nom_prenom.ExecuteReader();

                                        string nom_Affiche = "", prenom_Affiche = "";
                                        if (reader_nom_prenom.Read())
                                        {
                                            nom_Affiche = reader_nom_prenom.GetString(0);
                                            prenom_Affiche = reader_nom_prenom.GetString(1);
                                        }
                                        reader_nom_prenom.Close();

                                        ConsoleKeyInfo cki_cuisinier;
                                        bool continuer_cuisinier = true;

                                        while (continuer_cuisinier)
                                        {
                                            Console.Clear();
                                            Console.WriteLine($"{prenom_Affiche} {nom_Affiche}");
                                            Console.WriteLine();
                                            Console.WriteLine("1: Voir les commandes à préparer");
                                            Console.WriteLine("2: Mettre à jour le statut d'une commande");
                                            Console.WriteLine("3: Ajouter un plat à votre menu");
                                            Console.WriteLine("4: Voir la liste de vos plats");
                                            Console.WriteLine("5: Modifier vos informations");
                                            Console.WriteLine("Echap: Retour au menu principal");

                                            cki_cuisinier = Console.ReadKey(true);
                                            switch (cki_cuisinier.Key)
                                            {
                                                case ConsoleKey.D1:
                                                case ConsoleKey.NumPad1:
                                                    Console.Clear();
                                                    Afficher_Commande_Cuisinier(id_connection_cuisinier, connection, infos_noeuds, lienscsv, Dico_adjacence_reel);
                                                    Pause();
                                                    break;

                                                case ConsoleKey.D2:
                                                case ConsoleKey.NumPad2:
                                                    Console.Clear();
                                                    Afficher_Commande_Cuisinier(id_connection_cuisinier, connection, infos_noeuds, lienscsv, Dico_adjacence_reel);

                                                    Console.WriteLine("Entrez le numéro de la commande à mettre à jour :");
                                                    int numCommande = Convert.ToInt32(Console.ReadLine());

                                                    Console.WriteLine("Nouveau statut (En préparation, Expédiée, Livrée) :");
                                                    string nouveauStatut = Console.ReadLine();

                                                    MySqlCommand update_commande = connection.CreateCommand();
                                                    update_commande.CommandText = "UPDATE Commande SET Status = @Status WHERE Numero_Commande = @Numero_Commande";
                                                    update_commande.Parameters.AddWithValue("@Status", nouveauStatut);
                                                    update_commande.Parameters.AddWithValue("@Numero_Commande", numCommande);

                                                    int rowsAffected = update_commande.ExecuteNonQuery();


                                                    if (rowsAffected > 0)
                                                    {
                                                        Console.WriteLine("Statut mis à jour avec succès !");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Erreur lors de la mise à jour.");
                                                    }
                                                    break;
                                                    Pause();

                                                case ConsoleKey.D3:
                                                case ConsoleKey.NumPad3:
                                                    Console.Clear();
                                                    Console.WriteLine("Ajout d'un plat :");
                                                    Console.WriteLine("Nom du plat ?");
                                                    string nomPlat = Console.ReadLine();

                                                    Console.WriteLine("Prix du plat ?");
                                                    decimal prixPlat = Convert.ToDecimal(Console.ReadLine());

                                                    Console.WriteLine("Type de cuisine (Française, Italienne...) ?");
                                                    string typeCuisine = Console.ReadLine();

                                                    Console.WriteLine("Régime alimentaire (Végétarien, Sans Gluten, Aucun) ?");
                                                    string regime = Console.ReadLine();

                                                    Console.WriteLine("Est-ce une entrée un plat ou un dessert ?");
                                                    string type_repas = Console.ReadLine();

                                                    Console.WriteLine("Pour combien de personne ?");
                                                    int nb_personne = Convert.ToInt32(Console.ReadLine());

                                                    MySqlCommand checkPlat = connection.CreateCommand();
                                                    checkPlat.CommandText = "SELECT COUNT(*) FROM Plat WHERE Nom_Plat = @Nom_Plat";
                                                    checkPlat.Parameters.AddWithValue("@Nom_Plat", nomPlat);

                                                    MySqlDataReader reader_checkPlat = checkPlat.ExecuteReader();
                                                    int count = 0;

                                                    if (reader_checkPlat.Read())
                                                    {
                                                        count = reader_checkPlat.GetInt32(0);
                                                    }
                                                    reader_checkPlat.Close();
                                                    if (count > 0)
                                                    {
                                                        while (count > 0)
                                                        {
                                                            Console.WriteLine($"Le plat '{nomPlat}'est déjà utilisé. Nommez le autrement (avec votre nom par exemple).");
                                                            nomPlat = Console.ReadLine();
                                                            if (nomPlat.Contains(' '))
                                                            {
                                                                nomPlat = nomPlat.Replace(' ', '_');
                                                            }
                                                            MySqlCommand checkPlat_2 = connection.CreateCommand();
                                                            checkPlat_2.CommandText = "SELECT COUNT(*) FROM Plat WHERE Nom_Plat = @Nom_Plat";
                                                            checkPlat_2.Parameters.AddWithValue("@Nom_Plat", nomPlat);

                                                            MySqlDataReader reader_checkPlat_2 = checkPlat_2.ExecuteReader();
                                                            if (reader_checkPlat_2.Read())
                                                            {
                                                                count = reader_checkPlat_2.GetInt32(0);
                                                            }
                                                            reader_checkPlat_2.Close();
                                                        }

                                                    }

                                                    if (count == 0)
                                                    {
                                                        MySqlCommand ajouter_Plat = connection.CreateCommand();
                                                        ajouter_Plat.CommandText = @"
                                                                INSERT INTO Plat (Nom_Plat, Date_Fabrication, Date_Peremption, Prix_plat, Regime, Origine, Type_plat, Nombre_personne, Image)
                                                                VALUES (@Nom_Plat, @Date_Fabrication, @Date_Peremption, @Prix_plat, @Regime, @Origine, @Type_plat, @Nombre_personne, @Image)";

                                                        ajouter_Plat.Parameters.AddWithValue("@Nom_Plat", nomPlat);
                                                        ajouter_Plat.Parameters.AddWithValue("@Date_Fabrication", DateTime.Today);
                                                        ajouter_Plat.Parameters.AddWithValue("@Date_Peremption", DateTime.Now.AddDays(5));
                                                        ajouter_Plat.Parameters.AddWithValue("@Prix_plat", prixPlat);
                                                        ajouter_Plat.Parameters.AddWithValue("@Regime", regime);
                                                        ajouter_Plat.Parameters.AddWithValue("@Origine", typeCuisine);
                                                        ajouter_Plat.Parameters.AddWithValue("@Type_plat", type_repas);
                                                        ajouter_Plat.Parameters.AddWithValue("@Nombre_personne", nb_personne);
                                                        ajouter_Plat.Parameters.AddWithValue("@Image", 1);
                                                        ajouter_Plat.ExecuteNonQuery();
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Le plat existe déjà.");
                                                    }
                                                    
                                                    MySqlCommand assigner_Cuisinier = connection.CreateCommand();
                                                    assigner_Cuisinier.CommandText = "INSERT IGNORE INTO Cuisine (ID_Client, Nom_Plat) VALUES (@ID_Client, @Nom_Plat)";
                                                    assigner_Cuisinier.Parameters.AddWithValue("@ID_Client", id_connection_cuisinier);
                                                    assigner_Cuisinier.Parameters.AddWithValue("@Nom_Plat", nomPlat);
                                                    assigner_Cuisinier.ExecuteNonQuery();
                                                    Console.WriteLine("Plat ajouté avec succès !");
                                                    Pause();
                                                    break;

                                                case ConsoleKey.D4:
                                                case ConsoleKey.NumPad4:
                                                    Console.Clear();
                                                    Console.WriteLine("Liste de vos plats :\n");                                                

                                                    MySqlCommand liste_plats = connection.CreateCommand();
                                                    liste_plats.CommandText = @"
                                                                            SELECT p.Nom_Plat, p.Date_Fabrication, p.Date_Peremption, p.Prix_plat, p.Type_plat
                                                                            FROM Plat p
                                                                            JOIN Cuisine c ON p.Nom_Plat = c.Nom_Plat
                                                                            WHERE c.ID_Client = @idClient;
                                                                            ";
                                                    liste_plats.Parameters.AddWithValue("@idClient", id_connection_cuisinier);

                                                    MySqlDataReader reader_liste_plats = liste_plats.ExecuteReader();

                                                    while (reader_liste_plats.Read())
                                                    {

                                                        string nom = reader_liste_plats.GetString(0);
                                                        string date_fabrication = reader_liste_plats.GetDateTime(1).ToString("dd/MM/yyyy");
                                                        string date_peremption = reader_liste_plats.GetDateTime(2).ToString("dd/MM/yyyy");
                                                        decimal prixDecimal = reader_liste_plats.GetDecimal(3);
                                                        string type_plat = reader_liste_plats.GetString(4);

                                                        Console.WriteLine($"{type_plat} : {nom} à {prixDecimal} euro, fait le {date_fabrication}, bon jusq'au {date_peremption}");
                                                        
                                                    }
                                                    reader_liste_plats.Close();
                                                    Pause();
                                                    break;

                                                case ConsoleKey.D5:
                                                case ConsoleKey.NumPad5:
                                                    Console.Clear();
                                                    Console.WriteLine("Modification de votre profil :");
                                                    Console.WriteLine("Nouveau prénom ?");
                                                    string nouveau_Prenom = Console.ReadLine();

                                                    Console.WriteLine("Nouveau numéro de téléphone ?");
                                                    int nouveauTel = Convert.ToInt32(Console.ReadLine());

                                                    MySqlCommand update_profil = connection.CreateCommand();
                                                    update_profil.CommandText = "UPDATE Utilisateur SET Prenom = @Prenom, Telephone = @Telephone WHERE ID_Client = @ID_Client";
                                                    update_profil.Parameters.AddWithValue("@Prenom", nouveau_Prenom);
                                                    update_profil.Parameters.AddWithValue("@Telephone", nouveauTel);
                                                    update_profil.Parameters.AddWithValue("@ID_Client", id_connection_cuisinier);

                                                    update_profil.ExecuteNonQuery();
                                                    Console.WriteLine("Profil mis à jour avec succès !");
                                                    break;

                                                case ConsoleKey.Escape:
                                                    Console.Clear();
                                                    Console.WriteLine("Retour au menu principal... Appuyez sur une touche");
                                                    continuer_cuisinier = false;
                                                    break;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Mot de passe incorrect.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Aucun cuisinier trouvé.");
                                }
                                reader_mdp.Close();
                            }
                            Pause();
                            break;

                        case ConsoleKey.D4:
                        case ConsoleKey.NumPad4:
                            Console.Clear();
                            Console.WriteLine("Génération du graphe");
                            string dotFile = "graph.dot";
                            GenerateDotFile(Dico_adjacence_affichage, dotFile);
                            GenererDotAvecCoordonnees(Dico_adjacence_affichage, infos_noeuds, "dotFile.dot");
                            Console.WriteLine("Veuillez patienter, génération du fichier PNG...");
                            GenererImageAvecDot("dotFile.dot", "ImageGrapheDot.png");
                            Console.WriteLine("\nFichier généré avec succès, cliquer pour quitter");
                            Process.Start(new ProcessStartInfo("ImageGrapheDot.png") { UseShellExecute = true });
                            Pause();
                            break;

                        case ConsoleKey.D5:
                        case ConsoleKey.NumPad5:
                            Console.Clear();
                            Console.WriteLine("Coloration du graphe via l'algorithme de WelshPowell");
                            Dictionary<string, int> Dico_adj_color = WelshPowellColoration(Dico_adjacence_affichage);
                            Console.WriteLine("génération de l'image du graphe coloré");
                            GenererGraphvizAvecCouleurs(Dico_adjacence_affichage, Dico_adj_color);
                            Console.WriteLine("Graphe coloré généré !");
                            Process.Start(new ProcessStartInfo("graphe.png") { UseShellExecute = true });
                            Pause();
                            break;

                        case ConsoleKey.D6:
                        case ConsoleKey.NumPad6:

                            Console.Clear();
                            Console.WriteLine("Vous avez choisi la recherche de PCC");
                            List<Noeud> noeuds_trajet = new List<Noeud>();
                            int id_depart = -1;
                            while (id_depart == -1)
                            {
                                Console.WriteLine("Entrez le nom de la station de départ");
                                id_depart = IdDepuisLibelle(Console.ReadLine(), infos_noeuds);
                            }
                            int id_arrivee = -1;
                            while (id_arrivee == -1)
                            {
                                Console.WriteLine("Entrez le nom de la station de d'arrivée");
                                id_arrivee = IdDepuisLibelle(Console.ReadLine(), infos_noeuds);
                            }
                            Console.WriteLine("\nPCC algo de Djikstra : ");
                            Console.WriteLine($"Chemin de {infos_noeuds[id_depart - 1].Libelle_station} a {infos_noeuds[id_arrivee - 1].Libelle_station}");
                            noeuds_trajet = Pcc_Dijkstra(infos_noeuds[id_depart - 1], infos_noeuds[id_arrivee - 1], Dico_adjacence_reel, lienscsv, infos_noeuds);
                            Console.WriteLine($"\nNombre de stations : {noeuds_trajet.Count()}");
                            AfficherTrajet(noeuds_trajet, lienscsv);

                            Console.WriteLine("\n\n PCC Algo de Bellman-Ford");
                            Console.WriteLine($"Chemin de {infos_noeuds[id_depart - 1].Libelle_station} a {infos_noeuds[id_arrivee - 1].Libelle_station}");
                            noeuds_trajet = Pcc_BellmanFord(infos_noeuds[id_depart - 1], infos_noeuds[id_arrivee - 1], Dico_adjacence_reel, lienscsv, infos_noeuds);
                            Console.WriteLine($"\nNombre de stations : {noeuds_trajet.Count()}");
                            AfficherTrajet(noeuds_trajet, lienscsv);

                            Console.WriteLine("\n\n PCC Algo de Floyd-Warshall");
                            Console.WriteLine($"Chemin de {infos_noeuds[id_depart - 1].Libelle_station} a {infos_noeuds[id_arrivee - 1].Libelle_station}");
                            noeuds_trajet = Pcc_Floyd_Warshall(infos_noeuds[id_depart - 1], infos_noeuds[id_arrivee - 1], Dico_adjacence_reel, lienscsv, infos_noeuds);
                            Console.WriteLine($"\nNombre de stations : {noeuds_trajet.Count()}");
                            AfficherTrajet(noeuds_trajet, lienscsv);
                            Pause();
                            break;

                        
                        case ConsoleKey.D7:
                        case ConsoleKey.NumPad7:
                            Console.Clear();
                            Console.WriteLine("Création du graphe d'interactions client/cuisinier...");
                            Dictionary<string, List<string>> interactions = new Dictionary<string, List<string>>();

                            string requete_affichage = @"
                                                SELECT 
                                                    pa.ID_Client AS IdClient,
                                                    cli.Type_client AS TypeClient,
                                                    cli.Prenom AS PrenomClient,
                                                    cli.Nom AS NomClient,

                                                    pr.ID_Client AS IdCuisinier,
                                                    cui.Type_client AS TypeCuisinier,
                                                    cui.Prenom AS PrenomCuisinier,
                                                    cui.Nom AS NomCuisinier,

                                                    COUNT(*) AS NombreCommandes

                                                FROM Passer pa
                                                JOIN Preparer pr ON pa.Numero_Commande = pr.Numero_Commande
                                                JOIN Utilisateur cli ON pa.ID_Client = cli.ID_Client
                                                JOIN Utilisateur cui ON pr.ID_Client = cui.ID_Client

                                                GROUP BY 
                                                    pa.ID_Client, cli.Type_client, cli.Prenom, cli.Nom,
                                                    pr.ID_Client, cui.Type_client, cui.Prenom, cui.Nom;";

                            Dictionary<(string, string), int> weightedEdges = new Dictionary<(string, string), int>();

                            using (MySqlCommand cmd = new MySqlCommand(requete_affichage, connection))
                            using (MySqlDataReader reader_cmd = cmd.ExecuteReader())
                            {
                                while (reader_cmd.Read())
                                {
                                    string clientLabel = $"{reader_cmd["TypeClient"]}\n{reader_cmd["PrenomClient"]} {reader_cmd["NomClient"]}";
                                    string cuisinierLabel = $"{reader_cmd["TypeCuisinier"]}\n{reader_cmd["PrenomCuisinier"]} {reader_cmd["NomCuisinier"]}";
                                    int nombreCommandes = Convert.ToInt32(reader_cmd["NombreCommandes"]);

                                    var edge = (from: clientLabel, to: cuisinierLabel);
                                    if (String.Compare(clientLabel, cuisinierLabel) > 0)
                                        edge = (from: cuisinierLabel, to: clientLabel);

                                    if (weightedEdges.ContainsKey(edge))
                                        weightedEdges[edge] += nombreCommandes;
                                    else
                                        weightedEdges[edge] = nombreCommandes;
                                }
                            }
                            string emplacement = "interactions_pondere.dot";
                            GenererDotGraphePondere(weightedEdges, emplacement);
                            GenererImageAvecDot(emplacement, "interactions_pondere.png");


                            bool biparti = EstBiparti(interactions);
                            bool planaire = EstPlanaire(interactions);

                            Console.WriteLine("Graphe bipartie? ->" + biparti);
                            Console.WriteLine("Graphe planaire? ->" + planaire);

                            Console.WriteLine("Export des données du graphe en XML...");
                            using (var writer = new StreamWriter("interactions.xml"))
                            {
                                writer.WriteLine("<GrapheInteractions>");

                                foreach (var edge in weightedEdges)
                                {
                                    writer.WriteLine("  <Interaction>");
                                    writer.WriteLine($"    <Noeud1>{edge.Key.Item1}</Noeud1>");
                                    writer.WriteLine($"    <Noeud2>{edge.Key.Item2}</Noeud2>");
                                    writer.WriteLine($"    <Poids>{edge.Value}</Poids>");
                                    writer.WriteLine("  </Interaction>");
                                }

                                writer.WriteLine("  <Proprietes>");
                                writer.WriteLine($"    <EstBiparti>{biparti}</EstBiparti>");
                                writer.WriteLine($"    <EstPlanaire>{planaire}</EstPlanaire>");
                                writer.WriteLine("  </Proprietes>");
                                writer.WriteLine("</GrapheInteractions>");
                            }

                            Console.WriteLine("Fichier XML généré avec succès : interactions.xml");
                            Process.Start(new ProcessStartInfo("interactions_pondere.png") { UseShellExecute = true });
                            Process.Start(new ProcessStartInfo("interactions.xml") { UseShellExecute = true });
                            Pause();
                            break;

                        case ConsoleKey.D8:
                        case ConsoleKey.NumPad8:
                            Console.Clear();
                            Console.WriteLine("Exportation de la base de données en XML et Json");
                            string xmlFilePath = "ApplicationDB.xml";
                            string jsonFilePath = "ApplicationDB.json";
                            ExportDatabaseToSingleXml(connection, xmlFilePath);

                            Console.WriteLine("Export XML global terminé ! Fichier généré : " + xmlFilePath);
                            ExportDatabaseToJson(connection, jsonFilePath);

                            Console.WriteLine("Export XML et JSON terminés !");
                            Process.Start(new ProcessStartInfo("ApplicationDB.xml") { UseShellExecute = true });
                            Process.Start(new ProcessStartInfo("ApplicationDB.json") { UseShellExecute = true });
                            Pause();
                            break;


                        case ConsoleKey.Escape:
                            Console.Clear();
                            Console.WriteLine("Vous avez choisi de quitter. Au revoir !");
                            continuer = false;
                            break;

                        default:
                            Console.Clear();
                            Console.WriteLine("Option invalide, veuillez choisir une option valide.");
                            Pause();
                            break;
                    }
                }

            }
                catch (Exception ex) { Console.WriteLine("Erreur de connexion"); 
            }
            

        }

    /// <summary>
    /// Fonction qui permet d'attendre une action de l'utilisateur avant de revenir au menu de base (lui permet de visualiser le résultat de ses requêtes
    /// </summary>
    static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
        Console.ReadKey(true);
    }

    /// <summary>
    /// Fonction qui importe les noeux du csv dans un tableau (donc les stations par lesquelles passent deux lignes sont représentées par deux noeuds distincts) puis renvoie ce tableau
    /// On créé une liste de noeuds
    /// Ensuite on lit le csv
    /// Puis on importe les données du csv dans la liste sans prendre la première ligne du tableau (qui est le label de chaque colonne)
    /// </summary>
    /// <param name="nom_du_csv">Fichier csv dans lequel sont rangées les stations de métro</param>
    /// <returns>La liste des stations du métro de Paris</returns>
    static Noeud[] Noeud_depuis_csv(string nom_du_csv)
    {
        List<Noeud> NoeudsMetro = new List<Noeud>();
        string[] lignes = File.ReadAllLines(nom_du_csv);

        for (int i = 1; i < lignes.Length; i++)
        {
            string[] tempo = lignes[i].Split(';');
            Noeud noeudtempo = new Noeud(
                Convert.ToInt32(tempo[0]),
                tempo[1],
                tempo[2],
                float.Parse(tempo[3].Replace('.', ',')),
                float.Parse(tempo[4].Replace('.', ',')),
                tempo[5],
                Convert.ToInt32(tempo[6])
                );
            NoeudsMetro.Add(noeudtempo);
        }
        Console.WriteLine($"{NoeudsMetro.Count()} noeuds importés avec succès");
        return NoeudsMetro.ToArray();
    }


    /// <summary>3]
    /// Fonction qui renvoie un tableau avec les liens du csv (donc pour l'instant, les stations à deux lignes étant deux noeuds, on a un graphe non connexe,car les lignes différentes ne se touchent pas)
    /// On créé la liste des noeuds et on lit le csv
    /// On importe ensuite chaque lignes dans la liste (en sautant la ligne des labels)
    /// Tempo[2] est station precedente et tempo[3] station suivante, si on est au bout ou au debut de la ligne, 
    /// on prend un id_station impossible : -1
    /// </summary>
    /// <param name="nom_du_csv">Fichier csv dans lequel sont rangés les liens entre les stations</param>
    /// <returns>La liste des liens entre les stations du métro de Paris</returns>
    static Lien[] Lien_depuis_csv(string nom_du_csv)
    {
        List<Lien> LienMetro = new List<Lien>();
        string[] lignes = File.ReadAllLines(nom_du_csv);

        for (int i = 1; i < lignes.Length; i++)
        {
            string[] tempo = lignes[i].Split(";");
            int id_station = Convert.ToInt32(tempo[0]);
            string nom = tempo[1];
            int id_precedent;

            if (string.IsNullOrEmpty(tempo[2]))
            { id_precedent = -1; }
            else
            { id_precedent = Convert.ToInt32(tempo[2]); }
            int id_suivant;

            if (string.IsNullOrEmpty(tempo[3]))
            { id_suivant = -1; }
            else
            { id_suivant = Convert.ToInt32(tempo[3]); }
            int temps_deux_stations = Convert.ToInt32(tempo[4]);
            int temps_changement = Convert.ToInt32(tempo[5]);

            Lien lientempo = new Lien(id_station, nom, id_precedent, id_suivant, temps_deux_stations, temps_changement);
            LienMetro.Add(lientempo);
        }
        Console.WriteLine($"{LienMetro.Count()} liens importés avec succès");
        return LienMetro.ToArray();
    }

    /// <summary>
    /// Creation d'une liste d'adjacence sans doublons 
    /// Fonction qui créee un dictionaire ou les stations en doublons sont une unique clé,
    /// Dictionnaire destiné à l'affichage uniquement, car ne prend pas en compte les changements.
    /// 
    /// If : Si la clé n'existe pas encore, on ajoute la station suivante et précédente
    /// Else : Sinon, on rajoute la station suivante et précédente à la station de même nom déjà trouvée
    /// Ensuite on transfère le dico de listes dans un dico de tables (optimisation mémoire)
    /// </summary>
    /// <param name="liensgen"></param>
    /// <param name="infos_noeuds"></param>
    /// <returns></returns>
    static Dictionary<string, string[]> Dico_adj_affichage(Lien[] liensgen, Noeud[] infos_noeuds)
    {
        Dictionary<string, List<string>> dico_listes = new Dictionary<string, List<string>>();
        foreach (Lien l in liensgen)
        {
            if (!dico_listes.ContainsKey(l.nom))
            {
                dico_listes[l.nom] = new List<string>();

                if (0 < l.id_precedent - 1 && l.id_precedent - 1 < infos_noeuds.Length)
                {
                    dico_listes[l.nom].Add(infos_noeuds[l.id_precedent - 1].Libelle_station);
                }
                if (0 < l.id_suivant - 1 && l.id_suivant - 1 < infos_noeuds.Length)
                {
                    dico_listes[l.nom].Add(infos_noeuds[l.id_suivant - 1].Libelle_station);
                }
            }
            else
            {

                if (0 < l.id_precedent - 1 && l.id_precedent - 1 < infos_noeuds.Length)
                {
                    if (!dico_listes[l.nom].Contains(infos_noeuds[l.id_precedent - 1].Libelle_station))
                    {
                        dico_listes[l.nom].Add(infos_noeuds[l.id_precedent - 1].Libelle_station);
                    }
                }
                if (0 < l.id_suivant - 1 && l.id_suivant - 1 < infos_noeuds.Length)
                {
                    if (!dico_listes[l.nom].Contains(infos_noeuds[l.id_suivant - 1].Libelle_station))
                    {
                        dico_listes[l.nom].Add(infos_noeuds[l.id_suivant - 1].Libelle_station);
                    }
                }
            }
        }
        Dictionary<string, string[]> dico_tableaux = new Dictionary<string, string[]>();
        foreach (var element in dico_listes)
        {
            dico_tableaux[element.Key] = element.Value.ToArray();
        }
        Console.WriteLine($"{dico_tableaux.Count()} données graphiques des liens traitées");
        return dico_tableaux;
    }


    /// <summary>
    /// Crée la liste d'adjacence réelle, où les stations où passent plusieurs lignes comptent double
    /// 
    /// On ajoute d'abord la station suivante et précédente si on n'est pas au bout de la ligne
    /// On ajoute ensuite les noeuds qui ont le même nom (pour les correspondances)
    /// Transfert des listes dans des tables ici aussi (optimisation mémoire)
    /// </summary>
    /// <param name="liensgen"></param>
    /// <param name="infos_noeuds"></param>
    /// <returns></returns>
    static Dictionary<Noeud, Noeud[]> Dico_ajd_reel(Lien[] liensgen, Noeud[] infos_noeuds)
    {
        Dictionary<Noeud, List<Noeud>> dico_listes = new Dictionary<Noeud, List<Noeud>>();
        foreach (Lien l in liensgen)
        {
            dico_listes[infos_noeuds[l.id_station - 1]] = new List<Noeud>();
            if (0 < l.id_precedent - 1 && l.id_precedent - 1 < infos_noeuds.Length)
            {
                dico_listes[infos_noeuds[l.id_station - 1]].Add(infos_noeuds[l.id_precedent - 1]);
            }
            if (0 < l.id_suivant - 1 && l.id_suivant - 1 < infos_noeuds.Length)
            {
                dico_listes[infos_noeuds[l.id_station - 1]].Add(infos_noeuds[l.id_suivant - 1]);
            }

            foreach (Noeud n in infos_noeuds)
            {
                if (l.nom == n.libelle_station &&
                    0 < l.id_suivant - 1 && l.id_suivant - 1 < infos_noeuds.Length)
                {
                    dico_listes[infos_noeuds[l.id_station - 1]].Add(n);
                }
            }
        }

        Dictionary<Noeud, Noeud[]> dico_tableaux = new Dictionary<Noeud, Noeud[]>();
        foreach (var element in dico_listes)
        {
            dico_tableaux[element.Key] = element.Value.ToArray();
        }
        Console.WriteLine($"{dico_tableaux.Count()} données des liens traitées");
        return dico_tableaux;
    }



    /// <summary>
    /// Fonction du calcul de la distance de Haversine
    /// On déclare le rayon terrestre R
    /// On converti en radians la latitude et la longitude de chaque noeuds
    /// On soustrait latitude et longitude de l'un à l'autre
    /// </summary>
    /// <param name="noeud1">Point de départ</param>
    /// <param name="noeud2">Point d'arrivée</param>
    /// <returns>Distance en km entre 2 stations</returns>
    public static double DistanceHaversine(Noeud noeud1, Noeud noeud2)
    {
        const double R = 6371.0;
        double lat1 = noeud1.latitude * Math.PI / 180.0;
        double lon1 = noeud1.longitude * Math.PI / 180.0;
        double lat2 = noeud2.latitude * Math.PI / 180.0;
        double lon2 = noeud2.longitude * Math.PI / 180.0;

        double diffLat = lat2 - lat1;
        double diffLon = lon2 - lon1;

        double a = Math.Pow(Math.Sin(diffLat / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(diffLon / 2), 2);
        double c = 2 * Math.Asin(Math.Sqrt(a));

        return R * c;
    }


    /// <summary>
    /// Fonction d'affichage d'un trajet
    /// On va afficher toutes les stations, la durée et le temps total de trajet donc on initialise ces variables
    /// 
    /// if : si le libellé est différent de la station précédente, cela signifie qu'on rest sur la même ligne
    /// else : sinon, cela veut dire qu'on fait un changement >> on prendra temps_changement pour le calcul du temps
    /// 
    /// a la fin on enlève la dernière flèche pour un affichage propre
    /// </summary>
    /// <param name="noeuds_trajet">"noeuds_trajet" sera la liste des noeuds à emprunter sur le chemin</param>
    /// <param name="lienscsv">"noeuds_trajet" sera la liste des noeuds à emprunter sur le chemin</param>
    static void AfficherTrajet(List<Noeud> noeuds_trajet, Lien[] lienscsv)
    {
        int duree = 0;
        double distance = 0;
        string message = "Trajet à emprunter : \n ";
        message += $"{noeuds_trajet[0].libelle_station} -> ";
        for (int i = 1; i < noeuds_trajet.Count - 1; i++)
        {
            if (noeuds_trajet[i].libelle_station != noeuds_trajet[i - 1].libelle_station)
            {
                message += $"{noeuds_trajet[i].libelle_station} -> ";
                duree += lienscsv[noeuds_trajet[i].id_station - 1].temps_deux_stations;
                distance += DistanceHaversine(noeuds_trajet[i], noeuds_trajet[i - 1]);
            }
            else
            {
                message += $"\nChanger pour la ligne {noeuds_trajet[i].libelle_ligne} -> ";
                duree += lienscsv[noeuds_trajet[i].id_station - 1].temps_deux_stations;
            }
        }
        message += $"{noeuds_trajet[noeuds_trajet.Count - 1].libelle_station} -> ";
        duree += lienscsv[noeuds_trajet[noeuds_trajet.Count - 1].id_station - 1].temps_deux_stations;


        message = message.Substring(0, message.Length - 3);
        message += $"\n\nDurée : {duree} min \n Distance : {distance} km";

        Console.WriteLine(message);
    }


    /// <summary>
    /// Fonction qui renvoie l'identifiant d'une station dont on connait le nom
    /// </summary>
    /// <param name="s">Nom de la station</param>
    /// <param name="infos_noeuds">informations sur toutes les stations</param>
    /// <returns>identifiant de la station</returns>
    static int IdDepuisLibelle(string s, Noeud[] infos_noeuds)
    {
        for (int i = 0; i < infos_noeuds.Length; i++)
        {
            if (infos_noeuds[i].Libelle_station == s)
            {
                return infos_noeuds[i].Id_station;
            }
        }
        return -1;
    }

    /// <summary>
    /// ALGO PCC -- DJIKSTRA
    /// 
    /// On prend des Hashset au lieu de listes pour éviter les doublons de stations
    /// On initialise les distances à l'infini (maxvalue)
    /// On ajoute la station de départ dans la liste "file"
    /// On trie file pour que le noeud avec la plus petite distance soit traité en premier
    /// break : si on est arrivés à la destination, on arrête et on ajoute le sommet dans les sommets vus
    /// 
    /// Ensuite on explore tous les noeuds autour de l'actuel et on calcule le poids jusqu'au voisin
    /// Si le poids est plus faible, on met à jour le poids et le chemin jusqu'à voisin
    /// </summary>
    /// <param name="depart">Noeud de départ</param>
    /// <param name="arrivee">Noeud d'arrivée</param>
    /// <param name="liste_adj">Liste d'adjacence des noeuds</param>
    /// <param name="liens">Liste des liens entre les stations</param>
    /// <param name="infos_noeuds">Informations sur le stations du métro (temps de changement etc)</param>
    /// <returns>Le plus court chemin entre les deux noeuds d'après Djikstra</returns>
    static List<Noeud> Pcc_Dijkstra(Noeud depart, Noeud arrivee, Dictionary<Noeud, Noeud[]> liste_adj, Lien[] liens, Noeud[] infos_noeuds)
    {
        Dictionary<Noeud, int> distances = new Dictionary<Noeud, int>();
        Dictionary<Noeud, Noeud> precedents = new Dictionary<Noeud, Noeud>();
        HashSet<Noeud> vus = new HashSet<Noeud>();
        List<Noeud> file = new List<Noeud>();

        foreach (Noeud n in infos_noeuds)
        {
            distances[n] = int.MaxValue;
            precedents[n] = null;
        }
        distances[depart] = 0;
        file.Add(depart);

        while (file.Count > 0)
        {
            file.Sort(delegate (Noeud a, Noeud b)
            {
                return Noeud.ComparerParDistance(a, b, distances);
            });
            Noeud courant = file[0];
            file.RemoveAt(0);

            if (courant == arrivee)
                break;

            vus.Add(courant);

            if (!liste_adj.ContainsKey(courant)) continue;

            foreach (Noeud voisin in liste_adj[courant])
            {
                if (vus.Contains(voisin)) continue;

                int poids = CalculPoidsArc(courant, voisin, liens);
                int nouvelle_dist = distances[courant] + poids;

                if (nouvelle_dist < distances[voisin])
                {
                    distances[voisin] = nouvelle_dist;
                    precedents[voisin] = courant;
                    if (!file.Contains(voisin)) file.Add(voisin);
                }
            }
        }

        List<Noeud> chemin = new List<Noeud>();
        Noeud actuel = arrivee;
        while (actuel != null)
        {
            chemin.Insert(0, actuel);
            actuel = precedents[actuel];
        }
        return chemin;
    }


    /// <summary>
    /// Trouve le temps entre deux stations : temps_deux_stations si on reste sur une ligne
    /// temps_changement si on change de ligne
    /// </summary>
    /// <param name="a">Station 1</param>
    /// <param name="b">Station 2</param>
    /// <param name="liens">Connexion entre les stations</param>
    /// <returns>Temps de changment entre deux station</returns>
    static int CalculPoidsArc(Noeud a, Noeud b, Lien[] liens)
    {
        foreach (Lien l in liens)
        {
            if (l.id_station == a.Id_station && (l.id_suivant == b.Id_station || l.id_precedent == b.Id_station))
            {
                if (a.Libelle_station == b.Libelle_station)
                    return l.Temps_changement;
                else
                    return l.Temps_deux_stations;
            }
        }
        return int.MaxValue;
    }


    /// <summary>
    /// ALGO PCC -- BELLMAN-FORD
    /// Même initialisatio à maxValue 
    /// On relaxe les arrêtes n-1 fois :
    /// Si passer par une autre station A pour aller à B est plus court, on met à jour le poids trouvé
    /// 
    /// On reconstruit le chemin dans une liste
    /// Et si pas de chemin ou la station de départ ne correspond pas, on n'a pas de solution 
    /// </summary>
    /// <<param name="depart">Noeud de départ</param>
    /// <param name="arrivee">Noeud d'arrivée</param>
    /// <param name="liste_adj">Liste d'adjacence des noeuds</param>
    /// <param name="liens">Liste des liens entre les stations</param>
    /// <param name="infos_noeuds">Informations sur le stations du métro (temps de changement etc)</param>
    /// <returns>Le plus court chemin entre les deux noeuds d'après Bellman-Ford</returns>
    static List<Noeud> Pcc_BellmanFord(Noeud depart, Noeud arrivee, Dictionary<Noeud, Noeud[]> liste_adj, Lien[] liens, Noeud[] infos_noeuds)
    {
        Dictionary<Noeud, int> distance = new Dictionary<Noeud, int>();
        Dictionary<Noeud, Noeud?> precedent = new Dictionary<Noeud, Noeud?>();

        foreach (Noeud n in infos_noeuds)
        {
            distance[n] = int.MaxValue;
            precedent[n] = null;
        }

        distance[depart] = 0;
        int nbNoeuds = infos_noeuds.Length;


        for (int i = 0; i < nbNoeuds - 1; i++)
        {
            foreach (Lien lien in liens)
            {
                Noeud u = infos_noeuds[lien.id_station - 1];
                if (!liste_adj.ContainsKey(u)) continue;

                foreach (Noeud v in liste_adj[u])
                {
                    int poids;
                    if (u.Libelle_station == v.Libelle_station)
                    {
                        poids = lien.Temps_changement;
                    }
                    else
                    {
                        poids = lien.Temps_deux_stations;
                    }
                    if (distance[u] != int.MaxValue && distance[u] + poids < distance[v])
                    {
                        distance[v] = distance[u] + poids;
                        precedent[v] = u;
                    }
                }
            }
        }

        List<Noeud> chemin = new List<Noeud>();
        Noeud? actuel = arrivee;

        while (actuel != null)
        {
            chemin.Insert(0, actuel);
            actuel = precedent[actuel];
        }

        if (chemin.Count == 0 || chemin[0] != depart)
            return new List<Noeud>();

        return chemin;
    }



    /// <summary>
    /// ALGO PCC -- FLOYD-WARSHALL
    /// On crée des dictionnaire pour accéder facilement aux noeuds avec leur index 
    /// Initialisation des matrices "distance" et "prédecesseurs"
    /// for : Initialisation des distances infinies sauf pour les voisins directs 
    /// (on met maxvalue/2 car la somme ne dépasse jamais ça, et plantage si on additionne deux maxvalues ensemble)
    /// On met ensuite à jour le poids des liens
    /// for : for : for : Algorithme de Floyd-Warshall
    /// Enfin, on reconstruit le chemin trouvé dans une liste
    /// 
    /// </summary>
    /// <<param name="depart">Noeud de départ</param>
    /// <param name="arrivee">Noeud d'arrivée</param>
    /// <param name="liste_adj">Liste d'adjacence des noeuds</param>
    /// <param name="liens">Liste des liens entre les stations</param>
    /// <param name="infos_noeuds">Informations sur le stations du métro (temps de changement etc)</param>
    /// <returns></returns>
    static List<Noeud> Pcc_Floyd_Warshall(Noeud depart, Noeud arrivee, Dictionary<Noeud, Noeud[]> liste_adj, Lien[] liens, Noeud[] infos_noeuds)
    {
        int n = infos_noeuds.Length;

        Dictionary<Noeud, int> noeudToIndex = new Dictionary<Noeud, int>();
        Dictionary<int, Noeud> indexToNoeud = new Dictionary<int, Noeud>();

        for (int i = 0; i < n; i++)
        {
            noeudToIndex[infos_noeuds[i]] = i;
            indexToNoeud[i] = infos_noeuds[i];
        }

        int[,] dist = new int[n, n];
        int?[,] pred = new int?[n, n];


        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i == j)
                {
                    dist[i, j] = 0;
                    pred[i, j] = null;
                }
                else
                {
                    dist[i, j] = int.MaxValue / 2;
                    pred[i, j] = null;
                }
            }
        }

        foreach (Lien lien in liens)
        {
            int u = lien.id_station - 1;
            Noeud noeudU = infos_noeuds[u];

            if (!liste_adj.ContainsKey(noeudU))
                continue;

            foreach (Noeud voisin in liste_adj[noeudU])
            {
                int v = noeudToIndex[voisin];
                int poids;
                if (noeudU.Libelle_station == voisin.Libelle_station)
                {
                    poids = lien.Temps_changement;
                }
                else
                {
                    poids = lien.Temps_deux_stations;
                }

                dist[u, v] = poids;
                pred[u, v] = u;
            }
        }


        for (int k = 0; k < n; k++)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (dist[i, k] + dist[k, j] < dist[i, j])
                    {
                        dist[i, j] = dist[i, k] + dist[k, j];
                        pred[i, j] = pred[k, j];
                    }
                }
            }
        }


        int indexDepart = noeudToIndex[depart];
        int indexArrivee = noeudToIndex[arrivee];

        if (dist[indexDepart, indexArrivee] >= int.MaxValue / 2)
        {
            return new List<Noeud>();
        }

        List<Noeud> chemin = new List<Noeud>();
        int? current = indexArrivee;

        while (current != null && current != indexDepart)
        {
            chemin.Insert(0, indexToNoeud[(int)current]);
            current = pred[indexDepart, (int)current];
        }

        chemin.Insert(0, depart);

        return chemin;
    }

    /// <summary>
    /// Fonction qui affiche toutes les information de chaque station
    /// </summary>
    /// <param name="noeuds">Liste des stations de métro</param>
    static void AfficherTableauNoeuds(Noeud[] noeuds)
    {
        foreach (var noeud in noeuds)
        {
            Console.WriteLine($"ID Station: {noeud.Id_station}");
            Console.WriteLine($"Ligne: {noeud.Libelle_ligne}");
            Console.WriteLine($"Station: {noeud.Libelle_station}");
            Console.WriteLine($"Latitude: {noeud.Latitude}");
            Console.WriteLine($"Longitude: {noeud.Longitude}");
            Console.WriteLine($"Commune: {noeud.Commune}");
            Console.WriteLine($"Code Commune: {noeud.Code_commune}");
            Console.WriteLine(new string('-', 20));
        }
    }

    /// <summary>
    /// Affiche tous les liens entre toutes les stations
    /// </summary>
    /// <param name="liens">Liste des liens entre toutes les stations</param>
    static void AfficherTableauLiens(Lien[] liens)
    {
        foreach (var lien in liens)
        {
            Console.WriteLine($"ID Station: {lien.Id_station}");
            Console.WriteLine($"Nom: {lien.Nom}");
            Console.WriteLine($"ID Précédent: {lien.Id_precedent}");
            Console.WriteLine($"ID Suivant: {lien.Id_suivant}");
            Console.WriteLine($"Temps entre deux stations: {lien.Temps_deux_stations} minutes");
            Console.WriteLine($"Temps de changement: {lien.Temps_changement} minutes");
            Console.WriteLine(new string('-', 20));
        }
    }

    /// <summary>
    /// Affiche la liste d'adjacence de toutes les stations du métro
    /// Pas appelée dans le menu actuel mais utile
    /// </summary>
    /// <param name="dicoAdj"></param>
    static void AfficherListeAdjacence(Dictionary<Noeud, Noeud[]> dicoAdj)
    {
        foreach (var paire in dicoAdj)
        {
            Noeud noeudPrincipal = paire.Key;
            Noeud[] adjacents = paire.Value;

            List<string> nomsAdjacents = new List<string>();
            foreach (Noeud n in adjacents)
            {
                nomsAdjacents.Add(n.Libelle_station);
            }

            string ligne = $"{noeudPrincipal.Libelle_station} : {string.Join(", ", nomsAdjacents)}";
            Console.WriteLine(ligne);
        }
    }



    /// <summary>
    /// Générer le fichier DOT à partir de la matrice d'adjacence
    /// On spécifie le format de graphique pour être horizontal
    /// On spécifie la direction du graphe (horizontal)
    /// On réduit l'espace entre les noeuds et les rangs
    /// On ajoute les noeuds et les liens dans le fichier DOT
    /// </summary>
    /// <param name="matriceAdjacence">Matrice d'adjacence des noeuds</param>
    /// <param name="dotFile">Le nom du fichier dot du graphe</param>
    static void GenerateDotFile(Dictionary<string, string[]> matriceAdjacence, string dotFile)
    {
        using (StreamWriter sw = new StreamWriter(dotFile))
        {
            sw.WriteLine("graph G {");
            sw.WriteLine("    rankdir=LR;");

            sw.WriteLine("    nodesep=0.5;");
            sw.WriteLine("    ranksep=0.5;");

            foreach (var station in matriceAdjacence)
            {
                foreach (var adjacente in station.Value)
                {
                    sw.WriteLine($"    \"{station.Key}\" -- \"{adjacente}\";");
                }
            }

            sw.WriteLine("}");
        }

        Console.WriteLine("Fichier DOT généré avec succès.");
    }


    /// <summary>
    /// Appeler Graphviz pour générer l'image à partir du fichier DOT
    /// On définie le nom du fichier de sortie de l'image 
    /// On s'assure que Graphviz est installé et accessible pou éviter les erreurs
    /// On appel Graphviz via 'ProcessStartInfo startInfo' pour générer l'image
    /// </summary>
    /// <param name="dotFile"></param>
    static void GenerateGraphImage(string dotFile)
    {
        string outputImage = "graph.png";

        string graphvizPath = @"C:\Program Files (x86)\Graphviz\bin\dot.exe";

        if (!File.Exists(graphvizPath))
        {
            Console.WriteLine("Graphviz n'est pas installé ou son chemin n'est pas valide.");
            return;
        }

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = graphvizPath,
            Arguments = $"-Tpng {dotFile} -o {outputImage}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process process = Process.Start(startInfo);
        process.WaitForExit();

        Console.WriteLine($"Image générée : {outputImage}");
    }

    /// <summary>
    /// Fonction qui affiche la liste des commandes d'un client de manière propre et lisible (sous forme de tableau)
    /// On créé la requête SQL adaptée et on lit le résultat
    /// On parcours cellule par cellule le résultat obtenu
    /// On sépare chaque données par un '!' (on aurait pu le séparer autrement mais les '.' et les ',' posaient problème au niveau des chiffres
    /// On vérifie la taille du plus grand élément de chaque informations (nom, prénom, prix...), cette vérification est lourde et peut utile dans notre cas donc elle n'apparaitra plus dans les autres programmes d'affichage
    /// </summary>
    /// <param name="id">Identifiant du client</param>
    /// <param name="connection">Connection utilisé pour accéder aux données de la base de données</param>
    static void Afficher_Commande_client(int id, MySqlConnection connection)
    {
        Console.WriteLine("Voici la liste de vos commande !");

        MySqlCommand Afficher_commande_utilisateur = connection.CreateCommand();
        Afficher_commande_utilisateur.CommandText = @"SELECT C.Numero_Commande, Pl.Nom_Plat, C.Prix_commande, C.Quantite, C.Date_Commande, C.Date_Livraison, C.Status
                                                                                                        FROM Commande C
                                                                                                        JOIN Passer P ON C.Numero_Commande = P.Numero_Commande 
                                                                                                        JOIN Contient Co ON C.Numero_Commande = Co.Numero_Commande
                                                                                                        JOIN Plat Pl ON Co.Nom_Plat = Pl.Nom_Plat
                                                                                                        WHERE P.ID_Client = @ID_Client
                                                                                                        ORDER BY C.Numero_Commande ASC; ";
        Afficher_commande_utilisateur.Parameters.AddWithValue("@ID_Client", id);

        MySqlDataReader reader_Afficher_commande_utilisateur = Afficher_commande_utilisateur.ExecuteReader();
        List<string> Liste_commande = new List<string>();

        int taille_Max_Numero_commande_1 = "Numéro de commande".Length;
        int taille_Max_Plat_1 = "Nom du plat".Length;
        int taille_Max_Prix_1 = "Prix".Length;
        int taille_Max_Quantite_1 = "Quantité".Length;
        int taille_Max_Date_1 = "Date livraison".Length;
        int taille_Max_Statut_1 = "Statut".Length;
        while (reader_Afficher_commande_utilisateur.Read())
        {
            string currentRowAsString = "";
            for (int i = 0; i < reader_Afficher_commande_utilisateur.FieldCount; i++)
            {
                string valueAsString = reader_Afficher_commande_utilisateur.GetValue(i).ToString();
                currentRowAsString += valueAsString + "!";
                if (i == 0)
                {
                    if (valueAsString.Length > taille_Max_Numero_commande_1)
                    {
                        taille_Max_Numero_commande_1 = valueAsString.Length;
                    }
                }
                else if (i == 1)
                {
                    if (valueAsString.Length > taille_Max_Plat_1)
                    {
                        taille_Max_Plat_1 = valueAsString.Length;
                    }
                }
                else if (i == 2)
                {
                    if (valueAsString.Length > taille_Max_Prix_1)
                    {
                        taille_Max_Prix_1 = valueAsString.Length;
                    }
                }
                else if (i == 3)
                {
                    if (valueAsString.Length > taille_Max_Quantite_1)
                    {
                        taille_Max_Quantite_1 = valueAsString.Length;
                    }
                }
                else if (i == 4)
                {
                    if (valueAsString.Length > taille_Max_Date_1)
                    {
                        taille_Max_Date_1 = valueAsString.Length;
                    }
                }
                else if (i == 6)
                {
                    if (valueAsString.Length > taille_Max_Statut_1)
                    {
                        taille_Max_Statut_1 = valueAsString.Length;
                    }
                }


            }
            Liste_commande.Add(currentRowAsString);
        }
        Console.WriteLine("Numéro de commande".PadRight(taille_Max_Numero_commande_1) + " | " + "Nom du plat".PadRight(taille_Max_Plat_1) + " | " + "Prix".PadRight(taille_Max_Prix_1) + " | " + "Quantité".PadRight(taille_Max_Quantite_1) + " | " + "Date envoie".PadRight(taille_Max_Date_1) + " | " + "Date livraison".PadRight(taille_Max_Date_1) + " | " + "Statut".PadRight(taille_Max_Statut_1) + " | ");
        int nb_caractère_1 = taille_Max_Numero_commande_1 + taille_Max_Plat_1 + taille_Max_Prix_1 + taille_Max_Quantite_1 + (taille_Max_Date_1 * 2) + taille_Max_Statut_1 + (7 * 3) - 1;
        string tiret_1 = "";
        for (int i = 0; i < nb_caractère_1; i++)
        {
            tiret_1 += "=";
        }
        Console.WriteLine(tiret_1);
        foreach (string com in Liste_commande)
        {
            string[] elements = com.Split('!');
            Console.WriteLine(elements[0].PadRight(taille_Max_Numero_commande_1) + " | " + elements[1].PadRight(taille_Max_Plat_1) + " | " + elements[2].PadRight(taille_Max_Prix_1) + " | " + elements[3].PadRight(taille_Max_Quantite_1) + " | " + elements[4].PadRight(taille_Max_Date_1) + " | " + elements[5].PadRight(taille_Max_Date_1) + " | " + elements[6].PadRight(taille_Max_Statut_1) + " | ");
        }
        reader_Afficher_commande_utilisateur.Close();
    }

    /// <summary>
    /// Même principe que la fonction précédante sauf qu'on se place du point de vue du cuisinier
    /// On fait la bonne requete SQL
    /// On parcours toutes les cellules du résultat, on associe chaque valeurs à une variable
    /// On récupère le metro de chaque client ayant passé une commande ainsi que celui du cuisinier et on affiche le trajet
    /// On affiche tout
    /// </summary>
    /// <param name="id_connection_cuisinier">identifiant du cuisinier</param>
    /// <param name="connection">connection à la base de données</param>
    /// <param name="infos_noeuds">Liste des infos sur les stations de Paris</param>
    /// <param name="lienscsv">Liens entre les stations</param>
    /// <param name="Dico_adjacence_reel">Liste d'adjacence des noeuds</param>
    static void Afficher_Commande_Cuisinier(int id_connection_cuisinier, MySqlConnection connection, Noeud[] infos_noeuds, Lien[] lienscsv, Dictionary<Noeud, Noeud[]> Dico_adjacence_reel)
    {
        Console.WriteLine("Liste des commandes à préparer :");
        Console.WriteLine();
        Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        MySqlCommand recup_metro_cuiso = connection.CreateCommand();
        recup_metro_cuiso.CommandText = @"SELECT Metro_Proche FROM Utilisateur WHERE ID_Client=@ID_Cuisinier";
        recup_metro_cuiso.Parameters.AddWithValue("@ID_Cuisinier", id_connection_cuisinier);
        MySqlDataReader reader_metro_cuisto = recup_metro_cuiso.ExecuteReader();
        string metro_cuisto = "";
        while (reader_metro_cuisto.Read())
        {
            metro_cuisto = reader_metro_cuisto.GetString(0);
        }
        reader_metro_cuisto.Close();

        MySqlCommand recup_commandes = connection.CreateCommand();
        recup_commandes.CommandText = @"SELECT
                                            C.Numero_Commande,
                                            Pl.Nom_Plat,
                                            C.Prix_commande,
                                            C.Quantite,
                                            C.Date_Commande,
                                            C.Date_Livraison,
                                            U.Nom AS Nom_Client,
                                            U.Metro_Proche
                                       FROM Commande C
                                       JOIN Contient Co ON C.Numero_Commande = Co.Numero_Commande
                                       JOIN Plat Pl ON Co.Nom_Plat = Pl.Nom_Plat
                                       JOIN Cuisine Cu ON Pl.Nom_Plat = Cu.Nom_Plat
                                       JOIN Passer P ON C.Numero_Commande = P.Numero_Commande
                                       JOIN Client Cl ON P.ID_Client = Cl.ID_Client
                                       JOIN Utilisateur U ON Cl.ID_Client = U.ID_Client
                                       WHERE Cu.ID_Client = @ID_Cuisinier
                                       ORDER BY C.Date_Commande;";


        recup_commandes.Parameters.AddWithValue("@ID_Cuisinier", id_connection_cuisinier);

        MySqlDataReader reader_commandes = recup_commandes.ExecuteReader();

        while (reader_commandes.Read())
        {

            int numeroCommande = reader_commandes.GetInt32(0);
            string plat = reader_commandes.GetString(1);
            decimal prixDecimal = reader_commandes.GetDecimal(2);
            string prix = prixDecimal.ToString("0.00");
            int quantite = reader_commandes.GetInt32(3);
            string dateCmd = reader_commandes.GetDateTime(4).ToString("dd/MM/yyyy");
            string dateLiv = reader_commandes.GetDateTime(5).ToString("dd/MM/yyyy");
            string nomClient = reader_commandes.GetString(6);
            string metro = reader_commandes.GetString(7);

            Console.WriteLine($"Numéro de commande: {numeroCommande} Contenue: {plat} \nPrix: {prix} euro \nPour {quantite} personnes \nDate de la commande {dateCmd}\nDate de livraison: {dateLiv}\nPour {nomClient} qui est proche du métro {metro}");
            Console.WriteLine();
            List<Noeud> noeuds_trajet_1 = new List<Noeud>();
            int id_depart_1 = IdDepuisLibelle(metro_cuisto, infos_noeuds);
            int id_arrivee_1 = IdDepuisLibelle(metro, infos_noeuds);

            Console.WriteLine($"Chemin de {infos_noeuds[id_depart_1 - 1].Libelle_station} a {infos_noeuds[id_arrivee_1 - 1].Libelle_station}");
            noeuds_trajet_1 = Pcc_BellmanFord(infos_noeuds[id_depart_1 - 1], infos_noeuds[id_arrivee_1 - 1], Dico_adjacence_reel, lienscsv, infos_noeuds);
            //Console.WriteLine($"\nNombre de stations : {noeuds_trajet_1.Count()}");
            AfficherTrajet(noeuds_trajet_1, lienscsv);
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------");

        }
        reader_commandes.Close();
    }

    /// <summary>
    /// Fonction qui demande une saisie à l'utilisateur 
    /// Elle s'assure que l'utilisateur saisie un entier 
    /// Compte le nombre de caractères saisie et vérifie qu'il y a autant de caractères que de caractères numériques, et que le mot n'est pas null
    /// On fait un do while pour gagner des lignes
    /// </summary>
    /// <param name="message">Le message qui doit etre affiché à l'tilisateur, en général "Saisissez un nombre entier"</param>
    /// <returns></returns>
    public static int SaisieEntier(string message)
    {
        string saisie;
        char[] entier = new char[10];
        entier = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
        int verificateur = 0;

        do
        {
            Console.WriteLine("");
            Console.WriteLine(message);
            saisie = Console.ReadLine();
            verificateur = 0;
            if (saisie != null)
            {
                for (int j = 0; j < saisie.Length; j++)
                {
                    for (int k = 0; k < entier.Length; k++)
                    {
                        if (saisie[j] == entier[k])
                        {
                            verificateur++;
                        }
                    }
                }
            }
            if (saisie == null || saisie.Length == 0 || verificateur != saisie.Length)
            {
                Console.Clear();
                Console.WriteLine("Mauvaise saisie recommencez");
            }
        }
        while (saisie == null || saisie.Length == 0 || verificateur != saisie.Length);
        return Convert.ToInt32(saisie);
    }


    /// <summary>
    /// Algorithme de Weslh-Powell
    /// Tri dans l'ordre décroissant les sommets du graphes
    /// Créé une couleur et l'associe au maximum de sommet possible en commençant par e sommet de plus haut degré
    /// Permet de colorier le graphe avec un nombre minimum de couleur
    /// On ajoute une nouvelle couleur à chaque fois qu'on est bloqués
    /// </summary>
    /// <param name="graph">Graphe à colorier</param>
    /// <returns>Graphe colorié</returns>
    static Dictionary<string, int> WelshPowellColoration(Dictionary<string, string[]> graph)
    {
        var CouleursTriees = graph.OrderByDescending(kv => kv.Value.Length).Select(kv => kv.Key).ToList();
        Dictionary<string, int> Couleurs_stations = new Dictionary<string, int>();
        int NbCouleurs = 0;

        foreach (var couleur in CouleursTriees)
        {
            if (!Couleurs_stations.ContainsKey(couleur))
            {
                Couleurs_stations[couleur] = NbCouleurs;

                foreach (var autre in CouleursTriees)
                {
                    if (!Couleurs_stations.ContainsKey(autre) &&
                        !graph[couleur].Contains(autre) &&
                        !graph[autre].Any(n => Couleurs_stations.ContainsKey(n) && Couleurs_stations[n] == NbCouleurs))
                    {
                        Couleurs_stations[autre] = NbCouleurs;
                    }
                }

                NbCouleurs++;
            }
        }

        Console.WriteLine($"Coloration terminée avec {NbCouleurs} couleurs.");
        return Couleurs_stations;
    }


    /// <summary>
    /// Permet de générer un graphe non pondéré colorié
    /// Il définit la couleur de chaque noeud dans un premier temps
    /// Il ajoute les arrêtes sans doublon
    /// Enfin la fonction génère le graphe grâce à Graphviz
    /// </summary>
    /// <param name="graph">Graphe à générer en couleur</param>
    /// <param name="couleurs">Palette de couleur</param>
    /// <param name="cheminDot">Chemin du fichier dot</param>
    /// <param name="cheminPng">Chemin ou atterit l'image générée</param>
    static void GenererGraphvizAvecCouleurs(Dictionary<string, string[]> graph, Dictionary<string, int> couleurs, string cheminDot = "graphe.dot", string cheminPng = "graphe.png")
    {
        using (StreamWriter sw = new StreamWriter(cheminDot))
        {
            sw.WriteLine("graph G {");
            sw.WriteLine("node [style=filled];");

            foreach (var kvp in couleurs)
            {
                string nodeId = $"\"{kvp.Key}\"";
                string color = CouleurDepuisIndice(kvp.Value);
                sw.WriteLine($"{nodeId} [fillcolor={color}];");
            }

            HashSet<string> addedEdges = new HashSet<string>();
            foreach (var kvp in graph)
            {
                string node = $"\"{kvp.Key}\"";
                foreach (var voisin in kvp.Value)
                {
                    string voisinQuoted = $"\"{voisin}\"";
                    string edge = $"{node} -- {voisinQuoted}";
                    string edgeReverse = $"{voisinQuoted} -- {node}";
                    if (!addedEdges.Contains(edge) && !addedEdges.Contains(edgeReverse))
                    {
                        sw.WriteLine($"{node} -- {voisinQuoted};");
                        addedEdges.Add(edge);
                    }
                }
            }

            sw.WriteLine("}");
        }

        var process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "dot";
        process.StartInfo.Arguments = $"-Tpng {cheminDot} -o {cheminPng}";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.Start();
        process.WaitForExit();

        Console.WriteLine($"Image générée : {cheminPng}");
    }

    /// <summary>
    /// Palette de couleur
    /// </summary>
    /// <param name="index">Indice de la couleur voulue dans la palette</param>
    /// <returns>Couleur voulue</returns>
    static string CouleurDepuisIndice(int index)
    {
        string[] couleurs = { "red", "blue", "green", "yellow", "orange", "cyan", "purple", "pink", "brown", "grey" };
        return couleurs[index % couleurs.Length];
    }

    /// <summary>
    /// Génère un fichier dot avec des coordonnées centrées pour un meilleur effet visuel et un plus beau rendu
    /// Pour commencer on crée un dictionnaire pour accéder aux coordonnées par libellé de station
    /// Si doublons, on prend le 1er
    /// Pour chaque position on dessin des noeuds avec positions
    /// On sécurise le label avec 'string libelle = kvp.Key.Replace("\"", "");'
    /// On ajoute ensuite les arrêtes entre chaque noeuds
    /// </summary>
    /// <param name="adjacence">Liste d'adjacence des noeuds</param>
    /// <param name="infos_noeuds">Informations sur les stations</param>
    /// <param name="nomFichierDot">Nom du fichier généré</param>
    static void GenererDotAvecCoordonnees(Dictionary<string, string[]> adjacence, Noeud[] infos_noeuds, string nomFichierDot)
    {
        var positions = infos_noeuds
            .GroupBy(n => n.libelle_station)
            .ToDictionary(g => g.Key, g => g.First());

        using (StreamWriter sw = new StreamWriter(nomFichierDot))
        {
            sw.WriteLine("graph G {");
            sw.WriteLine("    layout=neato;");
            sw.WriteLine("    overlap=false;");
            sw.WriteLine("    splines=true;");

            foreach (var kvp in positions)
            {
                string libelle = kvp.Key.Replace("\"", "");
                Noeud noeud = kvp.Value;
                sw.WriteLine($"    \"{libelle}\" [pos=\"{noeud.longitude.ToString(CultureInfo.InvariantCulture)},{noeud.latitude.ToString(CultureInfo.InvariantCulture)}!\", shape=circle];");
            }

            HashSet<string> dejaAjoute = new HashSet<string>();
            foreach (var kvp in adjacence)
            {
                string source = kvp.Key;
                foreach (string dest in kvp.Value)
                {
                    string edgeKey = source + "--" + dest;
                    string reverseKey = dest + "--" + source;
                    if (!dejaAjoute.Contains(edgeKey) && !dejaAjoute.Contains(reverseKey))
                    {
                        sw.WriteLine($"    \"{source}\" -- \"{dest}\";");
                        dejaAjoute.Add(edgeKey);
                    }
                }
            }
            sw.WriteLine("}");
        }
    }

    /// <summary>
    /// Génère une image à partir d'un fichier dot
    /// </summary>
    /// <param name="dotFilePath">Fichier dot</param>
    /// <param name="pngOutputPath">Fichier de sortie</param>
    static void GenererImageAvecDot(string dotFilePath, string pngOutputPath)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "dot",
            Arguments = $"-Tpng \"{dotFilePath}\" -o \"{pngOutputPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process proc = Process.Start(psi))
        {
            string stdout = proc.StandardOutput.ReadToEnd();
            string stderr = proc.StandardError.ReadToEnd();
            proc.WaitForExit();

            if (proc.ExitCode != 0)
            {
                Console.WriteLine("Erreur lors de l'exécution de dot :");
                Console.WriteLine(stderr);
            }
            else
            {
                Console.WriteLine("Image PNG générée avec succès.");
            }
        }

    }


    /// <summary>
    /// La fonction de génération de graphe précédente ne permet pas de générer des graphes pondéré.
    /// Cependant pour créer un graphe de liens entre utilisateur, il est plus approprié de générer un graphe pondérer qui prend en poid des arrêtes le nombre d'interractions entre chaque utilisateur
    /// D'abord on construit la liste d'adjacence 'adjacencyList' (non orienté)
    /// On utilise ensuite l'algorithme de welsh-powell créé plus tôt pour colorier ce graphe
    /// On passe ensuite à la génération du fichier DOT avec couleurs et poids
    /// Pour cela on génère d'abord les sommets coloriés
    /// Ensuite on crée les arrêtes avec leur poid
    /// </summary>
    /// <param name="edges">Dictionnaire qui prend les deux utilisateur (from et to) liés et leur nombre d'interraction </param>
    /// <param name="filePath">Nom du fichier généré</param>
    static void GenererDotGraphePondere(Dictionary<(string from, string to), int> edges, string filePath)
    {
        Dictionary<string, HashSet<string>> adjacencyList = new();
        foreach (var edge in edges)
        {
            string from = edge.Key.from;
            string to = edge.Key.to;

            if (!adjacencyList.ContainsKey(from)) adjacencyList[from] = new HashSet<string>();
            if (!adjacencyList.ContainsKey(to)) adjacencyList[to] = new HashSet<string>();

            adjacencyList[from].Add(to);
            adjacencyList[to].Add(from);
        }

        var graph = adjacencyList.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToArray()
        );
        Dictionary<string, int> nodeColors = WelshPowellColoration(graph);

        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("graph G {");
            writer.WriteLine("  node [shape=ellipse, fontname=\"Arial\"];");
            writer.WriteLine("  edge [fontname=\"Arial\"];");

            foreach (var node in adjacencyList.Keys)
            {
                string escapedNode = node.Replace("\"", "\\\"").Replace("\n", "\\n");
                string colorName = CouleurDepuisIndice(nodeColors[node]);
                writer.WriteLine($"  \"{escapedNode}\" [style=filled, fillcolor={colorName}];");
            }

            foreach (var edge in edges)
            {
                string fromEscaped = edge.Key.from.Replace("\"", "\\\"").Replace("\n", "\\n");
                string toEscaped = edge.Key.to.Replace("\"", "\\\"").Replace("\n", "\\n");
                int weight = edge.Value;

                writer.WriteLine($"  \"{fromEscaped}\" -- \"{toEscaped}\" [label=\"{weight}\"];");
            }

            writer.WriteLine("}");
        }
    }

    /// <summary>
    /// Indique si le graphe est biparti ou non
    /// </summary>
    /// <param name="graphe">Graphe à vérifier</param>
    /// <returns>True si il est biparti, false sinon</returns>
    static bool EstBiparti(Dictionary<string, List<string>> graphe)
    {
        var couleurs = new Dictionary<string, int>();

        foreach (var sommet in graphe.Keys)
        {
            if (!couleurs.ContainsKey(sommet))
            {
                var file = new Queue<string>();
                file.Enqueue(sommet);
                couleurs[sommet] = 0;

                while (file.Count > 0)
                {
                    string u = file.Dequeue();
                    foreach (var voisin in graphe[u])
                    {
                        if (!couleurs.ContainsKey(voisin))
                        {
                            couleurs[voisin] = 1 - couleurs[u];
                            file.Enqueue(voisin);
                        }
                        else if (couleurs[voisin] == couleurs[u])
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Vérifie si un graphe est planaire
    /// On regarde le nombre de sommet du graphe
    /// Si le nombre de sommet est inférieur à 3 alors le graphe est forcément planaire
    /// </summary>
    /// <param name="graphe">Graphe à véifier</param>
    /// <returns>True si le graphe est planaire, false sinon</returns>
    static bool EstPlanaire(Dictionary<string, List<string>> graphe)
    {
        int n = graphe.Count;
        int m = graphe.Sum(pair => pair.Value.Count) / 2;

        if (n < 3)
        {
            return true;
        }
        else {
            return m <= 3 * n - 6;
        }  
    }

    /// <summary>
    /// Exporte une base de données en un seul fichier XML
    /// Récupère toutes les tables de la base de données 
    /// Écrit tout dans un seul fichier XML avec le schéma
    /// </summary>
    /// <param name="connection">Connection SQL</param>
    /// <param name="xmlFilePath">Chemin de sortie du fichier</param>
    static void ExportDatabaseToSingleXml(MySqlConnection connection, string xmlFilePath)
    {
        var dataSet = new DataSet("ApplicationDatabase");
        var tables = connection.GetSchema("Tables");

        foreach (DataRow row in tables.Rows)
        {
            string tableName = row["TABLE_NAME"].ToString();

            using var command = new MySqlCommand($"SELECT * FROM `{tableName}`", connection);
            using var adapter = new MySqlDataAdapter(command);
            var dataTable = new DataTable(tableName);
            adapter.Fill(dataTable);

            dataSet.Tables.Add(dataTable);
        }
        using var writer = XmlWriter.Create(xmlFilePath, new XmlWriterSettings { Indent = true });
        dataSet.WriteXml(writer, XmlWriteMode.WriteSchema);
    }

    /// <summary>
    /// Exporte la base de données dans un seul fichier JSON
    /// </summary>
    /// <param name="connection">Connection à la base de données</param>
    /// <param name="jsonFilePath">Nom du fichier de sortie JSON</param>
    static void ExportDatabaseToJson(MySqlConnection connection, string jsonFilePath)
    {
        var dbDict = new Dictionary<string, object>();

        var tables = connection.GetSchema("Tables");

        foreach (DataRow row in tables.Rows)
        {
            string tableName = row["TABLE_NAME"].ToString();

            using var command = new MySqlCommand($"SELECT * FROM `{tableName}`", connection);
            using var reader = command.ExecuteReader();

            var tableData = new List<Dictionary<string, object>>();

            while (reader.Read())
            {
                var rowData = new Dictionary<string, object>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    rowData[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }

                tableData.Add(rowData);
            }

            dbDict[tableName] = tableData;
            reader.Close();
        }

        var json = JsonConvert.SerializeObject(dbDict, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(jsonFilePath, json);
    }

}
