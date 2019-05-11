using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigController : MonoBehaviour
{
    Text bgmText;

    // Start is called before the first frame update
    void Start()
    {
        bgmText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void volumeUpdate (float bgmValue)
    {
        BgmManager.Instance.TargetVolume = bgmValue;
        BgmManager.Instance.CurrentAudioSource.volume = bgmValue;
        bgmText.text = Mathf.RoundToInt(bgmValue*100).ToString();
    }
}
