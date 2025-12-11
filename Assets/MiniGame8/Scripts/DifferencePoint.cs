using UnityEngine;
using UnityEngine.UI;

namespace MiniGame8
{
    public class DifferencePoint : MonoBehaviour
    {
        private bool found = false;

        [HideInInspector] public DifferencePoint pair;

        private Image highlight;
        private void Awake()
        {
            highlight = GetComponent<Image>();
        }

        public void Init()
        {
            found = false;
            highlight.color = new Color(1, 1, 1, 0); 
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        public void OnClick()
        {
            if (found) return;

            Reveal();
            pair.Reveal();

            GameManager.Instance.FoundPoint();
        }

        public void Reveal()
        {
            if (found) return;

            found = true;

            highlight.color = new Color(1, 1, 1, 1);
        }
    }


}
