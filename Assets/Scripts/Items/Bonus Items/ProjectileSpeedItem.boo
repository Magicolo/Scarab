import UnityEngine

class ProjectileSpeedItem (BonusItem): 

	[Separator]
	public color as Color = Color.green
	
	override def ApplyBonus(player as Player):
		super.ApplyBonus(player)
		
		bonusAmount as single = ((bonus / 100) * player.projectileSpeed.bonusAmount).Round(1)
		player.projectileSpeed.bonus += bonusAmount
		player.UpdateStats()
		SpawnInfoText("Projectile Speed +" + bonusAmount.ToString() + "%", player.gameObject, color)
		hObjectPool.Instance.Despawn(gameObject)
		