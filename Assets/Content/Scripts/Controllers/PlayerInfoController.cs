using TMPro;
using UnityEngine;

namespace Content.Scripts.Controllers
{
    public class PlayerInfoController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreAmount;
        [SerializeField] private TextMeshProUGUI distanceAmount;

        private int score;
        private float distance;

        private void Start()
        {
            score = PlayerData.Instance.score;
            distance = PlayerData.Instance.distance;
            
            PlayerData.Instance.score.OnChanged += SetScoreAmount;
            PlayerData.Instance.distance.OnChanged += SetDistanceAmount;
            
            scoreAmount.text = score.ToString();
            distanceAmount.text = distance.ToString();
        }

        private void SetScoreAmount(int amountReceived)
        {
            scoreAmount.text = amountReceived.ToString();
        }
        
        private void SetDistanceAmount(float amountReceived)
        {
            distanceAmount.text = amountReceived.ToString();
        }

        private void OnDestroy()
        {
            PlayerData.Instance.score.OnChanged -= SetScoreAmount;
            PlayerData.Instance.distance.OnChanged -= SetDistanceAmount;
        }
    }
}