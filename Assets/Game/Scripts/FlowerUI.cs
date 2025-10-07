using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlowerUI : MonoBehaviour, IPointerDownHandler
{
    public SoilController.FlowerType myFlower;
    public Sprite[] sprites;

    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.instance.currentSelection = this;
        GameManager.instance.selectedIcon.sprite = GetComponent<Image>().sprite;

        Debug.Log("On PointerDown Success");
    }

}
