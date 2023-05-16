using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToMain : MonoBehaviour
{
    private Button _button;
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => SceneManager.LoadScene("Main"));
    }
}
