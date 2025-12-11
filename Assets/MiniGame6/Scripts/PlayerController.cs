using UnityEngine;
namespace MiniGame6
{
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 4f;
        public bool isMoving = false;
        public Vector3 direction = Vector3.right;

        private void Update()
        {
            if (!GameManager.Instance.IsGamePlaying()) return;
            if (isMoving)
            {
                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            GameManager.Instance.Lose();
        }
    }

}
