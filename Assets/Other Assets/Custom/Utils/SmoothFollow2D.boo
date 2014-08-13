import UnityEngine

[ExecuteInEditMode]
class SmoothFollow2D (MonoBehaviour): 

	public target as Transform
	public offset as Vector3
	public damping as Vector3 = Vector3(100, 100, 100)
	public useMaxDistance as bool
	public maxDistance as Vector3
	
	def Update():
		if not Application.isPlaying and target:
			damping.x = Mathf.Max(damping.x, 0)
			damping.y = Mathf.Max(damping.y, 0)
			damping.z = Mathf.Max(damping.z, 0)
			transform.position.x = target.position.x + offset.x
			transform.position.y = target.position.y + offset.y
			transform.position.z = target.position.z + offset.z
		elif Application.isPlaying and target:
			transform.position.x = Mathf.Lerp(transform.position.x, target.position.x + offset.x, damping.x * Time.deltaTime)
			transform.position.y = Mathf.Lerp(transform.position.y, target.position.y + offset.y, damping.y * Time.deltaTime)
			transform.position.z = Mathf.Lerp(transform.position.z, target.position.z + offset.z, damping.z * Time.deltaTime)
			if useMaxDistance:
				transform.position.x = Mathf.Clamp(transform.position.x, target.position.x - maxDistance.x, target.position.x + maxDistance.x)
				transform.position.y = Mathf.Clamp(transform.position.y, target.position.y - maxDistance.y, target.position.y + maxDistance.y)
				transform.position.z = Mathf.Clamp(transform.position.z, target.position.z - maxDistance.z, target.position.z + maxDistance.z)
			