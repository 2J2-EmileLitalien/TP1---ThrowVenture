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
    using UnityEngine.UI;
    using System.Net.Sockets;

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
    private static float score = 0;
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

    // Variables des sons
    public AudioClip sonBalleLancer;
    public AudioClip sonBalleApparaitre;
    public AudioClip sonBalleTeleporation;
    public AudioClip sonBalleExplosion;
    public AudioClip sonPegToucher;
    public AudioClip sonMurToucher;

    // Variable descriptions 
    public TextMeshProUGUI texteDescription;
    public RawImage imageDescription;
    public Texture imageBalleNormale;
    public Texture imageBalleMagique;
    public Texture imageBalleExplosive;
    public Texture imageBalleOr;

    public Scene sceneActuelle;
    public String nomSceneActuelle;

    // Variable fin du jeu
    public float scoreVoulu;
    public Image imageNoir;
    private float durationFondu = 1.5f;
   

    public class Balles
    {
        // General
        public String nom;
        public Sprite sprite;
        public Boolean etat;
        public Texture image;
        public string description;

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
        // On prend le nom de la scene actuelle
            sceneActuelle = SceneManager.GetActiveScene();
            nomSceneActuelle = sceneActuelle.name;
        //

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
            balleNormale.description = "La balle de base. Elle n'a aucun pouvoirs!";
            balleNormale.image = imageBalleNormale;

            // BALLE MAGIQUE (Passe a travers de 1 peg)
            balleMagique = new Balles();
            balleMagique.nom = "Balle Magique"; 
            balleMagique.sprite = balleMagiqueSprite; 
            balleMagique.etat = true; 
            balleMagique.pointsMultiplicateur = 1f; 
            balleMagique.taille = 0.812f; 
            balleMagique.magique = 1; 
            balleMagique.explosion = false;
            balleMagique.description = "Cette balle ré-apparait en haut du plateau de jeu quand elle touche le fond, mais une seule fois!"; 
            balleMagique.image = imageBalleMagique;

            // BALLE EXPLOSIVE (Explose au contact)
            balleExplosive = new Balles();
            balleExplosive.nom = "Balle Explosive";
            balleExplosive.sprite = balleExplosiveSprite;
            balleExplosive.etat = true;
            balleExplosive.pointsMultiplicateur = 1f;
            balleExplosive.taille = 0.812f;
            balleExplosive.magique = 0;
            balleExplosive.explosion = true;
            balleExplosive.description = "Cette balle explose quand elle touche un peg, détruisant tous les pegs dans le rayon de l'explosion!";
            balleExplosive.image = imageBalleExplosive;

            // BALLE EN OR (2x points)
            balleOr = new Balles();
            balleOr.nom = "Balle En Or";
            balleOr.sprite = balleOrSprite;
            balleOr.etat = true;
            balleOr.pointsMultiplicateur = 2f;
            balleOr.taille = 0.812f;
            balleOr.magique = 0;
            balleOr.explosion = false;
            balleOr.description = "Cette balle double les points obtenu lorsqu'elle touche un peg, très utile en combination avec les pegs jaunes!";
            balleOr.image = imageBalleOr;
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

        // On set le Meilleur Score et le score qui sauvegarde a travers les parties

        variableScoreTemp = Convert.ToString(meilleurScore);
        texteMeilleurScore.text = variableScoreTemp;

        variableScoreTemp = Convert.ToString(score);
        texteScore.text = variableScoreTemp;
        //



        // On set la quantite de balles que le joueur peut lancer;
        nmbDeBallesRestante = nmbDeBallesTotale;
            texteBallesRestante.text = nmbDeBallesRestante + "/" + nmbDeBallesTotale;
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
                texteBallesRestante.text = nmbDeBallesRestante + "/" + nmbDeBallesTotale;

                // On track la souris quand elle clique 
                positionSouris = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                balleDisponible = false;
                lancerLaBalle();
            }

        // Fin du jeu. On attend que la balle devienne disponible (Donc on attend la fin du lancer) sinon ca va reset en plein milieu d'un lancer
        } else if (balleDisponible)
        {
            finDePartie();
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

        // Joue le son 
        GetComponent<AudioSource>().PlayOneShot(sonBalleLancer);
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

                // ... et on lance l'animation (Qui contient le segment qui arrete la simulation de la balle pendant l'animation...
                GetComponent<Animator>().enabled = true;
                GetComponent<Animator>().SetTrigger("teleportation");
                // 

                // .. et on joue le son
                GetComponent<AudioSource>().PlayOneShot(sonBalleTeleporation);

            } else
            {
                // Remplacer la balle, etc.
                Invoke("replacerBalle", 0f);
            }

    } else if (collision.gameObject.tag == "limiteDuJeu")
        {
            // Ne rien faire si limite de jeu (Murs, plafond, separateurs en bas)
            // Joue le son 
            GetComponent<AudioSource>().PlayOneShot(sonMurToucher);
        }
        else
        {
            // 100% collision = avec un peg

            // Joue le son 
            GetComponent<AudioSource>().PlayOneShot(sonPegToucher);

            // EFFET DE BALLE SELON LE POINTEUR 

            if (lesBalles[pointeur].explosion)
            {
                // SI BALLE = PEUT EXPLOSER
                lesBalles[pointeur].explosion = false;
                // On change le sprite de la balle 
                GetComponent<SpriteRenderer>().sprite = lesBalles[pointeur].sprite;
                //
                GetComponent<Animator>().enabled = true;
                corpsRigideBalle.constraints = RigidbodyConstraints2D.FreezeAll;

                GetComponent<Animator>().SetTrigger("explosion");

                // Joue le son 
                GetComponent<AudioSource>().PlayOneShot(sonBalleExplosion);

                Invoke("replacerBalle", 1f);


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
        corpsRigideBalle.constraints = RigidbodyConstraints2D.None;

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

        // On change la description 
        texteDescription.text = lesBalles[pointeur].description;
        imageDescription.texture = lesBalles[pointeur].image;

        transform.position = positionInitiale;
        corpsRigideBalle.simulated = false;
        balleDisponible = true;

        // Joue le son 
        GetComponent<AudioSource>().PlayOneShot(sonBalleApparaitre);

        GetComponent<Transform>().localScale = Vector3.one;
        print("Balle actuelle = " + lesBalles[pointeur].nom);
    }

    void finDePartie()
    {
        print("Score = " + score);
        print("Score voulu = " + scoreVoulu);

        // Si score est plus haut ou egal a celui voulu, on avance
        if (score >= scoreVoulu)
        {
            print("On passe au prochain niveau");

            // Commence par tutoriel
            if (nomSceneActuelle == "Tutoriel")
            {
                // On ne veut pas le score du tutoriel dans le score du jeu (Donc il ne sera pas dans le highscore a la fin, etc.) 
                score = 0;
                StartCoroutine(fonduChangerSceneDefondu("Niveau1"));
            }
            else if (nomSceneActuelle == "Niveau1")
            {
                StartCoroutine(fonduChangerSceneDefondu("Niveau2"));
            }
            else if (nomSceneActuelle == "Niveau2")
            {
                StartCoroutine(fonduChangerSceneDefondu("Niveau3"));
            }
            else
            {
                // 100% au niveau 3 et on reussi le niveau

                // Si le score actuel est meilleur que le meilleurScore, on change le meilleurScore
                if (score > meilleurScore)
                {
                    meilleurScore = score;
                    variableScoreTemp = Convert.ToString(meilleurScore);
                    texteMeilleurScore.text = variableScoreTemp;
                }
                score = 0;
                // Fin du jeu revient a l'intro
                StartCoroutine(fonduChangerSceneDefondu("Intro"));
            }
        }
        // Sinon on recommence depuis le debut! (DEFAITE)
        else
        {
            print("On recommence au niveau 1");

            // Recommence
            StartCoroutine(fonduChangerSceneDefondu("Niveau1"));
        }
    }


    private IEnumerator fonduChangerSceneDefondu(string nomDeScene)
    {
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

        print("Fondu au noir fini!");
    }


}

