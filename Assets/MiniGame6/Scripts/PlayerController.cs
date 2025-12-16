using UnityEngine;
namespace MiniGame6
{
    public class PlayerController : MonoBehaviour
    {
        public bool isMoving = false;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private Vector3 direction = Vector3.right;
        [SerializeField] private GameObject loseEffect;

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
            Vector3 hitPoint = collision.contacts[0].point;
            Instantiate(loseEffect, hitPoint, Quaternion.identity);
            GameManager.Instance.Lose();
        }
    }

}
