using System.Collections.Generic;
using UnityEngine;

namespace MiniGame9
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private List<Sprite> spriteImg;
        [SerializeField] private List<Sprite> spriteTitle;
        [SerializeField] private CardController cardPrefab;
        [SerializeField] private Transform cardParent;

        [SerializeField] private Sprite backSprite;

        private CardController firstCard;
        private CardController secondCard;
        private int pairLeft;

        void Start()
        {
            SetupCards();
        }

        void SetupCards()
        {
            List<CardData> temp = new List<CardData>();

            for (int i = 0; i < spriteImg.Count; i++)
            {
                temp.Add(new CardData(i, spriteImg[i]));
                temp.Add(new CardData(i, spriteTitle[i]));
            }

            Shuffle(temp);

            foreach (var c in temp)
            {
                var card = Instantiate(cardPrefab, cardParent);
                card.id = c.id;
                card.frontSprite = c.sprite;
                card.backSprite = backSprite;
            }

            pairLeft = spriteImg.Count; 
        }

        public void OnCardClicked(CardController card)
        {
            if (firstCard == null)
            {
                firstCard = card;
                card.FlipOpen();
            }
            else if (secondCard == null && card != firstCard)
            {
                secondCard = card;
                card.FlipOpen();
                Invoke(nameof(CheckMatch), 0.6f);
            }
        }

        private void CheckMatch()
        {
            if (firstCard.id == secondCard.id)
            {
                firstCard.HideCard();
                secondCard.HideCard();

                pairLeft--;
                if (pairLeft <= 0)
                {
                    Debug.Log("YOU WIN!");
                }
            }
            else
            {
                firstCard.FlipClose();
                secondCard.FlipClose();
            }

            firstCard = null;
            secondCard = null;
        }

        void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rnd = Random.Range(i, list.Count);
                (list[i], list[rnd]) = (list[rnd], list[i]);
            }
        }
    }

    [System.Serializable]
    public class CardData
    {
        public int id;
        public Sprite sprite;

        public CardData(int id, Sprite sprite)
        {
            this.id = id;
            this.sprite = sprite;
        }
    }

}
