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


    [Header("DayNight")]
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text nightText;
    [SerializeField] private GameObject day;
    [SerializeField] private GameObject night;
    [SerializeField] private RectTransform effectTransform;
    [SerializeField] private GameObject nightAlarmEffect;
    [SerializeField] private GameObject eventTimeObject;
    private Image dayImage;
    private Image nightImage;
    private ObjectiveUpdateEvent ev;

    // EventStarter parameters
    private bool isEventStarted;
    private float eventTime;
    private float eventTimeCounter;
    private bool IsUntilDawn;
    private float timeUntilDawn;
    public bool IsNight { get; private set; }



    [Header("Other stuff")]
    //[SerializeField] private LightingManager lightingManager;

    private float nightLength;
    private float dayLength;
    private float timeLeft = 0;
    private float flashTimer = 0;
    private float flashduration = 0.5f;
    private bool flashing = false;
    private bool isEffectFlashing = false;

    private AudioSource source;
    private LightingManager lightingManager;
    private static bool loadedData = false;
    private bool initialized = false;
    public AudioClip clip;

    private void OnEnable()
    {
        StartCoroutine(SearchForLightManager());
    }

    private void OnDisable()
    {
        StopCoroutine(SearchForLightManager());
        StopCoroutine(Flash3());
        StopCoroutine(Flash3Effect());
    }

    void Start()
    {
        if(initialized == false)
        {
            Initialize();
            initialized = true;
        }
    }

    private void Initialize()
    {
        EventSystem.Instance.RegisterListener<EventEvent>(DisplayingTime);
        EventSystem.Instance.RegisterListener<ShipPartEvent>(TimeUntilDawn);
        source = GetComponent<AudioSource>();
        dayImage = day.GetComponent<Image>();
        nightImage = night.GetComponent<Image>();
        ev = new ObjectiveUpdateEvent();

        if (!lightingManager.IsNight)
        {
            Color color = new Color(1, 1, 1, 0);
            nightImage.color = color;
            IsNight = lightingManager.IsNight;
        }
        else
        {
            Color color = new Color(1, 1, 1, 0);
            dayImage.color = color;
            IsNight = lightingManager.IsNight;
        }
        timeLeft = lightingManager.TimeUntilCycle;

        dayLength = lightingManager.DayLength;
        nightLength = lightingManager.NightLength;
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

        if (isEventStarted)
        {
            eventTimeCounter -= Time.deltaTime;
            if (eventTimeCounter > 0 && eventTimeCounter < 3 && !flashing)
            {
                StartCoroutine(Flash3());
            }

            updateTimer(eventTimeCounter);
            if (eventTimeCounter <= 0f)
            {
                isEventStarted = false;
                eventTimeCounter = 0f;
                eventTimeObject.SetActive(false);
            }
        }

        if (timeLeft > 0 && timeLeft < 5 && isEffectFlashing == false)
        {

            StartCoroutine(Flash3Effect());

            source.PlayOneShot(clip);
            if (effectTransform != null)
            {
                var vfx = Instantiate(nightAlarmEffect, effectTransform.position, Quaternion.identity) as GameObject;
                vfx.transform.SetParent(effectTransform);
                var ps = vfx.GetComponent<ParticleDestroyer>();
                Destroy(vfx, ps.DestroyDelay);
            }
        }

        NightTime();
        DayTime();
    }

    private void TimeUntilDawn(ShipPartEvent ev)
    {
        timeUntilDawn = ev.TimeUntilDawn;
        IsUntilDawn = true;
    }

    private void NightTime()
    {
        if (lightingManager.IsNight == false)
        {
            return;
        }

        if (lightingManager.IsNight == true)
        {
            if (nightText.gameObject.activeSelf == false)
            {
                dayText.gameObject.SetActive(false);
                nightText.gameObject.SetActive(true);
                nightImage.gameObject.SetActive(true);
                // Fix for loading save file
                if (loadedData == false)
                {
                    nightImage.fillAmount = 1f;
                }
                else
                {
                    loadedData = false;
                }
                ev.IsNight = lightingManager.IsNight;
                ev.IsShipPartEvent = false;
                //IsUntilDawn = false;
                EventSystem.Instance.FireEvent(ev);
            }

            Color dayColor = dayImage.color;
            dayColor.a = Mathf.Lerp(dayColor.a, 0, 2f * Time.deltaTime);
            dayImage.color = dayColor;
            dayImage.gameObject.SetActive(dayImage.color == dayColor);

            Color nightColor = nightImage.color;
            nightColor.a = Mathf.Lerp(nightColor.a, 1f, 2f * Time.deltaTime);
            nightImage.color = nightColor;
        }

        nightImage.fillAmount -= (1f / nightLength) * Time.deltaTime;
    }

    private void DayTime()
    {
        if (lightingManager.IsNight == true)
        {
            return;
        }

        if (lightingManager.IsNight == false)
        {
            if (dayText.gameObject.activeSelf == false)
            {
                nightText.gameObject.SetActive(false);
                dayText.gameObject.SetActive(true);
                dayImage.gameObject.SetActive(true);
                // Fix for loading save file
                if (loadedData == false)
                {
                    dayImage.fillAmount = 1f;
                }
                else
                {
                    loadedData = false;
                }
                
                ev.IsNight = lightingManager.IsNight;
                ev.IsShipPartEvent = false;
                //IsUntilDawn = false;
                EventSystem.Instance.FireEvent(ev);
            }

            Color dayColor = dayImage.color;
            dayColor.a = Mathf.Lerp(dayColor.a, 1f, 2f * Time.deltaTime);
            dayImage.color = dayColor;

            Color nightColor = nightImage.color;
            nightColor.a = Mathf.Lerp(nightColor.a, 0f, 2f * Time.deltaTime);
            nightImage.color = nightColor;
            nightImage.gameObject.SetActive(nightImage.color == nightColor);
        }

        if (IsUntilDawn == true)
        {
            dayImage.fillAmount = timeUntilDawn / dayLength;
            IsUntilDawn = false;
        }


        dayImage.fillAmount -= (1f / dayLength) * Time.deltaTime;
     }

    public void LoadTime()
    {
        loadedData = true;
        Initialize();
        if (lightingManager.IsNight)
        {
            nightImage.gameObject.SetActive(true);
            nightImage.fillAmount =  1f - ((lightingManager.TimeOfDay - lightingManager.DayLength) / lightingManager.NightLength);
        }
        else
        {
            dayImage.gameObject.SetActive(true);
            dayImage.fillAmount = 1f - (lightingManager.TimeOfDay / lightingManager.DayLength);
        }
    }

    private void updateTimer(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
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

    private void setTextDisplay(bool enabled)
    {
        minutEtt.enabled = enabled;
        minutTwo.enabled = enabled;
        separate.enabled = enabled;
        sekundEtt.enabled = enabled;
        sekundTwo.enabled = enabled;
    }

    public void Show(bool enabled)
    {
        //day.enabled = enabled;
        //night.enabled = enabled;
        //day.SetActive(enabled);
        //night.SetActive(enabled);
    }

    private IEnumerator Flash3()
    {
        flashing = true;

        while (flashing)
        {
            setTextDisplay(!minutEtt.enabled);
            yield return new WaitForSeconds(flashduration);
            if (eventTimeCounter > 5)
            {
                flashing = false;
            }
        }

        setTextDisplay(true);
        //day.gameObject.SetActive(!day.gameObject.activeSelf);
        //night.gameObject.SetActive(!night.gameObject.activeSelf);
    }

    private IEnumerator Flash3Effect()
    {
        isEffectFlashing = true;

        while (flashing)
        {
            yield return new WaitForSeconds(flashduration);
            if (timeLeft > 7)
            {
                isEffectFlashing = false;
            }
        }
    }

    public void DisplayingTime(EventEvent eventEvent)
    {
        ev.IsNight = lightingManager.IsNight;
        ev.ObjectiveDescription = "Event started, secure ship part!";

        eventTime = eventEvent.EventTime;

        isEventStarted = eventEvent.Start;
        ev.IsShipPartEvent = eventEvent.Start;

        if (isEventStarted)
        {
            eventTimeCounter = eventTime;
            eventTimeObject.SetActive(isEventStarted);
        }
        else
        {
            eventTimeObject.SetActive(isEventStarted);
        }
        EventSystem.Instance.FireEvent(ev);
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
