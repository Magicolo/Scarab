import UnityEngine

class Fighter (Character): 

	public Damage as single:
		get: return damage.value
		set: damage.value = value
	public damage as Stat = Stat("Damage", 10, 0)
		
	public FireSpeed as single:
		get: return fireSpeed.value
		set: fireSpeed.value = value
	public fireSpeed as Stat = Stat("FireSpeed", 5, 0)
	
	public ProjectileSpeed as single:
		get: return projectileSpeed.value
		set: projectileSpeed.value = value
	public projectileSpeed as Stat = Stat("ProjectileSpeed", 10, 0)
		
	public MoveSpeed as single:
		get: return moveSpeed.value
		set: moveSpeed.value = value
	public moveSpeed as Stat = Stat("MoveSpeed", 3, 0)
		
	public Accuracy as single:
		get: return accuracy.value
		set: accuracy.value = value
	public accuracy as Stat = Stat("Accuracy", 100, 0)
		
	public BurstAmount as int:
		get: return burstAmount.value
		set: burstAmount.value = value
	public burstAmount as Stat = Stat("BurstAmount", 1, 0)
		
	fireCounter as single
	
	override def Awake():
		super.Awake()
		
		stats.Add(damage)
		stats.Add(fireSpeed)
		stats.Add(projectileSpeed)
		stats.Add(moveSpeed)
		stats.Add(burstAmount)
		stats.Add(accuracy)
		UpdateStats()
	
	virtual def UpdateStats():
		for stat as Stat in stats:
			if stat.name == "Accuracy":
				stat.maxValue = 95 * BurstAmount
			elif stat.name == "FireSpeed":
				stat.maxValue = 30 * BurstAmount
			stat.UpdateStat()
	
	override def Normal():
		super.Normal()
		
		if FireSpeed > 0:
			if fireCounter > (1.0 / FireSpeed) * BurstAmount:
				Fire()
			else:
				fireCounter += Time.deltaTime
			
	virtual def Fire():
		for i in range(BurstAmount):
			fireCounter = 0
			direction as Vector2
			adjustedAccuracy as single
			
			if i == 0:
				adjustedAccuracy = 100 - Mathf.Clamp(Accuracy, 0, 95)
			else:
				adjustedAccuracy = 100 - Mathf.Clamp(Accuracy / BurstAmount, 0, 95)
			direction = (transform.TransformDirection(Vector2.right) + Vector2(Random.Range(-adjustedAccuracy, adjustedAccuracy) / 100, Random.Range(-adjustedAccuracy, adjustedAccuracy) / 100)).normalized
				
			newProjectile as GameObject = hObjectPool.Instance.Spawn(References.RockID, gameObject.transform.position + Vector2(transform.localScale.x * (direction / 2).x, transform.localScale.y * (direction / 2).y), Quaternion.identity)
			newProjectile.rigidbody2D.velocity = direction * ProjectileSpeed
			newProjectile.layer = gameObject.layer
			rock as Rock = newProjectile.GetComponent(Rock)
			rock.transform.parent = References.RoomManager.currentRoom.transform
			rock.owner = self
			rock.damage = rock.baseDamage + Damage
			rock.gameObject.layer = gameObject.layer
			rock.transform.localScale = rock.initScale * transform.localScale.x
			rock.rigidbody2D.mass = rock.initMass * transform.localScale.x
			AudioPlayer.Play("SFX_Character_Fire")
			
