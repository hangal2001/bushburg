using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StorageBehavior : MonoBehaviour {

	static int NUMCROPS = 21;

	public GameObject cropPadPrefab;

	public List<GameObject> cropPads{get; private set;}

	public float offsetX, offsetZ;
	public float multiplierX, multiplierZ;
	
	public Dictionary<Utilities.CropTypes, List<float>> crops;

    GameObject[] stocks;
    int level;

	// Use this for initialization
	void Start () 
	{
		cropPads = new List<GameObject>();
		crops = new Dictionary<Utilities.CropTypes, List<float>>();
        level = 0;

		BuildEmptyStorage();
        LevelUp();

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
		for (int c=1; c < NUMCROPS; c++)
		{
			crops.Add ((Utilities.CropTypes)c, new List<float>());

            Vector3 nextLoc = new Vector3(offsetX + ((c - 1) / 11) * multiplierX, 0f, ((c - 1) % 11) * multiplierZ + offsetZ);
            GameObject nextPad = Instantiate(cropPadPrefab, transform.position + nextLoc, Quaternion.identity) as GameObject;
            nextPad.GetComponent<CropPadBehavior>().SetCrop((Utilities.CropTypes)c);
            cropPads.Add(nextPad);

        }


        //THIS CURRENTLY GETS AN ARRAY OF STOCK OBJECTS TO DISABLE THEM DEPENDING ON LEVEL
        GameObject stock = transform.GetChild(0).gameObject;
        stocks = new GameObject[NUMCROPS - 1];

        for (int c = 2; c < 22; c++)
        {
            stocks[c - 2] = stock.transform.GetChild(c).gameObject;

            if (c > 4)
            {
                stocks[c - 2].SetActive(false);
                cropPads[c - 2].SetActive(false);
            }
        }
    }

    public void LevelUp()
    {
        level++;

        switch (level)
        {
            case 1:
                break;
            case 2:
                stocks[3].SetActive(true);
                stocks[4].SetActive(true);
                stocks[5].SetActive(true);
                cropPads[3].SetActive(true);
                cropPads[4].SetActive(true);
                cropPads[5].SetActive(true);
                break;
            case 3:
                stocks[6].SetActive(true);
                stocks[7].SetActive(true);
                stocks[8].SetActive(true);
                cropPads[6].SetActive(true);
                cropPads[7].SetActive(true);
                cropPads[8].SetActive(true);
                break;
            case 4:
                stocks[9].SetActive(true);
                stocks[10].SetActive(true);
                stocks[11].SetActive(true);
                cropPads[9].SetActive(true);
                cropPads[10].SetActive(true);
                cropPads[11].SetActive(true);
                break;
            case 5:
                stocks[12].SetActive(true);
                stocks[13].SetActive(true);
                cropPads[12].SetActive(true);
                cropPads[13].SetActive(true);
                break;
            case 6:
                stocks[14].SetActive(true);
                stocks[15].SetActive(true);
                cropPads[14].SetActive(true);
                cropPads[15].SetActive(true);
                break;
            case 7:
                stocks[16].SetActive(true);
                stocks[17].SetActive(true);
                cropPads[16].SetActive(true);
                cropPads[17].SetActive(true);
                break;
            case 8:
                stocks[18].SetActive(true);
                stocks[19].SetActive(true);
                cropPads[18].SetActive(true);
                cropPads[19].SetActive(true);
                break;

        }
    }
}
