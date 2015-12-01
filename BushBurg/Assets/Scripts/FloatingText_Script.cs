using UnityEngine;
using System.Collections;

public class FloatingText_Script : MonoBehaviour {

    public GameObject FloatingTextPrefab;
    public TextMesh floatingText;
    
    // Use this for initialization
    void Start()
    {
       
    }
    
	void Awake () {
        FloatingTextPrefab = GameObject.Find("FloatingText");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Create Floating text
    public void GenerateFloatingText(GameObject target, string content)
    {
        FloatingTextPrefab = GameObject.Find("FloatingText");
        GameObject SelectedCitizen = GameObject.Find("SelectedCitizen");
        GameObject myFloatingText = Instantiate(FloatingTextPrefab, new Vector3(SelectedCitizen.transform.position.x, SelectedCitizen.transform.position.y + 10, SelectedCitizen.transform.position.z), Quaternion.identity) as GameObject;
        myFloatingText.GetComponent<TextMesh>().text = content;
        Destroy(myFloatingText, 100);
	}
 
}
