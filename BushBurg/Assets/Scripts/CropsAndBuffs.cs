using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using attrib = Utilities.Attributes;//less typing..

public class CropsAndBuffs : MonoBehaviour 
{
	public struct Crop
	{
		public string name;

		public Utilities.Attributes primaryEff;
		public Utilities.Attributes secondaryEff;
		public Utilities.Attributes primaryQual;
		public Utilities.Attributes secondaryQual;
		public float fatigueRate;		//1 is baseline, scale set in citizenbehavior atm
		public float timeToProduce; 	//in seconds
		public float baseRecovery;
		public int baseValue;
	}


	public struct Buff
	{
		public string name;

		public Utilities.BuffTypes buffType;
		public Utilities.Attributes attribute;
		public float value;
		public float duration;
	}

	public static Dictionary<Utilities.CropTypes, Crop> cropList;
	public static Dictionary<Utilities.CropTypes, Buff> buffList;

	/*Empty Buff
		newBuff.name = "";
		newBuff.buffType = Utilities.BuffTypes.;
		newBuff.attribute = attrib.;
		newBuff.value = ;
		newBuff.duration = ;
		buffList.Add(Utilities.CropTypes., newBuff);
	*/
	public static void GenerateBuffList()
	{
		buffList = new Dictionary<Utilities.CropTypes, Buff>();

		Buff newBuff;

		newBuff.name = "Yam";
		newBuff.buffType = Utilities.BuffTypes.AttributeScalar;
		newBuff.attribute = Utilities.Attributes.Perception;
		newBuff.value = 1;
		newBuff.duration = 60;
		buffList.Add (Utilities.CropTypes.Yam, newBuff);

		newBuff.name = "Corn";
		newBuff.buffType = Utilities.BuffTypes.Recovery;
		newBuff.attribute = Utilities.Attributes.Endurance;
		newBuff.value = 50;
		newBuff.duration = 60;
		buffList.Add(Utilities.CropTypes.Corn, newBuff);

		newBuff.name = "Collard Greens";
		newBuff.buffType = Utilities.BuffTypes.Drain;
		newBuff.attribute = Utilities.Attributes.Dexterity;
		newBuff.value = -20;
		newBuff.duration = 60;
		buffList.Add(Utilities.CropTypes.CollardGreens, newBuff);
	}

	public static void GenerateCropList()
	{
		cropList = new Dictionary<Utilities.CropTypes, Crop>();

		Crop newCrop;
		newCrop.name = "Yam";
		newCrop.primaryEff = attrib.Strength;
		newCrop.secondaryEff = attrib.Endurance;
		newCrop.primaryQual = attrib.Focus;
		newCrop.secondaryQual = attrib.Acumen;
		newCrop.fatigueRate = 1;
		newCrop.timeToProduce = 20;
		newCrop.baseRecovery = .5f;
		newCrop.baseValue = 10;
		cropList.Add (Utilities.CropTypes.Yam, newCrop);

		newCrop.name = "Corn";
		newCrop.primaryEff = attrib.Dexterity;
		newCrop.secondaryEff = attrib.Strength;
		newCrop.primaryQual = attrib.Perception;
		newCrop.secondaryQual = attrib.Acumen;
		newCrop.fatigueRate = 1.25f;
		newCrop.timeToProduce = 25;
		newCrop.baseRecovery = 1;
		newCrop.baseValue = 16;
		cropList.Add (Utilities.CropTypes.Corn, newCrop);

		newCrop.name = "Collard Greens";
		newCrop.primaryEff = attrib.Endurance;
		newCrop.secondaryEff = attrib.Dexterity;
		newCrop.primaryQual = attrib.Perception;
		newCrop.secondaryQual = attrib.Focus;
		newCrop.fatigueRate = 1;
		newCrop.timeToProduce = 15;
		newCrop.baseRecovery = .5f;
		newCrop.baseValue = 6;
		cropList.Add (Utilities.CropTypes.CollardGreens, newCrop);

		newCrop.name = "Okra";
		newCrop.primaryEff = attrib.Endurance;
		newCrop.secondaryEff = attrib.Strength;
		newCrop.primaryQual = attrib.Acumen;
		newCrop.secondaryQual = attrib.Focus;
		newCrop.fatigueRate = 1.75f;
		newCrop.timeToProduce = 30;
		newCrop.baseRecovery = .5f;
		newCrop.baseValue = 24;
		cropList.Add (Utilities.CropTypes.Okra, newCrop);

		newCrop.name = "Pumpkin";
		newCrop.primaryEff = attrib.Strength;
		newCrop.secondaryEff = attrib.Dexterity;
		newCrop.primaryQual = attrib.Perception;
		newCrop.secondaryQual = attrib.Acumen;
		newCrop.fatigueRate = 1;
		newCrop.timeToProduce = 60;
		newCrop.baseRecovery = 2.25f;
		newCrop.baseValue = 20;
		cropList.Add (Utilities.CropTypes.Pumpkin, newCrop);

		newCrop.name = "Agave";
		newCrop.primaryEff = attrib.Dexterity;
		newCrop.secondaryEff = attrib.Strength;
		newCrop.primaryQual = attrib.Focus;
		newCrop.secondaryQual = attrib.Perception;
		newCrop.fatigueRate = 3;
		newCrop.timeToProduce = 40;
		newCrop.baseRecovery = 1;
		newCrop.baseValue = 42;
		cropList.Add (Utilities.CropTypes.Agave, newCrop);

		newCrop.name = "Cabbage";
		newCrop.primaryEff = attrib.Strength;
		newCrop.secondaryEff = attrib.Endurance;
		newCrop.primaryQual = attrib.Perception;
		newCrop.secondaryQual = attrib.Focus;
		newCrop.fatigueRate = .5f;
		newCrop.timeToProduce = 25;
		newCrop.baseRecovery = .6f;
		newCrop.baseValue = 10;
		cropList.Add (Utilities.CropTypes.Cabbage, newCrop);

		newCrop.name = "Pepper";
		newCrop.primaryEff = attrib.Strength;
		newCrop.secondaryEff = attrib.Dexterity;
		newCrop.primaryQual = attrib.Focus;
		newCrop.secondaryQual = attrib.Acumen;
		newCrop.fatigueRate = 1.5f;
		newCrop.timeToProduce = 55;
		newCrop.baseRecovery = 1.25f;
		newCrop.baseValue = 40;
		cropList.Add (Utilities.CropTypes.Pepper, newCrop);

		newCrop.name = "Egg";
		newCrop.primaryEff = attrib.Dexterity;
		newCrop.secondaryEff = attrib.Endurance;
		newCrop.primaryQual = attrib.Acumen;
		newCrop.secondaryQual = attrib.Perception;
		newCrop.fatigueRate = 1.5f;
		newCrop.timeToProduce = 75;
		newCrop.baseRecovery = 3;
		newCrop.baseValue = 60;
		cropList.Add (Utilities.CropTypes.Egg, newCrop);

		newCrop.name = "Squash";
		newCrop.primaryEff = attrib.Endurance;
		newCrop.secondaryEff = attrib.Dexterity;
		newCrop.primaryQual = attrib.Perception;
		newCrop.secondaryQual = attrib.Acumen;
		newCrop.fatigueRate = 1;
		newCrop.timeToProduce = 45;
		newCrop.baseRecovery = 1.75f;
		newCrop.baseValue = 20;
		cropList.Add (Utilities.CropTypes.Squash, newCrop);

		newCrop.name = "Banana";
		newCrop.primaryEff = attrib.Endurance;
		newCrop.secondaryEff = attrib.Strength;
		newCrop.primaryQual = attrib.Acumen;
		newCrop.secondaryQual = attrib.Perception;
		newCrop.fatigueRate = 2;
		newCrop.timeToProduce = 30;
		newCrop.baseRecovery = 1.6f;
		newCrop.baseValue = 22;
		cropList.Add (Utilities.CropTypes.Banana, newCrop);

		newCrop.name = "Honey";
		newCrop.primaryEff = attrib.Dexterity;
		newCrop.secondaryEff = attrib.Strength;
		newCrop.primaryQual = attrib.Acumen;
		newCrop.secondaryQual = attrib.Focus;
		newCrop.fatigueRate = 2.25f;
		newCrop.timeToProduce = 75;
		newCrop.baseRecovery = 1;
		newCrop.baseValue = 90;
		cropList.Add (Utilities.CropTypes.Honey, newCrop);

		newCrop.name = "Lettuce";
		newCrop.primaryEff = attrib.Strength;
		newCrop.secondaryEff = attrib.Endurance;
		newCrop.primaryQual = attrib.Focus;
		newCrop.secondaryQual = attrib.Perception;
		newCrop.fatigueRate = .6f;
		newCrop.timeToProduce = 45;
		newCrop.baseRecovery = 1.25f;
		newCrop.baseValue = 20;
		cropList.Add (Utilities.CropTypes.Lettuce, newCrop);

		newCrop.name = "Bacon";
		newCrop.primaryEff = attrib.Strength;
		newCrop.secondaryEff = attrib.Dexterity;
		newCrop.primaryQual = attrib.Acumen;
		newCrop.secondaryQual = attrib.Focus;
		newCrop.fatigueRate = 3.75f;
		newCrop.timeToProduce = 55;
		newCrop.baseRecovery = 3.25f;
		newCrop.baseValue = 70;
		cropList.Add (Utilities.CropTypes.Bacon, newCrop);

		newCrop.name = "Pineapple";
		newCrop.primaryEff = attrib.Dexterity;
		newCrop.secondaryEff = attrib.Endurance;
		newCrop.primaryQual = attrib.Perception;
		newCrop.secondaryQual = attrib.Focus;
		newCrop.fatigueRate = 1.75f;
		newCrop.timeToProduce = 65;
		newCrop.baseRecovery = 1.75f;
		newCrop.baseValue = 60;
		cropList.Add (Utilities.CropTypes.Pineapple, newCrop);

		newCrop.name = "Goat Milk";
		newCrop.primaryEff = attrib.Endurance;
		newCrop.secondaryEff = attrib.Strength;
		newCrop.primaryQual = attrib.Focus;
		newCrop.secondaryQual = attrib.Acumen;
		newCrop.fatigueRate = 1;
		newCrop.timeToProduce = 80;
		newCrop.baseRecovery = 3;
		newCrop.baseValue = 60;
		cropList.Add (Utilities.CropTypes.GoatMilk, newCrop);

		newCrop.name = "Tomato";
		newCrop.primaryEff = attrib.Endurance;
		newCrop.secondaryEff = attrib.Strength;
		newCrop.primaryQual = attrib.Perception;
		newCrop.secondaryQual = attrib.Acumen;
		newCrop.fatigueRate = 1.25f;
		newCrop.timeToProduce = 80;
		newCrop.baseRecovery = 2.25f;
		newCrop.baseValue = 70;
		cropList.Add (Utilities.CropTypes.Tomato, newCrop);

		newCrop.name = "Beef";
		newCrop.primaryEff = attrib.Strength;
		newCrop.secondaryEff = attrib.Endurance;
		newCrop.primaryQual = attrib.Acumen;
		newCrop.secondaryQual = attrib.Focus;
		newCrop.fatigueRate = 1.55f;
		newCrop.timeToProduce = 115;
		newCrop.baseRecovery = 5;
		newCrop.baseValue = 110;
		cropList.Add (Utilities.CropTypes.Beef, newCrop);

		newCrop.name = "Sugarcane";
		newCrop.primaryEff = attrib.Dexterity;
		newCrop.secondaryEff = attrib.Strength;
		newCrop.primaryQual = attrib.Perception;
		newCrop.secondaryQual = attrib.Focus;
		newCrop.fatigueRate = 3.25f;
		newCrop.timeToProduce = 45;
		newCrop.baseRecovery = .75f;
		newCrop.baseValue = 70;
		cropList.Add (Utilities.CropTypes.Sugar, newCrop);

		newCrop.name = "Peanut";
		newCrop.primaryEff = attrib.Endurance;
		newCrop.secondaryEff = attrib.Dexterity;
		newCrop.primaryQual = attrib.Focus;
		newCrop.secondaryQual = attrib.Perception;
		newCrop.fatigueRate = .5f;
		newCrop.timeToProduce = 150;
		newCrop.baseRecovery = 1.25f;
		newCrop.baseValue = 90;
		cropList.Add (Utilities.CropTypes.Peanut, newCrop);


	}
	
}
