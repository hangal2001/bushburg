using UnityEngine;
using System.Collections;

public class CurrentBuffUI_Script : MonoBehaviour {
    
    bool isActive;

    GameController_Script gameController;
    GameObject selectedBuff;

    CropsAndBuffs.Buff currentBuff;

    // Use this for initialization
    void Start ()
    {
        Deactivate();

        gameController = GameObject.Find("GameController").GetComponent<GameController_Script>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (gameController.selectedCitizen == null && isActive)
        {
            Deactivate();
        }

        if (!isActive && gameController.selectedCitizen != null)
        {
            Activate();
            SetBuff();
        }

        if (isActive)
        {
            if (selectedBuff != gameController.selectedCitizen)
            {
                SetBuff();
            }
        }

        UpdateMetrics();
    }

    public void SetBuff()
    {
        if (gameController.selectedCitizen != null)
        {
            selectedBuff = gameController.selectedCitizen;
            currentBuff = selectedBuff.GetComponent<CitizenBehavior>().currentBuff;

            Utilities.SetCropTexture(transform.GetChild(1).gameObject, selectedBuff.GetComponent< CitizenBehavior>().buffCropType);
            //currentBuff = selectedBuff.GetComponent<CitizenBehavior>().currentBuff;
        }
    }
    
    void UpdateMetrics()
    {
        if (isActive && selectedBuff.tag == "Citizen")
        {
            currentBuff = selectedBuff.GetComponent<CitizenBehavior>().currentBuff;
            transform.GetChild(2).GetComponent<TextMesh>().text = Utilities.GenerateBuffText(currentBuff);
        }
    }

    void Activate()
    {
        isActive = true;

        transform.GetChild(2).gameObject.SetActive(true);
    }

    void Deactivate()
    {
        isActive = false;

        transform.GetChild(2).gameObject.SetActive(false);
    }
}
