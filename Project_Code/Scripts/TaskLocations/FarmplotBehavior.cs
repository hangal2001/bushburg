﻿using UnityEngine;
using System.Collections;

public class FarmplotBehavior : WorkStationBehavior
{

    /* INHERITED VARIABLES */

 //  /****set from editor****/
 //  public GameObject cropPrefab;
 //  public GameObject progressIndicatorPrefab;
 //  public GameObject selectedIndicator;
 //  /*---------------------*/
 //
 //  /****   locations  ****/
 //  protected Vector3 progressIndicatorOrigin;
 //  protected Vector3 progressIndicatorLength;
 //  protected Vector3 slot1Loc, slot2Loc;
 //  protected Vector3 productLoc;
 //  /*---------------------*/
 //
 //  /****    scripts   ****/
 //  protected GameController_Script gameController;
 //  protected CitizenBehavior citizen1Script;
 //  protected CitizenBehavior citizen2Script;
 //  protected CropBehavior cropScript;
 //  /*---------------------*/
 //
 //  /****     enums    ****/
 //  public Utilities.ItemTypes itemType { get; protected set; }
 //  public Utilities.WorkStations stationType;
 //  public Utilities.CropTypes cropType;
 //  /*---------------------*/
 //
 //  /****citizen related****/
 //  public GameObject citizen1 { get; protected set; }
 //  public GameObject citizen2 { get; protected set; }
 //
 //  public Utilities.Attributes primaryEff { get; protected set; }
 //  public Utilities.Attributes secondaryEff { get; protected set; }
 //  public Utilities.Attributes primaryQual { get; protected set; }
 //  public Utilities.Attributes secondaryQual { get; protected set; }
 //  public float fatigueRate { get; protected set; }
 //  /*---------------------*/
 //
 //  /****product related****/
 //  protected GameObject currentProduct;
 //  protected LineRenderer progressIndicator;
 //
 //  public bool isActive { get; protected set; }
 //
 //  protected float maxProductionTime;
 //  protected float timeToAutoCollect;
 //  public float timeToProduce { get; protected set; }
 //  public float timeModifier { get; protected set; }
 //  /*---------------------*/
 //
 //  /****  buff related ****/
 //  protected CropsAndBuffs.Buff currentBuff;
 //  protected float recoveryRestore;
 //  protected float restoreQuality;
 //  protected float buffQuality;
 //  /*---------------------*/

	/*----------------------*/
	//SPECIFIC TO THIS CLASS//
	static float BASEPESTCHANCE = .5f; //chance to detect pests per perception point

	PestBehavior pestScript;
	public bool eradicating {get; private set;}

	ForageGhostBehavior forageGhost;
	//Vector3 pestIndicatorOrigin;

	/*
	protected override void UpdateMetrics()
	{
		if (eradicating)
		{

		}
		else
		{
			base.UpdateMetrics ();
		}
	}
	*/

    /* xAssign
       called by citizens (usually when dropped) to update task 
       attributes and possibly start production*/
    public override void Assign(GameObject citizen_in)
    {
        if (citizen1 == null)
        {
            citizen1 = citizen_in;
            citizen1Script = citizen1.GetComponent<CitizenBehavior>();
            citizen1.transform.position = slot1Loc;
        }
        else if (citizen2 == null)
        {
            citizen2 = citizen_in;
            citizen2Script = citizen2.GetComponent<CitizenBehavior>();
            citizen2.transform.position = slot2Loc;
        }
        else
        {
            //FIXFIX retur to sender
        }

        Activate();
    }
    
    /* xCompleteTask
       goes about creating a product when timer reaches 0*/
    public override void CompleteTask()
    {
        base.CompleteTask();

		if (eradicating)
		{
			eradicating = false;
			pestScript.CompleteEradication ();

			progressIndicatorOrigin = transform.position + new Vector3(-1.5f, .5f, -1.8f);
			progressIndicatorLength = new Vector3(3, 0, 0);
			progressIndicator.SetPosition(0, progressIndicatorOrigin);
			progressIndicator.SetPosition(1, progressIndicatorOrigin);

			maxProductionTime = CropsAndBuffs.cropList[cropType].timeToProduce/Utilities.TIMESCALE;

			if (gameController.selectedTask == this.gameObject)
				GameObject.Find("Current_Task_UI").GetComponent<CurrentTaskUI_Script>().SetTask();

			Utilities.FloatText (transform.position + new Vector3(3, 0, 0), "pest eradicated!", Utilities.TextTypes.Positive);
		}
		else
		{
	        currentProduct = Instantiate(cropPrefab, productLoc, Quaternion.identity) as GameObject;
	        cropScript = currentProduct.GetComponent<CropBehavior>();

			restoreQuality = GetQuality();
			restoreQuality *= pestScript.ApplyDamage (restoreQuality);
	        buffQuality = 0f;

			if (!pestScript.IsRevealed())
			{
				PestCheck ();

				if (forageGhost.citizen !=  null)
					GhostCheck ();
			}

	        cropScript.CreateCrop(cropType, Utilities.ItemTypes.Crop, this.gameObject, restoreQuality, buffQuality);      
		}

		timeToProduce = maxProductionTime;
		timeToAutoCollect = maxProductionTime * Utilities.AUTOCOLLECTSCALE;

    }
    
    /* xSetItem
       called by cropbehavior when slotting a new item, 
       code varies by type of station and type of crop
       also sets buff values*/
    public override void SetItem(Utilities.CropTypes crop_in, Utilities.ItemTypes itemType_in, float restQuality_in, CropsAndBuffs.Buff buff_in)
    {
		pestScript.RotateCrop();
        itemType = itemType_in;
        cropType = crop_in;
        restoreQuality = restQuality_in;

        CropsAndBuffs.Crop newCrop = CropsAndBuffs.cropList[cropType];

        primaryEff = newCrop.primaryEff;
        secondaryEff = newCrop.secondaryEff;
        primaryQual = newCrop.primaryQual;
        secondaryQual = newCrop.secondaryQual;
        fatigueRate = newCrop.fatigueRate;
        maxProductionTime = newCrop.timeToProduce / Utilities.TIMESCALE;

        base.SetItem(crop_in, itemType_in, restQuality_in, buff_in);
    }
    
    /* xCanDrop
       asked by draggable crops to see if they can be slotted into this workstation*/
    public override bool CanDrop()
    {
        return true;
    }
    
    /* xIsFull
       Asked by citizens to see if they can be slotted into this workstation*/
    public override bool IsFull()
    {
        return (citizen1 != null && citizen2 != null);
    }

	void GhostCheck()
	{
		Vector3 distance = transform.position - forageGhost.transform.position;

		if (distance.magnitude < 6)
		{
			int perception = Mathf.CeilToInt (forageGhost.citizen.currentAttributes[Utilities.Attributes.Perception]);
			int focus = Mathf.CeilToInt (forageGhost.citizen.currentAttributes[Utilities.Attributes.Focus]);

			float chance = (perception + .5f*focus)*5*BASEPESTCHANCE;

			Utilities.FloatText (transform.position + new Vector3(2.3f, 0, .5f), chance.ToString () + "% G-reveal", Utilities.TextTypes.Dbug);
			
			if (Random.Range (0,101) <= chance)
				pestScript.Reveal ();

		}
	}

	/* xPestCheck()
	   used for perception checks on pests*/
	void PestCheck()
	{
		int perception = 0;

		if (citizen1 != null)
		{
			perception += Mathf.CeilToInt (citizen1Script.currentAttributes[Utilities.Attributes.Perception]);
		}

		if (citizen2 != null)
		{
			perception += Mathf.CeilToInt (citizen2Script.currentAttributes[Utilities.Attributes.Perception]);
		}

		float chance = perception*BASEPESTCHANCE;


		Utilities.FloatText (transform.position + new Vector3(2.3f, 0, 0), chance.ToString () + "% reveal", Utilities.TextTypes.Dbug);

		if (Random.Range (0,101) <= chance)
			pestScript.Reveal ();
	}

	/* xEradicate()
	 * used to remove pest- stops production of current crop and moves the bar indicator over*/
	public void Eradicate()
	{
		if (eradicating == false)
		{
			eradicating = true;
			pestScript.StartEradication ();
			maxProductionTime = 60/Utilities.TIMESCALE;
			timeToProduce = maxProductionTime;

			progressIndicatorOrigin = transform.position + new Vector3(3f, .5f, -1.5f);
			progressIndicatorLength = new Vector3(0,0,3f);
			progressIndicator.SetPosition(0, progressIndicatorOrigin);
			progressIndicator.SetPosition(1, progressIndicatorOrigin);

			if (gameController.selectedTask == this.gameObject)
				GameObject.Find("Current_Task_UI").GetComponent<CurrentTaskUI_Script>().SetTask();

		}
	}

    /* xGetEfficiency()
    called by this class to find efficiency given assigned citizens*/
    protected override float GetEfficiency()
    {
        return base.GetEfficiency() / 2;
    }

    /* xGetQuality
       returns a quality value based on citizen attributes
       called on product completion*/
    public override float GetQuality()
    {
        return base.GetQuality() / 2;
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
    protected override void Initialize()
    {
        /* setting up subobject locations and initial prim/secondary stats
           citizens will snap to these locations when assigned,
           and products will spawn in productLoc after completion of a cycle
           these locations are dependent on the type of the workstation and can
           vary considerably*/
        slot1Loc = new Vector3(-.666f, 0, 0) + transform.position;
        slot2Loc = new Vector3(.666f, 0, 0) + transform.position;
        productLoc = new Vector3(.666f, 0, 2.1f) + transform.position;

        primaryEff = Utilities.Attributes.None;
        secondaryEff = Utilities.Attributes.None;
        primaryQual = Utilities.Attributes.None;
        secondaryQual = Utilities.Attributes.None;

        progressIndicatorOrigin = transform.position + new Vector3(-1.5f, .5f, -1.8f);
        progressIndicatorLength = new Vector3(3, 0, 0);

		pestScript = transform.GetChild (5).GetComponent<PestBehavior>();
		//progressIndicatorOrigin = transform.position + new Vector3(3f, .5f, -1.8f);

		forageGhost = GameObject.Find("Ghost").GetComponent<ForageGhostBehavior>();
        base.Initialize();
    }




    ///* xUpdateMetrics
    //    called every frame to update things that are time dependent*/
    //protected void UpdateMetrics()
    //{
    //    //uses base class
    //}
    //
    //    //uses base class
    //}
    ///* xUnassign
    //    called by citizens when they are attempting to be 
    //    assigned elsewhere - may stop production as well*/
    //public void Unassign(GameObject citizen_in)
    //{
    //    //uses base class
    //}
    ///* xGetTimeModifier()
    //    called by metrics update to know how much that assigned
    //    citizens contribute to reducing production time*/
    //protected void GetTimeModifier()
    //{
    //    //uses base class
    //}
    //
    ///* xConvertedTimeModifier
    //   called as a convenience whenever the time modifier must
    //   be inverted for calculation*/
    //public float ConvertedTimeModifier()
    //{
    //    //uses base class
    //}
    //
    ///* xActivate
    //   called from this script or from citizens or from cropbehavior
    //   boolean flag is used because other scripts check for activity 
    //   periodically*/
    //public void Activate()
    //{
    //    //uses base class
    //}
    //
    ///* xDeactivate
    //  called from thsi script or from citizens when unassigned
    //  boolean flag used because other scripts check for activity
    //  periodically*/
    //public void Deactivate()
    //{
    //    //uses base class
    //}
    //
    ///* xSelect
    //   called by Game Controller when clicked on*/
    //public void Select()
    //{
    //    //uses base class
    //}
    //
    ///* xDeselect
    //    called by Game Controller when another station is clicked on*/
    //public void Deselect()
    //{
    //    //uses base class
    //}
    //
    ///*xRemoveProductReference
    //  cropBehavior calls this when the item is collected
    //  because the object is not destroyed in that specific case*/
    //public void RemoveProductReference()
    //{
    //    //uses base class
    //}

}
