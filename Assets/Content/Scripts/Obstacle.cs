using UnityEngine;

namespace Content.Scripts
{
    public class Obstacle : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private int scoreAmountReceived;

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerData.Instance.score.Value += scoreAmountReceived;
            PlayerData.Instance.score.Save();
            GameSettings.spawnedObstacleNumber -= 1;
            Destroy(gameObject);
        }
    }
}