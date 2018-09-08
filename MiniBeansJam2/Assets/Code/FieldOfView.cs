using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;

public class FieldOfView : MonoBehaviour
{
	public float ViewRadius;
	[Range(0, 360)]
	public float ViewAngle;
	[Range(0, 360)]
	public float MaxViewRotation;

	public float ViewSpeed;

	public LayerMask TargetMask;
	public LayerMask ObstacleMask;

	public MeshRenderer FovMeshRenderer;

	[HideInInspector]
	public List<Transform> VisibleTargets = new List<Transform>();

	public float MeshResolution;
	public int EdgeResolveIterations;
	public float EdgeDstThreshold;

	public MeshFilter ViewMeshFilter;

	private Mesh _viewMesh;

	private GameObject _enemyTarget;
	private bool _showFov;

	private float _rotationStep;
	private bool _increasing;

	private Vector3 _lookDir;

	private Vector3 _lastPointPlayerSeen;

	void Start()
	{
		_viewMesh = new Mesh();
		_viewMesh.name = "View Mesh";
		ViewMeshFilter.mesh = _viewMesh;

		_lookDir = transform.forward;
		StartCoroutine("FindTargetsWithDelay", .2f);
	}

	IEnumerator FindTargetsWithDelay(float delay)
	{
		while (true)
		{
			yield return new WaitForSeconds(delay);
			FindVisibleTargets();
		}
	}

	void LateUpdate()
	{
		DrawFieldOfView();
	}

	void Update()
	{
		switch (GetComponentInParent<Zombie>().GetZombieState())
		{
			case ZombieState.IDLE:
				break;
			case ZombieState.FOLLOWING:
				if (_enemyTarget != null)
				{
					var zombieComponent = GetComponentInParent<Zombie>();
					zombieComponent.Target = _enemyTarget;
				}

				break;
			case ZombieState.ALARMED:
				break;
		}
	}

	void FindVisibleTargets()
	{
		PingPongViewAngle();
		VisibleTargets.Clear();

		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, ViewRadius, TargetMask);
		if (targetsInViewRadius.Length > 0)
		{
			for (int i = 0; i < targetsInViewRadius.Length; i++)
			{
				var current = targetsInViewRadius[i];
				//if (!current.GetComponent<Player>().IsAlive())
				//{
				//	continue;
				//}
				Transform target = current.transform;
				Vector3 dirToTarget = (target.position - transform.position).normalized;

				if (Vector3.Angle(_lookDir, dirToTarget) < ViewAngle / 2)
				{
					float dstToTarget = Vector3.Distance(transform.position, target.position);
					if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, ObstacleMask))
					{
						//visibleTargets.Add(target);
						_lookDir = dirToTarget;
						GetComponentInParent<Zombie>().SetAgentDestination(target.transform.position); // Does not work properly
						_rotationStep = 0;

						GetComponentInParent<Zombie>().SetZombieState(ZombieState.FOLLOWING);
						_enemyTarget = target.gameObject;
						ViewSpeed = 15;
						Debug.DrawLine(transform.position, target.position, Color.black);

						_lastPointPlayerSeen = target.transform.position;
					}
					else
					{
						if (GetComponentInParent<Zombie>().GetZombieState() == ZombieState.FOLLOWING)
						{
							LostSight();
						}
					}
				}
				else
				{
					if (GetComponentInParent<Zombie>().GetZombieState() == ZombieState.FOLLOWING)
					{
						LostSight();
					}
				}
			}
		}
		else
		{
			if (GetComponentInParent<Zombie>().GetZombieState() == ZombieState.FOLLOWING)
			{
				LostSight();
			}
		}
	}

	private void PingPongViewAngle()
	{
		// Rotate FOV if not following
		if (GetComponentInParent<Zombie>().GetZombieState() != ZombieState.FOLLOWING)
		{
			if (_increasing)
			{
				if (_rotationStep < MaxViewRotation)
				{
					_rotationStep += ViewSpeed;
					_lookDir = DirFromAngle(ViewAngle, true);
				}
				else
				{
					_increasing = false;
				}
			}
			else
			{
				if (_rotationStep > -MaxViewRotation)
				{
					_rotationStep -= ViewSpeed;
					_lookDir = DirFromAngle(ViewAngle, true);
				}
				else
				{
					_increasing = true;
				}
			}
		}
	}

	private void LostSight()
	{
		GetComponentInParent<Zombie>().SetZombieState(ZombieState.ALARMED);
		_enemyTarget = null;
		GetComponentInParent<Zombie>().SetAgentDestination(_lastPointPlayerSeen);
		_lookDir = transform.forward;
		_rotationStep = 0;
	}

	void DrawFieldOfView()
	{
		if (_showFov)
		{
			int stepCount = Mathf.RoundToInt(ViewAngle * MeshResolution);
			float stepAngleSize = ViewAngle / stepCount;
			List<Vector3> viewPoints = new List<Vector3>();
			ViewCastInfo oldViewCast = new ViewCastInfo();
			for (int i = 0; i <= stepCount; i++)
			{
				float angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;
				ViewCastInfo newViewCast = ViewCast(angle);

				if (i > 0)
				{
					bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.Dst - newViewCast.Dst) > EdgeDstThreshold;
					if (oldViewCast.Hit != newViewCast.Hit || (oldViewCast.Hit && newViewCast.Hit && edgeDstThresholdExceeded))
					{
						EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
						if (edge.PointA != Vector3.zero)
						{
							viewPoints.Add(edge.PointA);
						}

						if (edge.PointB != Vector3.zero)
						{
							viewPoints.Add(edge.PointB);
						}
					}

				}
				viewPoints.Add(newViewCast.Point);
				oldViewCast = newViewCast;
			}

			int vertexCount = viewPoints.Count + 1;
			Vector3[] vertices = new Vector3[vertexCount];
			int[] triangles = new int[(vertexCount - 2) * 3];

			vertices[0] = Vector3.zero;
			for (int i = 0; i < vertexCount - 1; i++)
			{
				vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

				if (i < vertexCount - 2)
				{
					triangles[i * 3] = 0;
					triangles[i * 3 + 1] = i + 1;
					triangles[i * 3 + 2] = i + 2;
				}
			}

			_viewMesh.Clear();

			Renderer rend = ViewMeshFilter.GetComponent<Renderer>();

			if (rend != null)
			{
				switch (GetComponentInParent<Zombie>().GetZombieState())
				{
					case ZombieState.IDLE:
						rend.material.SetColor("_Color", Color.green);
						//fovMaterial.color = Color.green;
						break;
					case ZombieState.ALARMED:
						rend.material.SetColor("_Color", Color.yellow);
						//fovMaterial.color = Color.yellow;
						break;
					case ZombieState.FOLLOWING:
						rend.material.SetColor("_Color", Color.red);
						//fovMaterial.color = Color.red;
						break;
				}
			}

			_viewMesh.vertices = vertices;
			_viewMesh.triangles = triangles;
			_viewMesh.RecalculateNormals();
		}
		else
		{
			_viewMesh.Clear();
		}
	}


	EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
	{
		float minAngle = minViewCast.Angle;
		float maxAngle = maxViewCast.Angle;
		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for (int i = 0; i < EdgeResolveIterations; i++)
		{
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast(angle);

			bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.Dst - newViewCast.Dst) > EdgeDstThreshold;
			if (newViewCast.Hit == minViewCast.Hit && !edgeDstThresholdExceeded)
			{
				minAngle = angle;
				minPoint = newViewCast.Point;
			}
			else
			{
				maxAngle = angle;
				maxPoint = newViewCast.Point;
			}
		}

		return new EdgeInfo(minPoint, maxPoint);
	}


	ViewCastInfo ViewCast(float globalAngle)
	{
		Vector3 dir = DirFromAngle(globalAngle, true);
		RaycastHit hit;

		if (Physics.Raycast(transform.position, dir, out hit, ViewRadius, ObstacleMask))
		{
			return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
		}
		else
		{
			return new ViewCastInfo(false, transform.position + dir * ViewRadius, ViewRadius, globalAngle);
		}
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin((_rotationStep + angleInDegrees) * Mathf.Deg2Rad), 0, Mathf.Cos((_rotationStep + angleInDegrees) * Mathf.Deg2Rad));
	}

	public struct ViewCastInfo
	{
		public bool Hit;
		public Vector3 Point;
		public float Dst;
		public float Angle;

		public ViewCastInfo(bool hit, Vector3 point, float dst, float angle)
		{
			this.Hit = hit;
			this.Point = point;
			this.Dst = dst;
			this.Angle = angle;
		}
	}

	public struct EdgeInfo
	{
		public Vector3 PointA;
		public Vector3 PointB;

		public EdgeInfo(Vector3 pointA, Vector3 pointB)
		{
			this.PointA = pointA;
			this.PointB = pointB;
		}
	}

	public void SetSelected(bool selected)
	{
		_showFov = selected;
	}
}