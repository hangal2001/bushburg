using UnityEngine;
using System.Collections;

public class CreatureManager_Script : MonoBehaviour 
{
	public GameObject caterpillarPrefab;
	public GameObject pestPrefab;

	GameObject[] caterpillars;

	// Use this for initialization
	void Start () 
	{
		caterpillars = new GameObject[10];

		for (int c=0; c < 10; c++)
			caterpillars[c] = CreateCaterpillar();
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateMetrics();
	}

	void UpdateMetrics()
	{
		/*
		for (int c=0; c < 10; c++)
		{
			if (caterpillars[c] == null)
				caterpillars[c] = CreateCaterpillar();
		}*/
	}

	GameObject CreateCaterpillar()
	{

		float xloc = Random.Range (-30, 30);
		float zloc;
		
		if (xloc > -2)
		{
			zloc = Random.Range (-16, 18);
		}
		else
		{
			zloc = Random.Range (-16, 4);
		}
		
		return Instantiate (caterpillarPrefab, new Vector3(xloc, -.2f, zloc), Quaternion.identity) as GameObject;

	}

	public void DestroyCaterpillar(GameObject caterpillar_in)
	{
		for (int c=0; c < 10; c++)
		{
			if (caterpillars[c] == caterpillar_in)
			{
				Destroy (caterpillar_in);
				caterpillars[c] = CreateCaterpillar ();
			}
		}
	}
}
