// Controls the spawning of groceries in level queues as well as specific level features like the conveyor speed and height limit
using UnityEngine;

public class GrocerySpawner : MonoBehaviour
{
    public GameObject[] groceryPrefabs;
    public Transform spawnPoint;
    public Transform groceriesParent;
    public float spawnInterval = 3f; // Interval between groceries spawning
    public float fasterSpeedMultiplier = 4f; // Multiplier for the conveyor speed after Level 3
    public float normalHeightLimitY = 2f; // Height limit of the yellow dotted line
    public float loweredHeightLimitY = 0.5f; // Lowered height limit of the yellow dotted line for levels 6, 8, and 10
    public float normalBagZoneY = -1.43f; // Zone in which groceries are successfully bagged
    public float loweredBagZoneY = -2.93f; // Lowered zone in which groceries are successfully bagged for levels 6, 8, and 10
    private int[][] levelQueues = // Predetermined order of groceries spawning for each level where every grocery item is assigned a unique number in Unity and called here
    {
        new int[] {0, 1, 2, 0, 1, 1, 0, 2, 0, 0, 1, 2}, // Level 1
        new int[] {3, 0, 2, 2, 1, 4, 0, 4, 4, 3, 1, 0, 2, 4}, // Level 2
        new int[] {5, 4, 3, 1, 1, 0, 0, 0, 2, 3, 2, 5, 4, 5, 0}, // Level 3
        new int[] {1, 6, 2, 4, 0, 4, 3, 5, 0, 3, 6, 4, 0, 1}, // Level 4
        new int[] {6, 5, 7, 1, 2, 0, 5, 7, 6, 3, 1, 4, 1, 4, 4, 0, 0}, // Level 5
        new int[] {8, 0, 4, 8, 8, 0, 5, 4, 0}, // Level 6
        new int[] {9, 3, 1, 2, 3, 6, 4, 7, 1, 3, 1, 5, 5, 4}, // Level 7
        new int[] {2, 2, 8, 0, 10, 5, 6, 10, 7, 7}, // Level 8
        new int[] {11, 12, 0, 0, 6, 3, 9, 4, 11, 1, 0, 10, 12, 7, 2}, // Level 9
        new int[] {10, 4, 2, 10, 8, 11, 3, 7, 4, 5} // Level 10
    };
    private int currentItemIndex = 0;
    private int currentLevel = 0;
    public bool HasFinishedSpawning => currentItemIndex >= levelQueues[currentLevel].Length; // Tracks whether all groceries in the level were spawned
    public int TotalItemsInLevel => levelQueues[currentLevel].Length; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() 
    {
        StartLevel(1); // Starts the game with level 1
    }
    // Starts the current level
    public void StartLevel(int levelNumber)
    {
        CancelInvoke(nameof(SpawnGrocery)); // Cancels all groceries spawning before starting the level
        // Deletes all groceries onscreen from the previous level
        for (int i = groceriesParent.childCount - 1; i >= 0; i--)
        {
            Destroy(groceriesParent.GetChild(i).gameObject);
        }
        BagZone bagZone = FindAnyObjectByType<BagZone>();
        bagZone.ResetBag(); // Resets the number of bagged groceries
        BagHeightLimit heightLimit = FindAnyObjectByType<BagHeightLimit>();
        heightLimit.ResetHeightLimit(); // Resets the height limit in case the last level had a lowered yellow dotted line
        currentLevel = levelNumber - 1;
        currentItemIndex = 0;
        Vector3 linePosition = heightLimit.transform.position;
        // Lowers the height limit for only levels 6, 8, and 10
        if (levelNumber == 6 || levelNumber == 8 || levelNumber == 10)
        {
            linePosition.y = loweredHeightLimitY;
        }
        else
        {
            linePosition.y = normalHeightLimitY;
        }
        heightLimit.transform.position = linePosition;
        BagZone bagZoneObject = FindAnyObjectByType<BagZone>();
        Vector3 bagZonePosition = bagZoneObject.transform.position;
        // Lowers the bagged zone for only levels 6, 8, and 10
        if (levelNumber == 6 || levelNumber == 8 || levelNumber == 10)
        {
            bagZonePosition.y = loweredBagZoneY;
        }
        else
        {
            bagZonePosition.y = normalBagZoneY;
        }
        bagZoneObject.transform.position = bagZonePosition;
        InvokeRepeating(nameof(SpawnGrocery), 1f, spawnInterval); // Delays 1 second before spawning groceries on the conveyor belt
    }
    // Spawns the next grocery in queue
    void SpawnGrocery()
    {
        // Cancels if all groceries in the current queue have finished spawning
        if (currentItemIndex >= levelQueues[currentLevel].Length)
        {
            CancelInvoke(nameof(SpawnGrocery));
            return;
        }
        int prefabIndex = levelQueues[currentLevel][currentItemIndex]; // Index of the next grocery in queue
        GameObject newGrocery = Instantiate(groceryPrefabs[prefabIndex], spawnPoint.position, Quaternion.identity, groceriesParent);
        // Upgrades the conveyor speed after Level 3
        if (currentLevel + 1 >= 4)
        {
            ConveyorItem item = newGrocery.GetComponent<ConveyorItem>();
            item.speed *= fasterSpeedMultiplier;
        }
        currentItemIndex++; // Moves to the next grocery in queue
    }
}
