using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackController : MonoBehaviour {

	[SerializeField] float difficulty;
	[SerializeField] float track_speed;
	[SerializeField] float track_length;
	[SerializeField] float spawn_distance;

	public GameObject[] lanes;

	[SerializeField] GameObject track_prefab;
	[SerializeField] GameObject[] obstacle_prefabs;

    void Start() {
        for (float i = 0f; i < spawn_distance; i += track_length) {
            InstantiateTrack(i, 0);
        }
    }

    void FixedUpdate() {
        MoveTrack(track_speed);
    }

    void InstantiateTrack(float distance, float difficulty) {
        GameObject track = Instantiate(
            track_prefab,
            new Vector3(0f, 0f, distance),
            Quaternion.identity,
            transform
        );

        if (difficulty > 0f) {
            InstantiateObstacle(Random.Range(1, 5), 0, track);
        }

    }

    //Instantiates an obsticle on the given lane, distance down from the start of that track section
    void InstantiateObstacle(int lane, float distance, GameObject track) {
        GameObject obstacle = Instantiate(
            obstacle_prefabs[0],
            track.transform,
            false
        );

        obstacle.transform.localPosition = new Vector3(
            lanes[lane - 1].transform.position.x, 0f, distance
        );
    }

    void MoveTrack(float speed) {
        float max_distance = 0;
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).position += Vector3.back * speed * Time.deltaTime;
            if (transform.GetChild(i).position.z > max_distance) {
                max_distance = transform.GetChild(i).position.z;
            }
            if (transform.GetChild(i).position.z <= -2.0f * track_length) {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        // Ensures that the track is contiguous... might swap this to recycle track parts rather than making and destroying
        if (max_distance < spawn_distance) {
            InstantiateTrack(max_distance + track_length, difficulty);
        }
    }

    public void SetSpeed(float speed) {
        track_speed = speed;
    }

}
