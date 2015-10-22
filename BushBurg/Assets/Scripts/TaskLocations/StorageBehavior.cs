using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StorageBehavior : MonoBehaviour {

	static int NUMCROPS = 21;

	public GameObject cropPadPrefab;
    public GameObject stockItemsPrefab;

	public List<GameObject> cropPads{get; private set;}
    public List<GameObject> stockItems { get; private set; }

	public float offsetX, offsetZ;
	public float multiplierX, multiplierZ;
	
	public Dictionary<Utilities.CropTypes, List<float>> crops;

	// Use this for initialization
	void Start () 
	{
		cropPads = new List<GameObject>();
        stockItems = new List<GameObject>();
		crops = new Dictionary<Utilities.CropTypes, List<float>>();

		BuildEmptyStorage();

		for (int c=1; c < NUMCROPS; c++)
		{
			Vector3 nextLoc = new Vector3(offsetX +((c-1)/11)*multiplierX, 0f, ((c-1)%11)*multiplierZ + offsetZ); 
			GameObject nextPad = Instantiate(cropPadPrefab, transform.position+nextLoc, Quaternion.identity) as GameObject;
			nextPad.GetComponent<CropPadBehavior>().SetCrop((Utilities.CropTypes)c);
			cropPads.Add (nextPad);
            GameObject item = Instantiate(stockItemsPrefab, transform.position + nextLoc, Quaternion.Euler(90, 0, 0)) as GameObject;
            item.GetComponent<Stock_item_Script>().SetCrop((Utilities.CropTypes)c);
            stockItems.Add(item);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public float GetLowestQuality(List<float> list_in)
	{
		float lowest = 100f;

		foreach (float next in list_in)
		{
			if (next < lowest)
				lowest = next;
		}

		list_in.Remove (lowest);
		//print("New count" + list_in.Count + " " + lowest);
		return lowest;
	}

	public float GetHighestQuality(List<float> list_in)
	{
		float highest = 0;
		
		foreach (float next in list_in)
		{
			if (next > highest)
				highest = next;
		}
		
		list_in.Remove (highest);

		//print("new count: " + list_in.Count + " " + "highest qual: " + highest);
		return highest;
	}

	public void AddCrop(Utilities.CropTypes crop_in, float quality_in)
	{
		crops[crop_in].Add(quality_in);


		//print (crops[crop_in].Count);
	}

	public float RemoveCropLowest(Utilities.CropTypes crop_in)
	{
		return GetLowestQuality (crops[crop_in]);
	}

	public float RemoveCropHighest(Utilities.CropTypes crop_in)
	{
		return GetHighestQuality (crops[crop_in]);
	}

	public bool hasCrop(Utilities.CropTypes crop_in)
	{
		if (crops[crop_in].Count != 0)
			return true;
		else
			return false;
	}

	void BuildEmptyStorage()
	{
		for (int c=0; c < NUMCROPS; c++)
		{
			crops.Add ((Utilities.CropTypes)c, new List<float>());
		}
	}
}
