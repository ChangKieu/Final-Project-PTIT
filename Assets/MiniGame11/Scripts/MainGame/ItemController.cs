using UnityEngine;
namespace MiniGame11
{
    public class ItemController : MonoBehaviour
    {
        [SerializeField] private int id;

        public int GetID()
        {
            return id;
        }
        public Vector3 GetPosition()
        {
            return transform.position;
        }
        public void SetOriginalPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetSize(Vector3 size)
        {
            transform.localScale = size;
        }

        public void SetDoneSprite()
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }

    }

}
