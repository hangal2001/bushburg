using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Progress : MonoBehaviour
{
    public CitizenBehavior citizenscriptvariable;
    //public CitizenManager_Script citizenManager;

    public Utilities.Attributes maxAttributes;
    public Utilities.CropTypes cropType;

    void Start()

    {
        //CitizenBehavior citizenscriptvariable = (CitizenBehavior) maxAttributes;
        //citizenscriptvariable = GameObject.Find("CitizenBehavior").GetComponent<CitizenBehavior>();
        //citizenManager = GameObject.Find("CitizenManager").GetComponent<CitizenManager_Script>();
    }


    void Awake()

    {
        CurrentCitizenAttributeUpdate();
    }


    void CurrentCitizenAttributeUpdate()

    {

       // print(maxAttributes[Utilities.Attributes.Health] + " updated on progress.cs");
        
        //print(currentValue = [Utilities.Attributes.Health]);

    }

}