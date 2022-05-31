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
    [SerializeField] private TMP_Text objective;
    [SerializeField] private float smoothTime;
    [SerializeField] private TMP_Text upgradedPartsText;
    [SerializeField] private TMP_Text totalNumberText;
    [SerializeField] private Timer timer;
    [SerializeField] private GameObject objectiveUpdatedEffect;
    [SerializeField] private RectTransform effectTransform;


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
        if (ev.IsShipPartEvent == false)
        {
            if (ev.IsNight == true)
            {
                objective.text = "Defend the ship during the night!";
                shipObjective.SetActive(false);
            }

            if (ev.IsNight == false)
            {
                objective.text = "Explore and find ship parts!";
                shipObjective.SetActive(true);
            }
        }
        else
        {
            objective.text = ev.ObjectiveDescription;
            shipObjective.SetActive(true);
        }
        
        if (effectTransform != null)
        {
            var vfx = Instantiate(objectiveUpdatedEffect, effectTransform.position, Quaternion.identity) as GameObject;
            vfx.transform.SetParent(effectTransform);
            var ps = vfx.GetComponent<ParticleDestroyer>();
            Destroy(vfx, ps.DestroyDelay);
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
