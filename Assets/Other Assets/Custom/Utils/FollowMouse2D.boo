import UnityEngine

class FollowMouse2D (MonoBehaviour): 

	public keys as (KeyCode) = (KeyCode.Mouse0, )
	public continuous as bool
	public alwaysFaceMouse as bool
	public moveSpeed as single = 3
	public turnSpeed as single = 10
	
	[HideInInspector] public targetPosition as Vector3
	
	mainCamera as Camera
	

	def Awake ():
		mainCamera = Camera.main
		targetPosition = transform.position
	
	def Update ():
		SetTargetPosition()
		Move()
			
	def SetTargetPosition():
		for key in keys:
			if continuous:
				if Input.GetKey(key):
					targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition)
					targetPosition.z = transform.position.z
			else:
				if Input.GetKeyDown(key):
					targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition)
					targetPosition.z = transform.position.z
				
	def Move():
		if (targetPosition - transform.position).magnitude > moveSpeed * Time.deltaTime:
			transform.rigidbody2D.MovePosition(transform.position + (targetPosition - transform.position).normalized * moveSpeed * Time.deltaTime)
			transform.rigidbody2D.MoveRotation(transform.LookingAt2D(targetPosition, 0, turnSpeed).eulerAngles.z)
		else:
			transform.position = targetPosition
			
		if alwaysFaceMouse:
			targetDirection as Vector3 = mainCamera.ScreenToWorldPoint(Input.mousePosition)
			targetDirection.z = transform.position.z
			transform.rigidbody2D.MoveRotation(transform.LookingAt2D(targetDirection, 0, turnSpeed).eulerAngles.z)
