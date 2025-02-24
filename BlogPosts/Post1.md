# Roll-A-Ball Game

Devlin Onichuk
24 02 2025
## Description

We kicked off our first game with a simple roll a ball.

This was done by creating a basic plane for the base of the map. As well as a material to give the map some color. As well Walls were added in order to have an enclosed space where the ball/player would reside.

Next we defined the player with a sphere object, this was then given a playerController. This defines the player movement and interactions with the collectibles we added as the objective for the player to collect. 

After adding the player we defined the enemy, in which the player would avoid. If the player gets too close, or collides with the enemy, they will be shown a gave over overlay. 

Objects and pathing were added using a navMesh, this allows the enemy to avoid objects placed on the map, as well there are dynamic objects placed. These can be moved and block the enemy from pathing to the player.

UI was added to display the amount of collectibles have been gathered.

If the player collects all object sucessfully the enemies will be destroied and they will be shown a win overlay!

In conclusion we implemented
1. Basic Player Movement
2. Basic Enemy Movement
3. Basic collision physics
4. UI Overlay
5. Collectibles
6. Expanded Map

These although simple are a great way to learn the basics with Unity. As well helps us come up with ideas for our game development project.