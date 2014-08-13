
interface IKillable:
	
	Alive as bool:
		get
		set
	DeadColor as Color:
		get
		set
		
	def Kill()
		
	def Dead()