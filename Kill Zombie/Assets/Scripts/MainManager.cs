using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainManager : MonoBehaviour
{
    public GameObject[] fireWoodPrefabs;
    public Button quitButton;
    public Text levelText;
    public Text startCountText;
    public Text gameOverText;
    public Text playerWinText;
    public Text nameText;

    private CameraHandler swichCamera;

    public bool isLevelUp;
    public bool isGameActive = false;
    public bool isFireWoodDestroyed;
    public bool isHitByBullet;
    public string currentCamera;

    public int timer = 0;
    public int level;
    public int killCount;
    public int zombieCount;
    public float spawnZombieRate;

    public Vector3 fireWoodPosition;
    public Vector3[] randomPosition = new[] {new Vector3(0, 0.38f, -11.358f), new Vector3(0, 0.38f, 11.358f),
                                             new Vector3(6.15f, 0.38f, -12.21f), new Vector3(6.15f, 0.38f, 12.21f),
                                             new Vector3(-6.15f, 0.38f, -12.21f), new Vector3(-6.15f, 0.38f, 12.21f)};

    private int startCnt;

    private void Start()
    {
        swichCamera = GameObject.Find("Cameras").GetComponent<CameraHandler>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        nameText.text = GameManager.Instance.userName;

        isGameActive = false;
        startCnt = 3;
        timer = 0;
        level = 1;
        spawnZombieRate = 2.5f;
        levelText.text = "Level " + level;

        startCountText.gameObject.SetActive(true);
        StartCoroutine(StartCount());
    }

    public void GameStart()
    {
        isGameActive = true;
        SpawnFireWood();
        StartCoroutine(StartTimer());
    }

    IEnumerator StartCount()
    {
        while (startCnt > -1)
        {
            yield return new WaitForSeconds(1);

            startCountText.text = "" + startCnt;

            startCnt -= 1;

            if (startCnt == -1)
            {
                startCountText.gameObject.SetActive(false);
                GameStart();
            }
        }
    }

    IEnumerator StartTimer()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(1);

            timer += 1;
        }
    }

    public void GameOver()
    {
        isGameActive = false;
        gameOverText.gameObject.SetActive(true);
    }

    public void PlayerWin()
    {
        isGameActive = false;
        playerWinText.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void ClickQuitButton()
    {
        GameManager.Instance.SaveScore(timer);

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    void SpawnFireWood()
    {
        killCount = 0;
        zombieCount = 0;
        isFireWoodDestroyed = false;
        fireWoodPosition = randomPosition[Random.Range(0, randomPosition.Length)];
        Instantiate(fireWoodPrefabs[level - 1], fireWoodPosition, fireWoodPrefabs[level - 1].transform.rotation);
    }

    public void UpdateBodyCount()
    {
        killCount += 1;
        Debug.Log("You killed : " + killCount + ", Spawned Zombie : " + zombieCount);
    }

    public void UpdateLevel()
    {
        if (killCount == zombieCount && isFireWoodDestroyed)
        {
            level += 1;
            spawnZombieRate -= 0.5f;


            if (level == 6)
            {
                PlayerWin();
                Debug.Log("You Win!");

            }
            else if (level < 6)
            {
                levelText.text = "Level " + level;
                isLevelUp = true;
                swichCamera.ActivateThirdPersonCamera();
                SpawnFireWood();
            }
        }
    }
}
