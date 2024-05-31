using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEditor;
using System;
using JetBrains.Annotations;
using UnityEngine.UI;
using System.Net.Sockets;

public class changementDeScenes : MonoBehaviour
{

    // Variable de son
    public AudioClip sonUI;
    // Variable fondu au noir 
    public Image imageNoir;
    private float durationFondu = 1f;


    void Start()
    {
        StartCoroutine(fonduAuVisible());
    }

    public void commencez()
    {
        StartCoroutine(fonduChangerSceneDefondu("Niveau1"));
    }

    public void tutoriel()
    {
       // GetComponent<AudioSource>().PlayOneShot(sonUI);
       // SceneManager.LoadScene("Tutoriel");
        StartCoroutine(fonduChangerSceneDefondu("Tutoriel"));
    }
    public void information()
    {
        StartCoroutine(fonduChangerSceneDefondu("Informations"));
    }

    public void accueil()
    {
        StartCoroutine(fonduChangerSceneDefondu("Intro"));
    }
    public void quit()
    {
        GetComponent<AudioSource>().PlayOneShot(sonUI);
        Application.Quit();
    }



    private IEnumerator fonduChangerSceneDefondu(string nomDeScene)
    {
        GetComponent<AudioSource>().PlayOneShot(sonUI);
        yield return StartCoroutine(fonduAuNoir());

        SceneManager.LoadScene(nomDeScene);
    }

    private IEnumerator fonduAuNoir()
    {
        imageNoir.gameObject.SetActive(true);
        float tempsPasser = 0;
        Color couleur = imageNoir.color;

        // On met l'image de plus en plus noir (Moins en moins invisible) 
        while (tempsPasser < durationFondu)
        {
            couleur.a = Mathf.Clamp01(tempsPasser / durationFondu);
            imageNoir.color = couleur;
            tempsPasser += Time.deltaTime;
            yield return null; // Aka attend la prochaine frame et non un temps X 
        }

        // Invisibiliter a 100% a la fin peut importe quoi
        couleur.a = 1;
        imageNoir.color = couleur;
    }

    private IEnumerator fonduAuVisible()
    {
        imageNoir.gameObject.SetActive(true);
        float tempsPasser = 0;
        Color couleur = imageNoir.color;
        couleur.a = 1;
        imageNoir.color = couleur;

        // On met l'image de moins en moins (Plus en plus invisible) 
        while (tempsPasser < durationFondu)
        {
            couleur.a = Mathf.Clamp01(1 - (tempsPasser / durationFondu));
            imageNoir.color = couleur;
            tempsPasser += Time.deltaTime;
            yield return null; // Aka attend la prochaine frame et non un temps X 
        }

        // Invisibiliter a 0% a la fin peut importe quoi
        couleur.a = 0;
        imageNoir.color = couleur;
        imageNoir.gameObject.SetActive(false);
    }
}
