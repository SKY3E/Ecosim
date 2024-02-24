using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    // Game Board
    public float heightBoard = 10;
    public float widthBoard = 20;
    // Camera
    public CinemachineVirtualCamera vcam;
    private float moveSpeed = 5f;
    public float zoomSpeed = 5f;
    public float minZoom = 3f;
    public float maxZoom = 10f;
    // Entities
    public GameObject animalPrefab;
    public GameObject plantPrefab;
    public Transform spawnPoint;
    // Entity Properties
    // Animal (A)
    public int maxLifespanA = 100000;
    public int minLifespanA = 0;
    public int maxSpeedA = 20;
    public int minSpeedA = 1;
    public int maxFoodCapacityA = 1000;
    public int minFoodCapacityA = 10;
    public int maxWaterCapacityA = 1000;
    public int minWaterCapacityA = 10;
    public int maxReproductiveRateA = 5;
    public int minReproductiveRateA = 1;
    // Plant (P)
    public int maxLifespanP = 100000;
    public int minLifespanP = 0;
    public int maxFoodP = 1000;
    public int minFoodP = 10;
    public int maxReproductiveRateP = 5;
    public int minReproductiveRateP = 1;

    void Start()
    {
        for (int i = 0; i <= 6; i++)
        {ChooseEntity("A1");}
        for (int i = 0; i <= 3; i++)
        {ChooseEntity("A2");}
        for (int i = 0; i <= 0; i++)
        {ChooseEntity("P1");}
    }

    void Update()
    {
        MoveCamera();
    }

    public void CreateNewAnimal(string species, int lifespan, int speed, int foodCapacity, int waterCapacity, int reproductiveRate, Vector3 origin)
    {
        GameObject Animal = Instantiate(animalPrefab, origin, Quaternion.identity);
        AnimalManager animalManager = Animal.GetComponent<AnimalManager>();
        if(animalManager != null)
        {animalManager.SetCharacteristics(species, lifespan, speed, foodCapacity, waterCapacity, reproductiveRate);}
        else
        {Debug.LogError("AnimalManager script not found on the prefab!");}
    }

    public void CreateNewPlant(string species, int lifespan, int food, int reproductiveRate, Vector3 origin)
    {
        GameObject Plant = Instantiate(plantPrefab, origin, Quaternion.identity);
        PlantManager plantManager = Plant.GetComponent<PlantManager>();
        if(plantManager != null)
        {plantManager.SetCharacteristics(species, lifespan, food, reproductiveRate);}
        else
        {Debug.LogError("PlantManager script not found on the prefab!");}
    }

    private void ChooseEntity(string species)
    {
        if (species == "A1") // Initialize animal with predefined attributes
        {CreateNewAnimal("A1", 5000, 1, 100, 100, 5000, new Vector3(0, 0, 0));}
        if (species == "P1") // Initialize plant with predefined attributes
        {CreateNewPlant("P1", 5000, 100, 5000, new Vector3(Random.Range(-widthBoard/2, widthBoard/2), Random.Range(-heightBoard/2, heightBoard/2), 0));}

        if (species == "AR") // Initialize animal with random attributes
        {CreateNewAnimal("AR", Random.Range(minLifespanA, maxLifespanA), Random.Range(minSpeedA, maxSpeedA), Random.Range(minFoodCapacityA, maxFoodCapacityA), Random.Range(minWaterCapacityA, maxWaterCapacityA), Random.Range(minReproductiveRateA, maxReproductiveRateA), new Vector3(0, 0, 0));}
        if (species == "PR") // Initialize plant with random attributes
        {CreateNewAnimal("PR", Random.Range(minLifespanP, maxLifespanP), Random.Range(minFoodP, maxFoodP), Random.Range(minReproductiveRateP, maxReproductiveRateP), new Vector3(0, 0, 0));}
    }

    private void MoveCamera()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;
        vcam.transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Q))
        {ZoomCamera(-zoomSpeed * Time.deltaTime);}
        if (Input.GetKey(KeyCode.E))
        {ZoomCamera(zoomSpeed * Time.deltaTime);}

        void ZoomCamera(float delta)
        {
            if (vcam.m_Lens.Orthographic)
            {vcam.m_Lens.OrthographicSize = Mathf.Clamp(vcam.m_Lens.OrthographicSize + delta, minZoom, maxZoom);}
        }
    }
}
