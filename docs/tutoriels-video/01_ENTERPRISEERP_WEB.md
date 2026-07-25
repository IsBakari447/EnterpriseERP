# Tutoriel video 01 - EnterpriseERP Web

Duree cible : 5 a 7 minutes  
Projet : `C:\ERP_Project\EnterpriseERP`

## Objectif de la video

Presenter EnterpriseERP comme une application ERP web complete : dashboard, clients, produits, factures, paiements, exports, audit et securite.

## Preparation

```bash
cd C:\ERP_Project\EnterpriseERP
dotnet run --project EnterpriseERP.csproj
```

Ouvrir :

```text
http://localhost:5167
```

Verifier :

```text
http://localhost:5167/health
```

## Script voix-off

### 0:00 - Intro

Bonjour, je vous presente EnterpriseERP, une plateforme web ERP developpee avec ASP.NET Core.  
L'objectif est de centraliser la gestion d'une entreprise : clients, produits, stock, devis, factures, paiements, rapports, utilisateurs et securite.

### 0:30 - Dashboard

Ici, le tableau de bord donne une vue rapide de l'activite : chiffre d'affaires, commandes, factures, clients et indicateurs importants.  
Cette page sert de point d'entree pour un manager ou un administrateur.

Action ecran :

- ouvrir le dashboard ;
- montrer les KPI ;
- montrer les graphiques ou cartes.

### 1:15 - Clients et produits

Je passe maintenant aux modules metier.  
Dans Clients, on peut gerer les informations clients.  
Dans Produits et Stock, on peut suivre les articles, les quantites et les mouvements de stock.

Action ecran :

- ouvrir Clients ;
- ouvrir Produits ;
- ouvrir Stock ;
- montrer ajouter/modifier si disponible.

### 2:10 - Cycle commercial

Un cycle commercial typique commence par un client et un produit.  
Ensuite on peut creer un devis, une commande, une facture, puis enregistrer le paiement.

Action ecran :

- ouvrir Devis ;
- ouvrir Factures ;
- ouvrir Paiements ;
- montrer un bouton PDF/export si disponible.

### 3:20 - Rapports, exports et documents

EnterpriseERP inclut aussi des rapports et des exports Excel/PDF.  
C'est important pour transformer les donnees en documents professionnels et exploitables.

Action ecran :

- ouvrir Rapports ;
- ouvrir Exports ;
- montrer PDF ou Excel.

### 4:10 - Administration et securite

La partie administration contient les utilisateurs, roles, permissions, audit, security center et backup center.  
Cela montre que le projet prend en compte la gouvernance, la tracabilite et la maintenance.

Action ecran :

- ouvrir Utilisateurs ;
- ouvrir Roles & Permissions ;
- ouvrir Audit ;
- ouvrir Security Center ;
- ouvrir Backup Center.

### 5:15 - Conclusion

EnterpriseERP est la base web de la suite.  
Elle expose aussi une API mobile securisee qui permet a l'application Android de se connecter au meme systeme.

## Points a montrer absolument

- Dashboard executif.
- Creation ou consultation client.
- Produits et stock.
- Factures ou devis.
- Exports.
- Audit et securite.

## Texte description YouTube/GitHub

Demonstration de EnterpriseERP, une plateforme ERP web en ASP.NET Core pour gerer clients, produits, stock, devis, factures, paiements, rapports, utilisateurs, roles, audit et securite.
