using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class InitController : MonoBehaviour
{
    [FormerlySerializedAs("gobjTextbox")] [SerializeField]
    private GameObject _gobjTextbox;
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void SetUsername()
    {
        if (_gobjTextbox.GetComponent<InputField>().text != "")
        {
            PlayerPrefs.SetString("userName", _gobjTextbox.GetComponent<InputField>().text);
            Debug.Log("Player情報を格納しました！");
            SceneManager.LoadScene("Tutorial");
        }
    }
}
