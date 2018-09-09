using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
	public int WinningScene;
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			SceneManager.LoadScene(WinningScene);
		}
	}
}
