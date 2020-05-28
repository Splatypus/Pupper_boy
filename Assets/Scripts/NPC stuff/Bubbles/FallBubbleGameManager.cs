using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FallBubbleGameManager : MonoBehaviour
{
    [Header("Setup")]
    public GameObject[] objectives;
    public GameObject canvas;
    public TMP_Text canvasTimeField; //reference to the text box that displays time. Must be set
    public TMP_Text scoreText;
    public TMP_Text countdownText;
    public GameObject startLocation;

    [Header("Inital Constraints")]
    public float timeLimit;
    public float countdownTimer = 3.0f;
    public float fadeDuration = 2.0f;

    //private
    float startTime;
    bool isPlaying = false;
    int totalObjectives;
    int completedObjectives;
    System.Action<bool> OnWin;


    public void Init(System.Action<bool> EndCallback) {
        OnWin = EndCallback;
        
        //make sure objectives start disabled
        foreach (GameObject o in objectives) {
            o.SetActive(false);
        }
    }

    public void Start()
    {
        totalObjectives = objectives.Length;  
    }
    public void Update()
    {
        if (isPlaying)
        {
            if (Time.time > startTime + timeLimit)
            {
                EndGame(false);
            }
            else
            {
                canvasTimeField.text = ((int)(startTime + timeLimit - Time.time)).ToString();
            }
        }
    }

    public void CompleteObjective(FallObjective objective) {
        //Disable objective
        objective.gameObject.SetActive(false);
        //increment objectives reached
        completedObjectives++;
        scoreText.text = completedObjectives.ToString();
        //if we have them all, we win
        if (completedObjectives >= totalObjectives) {
            EndGame(true);
        }
    }

    public void StartGame() {
        //Get player reference
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //fade out camera, then setup game
        ScreenEffects.GetInstance().FadeToBlack(fadeDuration, () => {
            //setup
            canvas.SetActive(true);
            canvasTimeField.gameObject.SetActive(true);
            scoreText.gameObject.SetActive(true);
            scoreText.text = "0";
            completedObjectives = 0;
            //move player
            player.transform.position = startLocation.transform.position;
            player.transform.rotation = startLocation.transform.rotation;
            Camera.main.GetComponent<FreeCameraLook>().CenterCamera();
            player.GetComponent<PlayerControllerManager>().ChangeMode(PlayerControllerManager.Modes.MovementLock);
            ScreenEffects.GetInstance().ReverseFade(fadeDuration, ()=> { StartCoroutine(CountDown(countdownTimer, player)); });
        });

        //enable all objectives
        foreach (GameObject o in objectives) {
            o.SetActive(true);
        }
    }

    public void EndGame(bool didWin) {

        //disable all objectives
        foreach (GameObject o in objectives)
        {
            o.SetActive(true);
        }
        canvas.SetActive(false);
        OnWin?.Invoke(didWin);
    }

    IEnumerator CountDown(float duration, GameObject player) {
        float endTime = Time.time + duration;
        countdownText.gameObject.SetActive(true);
        while (Time.time <= endTime) {
            countdownText.text = (endTime - Time.time).ToString();
            yield return null;
        }
        countdownText.gameObject.SetActive(false);

        //start game
        startTime = Time.time;
        isPlaying = true;
        player.GetComponent<PlayerControllerManager>().ChangeMode(PlayerControllerManager.Modes.Walking);

    }

}
