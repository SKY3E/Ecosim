using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    // Game Board
    public float heightBoard = 30;
    public float widthBoard = 60;
    // Camera
    public CinemachineVirtualCamera vcam;
    private float moveSpeed = 5f;
    public float zoomSpeed = 5f;
    public float minZoom = 3f;
    public float maxZoom = 10f;
    // Entity References
    public GameObject animalPrefab;
    public GameObject plantPrefab;
    public Transform spawnPoint;
    // Folders
    public GameObject AnimalFolder;
    public GameObject PlantFolder;
    // Maximum Entities
    public int maxAnimals = 10;
    public int maxPlants = 300;
    // Animal Properties (A)
    public int maxLifespanA = 100000;
    public int minLifespanA = 0;
    public int maxSpeedA = 10;
    public int minSpeedA = 1;
    public int maxFoodCapacityA = 10000;
    public int minFoodCapacityA = 100;
    public int maxWaterCapacityA = 1000;
    public int minWaterCapacityA = 10;
    public int maxReproductiveRateA = 5;
    public int minReproductiveRateA = 1;
    public int maxReproductiveTimeoutA = 100000;
    public int minReproductiveTimeoutA = 1000;
    public float mutationRateA = 0.2f;
    public float minMutationAmtA = 0.8f;
    public float maxMutationAmtA = 1.2f;
    public float minDetectionRadiusA = 1.5f;
    public float maxDetectionRadiusA = 2.5f;
    // Plant Properties (P)
    public int maxLifespanP = 100000;
    public int minLifespanP = 0;
    public int maxFoodP = 10000;
    public int minFoodP = 100;
    public int maxReproductiveRateP = 5;
    public int minReproductiveRateP = 1;
    public int maxReproductiveTimeoutP = 100000;
    public int minReproductiveTimeoutP = 1000;
    public float mutationRateP = 0.2f;
    public float minMutationAmtP = 0.8f;
    public float maxMutationAmtP = 1.2f;
    // UI
    public GameObject selector;
    public GameObject selectedEntity;

    void Start()
    {
        for (int i = 0; i <= 4; i++)
        {ChooseEntity("AR");}
        for (int i = 0; i <= 59; i++)
        {ChooseEntity("PR");}
    }

    void Update()
    {
        MoveCamera();
        UpdateEditorUI();
        UpdateUI();
    }

    public void CreateNewAnimal(string species, int lifespan, int speed, int foodCapacity, int waterCapacity, int reproductiveRate, int reproductiveTimeout, float detectionRadius, Vector3 origin)
    {
        if (AnimalFolder.transform.childCount < maxAnimals)
        {
            GameObject Animal = Instantiate(animalPrefab, origin, Quaternion.identity);
            Animal.transform.SetParent(AnimalFolder.transform);
            AnimalManager animalManager = Animal.GetComponent<AnimalManager>();
            if(animalManager != null)
            {animalManager.SetCharacteristics(species, lifespan, speed, foodCapacity, waterCapacity, reproductiveRate, reproductiveTimeout, detectionRadius);}
            else
            {Debug.LogError("AnimalManager script not found on the prefab!");}
        } else {Debug.Log("Max animals reached");}
    }

    public void CreateNewPlant(string species, int lifespan, int food, int reproductiveRate, int reproductiveTimeout, Vector3 origin)
    {
        if (PlantFolder.transform.childCount < maxPlants)
        {
            GameObject Plant = Instantiate(plantPrefab, origin, Quaternion.identity);
            Plant.transform.SetParent(PlantFolder.transform);
            PlantManager plantManager = Plant.GetComponent<PlantManager>();
            if(plantManager != null)
            {plantManager.SetCharacteristics(species, lifespan, food, reproductiveRate, reproductiveTimeout);}
            else
            {Debug.LogError("PlantManager script not found on the prefab!");}
        } else {Debug.Log("Max plants reached");}
    }

    private void ChooseEntity(string species)
    {
        if (species == "A1") // Initialize animal with predefined attributes
        {CreateNewAnimal("A1", 5000, 1, 1000, 100, 1, 5000, 2, new Vector3(0, 0, 0));}
        if (species == "P1") // Initialize plant with predefined attributes
        {CreateNewPlant("P1", 5000, 1000, 4, 5000, new Vector3(Random.Range(-widthBoard/2, widthBoard/2), Random.Range(-heightBoard/2, heightBoard/2), 0));}

        if (species == "AR") // Initialize animal with random attributes
        {CreateNewAnimal("AR", Random.Range(minLifespanA, maxLifespanA), Random.Range(minSpeedA, maxSpeedA), Random.Range(minFoodCapacityA, maxFoodCapacityA), Random.Range(minWaterCapacityA, maxWaterCapacityA), Random.Range(minReproductiveRateA, maxReproductiveRateA), Random.Range(minReproductiveTimeoutA, maxReproductiveTimeoutA), Random.Range(minDetectionRadiusA, maxDetectionRadiusA), new Vector3(Random.Range(-widthBoard/2, widthBoard/2), Random.Range(-heightBoard/2, heightBoard/2), 0));}
        if (species == "PR") // Initialize plant with random attributes
        {CreateNewPlant("PR", Random.Range(minLifespanP, maxLifespanP), Random.Range(minFoodP, maxFoodP), Random.Range(minReproductiveRateP, maxReproductiveRateP), Random.Range(minReproductiveTimeoutP, maxReproductiveTimeoutP), new Vector3(Random.Range(-widthBoard/2, widthBoard/2), Random.Range(-heightBoard/2, heightBoard/2), 0));}
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

    private void UpdateEditorUI()
    {
        AnimalFolder.name = "Animal Folder - " + AnimalFolder.transform.childCount;
        PlantFolder.name = "Plant Folder - " + PlantFolder.transform.childCount;
    }

    private void UpdateUI()
    {
        // Update Selector and selectedEntity
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);

            if (hit.collider != null)
            {
                selectedEntity = hit.collider.gameObject;
                selector.transform.localScale = selectedEntity.transform.localScale * 2f;
            }
        }
        if (selectedEntity != null)
        {selector.transform.position = selectedEntity.transform.position;}
    }
}
