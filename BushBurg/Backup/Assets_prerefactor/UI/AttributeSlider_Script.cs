using UnityEngine;
using System.Collections;

public class AttributeSlider_Script : MonoBehaviour
{
    GameController_Script gameController;

    GameObject selectedCitizen;
    CitizenBehavior citizenScript;

    public Utilities.Attributes attribute;

    int currentValue;
    int maxValue;

    Renderer[] redSegments;
    Renderer[] greenSegments;
    Renderer icon;

    TextMesh title;

    Texture drainingTexture;
    Texture lockedTexture;
    Texture recoveringTexture;

	// Use this for initialization
	void Awake ()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController_Script>();

        title = transform.GetChild(0).GetComponent<TextMesh>();
        title.text = attribute.ToString();
        icon = transform.GetChild(3).GetComponent<Renderer>();
        icon.enabled = false;

        drainingTexture = Resources.Load("Attribute Icons/Draining") as Texture;
        lockedTexture = Resources.Load("Attribute Icons/Locked") as Texture;
        recoveringTexture = Resources.Load("Attribute Icons/Recovering") as Texture;

        //title = transform.GetChild(0).GetComponent<TextMesh>().text;

        redSegments = transform.GetChild(1).GetComponentsInChildren<Renderer>();
        greenSegments = transform.GetChild(2).GetComponentsInChildren<Renderer>();

        currentValue = 0;
        maxValue = 0;

        ClearSegments();
	}
	
	// Update is called once per frame
	void Update ()
    {
        /*
        if (selectedCitizen == null)
        {
            if (gameController.selectedCitizen != null)
            {
                selectedCitizen = gameController.selectedCitizen;
                UpdateSelection();
            }
        }
        else if (selectedCitizen != gameController.selectedCitizen)
        {
            selectedCitizen = gameController.selectedCitizen;
            UpdateSelection();
        }*/

        if (selectedCitizen != gameController.selectedCitizen)
        {
            selectedCitizen = gameController.selectedCitizen;
            citizenScript = selectedCitizen.GetComponent<CitizenBehavior>();

            if (attribute != Utilities.Attributes.Health && attribute != Utilities.Attributes.Happiness && attribute != Utilities.Attributes.Recovery)
                icon.enabled = true;

            UpdateSelection();
        }

        if (selectedCitizen != null)
        {

            UpdateIcon();

            if (currentValue != citizenScript.currentAttributes[attribute])
            {
                UpdateSelection();
            }
        }
       
        
    }

    void ClearSegments()
    {
        foreach (Renderer r in redSegments)
            r.enabled = false;

        foreach (Renderer r in greenSegments)
            r.enabled = false;
    }

    void UpdateSelection()
    {
        maxValue = Mathf.CeilToInt(citizenScript.maxAttributes[attribute]);
        currentValue = Mathf.CeilToInt(citizenScript.currentAttributes[attribute]);

        title.text = attribute.ToString() + ": " + currentValue + "/" + maxValue;

        for (int c=0; c < 12; c++)
        {
            if (c <= maxValue-1)
            {
                redSegments[c].enabled = true;
            }
            else
            {
                redSegments[c].enabled = false;
            }
            

            if (c <= currentValue-1)
            {
                greenSegments[c].enabled = true;
            }
            else
            {
                greenSegments[c].enabled = false;
            }
        }
    }

    void UpdateIcon()
    {
        if (selectedCitizen != null)
        {
            if (!citizenScript.isActive || citizenScript.primaryEff == Utilities.Attributes.None)
            {
                icon.material.mainTexture = recoveringTexture;
            }
            else
            {
                if (attribute == citizenScript.primaryEff || attribute == citizenScript.primaryQual)
                {
                    icon.material.mainTexture = drainingTexture;
                }
                else if (attribute == citizenScript.secondaryEff || attribute == citizenScript.secondaryQual)
                {
                    icon.material.mainTexture = lockedTexture;
                }
                else
                {
                    icon.material.mainTexture = recoveringTexture;
                }
            }
        }

    }

}
