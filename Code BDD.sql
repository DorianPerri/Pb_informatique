DROP DATABASE IF EXISTS Application;
CREATE DATABASE IF NOT EXISTS Application;
USE Application;

CREATE TABLE Commande(
   Numero_Commande INT AUTO_INCREMENT,
   Prix_commande DECIMAL(15,2),
   Quantite INT,
   Date_Commande DATETIME,
   Date_Livraison DATETIME,
   Status VARCHAR(50),
   PRIMARY KEY(Numero_Commande)
);

CREATE TABLE Plat(
   Nom_Plat VARCHAR(50),
   Date_Fabrication DATE,
   Date_Peremption VARCHAR(50),
   Prix_plat DECIMAL(15,2),
   Regime VARCHAR(50),
   Origine VARCHAR(50),
   Type_plat VARCHAR(50),
   Nombre_personne INT,
   Image INT,
   PRIMARY KEY(Nom_Plat)
);

CREATE TABLE Ingredient(
   Nom_Ingredient VARCHAR(50),
   Prix_au_kilo DECIMAL(15,2),
   Origine_ingredient VARCHAR(50),
   Categorie VARCHAR(50),
   Quantite_stockee DECIMAL(15,2),
   Seuil DECIMAL(15,2),
   PRIMARY KEY(Nom_Ingredient)
);

CREATE TABLE Utilisateur(
   ID_Client INT AUTO_INCREMENT,
   Nom VARCHAR(50),
   Prenom VARCHAR(50),
   Rue VARCHAR(50),
   Numero INT,
   Code_Postal INT,
   Ville VARCHAR(50),
   Telephone INT,
   Email VARCHAR(50),
   Metro_Proche VARCHAR(50),
   Type_client VARCHAR(50),
   Mot_De_Passe VARCHAR(100) NOT NULL,
   PRIMARY KEY(ID_Client)

);

CREATE TABLE Cuisinier(
   ID_Client INT,
   Prenom_cuisinier VARCHAR(50),
   Nombre_commande_livrer BIGINT,
   Note_moyenne_cuisinier DECIMAL(15,2),
   PRIMARY KEY(ID_Client),
   FOREIGN KEY(ID_Client) REFERENCES Utilisateur(ID_Client)
);

CREATE TABLE Client(
   ID_Client INT,
   Date_inscription DATE,
   Nombre_commande INT,
   PRIMARY KEY(ID_Client),
   FOREIGN KEY(ID_Client) REFERENCES Utilisateur(ID_Client)
);

CREATE TABLE Entreprise(
   ID_Client INT,
   Referent VARCHAR(50),
   telephone_ref INT,
   mail_ref VARCHAR(50),
   PRIMARY KEY(ID_Client),
   FOREIGN KEY(ID_Client) REFERENCES Client(ID_Client)
);

CREATE TABLE Particulier(
   ID_Client INT,
   Prenom VARCHAR(50),
   PRIMARY KEY(ID_Client),
   FOREIGN KEY(ID_Client) REFERENCES Client(ID_Client)
);

CREATE TABLE Contient(
   Numero_Commande INT,
   Nom_Plat VARCHAR(50),
   PRIMARY KEY(Numero_Commande, Nom_Plat),
   FOREIGN KEY(Numero_Commande) REFERENCES Commande(Numero_Commande),
   FOREIGN KEY(Nom_Plat) REFERENCES Plat(Nom_Plat)
);

CREATE TABLE Composer(
   Nom_Plat VARCHAR(50),
   Nom_Ingredient VARCHAR(50),
   Quantite DECIMAL(15,2),
   PRIMARY KEY(Nom_Plat, Nom_Ingredient),
   FOREIGN KEY(Nom_Plat) REFERENCES Plat(Nom_Plat),
   FOREIGN KEY(Nom_Ingredient) REFERENCES Ingredient(Nom_Ingredient)
);

CREATE TABLE Cuisine(
   ID_Client INT,
   Nom_Plat VARCHAR(50),
   PRIMARY KEY(ID_Client, Nom_Plat),
   FOREIGN KEY(ID_Client) REFERENCES Cuisinier(ID_Client),
   FOREIGN KEY(Nom_Plat) REFERENCES Plat(Nom_Plat)
);

CREATE TABLE Evaluer(
   ID_Client INT,
   ID_Client_1 INT,
   Note DECIMAL(15,2),
   Commente TEXT,
   PRIMARY KEY(ID_Client, ID_Client_1),
   FOREIGN KEY(ID_Client) REFERENCES Cuisinier(ID_Client),
   FOREIGN KEY(ID_Client_1) REFERENCES Client(ID_Client)
);

CREATE TABLE Passer(
   Numero_Commande INT,
   Adresse_livraison VARCHAR(50),
   ID_Client INT NOT NULL,
   PRIMARY KEY(Numero_Commande),
   FOREIGN KEY(Numero_Commande) REFERENCES Commande(Numero_Commande),
   FOREIGN KEY(ID_Client) REFERENCES Client(ID_Client)
);

CREATE TABLE Preparer(
   Numero_Commande INT,
   Adresse_expedition VARCHAR(50),
   ID_Client INT NOT NULL,
   PRIMARY KEY(Numero_Commande),
   FOREIGN KEY(Numero_Commande) REFERENCES Commande(Numero_Commande),
   FOREIGN KEY(ID_Client) REFERENCES Cuisinier(ID_Client)
);

-- Pour régler les problèmes de sécurité workbench
SET GLOBAL local_infile = 1;
-- SHOW VARIABLES LIKE 'secure_file_priv';

-- SHOW VARIABLES LIKE 'local_infile';
-- show VARIABLES LIKE "%secure_file%"; 
-- SET SECURE_FILE_PRIV="";
-- SET GLOBAL local_infile = 1;


-- IMPORTATION des Clients 
-- Table temporaire pour avoir un nombre de colonnes correspondantes au CSV
CREATE TABLE Temp_Client (
   Nom VARCHAR(50),
   Prenom VARCHAR(50),
   Rue VARCHAR(50),
   Numero INT,
   Code_Postal INT,
   Ville VARCHAR(50),
   Telephone VARCHAR(50),
   Email VARCHAR(50),
   Metro_Proche VARCHAR(50)
);
LOAD DATA INFILE "C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\Clients_csv.csv"
INTO TABLE Temp_Client
FIELDS TERMINATED BY ';'
(@dummy, Nom, Prenom, Rue, Numero, Code_Postal, Ville, Telephone, Email, Metro_Proche);
-- On transpose dans utilisateur les données corresp.
INSERT INTO Utilisateur (Nom, Prenom, Rue, Numero, Code_Postal, Ville, Telephone, Email, Metro_Proche, Type_client, Mot_De_Passe)
SELECT Nom, Prenom, Rue, Numero, Code_Postal, Ville, Telephone, Email, Metro_Proche, 'client', 'default' FROM Temp_Client;
-- Puis dans client
INSERT INTO Client (ID_Client, Date_inscription, Nombre_commande)
SELECT ID_Client, '2025-02-28', 0 FROM Utilisateur WHERE Type_client = 'client';
--  Plus besoin de la table tempo
DROP TABLE Temp_Client;
-- Verif
SELECT * FROM client;
SELECT * FROM Utilisateur; 


-- IMPORTATION des Cuisiniers  
-- Table temporaire pour avoir un nombre de colonnes correspondantes au CSV
DROP TABLE IF EXISTS Temp_Cuisinier;
CREATE TABLE Temp_Cuisinier (
    Nom VARCHAR(50),
    Prenom VARCHAR(50),
    Rue VARCHAR(50),
    Numero INT,
    Code_Postal INT,
    Ville VARCHAR(50),
    Telephone VARCHAR(20),
    Email VARCHAR(50),
    Metro_Proche VARCHAR(50)
);
LOAD DATA INFILE "C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\Cuisiniers_csv.csv"
INTO TABLE Temp_Cuisinier
FIELDS TERMINATED BY ';'
(@dummy, Nom, Prenom, Rue, Numero, Code_Postal, Ville, Telephone, Email, Metro_Proche);
-- On transpose dans utilisateur les données corresp.
INSERT INTO Utilisateur (Nom, Prenom, Rue, Numero, Code_Postal, Ville, Telephone, Email, Metro_Proche, Type_client, Mot_De_Passe)
SELECT Nom, Prenom, Rue, Numero, Code_Postal, Ville, Telephone, Email, Metro_Proche, 'cuisinier', '' FROM Temp_Cuisinier;
-- Puis dans cuisinier (pas de beug normalement)
INSERT INTO Cuisinier (ID_Client, Prenom_cuisinier, Nombre_commande_livrer, Note_moyenne_cuisinier)
SELECT ID_Client, Prenom, 0, 0.0 FROM Utilisateur WHERE Type_client = 'cuisinier';
--  Plus besoin de la table tempo
DROP TABLE Temp_Cuisinier;
-- Verif
SELECT * FROM Utilisateur WHERE Type_client = 'cuisinier';
SELECT * FROM Cuisinier;


-- IMPORTATION des Commandes  
-- Table temporaire pour avoir un nombre de colonnes correspondantes au CSV
DROP TABLE IF EXISTS Temp_Commandes;
CREATE TABLE Temp_Commandes (
    Numero_Commande INT,
    Numero_Cuisinier INT,
    Numero_Client INT,
    Nom_Plat VARCHAR(50),
    Prix DECIMAL(15,2),
    Quantite INT,
    Type_Plat VARCHAR(50),
    Date_Fabrication VARCHAR(10),
    Date_Peremption VARCHAR(10),
    Regime VARCHAR(50),
    Origine VARCHAR(50),
    Ingredient_1 VARCHAR(50),
    Volume_1 VARCHAR(10),
    Ingredient_2 VARCHAR(50),
    Volume_2 VARCHAR(10),
    Ingredient_3 VARCHAR(50),
    Volume_3 VARCHAR(10),
    Ingredient_4 VARCHAR(50),
    Volume_4 VARCHAR(10)
);
LOAD DATA INFILE "C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\Commandes_csv.csv"
INTO TABLE Temp_Commandes
FIELDS TERMINATED BY ';'
(@dummy, Numero_Cuisinier, Numero_Client, Nom_Plat, Prix, Quantite, Type_Plat, 
 Date_Fabrication, Date_Peremption, Regime, Origine, 
 Ingredient_1, Volume_1, Ingredient_2, Volume_2, Ingredient_3, Volume_3, Ingredient_4, Volume_4); 
 
 -- Passage des dates du format string à date (safe mode bloque)
SET SQL_SAFE_UPDATES = 0;
UPDATE Temp_Commandes 
SET Date_Fabrication = STR_TO_DATE(Date_Fabrication, '%d/%m/%Y'),
    Date_Peremption = STR_TO_DATE(Date_Peremption, '%d/%m/%Y');
    
ALTER TABLE Temp_Commandes 
MODIFY Date_Fabrication DATE, 
MODIFY Date_Peremption DATE;


-- On transpose dans commande les données corresp.
INSERT INTO Commande (Prix_Commande, Quantite, Date_Commande, Date_Livraison, Status)
SELECT Prix, Quantite, CURDATE(), Date_Peremption, 'default'
FROM Temp_Commandes;

-- On ajoute les données dans ingrédients si non null,
-- mais si on en a déjà un, alors on augmente juste la quantité
INSERT INTO Ingredient (Nom_Ingredient, Quantite_stockee)
SELECT Ingredient_1, REPLACE(Volume_1, 'g', '') FROM Temp_Commandes WHERE Ingredient_1 IS NOT NULL
ON DUPLICATE KEY UPDATE Quantite_stockee = Quantite_stockee + REPLACE(Volume_1, 'g', '');

INSERT INTO Ingredient (Nom_Ingredient, Quantite_stockee)
SELECT Ingredient_2, REPLACE(Volume_2, 'g', '') FROM Temp_Commandes WHERE Ingredient_2 IS NOT NULL
ON DUPLICATE KEY UPDATE Quantite_stockee = Quantite_stockee + REPLACE(Volume_2, 'g', '');

INSERT INTO Ingredient (Nom_Ingredient, Quantite_stockee)
SELECT Ingredient_3, REPLACE(Volume_3, 'g', '') FROM Temp_Commandes WHERE Ingredient_3 IS NOT NULL
ON DUPLICATE KEY UPDATE Quantite_stockee = Quantite_stockee + REPLACE(Volume_3, 'g', '');

-- Pb avec 4 à voir, à cause du ";;" quand pas de 4eme ingerdient (donc pb pour 2,3 aussi)

-- Supp la table tempo
DROP TABLE IF EXISTS Temp_Commandes;
SELECT * FROM Commande;
SELECT * FROM Ingredient; 

-- INSERTION DES CLIENTS
INSERT INTO Utilisateur (ID_Client, Nom, Prenom, Rue, Numero, Code_Postal, Ville, Telephone, Email, Metro_Proche, Type_client, Mot_De_Passe)
VALUES
(2875652, 'Guérain', 'Tom', 'rue des Moines', 4, 75017, 'Paris', 0612345678, 'tom.guerain@example.com', 'Brochant', 'client','123'),
(1234567, 'Dupont', 'Antoine', 'allée des seigneurs', 65, 75001, 'Paris', 0698765432, 'antoine.dupont@example.com', 'Champs-Elysées - Clemenceau', 'client', '123');

-- Association à la table Client
INSERT INTO Client (ID_Client, Date_inscription, Nombre_commande)
VALUES
(2875652, CURDATE(), 0),
(1234567, CURDATE(), 0);

-- Association à la table Particulier
INSERT INTO Particulier (ID_Client, Prenom)
VALUES
(2875652, 'Tom'),
(1234567, 'Antoine');

-- INSERTION DES CUISINIERS
INSERT INTO Utilisateur (ID_Client, Nom, Prenom, Rue, Numero, Code_Postal, Ville, Telephone, Email, Metro_Proche, Type_client, Mot_De_Passe)
VALUES
(7654321, 'Etchebest', 'Phillipe', 'rue des angles', 89, 75017, 'Paris', 0678954321, 'phillipe.etchebest@example.com', 'Concorde', 'cuisinier', '123'),
(7896524, 'Diop', 'Karim', 'rue des boulangers', 45, 75003, 'Paris', 0687412569, 'karim.diop@example.com', 'Olympiades', 'cuisinier', '123');

-- Association à la table Cuisinier
INSERT INTO Cuisinier (ID_Client, Prenom_cuisinier, Nombre_commande_livrer, Note_moyenne_cuisinier)
VALUES
(7654321, 'Phillipe', 0, 0.0),
(7896524, 'Karim', 0, 0.0);

INSERT INTO Plat (Nom_Plat, Date_Fabrication, Date_Peremption, Prix_plat, Regime, Origine, Type_plat, Nombre_personne, Image)
VALUES ('Quiche', CURDATE(), '2025-01-01', 18.00, '', 'française', 'plat', 2, 0);

INSERT INTO Cuisine (ID_Client, Nom_Plat)
VALUES (7654321, 'Quiche');

-- Plats pour Phillipe
INSERT INTO Plat (Nom_Plat, Date_Fabrication, Date_Peremption, Prix_plat, Regime, Origine, Type_plat, Nombre_personne, Image)
VALUES 
('Lasagnes', CURDATE(), '2025-12-31', 15.50, 'standard', 'italienne', 'plat', 2, 0),
('Tartiflette', CURDATE(), '2025-12-31', 17.00, 'standard', 'savoyarde', 'plat', 2, 0);

INSERT INTO Cuisine (ID_Client, Nom_Plat) VALUES 
(7654321, 'Lasagnes'),
(7654321, 'Tartiflette');

-- Plats pour Karim
INSERT INTO Plat (Nom_Plat, Date_Fabrication, Date_Peremption, Prix_plat, Regime, Origine, Type_plat, Nombre_personne, Image)
VALUES 
('Couscous', CURDATE(), '2025-12-31', 16.00, 'halal', 'maghreb', 'plat', 2, 0),
('Poulet Yassa', CURDATE(), '2025-12-31', 14.50, 'halal', 'sénégalaise', 'plat', 2, 0);

INSERT INTO Cuisine (ID_Client, Nom_Plat) VALUES 
(7896524, 'Couscous'),
(7896524, 'Poulet Yassa');

SET @date_commande = NOW();
SET @date_livraison = DATE_ADD(NOW(), INTERVAL 2 DAY);

INSERT INTO Commande (Prix_commande, Quantite, Date_Commande, Date_Livraison, Status)
VALUES (15.50, 1, @date_commande, @date_livraison, 'en cours');
SET @cmd1 = LAST_INSERT_ID();

INSERT INTO Preparer (Numero_Commande, Adresse_expedition, ID_Client)
VALUES (@cmd1, 'rue des angles, Paris', 7654321);
INSERT INTO Passer (Numero_Commande, Adresse_livraison, ID_Client)
VALUES (@cmd1, 'rue des Moines, Paris', 2875652);
INSERT INTO Contient (Numero_Commande, Nom_Plat) VALUES (@cmd1, 'Lasagnes');

INSERT INTO Commande (Prix_commande, Quantite, Date_Commande, Date_Livraison, Status)
VALUES (16.00, 1, @date_commande, @date_livraison, 'en cours');
SET @cmd2 = LAST_INSERT_ID();

INSERT INTO Preparer (Numero_Commande, Adresse_expedition, ID_Client)
VALUES (@cmd2, 'rue des boulangers, Paris', 7896524);
INSERT INTO Passer (Numero_Commande, Adresse_livraison, ID_Client)
VALUES (@cmd2, 'rue des Moines, Paris', 2875652);
INSERT INTO Contient (Numero_Commande, Nom_Plat) VALUES (@cmd2, 'Couscous');

INSERT INTO Commande (Prix_commande, Quantite, Date_Commande, Date_Livraison, Status)
VALUES (17.00, 1, @date_commande, @date_livraison, 'en cours');
SET @cmd3 = LAST_INSERT_ID();

INSERT INTO Preparer (Numero_Commande, Adresse_expedition, ID_Client)
VALUES (@cmd3, 'rue des angles, Paris', 7654321);
INSERT INTO Passer (Numero_Commande, Adresse_livraison, ID_Client)
VALUES (@cmd3, 'allée des seigneurs, Paris', 1234567);
INSERT INTO Contient (Numero_Commande, Nom_Plat) VALUES (@cmd3, 'Tartiflette');

INSERT INTO Commande (Prix_commande, Quantite, Date_Commande, Date_Livraison, Status)
VALUES (14.50, 1, @date_commande, @date_livraison, 'en cours');
SET @cmd4 = LAST_INSERT_ID();

INSERT INTO Preparer (Numero_Commande, Adresse_expedition, ID_Client)
VALUES (@cmd4, 'rue des boulangers, Paris', 7896524);
INSERT INTO Passer (Numero_Commande, Adresse_livraison, ID_Client)
VALUES (@cmd4, 'allée des seigneurs, Paris', 1234567);
INSERT INTO Contient (Numero_Commande, Nom_Plat) VALUES (@cmd4, 'Poulet Yassa');
