

The game is a bit different form the original:

 - Pressing F5 will instantly complete the current level.
 - Completing the game will make the next playthrough faster to add more challenge (in the current session, if the game is restarted all is back to normal).
 
Creating levels:

 All game levels are contained in the Content/levels subdirectory.
 You can rename/delete all of the exisiting levels.
 The game will load all levels starting form level_0.txt consecutively. All level numbers must be in order otherwise some levels won't be loaded.
 For example, if the directory contains:
 level_0.txt
 level_1.txt
 level_2.txt
 level_4.txt
 
 The file level_4.txt will never get loaded. Rename it to level_3.txt to get it working.
 
 
Levels are just text files, and you can create your own just by using any text editor (like Notepad):
Please make sure that in the level there are always one player (P or p symbol) and one mailbox (cappital L symbol) otherwise the game will crash.
The object will be placed on the level as-if the text was the level itself.

p or P - means the player
x or X - means a solid block
g or G - is a ghost
f or F - is fire
i or I - is an ice spike
L - is the mailbox for finishing the level
l - is a letter to be collected
w - is walking enemy which goes right initialy
W - is walking enemy which goes left initialy
t - is a blue time switch, which starts solid
T - is a read time switch, which starts non-solid
J - is a a yellow jump switch, which starts solid
j - is a green jump switch, which start non-solid

Everything else is just and empty space (I prefer use "space" or .) do not use tabs :)

Here is an example:

................
................
.p...l..w....L..
.xxx.x.xxx.xxxx.

Notepad++ is a good tool for making level (as it has multiline edit)


