using UnityEngine;
using System.Collections;

public class FloatingTextBehavior : MonoBehaviour 
{
	float countdown;
	TextMesh text;
	Color textColor;

	// Use this for initialization
	void Awake () 
	{
		text = GetComponent<TextMesh>();
		textColor = Color.white;
		text.text = "";
		countdown = 1f;
	}

	public void CreateFloatingText(string text_in, Utilities.TextTypes type_in)
	{
		if (type_in == Utilities.TextTypes.Positive)
			textColor = Color.green;
		else if (type_in == Utilities.TextTypes.Negative)
			textColor = Color.red;
		else if (type_in == Utilities.TextTypes.Dbug)
			textColor = Color.yellow;

		text.text = text_in;
		text.color = textColor;

		countdown = 2f;
	}

	// Update is called once per frame
	void Update () 
	{
		countdown -= Time.deltaTime;
		transform.position += new Vector3(0, 0, Time.deltaTime);

		if (countdown < 0)
			Fade ();
	}

	void Fade()
	{
		textColor.a -= Time.deltaTime*2;
		text.color = textColor;

		if (text.color.a < 0)
			Destroy (gameObject);
	}
}
