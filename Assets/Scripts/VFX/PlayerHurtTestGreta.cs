using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurtTestGreta : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Lerp between startValue and endValue over 'duration' seconds
    /*private IEnumerator LerpVignette(float startValue, float endValue, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float value = Mathf.Lerp(startValue, endValue, elapsed / duration);
            bodySkinnedMeshRenderer.SetBlendShapeWeight(0, value);
            yield return null;
        }
    }*/

    /*private bool talking = true;
    //animate open and closed, then repeat
    private IEnumerator AnimateMouth()
    {
        while (talking)
        {
            //yield return StartCoroutine waits for that coroutine to finish before continuing
            yield return StartCoroutine(LerpShape(0, 23, .5f));
            yield return StartCoroutine(LerpShape(23, 0, .5f));
        }
    }*/
}
