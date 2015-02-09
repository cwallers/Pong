# Pong
Catherine Wallers & Nathan Roberts \n

URL:  https://github.com/cwallers/Pong.git \n

Summary:  XNA Pong game with a Pokemon theme \n
 - Objective, keep the original Pokemon ball (the red one) in play
 - the other ball acts as a distraction, and can alter the course of the original ball
 - Escape = pause/credit screen (best visually when in small screen, not full)
 - F = full screen
 - N = new game, only avalible when someone has won
 - M = change the control between a mouse and the arrow keys
 - Backspace = exit the game

Bugs: 
- Every once in a blue moon, when the ball is moving too fast it does not detect collision because the update method is not called fast enough... but that's unavoidable and it is practically impossible to play at that level for long

Catherine Wallers: (55%)
- FontSprite (aka, where it showed up, when it showes up and it's settings)
- Set the score system up
- Sprite Sheet (chose the image, edited it to work for what we needed, and implemented it)
- Updated the paddle movement to work with a mouse and keyboard
- Made the win/lose screen and a way to restart the game
- Made the credit/pause screen and it's image change and text placement
- Added audio and touched up the images as well as the background
- Added variables to the audio and images to make it more reusable in the future
- Added the new spritesheet to the obstacle class
- Cleaned up the code and fixed some of the image/text within the game

Nathan Roberts:(45%)
- Added Physical base class to standardize position and velocity variables
- Made computer paddle AI
- Smoothened player paddle mouse tracking and computer movement
- Added BoundingSphere collision detection to paddles
- Added reflection to collision detection method
- Randomized ball reset velocity
- Added manual ball reset



