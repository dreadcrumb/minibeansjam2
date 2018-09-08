using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour 
{
	
	public AudioMixer mainMixer;

	public void SetMusicdB (float volume)
	{
		if(volume > -20F)
			mainMixer.SetFloat ("musicVol", volume);
		else
			mainMixer.SetFloat ("musicVol", -80F);
	}

	public void SetSFXdB (float volume)
	{
		if(volume > -20F)
		mainMixer.SetFloat ("sfxVol", volume);
		else
		mainMixer.SetFloat ("sfxVol", -80F);	
	}
}
