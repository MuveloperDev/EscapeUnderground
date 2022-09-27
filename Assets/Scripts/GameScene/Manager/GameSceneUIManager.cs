using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup panel = null;

    private void Awake()
    {
        panel = FindObjectOfType<CanvasGroup>();
    }

    private void Start()
    {
        panel.gameObject.SetActive(true);
        panel.alpha = 1.0f;
        StartCoroutine(PanelGetDark());
    }

    IEnumerator PanelGetDark()
    {
        Debug.Log("코루틴 시작");
        yield return new WaitForSeconds(0.5f);
        while (panel.alpha != 0)
        {
            Debug.Log(panel.alpha);
            panel.alpha -= 0.005f;
            yield return null;
        }
        panel.gameObject.SetActive(false);
        GameManager.Instance.SetSceneChang(true);
    }
}
