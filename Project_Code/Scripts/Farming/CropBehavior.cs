using UnityEngine;
using System.Collections;

public class CropBehavior : MonoBehaviour {

    GameController_Script gameController;

    GameObject indicator;
    public GameObject indicatorPrefab;

    public StorageBehavior storage;
    public MealTrayBehavior mealTray;

    public Utilities.CropTypes cropType{get; private set;}
	public Utilities.ItemTypes itemType{get; private set;}
		
	public GameObject currentProspect{get; private set;}
    public GameObject creator { get; private set; } //reference to the object that created or stores this object

    public Vector3 currentSlot{get; private set;}   //location of current anchor (only relevant to meal tray items)
    Vector3 moneyLoc;

	public float restoreQuality{get; private set;}
	public float buffQuality{get; private set;}

    CropsAndBuffs.Buff currentBuff;

	// Use this for initialization
	void Awake () 
	{
		storage = GameObject.Find ("Storage").GetComponent<StorageBehavior>();
		gameController = GameObject.Find ("GameController").GetComponent<GameController_Script>();
		mealTray = GameObject.Find ("MealTray").GetComponent<MealTrayBehavior>();
		moneyLoc = new Vector3(-3.6f, 0, 20);

        
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

        restoreQuality = restQuality_in;
        buffQuality = buffQuality_in;

		if (itemType == Utilities.ItemTypes.Meal)
		{
            currentBuff = CropsAndBuffs.buffList[cropType];
            currentBuff.maxDuration += currentBuff.maxDuration * restoreQuality;
            currentBuff.value += currentBuff.value * buffQuality_in;
            currentBuff.duration = currentBuff.maxDuration;
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
					currentProspect.GetComponent<WorkStationBehavior>().SetItem (cropType, Utilities.ItemTypes.Meal, restoreQuality, currentBuff);
					mealTray.Unassign (this.gameObject);
					Destroy (this.gameObject);
				}


			}

			//did not find a suitable location, return to last known meal tray position
			if (currentProspect == null || currentProspect.tag != "WorkStation")
			{
				Utilities.FloatText (transform.position + new Vector3(0,1,0), "no assign!", Utilities.TextTypes.Neutral); 
			}
			else
			{
				WorkStationBehavior workstationScript = currentProspect.GetComponent<WorkStationBehavior>();
				
				if (workstationScript.stationType == Utilities.WorkStations.Cauldron && GameObject.Find("MealTray").GetComponent<MealTrayBehavior>().IsFull())
				{
					Utilities.FloatText (transform.position + new Vector3(0,1,0), "meal tray full!", Utilities.TextTypes.Neutral); 
				}
				else
				{
					Utilities.FloatText (transform.position + new Vector3(0,1,0), "station busy!", Utilities.TextTypes.Neutral); 
				}
			}
		
			transform.position = currentSlot;
		}
		else //item type is not a meal
		{
			if (currentProspect != null)
			{
                WorkStationBehavior stationScript = currentProspect.GetComponent<WorkStationBehavior>();

				if (stationScript.CanDrop ())
				{                  

                    if (stationScript.stationType == Utilities.WorkStations.FarmPlot)
                    {
                        stationScript.SetItem(cropType, Utilities.ItemTypes.Crop, 0f, currentBuff);
                    }
                    else
                    {
                        if (storage.crops[cropType].Count > 0)
                        {
                            if (stationScript.stationType == Utilities.WorkStations.Depot)
                            {
                                restoreQuality = storage.RemoveCropLowest(cropType);
                            }
                            else
                            {
                                restoreQuality = storage.RemoveCropHighest(cropType);
                            }

                            stationScript.SetItem(cropType, Utilities.ItemTypes.Crop, restoreQuality, currentBuff);
                        }
						else//there are no plants of that type to cook/trade/eat
						{
							Utilities.FloatText (transform.position + new Vector3(0,1,0), "no inventory!", Utilities.TextTypes.Neutral);
						}
                    }
					
				}
				else//station can not handle a new request
				{
					Utilities.FloatText (transform.position + new Vector3(0,1,0), "station busy!", Utilities.TextTypes.Neutral); 
				}
			}
			else //current prospect is null
			{
				Utilities.FloatText (transform.position + new Vector3(0,1,0), "no assign!", Utilities.TextTypes.Neutral);
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
			if (collision_in.tag == "WorkStation")
			{
				currentProspect = collision_in.gameObject;
			}
		}
	}

	//this is checked to prevent manual location of citizens
	//probably only a milestone 1 thing
	void OnTriggerExit(Collider collision_in)
	{

		if (collision_in.tag == "WorkStation")
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

			string floating = "qual:" + (Mathf.Round (restoreQuality*100)).ToString () + "%";
			Utilities.FloatText (transform.position + new Vector3(1,1,0), floating, Utilities.TextTypes.Neutral); 

			Destroy (this.gameObject);
		}
		else if (itemType == Utilities.ItemTypes.Meal)
		{
			string floating = "qual:" + (Mathf.Round (restoreQuality*100)).ToString () + "%";
			Utilities.FloatText (transform.position + new Vector3(1,1,0), floating, Utilities.TextTypes.Neutral); 

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
			int value = (int)(CropsAndBuffs.cropList[cropType].baseValue * (1 + buffQuality));
            //print(CropsAndBuffs.cropList[cropType].baseValue * (1 + buffQuality) + " " +  buffQuality);

			indicator.GetComponent<CollectionBehavior>().FlyToTarget (moneyLoc);
			gameController.AddMoney (value);

			creator.GetComponent<WorkStationBehavior>().Deactivate();

			string floating = "val: " + value.ToString ();
			Utilities.FloatText (transform.position + new Vector3(1,1,0), floating, Utilities.TextTypes.Neutral); 

			Destroy (this.gameObject);
		}
		else if (itemType == Utilities.ItemTypes.TradeMeal)
		{
			int value = (int)(CropsAndBuffs.cropList[cropType].baseValue * Utilities.TRADEBONUSVALUESCALE * (1 + buffQuality));
			
			indicator.GetComponent<CollectionBehavior>().FlyToTarget (moneyLoc);
			gameController.AddMoney (value);
			
			creator.GetComponent<WorkStationBehavior>().Deactivate();

			string floating = "val: " + value.ToString ();
			Utilities.FloatText (transform.position + new Vector3(1,1,0), floating, Utilities.TextTypes.Neutral); 

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
