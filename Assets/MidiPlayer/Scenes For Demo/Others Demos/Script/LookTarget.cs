using UnityEngine;

namespace Assets.MidiPlayer.Scenes_For_Demo.Others_Demos.Script
{
	public class LookTarget : MonoBehaviour {
		void OnDrawGizmos(){
			Gizmos.color=Color.cyan;
			Gizmos.DrawSphere(transform.position,.25f);	
		}
	}
}
