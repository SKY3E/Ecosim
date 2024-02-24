using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    // Game
    public GameObject gameManagerObj;
    public float heightBoard = 0;
    public float widthBoard = 0;
    // Entity Initialization (Constant Traits)
    public string species;
    public int lifespan;
    public int speed;
    public int foodCapacity;
    public int waterCapacity;
    public int reproductiveRate;
    // Entity Updates (Updatable Traits)
    public int age;
    public int food;
    public int reproductionTimer;
    // Entity Movement
    public Vector3 targetPosition = Vector3.zero;

    void Start()
    {
        SetGameCharacteristics();
        age = lifespan;
    }

    void Update()
    {
        age--;
        if(age < 0)
        {Destroy(gameObject);}
        Movement();
    }

    public void SetCharacteristics(string species, int lifespan, int speed, int foodCapacity, int waterCapacity, int reproductiveRate)
    {
        this.species = species;
        this.lifespan = lifespan;
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Circle");
        GetComponent<SpriteRenderer>().color = Color.black;
        this.speed = speed;
        this.foodCapacity = foodCapacity;
        this.waterCapacity = waterCapacity;
        this.reproductiveRate = reproductiveRate;
        CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = 0.5f;
    }

    private void SetGameCharacteristics()
    {
        gameManagerObj = GameObject.FindWithTag("GameController");
        GameManager gameManager = gameManagerObj.GetComponent<GameManager>();
        heightBoard = gameManager.heightBoard;
        widthBoard = gameManager.widthBoard;
    }

    private void Movement()
    {
        // Target Movement
        if (targetPosition == Vector3.zero || transform.position == targetPosition)
        {targetPosition = new Vector3(Random.Range(-widthBoard/2, widthBoard/2), Random.Range(-heightBoard/2, heightBoard/2), 0);}
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }
}
