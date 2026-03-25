using TMPro;
using UnityEngine;

public class LocationDropdownAutoId : MonoBehaviour
{
    [Header("Dropdowns")]
    public TMP_Dropdown districtDropdown;
    public TMP_Dropdown sectorDropdown;
    public TMP_Dropdown cellDropdown;

    [Header("Output")]
    public TMP_Text cellIdText;                // if cellid is a TMP_Text
    public TMP_InputField cellIdInputField;    // if cellid is a TMP_InputField

    [Header("Options")]
    public bool useUppercase = true;
    public string separator = "-";

    private void Start()
    {
        UpdateCellId();
    }

    public void OnLocationDropdownChanged()
    {
        UpdateCellId();
    }

    private void UpdateCellId()
    {
        string district = GetSelectedOption(districtDropdown);
        string sector = GetSelectedOption(sectorDropdown);
        string cell = GetSelectedOption(cellDropdown);

        // If any dropdown is still empty / placeholder, do not build a fake ID
        if (string.IsNullOrWhiteSpace(district) ||
            string.IsNullOrWhiteSpace(sector) ||
            string.IsNullOrWhiteSpace(cell))
        {
            SetCellId("");
            return;
        }

        string finalId = district + separator + sector + separator + cell;

        if (useUppercase)
            finalId = finalId.ToUpper();

        SetCellId(finalId);
    }

    private string GetSelectedOption(TMP_Dropdown dropdown)
    {
        if (dropdown == null || dropdown.options == null || dropdown.options.Count == 0)
            return "";

        string value = dropdown.options[dropdown.value].text.Trim();

        // Treat blank or placeholder as empty
        if (string.IsNullOrWhiteSpace(value) ||
            value.ToLower() == "select" ||
            value.ToLower() == "select district" ||
            value.ToLower() == "select sector" ||
            value.ToLower() == "select cell")
        {
            return "";
        }

        return value;
    }

    private void SetCellId(string value)
    {
        if (cellIdText != null)
            cellIdText.text = value;

        if (cellIdInputField != null)
            cellIdInputField.text = value;
    }

    public string GetCurrentCellId()
    {
        if (cellIdInputField != null)
            return cellIdInputField.text.Trim();

        if (cellIdText != null)
            return cellIdText.text.Trim();

        return "";
    }
}