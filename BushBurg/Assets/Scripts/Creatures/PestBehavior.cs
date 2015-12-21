using UnityEngine;
using System.Collections;

public class PestBehavior : MonoBehaviour 
{
	//static float BASECHANCE = .01f;

	public float damage {get; private set;}
	bool eradicating;
	//float eradicationTimer;
	//float eradicationEfficiency;
	ParticleSystem activeParticles;
	ParticleSystem eradicatingParticles;
	Renderer render;
	float revealedTime;
	CapsuleCollider collide;

	// Use this for initialization
	void Awake () 
	{
		damage = -2;
		eradicating = false;
		//eradicationTimer = 0;
		revealedTime = 0;

		activeParticles = transform.GetChild(0).GetComponent<ParticleSystem>();
		eradicatingParticles = transform.GetChild (1).GetComponent<ParticleSystem>();
		render = GetComponent<Renderer>();
		collide = GetComponent<CapsuleCollider>();

		Hide ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateMetrics();
	}

	void UpdateMetrics()
	{
		if (Input.GetKeyDown ("x"))
		{
			Reveal ();
		}
		if (eradicating)
		{

		}
		else if (render.isVisible)
		{
			revealedTime -= Time.deltaTime;
			if (revealedTime < 0)
				Hide ();
		}
	}

	public void Hide()
	{
		activeParticles.Clear ();
		activeParticles.Stop();
		eradicatingParticles.Clear ();
		eradicatingParticles.Stop();
		render.enabled = false;
		collide.enabled = false;

	}

	public void Reveal()
	{
		activeParticles.Play ();
		render.enabled = true;
		collide.enabled = true;

		revealedTime = 15*Utilities.TIMESCALE;
	}

	public void RotateCrop()
	{
		if (damage > 0 && !eradicating)
			damage-=3;
	}

	public void StartEradication()
	{
		eradicating = true;
		collide.enabled = false;
		//eradicationEfficiency = efficiency_in;
		activeParticles.Clear ();
		activeParticles.Stop ();
		eradicatingParticles.Play ();

	}

	public void CompleteEradication()
	{
		eradicating = false;
		Hide ();

		damage = -2;
	}

	public bool IsRevealed()
	{
		return render.isVisible;
	}

	public void UpdateDamage()
	{
		damage++;
	}

	public float ApplyDamage(float quality_in)
	{
		UpdateDamage ();

		if (damage > 0)
		{
			if (render.isVisible)
				Utilities.FloatText (transform.position + new Vector3(1.3f, 0, .5f), (Mathf.Min(100, Mathf.Round(.05f*damage*100))).ToString () + "% dmg!", Utilities.TextTypes.Negative);

			return Mathf.Max (0, quality_in*(1- 0.05f*damage));
		}
		else
		{
			return quality_in;
		}
	}
}
