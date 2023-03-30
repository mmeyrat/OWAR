# Organization of the workspace in Augmented Reality - PFE

This project was realized within the scope of a "Projet de Fin d'Etude" (End of Study Project) at University of Bordeaux.

This software is a prototype of a document visualization application (images & text visualization) which proposes an automatic organization of its documents according to certain criteria (physical environment, object of interest...). The user is also allowed to manually modify this organization. It was implementated by using the Unity development platform and runs on the HoloLens augmented reality headset.

## Description

Our application currently supports two types of documents which are images files (JPG, JPEG & PNG) and text files (TXT). Each of the files is loaded from a folder of the application. Texts files are organized with a pagination system. For both types of documents, the level of detail evolves according to the distance to the user's head.

It is possible to anchor a file either in world space or in view space. View space anchored documents follow the user's head and are always visible on screen.  

For this prototype, we developped two main visualisation methods opening different automatic organizations. The first one is based on tag areas. A user can place a zone object linked to a specific label, around which the documents are going to be automatically placed (available on main branch). The labels are retrieved from a local JSON setting file, linking each file with a tag. 
The second one is based on visualization heat maps. The more a place is looked at, the more it will gain interest. A sphere is placed where the user is looking curently. If he is staying on this zone, the sphere is bigger and become more red. Documents are then automatically placed following the most 3 interesting places.
To avoid overlapping, a force system is used. On each zone, each files are repelling one another and are repelled by the center. This visualization mode is available on the heatmap branch. 

## Controls

The controls of this application are based on head movements and hand gestures. The cursor is positionned at the center of the field of view and can be moved by moving the head. It is then possible to interact by using hand gestures, which corresponds to an air tap. When the cursor moves from a dot to a ring, an interaction can be performed by doing an air tap.  

## Visuals

Soon

## Installation

Make sure before everything, you have installed the specific version 2019.4.40f of Unity. 
To make this project work on Unity, first install the Windows SDK which is available here https://developer.microsoft.com/en-US/windows/downloads/windows-sdk/. 
Normally the MRTK Foundation package (Microsoft MixedReality Toolkit Unity Foundation 2.8.3) is already included in the Packages folder. If you have to download it, it's available here https://github.com/Microsoft/MixedRealityToolkit-Unity/releases. Then, this package must be imported in the project, in Unity go to Assets > Import Package > Custom Package and select the package you have just downloaded. The project can now be executed on Unity.

### Deployment on Hololens 
This project was developped to work properly on Hololens first generation. If you want to deploy on your own hololens, make sure when you build, the parameters below are the same for you. 
![Build settings](https://gitlab.emi.u-bordeaux.fr/-/ide/project/group_pfe/pfe_project/tree/heatmap/-/BuildSettings.PNG/)

## Contribution standards

This project is a prototype but is opened to any future contribution remaining in the main idea of the application.

Since this project was made with Unity, it uses C# as its scripting language. Thus, for any contribution, we will ask you to follow C# Coding Conventions recommended by Microsoft. 

Our strictest conventions are as follows :

* `Pascal case` for classes, records and structs.
* `Camel case` for private or internal variables. 
* Write only one statement or declaration per line.
* Use parentheses to make clauses in an expression apparent.
* Keep sections of code arranged without returns.
* Use four spaces when indenting.
* Do not use magic numbers (hard coded values).

More precisions and informations on these coding conventions can be found on the official [Microsoft website](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions). 

## Code of Conduct

Be respectful to other contributors and members of this project. No disrespectful comments regarding race, religious preference, sexual orientation, gender identity, military status or age will be tolerated. Any violation of the previously mentioned terms can result in being banned from any further contributing to this project. Please follow the contribution standards (above) when contributing or your contribution may be rejected.

## Authors

This project was made by :
* Maxime Dumonteil (<maxime.dumonteil@etu.u-bordeaux.fr>)
* Loup Germon (<loup.germon@etu.u-bordeaux.fr>)
* Maxime Meyrat (<maxime.meyrat@etu.u-bordeaux.fr>)
* Alex Pepi (<alex.pepi@etu.u-bordeaux.fr>)
