using TMPro;
using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
    [Header("GUI stuff")]
    [SerializeField]
    private TextMeshProUGUI minutEtt; 
    [SerializeField]
    private TextMeshProUGUI minutTwo; 
    [SerializeField]
    private TextMeshProUGUI separate; 
    [SerializeField]
    private TextMeshProUGUI sekundEtt; 
    [SerializeField]
    private TextMeshProUGUI sekundTwo; 
    [SerializeField]
    private TextMeshProUGUI day; 
    [SerializeField]
    private TextMeshProUGUI night;

    [Header("Other stuff")]
    [SerializeField] private LightingManager lightingManager;

    private float timeLeft = 0;
    private float flashTimer = 0;
    private float flashduration = 0.5f;
    private bool flashing = false;

    // Start is called before the first frame update
    void Start()
    {
        if(!lightingManager.IsNight)
        {
            day.gameObject.SetActive(true);
            night.gameObject.SetActive(false);
            
        }
        else
        {
            day.gameObject.SetActive(false);
            night.gameObject.SetActive(true);
        }
        timeLeft = lightingManager.TimeUntilCycle;
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft = lightingManager.TimeUntilCycle;
        updateTimer(timeLeft);

        if (timeLeft > 0 && timeLeft < 5 && !flashing)
        {
            StartCoroutine(Flash3());
        }
    }

    private void updateTimer(float time){
        float minutes = Mathf.FloorToInt(time/60);
        float seconds = Mathf.FloorToInt(time % 60);

        string currentTime = string.Format("{00:00}{1:00}", minutes, seconds, day);
        minutEtt.text = currentTime[0].ToString();
        minutTwo.text = currentTime[1].ToString();
        sekundEtt.text = currentTime[2].ToString();
        sekundTwo.text = currentTime[3].ToString();
    }

    private void Flash33 (){
        flashTimer += Time.deltaTime;
        if(flashTimer > flashduration){
            setTextDisplay(!minutEtt.enabled);
            flashTimer = 0;
        }
        setTextDisplay(true);
        day.gameObject.SetActive(!day.gameObject.activeSelf);
        night.gameObject.SetActive(!night.gameObject.activeSelf);
    }

    private void setTextDisplay(bool enabled){
        minutEtt.enabled = enabled;
        minutTwo.enabled = enabled;
        separate.enabled = enabled;
        sekundEtt.enabled = enabled;
        sekundTwo.enabled = enabled;
    }

    private IEnumerator Flash3()
    {
        flashing = true;

        while (flashing)
        {
            setTextDisplay(!minutEtt.enabled);
            yield return new WaitForSeconds(flashduration);
            if(timeLeft > 5)
            {
                flashing = false;
            }
        }

        setTextDisplay(true);
        day.gameObject.SetActive(!day.gameObject.activeSelf);
        night.gameObject.SetActive(!night.gameObject.activeSelf);
    }
}
