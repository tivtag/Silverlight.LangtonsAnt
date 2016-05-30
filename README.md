#Langton's Ant

##How to start
Open ant.html with a Silverlight compatible browser like Microsoft Internet Explorer

##Download
http://paul.ennemoser.com/files/LangtonsAnt.zip

##About
Imagine an ant-field made out of boxes, called cells.
Each cell may be either colored black or white.
At the start of the journey every cell is white,
but the cell of our little ant is black.
The algorithm, which moves our ant over the ant-field,
is as followed:

1. Move Ant.
2. The cell the Ant moved onto changes color (black -> white, white -> black)
3. If the cell was black, then turn the Ant 90° left. 
   If the cell was white, then turn the Ant 90° right.
4. Start again at 1.

If we follow the ant on her journey she seems to generate a quite chaotic structure.
But suddenly at around 10000 steps the structure the ant is creating
turns into a non-chaotic, for-ever repeating structure.
This proves that a simple algorithm that produces a chaotic structure may infact change its behaviour to be non-chaotic.

##See Also
http://mathworld.wolfram.com/LangtonsAnt.html
