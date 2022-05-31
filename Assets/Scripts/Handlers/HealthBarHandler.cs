using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HealthBarHandler : MonoBehaviour
{
    private Image healthBarImage;

    public void SetHealthBarValue(float value)
    {
        if (healthBarImage == null) AssignHealthBarImage();

        healthBarImage.fillAmount = value;
        
        //if (value < 0.2f)
        //{
        //    healthBarImage.color = Color.red;
        //}
        //else if (value < 0.4f)
        //{
        //    healthBarImage.color = Color.yellow;
        //}
        //else if(value >= 0.4f)
        //{
        //    healthBarImage.color = Color.white;
        //}
    }

    void AssignHealthBarImage()
    {
        healthBarImage = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        AssignHealthBarImage();
        SetHealthBarValue(1f);
    }
}
