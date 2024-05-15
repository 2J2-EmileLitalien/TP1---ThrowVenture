using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class changementDeScenes : MonoBehaviour
{

    // Variable de son
    public AudioClip sonUI;
   public void commencez()
    {
        GetComponent<AudioSource>().PlayOneShot(sonUI);
        SceneManager.LoadScene("SampleScene");
    }
    public void information()
    {
        GetComponent<AudioSource>().PlayOneShot(sonUI);
        SceneManager.LoadScene("Informations");
    }

    public void accueil()
    {
        GetComponent<AudioSource>().PlayOneShot(sonUI);
        SceneManager.LoadScene("Intro");
    }
    public void quit()
    {
        GetComponent<AudioSource>().PlayOneShot(sonUI);
        Application.Quit();
    }
}
