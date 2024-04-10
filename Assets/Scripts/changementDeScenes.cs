using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class changementDeScenes : MonoBehaviour
{
    private string sceneActuelle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        sceneActuelle = SceneManager.GetActiveScene().name;

        if (Input.GetKeyDown(KeyCode.W) && sceneActuelle != "SampleScene")
        {
            // On lance la scene de jeu 
            SceneManager.LoadScene("SampleScene");


        } else if (Input.GetKeyDown(KeyCode.F) && sceneActuelle == "Intro")
        {
            // On lance la scene d'informations
                SceneManager.LoadScene("Informations");


        } else if (Input.GetKeyDown(KeyCode.X))
        {
            // On quitte le jeu si la scene est l'introduction. On retourne a l'introduction si on est dans la scene de jeu
            if (sceneActuelle == "SampleScene" || sceneActuelle == "Informations")
            {
                SceneManager.LoadScene("Intro");
            } else
            {
                Application.Quit();
            }
        }
    }
}
