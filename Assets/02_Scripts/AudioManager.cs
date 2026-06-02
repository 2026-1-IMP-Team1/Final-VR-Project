using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource pipePuzzleSlotSound;
    [SerializeField] private AudioSource pipePuzzleRotateSound;

    public void PlayPipePuzzleSlotSound()
    {
        pipePuzzleSlotSound.Play();
    }

    public void PlayPipePuzzleRotateSound()
    {
        pipePuzzleRotateSound.Play();
    }
}
