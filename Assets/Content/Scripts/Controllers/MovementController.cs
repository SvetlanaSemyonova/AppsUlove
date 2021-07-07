using UnityEngine;

namespace Content.Scripts.Controllers
{
    public class MovementController : MonoBehaviour
    {
        [SerializeField]
        private float speed = 4f;
    
        private Vector3 target;
        
        private void Start() 
        {
            target = transform.position;
        }

        private void Update() 
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                target.z = transform.position.z;
            
                var distance = Vector2.Distance (target, transform.position);
                PlayerData.Instance.distance.Value += Mathf.Round(distance);
                PlayerData.Instance.distance.Save();
            }
        
            if (IsTargetReached())
            {
                transform.position = transform.position;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
            }
        }

        private bool IsTargetReached()
        {
            return Vector2.Distance (target, transform.position) <= 0;
        }
    }
}
