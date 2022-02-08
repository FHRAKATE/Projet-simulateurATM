# Eval2-simulateurATM
Projet d'ecole. Application WPF MVVM full stack.
Packages: EntityFramework.6.2.0, EntityFramework.6.4.4

# Installation
Ouvrir Microsoft SQL Server Management Studio, importer la base de donnees a partir de guichetAutomatique.sql. (Nom de la base de donnés: guichetAutomatique) Ouvrir la solution avec visual studio (GestionGuichetAutomatique.sln). Installer les packages requis. Lancer l'application

# Comptes enregistrés dans la base de données
Client : Username: fharakate Password: 1111 Admin: Username: ADMIN Password: 0000

# Fonctionalités système
Appuyez sur le bouton Déconnection pour fermer la session

# Fonctionalités client
Page Guichet:
Affichage des soldes de tout les comptes du client.

Effectuer un Retrait: 	Sélectionnez le compte source, entrez le montant et appuyer sur "Valider".

Effectuer un Dépôt:     Sélectionnez le compte cible, entrez le montant et appuyer sur "Valider"

Effectuer un Virement: Sélectionnez le compte source, sélectionnez le compte cible, entrez le montant et appuyer sur "Valider".

Effectuer un Paiement:	Sélectionnez le compte source, entrez le montant et appuyer sur "Valider".

# Fonctionalités administrateur
Gestion:
	Ajouter argent papier:				Affichage du montant disponnible dans le guichet pour les retraits.
	                                                Entrez le montant a ajouter au guichet et appuyez sur "Valider".		  	
	
	Payer intérêts des comptes épargnes:		Appuyer sur le boutton pour éffectuer l'opération. 
	
	Augmenter les soldes des marges de crédit:	Appuyer sur le boutton pour éffectuer l'opération.

Créer utilisateur:	Entrez les informations requises et appuyer sur enregistrer.

Créer compte:		Choisisez le l'utilisateur à qui le compte appartiendra, choisisez le type de compte, entrez les informations requises et appuyer sur "Enregistrer". 

Transactions:		Affichage de la liste des transactions de tout les comptes enregistrés dans la base de données par Client. 

Prélèvements:		Sélectionnez un compte hypothéquecaire enregistré dans le système comme compte source, entrez le montant à prélever et appuyez sur "Valider".

