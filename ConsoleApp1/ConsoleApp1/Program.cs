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
class Program
{
        static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey(true);
        }
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

            //AfficherListeAdjacence(Dico_adjacence_reel); 
            //AfficherTableauNoeuds(infos_noeuds);
            //AfficherTableauLiens(lienscsv); 

            Console.WriteLine("fin de l'intitialisation");
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
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
        
            bool continuer = true;

            while (continuer)
            {
                Console.Clear();
                Console.WriteLine("1: Créer un utilisateur");
                Console.WriteLine("2: Vous êtes un client");
                Console.WriteLine("3: Vous êtes un cuisinier");
                Console.WriteLine("4: Générer la carte du métro");
                Console.WriteLine("5: Afficher le graphe");
                Console.WriteLine("6: Chercher un PCC");
                Console.WriteLine("Échap: Quitter");

                ConsoleKeyInfo cki = Console.ReadKey(true);

                switch (cki.Key)
                {
                    // Créer un Utilisateur
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        Console.Clear();
                        Console.WriteLine("Vous allez créer un utilisateur");

                        Console.WriteLine("L'utilisateur est-il un cuisinier ou un client?");
                        string type_utilisateur = Console.ReadLine();
                        type_utilisateur = type_utilisateur.ToLower();

                        if (type_utilisateur != "cuisinier" && type_utilisateur != "client") {
                            while (type_utilisateur != "cuisinier" && type_utilisateur != "client") {
                                Console.WriteLine("Vous devez indiquer client ou cuisinier");
                                type_utilisateur = Console.ReadLine();
                                type_utilisateur = type_utilisateur.ToLower();
                            }
                        }

                        string nom_utilisateur = "";
                        string prenom_utilisateur = "";
                        int nb_commande_utilisateur = 0;

                        //si l'utilisateur est un client on aura besoin de ces variables
                        string type_de_client = "";
                        DateTime date_inscription_client = DateTime.Today;
                        string nom_referent = "";
                        string mail_referent = "";
                        string tel_referent = "";

                        //si l'utilisateur est un cuisinier on aura besoin de sa note moyenne
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

                        Console.WriteLine("Quel est le numéro de sa rue?");
                        int numeroRue_utilisateur = int.Parse(Console.ReadLine());

                        Console.WriteLine("Quel est son code postal?");
                        int codePostal_utilisateur = int.Parse(Console.ReadLine());

                        Console.WriteLine("Quel est le nom de sa ville?");
                        string ville_utilisateur = Console.ReadLine();

                        Console.WriteLine("Quel est son numéro de téléphone?");
                        int telephone_utilisateur = int.Parse(Console.ReadLine());

                        Console.WriteLine("Quel est son mail?");
                        string mail_utilisateur = Console.ReadLine();

                        Console.WriteLine("Quel est le métro le plus proche de lui?");
                        string metro_utilisateur = Console.ReadLine();

                        Console.WriteLine("Quel est son mot de passe?");
                        string mdp_utilisateur = Console.ReadLine();

                        string identifiant_utilisateur = "";

                        for (int i = 0; i < 7; i++) {
                            Random random = new Random();
                            int nombreAleatoire = random.Next(9);
                            identifiant_utilisateur += nombreAleatoire;
                        }
                        long identifiant_utilisateur_int = long.Parse(identifiant_utilisateur);


                        // Insérer tout dans la base de données
                        MySqlCommand command = connection.CreateCommand();
                        command.CommandText = "INSERT INTO Utilisateur (ID_Client, Nom, Prenom, Rue, Numero, Code_Postal, Ville, Telephone, Email, Metro_Proche, Type_client, Mot_De_Passe) " +
                          "VALUES (@ID_Client, @Nom, @Prenom, @Rue, @Numero, @Code_Postal, @Ville, @Telephone, @Email, @Metro_Proche, @Type_client, @Mot_De_Passe)";

                        // Ajout des paramètres
                        command.Parameters.AddWithValue("@ID_Client", identifiant_utilisateur_int);
                        command.Parameters.AddWithValue("@Nom", nom_utilisateur);
                        command.Parameters.AddWithValue("@Prenom", prenom_utilisateur);
                        command.Parameters.AddWithValue("@Rue", nomRue_utilisateur);
                        command.Parameters.AddWithValue("@Numero", numeroRue_utilisateur);
                        command.Parameters.AddWithValue("@Code_Postal", codePostal_utilisateur);
                        command.Parameters.AddWithValue("@Ville", ville_utilisateur);
                        command.Parameters.AddWithValue("@Telephone", telephone_utilisateur);
                        command.Parameters.AddWithValue("@Email", mail_utilisateur);
                        command.Parameters.AddWithValue("@Metro_Proche", metro_utilisateur);
                        command.Parameters.AddWithValue("@Type_client", type_utilisateur);
                        command.Parameters.AddWithValue("@Mot_De_Passe", mdp_utilisateur);

                        MySqlDataReader reader;
                        reader = command.ExecuteReader();
                        reader.Close();

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
                        else {
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


                    // Vous etes un client
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
                                string motDePasseStocke = reader_mdp_requete.GetString(0);
                                if (motDePasseStocke == mdp_connexion)
                                {
                                    reader_mdp_requete.Close();
                                    Console.Clear();
                                    Console.WriteLine("Connexion réussie !\nVeuillez patienter quelques secondes");
                                    Thread.Sleep(2000);
                                    Console.Clear();

                                    MySqlCommand nom_connecte = connection.CreateCommand();
                                    nom_connecte.CommandText = "SELECT nom FROM Utilisateur WHERE ID_Client = @ID_Client AND Type_client = 'client'";
                                    nom_connecte.Parameters.AddWithValue("@ID_Client", id_connection_client);

                                    MySqlDataReader reader_nom_connecte = nom_connecte.ExecuteReader();

                                    string nom_Affiche = "";
                                    if (reader_nom_connecte.Read()) {
                                        nom_Affiche = reader_nom_connecte.GetString(0);                                    
                                    }
                                    reader_nom_connecte.Close();


                                    MySqlCommand prenom_connecte = connection.CreateCommand();
                                    prenom_connecte.CommandText = "SELECT prenom FROM Utilisateur WHERE ID_Client = @ID_Client AND Type_client = 'client'";
                                    prenom_connecte.Parameters.AddWithValue("@ID_Client", id_connection_client);

                                    MySqlDataReader reader_prenom_connecte = prenom_connecte.ExecuteReader();

                                    string prenom_Affiche="";
                                    if (reader_prenom_connecte.Read()) {
                                        prenom_Affiche = reader_prenom_connecte.GetString(0);
                                    }
                                    reader_prenom_connecte.Close();

                                    Console.WriteLine($"{prenom_Affiche} {nom_Affiche}");
                                    Console.WriteLine();
                                    Console.WriteLine("1: Passer commande");
                                    Console.WriteLine("2: Annuler une commande");
                                    Console.WriteLine("3: Voir vos commandes");

                                    ConsoleKeyInfo cki_client = Console.ReadKey(true);
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
                                            else {
                                                prix = $" AND Prix_plat<={prix_maximum}";
                                            }
                                            Console.WriteLine();

                                            Console.WriteLine("Voulez-vous une cuisine particulière (française, italienne...) (oui/non)");
                                            string origine = (Console.ReadLine()).ToLower();
                                            string origine_plat = "";
                                            if (origine == "oui") {
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

                                            string commande_totale = "SELECT* FROM Plat"+vege+prix+origine_plat+";";
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
                                                for (int i = 0; i < reader_commande_plat.FieldCount; i++)    // parcours cellule par cellule
                                                {
                                                    string valueAsString = reader_commande_plat.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                                                    currentRowAsString += valueAsString + ", ";
                                                }
                                                Liste_plat.Add(numero_plat_comande, currentRowAsString);
                                                Console.WriteLine(numero_plat_comande+" : "+currentRowAsString);    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
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
                                                Recuperer_adresse.CommandText = "SELECT* FROM Utilisateur WHERE ID_Client='"+id_connection_client+"'";

                                                MySqlDataReader reader_Recuperer_adresse;
                                                reader_Recuperer_adresse = Recuperer_adresse.ExecuteReader();
                                            
                                                string donnes_client = "";
                                                while (reader_Recuperer_adresse.Read())
                                                {                                                
                                                    for (int i = 0; i < reader_Recuperer_adresse.FieldCount; i++)    // parcours cellule par cellule
                                                    {
                                                        string valueAsString = reader_Recuperer_adresse.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                                                        donnes_client += valueAsString + ", ";
                                                    }
                                                }
                                                string[] detail_client = donnes_client.Split(", ");
                                                string adresse_client_connecte = $"{detail_client[4]} {detail_client[3]} {detail_client[5]} {detail_client[6]}";
                                                reader_Recuperer_adresse.Close();


                                                // Définir les valeurs
                                                decimal prixCommande = decimal.Parse(details_Plat[3]);
                                                int quantite = 1;
                                                DateTime dateLivraison = DateTime.Today;
                                                string adresseLivraison = adresse_client_connecte;

                                                // Insérer la commande dans `Commande`
                                                MySqlCommand ajouter_Commande = connection.CreateCommand();                                            
                                                ajouter_Commande.CommandText = "INSERT INTO Commande (Prix_Commande, Quantite, Date_Commande, Date_Livraison, Status) VALUES (@Prix_Commande, @Quantite, NOW(), @Date_Livraison, 'En attente')";
                                                ajouter_Commande.Parameters.AddWithValue("@Prix_Commande", prixCommande);
                                                ajouter_Commande.Parameters.AddWithValue("@Quantite", quantite);
                                                ajouter_Commande.Parameters.AddWithValue("@Date_Livraison", dateLivraison);

                                                MySqlDataReader reader_inserer_commande;
                                                reader_inserer_commande = ajouter_Commande.ExecuteReader();
                                                reader_inserer_commande.Close();

                                                // Récupérer l'ID de la commande insérée
                                                long numeroCommande;
                                                MySqlCommand getID = new MySqlCommand("SELECT LAST_INSERT_ID();", connection);
                                                numeroCommande = Convert.ToInt64(getID.ExecuteScalar());
                                                MySqlDataReader reader_getID;
                                                reader_getID = getID.ExecuteReader();
                                                reader_getID.Close();

                                                // Insérer dans `Passer` 
                                                MySqlCommand ajouter_Passer = connection.CreateCommand();
                                                ajouter_Passer.CommandText = "INSERT INTO Passer (Numero_Commande, Adresse_livraison, ID_Client) VALUES (@Numero_Commande, @Adresse_Livraison, @ID_Client)";
                                                ajouter_Passer.Parameters.AddWithValue("@Numero_Commande", numeroCommande);
                                                ajouter_Passer.Parameters.AddWithValue("@Adresse_Livraison", adresseLivraison);
                                                ajouter_Passer.Parameters.AddWithValue("@ID_Client", id_connection_client);

                                                MySqlDataReader reader_ajouter_Passer;
                                                reader_ajouter_Passer = ajouter_Passer.ExecuteReader();
                                                reader_ajouter_Passer.Close();

                                                // Insérer dans `Contient`
                                                MySqlCommand ajouter_Contient = connection.CreateCommand();
                                                ajouter_Contient.CommandText = "INSERT INTO Contient (Numero_Commande, Nom_Plat) VALUES (@Numero_Commande, @Nom_Plat)";
                                                ajouter_Contient.Parameters.AddWithValue("@Numero_Commande", numeroCommande);
                                                ajouter_Contient.Parameters.AddWithValue("@Nom_Plat", Nom_plat_commande);

                                                MySqlDataReader reader_ajouter_Contient;
                                                reader_ajouter_Contient = ajouter_Contient.ExecuteReader();
                                                reader_ajouter_Contient.Close();

                                                // Récupération de l'ID de la commande qui vient d'être créée
                                                int idCommande;
                                                MySqlCommand recupCommande = connection.CreateCommand();
                                                recupCommande.CommandText = "SELECT LAST_INSERT_ID()"; // Récupérer le dernier ID inséré
                                                idCommande = Convert.ToInt32(recupCommande.ExecuteScalar());                                                

                                                Console.WriteLine($"Votre commande n°{idCommande} a bien été enregistrée.");

                                                // Récupération du cuisinier associé au plat
                                                int idCuisinier = -1; // Valeur par défaut
                                                using (MySqlCommand recupCuisinier = connection.CreateCommand())  // Utilisation de 'using' pour assurer que la commande est bien libérée après utilisation
                                                {
                                                    recupCuisinier.CommandText = @"
                                                                                    SELECT U.ID_Client 
                                                                                    FROM Utilisateur U
                                                                                    JOIN Cuisinier C ON U.ID_Client = C.ID_Client
                                                                                    JOIN Cuisine Cu ON C.ID_Client = Cu.ID_Client
                                                                                    WHERE TRIM(LOWER(Cu.Nom_Plat)) = TRIM(LOWER(@Nom_Plat))
                                                                                    LIMIT 1";
                                                    recupCuisinier.Parameters.AddWithValue("@Nom_Plat", Nom_plat_commande);

                                                    using (MySqlDataReader readerCuisinier = recupCuisinier.ExecuteReader())  // S'assurer que le reader est aussi libéré
                                                    {
                                                        if (readerCuisinier.HasRows)
                                                        {
                                                            while (readerCuisinier.Read())
                                                            {
                                                                idCuisinier = readerCuisinier.GetInt32(0);  // Récupère l'ID du cuisinier
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
                                                    for (int i = 0; i < reader_Recuperer_adresse_expedition.FieldCount; i++)    // parcours cellule par cellule
                                                    {
                                                        string valueAsString = reader_Recuperer_adresse_expedition.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                                                        donnes_cuisinier += valueAsString + ", ";
                                                    }
                                                }
                                                string[] detail_cuisinier = donnes_cuisinier.Split(", ");
                                                string adresse_expedition = $"{detail_cuisinier[4]} {detail_cuisinier[3]} {detail_cuisinier[5]} {detail_cuisinier[6]}";
                                                reader_Recuperer_adresse_expedition.Close();

                                            // Insérer dans `Preparer`
                                            MySqlCommand ajouter_Preparer = connection.CreateCommand();
                                            ajouter_Preparer.CommandText = @"
                                                                            INSERT IGNORE INTO Preparer (Numero_Commande, Adresse_expedition, ID_Client)
                                                                            VALUES (LAST_INSERT_ID(), @adresseExpedition, @idCuisinier);
                                                                        ";

                                            ajouter_Preparer.Parameters.AddWithValue("@adresseExpedition", adresse_expedition);
                                            ajouter_Preparer.Parameters.AddWithValue("@idCuisinier", idCuisinier);

                                            // Exécuter la commande
                                            ajouter_Preparer.ExecuteNonQuery();
                                        }
                                        else
                                                {
                                                    Console.WriteLine("Clé introuvable");
                                                }
                                            break;

                                        case ConsoleKey.D2:
                                        case ConsoleKey.NumPad2:
                                            Console.Clear();
                                            Console.WriteLine("Voici la liste de vos commande !");

                                            MySqlCommand Afficher_commande_utilisateur = connection.CreateCommand();
                                            Afficher_commande_utilisateur.CommandText = @"SELECT C.Numero_Commande, C.Prix_commande, C.Quantite, C.Date_Commande, C.Date_Livraison, C.Status
                                                                                            FROM Commande C
                                                                                            JOIN Passer P ON C.Numero_Commande = P.Numero_Commande 
                                                                                            WHERE P.ID_Client = @ID_Client; ";
                                            Afficher_commande_utilisateur.Parameters.AddWithValue("@ID_Client", id_connection_client);

                                            MySqlDataReader reader_Afficher_commande_utilisateur = Afficher_commande_utilisateur.ExecuteReader();
                                            Dictionary<int, string> Liste_commande = new Dictionary<int, string>();
                                            int numero_commande = 1;
                                            while (reader_Afficher_commande_utilisateur.Read())
                                            {
                                                string currentRowAsString = "";
                                                for (int i = 0; i < reader_Afficher_commande_utilisateur.FieldCount; i++)    // parcours cellule par cellule
                                                {
                                                    string valueAsString = reader_Afficher_commande_utilisateur.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                                                    currentRowAsString += valueAsString + ", ";
                                                }
                                                Liste_commande.Add(numero_commande, currentRowAsString);
                                                Console.WriteLine(numero_commande + " : " + currentRowAsString);    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
                                                numero_commande++;
                                            }
                                            Console.WriteLine("Entrez le numéro de la commande que vous souhaitez supprimer");
                                            reader_Afficher_commande_utilisateur.Close();

                                        int numero_commande_a_supprimer = Convert.ToInt32(Console.ReadLine());
                                        if (Liste_commande.TryGetValue(numero_commande_a_supprimer, out string commande))
                                        {
                                            Console.WriteLine("Vous avez choisi de supprimer la commande suivante:");
                                            Console.WriteLine(commande);

                                            string[] details_commande = commande.Split(", ");
                                            int id_commande = Convert.ToInt32(details_commande[0]);

                                            MySqlCommand Supprimer_commande = connection.CreateCommand();
                                            Supprimer_commande.CommandText = @"DELETE FROM Passer 
                                                                               WHERE Numero_Commande = @Numero_Commande AND ID_Client = @ID_Client;
                                                                               DELETE FROM Contient WHERE Numero_Commande = @Numero_Commande;
                                                                               DELETE FROM Preparer WHERE Numero_Commande = @Numero_Commande;
                                                                               DELETE FROM Commande WHERE Numero_Commande = @Numero_Commande;";

                                            // Ajouter les paramètres correctement à Supprimer_commande
                                            Supprimer_commande.Parameters.AddWithValue("@ID_Client", id_connection_client);
                                            Supprimer_commande.Parameters.AddWithValue("@Numero_Commande", id_commande);

                                            MySqlDataReader reader_Supprimer_commande_utilisateur = Supprimer_commande.ExecuteReader();
                                            reader_Supprimer_commande_utilisateur.Close();
                                            Console.WriteLine($"La commande {id_commande} a été supprimée avec succès");
                                        }

                                        Pause();
                                        break;

                                        case ConsoleKey.D3:
                                        case ConsoleKey.NumPad3:
                                            Console.Clear();
                                            Console.WriteLine("Voici la liste de vos commande !");

                                            MySqlCommand Afficher_commande_utilisateur_2 = connection.CreateCommand();
                                            Afficher_commande_utilisateur_2.CommandText = @"SELECT C.Numero_Commande, C.Prix_commande, C.Quantite, C.Date_Commande, C.Date_Livraison, C.Status
                                                                                                FROM Commande C
                                                                                                JOIN Passer P ON C.Numero_Commande = P.Numero_Commande 
                                                                                                WHERE P.ID_Client = @ID_Client; ";
                                            Afficher_commande_utilisateur_2.Parameters.AddWithValue("@ID_Client", id_connection_client);

                                            MySqlDataReader reader_Afficher_commande_utilisateur_2 = Afficher_commande_utilisateur_2.ExecuteReader();
                                            while (reader_Afficher_commande_utilisateur_2.Read())
                                            {
                                                string currentRowAsString = "";
                                                for (int i = 0; i < reader_Afficher_commande_utilisateur_2.FieldCount; i++)    // parcours cellule par cellule
                                                {
                                                    string valueAsString = reader_Afficher_commande_utilisateur_2.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                                                    currentRowAsString += valueAsString + ", ";
                                                }
                                                Console.WriteLine(currentRowAsString);
                                            }
                                            Console.WriteLine("Entrez le numéro de la commande que vous souhaitez supprimer");
                                            reader_Afficher_commande_utilisateur_2.Close();
                                        break;

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



                    case ConsoleKey.D3: // Supposons que D3 corresponde à "Connexion en tant que Cuisinier"
                    case ConsoleKey.NumPad3:
                        Console.Clear();
                        Console.WriteLine("Connexion Cuisinier");

                        // Saisie des identifiants
                        Console.WriteLine("Quel est votre identifiant?");
                        int id_connection_cuisinier = Convert.ToInt32(Console.ReadLine());

                        Console.WriteLine("Quel est votre mot de passe?");
                        string mdp_connexion_cuisinier = Console.ReadLine();

                        // Vérifier si l'ID existe dans la table des utilisateurs
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

                            // Vérifier si l'utilisateur est un cuisinier et récupérer son mot de passe
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

                                    // Récupération du nom et prénom du cuisinier
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

                                    Console.WriteLine($"Bienvenue, Chef {prenom_Affiche} {nom_Affiche}");
                                    Console.WriteLine();
                                    Console.WriteLine("1: Voir les commandes à préparer");
                                    Console.WriteLine("2: Mettre à jour le statut d'une commande");
                                    Console.WriteLine("3: Ajouter un plat à votre menu");

                                    ConsoleKeyInfo cki_cuisinier = Console.ReadKey(true);
                                    switch (cki_cuisinier.Key)
                                    {
                                        case ConsoleKey.D1:
                                        case ConsoleKey.NumPad1:
                                            Console.Clear();
                                            Console.WriteLine("Liste des commandes à préparer :");

                                            MySqlCommand recup_commandes = connection.CreateCommand();
                                            recup_commandes.CommandText = "SELECT c.Numero_Commande, c.Date_Commande, c.Status FROM Commande c JOIN Preparer g ON c.Numero_Commande = g.Numero_Commande WHERE g.ID_Client = @ID_Client AND c.Status != 'Livrée'";
                                            recup_commandes.Parameters.AddWithValue("@ID_Client", id_connection_cuisinier);                                           
                                        
                                        
                                        
                                            MySqlDataReader reader_commandes = recup_commandes.ExecuteReader();
                                            while (reader_commandes.Read())
                                            {
                                                Console.WriteLine($"Commande {reader_commandes.GetInt32(0)} - Passée le {reader_commandes.GetDateTime(1)} - Statut : {reader_commandes.GetString(2)}");
                                            }
                                            reader_commandes.Close();
                                            break;

                                        case ConsoleKey.D2:
                                        case ConsoleKey.NumPad2:

                                            Console.WriteLine("Liste des commandes à préparer :");

                                            MySqlCommand recup_liste_commandes = connection.CreateCommand();
                                            recup_liste_commandes.CommandText = "SELECT c.Numero_Commande, c.Date_Commande, c.Status FROM Commande c JOIN Preparer g ON c.Numero_Commande = g.Numero_Commande WHERE g.ID_Client = @ID_Client AND c.Status != 'Livrée'";
                                            recup_liste_commandes.Parameters.AddWithValue("@ID_Client", id_connection_cuisinier);



                                            MySqlDataReader reader_liste_commandes = recup_liste_commandes.ExecuteReader();
                                            while (reader_liste_commandes.Read())
                                            {
                                                Console.WriteLine($"Commande {reader_liste_commandes.GetInt32(0)} - Passée le {reader_liste_commandes.GetDateTime(1)} - Statut : {reader_liste_commandes.GetString(2)}");
                                            }
                                            reader_liste_commandes.Close();

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

                                            if (reader_checkPlat.Read())  // Lire la première ligne
                                            {
                                                count = reader_checkPlat.GetInt32(0);  // Récupérer la valeur du COUNT(*)
                                            }
                                            reader_checkPlat.Close(); // Ne pas oublier de fermer le reader !
                                            if (count > 0)
                                            {
                                                while (count > 0) {
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
                                                MySqlCommand ajouterPlat = connection.CreateCommand();
                                                ajouterPlat.CommandText = @"
                                                        INSERT INTO Plat (Nom_Plat, Date_Fabrication, Date_Peremption, Prix_plat, Regime, Origine, Type_plat, Nombre_personne, Image)
                                                        VALUES (@Nom_Plat, @Date_Fabrication, @Date_Peremption, @Prix_plat, @Regime, @Origine, @Type_plat, @Nombre_personne, @Image)";

                                                ajouterPlat.Parameters.AddWithValue("@Nom_Plat", nomPlat);
                                                ajouterPlat.Parameters.AddWithValue("@Date_Fabrication", DateTime.Today);
                                                ajouterPlat.Parameters.AddWithValue("@Date_Peremption", DateTime.Now.AddDays(5));
                                                ajouterPlat.Parameters.AddWithValue("@Prix_plat", prixPlat);
                                                ajouterPlat.Parameters.AddWithValue("@Regime", regime);
                                                ajouterPlat.Parameters.AddWithValue("@Origine", typeCuisine);
                                                ajouterPlat.Parameters.AddWithValue("@Type_plat", type_repas);
                                                ajouterPlat.Parameters.AddWithValue("@Nombre_personne", nb_personne);
                                                ajouterPlat.Parameters.AddWithValue("@Image", 1);
                                                ajouterPlat.ExecuteNonQuery();
                                            }
                                            else
                                            {
                                                Console.WriteLine("Le plat existe déjà.");
                                            }
                                            // Étape 2 : Associer le plat au cuisinier 
                                            MySqlCommand assignerCuisinier = connection.CreateCommand();
                                            assignerCuisinier.CommandText = "INSERT IGNORE INTO Cuisine (ID_Client, Nom_Plat) VALUES (@ID_Client, @Nom_Plat)";
                                            assignerCuisinier.Parameters.AddWithValue("@ID_Client", id_connection_cuisinier);
                                            assignerCuisinier.Parameters.AddWithValue("@Nom_Plat", nomPlat);
                                            assignerCuisinier.ExecuteNonQuery();
                                            Console.WriteLine("Plat ajouté avec succès !");
                                            Pause();
                                            break;

                                        case ConsoleKey.D5:
                                        case ConsoleKey.NumPad5:
                                            Console.Clear();
                                            Console.WriteLine("Modification de votre profil :");
                                            Console.WriteLine("Nouveau prénom ?");
                                            string nouveauPrenom = Console.ReadLine();

                                            Console.WriteLine("Nouveau numéro de téléphone ?");
                                            int nouveauTel = Convert.ToInt32(Console.ReadLine());

                                            MySqlCommand update_profil = connection.CreateCommand();
                                            update_profil.CommandText = "UPDATE Utilisateur SET Prenom = @Prenom, Telephone = @Telephone WHERE ID_Client = @ID_Client";
                                            update_profil.Parameters.AddWithValue("@Prenom", nouveauPrenom);
                                            update_profil.Parameters.AddWithValue("@Telephone", nouveauTel);
                                            update_profil.Parameters.AddWithValue("@ID_Client", id_connection_cuisinier);

                                            update_profil.ExecuteNonQuery();
                                            Console.WriteLine("Profil mis à jour avec succès !");
                                            break;
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

                        string baseDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
                        string Emplacement_fichiers = baseDirectory;
                        Directory.CreateDirectory(Emplacement_fichiers);

                        // Instancier GraphBuilder
                        var builder = new GraphBuilder();
                        var graph = builder.BuildGraph("Noeuds.csv", "Arcs.csv");

                        // Chemin de sortie pour le fichier texte
                        var outputPath = Path.Combine(Emplacement_fichiers, "metro_graph.txt");
                        var directory = Path.GetDirectoryName(outputPath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        // Sauvegarder le graphe dans un fichier texte
                        builder.SaveGraphToFile(graph, outputPath);

                        string metroGraphPath = outputPath; // Remplacez par le chemin réel de votre fichier texte

                        // Créer un GraphBuilder pour construire le graphe à partir du fichier texte
                        var graphBuilder = new GraphBuilder();

                        // Construire le graphe à partir du fichier texte
                        var graphique = graphBuilder.BuildGraphFromText(metroGraphPath);

                        // Afficher les stations et connexions pour débogage
                        foreach (var station in graph.Values)
                        {
                            Console.WriteLine($"Station {station.Id}: {station.Name} ({station.Longitude}, {station.Latitude})");
                            foreach (var connexion in station.Connections)
                            {
                                Console.WriteLine($"    → Station {connexion.TargetStationId} (Travel: {connexion.TravelTime} min, Change: {connexion.ChangeTime} min)");
                            }
                        }
                        // Générer l'image du graphe au format PNG

                        string chemin_image = Path.Combine(Emplacement_fichiers, "carte_metro.png");
                        graphBuilder.GenerateGraphvizImage(graph, chemin_image);

                        Console.WriteLine($"Image générée ici {chemin_image}");

                    Pause();
                    break;


                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:

                    Console.Clear();
                    Console.WriteLine("Génération du graphe");
                    string dotFile = "graph.dot";
                    GenerateDotFile(Dico_adjacence_affichage, dotFile);
                    // Appeler Graphviz pour générer l'image à partir du fichier DOT
                    Console.WriteLine("veuillez patienter, génération du fichier PNJ..."); 
                    GenerateGraphImage(dotFile);
                    Console.WriteLine("\nFichier généré avec succès, cliquer pour quitter");
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

    //Fonction qui prends les noeux du csv et les met dans un tableau 
    //(donc les stations par lesquelles passent deux lignes sont représentées par deux noeuds distincts)
    static Noeud[] Noeud_depuis_csv(string nom_du_csv)
    {
        List<Noeud> NoeudsMetro = new List<Noeud>();
        string[] lignes = File.ReadAllLines(nom_du_csv);

        //import des données, on ne prend pas la première ligne du tableau
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




    //Fonction qui renvoie un tableau avec les liens du csv,
    //(donc pour l'instant, les stations à deux lignes étant deux noeuds, on a un graphe non connexe,
    //car les lignes différentes ne se touchent pas)
    static Lien[] Lien_depuis_csv(string nom_du_csv)
    {
        List<Lien> LienMetro = new List<Lien>();
        string[] lignes = File.ReadAllLines(nom_du_csv);

        //import des données du csv, on ignore encore la première ligne
        for (int i = 1; i < lignes.Length; i++)
        {
            string[] tempo = lignes[i].Split(";");
            int id_station = Convert.ToInt32(tempo[0]);
            string nom = tempo[1];
            int id_precedent;
            //-1 si station de départ d'une ligne
            if (string.IsNullOrEmpty(tempo[2]))
            { id_precedent = -1; }
            else
            { id_precedent = Convert.ToInt32(tempo[2]); }
            int id_suivant;
            //-1 si station d'arrivée d'une ligne
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

    //Fonction qui créee un dictionnaire ou les stations en doublons sont une unique clé,
    //Dictionnaire destiné à l'affichage uniquement, car ne prend pas en compte les changements. 
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
                    dico_listes[l.nom].Add(infos_noeuds[l.id_precedent - 1].Libelle_station); //ajout du nom de la station prec
                }
                if (0 < l.id_suivant - 1 && l.id_suivant - 1 < infos_noeuds.Length)
                {
                    dico_listes[l.nom].Add(infos_noeuds[l.id_suivant - 1].Libelle_station);  //ajout du nom de la station suiv
                }
            }
            //si station déjà présente sur une autre ligne, on rajoute les correspondances de la ligne actuelle
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
        //on souvegarde les adjacences dans des tables plutot que des listes
        Dictionary<string, string[]> dico_tableaux = new Dictionary<string, string[]>();
        foreach (var element in dico_listes)
        {
            dico_tableaux[element.Key] = element.Value.ToArray();
        }
        Console.WriteLine($"{dico_tableaux.Count()} données graphiques des liens traitées");
        return dico_tableaux;
    }

    //Crée la liste d'adjacence réelle, où les stations où passent plusieurs lignes comptent double
    static Dictionary<Noeud, Noeud[]> Dico_ajd_reel(Lien[] liensgen, Noeud[] infos_noeuds)
    {
        Dictionary<Noeud, List<Noeud>> dico_listes = new Dictionary<Noeud, List<Noeud>>();
        foreach (Lien l in liensgen)
        {
            dico_listes[infos_noeuds[l.id_station - 1]] = new List<Noeud>();
            if (0 < l.id_precedent - 1 && l.id_precedent - 1 < infos_noeuds.Length)
            {
                dico_listes[infos_noeuds[l.id_station - 1]].Add(infos_noeuds[l.id_precedent - 1]); //ajout du précédent
            }
            if (0 < l.id_suivant - 1 && l.id_suivant - 1 < infos_noeuds.Length)
            {
                dico_listes[infos_noeuds[l.id_station - 1]].Add(infos_noeuds[l.id_suivant - 1]); //ajout du suivant 
            }
            //ajout des correspondances (autres noeuds qui ont le même nom)
            foreach (Noeud n in infos_noeuds)
            {
                if (l.nom == n.libelle_station &&
                    0 < l.id_suivant - 1 && l.id_suivant - 1 < infos_noeuds.Length)
                {
                    dico_listes[infos_noeuds[l.id_station - 1]].Add(n);
                }
            }
        }

        //on souvegarde les adjacences dans des tables plutot que des listes
        Dictionary<Noeud, Noeud[]> dico_tableaux = new Dictionary<Noeud, Noeud[]>();
        foreach (var element in dico_listes)
        {
            dico_tableaux[element.Key] = element.Value.ToArray();
        }
        Console.WriteLine($"{dico_tableaux.Count()} données des liens traitées");
        return dico_tableaux;
    }

    //Fonction du calcul de la distance de Haversine
    public static double DistanceHaversine(Noeud noeud1, Noeud noeud2)
    {
        const double R = 6371.0; //rayon terrestre
        // Conversion des coordonnées en radians
        double lat1 = noeud1.latitude * Math.PI / 180.0;
        double lon1 = noeud1.longitude * Math.PI / 180.0;
        double lat2 = noeud2.latitude * Math.PI / 180.0;
        double lon2 = noeud2.longitude * Math.PI / 180.0;

        double diffLat = lat2 - lat1;
        double diffLon = lon2 - lon1;

        double a = Math.Pow(Math.Sin(diffLat / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(diffLon / 2), 2);
        double c = 2 * Math.Asin(Math.Sqrt(a));

        return R * c; // Distance en km
    }


    //Fonction d'affichage d'un trajet
    //toujours le "liencsv" initial qui contient les infos de base
    //"noeuds_trajet" sera la liste des noeuds à emprunter sur le chemin
    static void AfficherTrajet(List<Noeud> noeuds_trajet, Lien[] lienscsv)
    {
        int duree = 0;
        double distance = 0;
        string message = "Trajet à emprunter : \n ";
        message += $"{noeuds_trajet[0].libelle_station} -> ";
        for (int i = 1; i < noeuds_trajet.Count - 1; i++)
        {
            if (noeuds_trajet[i].libelle_station != noeuds_trajet[i - 1].libelle_station) //=on reste sur la meme ligne
            {
                message += $"{noeuds_trajet[i].libelle_station} -> ";
                duree += lienscsv[noeuds_trajet[i].id_station - 1].temps_deux_stations;
                distance += DistanceHaversine(noeuds_trajet[i], noeuds_trajet[i - 1]);
            }
            else //si on fait un changement
            {
                message += $"\nChanger pour la ligne {noeuds_trajet[i].libelle_ligne} -> ";
                duree += lienscsv[noeuds_trajet[i].id_station - 1].temps_deux_stations;
            }
        }
        //dernier arrêt pour ne pas avoir une flèche en trop
        message += $"{noeuds_trajet[noeuds_trajet.Count - 1].libelle_station} -> ";
        duree += lienscsv[noeuds_trajet[noeuds_trajet.Count - 1].id_station - 1].temps_deux_stations;


        //enlever la flèche à la fin
        message = message.Substring(0, message.Length - 3);
        message += $"\n\nDurée : {duree} min \n Distance : {distance} km";

        Console.WriteLine(message);
    }


    //-------------------------------------------------DEBUT DES PROBLEMES------------------------------------------------------


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

    //ALGO PCC -- DJIKSTRA
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
            file.Sort((a, b) => distances[a].CompareTo(distances[b]));
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
        return int.MaxValue; // pas de lien direct trouvé
    }





    //ALGO PCC -- BELLMAN-FORD
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

        // Bellman-Ford classique
        for (int i = 0; i < nbNoeuds - 1; i++)
        {
            foreach (Lien lien in liens)
            {
                Noeud u = infos_noeuds[lien.id_station - 1];
                if (!liste_adj.ContainsKey(u)) continue;

                foreach (Noeud v in liste_adj[u])
                {
                    int poids = (u.Libelle_station == v.Libelle_station) ? lien.Temps_changement : lien.Temps_deux_stations;

                    if (distance[u] != int.MaxValue && distance[u] + poids < distance[v])
                    {
                        distance[v] = distance[u] + poids;
                        precedent[v] = u;
                    }
                }
            }
        }

        // Reconstruction du chemin
        List<Noeud> chemin = new List<Noeud>();
        Noeud? current = arrivee;

        while (current != null)
        {
            chemin.Insert(0, current);
            current = precedent[current];
        }

        // Si le départ n’est pas en tête => pas de chemin trouvé
        if (chemin.Count == 0 || chemin[0] != depart)
            return new List<Noeud>();

        return chemin;
    }



    //ALGO PCC -- FLOYD-WARSHALL
    static List<Noeud> Pcc_Floyd_Warshall(Noeud depart, Noeud arrivee, Dictionary<Noeud, Noeud[]> liste_adj, Lien[] liens, Noeud[] infos_noeuds)
    {
        int n = infos_noeuds.Length;

        // Création des index pour accéder facilement aux noeuds
        Dictionary<Noeud, int> noeudToIndex = new Dictionary<Noeud, int>();
        Dictionary<int, Noeud> indexToNoeud = new Dictionary<int, Noeud>();

        for (int i = 0; i < n; i++)
        {
            noeudToIndex[infos_noeuds[i]] = i;
            indexToNoeud[i] = infos_noeuds[i];
        }

        // Initialisation des matrices de distances et de prédecesseurs
        int[,] dist = new int[n, n];
        int?[,] pred = new int?[n, n];

        // Initialisation des distances infinies sauf pour les voisins directs
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
                    dist[i, j] = int.MaxValue / 2; // Pour éviter dépassement en addition
                    pred[i, j] = null;
                }
            }
        }

        // Remplissage initial avec les poids des arcs
        foreach (Lien lien in liens)
        {
            int u = lien.id_station - 1;
            Noeud noeudU = infos_noeuds[u];

            if (!liste_adj.ContainsKey(noeudU))
                continue;

            foreach (Noeud voisin in liste_adj[noeudU])
            {
                int v = noeudToIndex[voisin];
                int poids = (noeudU.Libelle_station == voisin.Libelle_station) ? lien.Temps_changement : lien.Temps_deux_stations;

                dist[u, v] = poids;
                pred[u, v] = u;
            }
        }

        // Algorithme de Floyd-Warshall
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

        // Reconstruction du chemin de depart à arrivee
        int indexDepart = noeudToIndex[depart];
        int indexArrivee = noeudToIndex[arrivee];

        if (dist[indexDepart, indexArrivee] >= int.MaxValue / 2)
        {
            return new List<Noeud>(); // Aucun chemin trouvé
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


    //Changer le libelle_ligne du noeud en string et adapter les fonctions qui appellent
    //Fait





    //afficher les noeuds
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

    //afficher les liens
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

    //afficher la liste d'adj 
    static void AfficherListeAdjacence(Dictionary<Noeud, Noeud[]> dicoAdj)
    {
        foreach (var paire in dicoAdj)
        {
            Noeud noeudPrincipal = paire.Key;
            Noeud[] adjacents = paire.Value;

            string ligne = $"{noeudPrincipal.Libelle_station} : ";
            ligne += string.Join(", ", adjacents.Select(n => n.Libelle_station));

            Console.WriteLine(ligne);
        }
    }


    //Affichage du graphe 
    // Générer le fichier DOT à partir de la matrice d'adjacence
    static void GenerateDotFile(Dictionary<string, string[]> matriceAdjacence, string dotFile)
    {
        using (StreamWriter sw = new StreamWriter(dotFile))
        {
            // Spécifier le format de graphique pour être horizontal
            sw.WriteLine("graph G {");

            // Spécifier la direction du graphe (horizontal)
            sw.WriteLine("    rankdir=LR;");

            // Paramètres pour rendre les stations plus proches
            sw.WriteLine("    nodesep=0.5;");  // Réduire l'espace entre les noeuds
            sw.WriteLine("    ranksep=0.5;");  // Réduire l'espace entre les rangs (couches de noeuds)

            // Ajouter les noeuds et les liens dans le fichier DOT
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

    // Appeler Graphviz pour générer l'image à partir du fichier DOT
    static void GenerateGraphImage(string dotFile)
    {
        string outputImage = "graph.png"; // L'image de sortie

        // Vérifie que Graphviz est installé et accessible
        string graphvizPath = @"C:\Program Files (x86)\Graphviz\bin\dot.exe"; // Ajuste ce chemin si nécessaire

        if (!File.Exists(graphvizPath))
        {
            Console.WriteLine("Graphviz n'est pas installé ou son chemin n'est pas valide.");
            return;
        }

        // Appel à Graphviz via la ligne de commande pour générer l'image
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
}
