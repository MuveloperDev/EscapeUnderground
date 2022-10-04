using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LightControllerCS : MonoBehaviour
{

    [SerializeField] SpriteRenderer cursor = null;
    [SerializeField] Light2D cursorLight = null;

    // Update is called once per frame
    private void Start()
    {
        Cursor.visible = false;
    }
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        cursor.transform.position = mousePos;
        if (SceneManager.GetActiveScene().name == "StartScene")
            cursorLight.transform.position = mousePos;
    }
}
