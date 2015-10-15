using UnityEngine;
using System.Collections;

public class CropBehavior : MonoBehaviour {

	public Utilities.CropTypes cropType{get; private set;}
	public Utilities.ItemTypes itemType{get; private set;}
		
	public GameObject currentProspect{get; private set;}
	
	public Vector3 currentSlot{get; private set;}	//location of current anchor (only relevant to meal tray items)
	public GameObject creator{get; private set;}	//reference to the object that created or stores this object

	public GameObject indicatorPrefab;

	GameController_Script gameController;

	public StorageBehavior storage;
	public MealTrayBehavior mealTray;

	Vector3 moneyLoc;

	GameObject indicator;

	public float restoreQuality{get; private set;}
	public float buffQuality{get; private set;}



	// Use this for initialization
	void Awake () 
	{
		storage = GameObject.Find ("Storage").GetComponent<StorageBehavior>();
		gameController = GameObject.Find ("GameController").GetComponent<GameController_Script>();
		mealTray = GameObject.Find ("MealTray").GetComponent<MealTrayBehavior>();
		moneyLoc = new Vector3(-20, 0, -2.5f);


	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void CreateCrop(Utilities.CropTypes crop_in, Utilities.ItemTypes type_in, GameObject creator_in, float restQuality_in, float buffQuality_in)
	{
		cropType = crop_in;
		itemType = type_in;
		creator = creator_in;

		if (itemType == Utilities.ItemTypes.Crop)
		{
			restoreQuality = restQuality_in;
			buffQuality =0;
		}

		if (itemType == Utilities.ItemTypes.Meal)
		{
			restoreQuality = restQuality_in;
			buffQuality = buffQuality_in;



			/*
			if (currentBuff.buffType == Utilities.BuffTypes.Recovery)
			{

			}
			else if (currentBuff.buffType == Utilities.BuffTypes.Drain)
			{

			}
			else if (currentBuff.buffType == Utilities.BuffTypes.AttributeScalar)
			{

			}
			else if (currentBuff.buffType == Utilities.BuffTypes.AttributeLockPositive)
			{

			}*/
		}

		Utilities.SetCropTexture(this.gameObject, crop_in);

		if (itemType != Utilities.ItemTypes.Seed)
		{
			CreateIndicator ();
		}
	}

	public void Drop()
	{
		if (itemType == Utilities.ItemTypes.Meal)
		{
			if (currentProspect != null && currentProspect.tag == "WorkStation" && currentProspect.GetComponent<WorkStationBehavior>().CanDrop ())
			{
				Utilities.WorkStations type = currentProspect.GetComponent<WorkStationBehavior>().stationType;

				if (type == Utilities.WorkStations.Table || type == Utilities.WorkStations.Depot)
				{
					currentProspect.GetComponent<WorkStationBehavior>().SetItem (cropType, Utilities.ItemTypes.Meal, restoreQuality, buffQuality);
					mealTray.Unassign (this.gameObject);
					Destroy (this.gameObject);
				}


			}

			//did not find a suitable location, return to last known meal tray position
			transform.position = currentSlot;
		}
		else
		{
			if (currentProspect != null)
			{
				if (currentProspect.tag == "FarmPlot")
				{
					currentProspect.GetComponent<FarmPlot_Cultivation>().SetCrop (cropType);
				}

				if (currentProspect.tag == "WorkStation" && currentProspect.GetComponent<WorkStationBehavior>().CanDrop () && storage.crops[cropType].Count > 0)
				{


					if (currentProspect.GetComponent<WorkStationBehavior>().stationType == Utilities.WorkStations.Depot)
					{
						restoreQuality = storage.RemoveCropLowest(cropType);
					}
					else
					{
						restoreQuality = storage.RemoveCropHighest(cropType);
					}

					if (currentProspect.GetComponent<WorkStationBehavior>().stationType == Utilities.WorkStations.Table)
					{
						currentProspect.GetComponent<WorkStationBehavior>().SetItem (cropType, Utilities.ItemTypes.Crop, restoreQuality, 0);
					}
					else
					{
						currentProspect.GetComponent<WorkStationBehavior>().SetItem (cropType, Utilities.ItemTypes.Crop, restoreQuality, buffQuality);
					}
				}
			}

			Destroy (this.gameObject);
		}
	}

	//when being wiggled around by mouse, a crop will collide with things
	//we have to use the OnTriggerEnter because it won't let us manually
	//check collisions later
	void OnTriggerEnter(Collider collision_in)
	{
		if (itemType == Utilities.ItemTypes.Meal)
		{
			if (collision_in.tag == "WorkStation")
			{
				Utilities.WorkStations type =  collision_in.GetComponent<WorkStationBehavior>().stationType;

				if (type == Utilities.WorkStations.Depot || type == Utilities.WorkStations.Table)
				{
					currentProspect = collision_in.gameObject;
				}
			}
		}
		else
		{
			if (collision_in.tag == "FarmPlot" || collision_in.tag == "WorkStation")
			{
				currentProspect = collision_in.gameObject;
			}
		}
	}

	//this is checked to prevent manual location of citizens
	//probably only a milestone 1 thing
	void OnTriggerExit(Collider collision_in)
	{

		if (collision_in.tag == "FarmPlot" || collision_in.tag == "WorkStation")
		{
			currentProspect = null;
		}

	}

	public void Collect()
	{
		if (itemType == Utilities.ItemTypes.Crop)
		{
			storage.AddCrop(cropType, restoreQuality);

			GameObject cropPad = storage.cropPads.Find (x => x.GetComponent<CropPadBehavior>().cropType == this.cropType);
			indicator.GetComponent<CollectionBehavior>().FlyToTarget (cropPad.transform.position);

			Destroy (this.gameObject);
		}
		else if (itemType == Utilities.ItemTypes.Meal)
		{
			mealTray.Assign (this.gameObject);

			creator.GetComponent<WorkStationBehavior>().Deactivate();

			//meals are not destroyed when collected from workstation so we must remove the reference
			creator.GetComponent<WorkStationBehavior>().RemoveProductReference();

			creator = mealTray.gameObject;

			indicator.GetComponent<CollectionBehavior>().FlyToTarget (transform.position);
			currentSlot = transform.position;


		}
		else if (itemType == Utilities.ItemTypes.TradeCrop)
		{	
			int value = CropsAndBuffs.cropList[cropType].baseValue;

			indicator.GetComponent<CollectionBehavior>().FlyToTarget (moneyLoc);
			gameController.AddMoney (value);

			creator.GetComponent<WorkStationBehavior>().Deactivate();

			Destroy (this.gameObject);
		}
		else if (itemType == Utilities.ItemTypes.TradeMeal)
		{
			int value = CropsAndBuffs.cropList[cropType].baseValue*3;
			
			indicator.GetComponent<CollectionBehavior>().FlyToTarget (moneyLoc);
			gameController.AddMoney (value);
			
			creator.GetComponent<WorkStationBehavior>().Deactivate();
			
			Destroy (this.gameObject);
		}
		else
		{
			print ("ERROR IN CROPBEHAVIOR COLLECT: ITEM TYPE NOT HANDLED");
		}
	}

	public void CreateIndicator()
	{
		indicator = GameObject.Instantiate (indicatorPrefab, transform.position + new Vector3(0, 5, 1f), indicatorPrefab.transform.rotation) as GameObject;
	}


}
