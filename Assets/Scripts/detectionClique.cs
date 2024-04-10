using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class detectionClique : MonoBehaviour
{

    // Variables du lancer 
    private bool balleDisponible = true;
    public float forceDuLancer;
    private Rigidbody2D corpsRigideBalle;

    private Vector3 positionSouris;
    private Vector3 directionDuLancer;
    private Vector2 positionInitiale;

    // Variables du score
    private float score = 0;
    public TextMeshProUGUI texteScore;
    private float meilleurScore = 0;
    public TextMeshProUGUI texteMeilleurScore;

    // Variables de fin de partie
    public TextMeshProUGUI texteBallesRestante;
    private float nmbDeBallesRestante;
    public float nmbDeBallesTotale;
    private float nmbDePegsToucher;
    public float nmbDePegsTotal;



    // Start is called before the first frame update
    void Start()
    {
        // Au lieu de set le corps rigide dans lancerLaBalle, donc le set a chaque clique, on le fait 1 fois
        corpsRigideBalle = GetComponent<Rigidbody2D>();

        // On sauvegarde la position initiale de la balle
        positionInitiale = transform.position;


        // On set le Meilleur Score qui sauvegarde a travers les parties
        meilleurScore = PlayerPrefs.GetFloat("MeilleurScore");
        texteMeilleurScore.text = "Meilleur score " + meilleurScore;

        // On set la quantite de balles que le joueur peut lancer;
        nmbDeBallesRestante = nmbDeBallesTotale;
        texteBallesRestante.text = nmbDeBallesRestante + " / " + nmbDeBallesTotale;
    }

    // Update is called once per frame
    void Update()
    {
        // Conditions de fin (Reste des balles a lancer / reste des pegs sur la planche de jeu
        if (nmbDeBallesRestante > 0 && nmbDePegsToucher < nmbDePegsTotal)
        {
            // Detection du clique
            if (Input.GetMouseButton(0) && balleDisponible)
            {
                nmbDeBallesRestante -= 1;
                texteBallesRestante.text = nmbDeBallesRestante + " / " + nmbDeBallesTotale;

                // On track la souris quand elle clique 
                positionSouris = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                balleDisponible = false;
                lancerLaBalle();
            }

        // Fin du jeu. On attend que la balle devienne disponible (Donc on attend la fin du lancer) sinon ca va reset en plein milieu d'un lancer
        } else if (balleDisponible)
        {
            // Si le score actuel est meilleur que le meilleurScore, on change le meilleurScore
            if (score > meilleurScore)
            { 
                meilleurScore = score;
                texteMeilleurScore.text = "Meilleur score: " + meilleurScore;
            }

            // Peut importe, on fait fin de la partie
            Invoke("finDePartie", 2f);
        }

        
    }

    void lancerLaBalle()
    {
        // On active la simulation du corps
        corpsRigideBalle.simulated = true;

        // On decide la direction selon direction originale (transform.position) et position du clique 
        directionDuLancer = positionSouris - transform.position;

        // On enleve le Z 
        directionDuLancer.z = 0;

        // On donne la force. Le ForceMode2D.Impulse = Le moyen par lequel on donne la force, ici on veut du Instant
        corpsRigideBalle.AddForce(directionDuLancer * forceDuLancer, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "fondDuPlateauDeJeu")
        {
            // Replace la balle, on enleve la simulation, la rend disponible
            transform.position = positionInitiale;
            corpsRigideBalle.simulated = false;
            balleDisponible = true;

        } else if (collision.gameObject.tag == "pegBleu")
        {
            // On enleve le peg et ajoute au score
            nmbDePegsToucher += 1;
            collision.gameObject.SetActive(false);
            score += 1;
   
        } else if (collision.gameObject.tag == "pegJaune")
        {
            // On enleve le peg et ajoute au score
            nmbDePegsToucher += 1;
            collision.gameObject.SetActive(false);
            score += 3;
        }

        // Peut importe la collision, on re-affiche le score (Englobe les collisions pegs, collision avec zone de fin qui reset la balle, etc.)
        texteScore.text = "Score: " + score;
    }


    void finDePartie()
    {
        // On sauvegarde le meilleur score 
        PlayerPrefs.SetFloat("MeilleurScore", meilleurScore);


        // print("Meilleur score on end = " + meilleurScore);
        // print("MeilleurScore de PlayerPrefs en fin de partie = " + PlayerPrefs.GetFloat("MeilleurScore"));

        // On reset la scene
        SceneManager.LoadScene("SampleScene");
    }


}

