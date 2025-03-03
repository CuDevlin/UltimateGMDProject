# Roll-A-Ball Game

Devlin Onichuk
24 02 2025 - (Updated: 03 03 2025)
## Description

We kicked off our first game with a simple roll a ball game!

This was done by creating a basic plane for the base of the map. As well as a material to give the map some color. Adding these materials allow us to mor easily make changes to objects should we need to change appearance
of these objects with the material applied.

As well Walls were added in order to have an enclosed space where the ball/player would reside. This ensures in the next step where we implement the pplayer that they dont go flying off the map area.

Next we defined the player with a sphere object, this was then given a playerController. This defines the player movement and interactions with the collectibles we added as the objective for the player to collect. 

After adding the player we defined the enemy, in which the player would avoid. If the player gets too close, or collides with the enemy, they will be shown a gave over overlay. 

Objects and pathing were added using a navMesh, this allows the enemy to avoid objects placed on the map, as well there are dynamic objects placed. These can be moved and block the enemy from pathing to the player.

UI was added to display the amount of collectibles have been gathered. This UI is implemented by using Unity's built in UI canvas and text-mesh components.

If the player collects all object sucessfully the enemies will be destroyed and they will be shown a win overlay! This is done with a simple check inside the player playerController that
checks if all the collectibles have been gathered, then calling .Destroy() on any objects tagged "Enemy".

Additionally using the window and enviroment settings, a new skybox was added. This was done to learn more about how to edit and change skyboxes. As well getting
more familiar with better practices by setting up which directional light is considered the "Sun". Fog was also introduced to add a more eerie enviroment.

An additional plane anad level has been included to add a challenge for players to navigate to this new level by using a ramp and jumping to this level. This jump is 
less to be meant as difficult but a second enemy is included and a player must be careful taking the jump as this enemy will be waiting for them once the leap is made.

Additional collectibles have been added to the second level to ensure the necessity that this area has to be naviagted in order for players to complete the game.

In conclusion we implemented
1. Player Movement
2. Enemy Movement
3. Collision physics
4. Simple UI Overlay
5. Collectibles
6. Expanded Map
8. Fog
9. Skybox

These although simple are a great way to learn the basics with Unity. As well helps us come up with ideas for our game development project.