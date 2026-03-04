using UnityEngine;
using UnityEngine.SceneManagement;

public class RecordHelpUI : MonoBehaviour
{
    [SerializeField] private GameObject recordHelpPanel;
    [SerializeField] private GameObject continueButton;

    [SerializeField] private string reportSceneName = "ReportScene";

    void Start()
    {
        // Continue button hidden at start
        if (continueButton != null)
            continueButton.SetActive(false);
    }

    public void ShowRecordHelp()
    {
        if (recordHelpPanel != null)
            recordHelpPanel.SetActive(true);
    }

    public void HideRecordHelp()
    {
        if (recordHelpPanel != null)
            recordHelpPanel.SetActive(false);

        // After user clicks Done, show Continue button
        if (continueButton != null)
            continueButton.SetActive(true);
    }

    public void GoToReportScene()
    {
        SceneManager.LoadScene(reportSceneName);
    }
}