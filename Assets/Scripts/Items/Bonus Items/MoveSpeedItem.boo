import UnityEngine

class MoveSpeedItem (BonusItem): 

	[Separator]
	public color as Color = Color.green
	
	override def ApplyBonus(player as Player):
		super.ApplyBonus(player)
		
		bonusAmount as single = ((bonus / 100) * player.moveSpeed.bonusAmount).Round(1)
		player.moveSpeed.bonus += bonusAmount
		player.UpdateStats()
		player.mouseFollow.moveSpeed = player.MoveSpeed
		player.mouseFollow.turnSpeed = player.MoveSpeed * 3
		SpawnInfoText("Move Speed +" + bonusAmount.ToString() + "%", player.gameObject, color)
		hObjectPool.Instance.Despawn(gameObject)
		