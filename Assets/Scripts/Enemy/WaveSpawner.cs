using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 
public class WaveSpawner : MonoBehaviour
{
   [Header("Wave settings")]
    public List<WaveEnemy> enemies = new List<WaveEnemy>();
    public int currWave;
    private int waveValue;
    public List<GameObject> enemiesToSpawn = new List<GameObject>();
 
    [Header("Spawn locations")]
    public Transform[] spawnLocation;
    public int spawnIndex {get; private set;}
 
    public int waveDuration;
    public float waveTimer {get; private set;}
    public float spawnInterval;
    public float spawnTimer;
 
    public List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        GenerateWave();
    }
 
    void FixedUpdate()
    {
        if(spawnTimer <=0)
        {
            if(enemiesToSpawn.Count >0)
            {
                GameObject enemy = (GameObject)Instantiate(enemiesToSpawn[0], spawnLocation[spawnIndex].position,Quaternion.identity); // spawn first enemy in our list
                enemiesToSpawn.RemoveAt(0); // and remove it
                spawnedEnemies.Add(enemy);
                spawnTimer = spawnInterval;
 
                if(spawnIndex + 1 <= spawnLocation.Length-1)
                {
                    spawnIndex++;
                }
                else
                {
                    spawnIndex = 0;
                }
            }
            else
            {
                waveTimer = 0; // if no enemies remain, end wave
            }
        }
        else
        {
            spawnTimer -= Time.fixedDeltaTime;
            waveTimer -= Time.fixedDeltaTime;
        }

        spawnedEnemies = spawnedEnemies.Where(x => x != null).ToList();
 
        if(waveTimer<=0 && spawnedEnemies.Count <=0)
        {
            currWave++;
            GenerateWave();
        }
    }
 
    public void GenerateWave()
    {
        waveValue = currWave * 10;
        GenerateEnemies();
 
        spawnInterval = waveDuration / (enemiesToSpawn.Count + 1); // gives a fixed time between each enemies
        waveTimer = waveDuration; // wave duration is read only
    }
 
    public void GenerateEnemies()
    {
        // Create a temporary list of enemies to generate
        // 
        // in a loop grab a random enemy 
        // see if we can afford it
        // if we can, add it to our list, and deduct the cost.
 
        // repeat... 
 
        //  -> if we have no points left, leave the loop
 
        List<GameObject> generatedEnemies = new List<GameObject>();
        while(waveValue>0 || generatedEnemies.Count <50)
        {
            int randEnemyId = Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randEnemyId].cost;
 
            if(waveValue-randEnemyCost>=0)
            {
                generatedEnemies.Add(enemies[randEnemyId].enemyPrefab);
                waveValue -= randEnemyCost;
            }
            else if(waveValue<=0)
            {
                break;
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }
  
}
 
[System.Serializable]
public class WaveEnemy
{
    public GameObject enemyPrefab;
    public int cost;
}