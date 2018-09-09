using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour 
{

	public Text nameText;
	public Text dialogueText;
	public Image profilImage;

	public CanvasGroup mainCG;

	private Queue<string> sentences;
	public int DialogFinishedScene;

	// Use this for initialization
	void Start () {
		sentences = new Queue<string> ();
	}

	public void StartDialogue (Dialogue dialogue)
	{
		mainCG.alpha = 1F;
		mainCG.blocksRaycasts = true;
		mainCG.interactable = true;

		profilImage.sprite = dialogue.person;
		nameText.text = dialogue.name;

		sentences.Clear ();

		foreach (string sentence in dialogue.sentences) 
		{
			sentences.Enqueue (sentence);	
		}

		DisplayNextSentence ();
	}

	public void DisplayNextSentence ()
	{
		if (sentences.Count == 0) {
		
			EndDialogue ();
			return;
		}

		string sentece = sentences.Dequeue ();
		StopAllCoroutines ();
		StartCoroutine (TypeSentence (sentece));
//		dialogueText.text = sentece;
	}

	IEnumerator TypeSentence (string sentence)
	{
		dialogueText.text = "";
		foreach (char letter in sentence) 
		{
			dialogueText.text += letter;
			yield return null;
		}
	}

	void EndDialogue()
	{
		mainCG.alpha = 0F;
		mainCG.blocksRaycasts = false;
		mainCG.interactable = false;
		SceneManager.LoadScene(DialogFinishedScene);
	}
}
