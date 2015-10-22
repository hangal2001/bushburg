using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//Main script for individual citizens
public class CitizenBehavior : MonoBehaviour 
{
	static float FATIGUESCALE = (1f/10f)*Utilities.TIMESCALE;//how many seconds to fatigue if fatigue rate is 1
															 //a higher denominator means more seconds
	static float HUNGERSCALE = (1f/60f)*Utilities.TIMESCALE;//how many seconds for the recovery stat to drop by 1
															//higher denominator means more seconds

	public GameController_Script gameController;

	public bool canMove{get; private set;}
	public GameObject currentProspect {get; private set;}	//holds the last slottable object this citizen collided with
															//referenced when the citizen is dropped
															//stored here b/c you cannot manually detect collisions

	public GameObject currentSlot {get; private set;}		//holds the current pad/plot that the citizen is slotted into
	
	GameObject selectedTask;	//gets selected task from gamecontroller for colormanagement purposes

	/*GET UI VALUES FOR ATTRIBUTES FROM HERE
	  EXAMPLE REFERENCE MIGHT LOOK LIKE 

	  int currentAttribute;
	  currentAttribute = dictName[Utilities.Attributes.Strength];

	 */
    public Dictionary<Utilities.Attributes, int> maxAttributes;
    public Dictionary<Utilities.Attributes, float> currentAttributes;

	public GameObject selectedIndicator;	//child object of each citizen, active if selected
	public GameObject buffIndicator;
	ParticleSystem buffParticles;
	float buffDurationLeft;
	
	Renderer render;	//for color management

	GameObject idlePad;

	//anytime this citizen is assigned to a new task, these are updated from
	//the task it is assigned to
	Utilities.Attributes primaryEff;
	Utilities.Attributes secondaryEff;
	Utilities.Attributes primaryQual;
	Utilities.Attributes secondaryQual;
	float fatigueRate;
	public bool isActive{get; private set;}

	CropsAndBuffs.Buff currentBuff;

	public float fitness{get; private set;} //used by tasks to calculate progression speed

	void Awake()
	{
		gameController = GameObject.Find ("GameController").GetComponent<GameController_Script>();

		Deselect ();
		Unbuff();
		canMove = true;

		maxAttributes = new Dictionary<Utilities.Attributes, int>();
		currentAttributes = new Dictionary<Utilities.Attributes, float>();
		GenerateRandomAttributes ();

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
        UpdateCurrentCitizenAttributeValues(); // update UI slider Attributes
	}

	//+++++++Core Every-Frame Functions++++++//


	//If something is updated every frame regardless of input, put it here
	void UpdateMetrics()
	{
		//updating canMove status
		if (currentSlot.tag == "WorkStation")
		{
			WorkStationBehavior currentScript = currentSlot.GetComponent<WorkStationBehavior>();
			canMove = !currentScript.isActive;
		}

		fitness = GetFitness ();

		//if (gameController.selectedCitizen == this.gameObject && primaryEff != Utilities.Attributes.None)
		//	print (currentAttributes[primaryEff] + " " + currentAttributes[secondaryEff] + " " + currentAttributes[primaryQual] + " " + currentAttributes[secondaryQual] + " " + fitness);

		Fatigue ();

		if (currentBuff.buffType != Utilities.BuffTypes.None)
		{
			if (buffDurationLeft > 0)
			{
				buffDurationLeft -= Time.deltaTime;
				buffParticles.startSize = Mathf.Min (.9f, Mathf.Max (buffDurationLeft/currentBuff.duration, .1f));
			}
			else
			{
				Unbuff ();
			}
		}
	}

	void Fatigue()
	{
		//if (gameController.selectedCitizen == this.gameObject)
			//print ("dex: " + currentAttributes[Utilities.Attributes.Dexterity] + " str: " + currentAttributes[Utilities.Attributes.Strength] + " end: " + currentAttributes[Utilities.Attributes.Recovery]);

		float currentRecovery = Mathf.Ceil (currentAttributes[Utilities.Attributes.Recovery])/10;
		float adjustment = Time.deltaTime * FATIGUESCALE * fatigueRate;

		//if (primaryEff == Utilities.Attributes.None || (currentSlot.tag == "WorkStation" && !currentSlot.GetComponent<WorkStationBehavior>().isActive))
		if (!isActive || primaryEff == Utilities.Attributes.None)
		{
			for(int c=4; c < 10; c++)
			{
				float currentAtt = currentAttributes[(Utilities.Attributes)c];

				currentAttributes[(Utilities.Attributes)c] = Mathf.Min (currentAtt + currentRecovery*adjustment, maxAttributes[(Utilities.Attributes)c]);
			}
		}
		else
		{
			for (int c=4; c < 10; c++)
			{
				//fatigue stats
				if ((Utilities.Attributes)c == primaryEff || (Utilities.Attributes)c == primaryQual)
				{
					currentAttributes[(Utilities.Attributes)c] = Mathf.Max (currentAttributes[(Utilities.Attributes)c] - adjustment, 0);
				}
				//recovery stats
				else if (((Utilities.Attributes)c != secondaryEff || (Utilities.Attributes)c != secondaryQual))
				{
					float currentAtt = currentAttributes[(Utilities.Attributes)c];

					currentAttributes[(Utilities.Attributes)c] = Mathf.Min (currentAtt + currentRecovery*adjustment, maxAttributes[(Utilities.Attributes)c]);
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
				currentAttributes[Utilities.Attributes.Recovery] = Mathf.Max (currentAttributes[Utilities.Attributes.Recovery] - Time.deltaTime*HUNGERSCALE*3, 1);
			}
			else
			{
				currentAttributes[Utilities.Attributes.Recovery] = Mathf.Max (currentAttributes[Utilities.Attributes.Recovery] - Time.deltaTime*HUNGERSCALE, 1);
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

		if (currentProspect.tag == "FarmPlot" && currentProspect.GetComponent<FarmPlot_Cultivation>().IsFull())
		{
			currentProspect = currentSlot;

		}

		if (currentProspect.tag == "WorkStation" && currentProspect.GetComponent<WorkStationBehavior>().IsFull())
		{
			currentProspect = currentSlot;
		}

		if (currentProspect.tag == "FarmPlot")
		{
			FarmPlot_Cultivation currentScript = currentProspect.GetComponent<FarmPlot_Cultivation>();
			currentScript.Assign (this.gameObject);
			SetTaskAttributes (currentScript.primaryEff, currentScript.secondaryEff, currentScript.primaryQual, currentScript.secondaryQual, currentScript.fatigueRate);

			if (primaryEff == Utilities.Attributes.None)
				Deactivate ();
			else
				Activate ();

			currentSlot = currentProspect;

		}
		else if (currentProspect.tag == "Pad")
		{
			PadBehavior currentScript = currentProspect.GetComponent<PadBehavior>();
			currentScript.Assign (this.gameObject);
			SetTaskAttributes (currentScript.primaryEff, currentScript.secondaryEff, currentScript.primaryQual, currentScript.secondaryQual, currentScript.fatigueRate);

			//Deactivate ();

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

		if (currentSlot.tag == "FarmPlot")
		{
			currentSlot.GetComponent<FarmPlot_Cultivation>().Unassign (this.gameObject);
		}
		else if (currentSlot.tag == "Pad")
		{
			currentSlot.GetComponent<PadBehavior>().Unassign (this.gameObject);
		}
		else if (currentSlot.tag == "WorkStation")
		{
			currentSlot.GetComponent<WorkStationBehavior>().Release ();
		}
		else
		{
			print ("ERROR IN CITIZENBEHAVIOR UNASSIGN - TAG OF CURRENT SLOT NOT HANDLED");
		}
	}



	//+++++++Task Functions++++++//


	public void Feed(float amount, CropsAndBuffs.Buff buff_in)
	{
		//print ("old amount: " + currentAttributes[Utilities.Attributes.Recovery]);
		
		currentAttributes[Utilities.Attributes.Recovery] = Mathf.Min (currentAttributes[Utilities.Attributes.Recovery]+amount, 10);
		currentBuff = buff_in;
		
		if (currentBuff.buffType != Utilities.BuffTypes.None)
		{
			buffIndicator.SetActive (true);
			buffDurationLeft = currentBuff.duration;
			//print (currentBuff.name + " duration: " + currentBuff.duration + " value: " + currentBuff.value);
		}
		else
		{
			//print ("NO BUFF");
		}
		//print ("new amount: " + currentAttributes[Utilities.Attributes.Recovery]);
	}

	public void SetTaskAttributes(Utilities.Attributes pEff_in, Utilities.Attributes sEff_in, Utilities.Attributes pQual_in, Utilities.Attributes sQual_in, float fatigue_in)
	{

		primaryEff = pEff_in;
		secondaryEff = sEff_in;
		primaryQual = pQual_in;
		secondaryQual = sQual_in;

		fatigueRate = fatigue_in;
	
	}

	public void IdleAttributes()
	{
		primaryEff = Utilities.Attributes.None;
		secondaryEff = Utilities.Attributes.None;
		primaryQual = Utilities.Attributes.None;
		secondaryQual = Utilities.Attributes.None;
		
		fatigueRate = 1;
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

		if (selectedTask.tag == "FarmPlot")
		{
			FarmPlot_Cultivation taskScript = selectedTask.GetComponent<FarmPlot_Cultivation>();

			if (taskScript.primaryEff == Utilities.Attributes.None)
				return -1;


			primEff = Mathf.Ceil (currentAttributes[taskScript.primaryEff]);
			secEff = Mathf.Ceil (currentAttributes[taskScript.secondaryEff]);
			primQual = Mathf.Ceil (currentAttributes[taskScript.primaryQual]);
			secQual = Mathf.Ceil (currentAttributes[taskScript.secondaryQual]);
		}
		else if (selectedTask.tag == "WorkStation")
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
		else if (collision_in.tag == "FarmPlot")
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
		else if (collision_in.tag == "FarmPlot")
		{
			currentProspect = null;
		}
		else if (collision_in.tag == "WorkStation")
		{
			currentProspect = null;
		}
	}





	//+++++++Debug, Setup, Misc++++++//




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
	}

	void GetSelectedTask()
	{
		selectedTask = gameController.GetComponent<GameController_Script>().selectedTask;
	}

    //update selected citizen 
    void UpdateCurrentCitizenAttributeValues ()
    {
        //updating slider values.
        GameObject.Find("Health_slider").GetComponent<Slider>().value = currentAttributes[Utilities.Attributes.Health];
        GameObject.Find("Strength_slider").GetComponent<Slider>().value = currentAttributes[Utilities.Attributes.Strength];
        GameObject.Find("Dexterity_slider").GetComponent<Slider>().value = currentAttributes[Utilities.Attributes.Dexterity];
        GameObject.Find("Endurance_slider").GetComponent<Slider>().value = currentAttributes[Utilities.Attributes.Endurance];
        GameObject.Find("Acumen_slider").GetComponent<Slider>().value = currentAttributes[Utilities.Attributes.Acumen];
        GameObject.Find("Focus_slider").GetComponent<Slider>().value = currentAttributes[Utilities.Attributes.Focus];
        GameObject.Find("Perception_slider").GetComponent<Slider>().value = currentAttributes[Utilities.Attributes.Perception];
        GameObject.Find("Recovery_slider").GetComponent<Slider>().value = currentAttributes[Utilities.Attributes.Recovery];
        GameObject.Find("Happiness_slider").GetComponent<Slider>().value = currentAttributes[Utilities.Attributes.Happiness];
        //print selectedCitizen Attibute maxValue next to slider
        GameObject.Find("Health_max_value").GetComponent<Text>().text = maxAttributes[Utilities.Attributes.Health].ToString();
        GameObject.Find("Strength_max_value").GetComponent<Text>().text = maxAttributes[Utilities.Attributes.Strength].ToString();
        GameObject.Find("Dexterity_max_value").GetComponent<Text>().text = maxAttributes[Utilities.Attributes.Dexterity].ToString();
        GameObject.Find("Endurance_max_value").GetComponent<Text>().text = maxAttributes[Utilities.Attributes.Endurance].ToString();
        GameObject.Find("Acumen_max_value").GetComponent<Text>().text = maxAttributes[Utilities.Attributes.Acumen].ToString();
        GameObject.Find("Focus_max_value").GetComponent<Text>().text = maxAttributes[Utilities.Attributes.Focus].ToString();
        GameObject.Find("Perception_max_value").GetComponent<Text>().text = maxAttributes[Utilities.Attributes.Perception].ToString();
        GameObject.Find("Recovery_max_value").GetComponent<Text>().text = maxAttributes[Utilities.Attributes.Recovery].ToString();
        GameObject.Find("Happiness_max_value").GetComponent<Text>().text = maxAttributes[Utilities.Attributes.Happiness].ToString();
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

	public void Unbuff ()
	{
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
