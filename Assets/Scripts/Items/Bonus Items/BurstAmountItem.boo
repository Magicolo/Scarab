import UnityEngine

class BurstAmountItem (BonusItem): 

	[Separator]
	public color as Color = Color.green
	
	override def ApplyBonus(player as Player):
		super.ApplyBonus(player)
		
		if not player.hasSword:
			player.burstAmount.base += 1
		player.UpdateStats()
		SpawnInfoText("More Rocks!", player.gameObject, color)
		hObjectPool.Instance.Despawn(gameObject)
