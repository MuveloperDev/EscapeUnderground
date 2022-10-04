using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    [SerializeField] Light2D light2D = null;
    [SerializeField] float pointLightOuterRadius = 1.5f;
    [SerializeField] float textAlpha = 0f;
    [SerializeField] TextMeshProUGUI titleText = null;
    [SerializeField] TextMeshProUGUI buttonText = null;
    [SerializeField] AudioManager audioManager = null;

    // Start is called before the first frame update
    void Start()
    {
        titleText.alpha = textAlpha;
        buttonText.alpha = textAlpha;
        StartCoroutine(MoveLight());
    }

    IEnumerator MoveLight()
    {
        while (true)
        {
            yield return null;
            if (light2D.transform.position.y <= 0)
            { 
                StartCoroutine(Lighting());
                yield break;
            }
            light2D.transform.Translate(Vector3.down * 2f * Time.deltaTime);
        }
    }
    IEnumerator Lighting()
    {
        audioManager.SoundPlay(audioManager.TitleFxSound);
        while (true)
        {
            yield return null;
            if (light2D.pointLightOuterRadius >= 9f) yield break;
            textAlpha += 0.01f;
            titleText.alpha = textAlpha;
            buttonText.alpha = textAlpha;
            pointLightOuterRadius += 0.05f;
            light2D.pointLightOuterRadius = pointLightOuterRadius;
        }
    }
}
