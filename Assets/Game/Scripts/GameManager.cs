using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SoilController;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject hud;
    public GameObject mainMenu;

    public Image selectedIcon;
    public Sprite seedIcon;

    public TextMeshProUGUI totalSeedsUI;

    public TextMeshProUGUI totalRosesUI;
    public TextMeshProUGUI totalPoppiesUI;
    public TextMeshProUGUI totalHibiscusUI;

    public GameObject poppyUI;
    public GameObject hibiscusUI;

    public FlowerUI currentSelection;

    public enum GameStates { mainmenu, play };
    public GameStates currentState;
    public GameStates lastState;

    public int matureRoses;
    public int maturePoppies;
    public int matureHibiscus;

    public int totalSeeds = 0;

    public int roseCost = 5;
    public int poppyCost = 15;
    public int hibiscusCost = 35;

    public bool poppiesUnlocked;
    public bool hibiscusUnlocked;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        matureRoses = 0;
        maturePoppies = 0;
        matureHibiscus = 0;

        totalSeeds = 5;

        poppiesUnlocked = false;
        hibiscusUnlocked = false;

        poppyUI.SetActive(false);
        hibiscusUI.SetActive(false);

        selectedIcon.sprite = seedIcon;

        mainMenu.SetActive(true);
        hud.SetActive(false);
    }

    private void Update()
    {
        // Game Loop
        if (currentState == GameStates.play) 
        {            
            GameLoop();
        } 
        else
        {
            Time.timeScale = 0;
        }

        // Special Keys
        if (Input.GetKeyUp(KeyCode.P))
        {
            if (currentState == GameStates.play)
            {
                PauseGame();
            }
            else
            {
                PlayGame();
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (currentState != GameStates.mainmenu)
            {
                PauseGame();
            }
            else
            {
                CloseGame();
            }
        }        
    }

    void GameLoop()
    {
        // UI Updates
        totalSeedsUI.text = totalSeeds.ToString();
        totalRosesUI.text = matureRoses.ToString();

        if (poppiesUnlocked)
        {
            totalPoppiesUI.text = maturePoppies.ToString();
        } else
        {
            totalPoppiesUI.text = "Grow 5 Roses";
        }

        if (hibiscusUnlocked)
        {
            totalHibiscusUI.text = matureHibiscus.ToString();
        } else
        {
            totalHibiscusUI.text = "Grow 5 Poppies";
        }

        // Game Conditions
        UpgradeConditions();
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        if (currentState == GameStates.mainmenu)
        {
            lastState = currentState;
            currentState = GameStates.play;
            Time.timeScale = 1;
            mainMenu.SetActive(false);
            hud.SetActive(true);
        }
    }

    public void PauseGame()
    {
        if (currentState == GameStates.play)
        {
            lastState = currentState;
            currentState = GameStates.mainmenu;
            Time.timeScale = 0;
            mainMenu.SetActive(true);
            hud.SetActive(false);
        }
    }

    public void IncrementFlower(SoilController.FlowerType flower)
    {
        switch (flower)
        {
            case FlowerType.rose:
                {
                    matureRoses++;
                    break;
                }

            case FlowerType.poppy:
                {
                    maturePoppies++;
                    break;
                }

            case FlowerType.hibiscus:
                {
                    matureHibiscus++;
                    break;
                }

            default: { break; }
        }
    }

    public void IncrementSeeds()
    {
        totalSeeds++;
    }

    void UpgradeConditions()
    {
        if(matureRoses >= 5 && !poppiesUnlocked)
        {
            Debug.Log("Poppy Condition Met, Poppies Unlocked");
            poppiesUnlocked = true;
            poppyUI.SetActive(true);
        }

        if(maturePoppies >= 5 && !hibiscusUnlocked)
        {
            Debug.Log("Hibiscus Condition Met, Hibiscus Unlocked");
            hibiscusUnlocked = true;
            hibiscusUI.SetActive(true);
        }
    }
}
