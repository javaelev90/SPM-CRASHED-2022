using TMPro;
using UnityEngine;
using System.Collections;
using EventCallbacksSystem;
using UnityEngine.UI;

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
    //[SerializeField]
    //private TextMeshProUGUI day; 
    //[SerializeField]
    //private TextMeshProUGUI night;

    [SerializeField] private GameObject day;
    [SerializeField] private GameObject night;

    [Header("Other stuff")]
    //[SerializeField] private LightingManager lightingManager;

    private float timeLeft = 0;
    private float flashTimer = 0;
    private float flashduration = 0.5f;
    private bool flashing = false;

    private AudioSource source;
    private LightingManager lightingManager;

    public AudioClip clip;

    private void OnEnable()
    {
        StartCoroutine(SearchForLightManager());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void Start()
    {
        EventSystem.Instance.RegisterListener<EventEvent>(DisplayingTime);
        source = GetComponent<AudioSource>();


        if (!lightingManager.IsNight)
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

    IEnumerator SearchForLightManager()
    {
        while (true)
        {
            lightingManager = FindObjectOfType<LightingManager>();
            if (lightingManager != null)
            {
                break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft = lightingManager.TimeUntilCycle;
        updateTimer(timeLeft);

        if (timeLeft > 0 && timeLeft < 5 && !flashing)
        {
            StartCoroutine(Flash3());
            source.PlayOneShot(clip);
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

    /*private void Flash3 (){
        flashTimer += Time.deltaTime;
        if(flashTimer > flashduration){
            setTextDisplay(!minutEtt.enabled);
            flashTimer = 0;
        }
        setTextDisplay(true);
        day.gameObject.SetActive(!day.gameObject.activeSelf);
        night.gameObject.SetActive(!night.gameObject.activeSelf);
    }*/

    private void setTextDisplay(bool enabled){
        minutEtt.enabled = enabled;
        minutTwo.enabled = enabled;
        separate.enabled = enabled;
        sekundEtt.enabled = enabled;
        sekundTwo.enabled = enabled;
    }

    public void Show(bool enabled){
        //day.enabled = enabled;
        //night.enabled = enabled;
        day.SetActive(enabled);
        night.SetActive(enabled);
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

    public void DisplayingTime(EventEvent eventEvent)
    {
        DisplayingTime(!eventEvent.Start);
    }

    public void DisplayingTime(bool on)
    {
        minutEtt.enabled = on;
        minutTwo.enabled = on;
        separate.enabled = on;
        sekundEtt.enabled = on;
        sekundTwo.enabled = on;
    }
}
