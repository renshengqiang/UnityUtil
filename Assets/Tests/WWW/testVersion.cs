using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class testVersion : MonoBehaviour {
    public string url = "http://sg.version.fusionwar.37.com:8003";
    public Text resultText;

 	// Use this for initialization
	void Start () {
        StartCoroutine(func());
	}
	
	// Update is called once per frame
	IEnumerator func () {
        WWW www = new WWW(url);
        yield return www;
        Debug.Log("www.bytes: " + www.bytes);
        if (string.IsNullOrEmpty(www.error))
        {
            resultText.text = "成功：" + System.Text.Encoding.UTF8.GetString(www.bytes);
        }
        else
        {
            resultText.text = "失败：" + www.error;
        }
	}
}
