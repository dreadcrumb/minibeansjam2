using UnityEngine;

namespace Assets.MidiPlayer.Scenes_For_Demo.Others_Demos.Script
{
	public class LookPoint : MonoBehaviour {
		void OnDrawGizmos(){
			Gizmos.color=Color.cyan;
			Gizmos.DrawWireSphere(transform.position,.25f);	
		}
	}
}