Pour la partie base de données

Copier-coller les fichiers dans :
C:\ProgramData\MySQL\MySQL Server 8.0\Uploads
(Obligatoire à cause des sécurités Workbench)

Si nouveau CSV : 
- Vérifier l'encodage UTF-8 et non UTF-8 BOM ou autre, avec Notepad++ ou similaire
- Si première ligne superflue, ajouter "IGNORE 1 ROWS" dans le code pour les lectures csv

Pour utiliser cette application il faudra d'abord insérer la base de données fournie dans votre workbench.
Copiez-coller le code dans la connexion de votre choix.
Ensuite lancez l'application, elle va tout d'abord importer les csv, patientez quelques secondes et cliquez sur une touche de votre clavier.

Maintenant il vous faut vous connecter à votre workbench, entrez votre le nom de votre server (tapez 1 si votre serveur par défaut est localhost), entrez votre identifiant et votre mot de passe.
L'application est maintenant ouverte.

Vous avez plusieurs choix. 
Pour créer un utilisateur vous serez guidé pendant le processus. Si vous comptez utiliser ce client retenez bien l'identifiant qui sera généré
Pour vous connecter en tant que client 2 choix s'offrent à vous:
  - soit vous utilisez un client par défaut créer pour que vous puissiez tester l'application. Son identifiant est 2875652 et son mot de passe est 123
  - soit vous utilisez le client que vous aurez créé précédemment

Pour vous connecter en tant que cuisinier 2 choix s'offrent à vous:
  - soit vous utilisez un client par défaut créer pour que vous puissiez tester l'application. Son identifiant est 7654321 et son mot de passe est 123
  - soit vous utilisez le cuisinier que vous aurez créé précédemment

Une fois dans l'application, vous serez face à un menu qui vous guidera dans vos options

Si le programme ne compile pas, installez les package nuggets:
- Graphizv
- MySQL
- JSON
