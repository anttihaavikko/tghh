using System;
using TMPro;
using UnityEngine;

public class NameChanger : MonoBehaviour
{
    [SerializeField] private TMP_InputField field;
    [SerializeField] private GameObject help;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("PlayerId")) PlayerPrefs.SetString("PlayerId", Guid.NewGuid().ToString());
        
        field.text = PlayerPrefs.GetString("PlayerName", "Anon");
        field.onEndEdit.AddListener(Change);
        field.onSelect.AddListener(HideHelp);
        if(field.text != "Anon") HideHelp();
    }

    public void HideHelp(string a = null)
    {
        if(help) help.SetActive(false);    
    }

    public void Change(string value)
    {
        if (value.Trim() == "") value = "Anon";
        field.text = value;
        PlayerPrefs.SetString("PlayerName", value);
    }
}