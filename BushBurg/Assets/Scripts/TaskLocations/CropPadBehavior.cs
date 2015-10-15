using UnityEngine;
using System.Collections;

public class CropPadBehavior : MonoBehaviour {

	public Utilities.CropTypes cropType{get; private set;}

	public GameObject cropPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetCrop(Utilities.CropTypes crop_in)
	{
		cropType = crop_in;
	}


	public GameObject CreateDraggableCrop(Vector3 location_in)
	{
		GameObject newDraggableCrop = Instantiate(cropPrefab, location_in, Quaternion.identity) as GameObject;
		newDraggableCrop.GetComponent<CropBehavior>().CreateCrop(cropType, Utilities.ItemTypes.Seed, this.gameObject, 0, 0);

		return newDraggableCrop;
	}
}
