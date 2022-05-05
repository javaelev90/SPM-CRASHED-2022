using TMPro;
using UnityEngine;

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

    private float timer;
    private float flashTimer = 0;
    private float flashduration = 0.5f; 

    // Start is called before the first frame update
    void Start()
    {
        if(lightingManager.TimeOfDay < lightingManager.DayLength)
        {
            day.gameObject.SetActive(true);
            night.gameObject.SetActive(false);
            timer = lightingManager.DayLength - lightingManager.TimeOfDay;
        }
        else
        {
            day.gameObject.SetActive(false);
            night.gameObject.SetActive(true);
            timer = lightingManager.NightLength - lightingManager.TimeOfDay + lightingManager.DayLength;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        updateTimer(timer);

        if (timer > 0 && timer < 5)
        {
            Flash3();
        }
        else if (timer <= 0)
        {
            setTextDisplay(true);
            day.gameObject.SetActive(!day.gameObject.activeSelf);
            night.gameObject.SetActive(!night.gameObject.activeSelf);
            reset();
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

    private void reset(){
        if (!lightingManager.IsNight)
        {
            timer = lightingManager.DayLength;
        }
        else
        {
            timer = lightingManager.NightLength;
        }
        
    }
    private void Flash3 (){
        flashTimer += Time.deltaTime;
        if(flashTimer > flashduration){
            setTextDisplay(!minutEtt.enabled);
            flashTimer = 0;
        }
    }

    private void setTextDisplay(bool enabled){
        minutEtt.enabled = enabled;
        minutTwo.enabled = enabled;
        separate.enabled = enabled;
        sekundEtt.enabled = enabled;
        sekundTwo.enabled = enabled;
    }
}
