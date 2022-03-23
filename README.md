# Binrio-Platform-Game
 2D Pixel Mario Style Platform Game
 
Animation: Player is default Player_Idle. 
		- isWalking : true => Walk animation 
		- isWalking : false => Goes back to Idle motion
Uncheck has exit time.

isJumping : true => player is jumping
isJumping : false => player is not jumping

Idle : isWalking => false / isJumping = > false
Walk: isWalking => true  / isJumping = > false
Jump: isWalking =>  true or false / isJumping => true 

Same applies for enemy

Coin is static but trigger 
