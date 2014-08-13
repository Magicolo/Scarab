import UnityEngine

class FireSpeedItem (BonusItem): 

	[Separator]
	public color as Color = Color.green
	
	override def ApplyBonus(player as Player):
		super.ApplyBonus(player)
		
		bonusAmount as single = ((bonus / 100) * player.fireSpeed.bonusAmount).Round(1)
		player.fireSpeed.bonus += bonusAmount
		player.UpdateStats()
		SpawnInfoText("Fire Speed +" + bonusAmount.ToString() + "%", player.gameObject, color)
		hObjectPool.Instance.Despawn(gameObject)
