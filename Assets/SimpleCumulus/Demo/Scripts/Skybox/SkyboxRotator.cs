using UnityEngine;

namespace Assets.SimpleCumulus.Demo.Scripts.Skybox
{
	public class SkyboxRotator : MonoBehaviour
	{
		public float RotationPerSecond = 1;
		private bool _rotate;

		protected void Update()
		{
			if (_rotate) RenderSettings.skybox.SetFloat("_Rotation", Time.time * RotationPerSecond);
		}

		public void ToggleSkyboxRotation()
		{
			_rotate = !_rotate;
		}
	}
}