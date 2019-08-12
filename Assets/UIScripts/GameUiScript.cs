using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.MidiPlayer.Scripts.MPTKGameObject;
using Assets.MidiPlayer.Scripts.MPTKMidi;
using Assets.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.UIScripts
{
	public class GameUiScript : MonoBehaviour
	{
		//public MidiFile midi;
		//public float ticks;
		//public float offset;

		private Stopwatch watch;
		private MidiFilePlayer midiPlayer;
		private Queue<TrackMidiEvent> eventQueue;

		public RectTransform energyBar;
		public RectTransform bluesBar;
		public RectTransform barContainer;
		public RectTransform noteAcceptanceArea;
		public RectTransform noteBackgroundArea;
		public GameObject notePrefab;
		public GameObject notePrefab1;
		public GameObject notePrefab2;
		public GameObject notePrefab3;
		public GameObject notePrefab4;
		GameObject noteTrailPrefab;
		public List<GameObject> noteList;
		private int noteCount;

		public int SetEnergyBarFillPercentage;
		public int SetBluesBarFillPercentage;

		private float onePercentSize;
		private float EnergyCurrentPerc = 100f;
		private float BluesCurrentPerc = 10f;

		private float startTime;

		public double noteSpeed = 20;

		public bool startPlaying = false;

		public bool isClicked;

		private GameObject gameController;

		private double _timeOffsetTicks;
		private float _dist;
		private Stopwatch _timer;

		// Use this for initialization
		void Start()
		{
			gameController = GameObject.FindGameObjectWithTag(Const.Tags.GameController);

			InitNoteList();

			float PercentageBarMaxWidth = barContainer.sizeDelta.x;
			onePercentSize = (PercentageBarMaxWidth / 100f);

			_dist = Vector3.Distance(new Vector3(-6.38f /* - .95f /* Half of sprite */, 2f, 1.05f),
				new Vector3(-1f, 2f, 1f));
			_timeOffsetTicks = _dist / noteSpeed * 1000;

			SetBluesPercentage(10);
		}

		private void InitNoteList()
		{
			noteList = new List<GameObject>
			{
				notePrefab,
				notePrefab1,
				notePrefab2,
				notePrefab3,
				notePrefab4,
			};
			noteCount = noteList.Count;
		}

		public void TriggerUiScript(string songName)
		{
			var MIDIPlayerGameObject = GameObject.FindGameObjectWithTag(Const.Tags.MidiPlayer);
			midiPlayer = MIDIPlayerGameObject.GetComponent<MidiFilePlayer>();
			midiPlayer.MPTK_MidiName = songName;

			midiPlayer.OnEventNotesMidi = new MidiFilePlayer.ListNotesEvent();
			midiPlayer.OnEventNotesMidi.AddListener(NotesToPlay);

			var notes = midiPlayer.MPTK_MidiEvents;

			midiPlayer.MPTK_Play();
			midiPlayer.MPTK_LogEvents = true;

			noteSpeed = midiPlayer.MPTK_Tempo / 25;

			var name = midiPlayer.MPTK_MidiName;

			var instrumentName = midiPlayer.MPTK_TrackInstrumentName;

			watch = new Stopwatch();
			watch.Start();
		}


		public void NotesToPlay(List<MidiNote> notes)
		{

			foreach (var note in notes)
			{
				SpawnNote(note);
			}

			midiPlayer.MPTK_PlayNotes(notes);
		}

		public void SpawnNote(MidiNote note)
		{
			var noteSpawn = GameObject.FindGameObjectWithTag(Const.Tags.NoteSpawn);
			var noteSpawnVec = noteSpawn.transform.position;

			var vec = new Vector3(-6.38f, 2f, -1.05f);
			float timeOffset;

			GameObject newNote = Instantiate(GetRandomNoteSprite(), noteSpawnVec, Quaternion.Euler(90, 0, 0));

			note.Delay = (float)(Math.Abs(noteSpawnVec.x / noteSpeed * 1000)) - 100;

			var dist = /*noteEvent.Event.DeltaTime * */(noteSpeed * Time.deltaTime);
			timeOffset = (float)(dist / (noteSpeed * Time.deltaTime));

			newNote.GetComponent<NoteBehavior>().InitNoteSpeed(noteSpeed, timeOffset);
		}


		private GameObject GetRandomNoteSprite()
		{

			int rand = Random.Range(0, noteCount);
			return noteList[rand];
		}

		public void IncreaseEnergy(float percentageToAdd)
		{
			float perc = EnergyCurrentPerc + percentageToAdd;
			if (perc > 100f)
				perc = 100f;
			else if (perc < 0f)
				perc = 0f;

			energyBar.sizeDelta = new Vector2(perc * onePercentSize, energyBar.sizeDelta.y); // m = dur * speed
			EnergyCurrentPerc = perc;
		}

		public void IncreaseBlues(float percentageToAdd)
		{
			float perc = BluesCurrentPerc + percentageToAdd;
			if (perc > 100f)
				perc = 100f;
			else if (perc < 0f)
				perc = 0f;

			bluesBar.sizeDelta = new Vector2(perc * onePercentSize, bluesBar.sizeDelta.y);
			BluesCurrentPerc = perc;
		}

		public void SetEnergyPercentage(float setPercentage)
		{
			if (setPercentage > 100f)
				setPercentage = 100f;
			else if (setPercentage < 0f)
				setPercentage = 0f;

			energyBar.sizeDelta = new Vector2(setPercentage * onePercentSize, energyBar.sizeDelta.y);
			EnergyCurrentPerc = setPercentage;
		}

		public void SetBluesPercentage(float setPercentage)
		{
			if (setPercentage > 100f)
				setPercentage = 100f;
			else if (setPercentage < 0f)
				setPercentage = 0f;

			bluesBar.sizeDelta = new Vector2(setPercentage * onePercentSize, bluesBar.sizeDelta.y);
			BluesCurrentPerc = setPercentage;
		}

		public void SetClicked()
		{
			isClicked = true;
		}
	}
}