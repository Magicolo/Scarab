import UnityEngine

[RequireComponent(Camera)]
[ExecuteInEditMode]
class CopyCamera (MonoBehaviour): 
	enum CopyMode:
		Everything
		Nothing
		Custom
	public cameraToCopy as Camera
	public copy as CopyMode = CopyMode.Custom
	public copyDetails as CopyDetails

	def Update ():
		if copy == CopyMode.Everything:
			copyDetails.position = true
			copyDetails.rotation = true
			copyDetails.clearFlags = true
			copyDetails.background = true
			copyDetails.cullingMask = true
			copyDetails.projection = true
			copyDetails.fieldOfView_Size = true
			copyDetails.clippingPlanes = true
			copyDetails.viewportRect = true
			copyDetails.depth = true
			copyDetails.renderingPath = true
			copyDetails.targetTexture = true
			copyDetails.occlusionCulling = true
			copyDetails.HDR = true
		elif copy == CopyMode.Nothing:
			copyDetails.position = false
			copyDetails.rotation = false
			copyDetails.clearFlags = false
			copyDetails.background = false
			copyDetails.cullingMask = false
			copyDetails.projection = false
			copyDetails.fieldOfView_Size = false
			copyDetails.clippingPlanes = false
			copyDetails.viewportRect = false
			copyDetails.depth = false
			copyDetails.renderingPath = false
			copyDetails.targetTexture = false
			copyDetails.occlusionCulling = false
			copyDetails.HDR = false
			
		if cameraToCopy:
			if copyDetails.position:
				if cameraToCopy.transform.position != transform.position:
					transform.position = cameraToCopy.transform.position
			if copyDetails.rotation:
				if cameraToCopy.transform.rotation != transform.rotation:
					transform.rotation = cameraToCopy.transform.rotation
			if copyDetails.clearFlags:
				if cameraToCopy.clearFlags != camera.clearFlags:
					camera.clearFlags = cameraToCopy.clearFlags
			if copyDetails.cullingMask:
				if cameraToCopy.cullingMask != camera.cullingMask:
					camera.cullingMask = cameraToCopy.cullingMask
			if copyDetails.background:
				if cameraToCopy.backgroundColor != camera.backgroundColor:
					camera.backgroundColor = cameraToCopy.backgroundColor
			if copyDetails.projection:
				if cameraToCopy.isOrthoGraphic != camera.isOrthoGraphic:
					camera.isOrthoGraphic = cameraToCopy.isOrthoGraphic
			if copyDetails.fieldOfView_Size:
				if cameraToCopy.isOrthoGraphic and camera.isOrthoGraphic:
					if cameraToCopy.orthographicSize != camera.orthographicSize:
						camera.orthographicSize = cameraToCopy.orthographicSize
				elif not cameraToCopy.isOrthoGraphic and not camera.isOrthoGraphic:
					if cameraToCopy.fieldOfView != camera.fieldOfView:
						camera.fieldOfView = cameraToCopy.fieldOfView
			if copyDetails.clippingPlanes:
				if cameraToCopy.farClipPlane != camera.farClipPlane or cameraToCopy.nearClipPlane != camera.nearClipPlane:
					camera.farClipPlane = cameraToCopy.farClipPlane
					camera.nearClipPlane = cameraToCopy.nearClipPlane
			if copyDetails.viewportRect:
				if cameraToCopy.rect != camera.rect:
					camera.rect = cameraToCopy.rect
			if copyDetails.depth:
				if cameraToCopy.depth != camera.depth:
					camera.depth = cameraToCopy.depth
			if copyDetails.renderingPath:
				if cameraToCopy.renderingPath != camera.renderingPath:
					camera.renderingPath = cameraToCopy.renderingPath
			if copyDetails.targetTexture:
				if cameraToCopy.targetTexture != camera.targetTexture:
					camera.targetTexture = cameraToCopy.targetTexture
			if copyDetails.occlusionCulling:
				if cameraToCopy.useOcclusionCulling != camera.useOcclusionCulling:
					camera.useOcclusionCulling = cameraToCopy.useOcclusionCulling
			if copyDetails.HDR:
				if cameraToCopy.hdr != camera.hdr:
					camera.hdr = cameraToCopy.hdr	
		else:
			if transform.parent:
				if transform.parent.GetComponent(Camera):
					cameraToCopy = transform.parent.GetComponent(Camera)
			else:
				cameraToCopy = Camera.main				

	class CopyDetails:
		public position as bool
		public rotation as bool
		public clearFlags as bool
		public background as bool
		public cullingMask as bool
		public projection as bool
		public fieldOfView_Size as bool
		public clippingPlanes as bool
		public viewportRect as bool
		public depth as bool
		public renderingPath as bool
		public targetTexture as bool
		public occlusionCulling as bool
		public HDR as bool