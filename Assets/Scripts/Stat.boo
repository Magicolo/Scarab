import UnityEngine

class Stat: 

	public name as string
	public base as single
	public levelUpFactor as single = 1.05
	public bonus as single
	public bonusAmount as single = 15
	public aura as single
	public minValue as single
	public maxValue as single
	public value as single
	
	def constructor(name as string):
		self.name = name
		UpdateStat()
	
	def constructor(name as string, base as single, bonus as single):
		self.name = name
		self.base = base
		self.bonus = bonus
		UpdateStat()
		
	def constructor(name as string, base as single, bonus as single, minValue as single, maxValue as single):
		self.name = name
		self.base = base
		self.bonus = bonus
		self.minValue = minValue
		self.maxValue = maxValue
		UpdateStat()
		
	def UpdateStat():
		value = Mathf.Max(base * (1 + (bonus + aura) / 100), 0)
		if maxValue > 0:
			value = Mathf.Clamp(value, minValue, maxValue)
