import UnityEngine

class RegenerationItem (BonusItem): 

	[Separator]
	public color as Color = Color.green
	
	override def ApplyBonus(player as Player):
		super.ApplyBonus(player)
		
		bonusAmount as single = ((bonus / 100) * player.regeneration.bonusAmount).Round(1)
		player.regeneration.base += bonusAmount
		player.UpdateStats()
		SpawnInfoText("Regeneration +" + bonusAmount.ToString() + "%", player.gameObject, color)
		hObjectPool.Instance.Despawn(gameObject)
		