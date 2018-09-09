using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
	public const String ITEM_TAG = "Item";
	public const String GROUND_TAG = "Ground";

	public Vector3 CurrentTarget;
	public double InteractionRange = 1;
	public double ThrowRange = 4;
	public float AttackRange = 1;
	public float AttackCooldown = 1;
	[Range(0, 100)] public double ZombificationLevel = 0;
	[Range(0, 100)] public double Health = 100;
	public GameObject Trap;
	public GameObject Stone;
	public GameObject Explosive;
	public double ZombificationPassiveIncrement;
	public double ZombificationDamageThreshold = 50;
	public double ZombificationDamage = 0.1;
	public GameObject ZombiePrefab;
	[Range(0, 1)] public double Armor = 0;

	public PlayerType Type = PlayerType.TANK;

	private NavMeshAgent _agent;
	private PlayerActionIntention _intention;
	private bool _selected;
	private double _lastAttackTime = 1;
	private AudioClip _arrowStoneAudio;

	private AudioSource _source;

	public Dictionary<ItemType, int> Items = new Dictionary<ItemType, int>();

	// Use this for initialization
	void Start()
	{
		Items[ItemType.PILLS] = 1;
		Items[ItemType.TRAPS] = 0;
		Items[ItemType.STONES] = 3;
		Items[ItemType.EXPLOSIVE] = 0;
		Items[ItemType.ARROW] = 0;
		_agent = GetComponent<NavMeshAgent>();
		_source = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{
		if (!IsAlive())
		{
			return;
			// TODO on animation finish delete?
		}

		if (_intention != null)
		{
			if (!_intention.Update())
			{
				_intention.Stop();
				_intention = null;
			}
		}

		if (_selected)
		{
			Ray clickRay;
			RaycastHit hit;
			if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
			{
				clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(clickRay, out hit))
				{
					var colliderGameObject = hit.collider.gameObject;
					if (colliderGameObject.CompareTag(ITEM_TAG))
					{
						PickUpItemIfInRange(colliderGameObject);
					}
					else if (colliderGameObject.CompareTag(GROUND_TAG))
					{
						MoveTo(hit.point);
					}
				}
			}

			// Only play for selected player
			SetZombieVolume();
		}

		if (Health > 0)
		{
			ZombificationLevel += Math.Min(ZombificationPassiveIncrement, 100);
			if (ZombificationLevel > ZombificationDamageThreshold)
			{
				Health = Math.Max(Health - ZombificationDamage, 0);
			}
		}

		if (ZombificationLevel >= 100)
		{
			TurnIntoZombie();
		}

		_lastAttackTime += Time.deltaTime;
		bool isWalking = _agent.remainingDistance > _agent.radius;
		GetComponentInChildren<Footsteps>().SetFootstepsPlaying(isWalking);
		GetComponent<Animator>().SetBool("Walking", isWalking);
	}

	private void TurnIntoZombie()
	{
		var position = transform.position;
		var rotation = transform.rotation;
		var createdZombie = Instantiate(ZombiePrefab, position, rotation);
		var zombie = createdZombie.GetComponent<Zombie>();
		zombie.WanderMode = WanderMode.AREA;
		zombie.AreaRange = 5;
		Destroy(gameObject);
	}

	private void SetZombieVolume()
	{
		GetComponentInChildren<PlayZombieSound>().SetZombiesVolume();
	}

	public void MoveTo(Vector3 location)
	{
		CurrentTarget = location;
		_agent.SetDestination(CurrentTarget);
		_intention = null;
	}

	public void TakeDamage(int amount)
	{
		Health -= amount - amount * Armor;
		GetComponent<Animator>().SetTrigger("Hit");
		_source.PlayOneShot(_source.clip);

		if (!IsAlive())
		{
			TurnIntoZombie();
		}
	}

	public void TakeInfection(int amount)
	{
		ZombificationLevel += amount - amount * Armor;
	}

	public void PickUpItemIfInRange(GameObject colliderGameObject)
	{
		if (!CanPickUpItemOfType(colliderGameObject.GetComponent<ItemObject>().Type))
		{
			return;
		}

		if (IsInRange(colliderGameObject.transform.position, InteractionRange))
		{
			PickUpItem(colliderGameObject);
			_intention = null;
		}
		else
		{
			_intention = new PlayerPickupActionIntention(this, colliderGameObject);
			_intention.Start();
		}
	}

	public void PickUpItem(GameObject colliderGameObject)
	{
		// TODO pickup animation
		var itemObject = colliderGameObject.GetComponent<ItemObject>();
		if (!CanPickUpItemOfType(itemObject.Type))
		{
			return;
		}

		Items[itemObject.Type] += 1;
		Destroy(colliderGameObject);
	}

	private bool IsInRange(Vector3 location, double range)
	{
		return Vector3.Distance(transform.position, location) < range;
	}

	public void PlaceTrapAtIfInRange(Vector3 location)
	{
		if (IsInRange(location, InteractionRange))
		{
			PlaceTrapAt(location);
		}
		else
		{
			_intention = new PlayerPlaceTrapActionIntention(this, location);
			_intention.Start();
		}
	}

	public void PlaceTrapAt(Vector3 location)
	{
		if (Items[ItemType.TRAPS] <= 0)
		{
			return;
		}

		// TODO place animation
		Instantiate(Trap, location, transform.rotation);
		Items[ItemType.TRAPS] -= 1;
	}

	public void SetSelected(bool selected)
	{
		_selected = selected;
	}

	public bool IsAlive()
	{
		return Health > 0;
	}

	public bool IsSelected()
	{
		return _selected;
	}

	public void TakePill()
	{
		if (Items[ItemType.PILLS] <= 0)
		{
			return;
		}

		Items[ItemType.PILLS] -= 1;
		ZombificationLevel = Math.Max(ZombificationLevel - 20, 0);
		GetComponentInChildren<DrinkPotion>().Drink();
	}

	public void ThrowStoneIfInRange(Vector3 location)
	{
		if (IsInRange(location, ThrowRange))
		{
			ThrowStone(location);
		}
		else
		{
			_intention = new PlayerThrowStoneActionIntention(this, location, ThrowRange);
			_intention.Start();
		}
	}

	public void ThrowStone(Vector3 location)
	{
		if (Items[ItemType.STONES] <= 0)
		{
			return;
		}

		var targetVector = location - transform.position;
		var stone = Instantiate(Stone, transform.position + new Vector3(0, 1, 0), transform.rotation);
		stone.GetComponent<Rigidbody>().AddForce(targetVector);
		Items[ItemType.STONES] -= 1;
	}

	public bool CanPickUpItemOfType(ItemType item)
	{
		switch (item)
		{
			case ItemType.ARROW:
				return Type == PlayerType.ARCHER;
			case ItemType.EXPLOSIVE:
				return Type == PlayerType.MAGE;
			default:
				return true;
		}
	}

	public void TryShootArrowAt(Zombie zombie)
	{
		if (IsInRange(zombie.transform.position, AttackRange))
		{
			ShootArrow(zombie);
		}
		else
		{
			_intention = new PlayerShootArrowActionItention(this, zombie, AttackRange);
			_intention.Start();
		}
	}

	public void ShootArrow(Zombie target)
	{
		if (!IsInRange(target.transform.position, AttackRange) || Items[ItemType.ARROW] <= 0 || !CanAttack())
		{
			return;
		}

		Debug.Log("Attacking!");
		Items[ItemType.ARROW] -= 1;
		Destroy(target.gameObject);
		_lastAttackTime = 0;
	}

	public void TryThrowExplosiveAt(Zombie zombie)
	{
		if (IsInRange(zombie.transform.position, AttackRange))
		{
			ThrowExplosiveAt(zombie);
		}
		else
		{
			_intention = new PlayerThrowExplosiveActionItention(this, zombie, AttackRange);
			_intention.Start();
		}
	}

	public void ThrowExplosiveAt(Zombie target)
	{
		if (!IsInRange(target.transform.position, AttackRange) || Items[ItemType.EXPLOSIVE] <= 0 || !CanAttack())
		{
			return;
		}

		Debug.Log("Attacking!");
		Items[ItemType.EXPLOSIVE] -= 1;
		var targetVector = target.transform.position - transform.position;
		var explosive = Instantiate(Explosive, transform.position + new Vector3(0, 1, 0), transform.rotation);
		explosive.GetComponent<Rigidbody>().AddForce(targetVector);
		_lastAttackTime = 0;
	}

	public void PushAway()
	{
		if (!CanAttack())
		{
			return;
		}

		Debug.Log("Attacking!");
		GetComponent<Animator>().SetBool("Attack", _agent.remainingDistance > _agent.radius);
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, AttackRange, LayerMask.GetMask("Target"));
		foreach (var targetCollider in targetsInViewRadius)
		{
			var targetZombie = targetCollider.gameObject;
			var force = targetZombie.transform.position - transform.position;
			targetZombie.GetComponent<Rigidbody>().AddForce(force.normalized * 5);
		}

		_lastAttackTime = 0;
	}

	public bool CanAttack()
	{
		return _lastAttackTime >= AttackCooldown;
	}
}