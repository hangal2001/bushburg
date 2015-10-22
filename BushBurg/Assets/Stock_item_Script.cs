using UnityEngine;
using System.Collections;

public class Stock_item_Script : MonoBehaviour
{

    //how to declare a variable of an enum type
    //do this for all things except gameobject.find
    //strings are error prone, enums are very safe and predictable
    public Utilities.CropTypes cropType;

    //make variables of script types so that you don't have to type a bunch of GetComponents all the time
    public GameObject cropPrefab;
    GameController_Script gameController;
    CitizenBehavior currentCitizen;
    FarmPlot_Cultivation currentPlot;
    WorkStationBehavior currentTask;

    StorageBehavior storage;
    int numCrops;
    string temp_numCrops = "";
    //public Utilities.CropTypes cropType;
    public TextMesh[] subTexts;

    // Use this for initialization
    void Awake()
    {
        //these two will never change because there is only 1 instance of them
        gameController = GameObject.Find("GameController").GetComponent<GameController_Script>();
        storage = GameObject.Find("Storage").GetComponent<StorageBehavior>();

    }

    // Update is called once per frame
    void Update()
    {
        //not needed for storage
        //this function would be useful in a script for the current buff/task window objects
        //SetReferences();

        //you can access dictionaries like arrays, where the key is the index.
        //this particular value type is a list itself because of varying quality of ingredients
        //you just need the total #, which is accessed by .Count as part of the List<> library
        //attributes will not need a .Count because they are floats or ints
        numCrops = storage.crops[cropType].Count;
        print(numCrops);
        temp_numCrops = numCrops.ToString();
        subTexts[1].text = temp_numCrops;

    }

    public void SetCrop(Utilities.CropTypes crop_in)
    {
        cropType = crop_in;
    }


    public GameObject CreateDraggableCrop(Vector3 location_in)
    {
        GameObject newDraggableCrop = Instantiate(cropPrefab, location_in, Quaternion.identity) as GameObject;
        newDraggableCrop.GetComponent<CropBehavior>().CreateCrop(cropType, Utilities.ItemTypes.Seed, this.gameObject, 0, 0);

        return newDraggableCrop;
    }

    //this function will set the script references from the gamecontroller's selected items
    //doing this in update is technically a bad idea for performance reasons but it is not super likely
    //that it will matter for us
    public void SetReferences()
    {
        if (gameController.selectedTask.tag == "FarmPlot")
        {
            currentPlot = gameController.selectedTask.GetComponent<FarmPlot_Cultivation>();
        }
        else if (gameController.selectedTask.tag == "WorkStation")
        {
            currentTask = gameController.selectedTask.GetComponent<WorkStationBehavior>();
        }

        currentCitizen = gameController.selectedCitizen.GetComponent<CitizenBehavior>();
    }
}