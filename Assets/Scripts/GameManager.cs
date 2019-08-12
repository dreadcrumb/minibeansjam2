using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.UIScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class GameManager : MonoBehaviour
	{
		#region Const

		private const float SpawnOffset = 8f;

		#endregion
		#region Members

		private GameObject _dino;

		private List<GameObject> _meatBags;

		private GameObject[] _notes;

		private GameObject _spawnArea;

		private GameObject _UIController;

		private GameObject _noteHitArea;

		private float _nextActionTime = 0.0f;

		private ParticleSystem[] _bloodSpatters;

		private Animator _anim;
		private Animator _animCrowd;

		private AudioSource[] _sounds;

		private List<AudioSource> _eatingSounds;
		private List<AudioSource> _schnappSounds;

		private AudioSource _dinoBluesNoInst;
		private AudioSource _dinoFairNoInst;
		private AudioSource _fressAtackeNoInst;
		#endregion Members

		#region Properties

		public float Hunger;

		public float Saturation;

		public int Blues;

		public int BluesGoal;

		public bool Running = true;

		public float missedNotePenalty;

		public float MeatbagSpawnProbability;

		public GameObject MeatbagPrefab;

		public float MinSpawnTime;

		int _eatAnimHash = Animator.StringToHash(Const.Animations.DinoEating);
		private AudioSource _dinoBluesInst;
		private AudioSource _fressAtackeInst;
		private AudioSource _dinoFairInst;
		private AudioSource _curTrack;

		#endregion Properties

		// Use this for initialization
		void Start()
		{
			// Dino stuff
			_dino = GameObject.FindGameObjectWithTag(Const.Tags.Player);
			_bloodSpatters = _dino.GetComponentsInChildren<ParticleSystem>();
			_anim = _dino.GetComponent<Animator>();


			// other GameObjects
			_meatBags = new List<GameObject>();
			_spawnArea = GameObject.FindGameObjectWithTag(Const.Tags.Spawn);

			_UIController = GameObject.FindGameObjectWithTag(Const.Tags.UIController);

			_noteHitArea = GameObject.FindGameObjectWithTag(Const.Tags.NoteHitArea);

			SetAllSounds();

			StartCoroutine(Const.Coroutine.ApplyHunger);
		}

		private void SetAllSounds()
		{
			_sounds = gameObject.GetComponentsInChildren<AudioSource>();

			_eatingSounds = _sounds.Where(x => x.clip.name.Contains(Const.AudioSources.Eating)).ToList();
			_schnappSounds = _sounds.Where(x => x.clip.name.Contains(Const.AudioSources.Bite)).ToList();

			var script = GetComponentInChildren<GameUiScript>();
			script.TriggerUiScript(LevelSave.Level);
		}

		void Update()
		{
			if (!Running || Saturation <= 0)
			{
				SceneManager.LoadScene(2);
			}

			#region Meatbag creation

			//var creationProbability = Blues * MeatbagSpawnRate; // Note: * random?
			float fBlues = Blues;
			var spawn = fBlues / 100;
			if (Time.time > _nextActionTime && Random.value < fBlues)
			{
				_nextActionTime += MinSpawnTime;
				var meatBag = Instantiate(MeatbagPrefab, GetSpawnPoint(), new Quaternion());
				_meatBags.Add(meatBag);
			}

			#endregion Meatbag creation


			#region InputHandling

			if (Input.GetMouseButtonDown(0))
			{
				RaycastHit hit;
				var touchedObj = ReturnClickedObject(out hit);
				if (touchedObj != null)
				{
					if (touchedObj.CompareTag(Const.Tags.Player) || touchedObj.CompareTag(Const.Tags.Head))
					{
						_anim.Play(_eatAnimHash);

						foreach (GameObject meatBag in _meatBags)
						{
							_animCrowd = meatBag.GetComponentInChildren<Animator>();
							_animCrowd.Play(Animator.StringToHash(Const.Animations.MeatbagEaten));
						}

						int rand = Random.Range(0, _schnappSounds.Count());
						_schnappSounds[rand].Play();
						_UIController.GetComponent<GameUiScript>().IncreaseEnergy(Saturation);
					}
					else if (touchedObj.CompareTag(Const.Tags.Note))
					{
						StartCoroutine(Const.Coroutine.FadeNote);
						float points;
						float x = System.Math.Abs(touchedObj.transform.position.x);
						if (_noteHitArea.GetComponent<RectTransform>().sizeDelta.x / 2 < x)
							points = -missedNotePenalty;
						else
						{
							if (x.Equals(0))
							{
								x = 0.001f;
							}
							// TODO
							//if (_meatBags.Count > 0)
							//	points = 1 / mapNumber(x, 0, touchedObj.transform.position.x, 0, 1) * _meatBags.Count; //remap distance to 0-1
							//else
							//	points = 1 / mapNumber(x, 0, touchedObj.transform.position.x, 0, 1);
							points = _meatBags.Count;
						}

						float percentage = BluesGoal / 100 * points;
						_UIController.GetComponent<GameUiScript>().IncreaseBlues(percentage); //Punkte von 0-1000

						Destroy(touchedObj.gameObject);
					}
				}
			}
			#endregion InputHandling
		}

		void OnCollision(Collider col)
		{
			var x = 5;
		}

		//Method to Return Clicked Object
		GameObject ReturnClickedObject(out RaycastHit hit)
		{
			GameObject target = null;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray.origin, ray.direction * 50, out hit))
			{
				target = hit.collider.gameObject;
			}
			return target;
		}

		//Method to Return Clicked Object
		GameObject ReturnClickedObject(Touch t, out RaycastHit hit)
		{
			GameObject target = null;
			Ray ray = Camera.main.ScreenPointToRay(t.position);
			if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
			{
				target = hit.collider.gameObject;
			}
			return target;
		}

		//Method to Return Clicked Ui Object
		bool ReturnClickedUiObject(Image imgIn)
		{
			if (imgIn.Raycast(new Vector2(10, 100), Camera.main))
				return true;
			return false;
		}

		IEnumerator ApplyHunger()
		{
			for (float i = 100; i >= 0; i -= Hunger)
			{
				Saturation -= Hunger;
				i = Saturation;
				_UIController.GetComponent<GameUiScript>().SetEnergyPercentage(Saturation);
				yield return null;
			}
			// i >= 0
			Running = false;
			// Load Game-Over Screen
		}

		Vector3 GetSpawnPoint()
		{
			Vector3 spawnPoint = _spawnArea.transform.position;
			if (Random.value < 0.5f)
			{
				// Spawn left
				spawnPoint -= _spawnArea.transform.right * SpawnOffset;
			}
			else
			{
				// Spawn right
				spawnPoint += _spawnArea.transform.right * SpawnOffset;
			}
			return spawnPoint;
		}

		public void RemoveMeatbag(GameObject meatBag)
		{
			_meatBags.Remove(meatBag);
		}

		public void Eat(GameObject meatBag)
		{
			Saturation += 10;
			_meatBags.Remove(meatBag);
			foreach (var splat in _bloodSpatters)
			{
				splat.Play();
			}

			int rand = Random.Range(0, _eatingSounds.Count());
			_eatingSounds[rand].Play();
		}

		IEnumerator PlayInstrument()
		{
			_curTrack.volume = 1;
			for (int i = 0; i < 6; i++)
			{
				_curTrack.volume -= 0.1f;
				yield return new WaitForSeconds(0.1f);

			}
		}

		public void SetGameActive(bool b)
		{
			Running = false;
		}

		public void FadeNote(GameObject note)
		{
			note.GetComponent<Image>().CrossFadeAlpha(0, 0.5f, true);
			note.GetComponent<Image>().CrossFadeColor(Color.green, 0.5f, true, false, true);
		}
	}
}