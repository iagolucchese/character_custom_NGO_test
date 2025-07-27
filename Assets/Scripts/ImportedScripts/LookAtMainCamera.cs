using UnityEngine;

namespace ImportedScripts
{	
	[AddComponentMenu("Utils/Look At Main Camera")]
	public class LookAtMainCamera : MonoBehaviour
	{
		private Transform lookAtTransform;
		
		private void Awake()
		{
			Camera mainCam = Camera.main;
			if (mainCam != null)
				lookAtTransform = mainCam.transform;
		}

		private void Update()
		{
			UpdateLookAtRotation();
		}

		private void UpdateLookAtRotation()
		{
			if (lookAtTransform == null)
			{
				enabled = false;
				return;
			}
			transform.rotation = Quaternion.LookRotation(lookAtTransform.forward);
		}
		
		[ContextMenu("Rotate To Camera")]
		public void RotateToCamera()
		{
			if (Camera.main)
				transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
		}
	}
}
