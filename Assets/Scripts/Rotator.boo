import UnityEngine

class Rotator (MonoBehaviour): 

	public rotateSpeed as single = 1
	public useRigidbody as bool = true
	
	def Update():
		if useRigidbody:
			rigidbody2D.MoveRotation(transform.eulerAngles.z + (rotateSpeed * 360 * Time.deltaTime))
		else:
			transform.eulerAngles.z += rotateSpeed * 360 * Time.deltaTime
