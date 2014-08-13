import UnityEngine

class DamageItem (BonusItem): 

	[Separator]
	public color as Color = Color.green
	
	override def ApplyBonus(player as Player):
		super.ApplyBonus(player)
		
		bonusAmount as single = ((bonus / 100) * player.damage.bonusAmount).Round(1)
		player.damage.bonus += bonusAmount
		player.UpdateStats()
		SpawnInfoText("Damage +" + bonusAmount.ToString() + "%", player.gameObject, color)
		hObjectPool.Instance.Despawn(gameObject)
		