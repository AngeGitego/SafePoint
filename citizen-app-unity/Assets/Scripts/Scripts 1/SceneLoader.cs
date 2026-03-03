using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadARScene()
    {
        SceneManager.LoadScene("ARScene");
    }
    public void LoadReportScene()
    {
        SceneManager.LoadScene("ReportDetailsScene");
    }
}
