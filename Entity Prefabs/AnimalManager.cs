using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    // Game
    public GameObject gameManagerObj;
    private GameManager gameManager;
    // Entity Initialization (Constant Traits)
    public string species;
    public int lifespan;
    public int speed;
    public int foodCapacity;
    public int waterCapacity;
    public int reproductiveRate;
    public int reproductiveTimeout;
    public float detectionRadius;
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
    public LayerMask plantLayer;

    void Start()
    {
        // Set game characteristics
        gameManagerObj = GameObject.FindWithTag("GameController");
        gameManager = gameManagerObj.GetComponent<GameManager>();
        plantLayer = LayerMask.GetMask("Plants");
        // Set entity traits
        age = lifespan;
        food = (int)(foodCapacity * 0.5f);
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

    public void SetCharacteristics(string species, int lifespan, int speed, int foodCapacity, int waterCapacity, int reproductiveRate, int reproductiveTimeout, float detectionRadius)
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
        this.detectionRadius = detectionRadius;
        CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = 0.5f;
    }

    private void Movement()
    {
        if (targetPosition == Vector3.zero)
        {targetPosition = new Vector3(transform.position.x + Random.Range(-6f, 6f), transform.position.y + Random.Range(-6f, 6f), 0);}
        if (targetPosition == transform.position && isEating == false)
        {targetPosition = new Vector3(transform.position.x + Random.Range(-6f, 6f), transform.position.y + Random.Range(-6f, 6f), 0);}
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    private void ActionChooser()
    {
        // Find food target
        if (food < foodCapacity * 0.75f)
        {
           Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, plantLayer);

            float closestDistance = Mathf.Infinity;
            closestPlantTarget = null;

            foreach (Collider2D collider in colliders)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlantTarget = collider.gameObject;
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
                    {food += closestPlantTarget.GetComponent<PlantManager>().food; closestPlantTarget.GetComponent<PlantManager>().food = 0;}
                    else {closestPlantTarget.GetComponent<PlantManager>().food -= 30; food += 30;}

                    // If plant is empty, destroy it and reset variables
                    if (closestPlantTarget.GetComponent<PlantManager>().food == 0)
                    {
                        Destroy(closestPlantTarget);
                        isEating = false;
                        closestPlantTarget = null;
                    }
                }
                else // Move towards the target only if not eating
                {Movement();}
            } else {isEating = false; Movement();}
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
                Random.Range(0f, 1f) > mutationRate ? this.detectionRadius : this.detectionRadius * Random.Range(minMutationAmt, maxMutationAmt),
                new Vector3(transform.position.x + Random.Range(-2f, 2f), transform.position.y + Random.Range(-2f, 2f), 0)
            );
            Debug.Log("Animal Entity Reproduction!");
            reproductionTimer = 0;
            food -= (int)(foodCapacity * 0.5f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
