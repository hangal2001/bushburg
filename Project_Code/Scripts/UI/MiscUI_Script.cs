using UnityEngine;
using System.Collections;

public class MiscUI_Script : MonoBehaviour
{
    GameController_Script gameController;

    TextMesh level;
    TextMesh currentMoney;
    TextMesh moneyToNext;
    TextMesh currentColorMode;

	// Use this for initialization
	void Start ()
    {
        gameController = Utilities.GetGameController();

        level = transform.GetChild(0).GetComponent<TextMesh>();
        currentMoney = transform.GetChild(1).GetComponent<TextMesh>();
        moneyToNext = transform.GetChild(2).GetComponent<TextMesh>();
        currentColorMode = transform.GetChild(3).GetComponent<TextMesh>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        level.text = "Level: " + gameController.level;
        currentMoney.text = "Money: " + gameController.currentMoney;
        moneyToNext.text = "Next level: " + gameController.baseLevelUp;
        currentColorMode.text = "Color Mode: " + gameController.colorMode.ToString();
	}
}
