using UnityEngine;
using System.Collections;

public class WorkStationBehavior : MonoBehaviour {

    public static float EATTIME = 7f;    //how long it takes to eat something
    public static float AUTOCOLLECTSCALE = .33f; //proportion of autocollect to production time

    public GameObject cropPrefab;

    //public GameObject currentCitizen;
    public GameObject citizen1 { get; private set; }
    public GameObject citizen2 { get; private set; }

    GameController_Script gameController;
    //StorageBehavior storage;

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
	Vector3 slot1Loc, slot2Loc;
	Vector3 productLoc;

    CropsAndBuffs.Buff currentBuff;

	// Use this for initialization
	void Awake () 
	{
		gameController = GameObject.Find ("GameController").GetComponent<GameController_Script>();
        //storage = GameObject.Find("Storage").GetComponent<StorageBehavior>();

        Deselect();

        Initialize();
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
			timeToProduce -= Time.deltaTime*ConvertedTimeModifier();

            //if(gameController.selectedTask == this.gameObject)
               // print(timeToProduce / Utilities.TIMESCALE);

			float progressPercentage = Mathf.Min(1 - timeToProduce/maxProductionTime, 1);
			
            if (stationType == Utilities.WorkStations.FarmPlot)
            {
                progressIndicator.SetPosition(1, progressIndicatorOrigin + new Vector3(3, 0, 0) * progressPercentage);
            }
            else
            {
                progressIndicator.SetPosition(1, progressIndicatorOrigin + new Vector3(4, 0, 0) * progressPercentage);
            }
			

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
				timeToAutoCollect -= Time.deltaTime*ConvertedTimeModifier();
			}
		}

	}
	
	public void Assign(GameObject citizen_in)
	{	
        if (stationType == Utilities.WorkStations.FarmPlot)
        {
            if (citizen1 == null)
            {
                citizen1 = citizen_in;
                citizen1.transform.position = slot1Loc;
            }
            else if (citizen2 == null)
            {
                citizen2 = citizen_in;
                citizen2.transform.position = slot2Loc;
            }
            else
            {
                //FIXFIX retur to sender
            }
        }
        else
        {
            if (citizen1 == null)
            {
                citizen1 = citizen_in;
                citizen_in.transform.position = slot1Loc;
            }
            else
            {
                //FIXFIX return to sender
            }
            
        }
        //If possible, activate workstation
		Activate();

	}

    public void Unassign(GameObject citizen_in)
    {
        if (citizen1 != null && citizen_in == citizen1)
        {
            citizen1 = null;
        }
        else if (stationType == Utilities.WorkStations.FarmPlot)
        {
            if (citizen2 != null & citizen_in == citizen2)
            {
                citizen2 = null;
            }
            else
            {
                print("ERROR IN WORKSTATION UNASSIGN: CITIZEN NOT ASSIGNED TO THIS FIELD");
            }
        }
        else
        {
            print("ERROR IN WORKSTATION UNASSIGN: CITIZEN NOT ASSIGNED TO THIS STATION");
        }

        if (citizen1 == null && citizen2 == null)
        {
            Deactivate();
        }

    }

	void GetTimeModifier()
	{
		float efficiency = 0;
		
		if (citizen1 != null)
		{
			efficiency += citizen1.GetComponent<CitizenBehavior>().GetEfficiency();
		}
		
        if (citizen2 != null)
        {
            efficiency += citizen2.GetComponent<CitizenBehavior>().GetEfficiency();
        }

        if (stationType == Utilities.WorkStations.FarmPlot)
        {
            efficiency /= 2;
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

    
    public float ConvertedTimeModifier()
    {
        return (1 / (1 - timeModifier));
    }


    /*
	public void Release()
	{
		if (!isActive)
			currentCitizen = null;
	}*/


    public void Activate()
	{
        if (citizen1 != null || citizen2 != null)
        {
            if (cropType != Utilities.CropTypes.None)
            {
                isActive = true;

                if (citizen1 != null)
                {
                    citizen1.GetComponent<CitizenBehavior>().Activate();
                }
                
                if (citizen2 != null)
                {
                    citizen2.GetComponent<CitizenBehavior>().Activate();
                }

                //currentCitizen.GetComponent<CitizenBehavior>().Activate();
            }
        }
		
	}

	public void Deactivate()
	{
		isActive = false;

        if (citizen1 != null)
        {
            citizen1.GetComponent<CitizenBehavior>().Deactivate();
        }

        if (citizen2 != null)
        {
            citizen2.GetComponent<CitizenBehavior>().Deactivate();
        }

        //currentCitizen.GetComponent<CitizenBehavior>().Deactivate();
	}

	public void CompleteTask()
	{
        if (stationType == Utilities.WorkStations.Table)
        {
            citizen1.GetComponent<CitizenBehavior>().Feed(recoveryRestore, currentBuff, cropType);

            Deactivate();

        }
        else
        {
            GameObject newDraggableCrop = Instantiate(cropPrefab, productLoc, Quaternion.identity) as GameObject;

            //initializing to seed to avoid compiler errors - this is checked for later
            Utilities.ItemTypes nextItem = Utilities.ItemTypes.Seed;

            if (stationType == Utilities.WorkStations.Cauldron)
            {
                buffQuality = GetQuality();

                nextItem = Utilities.ItemTypes.Meal;
            }
            else if (stationType == Utilities.WorkStations.Depot)
            {
                buffQuality = GetQuality();

                if (itemType == Utilities.ItemTypes.Crop)
                {
                    nextItem = Utilities.ItemTypes.TradeCrop;
                }
                else if (itemType == Utilities.ItemTypes.Meal)
                {
                    nextItem = Utilities.ItemTypes.TradeMeal;
                }
            }
            else if (stationType == Utilities.WorkStations.FarmPlot)
            {
                restoreQuality = GetQuality();
                buffQuality = 0f;

                nextItem = Utilities.ItemTypes.Crop;
            }
            else
            {
                print("ERROR IN WORKSTATIONBEHAVIOR COMPLETETASK: STATION TYPE NOT HANDLED");
            }

            if (nextItem == Utilities.ItemTypes.Seed)
            {
                print("ERROR: CREATED SEED IN WORKSTATION - SHOULD NOT HAPPEN");
            }

            newDraggableCrop.GetComponent<CropBehavior>().CreateCrop(cropType, nextItem, this.gameObject, restoreQuality, buffQuality);
            currentProduct = newDraggableCrop;
            timeToProduce = maxProductionTime;
            timeToAutoCollect = maxProductionTime * AUTOCOLLECTSCALE;         

        }

        if (stationType != Utilities.WorkStations.FarmPlot)
        {
            cropType = Utilities.CropTypes.None;
            Utilities.SetCropTexture(this.gameObject.transform.GetChild(1).gameObject, cropType);
        }

        progressIndicator.SetPosition(1, progressIndicatorOrigin);

    }

    public float GetQuality()
    {
        float quality = 0;

        if (citizen1 != null)
        {
            quality = citizen1.GetComponent<CitizenBehavior>().GetQuality();
            quality /= 15;
        }

        if (citizen2 != null)
        {
            quality += citizen2.GetComponent<CitizenBehavior>().GetQuality()/15;
         
        }

        if (stationType == Utilities.WorkStations.FarmPlot)
        {
            quality /= 2;
        }

        return quality;
    }

    
    /*xRemoveProductReference
      cropBehavior calls this when the item is collected
      because the object is not destroyed in that specific case*/
    public void RemoveProductReference()
	{
		currentProduct = null;
	}

    /* xIsFull

       Asked by citizens to see if they can be slotted into this workstation*/
	public bool IsFull()
	{
        if (citizen1 != null && citizen2 != null)
            return true;
        else
            return false;
	}

    /* xCanDrop
       
        Asked by draggable crops to see if they can be slotted into this workstation*/
	public bool CanDrop()
	{
        if (stationType != Utilities.WorkStations.FarmPlot)
            return (!isActive);
        else
            return true;

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
			maxProductionTime = (newCrop.timeToProduce*Utilities.COOKTIMERATIO)/(Utilities.TIMESCALE);
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

		}
		else if (stationType == Utilities.WorkStations.Depot)
		{
			maxProductionTime = newCrop.timeToProduce*Utilities.TRADETIMERATIO/(Utilities.TIMESCALE);
		}
        else if (stationType == Utilities.WorkStations.FarmPlot)
        {
            primaryEff = newCrop.primaryEff;
            secondaryEff = newCrop.secondaryEff;
            primaryQual = newCrop.primaryQual;
            secondaryQual = newCrop.secondaryQual;
            fatigueRate = newCrop.fatigueRate;
            maxProductionTime = newCrop.timeToProduce / Utilities.TIMESCALE;
        }

        timeToProduce = maxProductionTime;

		if (citizen1 != null)
		{
			citizen1.GetComponent<CitizenBehavior>().SetTaskAttributes (primaryEff, secondaryEff, primaryQual, secondaryQual, fatigueRate);

			Activate ();
		}

        if (citizen2 != null)
        {
            citizen2.GetComponent<CitizenBehavior>().SetTaskAttributes(primaryEff, secondaryEff, primaryQual, secondaryQual, fatigueRate);

            Activate();
        }
		
		Utilities.SetCropTexture(this.gameObject.transform.GetChild (1).gameObject, crop_in);

        if (gameController.selectedTask == this.gameObject)
            GameObject.Find("Current_Task_UI").GetComponent<CurrentTaskUI_Script>().SetTask();
    }

    /* xInitialize

        Sets up following values:
        location of slot(s)
        location to spawn product when a cycle ends
        locations for product completion line renderer
        primary and secondary attributes
        time modifiers
        fatigue rate
        buff type
    */
    void Initialize()
    {
        progressIndicator = progressIndicatorPrefab.GetComponent<LineRenderer>();

        /* setting up subobject locations and initial prim/secondary stats
           citizens will snap to these locations when assigned,
           and products will spawn in productLoc after completion of a cycle
           these locations are dependent on the type of the workstation and can
           vary considerably*/
        if (stationType == Utilities.WorkStations.FarmPlot)
        {
            slot1Loc = new Vector3(-.666f, 0, 0) + transform.position;
            slot2Loc = new Vector3(.666f, 0, 0) + transform.position;
            productLoc = new Vector3(.666f, 0, 2.1f) + transform.position;

            primaryEff = Utilities.Attributes.None;
            secondaryEff = Utilities.Attributes.None;
            primaryQual = Utilities.Attributes.None;
            secondaryQual = Utilities.Attributes.None;

            progressIndicatorOrigin = transform.position + new Vector3(-1.5f, .5f, -1.8f);

        }
        else //station is not farm plot
        {
            slot1Loc = transform.position + new Vector3(1, 0, 0);

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
                secondaryQual = Utilities.Attributes.Perception;
            }
            else if (stationType == Utilities.WorkStations.Depot)
            {
                primaryEff = Utilities.Attributes.Endurance;
                secondaryEff = Utilities.Attributes.Dexterity;
                primaryQual = Utilities.Attributes.Acumen;
                secondaryQual = Utilities.Attributes.Perception;
            }

            progressIndicatorOrigin = transform.position + new Vector3(-2f, .5f, -1.2f);
        }

        //initializing line renderer to appear empty
        progressIndicator.SetPosition(0, progressIndicatorOrigin);
        progressIndicator.SetPosition(1, progressIndicatorOrigin);

        timeToProduce = 0;
        timeModifier = 0;
        timeToAutoCollect = 0;
        fatigueRate = 1;

        currentBuff.buffType = Utilities.BuffTypes.None;
    }

}
