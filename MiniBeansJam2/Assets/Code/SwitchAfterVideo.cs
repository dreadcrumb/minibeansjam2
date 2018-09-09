using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SwitchAfterVideo : MonoBehaviour
{
	public int NextScene;
	private VideoPlayer _video;

	private void Start()
	{
		_video = GetComponent<VideoPlayer>();
		_video.loopPointReached += GoToNextScene;
	}

	private void GoToNextScene(VideoPlayer source)
	{
		SceneManager.LoadScene(NextScene);
	}
}
