using _Project.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class TestDataSave : MonoBehaviour
{
    public Button saveButton;
    public Button loadButton;
    public Button buttonAdd;
    public Button buttonCancel;
    public Button buttonRestore;
    
    void Start()
    {
        saveButton.onClick.AddListener(() =>
        {
            // App.Settings.Save();
        });
        
        loadButton.onClick.AddListener(() =>
        {
            // App.Settings.Load();
        });
        
        buttonAdd.onClick.AddListener(() =>
        {
            // App.Settings.Video.number++;
        });
        
        buttonCancel.onClick.AddListener(() =>
        {
            // App.Settings.Cancel();
        });
        
        buttonRestore.onClick.AddListener(() =>
        {
            // App.Settings.Restore();
        });
    }
}
