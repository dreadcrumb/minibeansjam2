using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour 
{
	public Dialogue dialogue;
	private bool _started = false;

	public void TriggerDialogue ()
	{
		FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
	}

	private void Update()
	{
		if (!_started)
		{
			TriggerDialogue();
			_started = true;
		}
	}
}
