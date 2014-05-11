using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LODController : MonoBehaviour
{
	public Mesh HighResolutionMesh;
	public Mesh MediumResolutionMesh;
	public Mesh LowResolutionMesh;

	public Material HighResolutionMaterial;
	public Material MediumResolutionMaterial;
	public Material LowResolutionMaterial;

	public enum Resolutions { High, Medium, Low };
	public Resolutions CurrentResolution = Resolutions.Low;

	const int MaxMediumResolutionDistance = 100;

	[Range(0, MaxMediumResolutionDistance)]
	public int HighResolutionDistance = 10;
	[Range(0, MaxMediumResolutionDistance)]
	public int MediumResolutionDistance = 20;

	static private int DistanceOffset = 10;

	private MeshFilter thisMeshFilter;
	
	public float RefreshRate = 5.0f;

	LODController()
	{
		#if UNITY_EDITOR
		EditorApplication.update += EditorUpdate;
		#endif
	}

	// Use this for initialization
	void Start () 
	{
		thisMeshFilter = gameObject.GetComponent<MeshFilter>();
		// set to low res to start off with;
		thisMeshFilter.mesh = LowResolutionMesh;
		gameObject.renderer.material = LowResolutionMaterial;
		CurrentResolution = Resolutions.Low;
	}
	
	// Update is called once per frame
	void Update () 
	{
		CheckLODDistance();
	}

	public void CheckLODDistance()
	{
		float currentDistance = (gameObject.transform.position - Globals.Player.gameObject.transform.position).magnitude;

		if (currentDistance < HighResolutionDistance && CurrentResolution != Resolutions.High) 
		{
			thisMeshFilter.mesh = HighResolutionMesh;
			gameObject.renderer.material = HighResolutionMaterial;
			CurrentResolution = Resolutions.High;
		} 
		else if (currentDistance >= HighResolutionDistance && currentDistance < MediumResolutionDistance && CurrentResolution != Resolutions.Medium)
		{
			thisMeshFilter.mesh = MediumResolutionMesh;
			gameObject.renderer.material = MediumResolutionMaterial;
			CurrentResolution = Resolutions.Medium;
		}
		else if (currentDistance >= MediumResolutionDistance && CurrentResolution != Resolutions.Low)
		{
			thisMeshFilter.mesh = LowResolutionMesh;
			gameObject.renderer.material = LowResolutionMaterial;
			CurrentResolution = Resolutions.Low;
		}
	}


	private int prevHigh, prevLow, prevOffset;
	
	void EditorUpdate()
	{
		if (HighResolutionDistance != prevHigh || DistanceOffset != prevOffset)
		{
			if (HighResolutionDistance + DistanceOffset > MaxMediumResolutionDistance)
				HighResolutionDistance = MaxMediumResolutionDistance - DistanceOffset;

			if (HighResolutionDistance + DistanceOffset > MediumResolutionDistance)
			{
				MediumResolutionDistance = HighResolutionDistance + DistanceOffset;
				prevLow = MediumResolutionDistance;
			}
			prevHigh = HighResolutionDistance;
			prevOffset = DistanceOffset;
		}
		else if (MediumResolutionDistance != prevLow)
		{
			if (MediumResolutionDistance < DistanceOffset)
				MediumResolutionDistance = DistanceOffset;

			if (MediumResolutionDistance - DistanceOffset < HighResolutionDistance)
			{
				HighResolutionDistance = MediumResolutionDistance - DistanceOffset;
				prevHigh = HighResolutionDistance;
			}
			prevLow = MediumResolutionDistance;
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(gameObject.transform.position, (float)HighResolutionDistance);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(gameObject.transform.position, (float)MediumResolutionDistance);
	}
}
