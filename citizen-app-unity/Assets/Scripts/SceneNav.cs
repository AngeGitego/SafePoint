using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNav : MonoBehaviour
{
    public void GoIntro() => SceneManager.LoadScene("IntroScene");
    public void GoAR() => SceneManager.LoadScene("ARScene");
    public void GoHowItWorks() => SceneManager.LoadScene("HowItWorksScene");
}