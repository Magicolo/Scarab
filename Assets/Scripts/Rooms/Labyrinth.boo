import UnityEngine

class Labyrinth (Room): 

	playerScale as Vector2
	
	override def OnRoomEnter(room as Room):
		super.OnRoomEnter(room)
		
		if room == self:
			if References.Player != null:
				playerScale = References.Player.currentScale
				References.Player.transform.localScale = Vector2(0.75, 0.75)
				References.Player.currentScale = References.Player.transform.localScale
				
	override def OnRoomExit(room as Room):
		super.OnRoomExit(room)
		
		if room == self:
			References.Player.transform.localScale = playerScale
			References.Player.currentScale = playerScale
