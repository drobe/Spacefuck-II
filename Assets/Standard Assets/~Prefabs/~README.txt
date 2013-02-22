/*

This folder contains a number of prefabs.

CTRL = "Control," prefabs that perform some sort of structural task.
TEST = "Test," prefabs that only exist for test cases.
ENTT = "Entity," prefabs for Gameplay entities.

CTRL - Administrative
	A multi-talented prefab, this contains everything from the ModeManager to each of the menu screens, as well as the camera and cursor.
	You'll always want to have this in the scene.
	For more information, check the wiki for pages on the Mode-Manager and each of the different mode screens.
	Additionally, this prefab contains references to many of the ScriptableObject assets. See the AssetBundle page on the wiki for more details.

CTRL - Gameplay
	The gameplay controller. This is a structure that assembles the playfield and determines when to end a play session.
	It also contains the arena boundaries. It should always be present.
	For more information, consult the Gameplay-Control section of the wiki.

CTRL - Network
	The network controller, which should always be present.
	This prefab does quite a bit; it contains the object pool and performs network synchronizations between clients and servers.
	Note: not all games are network games, but the network controller still exists.
	For more information, consult the Network section of the wiki.

CTRL - Visual
	Provides the semi-transparent background and scanline. Should always be present.
	The purpose of this prefab is to perform "semi-clears" of the pixel buffer.
	For more information, consult the Graphics section of the wiki.
	
TEST - Cage
	Is a cage of colliders that boxes everything in the play area.
	This is useful for testing AI, given that the AI will be forced to live very long lives.
	
ENTT - Planetoid
	Is a planetoid entity. These explode into bullets and asteroids when they leave the arena boundary.
	These are pooled entities. They shouldn't be pre-placed in scenes.

ENTT - Asteroid
	Is an asteroid entity. These explode into bullets when they leave the arena boundary.
	These are pooled entities. They shouldn't be pre-placed in scenes.

ENTT - Bullet
	Is a bullet entity. These don't explode when they leave the arena boundary, but are instead destroyed when they enter the second boundary.
	These are pooled entities. They shouldn't be pre-placed in scenes.

*/