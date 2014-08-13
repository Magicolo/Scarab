import UnityEngine

class Gold (Enemy): 

	public teleports as (Teleport)
	
	override def Kill():
		super.Kill()
		
		for teleport as Teleport in teleports:
			teleport.enabled = true
			teleport.GetComponent[of SpriteRenderer]().enabled = true
			teleport.GetComponent[of BoxCollider2D]().enabled = true			