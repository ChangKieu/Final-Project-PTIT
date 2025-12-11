using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectLocation : MonoBehaviour
{
    [System.Serializable]
    public class LocationData
    {
        public string title;
        public string description;
    }

    [SerializeField] private LocationData[] locationData;
    [SerializeField] private Transform listDescription;

    private Transform[] items;

    private void Start()
    {
        int childCount = transform.childCount;
        items = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            Transform item = transform.GetChild(i);
            items[i] = item;

            listDescription.GetChild(i).GetChild(1).GetComponent<Text>().text = locationData[i].title;
            listDescription.GetChild(i).GetChild(2).GetComponent<Text>().text = locationData[i].description;

            int index = i; 
            item.GetComponent<Button>().onClick.AddListener(() => OnItemClicked(index));
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                DeactivateAll();
                return;
            }

            if (!ClickedOnAnyItem())
            {
                DeactivateAll();
            }
        }
    }
    private void OnItemClicked(int index)
    {
        for (int i = 0; i < listDescription.childCount; i++)
        {
            listDescription.GetChild(i).gameObject.SetActive(i == index);
        }
    }

    private void DeactivateAll()
    {
        for (int i = 0; i < listDescription.childCount; i++)
        {
            listDescription.GetChild(i).gameObject.SetActive(false);
        }
    }

    private bool ClickedOnAnyItem()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var r in results)
        {
            foreach (Transform item in items)
            {
                if (r.gameObject.transform.IsChildOf(item))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
