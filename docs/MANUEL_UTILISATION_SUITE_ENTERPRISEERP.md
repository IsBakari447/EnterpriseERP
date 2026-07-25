# Manuel d'utilisation - EnterpriseERP Suite

Version : 2026-07-25  
Auteur projet : Issa Bakari Ndam Njounkou

## 1. Objectif du manuel

Ce manuel explique comment utiliser et presenter les trois projets EnterpriseERP :

- **EnterpriseERP** : plateforme ERP web pour gerer une entreprise.
- **EnterpriseERP.Mobile** : application mobile Android connectee a l'API EnterpriseERP.
- **EnterpriseERP Cloud** : version SaaS moderne avec interface web Next.js et API NestJS.

Le manuel est concu pour trois publics :

- utilisateur final : comprendre les fonctions principales ;
- recruteur ou jury : suivre une demonstration claire ;
- developpeur : lancer, verifier et maintenir le projet.

## 2. Vue d'ensemble de la suite

EnterpriseERP Suite couvre les besoins principaux d'une entreprise :

- tableau de bord et indicateurs ;
- clients, fournisseurs et CRM ;
- produits, stock et mouvements ;
- devis, commandes, factures et paiements ;
- depenses, rapports et exports ;
- employes, presences et rendez-vous ;
- utilisateurs, roles, permissions, audit, securite et sauvegarde ;
- profil utilisateur, photo, preferences et API mobile ;
- vision SaaS cloud avec pages marketing, demo et endpoints API.

## 3. Prerequis communs

### Outils utiles

```bash
dotnet --version
node --version
npm --version
git --version
```

### Ports courants

```text
EnterpriseERP backend web/API : http://localhost:5167
EnterpriseERP Cloud web       : http://localhost:3000
EnterpriseERP Cloud API       : http://localhost:4000/api
```

Sur telephone Android reel, ne jamais utiliser `localhost` pour joindre le backend du PC. Utiliser l'adresse IP locale du PC, par exemple :

```text
http://192.168.1.20:5167/
```

## 4. EnterpriseERP - Application web ERP

### 4.1 Description

EnterpriseERP est l'application ERP principale. Elle est construite avec ASP.NET Core MVC, Entity Framework Core, SQLite, QuestPDF, ClosedXML et JWT pour l'API mobile.

Chemin local :

```text
C:\ERP_Project\EnterpriseERP
```

URL production actuelle :

```text
https://enterpriseerp-1.onrender.com
```

### 4.2 Lancer en local

```bash
cd C:\ERP_Project\EnterpriseERP
dotnet restore
dotnet build --no-restore
dotnet run --project EnterpriseERP.csproj
```

Verification rapide :

```http
GET /health
GET /health/ready
```

### 4.3 Premiere connexion et compte demo

1. Ouvrir l'application web.
2. Aller sur inscription ou connexion selon l'etat de la base.
3. Creer le premier compte administrateur.
4. Verifier l'acces au dashboard.
5. Tester les menus principaux.

Dans l'API mobile, le premier utilisateur cree via mobile devient `SuperAdmin` si la base est vide.

### 4.4 Navigation principale

#### Dashboard

Utiliser le dashboard pour verifier :

- chiffre d'affaires ;
- commandes ;
- factures ;
- clients ;
- alertes ;
- indicateurs executifs.

#### Clients et fournisseurs

Actions principales :

- ajouter un client ;
- modifier les informations ;
- consulter l'historique ;
- filtrer ou rechercher ;
- supprimer avec prudence.

#### Produits et stock

Actions principales :

- ajouter un produit ;
- definir prix, quantite et seuil ;
- enregistrer un mouvement de stock ;
- suivre les alertes de stock faible.

#### Devis, commandes, factures, paiements

Parcours conseille :

1. Creer un client.
2. Creer un produit.
3. Creer un devis.
4. Convertir ou presenter la facture.
5. Enregistrer un paiement.
6. Exporter ou imprimer un document PDF.

#### Employes et presences

Actions principales :

- ajouter un employe ;
- enregistrer une presence ;
- consulter les entrees et sorties ;
- analyser les absences ou presences recentes.

#### Rapports et exports

L'application permet :

- rapports financiers ;
- exports Excel ;
- documents PDF ;
- donnees exploitables pour analyse.

#### Securite, roles et audit

Modules importants :

- utilisateurs ;
- roles et permissions ;
- audit logs ;
- security center ;
- backup center.

Ces pages sont essentielles pour montrer que le projet n'est pas seulement une interface, mais une application metier avec gouvernance.

### 4.5 Sauvegarde et maintenance

Avant une demonstration :

```bash
dotnet build /p:UseAppHost=false -o artifacts\demo-audit-build
```

Verifier :

- `/health` repond ;
- la base SQLite est presente ;
- aucun secret reel n'est versionne ;
- les uploads et sauvegardes ne sont pas commits.

## 5. EnterpriseERP.Mobile - Application Android MAUI

### 5.1 Description

EnterpriseERP.Mobile est le client mobile Android. Il se connecte a l'API mobile du backend EnterpriseERP.

Chemin local :

```text
C:\ERP_Project\EnterpriseERP.Mobile
```

### 5.2 Build sans appareil

```bash
cd C:\ERP_Project\EnterpriseERP.Mobile
dotnet build -f net10.0-android /p:AndroidBuildApplicationPackage=false
```

### 5.3 Lancer sur appareil ou emulateur

```bash
dotnet build -t:Run -f net10.0-android
```

Si l'erreur suivante apparait :

```text
XA0010: No available device
```

Il faut connecter un telephone Android avec le debogage USB active ou demarrer un emulateur.

### 5.4 Configuration API mobile

Sur telephone reel :

1. Lancer EnterpriseERP sur le PC.
2. Verifier `/health`.
3. Connecter le telephone au meme Wi-Fi.
4. Dans l'application mobile, saisir l'URL API :

```text
http://ADRESSE_IP_DU_PC:5167/
```

5. Appuyer sur **Tester l'API**.
6. Se connecter ou creer un compte.

### 5.5 Parcours utilisateur mobile

#### Accueil

L'ecran d'accueil presente l'application et permet d'aller vers la connexion.

#### Connexion

Actions :

- saisir email et mot de passe ;
- verifier l'URL API ;
- tester l'API ;
- se connecter.

Si l'API est indisponible :

- verifier l'adresse IP ;
- verifier que le backend tourne ;
- verifier que le telephone et le PC sont sur le meme reseau ;
- eviter `localhost` sur telephone reel.

#### Creation de compte

Actions :

- saisir nom, entreprise, email, mot de passe ;
- confirmer le mot de passe ;
- creer le compte.

Si c'est le premier compte, il devient SuperAdmin cote backend.

#### Dashboard mobile

Le dashboard mobile permet de consulter :

- chiffre d'affaires ;
- commandes ;
- factures ;
- clients ;
- indicateurs de sante ;
- alertes.

#### Modules mobiles

Menus principaux :

- Profil ;
- Clients ;
- Fournisseurs ;
- Devis ;
- Commandes ;
- Factures ;
- Paiements ;
- Produits ;
- Stock ;
- Depenses ;
- Rapports ;
- Exports Excel ;
- Employes ;
- Presences ;
- Rendez-vous ;
- Parametres ;
- Security Center ;
- Users Management ;
- Roles & Permissions ;
- Audit ;
- Backup Center.

#### Profil et photo

Actions :

- modifier les informations personnelles ;
- changer la photo depuis la camera ;
- choisir une photo depuis la galerie ;
- supprimer ou partager la photo ;
- enregistrer le profil.

Si la camera ne s'ouvre pas :

- verifier la permission camera Android ;
- verifier que l'application est installee correctement ;
- verifier que l'appareil possede une camera disponible.

#### Parametres

Utiliser la page Parametres pour :

- modifier l'URL API ;
- tester la connexion API ;
- mettre a jour le profil entreprise ;
- regler langue, theme, securite et sauvegarde.

### 5.6 Checklist demo mobile

Avant la demo :

- backend EnterpriseERP lance ;
- `/health` OK ;
- telephone sur le meme Wi-Fi ;
- URL API configuree ;
- bouton **Tester l'API** OK ;
- compte demo pret ;
- navigation Dashboard, Profil, Clients, Factures, Audit testee.

## 6. EnterpriseERP Cloud - Version SaaS

### 6.1 Description

EnterpriseERP Cloud est la version SaaS du projet. Elle met en avant une interface moderne, une architecture API-first et une vision produit commercialisable.

Chemin local :

```text
C:\ERP_Project\enterpriseerp-cloud
```

### 6.2 Structure importante

Le depot contient deux surfaces API :

- `apps/api` : API NestJS utilisee par l'interface web de demo recente.
- `services/api` : API service historique presente dans les scripts racine.

Pour la demo visuelle du site Cloud, utiliser surtout :

```text
apps/web
apps/api
```

### 6.3 Lancer le web

```bash
cd C:\ERP_Project\enterpriseerp-cloud
npm --prefix apps\web run dev
```

URL :

```text
http://localhost:3000
```

### 6.4 Lancer l'API de demo

```bash
cd C:\ERP_Project\enterpriseerp-cloud
npm --prefix apps\api run start:dev
```

URL :

```text
http://localhost:4000/api
```

La variable du front :

```text
NEXT_PUBLIC_API_URL=http://localhost:4000
```

Le client web ajoute `/api` automatiquement si necessaire.

### 6.5 Pages importantes

Pages web :

- `/` : accueil ;
- `/dashboard` : tableau de bord SaaS ;
- `/cloud` : presentation Cloud ;
- `/solutions` : solutions ;
- `/pricing` : offres ;
- `/demo` : demande de demo connectee API ;
- `/roi` : modele ROI ;
- `/security` : securite ;
- `/integrations` : integrations ;
- `/onboarding` : onboarding ;
- `/status` : statut ;
- `/faq` : questions frequentes.

Endpoints API :

```http
GET /api/health
GET /api/health/ready
GET /api/modules
GET /api/pricing
GET /api/demo/script
POST /api/demo/requests
GET /api/clients
GET /api/products
GET /api/invoices
```

### 6.6 Formulaire demo

Parcours :

1. Ouvrir `/demo`.
2. Saisir nom, email professionnel et besoin principal.
3. Envoyer.
4. Verifier le message de confirmation.
5. Si erreur, verifier que l'API tourne et que `NEXT_PUBLIC_API_URL` pointe vers le bon serveur.

### 6.7 Build de validation

```bash
npm --prefix apps\api run build
npm --prefix apps\web run build
```

## 7. Scenario de demonstration complet

### 7.1 Demo courte - 5 minutes

1. Ouvrir EnterpriseERP web.
2. Montrer Dashboard, Clients, Produits, Factures.
3. Ouvrir EnterpriseERP.Mobile.
4. Tester l'API et montrer le Dashboard mobile.
5. Ouvrir EnterpriseERP Cloud.
6. Montrer `/dashboard`, `/pricing`, `/demo`.
7. Envoyer une demande demo Cloud.

### 7.2 Demo complete - 15 minutes

1. Presenter la vision EnterpriseERP Suite.
2. Creer ou montrer un client.
3. Ajouter un produit.
4. Creer un devis ou une facture.
5. Enregistrer un paiement.
6. Exporter ou ouvrir un PDF.
7. Montrer roles, audit et security center.
8. Passer au mobile : connexion, dashboard, profil, photo.
9. Passer au Cloud : dashboard SaaS, pricing, demo API.
10. Terminer par la valeur produit : web + mobile + cloud.

## 8. Depannage rapide

### API non disponible sur mobile

Causes probables :

- backend arrete ;
- mauvaise IP ;
- telephone sur un autre reseau ;
- pare-feu Windows ;
- utilisation de `localhost` sur telephone reel.

Solution :

1. Verifier `/health` depuis le PC.
2. Trouver l'IP du PC.
3. Entrer `http://IP_DU_PC:5167/` dans l'app mobile.
4. Appuyer sur **Tester l'API**.

### Build Android OK mais Run echoue

Si `dotnet build` fonctionne mais `-t:Run` echoue, le probleme est souvent l'absence d'appareil Android connecte.

### Cloud demo ne s'envoie pas

Verifier :

- API `apps/api` lancee ;
- URL API correcte ;
- CORS configure ;
- route `POST /api/demo/requests` disponible.

### PDF ou export non ouvert

Verifier :

- permissions fichier ;
- chemin d'upload ;
- backend lance ;
- navigateur ou lecteur PDF disponible.

## 9. Bonnes pratiques avant GitHub

Avant commit :

```bash
git status
```

Builds recommandes :

```bash
cd C:\ERP_Project\EnterpriseERP
dotnet build /p:UseAppHost=false -o artifacts\demo-audit-build

cd C:\ERP_Project\EnterpriseERP.Mobile
dotnet build -f net10.0-android /p:AndroidBuildApplicationPackage=false

cd C:\ERP_Project\enterpriseerp-cloud
npm --prefix apps\api run build
npm --prefix apps\web run build
```

Ne pas publier :

- secrets ;
- bases de donnees de production ;
- fichiers `bin/` et `obj/` ;
- uploads personnels ;
- sauvegardes sensibles.

## 10. Conclusion

EnterpriseERP Suite presente une progression professionnelle :

- une application web ERP complete ;
- une application mobile connectee ;
- une vision Cloud/SaaS moderne.

Pour une demonstration convaincante, montrer toujours :

- un flux metier concret ;
- la connexion entre mobile et backend ;
- la partie securite/audit ;
- la vision Cloud et le formulaire demo.
