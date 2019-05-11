using UnityEngine;

namespace Assets.Scripts
{
	public class DinoController : MonoBehaviour
	{
		public Animator Anim;
		int TriggerHash = Animator.StringToHash("buttonclick");

		public void EatTrigger()
		{
			Anim.SetTrigger(TriggerHash);
		}
	}
}
