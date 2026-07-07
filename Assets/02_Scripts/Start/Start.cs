using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionPanel;

    private void Awake()
    {
        mainMenuPanel.SetActive(true);
        optionPanel.SetActive(false);
    }

    public void StartButton()
    {
        SceneManager.LoadScene("PipePuzzleRoom");
    }

    public void OpenOption()
    {
        mainMenuPanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void CloseOption()
    {
        optionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}