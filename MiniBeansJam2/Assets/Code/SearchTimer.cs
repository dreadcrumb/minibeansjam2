using UnityEngine;

public class SearchTimer : MonoBehaviour
{
	private float time;
	private bool timerRunning;

	public SearchTimer()
	{
		time = 0;
		timerRunning = false;
	}

	void Start()
	{
	}

	void Update()
	{
		if (timerRunning)
		{
			time += Time.deltaTime;

			var minutes = Mathf.Floor(time / 60);
			var seconds = time % 60;//Use the euclidean division for the seconds.
			var fraction = (time * 100) % 100;
		}
	}

	//Reset Timer
	public void ResetTimer()
	{
		time = 0;
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
