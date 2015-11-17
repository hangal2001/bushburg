using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//Main script for individual citizens
public class CitizenBehavior : MonoBehaviour 
{
    public GameController_Script gameController;
    GameObject selectedTask;    //gets selected task from gamecontroller for colormanagement purposes
    Renderer render;	//for color management

    public GameObject selectedIndicator;    //child object of each citizen, active if selected
    public GameObject buffIndicator;
    ParticleSystem buffParticles;

    GameObject idlePad;

    //access = maxAttributes[attrib];
    public Dictionary<Utilities.Attributes, float> maxAttributes;
    public Dictionary<Utilities.Attributes, float> currentAttributes;

    //anytime this citizen is assigned to a new task, these are updated from
    //the task it is assigned to
    public Utilities.Attributes primaryEff { get; private set; }
    public Utilities.Attributes secondaryEff { get; private set; }
    public Utilities.Attributes primaryQual { get; private set; }
    public Utilities.Attributes secondaryQual { get; private set; }
    public float fitness { get; private set; } //used by tasks to calculate progression speed
    float fatigueRate;

    
	public GameObject currentProspect {get; private set;}	//holds the last slottable object this citizen collided with
															//referenced when the citizen is dropped
															//stored here b/c you cannot manually detect collisions

	public GameObject currentSlot {get; private set;}       //holds the current pad/plot that the citizen is slotted into

    public bool canMove { get; private set; }
    public bool isActive{get; private set;}
    
    public CropsAndBuffs.Buff currentBuff;
    public Utilities.CropTypes buffCropType { get; private set; } //used for UI

	void Awake()
	{
		gameController = GameObject.Find ("GameController").GetComponent<GameController_Script>();

		Deselect ();
		Unbuff();
		canMove = true;

		maxAttributes = new Dictionary<Utilities.Attributes, float>();
		currentAttributes = new Dictionary<Utilities.Attributes, float>();

		render = this.gameObject.GetComponent<Renderer>();

		idlePad = GameObject.Find ("Pad_Idle");

		primaryEff = Utilities.Attributes.None;
		secondaryEff = Utilities.Attributes.None;
		primaryQual = Utilities.Attributes.None;
		secondaryQual = Utilities.Attributes.None;
		fatigueRate = 0f;

		buffParticles = buffIndicator.GetComponent<ParticleSystem>();

	}


	// Update is called once per frame
	void Update () 
	{
		UpdateMetrics();
	}

	void LateUpdate()
	{
		AdjustColors();
	}

	//+++++++Core Every-Frame Functions++++++//

	//If something is updated every frame regardless of input, put it here
	void UpdateMetrics()
	{
		//updating canMove status
		if (currentSlot.tag == "WorkStation")
		{
			WorkStationBehavior currentScript = currentSlot.GetComponent<WorkStationBehavior>();

            if (currentScript.stationType != Utilities.WorkStations.FarmPlot)
			    canMove = !currentScript.isActive;
		}
        

		fitness = GetFitness ();

		//if (gameController.selectedCitizen == this.gameObject && primaryEff != Utilities.Attributes.None)
		//	print (currentAttributes[primaryEff] + " " + currentAttributes[secondaryEff] + " " + currentAttributes[primaryQual] + " " + currentAttributes[secondaryQual] + " " + fitness);

		Fatigue ();

		if (currentBuff.buffType != Utilities.BuffTypes.None)
		{
			if (currentBuff.duration > 0)
			{
				currentBuff.duration -= Time.deltaTime*Utilities.TIMESCALE;
				buffParticles.startSize = Mathf.Min (.9f, Mathf.Max (currentBuff.duration/currentBuff.maxDuration, .1f));
                BuffFilter();
			}
			else
			{
				Unbuff ();
			}
		}
	}

    //runs after fatigue to check for buffs
    //if they exist, then attributes will be readjusted to compensate for the buff effects
    void BuffFilter()
    {
        //setting up some variables so the equations aren't silly long
        //recovery ratio = percentage of recovery out of max
        //current recovery/fatigue = how much of a primary attribute was recovered/fatigued this tick
        //curren att = attribute that is affected by the buff
        float recoveryRatio = Mathf.Ceil(currentAttributes[Utilities.Attributes.Recovery]) / maxAttributes[Utilities.Attributes.Recovery];
        float currentRecovery = Time.deltaTime  * Utilities.FATIGUESCALE * recoveryRatio;
        float currentFatigue = Time.deltaTime * Utilities.FATIGUESCALE * fatigueRate;
        float currentAtt = currentAttributes[currentBuff.attribute];
        float difference = 0f;

        if (currentBuff.buffType == Utilities.BuffTypes.Recovery)
        {
            difference = currentRecovery * (currentBuff.value / 100);
            
            currentAtt = Mathf.Min(currentAtt + difference, maxAttributes[currentBuff.attribute]);
            currentAttributes[currentBuff.attribute] = currentAtt;
        }
        else if (currentBuff.buffType == Utilities.BuffTypes.Drain)
        {
            difference = currentFatigue * (currentBuff.value / 100);
            currentAtt = Mathf.Min(currentAtt + difference, maxAttributes[currentBuff.attribute]);
            currentAttributes[currentBuff.attribute] = currentAtt;
        }
        else if (currentBuff.buffType == Utilities.BuffTypes.AttributeScalar)
        {
            //probably won't do anything

        }
        else if (currentBuff.buffType == Utilities.BuffTypes.AttributeLockPositive)//positive = food buff
        {
            if (currentAttributes[currentBuff.attribute] < currentBuff.value)
                currentAttributes[currentBuff.attribute] = currentBuff.value;

        }

    }

    void Fatigue()
	{
		float currentRecovery = (Time.deltaTime/maxAttributes[Utilities.Attributes.Recovery]) * Utilities.FATIGUESCALE * (Mathf.Ceil (currentAttributes[Utilities.Attributes.Recovery]));
		float currentFatigue = Time.deltaTime * Utilities.FATIGUESCALE * fatigueRate;

        //debug prints
        //if (gameController.selectedCitizen == this.gameObject)
            //print("rate : " + fatigueRate + " scale: " + Utilities.FATIGUESCALE + " curfat: " + currentFatigue + "currec: " + currentRecovery);

        //recover if citizen is idle, recover everything
		if (!isActive || primaryEff == Utilities.Attributes.None)
		{
			for(int c=4; c < 10; c++)
			{
				float currentAtt = currentAttributes[(Utilities.Attributes)c];

				currentAttributes[(Utilities.Attributes)c] = Mathf.Min (currentAtt + currentRecovery*Utilities.IDLEBONUS, maxAttributes[(Utilities.Attributes)c]);
			}
		}
		else //citizen is working
		{
			for (int c=4; c < 10; c++)
			{
				//fatigue stats
				if ((Utilities.Attributes)c == primaryEff || (Utilities.Attributes)c == primaryQual)
				{
					currentAttributes[(Utilities.Attributes)c] = Mathf.Max (currentAttributes[(Utilities.Attributes)c] - currentFatigue, 0);
				}
				//recovery stats
				else if (((Utilities.Attributes)c != secondaryEff || (Utilities.Attributes)c != secondaryQual))
				{
					float currentAtt = currentAttributes[(Utilities.Attributes)c];

					currentAttributes[(Utilities.Attributes)c] = Mathf.Min (currentAtt + currentRecovery, maxAttributes[(Utilities.Attributes)c]);
				}
			}
		}

		//producing the hunger effect
		//recovery does not reduce while idle or while in a station that is not functional
		//recovery currently is lost 3x faster if a primary stat is fully drained (simulating exhaustion)
		if (isActive && primaryEff != Utilities.Attributes.None)
		{
			if (currentAttributes[primaryEff] == 0 || currentAttributes[primaryQual] == 0)
			{
				currentAttributes[Utilities.Attributes.Recovery] = Mathf.Max (currentAttributes[Utilities.Attributes.Recovery] - Time.deltaTime*Utilities.HUNGERSCALE*3, 1);
			}
			else
			{
				currentAttributes[Utilities.Attributes.Recovery] = Mathf.Max (currentAttributes[Utilities.Attributes.Recovery] - Time.deltaTime*Utilities.HUNGERSCALE, 1);
			}
		}
	}



	//+++++++Assignment Functions++++++//



	//attempt to assign this citizen to currentProspect
	//reverts to its old assignment if that can't be done
	//BE CAREFUL ABOUT ORDER OF 'IF CHAIN' HERE
	public void Assign()
	{
		Unassign ();

		if (currentProspect == null)
		{
			currentProspect = currentSlot;
		}

		if (currentProspect.tag == "WorkStation" && currentProspect.GetComponent<WorkStationBehavior>().IsFull())
		{
			currentProspect = currentSlot;
		}

		if (currentProspect.tag == "Pad")
		{
			PadBehavior currentScript = currentProspect.GetComponent<PadBehavior>();
			currentScript.Assign (this.gameObject);
			SetTaskAttributes (currentScript.primaryEff, currentScript.secondaryEff, currentScript.primaryQual, currentScript.secondaryQual, currentScript.fatigueRate);

			currentSlot = currentProspect;
		}
		else if (currentProspect.tag == "WorkStation")
		{
			WorkStationBehavior currentScript = currentProspect.GetComponent<WorkStationBehavior>();
			currentScript.Assign (this.gameObject);
			SetTaskAttributes (currentScript.primaryEff, currentScript.secondaryEff, currentScript.primaryQual, currentScript.secondaryQual, currentScript.fatigueRate);

			currentSlot = currentProspect;
		}
		else
		{
			print ("ERROR IN CITIZENBEHAVIOR ASSIGN - TAG OF CURRENT PROSPECT NOT HANDLED");
		}


	}

	void Unassign()
	{
		Deactivate ();

		if (currentSlot.tag == "Pad")
		{
			currentSlot.GetComponent<PadBehavior>().Unassign (this.gameObject);
		}
		else if (currentSlot.tag == "WorkStation")
		{
			currentSlot.GetComponent<WorkStationBehavior>().Unassign (this.gameObject);
		}
		else
		{
			print ("ERROR IN CITIZENBEHAVIOR UNASSIGN - TAG OF CURRENT SLOT NOT HANDLED");
		}
	}



	//+++++++Task Functions++++++//


	public void Feed(float amount, CropsAndBuffs.Buff buff_in, Utilities.CropTypes cropType_in)
	{
        if (buff_in.buffType != Utilities.BuffTypes.None)
        {
            currentBuff = buff_in;
            currentBuff.duration *= Utilities.BUFFSCALE;
            buffCropType = cropType_in;
            ApplyBuff(currentBuff);

        }

        //eating provides a small boost to all stats depending on how much was recovered
        //recoveryDifference is used to make sure this bonus doesn't encroach into full recovery
        //(ie: eating when already full)
        float recoveryDifference = currentAttributes[Utilities.Attributes.Recovery];
        currentAttributes[Utilities.Attributes.Recovery] = Mathf.Min (currentAttributes[Utilities.Attributes.Recovery]+amount, 10);

        //recoveryDifference now refers to the difference between old and new
        recoveryDifference = currentAttributes[Utilities.Attributes.Recovery] - recoveryDifference;

        for (int c = (int)Utilities.Attributes.Strength; c <= (int)Utilities.Attributes.Acumen; c++)
        {
            currentAttributes[(Utilities.Attributes)c] = Mathf.Min(currentAttributes[(Utilities.Attributes)c] + recoveryDifference*Utilities.MEALBONUS, maxAttributes[(Utilities.Attributes)c]);
        }


        //change buff in ui if needed
        if (gameController.selectedCitizen == this.gameObject)
            GameObject.Find("Current_Buff_UI").GetComponent<CurrentBuffUI_Script>().SetBuff();

    }


	public void SetTaskAttributes(Utilities.Attributes pEff_in, Utilities.Attributes sEff_in, Utilities.Attributes pQual_in, Utilities.Attributes sQual_in, float fatigue_in)
	{

		primaryEff = pEff_in;
		secondaryEff = sEff_in;
		primaryQual = pQual_in;
		secondaryQual = sQual_in;

		fatigueRate = fatigue_in;
	
	}

	public float GetFitness()
	{
		if (primaryEff == Utilities.Attributes.None)
			return -1;


		float primEff, secEff, primQual, secQual;

		primEff = Mathf.Ceil (currentAttributes[primaryEff]);
		secEff = Mathf.Ceil (currentAttributes[secondaryEff]);
		primQual = Mathf.Ceil (currentAttributes[primaryQual]);
		secQual = Mathf.Ceil (currentAttributes[secondaryQual]);

		return primEff + primQual + secEff/2 + secQual/2;

	}

	public float GetFitnessFromSelectedTask()
	{
		if (selectedTask == null || selectedTask == idlePad)
			return -1;

		float primEff, secEff, primQual, secQual;

        if (selectedTask.tag == "WorkStation")
		{
			WorkStationBehavior taskScript = selectedTask.GetComponent<WorkStationBehavior>();

			if (taskScript.primaryEff == Utilities.Attributes.None)
				return -1;

			primEff = Mathf.Ceil (currentAttributes[taskScript.primaryEff]);
			secEff = Mathf.Ceil (currentAttributes[taskScript.secondaryEff]);
			primQual = Mathf.Ceil (currentAttributes[taskScript.primaryQual]);
			secQual = Mathf.Ceil (currentAttributes[taskScript.secondaryQual]);
		}
		else
		{
			print ("ERROR IN CIVILIANBEHAVIOR GETFITNESSFROMSELECTEDTASK: TAG NOT HANDLED");
			return -1;
		}

		return primEff + primQual + secEff/2 + secQual/2;
	}

	public float GetEfficiency()
	{
		if (primaryEff == Utilities.Attributes.None)
			return -1;
		
		
		float primEff, secEff;
		
		primEff = Mathf.Ceil (currentAttributes[primaryEff]);
		secEff = Mathf.Ceil (currentAttributes[secondaryEff]);
		
		return primEff + secEff/2;
	}

	public float GetQuality()
	{
		if (primaryEff == Utilities.Attributes.None)
			return -1;
		
		float primQual, secQual;

		primQual = Mathf.Ceil (currentAttributes[primaryQual]);
		secQual = Mathf.Ceil (currentAttributes[secondaryQual]);
		
		return primQual + secQual/2;
	}

	void AdjustColors()
	{
		GetSelectedTask ();

		if (gameController.colorMode == Utilities.ColorModes.CurrentFit)
		{

			if (fitness > 0)
			{
				float green = fitness/15;
				float red = 2 - fitness/15;
				
				render.material.SetColor ("_Color", new Color(red,green, 0f, 1f));
			}
			else
			{
				render.material.SetColor ("_Color",Color.black);
			}
		}
		else if (gameController.colorMode == Utilities.ColorModes.BestFit && selectedTask != null)
		{
			float fitnessToSelected = GetFitnessFromSelectedTask ();

			if (fitnessToSelected > 0)
			{
				float green = fitnessToSelected/15;
				float red = 2 - fitnessToSelected/15;
				
				render.material.SetColor ("_Color", new Color(red,green, 0f, 1f));
			}
			else
			{
				render.material.SetColor ("_Color",Color.black);
			}
			//print (primaryEff + " " + secondaryEff + " " + primaryQual + " " + secondaryQual + " " + fitness);
		}
		else//not currently used
		{
			render.material.SetColor ("_Color",Color.black);
		}
	}


	//+++++++Trigger Functions++++++//



	//when being wiggled around by mouse, a citizen will collide with things
	//we have to use the OnTriggerEnter because it won't let us manually
	//check collisions later
	void OnTriggerEnter(Collider collision_in)
	{
		
		if (collision_in.tag == "Pad")
		{
			currentProspect = collision_in.gameObject;
		}
		else if (collision_in.tag == "WorkStation")
		{
			currentProspect = collision_in.gameObject;
		}
	}
	
	//this is checked to prevent manual location of citizens
	//probably only a milestone 1 thing
	void OnTriggerExit(Collider collision_in)
	{
		if (collision_in.tag == "Pad")
		{
			currentProspect = null;
		}
		else if (collision_in.tag == "WorkStation")
		{
			currentProspect = null;
		}
	}





	//+++++++Debug, Setup, Misc++++++//

    //called by citizenmanager during initial set up
    //should not otherwise be used
    public void SetAttributes(int str_in, int dex_in, int end_in, int perc_in, int foc_in, int acu_in)
    {
        maxAttributes.Add(Utilities.Attributes.Health, 10);
        maxAttributes.Add(Utilities.Attributes.Happiness, 10);
        maxAttributes.Add(Utilities.Attributes.Recovery, 10);
        currentAttributes.Add(Utilities.Attributes.Health, 10);
        currentAttributes.Add(Utilities.Attributes.Happiness, 10);
        currentAttributes.Add(Utilities.Attributes.Recovery, 10);

        maxAttributes.Add(Utilities.Attributes.Strength, str_in);
        currentAttributes.Add(Utilities.Attributes.Strength, str_in);

        maxAttributes.Add(Utilities.Attributes.Dexterity, dex_in);
        currentAttributes.Add(Utilities.Attributes.Dexterity, dex_in);

        maxAttributes.Add(Utilities.Attributes.Endurance, end_in);
        currentAttributes.Add(Utilities.Attributes.Endurance, end_in);

        maxAttributes.Add(Utilities.Attributes.Perception, perc_in);
        currentAttributes.Add(Utilities.Attributes.Perception, perc_in);

        maxAttributes.Add(Utilities.Attributes.Focus, foc_in);
        currentAttributes.Add(Utilities.Attributes.Focus, foc_in);

        maxAttributes.Add(Utilities.Attributes.Acumen, acu_in);
        currentAttributes.Add(Utilities.Attributes.Acumen, acu_in);
    }

    /*
	//currently used to help test the UI
	void GenerateRandomAttributes()
	{
		for (int c=1; c < 10; c++)
		{
			int newValue = Random.Range (1,11);
			maxAttributes.Add ((Utilities.Attributes)c, newValue);
			currentAttributes.Add ((Utilities.Attributes)c, newValue);

			//print ("setting " + (Utilities.Attributes)c + " to " + newValue);

		}

		maxAttributes[Utilities.Attributes.Recovery] = 10;
		currentAttributes[Utilities.Attributes.Recovery] = 10;
		//print (maxHealth + " " + maxHappiness + " " + maxRecovery + " " + maxStr + " " + maxDex + " " + maxEnd);
	}*/

	void GetSelectedTask()
	{
		selectedTask = gameController.GetComponent<GameController_Script>().selectedTask;
	}

	//currently using this for debugging
	public void PrintAttribute(Utilities.Attributes attrib_in)
	{
		print (attrib_in + " max: " + maxAttributes[attrib_in] + ", current: " + currentAttributes[attrib_in]);
	}

	public void Select()
	{
		selectedIndicator.SetActive (true);
		
	}
	
	public void Deselect()
	{
		selectedIndicator.SetActive (false);
	}

    public void ApplyBuff(CropsAndBuffs.Buff buff_in)
    {
        //to remove any old buff effects
        Unbuff();

        currentBuff = buff_in;

        if (currentBuff.buffType != Utilities.BuffTypes.None)
        {

            buffIndicator.SetActive(true);
            //currentBuff.duration = currentBuff.maxDuration;

            //increase max attribute
            if (currentBuff.buffType == Utilities.BuffTypes.AttributeScalar)
            {
                maxAttributes[currentBuff.attribute] += (int)Mathf.Floor(currentBuff.value);
                currentAttributes[currentBuff.attribute] += (int)Mathf.Floor(currentBuff.value);
            }
            //print (currentBuff.name + " duration: " + currentBuff.duration + " value: " + currentBuff.value);
        }
        else
        {
            //print ("NO BUFF");
        }
    }

    public void Unbuff ()
	{
        if (currentBuff.buffType == Utilities.BuffTypes.AttributeScalar)
        {
            maxAttributes[currentBuff.attribute] -= (int)Mathf.Floor(currentBuff.value) ;
            currentAttributes[currentBuff.attribute] = Mathf.Min(currentAttributes[currentBuff.attribute], maxAttributes[currentBuff.attribute]);
        }

		currentBuff.buffType = Utilities.BuffTypes.None;
		buffIndicator.SetActive (false);
	}

	public void Activate()
	{
		isActive = true;
	}

	public void Deactivate()
	{
		isActive = false;
	}

	//DO NOT USE THIS GENERALLY
	//This is specifically called once when citizens are created
	//may be refactored out later
	public void Assign(GameObject pad_in)
	{
		pad_in.GetComponent<PadBehavior>().Assign (this.gameObject);
		currentProspect = pad_in;
		currentSlot = pad_in;
	}
	
}
