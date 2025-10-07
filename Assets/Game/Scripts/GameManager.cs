using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SoilController;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Space]

    public Camera gameCam;
    public Vector3 defaultCameraPosition;
    [Range(0f, 1f)]
    public float cameraSpeed = .5f;
    public int smallestCamSize = 3;
    public int largestCamSize = 5;
    public int defaultCamSize = 4;

    public int cameraYLimit = 2;
    public int cameraXLimit = 2;

    [Space]

    public GameObject hud;
    public GameObject mainMenu;

    public GameObject overviewText;
    public GameObject controlsText;

    public Image selectedIcon;
    public Sprite seedIcon;

    public TextMeshProUGUI totalSeedsUI;

    public TextMeshProUGUI totalRosesUI;
    public TextMeshProUGUI totalPoppiesUI;
    public TextMeshProUGUI totalHibiscusUI;

    public TextMeshProUGUI costRosesUI;
    public TextMeshProUGUI costPoppiesUI;
    public TextMeshProUGUI costHibiscusUI;

    public GameObject poppyUI;
    public GameObject hibiscusUI;

    public FlowerUI currentSelection;

    public enum GameStates { mainmenu, play };
    public GameStates currentState;
    public GameStates lastState;

    [Space]

    public int matureRoses;
    public int maturePoppies;
    public int matureHibiscus;

    public int totalSeeds = 0;

    public int roseCost = 5;
    public int poppyCost = 15;
    public int hibiscusCost = 35;

    float seedTimer = 0;
    public float seedSpeed = 3;

    public int roseSeeds = 1;
    public int poppySeeds = 3;
    public int hibiscusSeeds = 7;

    [Space]

    public bool poppiesUnlocked;
    public bool hibiscusUnlocked;

    public bool CameraDistanceUpgrade1;
    public bool CameraDistanceUpgrade2;
    public bool CameraDistanceUpgrade3;

    public bool CameraSpeedUpgrade1;


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
        gameCam = Camera.main;
        defaultCameraPosition = gameCam.transform.position;
        gameCam.orthographicSize = defaultCamSize;

        matureRoses = 0;
        maturePoppies = 0;
        matureHibiscus = 0;

        costRosesUI.text = "Cost: " + roseCost + " seeds";
        costPoppiesUI.text = "Cost: " + poppyCost + " seeds";
        costHibiscusUI.text = "Cost: " + hibiscusCost + " seeds";

        totalSeeds = 5;

        poppiesUnlocked = false;
        hibiscusUnlocked = false;

        poppyUI.SetActive(false);
        hibiscusUI.SetActive(false);

        selectedIcon.sprite = seedIcon;

        mainMenu.SetActive(true);
        hud.SetActive(false);

        ShowOverview();
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

        // Seed Counter
        if (seedTimer < seedSpeed)
        {
            seedTimer += 1 * Time.deltaTime;
        } 
        else
        {
            IncrementSeeds();
            seedTimer = 0;
        }

        // Camera Controls
        if (Input.GetKey(KeyCode.Mouse1))
        {
            gameCam.transform.position = Vector3.Lerp(gameCam.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), cameraSpeed * Time.deltaTime);
            gameCam.transform.position = new Vector3(
                Mathf.Clamp(gameCam.transform.position.x, defaultCameraPosition.x - cameraXLimit, defaultCameraPosition.x + cameraXLimit),
                Mathf.Clamp(gameCam.transform.position.y, defaultCameraPosition.y - cameraYLimit, defaultCameraPosition.y + cameraYLimit),
                gameCam.transform.position.z
                );
            //gameCam.transform.position += Vector3.Normalize(Camera.main.ScreenToWorldPoint(Input.mousePosition)) * cameraSpeed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            gameCam.transform.position = defaultCameraPosition;
            gameCam.orthographicSize = defaultCamSize;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            // Scroll up
            Debug.Log("Scrolling up " + scroll);
            gameCam.orthographicSize += scroll;
            gameCam.orthographicSize = Mathf.Clamp(gameCam.orthographicSize, smallestCamSize, largestCamSize);
        }
        else if (scroll < 0f)
        {
            // Scroll down
            Debug.Log("Scrolling down " + scroll);
            gameCam.orthographicSize += scroll;
            gameCam.orthographicSize = Mathf.Clamp(gameCam.orthographicSize, smallestCamSize, largestCamSize);
        }

        // Game Conditions
        UpgradeConditions();
    }

    public void ShowOverview()
    {
        overviewText.SetActive(true);
        controlsText.SetActive(false);
    }

    public void ShowControls()
    {
        overviewText.SetActive(false);
        controlsText.SetActive(true);
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
            ShowOverview();
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
        // Roses
        totalSeeds += matureRoses * roseSeeds;

        // Poppies
        totalSeeds += maturePoppies * poppySeeds;

        // Hibiscus
        totalSeeds += matureHibiscus * hibiscusSeeds;
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

        // Upgrade Camera View
        if(totalSeeds > 500 && !CameraDistanceUpgrade1)
        {
            cameraXLimit += 2;
            cameraYLimit += 2;
            CameraDistanceUpgrade1 = true;
        }
        else if (totalSeeds > 1000 && !CameraDistanceUpgrade2)
        {
            cameraXLimit += 2;
            cameraYLimit += 2;
            CameraDistanceUpgrade2 = true;
        }
        else if (totalSeeds > 2000 && !CameraDistanceUpgrade3)
        {
            cameraXLimit += 2;
            cameraYLimit += 2;
            CameraDistanceUpgrade3 = true;
        }

        // Upgrade Camera Speed
        if (matureHibiscus > 25 && !CameraSpeedUpgrade1)
        {
            cameraSpeed += .2f;
            CameraSpeedUpgrade1 = true;
        }
    }
}
