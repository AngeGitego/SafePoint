using UnityEngine;

public class UIFlowManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject homePanel;
    public GameObject arPanel;
    public GameObject hazardFormPanel;

    public void ShowHome()
    {
        homePanel.SetActive(true);
        arPanel.SetActive(false);
        hazardFormPanel.SetActive(false);
    }

    public void ShowAR()
    {
        homePanel.SetActive(false);
        arPanel.SetActive(true);
        hazardFormPanel.SetActive(false);
    }

    public void ShowHazardForm()
    {
        homePanel.SetActive(false);
        arPanel.SetActive(false);
        hazardFormPanel.SetActive(true);
    }
}