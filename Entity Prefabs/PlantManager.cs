using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    // Game
    public GameObject gameManagerObj;
    public float heightBoard = 0;
    public float widthBoard = 0;
    // Entity Initialization
    public string species;
    public int lifespan;
    public int food;
    public int reproductiveRate;
    // Entity Updates
    public int age;

    // Start is called before the first frame update
    void Start()
    {
        SetGameCharacteristics();
        age = lifespan;
        gameObject.name = species;

    }

    // Update is called once per frame
    void Update()
    {
        age--;
        if(age < 0)
        {
            Reproduce();
            Destroy(gameObject);
        }
    }

    public void SetCharacteristics(string species, int lifespan, int food, int reproductiveRate)
    {
        // Characteristics
        this.species = species;
        this.lifespan = lifespan;
        this.food = food;
        this.reproductiveRate = reproductiveRate;
        // Visuals
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Circle");
        GetComponent<SpriteRenderer>().color = Color.green;
        CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = 0.5f;
        // Initialization
        transform.localScale = new Vector3(0.5f, 0.5f, 1f);
    }

    private void SetGameCharacteristics()
    {
        gameManagerObj = GameObject.FindWithTag("GameController");
        GameManager gameManager = gameManagerObj.GetComponent<GameManager>();
        heightBoard = gameManager.heightBoard;
        widthBoard = gameManager.widthBoard;
    }

    private void Reproduce()
    {
        int numNewPlants = Random.Range(0, 5); 
        
        for (int i = 0; i < numNewPlants; i++)
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

            gameManagerObj.GetComponent<GameManager>().CreateNewPlant(this.species, this.lifespan, this.food, this.reproductiveRate, origin);
        }
    }
}
