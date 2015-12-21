using UnityEngine;
using System.Collections;

public class WorkStationBehavior : MonoBehaviour {

    /****set from editor****/
    public GameObject cropPrefab;
    public GameObject progressIndicatorPrefab;
    public GameObject selectedIndicator;
    /*---------------------*/

    /****   locations  ****/
    protected Vector3 progressIndicatorOrigin;
    protected Vector3 progressIndicatorLength;
    protected Vector3 slot1Loc, slot2Loc;
    protected Vector3 productLoc;
    /*---------------------*/

    /****    scripts   ****/
    protected GameController_Script gameController;
    protected CitizenBehavior citizen1Script;
    protected CitizenBehavior citizen2Script;
    protected CropBehavior cropScript;
    /*---------------------*/

    /****     enums    ****/
    public Utilities.ItemTypes itemType { get; protected set; }
    public Utilities.WorkStations stationType;
    public Utilities.CropTypes cropType { get; protected set; }
    /*---------------------*/

    /****citizen related****/
    public GameObject citizen1 { get; protected set; }
    public GameObject citizen2 { get; protected set; }

    public Utilities.Attributes primaryEff { get; protected set; }
    public Utilities.Attributes secondaryEff { get; protected set; }
    public Utilities.Attributes primaryQual { get; protected set; }
    public Utilities.Attributes secondaryQual { get; protected set; }
    public float fatigueRate { get; protected set; }
    /*---------------------*/

    /****product related****/
    protected GameObject currentProduct;
    protected LineRenderer progressIndicator;

    public bool isActive { get; protected set; }

    protected float maxProductionTime;
    protected float timeToAutoCollect;
    public float timeToProduce { get; protected set; }
    public float timeModifier { get; protected set; }
    /*---------------------*/

    /****  buff related ****/
    protected CropsAndBuffs.Buff currentBuff;
    protected float recoveryRestore;
    protected float restoreQuality;
    protected float buffQuality;
    /*---------------------*/


    // Use this for initialization
    void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController_Script>();

        Deselect();

        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMetrics();
    }

    /* xUpdateMetrics
       called every frame to update things that are time dependent*/
    protected virtual void UpdateMetrics()
    {
        GetTimeModifier();

        if (isActive && currentProduct == null)
        {
            timeToProduce -= Time.deltaTime * ConvertedTimeModifier();

            //if(gameController.selectedTask == this.gameObject)
            // print(timeToProduce / Utilities.TIMESCALE);

            float progressPercentage = Mathf.Min(1 - timeToProduce / maxProductionTime, 1);

            progressIndicator.SetPosition(1, progressIndicatorOrigin + progressIndicatorLength * progressPercentage);

            //if (gameController.selectedTask == this.gameObject)
            //print (timeModifier + " " + (1/(1-timeModifier)) + " " + timeToProduce);

            if (timeToProduce <= 0)
            {
                CompleteTask();
            }
        }

        if (isActive && currentProduct != null)
        {
            if (timeToAutoCollect < 0)
            {
                cropScript.Collect();
            }
            else
            {
                timeToAutoCollect -= Time.deltaTime * ConvertedTimeModifier();
            }
        }

    }

    /* xUnassign
       called by citizens when they are attempting to be 
       assigned elsewhere - may stop production as well*/
    public virtual void Unassign(GameObject citizen_in)
    {
        if (citizen1 != null && citizen_in == citizen1)
        {
            citizen1 = null;
            citizen1Script = null;
        }
        else if (citizen2 != null && citizen_in == citizen2)
        {
            citizen2 = null;
            citizen2Script = null;
        }
        else
        {
            print("ERROR IN WORKSTATION UNASSIGN: CITIZEN NOT ASSIGNED TO THIS WORKSTATION");
        }

        if (citizen1 == null && citizen2 == null)
        {
            Deactivate();
        }
    }

    /* xSelect
        called by Game Controller when clicked on*/
    public void Select()
    {
        selectedIndicator.SetActive(true);
    }

    /* xDeselect
        called by Game Controller when another station is clicked on*/
    public void Deselect()
    {
        selectedIndicator.SetActive(false);
    }

    /*xRemoveProductReference
      cropBehavior calls this when the item is collected
      because the object is not destroyed in that specific case*/
    public void RemoveProductReference()
    {
        currentProduct = null;
        cropScript = null;
    }

    /* xGetTimeModifier()
        called by metrics update to know how much that assigned
        citizens contribute to reducing production time*/
    protected void GetTimeModifier()
    {
        float efficiency = GetEfficiency();

        if (efficiency > 0)
        {
            timeModifier = Utilities.MAXTIMEMODIFIER * (efficiency / 15);
        }
        else
        {
            timeModifier = 0;
        }
    }

    /* xGetEfficiency()
        called by this class to find efficiency given assigned citizens*/
    protected virtual float GetEfficiency()
    {
        float efficiency = 0;

        if (citizen1 != null)
        {
            efficiency += citizen1Script.GetEfficiency();
        }

        if (citizen2 != null)
        {
            efficiency += citizen2Script.GetEfficiency();
        }

        return efficiency;
    }

    /* xConvertedTimeModifier
       called as a convenience whenever the time modifier must
       be inverted for calculation*/
    public float ConvertedTimeModifier()
    {
        return (1 / (1 - timeModifier));
    }

    /* xGetQuality
   returns a quality value based on citizen attributes
   called on product completion*/
    public virtual float GetQuality()
    {
        float quality = 0;

        if (citizen1 != null)
        {
            quality = citizen1Script.GetQuality();
            quality /= 15;
        }

        if (citizen2 != null)
        {
            quality += citizen2Script.GetQuality() / 15;
        }

        return quality;
    }

    /* xDeactivate
      called from thsi script or from citizens when unassigned
      boolean flag used because other scripts check for activity
      periodically*/
    public void Deactivate()
    {
        isActive = false;

        if (citizen1 != null)
        {
            citizen1Script.Deactivate();
        }

        if (citizen2 != null)
        {
            citizen2Script.Deactivate();
        }

    }


    /****  VIRTUAL METHODS   ****/

    /* xActivate
       called from this script or from citizens or from cropbehavior
       boolean flag is used because other scripts check for activity 
       periodically*/
    public virtual void Activate()
    {
        if (citizen1 != null || citizen2 != null)
        {
            if (cropType != Utilities.CropTypes.None)
            {
                isActive = true;

                if (citizen1 != null)
                {
                    citizen1Script.Activate();
                }

                if (citizen2 != null)
                {
                    citizen2Script.Activate();
                }
            }
        }

    }

    /* xAssign
        called by citizens (usually when dropped) to update task 
        attributes and possibly start production
        
        base class works for 1 citizen*/
    public virtual void Assign(GameObject citizen_in)
    {

        if (citizen1 == null)
        {
            citizen1 = citizen_in;
            citizen1Script = citizen1.GetComponent<CitizenBehavior>();
            citizen1.transform.position = slot1Loc;
        }
        else
        {
            //FIXFIX return to sender
        }

        //If possible, activate workstation
        Activate();

    }

    /* xCompleteTask
       goes about creating a product when timer reaches 0*/
    public virtual void CompleteTask()
    {
		progressIndicator.SetPosition(0, progressIndicatorOrigin);
        progressIndicator.SetPosition(1, progressIndicatorOrigin);
    }

    /* xSetItem
       called by cropbehavior when slotting a new item, 
       code varies by type of station and type of crop
       also sets buff values*/
    public virtual void SetItem(Utilities.CropTypes crop_in, Utilities.ItemTypes itemType_in, float restQuality_in, CropsAndBuffs.Buff buff_in)
    {
        timeToProduce = maxProductionTime;

        if (citizen1 != null)
        {
            citizen1Script.SetTaskAttributes(primaryEff, secondaryEff, primaryQual, secondaryQual, fatigueRate);

            Activate();
        }

        if (citizen2 != null)
        {
            citizen2Script.SetTaskAttributes(primaryEff, secondaryEff, primaryQual, secondaryQual, fatigueRate);

            Activate();
        }

        Utilities.SetCropTexture(this.gameObject.transform.GetChild(1).gameObject, crop_in);

        if (gameController.selectedTask == this.gameObject)
            GameObject.Find("Current_Task_UI").GetComponent<CurrentTaskUI_Script>().SetTask();
    }

    /* xCanDrop
   Asked by draggable crops to see if they can be slotted into this workstation*/
    public virtual bool CanDrop()
    {
        return (!isActive && cropType == Utilities.CropTypes.None);
    }

    /* xIsFull
       Asked by citizens to see if they can be slotted into this workstation*/
    public virtual bool IsFull()
    {
        return (citizen1 != null);
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
    protected virtual void Initialize()
    {
        //print(slot1Loc);
        progressIndicator = progressIndicatorPrefab.GetComponent<LineRenderer>();

        timeToProduce = 0;
        timeModifier = 0;
        timeToAutoCollect = 0;
        fatigueRate = 1;

        currentBuff.buffType = Utilities.BuffTypes.None;

        //initializing line renderer to appear empty
        progressIndicator.SetPosition(0, progressIndicatorOrigin);
        progressIndicator.SetPosition(1, progressIndicatorOrigin);
    }
}
