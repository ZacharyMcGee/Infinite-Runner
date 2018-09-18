using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Character_Controller : MonoBehaviour
{
    public GameObject[] roads;
    public GameObject[] buildings;

    public GameObject latestBuilding;
    public GameObject latestRoad;
    public GameObject latestRoadLeft;
    public GameObject latestRoadRight;

    public float offset = 22f;

    public bool turning = false;
    public bool pendingTurn = false;
    public bool pendingTRoad = false;

    public float spawnTime = .5f;

    public int count = 0;
    public int buildingCount = 0;
    public int straightCount = 0;

    public Vector3 startPoint = new Vector3(0f, 0f, 0f);

    public Vector3 man_Front_Rotation = new Vector3(0f, 0f, 0f);
    public Vector3 man_Left_Rotation = new Vector3(0f, -90f, 0f);
    public Vector3 man_Back_Rotation = new Vector3(0f, -180f, 0f);
    public Vector3 man_Right_Rotation = new Vector3(0f, -270f, 0f);

    public Vector3 road1_Front_Rotation = new Vector3(0f, 90f, 0f);
    public Vector3 road1_Left_Rotation = new Vector3(0f, 0f, 0f);
    public Vector3 road1_Back_Rotation = new Vector3(0f, -90f, 0f);
    public Vector3 road1_Right_Rotation = new Vector3(0f, -180f, 0f);

    public Vector3 road2_Front_Rotation = new Vector3(0f, -90f, 0f);
    public Vector3 road2_Left_Rotation = new Vector3(0f, -180f, 0f);
    public Vector3 road2_Back_Rotation = new Vector3(0f, -270f, 0f);
    public Vector3 road2_Right_Rotation = new Vector3(0f, 0f, 0f);

    public enum Directions { Front, Left, Right, Back } // Front is starting position, back is 180 degrees from front
    Directions currentDirection;

    public enum Positions { Left, Center, Right }
    Positions currentPosition;

    public Vector3 root;

    private Queue<GameObject> roadQueue = new Queue<GameObject>();
    private Queue<GameObject> buildingQueue = new Queue<GameObject>();


    public void Start()
    {
        currentDirection = Directions.Front;
        currentPosition = Positions.Center;
        latestRoad = Instantiate(roads[0], startPoint, Quaternion.Euler(road1_Front_Rotation));
        root = latestRoad.transform.position;
        roadQueue.Enqueue(latestRoad);
        MapBuilder();
    }

    public void Update()
    {
        transform.Translate(Vector3.forward * 22f * Time.deltaTime);
        GetInput();
    }

    public void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (pendingTurn)
            {
                Turn(Directions.Left);
            }
            else
            {
                if (currentPosition == Positions.Center)
                {
                    if (currentDirection == Directions.Front)
                    {
                        transform.position = new Vector3(transform.position.x - 4f, transform.position.y, transform.position.z);
                        currentPosition = Positions.Left;
                    }
                    else if (currentDirection == Directions.Back)
                    {
                        transform.position = new Vector3(transform.position.x + 4f, transform.position.y, transform.position.z);
                        currentPosition = Positions.Left;
                    }
                    else if (currentDirection == Directions.Left)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 4f);
                        currentPosition = Positions.Left;
                    }
                    else if (currentDirection == Directions.Right)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 4f);
                        currentPosition = Positions.Left;
                    }
                }
                else if(currentPosition == Positions.Right)
                {
                    if(currentDirection == Directions.Front)
                    {
                        currentPosition = Positions.Center;
                        transform.position = new Vector3(transform.position.x - 4f, transform.position.y, transform.position.z);
                    }
                    else if (currentDirection == Directions.Back)
                    {
                        currentPosition = Positions.Center;
                        transform.position = new Vector3(transform.position.x + 4f, transform.position.y, transform.position.z);
                    }
                    else if(currentDirection == Directions.Left)
                    {
                        currentPosition = Positions.Center;
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 4f);
                    }
                    else if (currentDirection == Directions.Right)
                    {
                        currentPosition = Positions.Center;
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 4f);
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (pendingTurn)
            {
                Turn(Directions.Right);
            }
            else
            {
                if (currentPosition == Positions.Center)
                {
                    if (currentDirection == Directions.Front)
                    {
                        transform.position = new Vector3(transform.position.x + 4f, transform.position.y, transform.position.z);
                        currentPosition = Positions.Right;
                    }
                    else if (currentDirection == Directions.Back) {
                        transform.position = new Vector3(transform.position.x - 4f, transform.position.y, transform.position.z);
                        currentPosition = Positions.Right;
                    }
                    else if (currentDirection == Directions.Left)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 4f);
                        currentPosition = Positions.Right;
                    }
                    else if (currentDirection == Directions.Right)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 4f);
                        currentPosition = Positions.Right;
                    }
                }
                else if (currentPosition == Positions.Left)
                {
                    if (currentDirection == Directions.Front)
                    {
                        currentPosition = Positions.Center;
                        transform.position = new Vector3(transform.position.x + 4f, transform.position.y, transform.position.z);
                    }
                    else if (currentDirection == Directions.Back)
                    {
                        currentPosition = Positions.Center;
                        transform.position = new Vector3(transform.position.x - 4f, transform.position.y, transform.position.z);
                    }
                    else if (currentDirection == Directions.Left)
                    {
                        currentPosition = Positions.Center;
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 4f);
                    }
                    else if (currentDirection == Directions.Right)
                    {
                        currentPosition = Positions.Center;
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 4f);
                    }
                }
            }
        }
    }

    public void SpawnStraightRoad()
    {
        if (currentDirection == Directions.Front)
        {
            latestRoad = Instantiate(roads[0], NewLocation(currentDirection), Quaternion.Euler(road1_Front_Rotation));
        }
        else if (currentDirection == Directions.Left)
        {
            latestRoad = Instantiate(roads[0], NewLocation(currentDirection), Quaternion.Euler(road1_Left_Rotation));
        }
        else if (currentDirection == Directions.Back)
        {
            latestRoad = Instantiate(roads[0], NewLocation(currentDirection), Quaternion.Euler(road1_Back_Rotation));
        }
        else if (currentDirection == Directions.Right)
        {
            latestRoad = Instantiate(roads[0], NewLocation(currentDirection), Quaternion.Euler(road1_Right_Rotation));
        }
        roadQueue.Enqueue(latestRoad);
        SpawnBuildings(latestRoad);
        count++;
        straightCount++;
    }

    public void SpawnTRoad()
    {
        if (currentDirection == Directions.Front)
        {
            latestRoad = Instantiate(roads[1], NewLocation(currentDirection), Quaternion.Euler(road2_Front_Rotation));
            roadQueue.Enqueue(latestRoad);
            SpawnBuildings(latestRoad);

            latestRoadLeft = Instantiate(roads[0], NewLocation(Directions.Left), Quaternion.Euler(road1_Left_Rotation));
            roadQueue.Enqueue(latestRoadLeft);

            latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoadLeft.transform.position.x, latestRoadLeft.transform.position.y, latestRoadLeft.transform.position.z + offset), Quaternion.Euler(road1_Right_Rotation));
            buildingQueue.Enqueue(latestBuilding);
            buildingCount++;

            latestRoadRight = Instantiate(roads[0], NewLocation(Directions.Right), Quaternion.Euler(road1_Left_Rotation));
            roadQueue.Enqueue(latestRoadRight);

            latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoadRight.transform.position.x, latestRoadRight.transform.position.y, latestRoadRight.transform.position.z + offset), Quaternion.Euler(road1_Right_Rotation));
            buildingQueue.Enqueue(latestBuilding);
            buildingCount++;
        }
        else if(currentDirection == Directions.Left)
        {
            latestRoad = Instantiate(roads[1], NewLocation(currentDirection), Quaternion.Euler(road2_Left_Rotation));
            roadQueue.Enqueue(latestRoad);
            SpawnBuildings(latestRoad);

            latestRoadLeft = Instantiate(roads[0], NewLocation(Directions.Back), Quaternion.Euler(road1_Front_Rotation));
            roadQueue.Enqueue(latestRoadLeft);

            latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoadLeft.transform.position.x - offset, latestRoadLeft.transform.position.y, latestRoadLeft.transform.position.z), Quaternion.Euler(road1_Front_Rotation));
            buildingQueue.Enqueue(latestBuilding);
            buildingCount++;

            latestRoadRight = Instantiate(roads[0], NewLocation(Directions.Front), Quaternion.Euler(road1_Front_Rotation));
            roadQueue.Enqueue(latestRoadRight);

            latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoadRight.transform.position.x - offset, latestRoadRight.transform.position.y, latestRoadRight.transform.position.z), Quaternion.Euler(road1_Front_Rotation));
            buildingQueue.Enqueue(latestBuilding);
            buildingCount++;
        }
        else if(currentDirection == Directions.Back)
        {
            latestRoad = Instantiate(roads[1], NewLocation(currentDirection), Quaternion.Euler(road2_Back_Rotation));
            roadQueue.Enqueue(latestRoad);
            SpawnBuildings(latestRoad);

            latestRoadLeft = Instantiate(roads[0], NewLocation(Directions.Right), Quaternion.Euler(road1_Right_Rotation));
            roadQueue.Enqueue(latestRoadLeft);


            latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoadLeft.transform.position.x, latestRoadLeft.transform.position.y, latestRoadLeft.transform.position.z - offset), Quaternion.Euler(road1_Left_Rotation));
            buildingQueue.Enqueue(latestBuilding);
            buildingCount++;

            latestRoadRight = Instantiate(roads[0], NewLocation(Directions.Left), Quaternion.Euler(road1_Right_Rotation));
            roadQueue.Enqueue(latestRoadRight);


            latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoadRight.transform.position.x, latestRoadLeft.transform.position.y, latestRoadLeft.transform.position.z - offset), Quaternion.Euler(road1_Left_Rotation));
            buildingQueue.Enqueue(latestBuilding);
            buildingCount++;
        }
        else if(currentDirection == Directions.Right)
        {
            latestRoad = Instantiate(roads[1], NewLocation(currentDirection), Quaternion.Euler(road2_Right_Rotation));
            roadQueue.Enqueue(latestRoad);
            SpawnBuildings(latestRoad);

            latestRoadLeft = Instantiate(roads[0], NewLocation(Directions.Front), Quaternion.Euler(road1_Back_Rotation));
            roadQueue.Enqueue(latestRoadLeft);

            latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoadLeft.transform.position.x + offset, latestRoadLeft.transform.position.y, latestRoadLeft.transform.position.z), Quaternion.Euler(road1_Back_Rotation));
            buildingQueue.Enqueue(latestBuilding);
            buildingCount++;

            latestRoadRight = Instantiate(roads[0], NewLocation(Directions.Back), Quaternion.Euler(road1_Back_Rotation));
            roadQueue.Enqueue(latestRoadRight);

            latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoadRight.transform.position.x + offset, latestRoadRight.transform.position.y, latestRoadRight.transform.position.z), Quaternion.Euler(road1_Back_Rotation));
            buildingQueue.Enqueue(latestBuilding);
            buildingCount++;
        }
        pendingTRoad = true;
        count++;
        straightCount = 0;
        ChangeRoot();
    }

    public void SpawnBuildings(GameObject type)
    {
        if (currentDirection == Directions.Front)
        {
            if(type.gameObject.name.Substring(0, 1) == "1")
            {
                latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoad.transform.position.x - offset, latestRoad.transform.position.y, latestRoad.transform.position.z), Quaternion.Euler(road1_Front_Rotation));
                buildingQueue.Enqueue(latestBuilding);
                latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoad.transform.position.x + offset, latestRoad.transform.position.y, latestRoad.transform.position.z), Quaternion.Euler(road1_Back_Rotation));
                buildingQueue.Enqueue(latestBuilding);
                buildingCount = buildingCount + 2;
            }
            else
            {
                latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length - 1)], new Vector3(latestRoad.transform.position.x, latestRoad.transform.position.y, latestRoad.transform.position.z + offset), Quaternion.Euler(road1_Right_Rotation));
                buildingQueue.Enqueue(latestBuilding);
                buildingCount++;
            }
        }
        else if (currentDirection == Directions.Left)
        {
            Vector3 newLocation = new Vector3(latestRoad.transform.position.x - offset, latestRoad.transform.position.y, latestRoad.transform.position.z);
            if (type.gameObject.name.Substring(0, 1) == "1")
            {
                latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoad.transform.position.x, latestRoad.transform.position.y, latestRoad.transform.position.z - offset), Quaternion.Euler(road1_Left_Rotation));
                buildingQueue.Enqueue(latestBuilding);
                latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoad.transform.position.x, latestRoad.transform.position.y, latestRoad.transform.position.z + offset), Quaternion.Euler(road1_Right_Rotation));
                buildingQueue.Enqueue(latestBuilding);
                buildingCount = buildingCount + 2;
            }
            else
            {
                latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoad.transform.position.x - offset, latestRoad.transform.position.y, latestRoad.transform.position.z), Quaternion.Euler(road1_Front_Rotation));
                buildingQueue.Enqueue(latestBuilding);
                buildingCount++;
            }
        }
        else if (currentDirection == Directions.Right)
        {
            Vector3 newLocation = new Vector3(latestRoad.transform.position.x - offset, latestRoad.transform.position.y, latestRoad.transform.position.z);
            if (type.gameObject.name.Substring(0, 1) == "1")
            {
                latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoad.transform.position.x, latestRoad.transform.position.y, latestRoad.transform.position.z - offset), Quaternion.Euler(road1_Left_Rotation));
                buildingQueue.Enqueue(latestBuilding);
                latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoad.transform.position.x, latestRoad.transform.position.y, latestRoad.transform.position.z + offset), Quaternion.Euler(road1_Right_Rotation));
                buildingQueue.Enqueue(latestBuilding);
                buildingCount = buildingCount + 2;
            }
            else
            {
                latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoad.transform.position.x + offset, latestRoad.transform.position.y, latestRoad.transform.position.z), Quaternion.Euler(road1_Back_Rotation));
                buildingQueue.Enqueue(latestBuilding);
                buildingCount++;
            }
        }
        else
        {
            Vector3 newLocation = new Vector3(latestRoad.transform.position.x - offset, latestRoad.transform.position.y, latestRoad.transform.position.z);
            if (type.gameObject.name.Substring(0, 1) == "1")
            {
                latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoad.transform.position.x - offset, latestRoad.transform.position.y, latestRoad.transform.position.z), Quaternion.Euler(road1_Front_Rotation));
                buildingQueue.Enqueue(latestBuilding);
                latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoad.transform.position.x + offset, latestRoad.transform.position.y, latestRoad.transform.position.z), Quaternion.Euler(road1_Back_Rotation));
                buildingQueue.Enqueue(latestBuilding);
                buildingCount = buildingCount + 2;
            }
            else
            {
                latestBuilding = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3(latestRoad.transform.position.x, latestRoad.transform.position.y, latestRoad.transform.position.z - offset), Quaternion.Euler(road1_Left_Rotation));
                buildingQueue.Enqueue(latestBuilding);
                buildingCount++;
            }
        }
    }

    public Vector3 NewLocation(Directions dir)
    {
        if (dir == Directions.Front)
        {
            Vector3 newLocation = new Vector3(latestRoad.transform.position.x, latestRoad.transform.position.y, latestRoad.transform.position.z + offset);
            return newLocation;
        }
        else if (dir == Directions.Left)
        {
            Vector3 newLocation = new Vector3(latestRoad.transform.position.x - offset, latestRoad.transform.position.y, latestRoad.transform.position.z);
            return newLocation;
        }
        else if (dir == Directions.Right)
        {
            Vector3 newLocation = new Vector3(latestRoad.transform.position.x + offset, latestRoad.transform.position.y, latestRoad.transform.position.z);
            return newLocation;
        }
        else
        {
            Vector3 newLocation = new Vector3(latestRoad.transform.position.x, latestRoad.transform.position.y, latestRoad.transform.position.z - offset);
            return newLocation;
        }
    }

    public void MapBuilder()
    {
        int rndAmt = Random.Range(2, 4);

        while(straightCount < rndAmt)
        {
            SpawnStraightRoad();
        }
        SpawnTRoad();
        MapCleaner();
    }

    public void MapCleaner()
    {
        while(count > 8)
        {
            Destroy(roadQueue.Dequeue());
            count--;
        }
    }

    public void BuildingCleaner()
    {
        while (buildingCount > count * 2)
        {
            Destroy(buildingQueue.Dequeue());
            buildingCount--;
        }
    }

    public void ChangePosition(Positions pos)
    {
        if(currentDirection == Directions.Front)
        {
            if(pos == Positions.Left)
            {
                transform.position = new Vector3(transform.position.x - 4f, transform.position.y, transform.position.z);
            }
            else if(pos == Positions.Center)
            {

            }
            else if(pos == Positions.Right)
            {

            }
        }
    }

    public void ChangeRoot()
    {
        root = latestRoad.transform.position;
    }

    public void Turn(Directions dir)
    {
        if (dir == Directions.Left)
        {
            if (currentDirection == Directions.Front)
            {
                currentDirection = Directions.Left;
            }
            else if (currentDirection == Directions.Left)
            {
                currentDirection = Directions.Back;
            }
            else if (currentDirection == Directions.Back)
            {
                currentDirection = Directions.Right;
            }
            else if (currentDirection == Directions.Right)
            {
                currentDirection = Directions.Front;
            }
            latestRoad = latestRoadLeft;
            StartCoroutine("Rotate", Directions.Left);
        }
        else if (dir == Directions.Right)
        {
            if (currentDirection == Directions.Front)
            {
                currentDirection = Directions.Right;
            }
            else if (currentDirection == Directions.Left)
            {
                currentDirection = Directions.Front;
            }
            else if (currentDirection == Directions.Back)
            {
                currentDirection = Directions.Left;
            }
            else if (currentDirection == Directions.Right)
            {
                currentDirection = Directions.Back;
            }
            Debug.Log("THE CURRENT POS IS " + currentPosition);
            latestRoad = latestRoadRight;
            StartCoroutine("Rotate", Directions.Right);
        }
        BuildingCleaner();
        MapBuilder();
        pendingTurn = false;
    }

    IEnumerator Rotate(Directions dir)
    {
        float timeToStart = Time.time;
        float rotateSpeed = 8f;
        float alignSpeed = 20f;
        Vector3 originalPosition = transform.position;

        if (dir == Directions.Left) // Left Key
        {
            if (currentDirection == Directions.Left)
            {
                Vector3 startMarker = transform.position;
                Vector3 endMarker = new Vector3(transform.position.x, transform.position.y, root.z);

                float journeyLength = Vector3.Distance(startMarker, endMarker);

                while (-(360f - transform.eulerAngles.y) != man_Left_Rotation.y)
                {
                    float distCovered = (Time.time - timeToStart) * alignSpeed;
                    float fracJourney = distCovered / journeyLength;
                    float tempTime = Mathf.Lerp(0, 90, (Time.time - timeToStart) * rotateSpeed);

                    transform.rotation = Quaternion.Euler(0f, -tempTime, 0f); // Rotate Man
                    transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney); // Reposition Man

                    if (-(360f - transform.eulerAngles.y) == man_Left_Rotation.y)
                    {
                        AlignPlayer();
                    }
                    yield return null;
                }
            }
            else if (currentDirection == Directions.Back)
            {
                Vector3 startMarker = transform.position;
                Vector3 endMarker = new Vector3(root.x, transform.position.y, transform.position.z);

                float journeyLength = Vector3.Distance(startMarker, endMarker);

                while (-(transform.eulerAngles.y) != man_Back_Rotation.y)
                {
                    float distCovered = (Time.time - timeToStart) * alignSpeed;
                    float fracJourney = distCovered / journeyLength;
                    float tempTime = Mathf.Lerp(0, 90, (Time.time - timeToStart) * rotateSpeed);

                    transform.rotation = Quaternion.Euler(0f, -90f + -tempTime, 0f);
                    transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney); // Reposition Man

                    if (-(transform.eulerAngles.y) == man_Back_Rotation.y)
                    {
                        AlignPlayer();
                    }
                    yield return null;
                }
            }
            else if (currentDirection == Directions.Right)
            {
                Vector3 startMarker = transform.position;
                Vector3 endMarker = new Vector3(transform.position.x, transform.position.y, root.z);

                float journeyLength = Vector3.Distance(startMarker, endMarker);

                while ((transform.eulerAngles.y + -360f) != man_Right_Rotation.y)
                {
                    float distCovered = (Time.time - timeToStart) * alignSpeed;
                    float fracJourney = distCovered / journeyLength;
                    float tempTime = Mathf.Lerp(0, 90, (Time.time - timeToStart) * rotateSpeed);

                    transform.rotation = Quaternion.Euler(0f, -180f + -tempTime, 0f);
                    transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney); // Reposition Man

                    if ((transform.eulerAngles.y + -360f) == man_Right_Rotation.y)
                    {
                        AlignPlayer();
                    }
                    yield return null;
                }
            }
            else if (currentDirection == Directions.Front)
            {
                Vector3 startMarker = transform.position;
                Vector3 endMarker = new Vector3(root.x, transform.position.y, transform.position.z);

                float journeyLength = Vector3.Distance(startMarker, endMarker);
                float tempTime = 0;

                while (tempTime < 90f)
                {
                    float distCovered = (Time.time - timeToStart) * alignSpeed;
                    float fracJourney = distCovered / journeyLength;
                    tempTime = Mathf.Lerp(0, 90, (Time.time - timeToStart) * rotateSpeed);

                    transform.rotation = Quaternion.Euler(0f, 90f - tempTime, 0f);
                    transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney); // Reposition Man

                    if (tempTime == 90f)
                    {
                        AlignPlayer();
                    }
                    yield return null;
                }
            }
        }
        else if(dir == Directions.Right) // Right Key
        {

            if (currentDirection == Directions.Right)
            {
                Vector3 startMarker = transform.position;
                Vector3 endMarker = new Vector3(transform.position.x, transform.position.y, root.z);

                float journeyLength = Vector3.Distance(startMarker, endMarker);
                float tempTime = 0;

                while (tempTime < 90f)
                {
                    float distCovered = (Time.time - timeToStart) * alignSpeed;
                    float fracJourney = distCovered / journeyLength;
                    tempTime = Mathf.Lerp(0, 90, (Time.time - timeToStart) * rotateSpeed);

                    transform.rotation = Quaternion.Euler(0f, 0 + tempTime, 0f);
                    transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney); // Reposition Man

                    if (tempTime == 90f)
                    {
                        AlignPlayer();
                    }
                    yield return null;
                }
            }
            else if (currentDirection == Directions.Back)
            {
                Vector3 startMarker = transform.position;
                Vector3 endMarker = new Vector3(root.x, transform.position.y, transform.position.z);

                float journeyLength = Vector3.Distance(startMarker, endMarker);
                float tempTime = 0;

                while (tempTime < 90f)
                {
                    float distCovered = (Time.time - timeToStart) * alignSpeed;
                    float fracJourney = distCovered / journeyLength;
                    tempTime = Mathf.Lerp(0, 90, (Time.time - timeToStart) * rotateSpeed);

                    transform.rotation = Quaternion.Euler(0f, 90f + tempTime, 0f);
                    transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney); // Reposition Man

                    if (tempTime == 90f)
                    {
                        AlignPlayer();
                    }
                    yield return null;
                }
            }
            else if (currentDirection == Directions.Left)
            {
                Vector3 startMarker = transform.position;
                Vector3 endMarker = new Vector3(transform.position.x, transform.position.y, root.z);

                float journeyLength = Vector3.Distance(startMarker, endMarker);
                float tempTime = 0;

                while (tempTime < 90f)
                {
                    float distCovered = (Time.time - timeToStart) * alignSpeed;
                    float fracJourney = distCovered / journeyLength;
                    tempTime = Mathf.Lerp(0, 90, (Time.time - timeToStart) * rotateSpeed);

                    transform.rotation = Quaternion.Euler(0f, 180f + tempTime, 0f);
                    transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney); // Reposition Man

                    if (tempTime == 90f)
                    {
                        AlignPlayer();
                    }
                    yield return null;
                }
            }
            else if (currentDirection == Directions.Front)
            {
                Vector3 startMarker = transform.position;
                Vector3 endMarker = new Vector3(root.x, transform.position.y, transform.position.z);

                float journeyLength = Vector3.Distance(startMarker, endMarker);
                float tempTime = 0;

                while (tempTime < 90f)
                {
                    float distCovered = (Time.time - timeToStart) * alignSpeed;
                    float fracJourney = distCovered / journeyLength;
                    tempTime = Mathf.Lerp(0, 90, (Time.time - timeToStart) * rotateSpeed);

                    transform.rotation = Quaternion.Euler(0f, 270f + tempTime, 0f);
                    transform.position = Vector3.Lerp(startMarker, endMarker, fracJourney); // Reposition Man

                    if (tempTime == 90f)
                    {
                        AlignPlayer();
                    }
                    yield return null;
                }
            }
        }
    }

    public void AlignPlayer()
    {
        if (currentDirection == Directions.Front || currentDirection == Directions.Back)
        {
            if (currentPosition == Positions.Center)
            {
                transform.position = new Vector3(root.x, transform.position.y, transform.position.z);
            }
            else if (currentPosition == Positions.Left)
            {
                transform.position = new Vector3(root.x - 4f, transform.position.y, transform.position.z);
            }
            else if (currentPosition == Positions.Right)
            {
                transform.position = new Vector3(root.x + 4f, transform.position.y, transform.position.z);
            }
        }
        else if (currentDirection == Directions.Left || currentDirection == Directions.Right)
        {
            if (currentPosition == Positions.Center)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, root.z);
            }
            else if (currentPosition == Positions.Left)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, root.z - 4f);
            }
            else if (currentPosition == Positions.Right)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, root.z + 4f);
            }
        }
        Debug.Log("Root is " + root);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "RoadMarker")
        {
            if(other.gameObject.name == "TRoad")
            {
                pendingTurn = true;
                pendingTRoad = false;
            }
        }
    }

}
