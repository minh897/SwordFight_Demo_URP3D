using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text winText;

    private bool isGamePause = false;
    private bool isRestart = false;
    private string currentScene;

    void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        Debug.Log("Active scene "+currentScene);
    }

    void Update()
    {
        if (isRestart)
        {
            isRestart = false;
            Restart();
        }
        if (isGamePause)
        {
            Debug.Log("Game is pause");
            var restartInput = InputSystem.actions.FindActionMap("UI").FindAction("Restart");
            if (restartInput.WasPressedThisFrame())
            {
                isRestart = true;
            }
        }
    }

    public void CheckWinLoseCondition(GameObject obj)
    {
        if (obj.CompareTag("Enemy"))
        {
            Debug.Log("Check win condition");
            // pause the game
            PauseGame();
            // display win text
            winText.transform.parent.gameObject.SetActive(true);
            // enable UI input action
            InputSystem.actions.FindActionMap("UI").Enable();
            Debug.Log("UI action is " + InputSystem.actions.FindActionMap("UI").enabled);
        }

        if (obj.CompareTag("Player"))
        {
            Debug.Log("Check lose condition");
        }
    }

    private void Restart()
    {
        Debug.Log("Restart game");
        ResumeGame();
        SceneManager.LoadScene(currentScene);
    }

    private void PauseGame()
    {
        isGamePause = true;
        Time.timeScale = 0;
    }
    
    private void ResumeGame()
    {
        isGamePause = false;
        Time.timeScale = 1;
    }
}
