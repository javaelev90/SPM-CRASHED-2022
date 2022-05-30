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
    private Image dayImage;
    private Image nightImage;
    private ObjectiveUpdateEvent ev;

    public bool IsNight { get; private set; }

    [Header("Other stuff")]
    //[SerializeField] private LightingManager lightingManager;

    private float nightLength;
    private float dayLength;
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
        StopCoroutine(SearchForLightManager());
    }

    void Start()
    {
        EventSystem.Instance.RegisterListener<EventEvent>(DisplayingTime);
        source = GetComponent<AudioSource>();
        dayImage = day.GetComponent<Image>();
        nightImage = night.GetComponent<Image>();
        ev = new ObjectiveUpdateEvent();

        if (!lightingManager.IsNight)
        {
            //day.gameObject.SetActive(true);
            //night.gameObject.SetActive(false);
            Color color = new Color(1, 1, 1, 0);
            nightImage.color = color;
            IsNight = lightingManager.IsNight;
        }
        else
        {
            //day.gameObject.SetActive(false);
            //night.gameObject.SetActive(true);
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
        updateTimer(timeLeft);

        if (timeLeft > 0 && timeLeft < 5 && !flashing)
        {
            StartCoroutine(Flash3());
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
                nightImage.fillAmount = 1f;
                ev.IsNight = lightingManager.IsNight;
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
                dayImage.fillAmount = 1f;
                ev.IsNight = lightingManager.IsNight;
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

        dayImage.fillAmount -= (1f / dayLength) * Time.deltaTime;
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
            if (timeLeft > 5)
            {
                flashing = false;
            }
        }

        setTextDisplay(true);
        //day.gameObject.SetActive(!day.gameObject.activeSelf);
        //night.gameObject.SetActive(!night.gameObject.activeSelf);
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
