using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Content.Scripts.Controllers
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private GameObject obstaclePrefab;
        
        [SerializeField]
        private int obstacleMaxNumber = 3;
        
        [SerializeField]
        private float spawnDelay = 2f;
        
        private bool isCoroutineStarted = false;

        void Start()
        {
            StartCoroutine(SpawnObstacle(obstacleMaxNumber, spawnDelay));
        }
        
        private void Update()
        {
            if (!isCoroutineStarted)
            {
                StartCoroutine(SpawnObstacle(obstacleMaxNumber, spawnDelay));
            }
        }

        private IEnumerator SpawnObstacle(int maxNumber, float delay)
        {
            isCoroutineStarted = true;
            if (GameSettings.spawnedObstacleNumber < obstacleMaxNumber)
            {
                isCoroutineStarted = true;
                var spawnY = Random.Range
                    (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
                var spawnX = Random.Range
                    (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);
     
                var spawnPosition = new Vector2(spawnX, spawnY);
                Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
                GameSettings.spawnedObstacleNumber += 1;
            }

            yield return new WaitForSeconds(delay);
            isCoroutineStarted = false;
        }
    }
}