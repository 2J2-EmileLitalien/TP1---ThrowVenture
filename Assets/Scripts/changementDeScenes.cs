using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class changementDeScenes : MonoBehaviour
{
   public void commencez()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void information()
    {
        SceneManager.LoadScene("Informations");
    }

    public void accueil()
    {
        SceneManager.LoadScene("Intro");
    }
    public void quit()
    {
        Application.Quit();
    }
}
