import UnityEngine

class SwordArtifact (Artifact): 

	override def ApplyArtifact():
		super.ApplyArtifact()
		
		if CurrentState != PickedUp:
			CurrentState = PickedUp
			AudioPlayer.Play("SFX_Artifact_Touch")
		else:
			References.Player.hasSword = true
			References.Player.EquipSword()
			SpawnInfoText("THE SWORD", References.Player.gameObject, Color.white, ColorMode.Normal, 4)
			AudioPlayer.Play("SFX_Artifact_Pickup")
