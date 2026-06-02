using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("PipePuzzleRoom");
    }
}
