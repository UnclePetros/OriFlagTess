[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://github.com/UnclePetros/OriFlagTess/blob/master/LICENSE)

# OriFlagTess - An origami flagstone molecule design tool

OriFlagTess is an origami design software dedicated to calculate and construct single flagstone molecules.  
It is based on mathematical concepts explained in the article "[Origami - Molecule construction in flagstone tessellation](https://pvitelli.net/2020/01/20/origami-flagstone-tessellations/)"

## Main features
- Real-time simulation of the crease pattern of a specific "flagstone" molecule
- Possibility to save a molecule crease pattern in svg format.

## Prerequisites

At the moment, it runs only on Microsoft Windows 7 and higher.
In order to run the software .NET Framework 4.72 or higher is required.  
Latest version of Windows OS have it already available.  
If not, you can download it from here: [https://dotnet.microsoft.com/download/dotnet-framework/net472](https://dotnet.microsoft.com/download/dotnet-framework/net472)  

## Installation

Download the latest version (current is v0.4): [https://github.com/UnclePetros/OriFlagTess/releases/download/v0.4/OriFlagTess.v0.4.zip](https://github.com/UnclePetros/OriFlagTess/releases/download/v0.4/OriFlagTess.v0.4.zip)  
Unzip the package and launch _OriFlagTess.exe_ file executable. 

## Use

The software starts with a 4-pleat molecule construction.  
Use the molecule type combobox to choose a different molecule type (from 3-pleat to 8-pleat).  
Use angles controls to change angles values of the molecule:  
- it is possible to set only the first n-1 angles of the n-pleat molecule; the last one is automatically calculated.  

Use sides controls to change sides dimensions of the molecule:  
- it is possible to set only the first n-2 side of the n-pleat molecule; the last 2 sides are automatically calculated.  

Use pleatCenters controls to change the center of the pleats of the molecule.  
Use center controls to change the position of the center of the molecule.  

Further info on its usage are available in the article "[Origami - A software tool for "Flagstone" molecule design](https://pvitelli.net/2020/03/09/origami-software-flagstone-molecule/)".

## Next challenge
- Convert software in a javascript online version.

## For developers

If you want to make changes or modify the program, simply:

* Clone this repository
* Open OriFlagTess solution file with Microsoft VisualStudio 2019
