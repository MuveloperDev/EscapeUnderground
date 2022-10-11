using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameSceneUIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup textsPanel = null;
    [SerializeField] private Light2D masterLight = null;
    [SerializeField] private Light2D challengerLight = null;
    [SerializeField] private float alpha = 0f;



    private void Start()
    {
        Cursor.visible = false;
        textsPanel.alpha = 0;
        masterLight.intensity = alpha;
        challengerLight.intensity = alpha;
        //StartCoroutine(TrunLight());
    }

    IEnumerator TrunLight()
    {
        while (true)
        {
            yield return null;
            if (challengerLight.intensity >= 1.5f)
            {
                GameManager.Instance.SetSceneChang(true);
                yield break;
            } 
            alpha += 0.002f;
            masterLight.intensity = alpha;
            challengerLight.intensity = alpha;
        }
        
    }

    private void FixedUpdate()
    {
        if(challengerLight.intensity >= 1.5f)
        {
            GameManager.Instance.SetSceneChang(true);
            return;
        }
        alpha += 0.02f;
        masterLight.intensity = alpha;
        challengerLight.intensity = alpha;
    }
}
