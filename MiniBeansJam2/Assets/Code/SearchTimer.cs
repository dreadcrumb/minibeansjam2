using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchTimer : MonoBehaviour
{

	private static SearchTimer _instance;

	public Text timerLabel;
	public int searchingTime;

	private float time;
	private bool timerRunning;

	void Awake()
	{
		if (!_instance)
		{ _instance = this; }
		else
		{ Destroy(this.gameObject); }

		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if (timerRunning)
		{
			if (time < searchingTime)
			{
				time += Time.deltaTime;

				var minutes = Mathf.Floor(time / 60);
				var seconds = time % 60;//Use the euclidean division for the seconds.
				var fraction = (time * 100) % 100;

				timerLabel.text = string.Format("{0:00} : {1:00} : {2:00}", minutes, seconds, fraction);
			}
			else
			{
				timerRunning = false;
			}
		}
	}

	//Reset Timer
	public void ResetTimer()
	{
		time = 0;
		Debug.Log("Timer Reset");
	}

	//Stop Timer
	public void StopTimer()
	{
		timerRunning = false;
	}

	public void StartTimer()
	{
		timerRunning = true;
	}

	public bool IsTimerRunning()
	{
		return timerRunning;
	}

	public float GetElapsed()
	{
		return time;
	}
}
