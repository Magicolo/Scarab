import UnityEngine

class BonusItem (MonoBehaviour, IPickable): 

	public minBonus as single = 25
	public maxBonus as single = 150
	
	[HideInInspector] public bias as single = 1
	[HideInInspector] public bonus as single
	[HideInInspector] public bonusText as InfoText
	[HideInInspector] public generated as bool
	
	initScale as Vector3
	spriteAnimator as Animator
	
	def Awake():
		initScale = transform.localScale
		spriteAnimator = transform.FindChild("Sprite").GetComponent(Animator)
		
	def OnEnable():
		spriteAnimator.Play("BonusItem")
		if not generated:
			bonus = Mathf.Clamp(HF.ProportionalRandomRange(minBonus, maxBonus) * bias, minBonus, maxBonus).Round(1)
			transform.localScale = initScale * (bonus / maxBonus) * 3
			generated = true
		
	virtual def ApplyBonus(player as Player):
		source as AudioSource = AudioPlayer.Play("SFX_BonusItem_Pickup")
		source.pitch = 1.5 - bonus / maxBonus
