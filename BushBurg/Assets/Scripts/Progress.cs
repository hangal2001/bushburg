using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Progress : MonoBehaviour
{

    Dictionary<Utilities.Attributes, int> maxAttributes;
    Dictionary<Utilities.Attributes, float> currentAttributes;

    
    void Start()

    {
        Awake();
    }


    void Awake()

    {
        maxAttributes = new Dictionary<Utilities.Attributes, int>();
        currentAttributes = new Dictionary<Utilities.Attributes, float>();
        CurrentCitizenAttributeUpdate();
    }


    void CurrentCitizenAttributeUpdate()

    {
        int currentValue = 1;
        print(maxAttributes.TryGetValue(Utilities.Attributes.Health, out currentValue));
        print(currentValue);

        //print(currentValue = [Utilities.Attributes.Health]);

    }

}