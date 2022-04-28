using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarHandler : MonoBehaviour
{
    private Image healthBarImage;

    public void SetHealthBarValue(float value)
    {
        healthBarImage.fillAmount = value;
        
        if (value < 0.2f)
        {
            healthBarImage.color = Color.red;
        }
        else if (value < 0.4f)
        {
            healthBarImage.color = Color.yellow;
        }
        else
        {
            healthBarImage.color = Color.green;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        healthBarImage = GetComponent<Image>();
    }
}
