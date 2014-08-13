import UnityEngine
import System.Collections

[ExecuteInEditMode]
class References (MonoBehaviour): 

	public infoText as GameObject
	public bark as GameObject
	public rock as GameObject
	public healthBar as GameObject
	
	public sprites as Sprites
	public bonusItems as BonusItems
	
	static public GameManager as GameObject
	static public Canvas as GameObject
	static public Map as MapUI
	static public RoomManager as RoomManager
	static public Rooms as GameObject
	static public MainCamera as Camera
	static public MainCameraFollow as SmoothFollow2D
	static public Player as Player
	static public BarkDict as Generic.Dictionary[of string, single]
	
	static public InfoTextID as int
	static public BarkID as int
	static public RockID as int
	static public HealthBarID as int
	
	def Awake():
		if Application.isPlaying:
			self.hideFlags = HideFlags.NotEditable
			for child as Transform in transform.GetChildren():
				child.gameObject.SetActive(true)
			SetReferences()
			for child as Transform in MainCamera.transform.GetChildren():
				child.gameObject.SetActive(true)
		else:
			self.SetExecutionOrder(-5)

	def Start():
		if Application.isPlaying:
			AudioPlayer.Play("Music_Scarab")

	def Update ():
		if not Application.isPlaying:
			transform.hideFlags = HideFlags.HideInInspector
			transform.position = Vector3.zero
			transform.rotation = Quaternion.identity
			transform.localScale = Vector3.one
		else:
			if Input.GetKeyDown(KeyCode.Escape):
				Application.Quit()
//			if Input.GetKeyDown(KeyCode.K):
//				Player.ApplyDamage(1000000000)
//			elif Input.GetKey(KeyCode.L):
//				SpawnRandomBonusItem(RoomManager.currentRoom.transform.position)
			
	def SetReferences():
		GameManager = gameObject
		Canvas = GameObject.Find("Canvas")
		Map = Canvas.transform.FindChild("Map").GetComponent(MapUI)
		RoomManager = FindObjectOfType[of RoomManager]()
		Rooms = GameObject.Find("Rooms")
		Player = FindObjectOfType[of Player]()
		BarkDict = GetBarkDict()
		InfoTextID = hObjectPool.Instance.Add(infoText)
		BarkID = hObjectPool.Instance.Add(bark)
		RockID = hObjectPool.Instance.Add(rock)
		HealthBarID = hObjectPool.Instance.Add(healthBar)
		MainCamera = Camera.main
		MainCameraFollow = MainCamera.GetComponent(SmoothFollow2D)
		
		Sprites.ExplosionID = hObjectPool.Instance.Add(sprites.explosion)
		Sprites.FireID = hObjectPool.Instance.Add(sprites.fire)
		
		BonusItems.MaxHealthItemID = hObjectPool.Instance.Add(bonusItems.maxHealthItem)
		BonusItems.ItemIDs.Add(BonusItems.MaxHealthItemID)
		BonusItems.DamageItemID = hObjectPool.Instance.Add(bonusItems.damageItem)
		BonusItems.ItemIDs.Add(BonusItems.DamageItemID)
		BonusItems.FireSpeedItemID = hObjectPool.Instance.Add(bonusItems.fireSpeedItem)
		BonusItems.ItemIDs.Add(BonusItems.FireSpeedItemID)
		BonusItems.ProjectileSpeedItemID = hObjectPool.Instance.Add(bonusItems.projectileSpeedItem)
		BonusItems.ItemIDs.Add(BonusItems.ProjectileSpeedItemID)
		BonusItems.MoveSpeedItemID = hObjectPool.Instance.Add(bonusItems.moveSpeedItem)
		BonusItems.ItemIDs.Add(BonusItems.MoveSpeedItemID)
		BonusItems.RegenerationItemID = hObjectPool.Instance.Add(bonusItems.regenerationItem)
		BonusItems.ItemIDs.Add(BonusItems.RegenerationItemID)
		BonusItems.AccuracyItemID = hObjectPool.Instance.Add(bonusItems.accuracyItem)
		BonusItems.BurstAmountItemID = hObjectPool.Instance.Add(bonusItems.burstAmountItem)
	
	class Sprites:
		public explosion as GameObject
		public fire as GameObject
		
		static public ExplosionID as int
		static public FireID as int

	class BonusItems:
		public maxHealthItem as GameObject
		public damageItem as GameObject
		public fireSpeedItem as GameObject
		public projectileSpeedItem as GameObject
		public moveSpeedItem as GameObject
		public regenerationItem as GameObject
		public accuracyItem as GameObject
		public burstAmountItem as GameObject
		
		static public ItemIDs as List[of int] = List[of int]()
		
		static public MaxHealthItemID as int
		static public DamageItemID as int
		static public FireSpeedItemID as int
		static public ProjectileSpeedItemID as int
		static public MoveSpeedItemID as int
		static public RegenerationItemID as int
		static public AccuracyItemID as int
		static public BurstAmountItemID as int
		