using DG.Tweening;
using UnityEngine;

namespace MiniGame6
{
    public class ObstacleController : MonoBehaviour
    {
        public float moveSpeed = 4f;
        public float delay = 0f;
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

        private void OnCollisionEnter(Collision collision)
        {
            if (isMoving)
            {
                GameManager.Instance.Lose();
            }
        }

        public void StartMoving()
        {
            DOVirtual.DelayedCall(delay, () => {
                isMoving = true;
            });
        }
    }

}
