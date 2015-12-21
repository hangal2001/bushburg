using UnityEngine;
using System.Collections;

public class ForagerBehavior : WorkStationBehavior

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
	
	Vector3 pathStartLoc;
	Vector3 pathEndLoc;

	ForageGhostBehavior ghost;

	protected override void UpdateMetrics()
	{
		base.UpdateMetrics ();

		if (Input.GetKeyDown ("g"))
		{
			setIconSearch ();
		}

		if (Input.GetKeyDown ("h"))
		{
			setIconPest ();
		}
	}

	/* xAssign
       called by citizens (usually when dropped) to update task 
       attributes and possibly start production*/
	public override void Assign(GameObject citizen_in)
	{
		base.Assign (citizen_in);

		ghost.Initialize(pathStartLoc, citizen_in.GetComponent<CitizenBehavior>());
		setIconSearch ();
	}

	/* xUnassign
	    called by citizens when they are attempting to be 
	    assigned elsewhere - may stop production as well*/
	public override void Unassign(GameObject citizen_in)
	{
		base.Unassign (citizen_in);
		ghost.Deactivate();
	}
	
	/* xCompleteTask
       goes about creating a product when timer reaches 0*/
	public override void CompleteTask()
	{
		base.CompleteTask();
		
		
	}
	
	void setIconSearch()
	{
		Utilities.SetTexture(this.gameObject.transform.GetChild(1).gameObject, "Creatures/search");
	}

	void setIconPest()
	{
		Utilities.SetTexture(this.gameObject.transform.GetChild(1).gameObject, "Creatures/pest");
	}
	
	/* xCanDrop
       asked by draggable crops to see if they can be slotted into this workstation*/
	public override bool CanDrop()
	{
		return false;
	}
	
	/* xIsFull
       Asked by citizens to see if they can be slotted into this workstation*/
	public override bool IsFull()
	{
		return base.IsFull ();
	}
	
	/* xGetEfficiency()
    called by this class to find efficiency given assigned citizens*/
	protected override float GetEfficiency()
	{
		return base.GetEfficiency();
	}
	
	/* xGetQuality
       returns a quality value based on citizen attributes
       called on product completion*/
	public override float GetQuality()
	{
		return base.GetQuality ();
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
		base.Initialize();

		slot1Loc = transform.position + new Vector3(1, 0, 0);
		pathStartLoc = transform.position + new Vector3(0, 0, 28);

		ghost = transform.GetChild (4).gameObject.GetComponent<ForageGhostBehavior>();

		ghost.Deactivate ();
		
	}
	
	
	
	
	///* xUpdateMetrics
	//    called every frame to update things that are time dependent*/
	//protected void UpdateMetrics()
	//{
	//    //uses base class
	//}
	//
	///* xGetQuality
	//   returns a quality value based on citizen attributes
	//   called on product completion*/
	//public float GetQuality()
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
	///* xSetItem
    //   called by cropbehavior when slotting a new item, 
    //   code varies by type of station and type of crop
    //   also sets buff values
	//public override void SetItem(Utilities.CropTypes crop_in, Utilities.ItemTypes itemType_in, float restQuality_in, CropsAndBuffs.Buff buff_in)
	//{
	//	//does nothing in this workstation
	//}*/
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
