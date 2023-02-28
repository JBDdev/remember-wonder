using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoteUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI motesCollectedTxt;
    [SerializeField] private TextMeshProUGUI totalMotesTxt;
    [SerializeField] private TextMeshProUGUI tutorialTxt;

    private int _collectedCount;
    private int _totalCount;

    private void Start()
    {
        motesCollectedTxt.text = "0";
        totalMotesTxt.text = "14";
    }

    /// <summary>
    /// The UI's internal count of how many motes have been collected.<br/>
    /// Automatically updates the UI when set.
    /// </summary>
    public int CollectedCount
    {
        get => _collectedCount;
        private set
        {
            _collectedCount = value;

            if (motesCollectedTxt)
                motesCollectedTxt.text = value.ToString();
        }
    }

    /// <summary>
    /// The UI's internal count of how many motes are in the scene, total.<br/>
    /// Automatically updates the UI when set.
    /// </summary>
    public int TotalCount
    {
        get => _totalCount;
        private set
        {
            _totalCount = value;

            if (totalMotesTxt)
                totalMotesTxt.text = value.ToString();
        }
    }

    private void Awake()
    {
        CollectMote.MoteSpawned += UpdateMoteCounts;
        CollectMote.MoteCollected += IncrementCollectedCount;
    }

    private void OnDestroy()
    {
        CollectMote.MoteSpawned -= UpdateMoteCounts;
        CollectMote.MoteCollected -= IncrementCollectedCount;
    }

    private void UpdateMoteCounts(CollectMote _, bool isCollected)
    {
        if (isCollected) { CollectedCount++; }
        TotalCount++;
    }

    private void IncrementCollectedCount(CollectMote _)
    {
        CollectedCount++;
    }

    //Displays tutorial text on screen
    public void DisplayTutorialText(string text) 
    {
        tutorialTxt.text = text;
        tutorialTxt.transform.parent.gameObject.SetActive(true);
    }

    public void DismissTutorialText()
    {
        tutorialTxt.transform.parent.gameObject.SetActive(false);
    }
}