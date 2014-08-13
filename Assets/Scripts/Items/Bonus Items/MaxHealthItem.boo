import UnityEngine

class MaxHealthItem (BonusItem): 

	[Separator]
	public color as Color = Color.green
	
	override def ApplyBonus(player as Player):
		super.ApplyBonus(player)
		
		bonusAmount as single = ((bonus / 100) * player.maxHealth.bonusAmount).Round(1)
		player.maxHealth.bonus += bonusAmount
		player.UpdateStats()
		SpawnInfoText("Maximum Health +" + bonusAmount.ToString() + "%", player.gameObject, color)
		hObjectPool.Instance.Despawn(gameObject)
		