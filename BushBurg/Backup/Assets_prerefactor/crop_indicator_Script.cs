using UnityEngine;
using System.Collections;

public class crop_indicator_Script : MonoBehaviour {

    public Utilities.CropTypes cropType { get; private set; }
    
    // Use this for initialization
	void Awake () {
       // selectedCrop = GameObject.Find("DraggableCrop").GetComponent<CropBehavior>();
	}
	
	// Update is called once per frame
	void Update () {
       // Utilities.SetCropTexture(this.gameObject.transform.GetChild(5).gameObject, selectedCrop);
	}
}
