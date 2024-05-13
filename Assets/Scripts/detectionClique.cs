using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

public class detectionClique : MonoBehaviour
{

    // Variables du lancer 
    private bool balleDisponible = true;
    public float forceDuLancer;
    private Rigidbody2D corpsRigideBalle;

    private Vector3 positionSouris;
    private Vector3 directionDuLancer;
    private Vector2 positionInitiale;
    private Vector2 positionBalleMagique;

    // Variables du score
    private float score = 0;
    public TextMeshProUGUI texteScore;
    public static float meilleurScore = 0;
    public TextMeshProUGUI texteMeilleurScore;
    private string variableScoreTemp;

    // Variables de fin de partie
    public TextMeshProUGUI texteBallesRestante;
    private float nmbDeBallesRestante;
    public float nmbDeBallesTotale;
    private float nmbDePegsToucher;
    public float nmbDePegsTotal;

    // Variables changement de balles 
    public Sprite balleMagiqueSprite;
    public Sprite balleExplosiveSprite;
    public Sprite balleOrSprite;
    public Sprite balleNormaleSprite;

    public class Balles
    {
        // General
        public String nom;
        public Sprite sprite;
        public Boolean etat;

        // Plus specifique
        public float pointsMultiplicateur;
        public float taille;
        public int magique;
        public Boolean explosion;
    }

    private Balles balleNormale;
    private Balles balleMagique;
    private Balles balleExplosive;
    private Balles balleOr;

    private Balles[] lesBalles;

    private int pointeur = 0;


    // Start is called before the first frame update
    void Start()
    {
        // Creation de mes balles + leurs informations
            // BALLE NORMALE (Aucun changement)
            balleNormale = new Balles();
            balleNormale.nom = "Balle Normale";
            balleNormale.sprite = balleNormaleSprite;
            balleNormale.etat = true;
            balleNormale.pointsMultiplicateur = 1f;
            balleNormale.taille = 0.812f;
            balleNormale.magique = 0;
            balleNormale.explosion = false;

            // BALLE MAGIQUE (Passe a travers de 1 peg)
            balleMagique = new Balles();
            balleMagique.nom = "Balle Magique"; 
            balleMagique.sprite = balleMagiqueSprite; 
            balleMagique.etat = true; 
            balleMagique.pointsMultiplicateur = 1f; 
            balleMagique.taille = 0.812f; 
            balleMagique.magique = 1; 
            balleMagique.explosion = false;

            // BALLE EXPLOSIVE (Explose au contact)
            balleExplosive = new Balles();
            balleExplosive.nom = "Balle Explosive";
            balleExplosive.sprite = balleExplosiveSprite;
            balleExplosive.etat = true;
            balleExplosive.pointsMultiplicateur = 1f;
            balleExplosive.taille = 0.812f;
            balleExplosive.magique = 0;
            balleExplosive.explosion = true;

            // BALLE EN OR (2x points)
            balleOr = new Balles();
            balleOr.nom = "Balle En Or";
            balleOr.sprite = balleOrSprite;
            balleOr.etat = true;
            balleOr.pointsMultiplicateur = 2f;
            balleOr.taille = 0.812f;
            balleOr.magique = 0;
            balleOr.explosion = false;
        //

        // On met les balles dans un array pour pouvoir les prendre en cycle (Chaque lancer = Une nouvelle) 
            lesBalles = new Balles[] { balleNormale, balleMagique, balleExplosive, balleOr };
        //

        print("Balle actuelle = " + lesBalles[pointeur].nom);


        // Au lieu de set le corps rigide dans lancerLaBalle, donc le set a chaque clique, on le fait 1 fois
            corpsRigideBalle = GetComponent<Rigidbody2D>();
        //

        // On sauvegarde la position initiale de la balle pour la replacer a chaque lancer
            positionInitiale = transform.position;
        //

        // On set le Meilleur Score qui sauvegarde a travers les parties

        variableScoreTemp = Convert.ToString(meilleurScore);
        texteMeilleurScore.text = variableScoreTemp;
        //

        // On set la quantite de balles que le joueur peut lancer;
            nmbDeBallesRestante = nmbDeBallesTotale;
            texteBallesRestante.text = nmbDeBallesRestante + " / " + nmbDeBallesTotale;
        //

        // On desactive l'animator 
            GetComponent<Animator>().enabled = false;
        //
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
                variableScoreTemp = Convert.ToString(meilleurScore);
                texteMeilleurScore.text = variableScoreTemp;
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
            // Si balle actuelle peut revenir en haut (Magique), sinon remplacer la balle 
            if (lesBalles[pointeur].magique > 0)
            {
            // La balle est magique + peut revenir en haut

                // On fait -1 a la quantiter de fois ou elle peut revenir en haut...
                    lesBalles[pointeur].magique -= 1;
                //

                // ... et on la remplace en haut...
                positionBalleMagique = new Vector2(transform.position.x, positionInitiale.y);
                transform.position = positionBalleMagique;
                //

                // ... et on lance l'animation (Qui contient le segment qui arrete la simulation de la balle pendant l'animation
                GetComponent<Animator>().enabled = true;
                GetComponent<Animator>().SetTrigger("teleportation");

            } else
            {
                // Remplacer la balle, etc.
                Invoke("replacerBalle", 0f);
            }

    } else if (collision.gameObject.tag == "limiteDuJeu")
        {
            // Ne rien faire si limite de jeu (Murs, plafond, separateurs en bas)
        }
        else
        {
         // 100% collision = avec un peg


            // EFFET DE BALLE SELON LE POINTEUR 

            if (lesBalles[pointeur].explosion)
            {
                // SI BALLE = PEUT EXPLOSER
                lesBalles[pointeur].explosion = false;
                GetComponent<Animator>().enabled = true;
                GetComponent<Animator>().SetTrigger("explosion");

                Invoke("replacerBalle", 0.9f);


            }

            // On enleve le peg et on ajoute qu'un peg a ete toucher...
                nmbDePegsToucher += 1;
                collision.gameObject.SetActive(false);
            //

            // ...et on arrange le score selon le peg toucher
                if (collision.gameObject.tag == "pegBleu")
                {
                    score += lesBalles[pointeur].pointsMultiplicateur * 1;

                }
                else if (collision.gameObject.tag == "pegJaune")
                {
                    score += lesBalles[pointeur].pointsMultiplicateur * 3;
                }
            //

            

        }

        // Peut importe la collision, on re-affiche le score (Englobe les collisions pegs, collision avec zone de fin qui reset la balle, etc.)
        variableScoreTemp = Convert.ToString(score);
        texteScore.text = variableScoreTemp;
    }


    void replacerBalle()
    {
        GetComponent<Animator>().enabled = false;

        // On reset le magique si la balle est la magique AVANT de changer de balle
        if (lesBalles[pointeur].nom == "Balle Magique")
        {
            lesBalles[pointeur].magique = 1;
        }
        //

        // On reset l'explosion si la balle est la explosive AVANT de changer de balle
        if (lesBalles[pointeur].nom == "Balle Explosive")
        {
            lesBalles[pointeur].explosion = true;
        }
        //

        // On cycle a travers les balles apres chaque lancer
        if (pointeur < lesBalles.Length - 1)
        {
            pointeur += 1;
        }
        else
        {
            pointeur = 0;
        }
        //

        // On change le sprite de la balle 
        GetComponent<SpriteRenderer>().sprite = lesBalles[pointeur].sprite;
        //

        transform.position = positionInitiale;
        corpsRigideBalle.simulated = false;
        balleDisponible = true;

        GetComponent<Transform>().localScale = Vector3.one;
        print("Balle actuelle = " + lesBalles[pointeur].nom);
    }

    void finDePartie()
    {
        // print("Meilleur score on end = " + meilleurScore);

        // On reset la scene
        SceneManager.LoadScene("SampleScene");
    }


}

