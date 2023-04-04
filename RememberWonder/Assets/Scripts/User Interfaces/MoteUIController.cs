using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoteUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI motesCollectedTxt;
    [SerializeField] private TextMeshProUGUI totalMotesTxt;
    [SerializeField] private Image tutorialImage;

    private int _collectedCount;
    private int _totalCount;

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
        CollectedCount = 0;
        CollectMote.MoteSpawned += OnMoteSpawned;
        CollectMote.MoteCollected += IncrementCollectedCount;
    }

    private void OnDestroy()
    {
        CollectMote.MoteSpawned -= OnMoteSpawned;
        CollectMote.MoteCollected -= IncrementCollectedCount;
    }

    private void OnMoteSpawned(CollectMote _, bool isCollected)
    {
        if (isCollected) { CollectedCount++; }
        TotalCount++;
    }

    private void IncrementCollectedCount(CollectMote _)
    {
        CollectedCount++;
    }

    //Displays tutorial text on screen
    public void DisplayTutorialText(Sprite sprite) 
    {
        tutorialImage.sprite = sprite;
        tutorialImage.transform.parent.gameObject.SetActive(true);
        tutorialImage.SetNativeSize();
    }

    public void DismissTutorialText()
    {
        tutorialImage.transform.parent.gameObject.SetActive(false);
    }
}