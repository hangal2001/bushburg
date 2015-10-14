using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Stores functions not associated with any particular object
 * This class should not contain variables and should not
 * instantiate any objects*/

public class Utilities : MonoBehaviour 
{
	public static float TIMESCALE = 3f;				//increase time scale to make game go faster
	public static float MAXTIMEMODIFIER = .8f;		//cap on how much stats can affect production time, must be less than 1
	public static float COOKTIMERATIO = 2f;			//multiplier for time costs to cook a specific crop
	public static float TRADETIMERATIO = 1f;		//multiplier for time costs to trade a specific crop

	/*NOTES ON ENUM FUNCTIONALITY
	 * Enum is kind of like an array where every index has a title
	 * 
	 * you can reference it by title like:
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

	public enum WorkStations : int {Table = 0, Cauldron, Depot};

	public enum ItemTypes : int {Seed = 0, Crop, Meal, TradeCrop, TradeMeal};

	public enum BuffTypes : int {None = 0, Recovery, Drain, AttributeScalar,
								AttributeLockPositive, AttributeLockNegative};

	public static void SetCropTexture(GameObject object_in, CropTypes crop_in)
	{
		//string newTextureName = crop_in.ToString;
		Texture newTexture = Resources.Load ("Crops/" + crop_in.ToString()) as Texture;
		object_in.GetComponent<Renderer>().material.mainTexture = newTexture;
	}
	
}
