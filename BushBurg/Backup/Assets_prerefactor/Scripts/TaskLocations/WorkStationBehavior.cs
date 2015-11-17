using UnityEngine;
using System.Collections;

public class WorkStationBehavior : MonoBehaviour {

	public static float EATTIME = 7f;    //how long it takes to eat something

	public GameObject cropPrefab;

	public GameObject currentCitizen;
	public GameController_Script gameController;
	public Utilities.ItemTypes itemType{get; private set;}

	public Utilities.WorkStations stationType;
	public GameObject selectedIndicator;

	public GameObject progressIndicatorPrefab;
	Vector3 progressIndicatorOrigin;
	LineRenderer progressIndicator;
	
	public Utilities.CropTypes cropType;

	public Utilities.Attributes primaryEff{get; private set;}
	public Utilities.Attributes secondaryEff{get; private set;}
	public Utilities.Attributes primaryQual{get; private set;}
	public Utilities.Attributes secondaryQual{get; private set;}
	public float fatigueRate{get; private set;}

	float restoreQuality;
	float buffQuality;
	float maxProductionTime;
	public float timeToProduce { get; private set; }
    public float timeModifier { get; private set; }
    float recoveryRestore;
	float timeToAutoCollect;
	GameObject currentProduct;

	public bool isActive{get; private set;}

	//Vector3 itemSlotLoc;
	Vector3 citizenSlotLoc;
	Vector3 productLoc;

    CropsAndBuffs.Buff currentBuff;

	// Use this for initialization
	void Awake () 
	{
		gameController = GameObject.Find ("GameController").GetComponent<GameController_Script>();

		Deselect();

		//this is sloppy but works
		//itemSlotLoc = transform.position + new Vector3(-1, 0, 0);
		citizenSlotLoc = transform.position + new Vector3(1, 0, 0);

		if (stationType == Utilities.WorkStations.Cauldron)
		{
			productLoc = transform.position + new Vector3(3, 0, 0);
		}
		else
		{
			productLoc = transform.position + new Vector3(3, 0, 0);
		}

		//setting stats for a given workstation
		if (stationType == Utilities.WorkStations.Table)
		{
			primaryEff = Utilities.Attributes.None;
			secondaryEff = Utilities.Attributes.None;
			primaryQual = Utilities.Attributes.None;
			secondaryQual = Utilities.Attributes.None;
		}
		else if (stationType == Utilities.WorkStations.Cauldron)
		{
			primaryEff = Utilities.Attributes.Dexterity;
			secondaryEff = Utilities.Attributes.Endurance;
			primaryQual = Utilities.Attributes.Focus;
			secondaryQual  = Utilities.Attributes.Perception;
		}
		else if (stationType == Utilities.WorkStations.Depot)
		{
			primaryEff = Utilities.Attributes.Endurance;
			secondaryEff = Utilities.Attributes.Dexterity;
			primaryQual = Utilities.Attributes.Acumen;
			secondaryQual = Utilities.Attributes.Perception;
		}

		progressIndicator = progressIndicatorPrefab.GetComponent<LineRenderer>();
		progressIndicatorOrigin = transform.position + new Vector3(-2f, .5f, -1.2f);
		progressIndicator.SetPosition (0, progressIndicatorOrigin);
		progressIndicator.SetPosition (1, progressIndicatorOrigin);

		timeToProduce = 0;
		timeModifier = 0;
		timeToAutoCollect = 0;
		fatigueRate = 1;

		currentBuff.buffType = Utilities.BuffTypes.None;
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateMetrics();
	}

	void UpdateMetrics()
	{
		GetTimeModifier ();

		if (isActive && currentProduct == null)
		{
			timeToProduce -= Time.deltaTime*(1/(1-timeModifier));

            //if(gameController.selectedTask == this.gameObject)
               // print(timeToProduce / Utilities.TIMESCALE);

			float progressPercentage = Mathf.Min(1 - timeToProduce/maxProductionTime, 1);
			
			progressIndicator.SetPosition (1, progressIndicatorOrigin + new Vector3(4,0,0)*progressPercentage);

			//if (gameController.selectedTask == this.gameObject)
				//print (timeModifier + " " + (1/(1-timeModifier)) + " " + timeToProduce);

			if (timeToProduce <= 0)
			{
				CompleteTask ();
			}
		}

		if (isActive && currentProduct != null)
		{
			if (timeToAutoCollect < 0)
			{
				currentProduct.GetComponent<CropBehavior>().Collect ();
			}
			else
			{
				timeToAutoCollect -= Time.deltaTime*(1/(1-timeModifier));
			}
		}

	}
	
	public void Assign(GameObject citizen_in)
	{	
		currentCitizen = citizen_in;
		citizen_in.transform.position = citizenSlotLoc;

		if (CanActivate())
		{
			Activate();
		}
	}

	void GetTimeModifier()
	{
		float efficiency = 0;
		
		if (currentCitizen != null && stationType != Utilities.WorkStations.Table)
		{
			efficiency += currentCitizen.GetComponent<CitizenBehavior>().GetEfficiency();
		}
		
		if (efficiency > 0)
		{
			timeModifier = Utilities.MAXTIMEMODIFIER*(efficiency/15);
		}
		else
		{
			timeModifier = 0;
		}
	}

	public void Release()
	{
		if (!isActive)
			currentCitizen = null;
	}

	public void Activate()
	{
		isActive = true;
		currentCitizen.GetComponent<CitizenBehavior>().Activate();
	}

	public void Deactivate()
	{
		isActive = false;
		currentCitizen.GetComponent<CitizenBehavior>().Deactivate();
	}

	public void CompleteTask()
	{
		if (stationType == Utilities.WorkStations.Table)
		{
			currentCitizen.GetComponent<CitizenBehavior>().Feed(recoveryRestore, currentBuff, cropType);

			isActive = false;

		}
		else if (stationType == Utilities.WorkStations.Cauldron)
		{

            buffQuality = GetQuality();
            //print ("qualitymeal: " + restoreQuality + " " + buffQuality);

            GameObject newDraggableCrop = Instantiate(cropPrefab, productLoc, Quaternion.identity) as GameObject;
			newDraggableCrop.GetComponent<CropBehavior>().CreateCrop(cropType, Utilities.ItemTypes.Meal, this.gameObject, restoreQuality, buffQuality);
			
			currentProduct = newDraggableCrop;
			timeToProduce = maxProductionTime;
			timeToAutoCollect = maxProductionTime/3f;

		}
		else if (stationType == Utilities.WorkStations.Depot)
		{
            buffQuality = GetQuality();
			//print ("qualitytrade: " + restoreQuality + " " + buffQuality);

			GameObject newDraggableCrop = Instantiate(cropPrefab, productLoc, Quaternion.identity) as GameObject;
			newDraggableCrop.GetComponent<CropBehavior>().CreateCrop(cropType, itemType+2, this.gameObject, restoreQuality, buffQuality);
			
			currentProduct = newDraggableCrop;
			timeToProduce = maxProductionTime;
			timeToAutoCollect = maxProductionTime/3f;

		}

		cropType = Utilities.CropTypes.None;
		Utilities.SetCropTexture(this.gameObject.transform.GetChild (1).gameObject, cropType);
		
		progressIndicator.SetPosition (1, progressIndicatorOrigin);


	}

    public float GetQuality()
    {
        float quality = 0;

        if (currentCitizen != null)
        {
            quality = currentCitizen.GetComponent<CitizenBehavior>().GetQuality();
            quality /= 15;
        }

        return quality;
    }

    public float ConvertedTimeModifier()
    {
        return (1 / (1 - timeModifier));
    }

    //cropBehavior calls this when the item is collected
    //because the object is not destroyed in that specific case
    public void RemoveProductReference()
	{
		currentProduct = null;
	}

	public bool IsFull()
	{
		if (currentCitizen != null)
			return true;
		else
			return false;
	}

	public bool CanActivate()
	{
		return (CanFeed() || CanCook() || CanTrade ());
	}

	public bool CanDrop()
	{
		return (!isActive);
	}

	public bool CanFeed()
	{
		return (currentCitizen != null && stationType == Utilities.WorkStations.Table && cropType != Utilities.CropTypes.None);
	}

	public bool CanCook()
	{
		return (currentCitizen != null && stationType == Utilities.WorkStations.Cauldron && cropType != Utilities.CropTypes.None);
	}

	public bool CanTrade()
	{
		return (currentCitizen != null&& stationType == Utilities.WorkStations.Depot && cropType != Utilities.CropTypes.None);
	}

	public void Select()
	{
		selectedIndicator.SetActive (true);
	}
	
	public void Deselect()
	{
		selectedIndicator.SetActive (false);
	}

	public void SetItem(Utilities.CropTypes crop_in, Utilities.ItemTypes itemType_in, float restQuality_in, CropsAndBuffs.Buff buff_in)
	{
		itemType = itemType_in;
		cropType = crop_in;
		restoreQuality = restQuality_in;
		//buffQuality = buffQuality_in;
		CropsAndBuffs.Crop newCrop = CropsAndBuffs.cropList[cropType];

		if (stationType == Utilities.WorkStations.Cauldron)
		{
			//fatigueRate = newCrop.fatigueRate/Utilities.COOKTIMERATIO;
			maxProductionTime = (newCrop.timeToProduce*Utilities.COOKTIMERATIO)/(Utilities.TIMESCALE);
            //print(maxProductionTime);
			timeToProduce = maxProductionTime;
		}
		else if (stationType == Utilities.WorkStations.Table)
		{
			if (itemType == Utilities.ItemTypes.Meal)
			{
                currentBuff = buff_in;
			}
			else
			{
				currentBuff.buffType = Utilities.BuffTypes.None;
			}

			recoveryRestore = newCrop.baseRecovery + newCrop.baseRecovery*restoreQuality;
			maxProductionTime = EATTIME/Utilities.TIMESCALE;
			timeToProduce = maxProductionTime;


		}
		else if (stationType == Utilities.WorkStations.Depot)
		{

			//fatigueRate = newCrop.fatigueRate/Utilities.TRADETIMERATIO;
			maxProductionTime = newCrop.timeToProduce*Utilities.TRADETIMERATIO/(Utilities.TIMESCALE);
			timeToProduce = maxProductionTime;
		}
		
		if (currentCitizen != null)
		{
			currentCitizen.GetComponent<CitizenBehavior>().SetTaskAttributes (primaryEff, secondaryEff, primaryQual, secondaryQual, fatigueRate);
			//print (primaryEff);

			if (CanActivate ())
				Activate ();
		}
		
		Utilities.SetCropTexture(this.gameObject.transform.GetChild (1).gameObject, crop_in);

        if (gameController.selectedTask == this.gameObject)
            GameObject.Find("Current_Task_UI").GetComponent<CurrentTaskUI_Script>().SetTask();
    }

}
