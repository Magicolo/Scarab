import UnityEngine

class Stealth (Enemy): 

	override def Start():
		pass
		
	override def Move():
		if References.Player != null:
			distance as single = Vector3.Distance(transform.position, References.Player.transform.position)
			if distance < sightRadius:
				playerInRange = true
				if distance < minDistance:
					transform.rigidbody2D.MovePosition(transform.position + (transform.position - References.Player.transform.position).normalized * MoveSpeed * Time.deltaTime)
					transform.rigidbody2D.MoveRotation(transform.LookingAt2D(References.Player.transform, 0, Mathf.Max(MoveSpeed, 1)).eulerAngles.z)
				else:
					transform.rigidbody2D.MovePosition(transform.position + transform.TransformDirection(Vector2.right) * MoveSpeed * Time.deltaTime)
					transform.rigidbody2D.MoveRotation(transform.LookingAt2D(References.Player.transform, 0, Mathf.Max(MoveSpeed, 1)).eulerAngles.z)
			else:
				playerInRange = false
		else:
			playerInRange = false
			