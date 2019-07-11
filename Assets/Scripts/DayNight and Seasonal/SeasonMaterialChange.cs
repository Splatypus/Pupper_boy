using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonMaterialChange : MonoBehaviour {

    [Tooltip("Set to the meshrenderer on this same object if left null")]
    public MeshRenderer target;
    public int targetMaterial = 0;
    public Material summer, fall, winter, spring;

    private Material defaultMat;

    // Use this for initialization
    void Awake() {
        //assign target mesh to our own if none are asssigned
        if (target == null)
            target = gameObject.GetComponent<MeshRenderer>();

        defaultMat = target.materials[targetMaterial];
        EventManager.OnSeasonChange += OnSeasonChanged;
    }

    void OnDestroy() {
        EventManager.OnSeasonChange -= OnSeasonChanged;
    }

    // called when a season is changed
    void OnSeasonChanged(SeasonManager.Seasons s) {
        //assign material to the given material for the season we changed to. Defaults to the original material if none are given
        Material newMaterial = null;
        switch (s) {
            case SeasonManager.Seasons.SUMMER:
                newMaterial = summer;
                break;
            case SeasonManager.Seasons.FALL:
                newMaterial = fall;
                break;
            case SeasonManager.Seasons.WINTER:
                newMaterial = winter;
                break;
            case SeasonManager.Seasons.SPRING:
                newMaterial = spring;
                break;
            default:
                break;
        }

        Material[] mats = target.materials;
        mats[targetMaterial] = newMaterial != null ? newMaterial : defaultMat;
        target.materials = mats;

    }
}
