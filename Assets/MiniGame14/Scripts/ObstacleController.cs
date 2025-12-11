using UnityEngine;
using UnityEngine.UI;

namespace MiniGame14
{
    public class ObstacleController : MonoBehaviour
    {
        [SerializeField] private int type = 0;
        float speed;

        private RectTransform rect;
        private Image img;

        public void Init(float s)
        {
            speed = s;
            rect = GetComponent<RectTransform>();
            img = GetComponent<Image>();
        }

        void Update()
        {
            if(GameManager.Instance.IsGameOver()) return;
            rect.anchoredPosition += Vector2.down * speed * Time.deltaTime;

            if (rect.anchoredPosition.y < -1200f)
                Destroy(gameObject);
        }

        public int GetObstacleType()
        {
            return type;
        }

        public void SetObstacleType(int t, Sprite sprite)
        {
            type = t;
            if (img == null) img = GetComponent<Image>();
            img.sprite = sprite;
        }
    }

}
