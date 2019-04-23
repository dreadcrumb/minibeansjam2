using UnityEngine;

namespace Assets.MidiPlayer.Scenes_For_Demo.Others_Demos.Script
{
	public class MovePoint : MonoBehaviour {
		void OnDrawGizmos(){
			Gizmos.color=Color.magenta;
			Gizmos.DrawWireSphere(transform.position,.25f);	
		}
	}
}
