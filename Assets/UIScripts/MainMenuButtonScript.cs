using System.IO;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.UIScripts
{
	public class MainMenuButtonScript : MonoBehaviour
	{
		public Animator anim;
		public GameObject LevelButtonPrefab;

		private readonly int TriggerHash = Animator.StringToHash(Const.UI.MenuReset);
		private int numLevels = 0;

		public void Start()
		{
			//	var levelButtonPrefab = Resources.Load(Const.File.LevelButtonPrefab);
		}

		public void PlayGame()
		{
			LevelSave.Level = 0;
			SceneManager.LoadScene(1);
		}

		public void QuitGame()
		{
			Application.Quit();
		}

		public void PlayLevel(int scene)
		{
			LevelSave.Level = scene;
			SceneManager.LoadScene(1);
		}

		public void TriggerMenuBack()
		{
			anim.SetTrigger(TriggerHash);
		}

		public void TriggerMenuLevels()
		{
			anim.SetTrigger(TriggerHash);

			var midiFolder = new DirectoryInfo(Const.File.MIDI_Location);
			var midiFiles = midiFolder.GetFiles().Where(x => x.Extension.Equals(Const.File.MIDI_FileExtension)).ToList();

			if (midiFiles.Count != numLevels)
			{
				var startButtonGameObject = GameObject.Find(Const.Tags.StartButton);
				var startButton = startButtonGameObject.GetComponent<Button>() as Button;
				int levelNumber = 1;

				foreach (var midiFile in midiFiles)
				{
					var newLevelButtonGameObject = Instantiate(LevelButtonPrefab) as GameObject;
					var levelPanel = GameObject.Find((Const.Tags.LevelPanel)).transform;
					newLevelButtonGameObject.transform.SetParent(levelPanel);
					newLevelButtonGameObject.transform.position = startButtonGameObject.transform.position;
					newLevelButtonGameObject.transform.Rotate(0, -90, 0);

					var newLevelButton = newLevelButtonGameObject.GetComponent<Button>() as Button;
					var buttonText = newLevelButton.GetComponentInChildren<Text>();
					buttonText.text = Const.Tags.Level + levelNumber;

					newLevelButton.image.rectTransform.localScale = new Vector3(1, 1, 1);
					newLevelButton.image.rectTransform.Translate(new Vector3(0,
						-newLevelButton.image.rectTransform.sizeDelta.y / 4 * (levelNumber - 1))); //All hail the magic 4
					levelNumber++;
				}

				numLevels = midiFiles.Count;
			}
		}
	}
}
