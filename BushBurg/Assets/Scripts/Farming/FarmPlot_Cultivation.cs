using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*This script is for dealing with the specifics of 
 * an individual farm plot*/

public class FarmPlot_Cultivation : MonoBehaviour {

	//static float MAXTIMEMODIFIER = .8f;
	
	GameObject slot1Citizen, slot2Citizen;
	Vector3 slot1Position, slot2Position, producePosition;

	public GameObject selectedIndicator;
	public GameObject cropPrefab;

	public GameObject progressIndicatorPrefab;
	Vector3 progressIndicatorOrigin;
	LineRenderer progressIndicator;

	public GameController_Script gameController;
	public StorageBehavior storage;

	public Utilities.CropTypes cropType;

	public Utilities.Attributes primaryEff{get; private set;}
	public Utilities.Attributes secondaryEff{get; private set;}
	public Utilities.Attributes primaryQual{get; private set;}
	public Utilities.Attributes secondaryQual{get; private set;}
	public float fatigueRate{get; private set;}

	float timeModifier;
	float maxProductionTime;
	float timeToProduce;
	float timeToAutoCollect;
	GameObject currentProduce;

	void Awake()
	{
		gameController = GameObject.Find ("GameController").GetComponent<GameController_Script>();
		storage = GameObject.Find ("Storage").GetComponent<StorageBehavior>();

		slot1Position = new Vector3 (-.666f, 0, 0) + transform.position;
		slot2Position = new Vector3 (.666f, 0, 0) + transform.position;
		producePosition = new Vector3(.666f, 0,2.1f) + transform.position;

		progressIndicator = progressIndicatorPrefab.GetComponent<LineRenderer>();
		progressIndicatorOrigin = transform.position + new Vector3(-1.5f, .5f, -1.8f);
		progressIndicator.SetPosition (0, progressIndicatorOrigin);
		progressIndicator.SetPosition (1, progressIndicatorOrigin);

		Deselect ();

		//this should be coded out later!! temporary
		primaryEff = Utilities.Attributes.None;
		secondaryEff = Utilities.Attributes.None;
		primaryQual = Utilities.Attributes.None;
		secondaryQual = Utilities.Attributes.None;
		fatigueRate = 1;
		//////////////////

		//timeToProduce = 60/Utilities.TIMESCALE;

	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateMetrics();
		Produce ();
	}

	//If something is updated every frame regardless of input, put it here
	void UpdateMetrics()
	{
		GetTimeModifier ();

		if (timeToProduce > 0 && ((slot1Citizen != null) || (slot2Citizen != null)) && cropType != Utilities.CropTypes.None && currentProduce == null)
		{
			timeToProduce -= (1/(1-timeModifier))*Time.deltaTime;

			if (gameController.selectedTask == this.gameObject)
			{
				//print ((1/(1-timeModifier)) + " " + timeToProduce);
			}

			float progressPercentage = Mathf.Min(1 - timeToProduce/maxProductionTime, 1);
			
			progressIndicator.SetPosition (1, progressIndicatorOrigin + new Vector3(3,0,0)*progressPercentage);
		}

		if (currentProduce != null)
		{
			if (timeToAutoCollect < 0)
			{
				currentProduce.GetComponent<CropBehavior>().Collect ();
			}
			else
			{
				timeToAutoCollect -= Time.deltaTime*(1/(1-timeModifier));
			}
		}


	}

	void GetTimeModifier()
	{
		float efficiency = 0;

		if (slot1Citizen != null)
		{
			efficiency += slot1Citizen.GetComponent<CitizenBehavior>().GetEfficiency();
		}
		
		if (slot2Citizen != null)
		{
			efficiency += slot2Citizen.GetComponent<CitizenBehavior>().GetEfficiency ();
		}
		
		if (efficiency > 0)
		{
			timeModifier = Utilities.MAXTIMEMODIFIER*(efficiency/30);
		}
		else
		{
			timeModifier = 0;
		}
	}

	//Produce a new crop
	void Produce()
	{

		if (timeToProduce < 0)
		{
			float quality = 0;
			
			if (slot1Citizen != null)
			{
				quality += slot1Citizen.GetComponent<CitizenBehavior>().GetQuality();
			}
			
			if (slot2Citizen != null)
			{
				quality += slot2Citizen.GetComponent<CitizenBehavior>().GetQuality ();
			}
			
			quality /= 30;

			GameObject newDraggableCrop = Instantiate(cropPrefab, producePosition, Quaternion.identity) as GameObject;
			newDraggableCrop.GetComponent<CropBehavior>().CreateCrop(cropType, Utilities.ItemTypes.Crop, this.gameObject, quality, 0);

			currentProduce = newDraggableCrop;
			timeToProduce = maxProductionTime;
			timeToAutoCollect = maxProductionTime/3f;
		}
	}

	//Positions an accepted citizen into a slot
	//the check for a full field is a different function
	//assigning to this without checking first could lead to problems
	public void Assign(GameObject citizen_in)
	{
		if (slot1Citizen == null)
		{
			slot1Citizen = citizen_in;

			citizen_in.transform.position = slot1Position;
		}
		else if (slot2Citizen == null)
		{
			slot2Citizen = citizen_in;

			citizen_in.transform.position = slot2Position;
		}
		else
		{
			print ("ERROR IN FARMPLOT_CULTIVATION ASSIGN: ASSIGNING INTO FULL FIELD:  CHECK ISFULL BEFORE ASSIGNING");
		}
	}

	//clears the slot that the citizen is leaving
	public void Unassign(GameObject citizen_in)
	{
		if (slot1Citizen != null && citizen_in.GetInstanceID() == slot1Citizen.GetInstanceID())
		{
			slot1Citizen = null;
		}
		else if (slot2Citizen != null && citizen_in.GetInstanceID() == slot2Citizen.GetInstanceID())
		{
			slot2Citizen = null;
		}
		else
		{
			print ("ERROR IN FARMPLOT_CULTIVATION UNASSIGN: CITIZEN NOT IN FIELD");
		}
		
	}

	//find out if the field has both slots occupied
	public bool IsFull()
	{
		if ((slot1Citizen != null) && (slot2Citizen != null))
			return true;
		else
			return false;

	}
	
	public void Select()
	{
		selectedIndicator.SetActive (true);
	}
	
	public void Deselect()
	{
		selectedIndicator.SetActive (false);
	}

	public void SetCrop(Utilities.CropTypes crop_in)
	{
		cropType = crop_in;

		CropsAndBuffs.Crop newCrop = CropsAndBuffs.cropList[crop_in];

		primaryEff = newCrop.primaryEff;
		secondaryEff = newCrop.secondaryEff;
		primaryQual = newCrop.primaryQual;
		secondaryQual  = newCrop.secondaryQual;
		fatigueRate = newCrop.fatigueRate;
		maxProductionTime = newCrop.timeToProduce/Utilities.TIMESCALE;
		timeToProduce = maxProductionTime;

        //GameObject.Find("CurrentPlot_CropSlot").GetComponent<crop_indicator_Script>().SetTexture(newCrop);
		print (newCrop.name);

		if (slot1Citizen != null)
		{
			slot1Citizen.GetComponent<CitizenBehavior>().SetTaskAttributes (primaryEff, secondaryEff, primaryQual, secondaryQual, fatigueRate);
			slot1Citizen.GetComponent<CitizenBehavior>().Activate();
		}

		if (slot2Citizen != null)
		{
			slot2Citizen.GetComponent<CitizenBehavior>().SetTaskAttributes (primaryEff, secondaryEff, primaryQual, secondaryQual, fatigueRate);
			slot2Citizen.GetComponent<CitizenBehavior>().Activate();
		}

		Utilities.SetCropTexture(this.gameObject.transform.GetChild (1).gameObject, crop_in);
		//print (plantedCrop);
	}

}
