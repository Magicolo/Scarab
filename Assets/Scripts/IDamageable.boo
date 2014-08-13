import UnityEngine

interface IDamageable:
	
	Health as single:
		get
		set
	MaxHealth as single:
		get
		set
	HealthBar as HealthBarGUI:
		get
		set
	NormalColor as Color:
		get
		set
	DamagedColor as Color:
		get
		set
	
	def ApplyDamage(damage as single)
	
	def Damaged()
