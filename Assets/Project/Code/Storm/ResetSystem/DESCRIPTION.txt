namespace Storm.ResetSystem {
  A system which allows scripts/mechanics to reset when the player either dies or transitions between screens/scenes.
  
  "Resetting" is a base class that can be inhereted from. 
  By inheriting from Resetting and implementing it's interface, the deriving Monobehavior 
  will automatically reset when the above conditions are met.
}