using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EventCallbacksSystem;

public class ObjectiveViewer : MonoBehaviour
{
    [SerializeField] private RectTransform displayObjectivePosition;
    [SerializeField] private RectTransform hideObjectivePosition;
    [SerializeField] private RectTransform objectivePanel;
    [SerializeField] private GameObject shipObjective;
    [SerializeField] private GameObject dayObjective;
    [SerializeField] private GameObject nightObjective;
    [SerializeField] private float smoothTime;
    [SerializeField] private TMP_Text upgradedPartsText;
    [SerializeField] private TMP_Text totalNumberText;
    [SerializeField] private Timer timer;

    public bool IsDisplayingPanel { get; set; }

    private void OnEnable()
    {
        EventSystem.Instance.RegisterListener<ObjectiveUpdateEvent>(UpdateObjectiveText);
    }

    private void OnDisable()
    {
        EventSystem.Instance.UnregisterListener<ObjectiveUpdateEvent>(UpdateObjectiveText);
    }


    void Update()
    {
        if (IsDisplayingPanel == true)
        {
            objectivePanel.anchoredPosition = Vector2.Lerp(objectivePanel.anchoredPosition, displayObjectivePosition.anchoredPosition, smoothTime);
            if (Vector2.Distance(objectivePanel.anchoredPosition, displayObjectivePosition.anchoredPosition) < 0.01f) { this.enabled = false; }
        }

        if (IsDisplayingPanel == false)
        {
            objectivePanel.anchoredPosition = Vector2.Lerp(objectivePanel.anchoredPosition, hideObjectivePosition.anchoredPosition, smoothTime);
            if (Vector2.Distance(objectivePanel.anchoredPosition, hideObjectivePosition.anchoredPosition) < 0.01f) { this.enabled = false; }
        }
    }

    public void UpdateObjectiveText(ObjectiveUpdateEvent ev)
    {
        if (ev.IsNight == true)
        {
            dayObjective.SetActive(false);
            nightObjective.SetActive(true);
            shipObjective.SetActive(true);
        }

        if (ev.IsNight == false)
        {
            dayObjective.SetActive(true);
            nightObjective.SetActive(false);
            shipObjective.SetActive(false);
        }
    }

    public void UpdateUpgradedShipParts(int upgraded)
    {
        upgradedPartsText.text = upgraded.ToString();
    }

    public void InitializeShipPartsAmount(int completed, int total)
    {
        upgradedPartsText.text = completed.ToString();
        totalNumberText.text = total.ToString();
    }
     
}
