# 🏢 PlateformeViewer 👁️

## Sommaire
1. [Présentation](#1-présentation)
2. [Fonctionnalités](#2-fonctionnalités)
3. [Construire le projet](#3-construire-le-projet)

## 1. Présentation
_PlateformeViewer_ est un outil de visualisation 3D immersive. Il permet aux utilisateurs d'explorer le rendu 3D d'un bâtiment réalisé sous Unity, et de voir en un clic les plannings liés aux pièces de ce bâtiment.

## 2. Fonctionnalités
- 🧭 Navigation dans une environnement 3D
  - Appuyez sur les touches ZQSD pour déplacer la caméra,
  - Appuyez sur les touches A et D pour respectivement monter et descendre
  - Maintenez clic droit et bougez la souris pour changer la direction de la caméra
- ℹ️ Informations sur les disponibilités des salles
  - Cliquez sur une salle/open space pour vous informer de sa disponibilité
  - Vous pouvez voir les évènements de la salle, ainsi que ceux à venir 
  - Les salles suivent le code couleur suivant :
    - 🟢 Libre : La salle est libre
    - 🟡 Bientôt : La salle est libre mais sera occupée d'ici 30 minutes ou moins
    - 🔴 Occupée : La salle est occupée
    - ⚪ Inconnu : Le statut de la salle est inconnu
- 🧩 Environnement modulaire
  - Pour ajouter des salles ou des bâtiments au projet, veuillez vous référer au [Guide PlateformeViewer](guide_plateformeviewer.pdf), section "4. Ajouter un nouveau bâtiment"

## 3. Construire le projet
Pour utiliser le projet localement, assurez-vous d'avoir installé au préalable [nodejs](https://nodejs.org/en) et [npm](https://www.npmjs.com/), puis rendez-vous dans `FrontEnd/` et lancez la commande suivante, pour récupérer les dépendances du front-end :

> `npm i`

Puis, entrez :

> `npm start --host`

Cela ouvrira une page web dans le navigateur par défaut. Vous pourrez ainsi utiliser _PlateformeViewer_ dans le navigateur.
