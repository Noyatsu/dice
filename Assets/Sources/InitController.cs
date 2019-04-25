using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InitController : MonoBehaviour
{
    [SerializeField] GameObject gobjTextbox;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setUsername()
    {
        if (gobjTextbox.GetComponent<InputField>().text != "")
        {
            PlayerPrefs.SetString("userName", gobjTextbox.GetComponent<InputField>().text);
            Debug.Log("Player情報を格納しました！");
            SceneManager.LoadScene("Tutorial");
        }
    }
}
