# Castle Defender
Castle Defender is a digital trading card board game designed to be fun, interactive, and competitive with friends while keeping with social-distancing guidelines. With player versus player gameplay, our team's mission was to create a rich and intuitive gameplay experience, with a comprehensive set of rules to promote strategic playing styles. The hybrid trading card and board game bring a unique feeling experience between sessions to set our project apart from the competition.

## Running the game
The game is built on top of the Unity game engine and have two methods for running. 

Despite how the game is launched, the game was developed using `Unity Relay` to support multiplayer. There is a psuedo-singleplayer available within the Unity Editor as mentioned in `Run from source`; that is the singleplayer launches you into the game, but without an opponent. It will always be your turn, and you can spawn/move cards, but there is no win-condition.

If you build and run the game within the local environment, multiplayer will not work since the game has been specifically set up using a developer account for the repository owner. To set multiplayer up with your own account, you can reference the [Unity Relay documentation](https://unity.com/products/relay).

### Run from source
The source code project can be opened within Unity and played directly within the Unity Editor. To run within the editor, ensure `GameManager` has `Allow Multiplayer in Editor` unchecked to allow for the game to be loaded within unity. Next, select the `Main Menu` scene, and press the Unity play button. More information about running a game in the Unity Editor can be found in their [official documentation](https://docs.unity3d.com/Manual/GameView.html).

### Run the executable
The executable either be downloaded from the itch.io server at [castle-defender-v1.0](https://puch.itch.io/castle-defender-v012) or generated using the Unity editor.

If generating via the Unity editor, you will need to set up your own Unity Relay account as described in the section `Running the game`

## Gameplay
After the game has launched, you will need to have a Host and another client to Join. To do this, you can either launch multiple instances of the game, or launch an instance on your host machine, and on another device launch another instance. The first player should press `Host Game`, thus generating a multiplayer code. The second player should press `Join Game` and enter that same code to join the game. 

The game is then broken down into phases and mechanics as thoroughly described in the project report, and briefly described below

### Setup
The setup phase allows each player to place their castle at the desired location on their side of the board. Once the castle has been placed, press the `Ready` button on the right-hand side.

### Player turn
The game play is divided into player turns (Player 1 versus Player 2), consisting of:
- Automatically drawing a card
- Resetting resources
- Play cards (assuming you have enough resources)
- Move creatures
- Attack enemy units or permanents

### Win Condition
- When a player reaches zero health for their castle, the opponents wins.

## Cards
There are many different card types within the game as described in depth within the project report, and briefly described below:

### Castle
A permanent structure that indicates each player. Once this structure reaches zero health, the opponent wins the game.

### Wall
A simple one-unit structure that can block movement of creatures.

### Creature
A token on the board that represents an entity that can move, attack and be attacked. Creatures have mechanics displayed in their card description

### Trap
A permanent token placed on the gameboard that is only visible to the player who placed it. Opponents can trigger the trap by moving their creature tokens on top of it.

### Spell
A card that can be cast on a given target, or on the gameboard in general. Each spell is unique and has a description describing its capabilities.

### Enchantment
Enchantments are cast on ally units or permanents that enhance an entities capabilities, as described by their description.