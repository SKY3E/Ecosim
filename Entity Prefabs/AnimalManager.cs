using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    // Game
    public GameObject gameManagerObj;
    private GameManager gameManager;
    public float heightBoard = 0;
    public float widthBoard = 0;
    // Entity Initialization (Constant Traits)
    public string species;
    public int lifespan;
    public int speed;
    public int foodCapacity;
    public int waterCapacity;
    public int reproductiveRate;
    public int reproductiveTimeout;
    // Entity Updates (Updatable Traits)
    public int age;
    public int food;
    public int reproductionTimer;
    public bool isEating = false;
    // Entity Reproduction
    public float mutationRate;
    public float minMutationAmt;
    public float maxMutationAmt;
    // Entity Movement
    public Vector3 targetPosition = Vector3.zero;
    public GameObject closestPlantTarget = null;

    void Start()
    {
        // Set game characteristics
        gameManagerObj = GameObject.FindWithTag("GameController");
        gameManager = gameManagerObj.GetComponent<GameManager>();
        heightBoard = gameManager.heightBoard;
        widthBoard = gameManager.widthBoard;
        // Set entity traits
        age = lifespan;
        food = foodCapacity;
        gameObject.name = species;
        // Set entity reproduction traits
        mutationRate = gameManager.mutationRateA;
        minMutationAmt = gameManager.minMutationAmtA;
        maxMutationAmt = gameManager.maxMutationAmtA;
    }

    void Update()
    {
        // Entity Updates
        age--;
        food -= 1;
        reproductionTimer++;

        if(age < 0 || food < 0)
        {Destroy(gameObject);}
        ActionChooser();
    }

    public void SetCharacteristics(string species, int lifespan, int speed, int foodCapacity, int waterCapacity, int reproductiveRate, int reproductiveTimeout)
    {
        this.species = species;
        this.lifespan = lifespan;
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Circle");
        GetComponent<SpriteRenderer>().color = Color.black;
        this.speed = speed;
        this.foodCapacity = foodCapacity;
        this.waterCapacity = waterCapacity;
        this.reproductiveRate = reproductiveRate;
        this.reproductiveTimeout = reproductiveTimeout;
        CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = 0.5f;
    }

    private void Movement()
    {
        if (targetPosition == Vector3.zero)
        {targetPosition = new Vector3(Random.Range(-widthBoard/2, widthBoard/2), Random.Range(-heightBoard/2, heightBoard/2), 0);}
        if (targetPosition == transform.position && isEating == false)
        {targetPosition = Vector3.zero;}
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    private void Reproduce()
    {}

    private void ActionChooser()
    {
        // Find food target
        if (food < foodCapacity)
        {   
            if (closestPlantTarget == null)
            {
                GameObject[] plants = GameObject.FindGameObjectsWithTag("Plant");
                if (plants.Length > 0)
                {
                    GameObject closestPlant = plants[0];
                    float closestDistance = Vector3.Distance(transform.position, closestPlant.transform.position);
                    foreach (GameObject plant in plants)
                    {
                        float distance = Vector3.Distance(transform.position, plant.transform.position);
                        if (distance < closestDistance)
                        {
                            closestPlant = plant;
                            closestDistance = distance;
                        }
                    }
                    closestPlantTarget = closestPlant;
                }
            }

            // Move towards the closest plant
            if (closestPlantTarget != null)
            {
                targetPosition = closestPlantTarget.transform.position;

                // Check if close enough to start eating
                if (Vector3.Distance(transform.position, closestPlantTarget.transform.position) < 0.5f)
                {
                    isEating = true;
                    // Reduce plant's food and increase animal's food
                    if (closestPlantTarget.GetComponent<PlantManager>().food < 30)
                    {
                        food += closestPlantTarget.GetComponent<PlantManager>().food;
                        closestPlantTarget.GetComponent<PlantManager>().food = 0;
                    }
                    else
                    {
                        closestPlantTarget.GetComponent<PlantManager>().food -= 30;
                        food += 30;
                    }

                    // If plant is empty, destroy it and reset variables
                    if (closestPlantTarget.GetComponent<PlantManager>().food == 0)
                    {
                        Destroy(closestPlantTarget);
                        targetPosition = Vector3.zero;
                        isEating = false;
                        closestPlantTarget = null;
                    }
                }
                else // Move towards the target only if not eating
                {Movement();}
            }
        }
        else // If full or no plant found, move randomly
        {Movement();}
        
        // Reproduce if possible
        if (reproductionTimer >= reproductiveTimeout && food >= foodCapacity * 0.75f)
        {
            gameManager.CreateNewAnimal(
                this.species, 
                (int)(Random.Range(0f, 1f) > mutationRate ? this.lifespan : this.lifespan * Random.Range(minMutationAmt, maxMutationAmt)),
                (int)(Random.Range(0f, 1f) > mutationRate ? this.speed : this.speed * Random.Range(minMutationAmt, maxMutationAmt)),
                (int)(Random.Range(0f, 1f) > mutationRate ? this.foodCapacity : this.foodCapacity * Random.Range(minMutationAmt, maxMutationAmt)),
                (int)(Random.Range(0f, 1f) > mutationRate ? this.waterCapacity : this.waterCapacity * Random.Range(minMutationAmt, maxMutationAmt)),
                (int)(Random.Range(0f, 1f) > mutationRate ? this.reproductiveRate : this.reproductiveRate * Random.Range(minMutationAmt, maxMutationAmt)),
                (int)(Random.Range(0f, 1f) > mutationRate ? this.reproductiveTimeout : this.reproductiveTimeout * Random.Range(minMutationAmt, maxMutationAmt)),
                new Vector3(transform.position.x + Random.Range(-2f, 2f), transform.position.y + Random.Range(-2f, 2f), 0)
            );
            Debug.Log("Animal Entity Reproduction!");
            reproductionTimer = 0;
            food -= (int)(foodCapacity * 0.5f);
        }
    }
}
