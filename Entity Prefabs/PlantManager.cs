using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    // Game
    public GameObject gameManagerObj;
    // Entity Initialization (Constant Traits)
    public string species;
    public int lifespan;
    public int food;
    public int reproductiveRate;
    public int reproductiveTimeout;
    // Entity Updates (Updatable Traits)
    public int age;
    public int reproductionTimer;
    // Entity Reproduction
    public float mutationRate;
    public float minMutationAmt;
    public float maxMutationAmt;

    void Start()
    {
        // Set game characteristics
        gameManagerObj = GameObject.FindWithTag("GameController");
        GameManager gameManager = gameManagerObj.GetComponent<GameManager>();
        // Set entity traits
        age = lifespan;
        gameObject.name = species;
        // Set entity reproduction traits
        mutationRate = gameManager.mutationRateP;
        minMutationAmt = gameManager.minMutationAmtP;
        maxMutationAmt = gameManager.maxMutationAmtP;
    }

    void Update()
    {
        age--;
        reproductionTimer++;
        transform.localScale = new Vector3(0.5f * ((float)age / (float)lifespan), 0.5f * ((float)age / (float)lifespan), 1f);
        Reproduce();
        if(age < 0)
        {this.food = 0; Destroy(gameObject);}
    }

    public void SetCharacteristics(string species, int lifespan, int food, int reproductiveRate, int reproductiveTimeout)
    {
        // Characteristics
        this.species = species;
        this.lifespan = lifespan;
        this.food = food;
        this.reproductiveRate = reproductiveRate;
        this.reproductiveTimeout = reproductiveTimeout;
        // Visuals
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Circle");
        GetComponent<SpriteRenderer>().color = new Color(214f / 255f, 188f / 255f, 81f / 255f, 1f);
        CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = 0.5f;
        // Initialization
        transform.localScale = new Vector3(0.5f, 0.5f, 1f);
    }

    private void Reproduce()
    {
        if (reproductionTimer >= reproductiveTimeout)
        {
            for (int i = 0; i < reproductiveRate; i++)
            {
                float offsetX = 0;
                float offsetY = 0;
                float offsetXSide = Random.Range(0, 2);
                if (offsetXSide == 0) {offsetX = Random.Range(-5f, -2f);}
                else if (offsetXSide == 1) {offsetX = Random.Range(2f, 5f);}
                float offsetYSide = Random.Range(0, 2);
                if (offsetYSide == 0) {offsetY = Random.Range(-5f, -2f);}
                else if (offsetYSide == 1) {offsetY = Random.Range(2f, 5f);}

                Vector3 origin = transform.position + new Vector3(offsetX, offsetY, 0f);

                gameManagerObj.GetComponent<GameManager>().CreateNewPlant(this.species, this.lifespan, this.food, this.reproductiveRate, this.reproductiveTimeout, origin);
            }
            reproductionTimer = 0;
        }
    }
}
