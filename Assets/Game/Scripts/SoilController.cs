using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoilController : MonoBehaviour, IPointerDownHandler
{
    public SpriteRenderer spriteRenderer;

    public enum SoilState { soil, bud, young, mature };
    SoilState condition;

    public enum FlowerType { unassigned, rose, poppy, hibiscus };
    public FlowerType flower;

    public Sprite[] sprites;

    public int budTime = 10;
    public int youngTime = 25;

    float pulseMin = .7f;
    float pulseMax = .75f;
    float pulseTime = 1.5f;

    bool pulseRunning;

    public void StartGrowth()
    {
        condition++;

        StartCoroutine(Grow());
    }

    private void Update()
    {
        if (condition != SoilState.soil && !pulseRunning)
        {
            StartCoroutine(Pulse());
        }
    }

    IEnumerator Pulse()
    {
        if(spriteRenderer.gameObject.transform.localScale.x <= pulseMin)
        {
            pulseRunning = true;
            spriteRenderer.gameObject.transform.localScale = Vector3.Lerp(spriteRenderer.gameObject.transform.localScale, new Vector3(pulseMax, pulseMax, pulseMax), pulseTime);
            yield return new WaitForSeconds(pulseTime);
            pulseRunning = false;
        } 
        else if (spriteRenderer.gameObject.transform.localScale.x >= pulseMax)
        {
            pulseRunning = true;
            spriteRenderer.gameObject.transform.localScale = Vector3.Lerp(spriteRenderer.gameObject.transform.localScale, new Vector3(pulseMin, pulseMin, pulseMin), pulseTime);
            yield return new WaitForSeconds(pulseTime);
            pulseRunning = false;
        }
    }

    IEnumerator Grow()
    {
        switch (condition) { 
        case SoilState.bud:
                {
                    UpdateFlowerSprite();
                    yield return new WaitForSeconds(budTime);
                    StartGrowth();
                    break;
                }

        case SoilState.young:
                {
                    UpdateFlowerSprite();
                    yield return new WaitForSeconds(youngTime);
                    StartGrowth();
                    break;
                }

        case SoilState.mature:
                {
                    UpdateFlowerSprite();
                    Debug.Log("Mature Grown");
                    UpdateFlowerCount();
                    break;
                }

        default:
                {
                    break;
                }
        }

    }

    void UpdateFlowerSprite()
    {
        spriteRenderer.sprite = sprites[(int)condition - 1];
    }

    void UpdateFlowerCount()
    {
        switch (flower)
        {
            case FlowerType.rose:
                {
                    Debug.Log("Rose++");
                    GameManager.instance.IncrementFlower(FlowerType.rose);
                    break;
                }

            case FlowerType.poppy:
                {
                    Debug.Log("Poppy++");
                    GameManager.instance.IncrementFlower(FlowerType.poppy);
                    break;
                }

            case FlowerType.hibiscus:
                {
                    Debug.Log("Hibiscus++");
                    GameManager.instance.IncrementFlower(FlowerType.hibiscus);
                    break;
                }

            default: { break; }
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.instance.currentSelection != null && condition == SoilState.soil)
        {
            switch (GameManager.instance.currentSelection.myFlower)
            {
                case FlowerType.rose:
                    {
                        if (GameManager.instance.totalSeeds >= GameManager.instance.roseCost)
                        {
                            GameManager.instance.totalSeeds -= GameManager.instance.roseCost;

                            flower = GameManager.instance.currentSelection.myFlower;
                            sprites = GameManager.instance.currentSelection.sprites;

                            GameManager.instance.currentSelection = null;
                            GameManager.instance.selectedIcon.sprite = GameManager.instance.seedIcon;

                            StartGrowth();
                        }
                        break;
                    }
                case FlowerType.poppy:
                    {
                        if (GameManager.instance.totalSeeds >= GameManager.instance.poppyCost)
                        {
                            GameManager.instance.totalSeeds -= GameManager.instance.poppyCost;

                            flower = GameManager.instance.currentSelection.myFlower;
                            sprites = GameManager.instance.currentSelection.sprites;

                            GameManager.instance.currentSelection = null;
                            GameManager.instance.selectedIcon.sprite = GameManager.instance.seedIcon;

                            StartGrowth();
                        }
                        break;
                    }
                case FlowerType.hibiscus:
                    {
                        if (GameManager.instance.totalSeeds >= GameManager.instance.hibiscusCost)
                        {
                            GameManager.instance.totalSeeds -= GameManager.instance.hibiscusCost;

                            flower = GameManager.instance.currentSelection.myFlower;
                            sprites = GameManager.instance.currentSelection.sprites;

                            GameManager.instance.currentSelection = null;
                            GameManager.instance.selectedIcon.sprite = GameManager.instance.seedIcon;

                            StartGrowth();
                        }
                        break;
                    }
            }
        } 
        else
        {
            Debug.Log("Current Selection = null OR soil condition is not soil");
        }
    }
}
