using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour {

    public Image FadeImg;
    public float fadeSpeed = 0.6f;
    public bool sceneStarting = true;
    //[HideInInspector]
    //public AllLogics logics;
    private float elapsedTime = 0;
    private bool fadeInOut = false;


    void Awake()
    {
        FadeImg.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
    }

    void FixedUpdate()
    {
        // If the scene is starting...
        if (sceneStarting)
            // ... call the StartScene function.
            StartScene();
        else if (fadeInOut)
        {
            if (FadeImg.color.a >= 0.95f)
            {
                //logics.fadeOutFinished = true;
                sceneStarting = true;
                fadeSpeed = 0.7f;
            }
            else FadeToBlack();
        }
    }


    void FadeToClear()
    {
        // Lerp the colour of the image between itself and transparent.
        //FadeImg.color = Color.Lerp(FadeImg.color, Color.clear, fadeSpeed * Time.deltaTime); //empieza rapido termina lento
        FadeImg.color = Color.Lerp(Color.black, Color.clear, elapsedTime); //mantiene velocidad
    }


    void FadeToBlack()
    {
        // Lerp the colour of the image between itself and black.
        FadeImg.color = Color.Lerp(FadeImg.color, Color.black, fadeSpeed * Time.deltaTime);
    }


    void StartScene()
    {
        // Fade the texture to clear.
        FadeToClear();
        elapsedTime += Time.deltaTime * fadeSpeed;
        // If the texture is almost clear...
        if (FadeImg.color.a <= 0.05f)
        {
            // ... set the colour to clear and disable the RawImage.
            FadeImg.color = Color.clear;
            FadeImg.enabled = false;

            // The scene is no longer starting.
            sceneStarting = false;
            fadeInOut = false;
            //if (logics != null) logics.fadeInFinished = true;
        }
    }


    public IEnumerator EndSceneRoutine(int SceneNumber)
    {
        // Make sure the RawImage is enabled.
        FadeImg.enabled = true;
        do
        {
            // Start fading towards black.
            FadeToBlack();

            // If the screen is almost black...
            if (FadeImg.color.a >= 0.95f)
            {
                // ... reload the level
                SceneManager.LoadScene(SceneNumber);
                yield break;
            }
            else
            {
                yield return null;
            }
        } while (true);
    }

    public void EndScene(int SceneNumber)
    {
        sceneStarting = false;
        StartCoroutine("EndSceneRoutine", SceneNumber);
    }

    public void FadeOutIn()
    {
        fadeInOut = true;
        FadeImg.enabled = true;
        fadeSpeed = 1.5f;
        elapsedTime = 0;
    }
}
