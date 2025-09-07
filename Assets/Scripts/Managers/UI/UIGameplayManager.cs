using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIGameplayManager : MonoBehaviour
{
    public static UIGameplayManager Instance { get; private set; }

    [Header("Level Info")]
    [SerializeField] private TMP_Text distanceText;
    [SerializeField] private TMP_Text histsValue;
    // [SerializeField] private TMP_Text levelTtimer;

    // [Header("Charge UI")]
    // [SerializeField] private Slider chargeSlider;
    // [SerializeField] private TMP_Text forceText;

    // [Header("Projectile Info")]
    // [SerializeField] private TMP_Text projectileCountText;
    // [SerializeField] protected GameObject pauseScreen;




    // Singleton
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // public void UpdateLevelUI(int level)
    // {
    //     levelText.text = $"Level: {level}";
    // }

    public void UpdateDistanceUI(string distance)
    {
        distanceText.text = distance;
    }

    public void UpdateHistsUI(string hists)
    {
        histsValue.text = hists;
    }

}