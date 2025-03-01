Pour la partie base de données

Copier-coller les fichiers dans :
C:\ProgramData\MySQL\MySQL Server 8.0\Uploads
(Obligatoire à cause des sécurités Workbench)

Si nouveau CSV : 
- Vérifier l'encodage UTF-8 et non UTF-8 BOM ou autre, avec Notepad++ ou similaire
- Si première ligne superflue, ajouter "IGNORE 1 ROWS" dans le code pour les lectures csv

Pour le ccode il suffit de lancer le programme, le retour console vous guidera pour récupérer le graphe
