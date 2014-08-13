import UnityEngine

class HUD (MonoBehaviour): 

	public debug as bool = true

	def OnGUI():
		style as GUIStyle = GUIStyle()
		style.normal.textColor = Color.black
		
		GUILayout.BeginHorizontal()
		GUILayout.Space(Screen.width / 3)
		GUILayout.BeginVertical()
		GUILayout.Space(Screen.height / 4)
		if debug:
			GUILayout.Label("Current Room: " + References.RoomManager.currentRoom.roomCoord.x + ", " + References.RoomManager.currentRoom.roomCoord.y, style)
			GUILayout.Label("Remaining Enemies: " + References.RoomManager.currentRoom.enemies.Count, style)
		GUILayout.Label("Lives: " + References.Player.lives, style)
		GUILayout.Label("Level: " + References.Player.level, style)
		GUILayout.Label("Experience: " + References.Player.experience, style)
		GUILayout.Label("Next Level: " + References.Player.nextLevel, style)
		GUILayout.Label("Health: " + References.Player.Health, style)
		for stat as Stat in References.Player.stats:
			GUILayout.Label(stat.name + ": " + stat.value, style)
		GUILayout.EndVertical()
		GUILayout.EndHorizontal()
		