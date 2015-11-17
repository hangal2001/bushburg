using UnityEngine;
using System.Collections;
using System;

public class CurrentTaskUI_Script : MonoBehaviour {

    bool isActive;

    GameController_Script gameController;
    GameObject selectedTask;

    WorkStationBehavior workTask;

    TextMesh[] texts;

    // Use this for initialization
    void Start ()
    {
        Deactivate();

        gameController = GameObject.Find("GameController").GetComponent<GameController_Script>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (gameController.selectedTask == null && isActive)
        {
            Deactivate();
        }

        if (!isActive && gameController.selectedTask != null)
        {
            Activate();
            SetTask();
        }

        if (isActive)
        {
            if (selectedTask != gameController.selectedTask)
            {
                SetTask();
            }
        }

        UpdateMetrics();
	}

    void UpdateMetrics()
    {
        if (workTask != null)
        {
            if (workTask.stationType == Utilities.WorkStations.Table)
            {
                texts[6].text = "";
                texts[7].text = "";

                if (workTask.isActive)
                    texts[8].text = Mathf.Round(workTask.timeToProduce / workTask.ConvertedTimeModifier()).ToString();
                else
                    texts[8].text = "";
            }
            else
            {
                texts[6].text = Mathf.Round(workTask.timeModifier * 100).ToString();
                texts[7].text = Mathf.Round(workTask.GetQuality() * 100).ToString();

                if (workTask.isActive)
                    texts[8].text = Mathf.Round(workTask.timeToProduce / workTask.ConvertedTimeModifier()).ToString();
                else
                    texts[8].text = "";
            }
        }

       
    }

    public void SetTask()
    {
        selectedTask = gameController.selectedTask;
        workTask = selectedTask.GetComponent<WorkStationBehavior>();
        Utilities.SetCropTexture(transform.GetChild(4).gameObject, workTask.cropType);

        texts = transform.GetChild(0).GetComponentsInChildren<TextMesh>();

        if (workTask.primaryEff == Utilities.Attributes.None)
        {
            foreach (TextMesh text in texts)
                text.text = "";
        }
        else
        {
            texts[0].text = workTask.primaryEff.ToString();
            texts[1].text = workTask.primaryQual.ToString();
            texts[2].text = workTask.secondaryEff.ToString();
            texts[3].text = workTask.secondaryQual.ToString();

            for (int c = 4; c < 7; c++)
                if (workTask.primaryEff != (Utilities.Attributes)c && workTask.secondaryEff != (Utilities.Attributes)c)
                    texts[4].text = ((Utilities.Attributes)c).ToString();

            for (int c = 7; c < 10; c++)
                if (workTask.primaryQual != (Utilities.Attributes)c && workTask.secondaryQual != (Utilities.Attributes)c)
                    texts[5].text = ((Utilities.Attributes)c).ToString();
        }
        

        
    }

    void Activate()
    {
        isActive = true;

        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
    }

    void Deactivate()
    {
        isActive = false;

        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
    }
}
