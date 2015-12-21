using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* Stores functions not associated with any particular object
 * This class should not contain variables and should not
 * instantiate any objects*/

public class Utilities : MonoBehaviour 
{
	public static float TIMESCALE = 1.5f;				//increase time scale to make game go faster
	public static float MAXTIMEMODIFIER = .8f;		//cap on how much stats can affect production time, must be less than 1
	public static float COOKTIMERATIO = 1.5f;		//multiplier for time costs to cook a specific crop
	public static float TRADETIMERATIO = 1f;		//multiplier for time costs to trade a specific crop
    public static float TRADEBONUSVALUESCALE = 1.5f;  //multiplier for value of sales based on quality
    public static float EATTIME = 7f;               //how long it takes to eat something
    public static float AUTOCOLLECTSCALE = .33f;    //proportion of autocollect to production time
    public static float FATIGUESCALE = (1f / 35f) * Utilities.TIMESCALE;//how many seconds to fatigue if fatigue rate is 1
                                                                        //a higher denominator means more seconds
    public static float HUNGERSCALE = (1f / 75f) * Utilities.TIMESCALE;//how many seconds for the recovery stat to drop by 1
                                                                       //higher denominator means more seconds
    public static float BUFFSCALE = 1.5f;   //flat scalar for all buff durations
    public static float IDLEBONUS = 2f;    //scalar for recovery time if idle
    public static float MEALBONUS = .25f;   //scalar for direct attribute recovery from eating
	
    /*NOTES ON ENUM FUNCTIONALITY
	 * Enum is kind of like an array where every index has a title
     * this will prevent errors and allow for autocomplete
	 * 
	 * you can reference enum by title like:
	 * 		Utilities.CropTypes.SweetPotato
	 * 
	 * or by index like:
	 * 		(Utilities.CropTypes)0
	 * 
	 * index is useful for loops and title is useful for 
	 * coding a specific reference, so enum is a nice way to have both
	 * I think enum class may have a foreach as well
	 */

    public enum CropTypes : int {None = 0, Yam, Corn, CollardGreens, Okra, Pumpkin,
							Agave, Cabbage, Pepper, Egg, Squash, Banana, Honey,
							Lettuce, Bacon, Pineapple, GoatMilk, Tomato, Beef, Sugar, Peanut};

	public enum ColorModes : int {CurrentFit = 0, BestFit};

	public enum Attributes : int {None = 0, Health, Happiness, Recovery, Strength, Dexterity,
								Endurance, Perception, Focus, Acumen};

	public enum WorkStations : int {Table = 0, Cauldron, Depot, FarmPlot, Forager};

	public enum ItemTypes : int {Seed = 0, Crop, Meal, TradeCrop, TradeMeal};

	public enum BuffTypes : int {None = 0, Recovery, Drain, AttributeScalar,
								AttributeLockPositive, AttributeLockNegative};

	public enum TextTypes : int {Neutral = 0, Positive, Negative, Dbug};

	public static void SetCropTexture(GameObject object_in, CropTypes crop_in)
	{
		//string newTextureName = crop_in.ToString;
		Texture newTexture = Resources.Load ("Crops/" + crop_in.ToString()) as Texture;
		object_in.GetComponent<Renderer>().material.mainTexture = newTexture;
	}

	public static void SetPestTexture(GameObject object_in)
	{
		//string newTextureName = crop_in.ToString;
		Texture newTexture = Resources.Load ("Creatures/pest") as Texture;
		object_in.GetComponent<Renderer>().material.mainTexture = newTexture;
	}

	public static void SetTexture(GameObject object_in, string filename_in)
	{
		//string newTextureName = crop_in.ToString;
		Texture newTexture = Resources.Load (filename_in) as Texture;
		object_in.GetComponent<Renderer>().material.mainTexture = newTexture;
	}

    public static GameController_Script GetGameController()
    {
        return GameObject.Find("GameController").GetComponent<GameController_Script>();
    }

    //This will generate the text needed for display depending on buff type
    public static string GenerateBuffText(CropsAndBuffs.Buff buff_in)
    {
        string buffText = "";

        if (buff_in.buffType == BuffTypes.None)
        {
            buffText = "None";
        }
        else if (buff_in.buffType == BuffTypes.Recovery)
        {
            buffText = buff_in.attribute.ToString() + " recovery up. \n";
        }
        else if (buff_in.buffType == BuffTypes.Drain)
        {
            buffText = buff_in.attribute.ToString() + " drain down. \n";
        }
        else if (buff_in.buffType == BuffTypes.AttributeScalar)
        {
            buffText = "Max " + buff_in.attribute.ToString() + " up. \n";
        }
        else if (buff_in.buffType == BuffTypes.AttributeLockPositive)
        {
            buffText = buff_in.attribute.ToString() + " stays above\n";
        }
        else if (buff_in.buffType == BuffTypes.AttributeLockNegative)
        {
			buffText = buff_in.attribute.ToString() + " stays below\n";
		}
		
		if (buff_in.buffType != BuffTypes.None)
        {
            //adding % sign if needed
            if (buff_in.buffType == BuffTypes.Recovery || buff_in.buffType == BuffTypes.Drain)
            {
                buffText += ("Value = " + Math.Round(buff_in.value, 2) + "%\nDuration = " + Mathf.Ceil(buff_in.duration / TIMESCALE));
            }
            else
            {
                buffText += ("Value = " + Math.Round(buff_in.value, 2) + "\nDuration = " + Mathf.Ceil(buff_in.duration / TIMESCALE));
            }
            
        }
        
        return buffText;

    }

	public static void FloatText(Vector3 loc_in, string text_in, TextTypes type_in)
	{
		GameObject newText = Instantiate (Resources.Load ("FloatingText"), loc_in, Quaternion.Euler (90,0,0)) as GameObject;
		newText.GetComponent<FloatingTextBehavior>().CreateFloatingText (text_in, type_in);

	}

}
