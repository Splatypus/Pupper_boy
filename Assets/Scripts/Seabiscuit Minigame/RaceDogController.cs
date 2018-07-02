using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceDogController : MonoBehaviour {

	[SerializeField] float switch_time;
	[SerializeField] AnimationCurve switch_curve;
	[SerializeField] float flicker_time;
	[SerializeField] float flicker_frequency;
	
	GameObject[] lanes;

	TrackController _track;

	int _lane_position, h;
	bool moving, movement_enabled; 

	void Awake()
	{
		_track = GameObject.Find("Track").GetComponent<TrackController>();
		lanes = _track.lanes;
	}

	void Start() 
	{
		_lane_position = 2;
		moving = false;
		movement_enabled = true;
	}
	
	void FixedUpdate()
	{
		h = (int)Input.GetAxisRaw("Horizontal");
		if (h != 0 && !moving && movement_enabled) 
		{
			moving = true;
			int starting_lane = _lane_position;
			_lane_position = Mathf.Clamp(_lane_position + h, 0, lanes.Length - 1);
			if (starting_lane != _lane_position)
			{
				StartCoroutine(SwitchLane(starting_lane));
			}
			else
			{
				moving = false;
			}
		} 
	}

	void OnTriggerEnter(Collider collider)
	{
		GameOver();
	}

	void GameOver()
	{
		movement_enabled = false;
		_track.SetSpeed(0f);
		StartCoroutine(Flicker(flicker_time, flicker_frequency));
	}

	private IEnumerator SwitchLane(int starting_lane)
	{
		float t = 0;
		while (t < switch_time)
		{
			transform.position = Vector3.Lerp(
				lanes[starting_lane].transform.position,
				lanes[_lane_position].transform.position,
				switch_curve.Evaluate(t / switch_time)
			);
			t += Time.deltaTime;
			yield return null;
		} 
		moving = false;
	}

	private IEnumerator Flicker(float flicker_time, float flicker_frequency)
	{
		float t = 0;
		while (t < flicker_time)
		{
			transform.GetChild(0).gameObject.SetActive(Mathf.PingPong(t, flicker_frequency) <= (flicker_frequency / 2f));
			t += Time.deltaTime;
			yield return null;
		}
	}

}
