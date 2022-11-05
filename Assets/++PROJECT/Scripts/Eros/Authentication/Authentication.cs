using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;
using SimpleJSON;
using TMPro;

public class Authentication : MonoBehaviour
{


    [SerializeField] private string BaseURL;

    [Header("Login")]
    [SerializeField] private GameObject _entryLogin;
    [SerializeField] private Button _login;

    [Header("Sign Up")]
    [SerializeField] private GameObject _registration;
    [SerializeField] private TMP_InputField _username;
    [SerializeField] private TMP_InputField _email;
    [SerializeField] private TMP_InputField _code;
    [SerializeField] private Button _sendCode;
    [SerializeField] private Button _signUp;
    [Header("Status")]
    [SerializeField] private CanvasGroup _status;
    [SerializeField] private TMP_Text _statusTxt;

    public void ToggleSendCode() => _sendCode.interactable = (!string.IsNullOrWhiteSpace(_username.text) && 
                                                              !string.IsNullOrWhiteSpace(_email.text));
    public void ToggleSignUp() => _signUp.interactable = (!string.IsNullOrWhiteSpace(_username.text) &&
                                                          !string.IsNullOrWhiteSpace(_email.text) &&
                                                          !string.IsNullOrWhiteSpace(_code.text));
    private void Awake()
    {
        //_login.interactable = false;

        _sendCode.interactable = false;
        _signUp.interactable = false;
    }

    public void OpenLogin() => Transition.instance.StartTransition(_entryLogin, _registration, Transition.LOADTYPE.UI);
    public void OpenRegistration() => Transition.instance.StartTransition(_registration, _entryLogin, Transition.LOADTYPE.UI);

    #region Login
    public void Login()
    {
        Transition.instance.StartTransition("03 Lobby",Transition.LOADTYPE.Scene);

    }
    #endregion

    #region Registration
    public void RequestOTP()
    {
        string jsonForm = "{" +
                            "\"email\":\"" + _email.text + "\"," +
                            "\"username\":\"" + _username.text + "\"," +
                            "\"type\":\"user\"," +
                            "\"version\": \"1.0.0\"," +
                            "\"platform\":\"" + ((Application.platform == RuntimePlatform.IPhonePlayer) ? "ios\"" : "android\"") +
                           "}";

        Debug.Log(jsonForm);

        StartCoroutine(SendRequest());
        IEnumerator SendRequest()
        {
            using (UnityWebRequest www = UnityWebRequest.Put($"{BaseURL}register/generate/otp", jsonForm))
            {
                www.method = UnityWebRequest.kHttpVerbPOST;
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("Authorization", "Basic YNGBRTVBQTVCOTkzOTk2PFGxQ0Q3NjkxRUQ0M0E6MClwaiNUeih4fmJGM0NvU3NjJU9MSnx5eUE0U2oxfWpRfH1zKVBtPS1YeV9AJiE+a3o5VUxgZkkoKTkhXWZtQw==");

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                   www.result == UnityWebRequest.Result.DataProcessingError ||
                   www.result == UnityWebRequest.Result.InProgress ||
                   www.result == UnityWebRequest.Result.ProtocolError)
                    Debug.Log("Error sending webrequest: " + www.downloadHandler.text);
                else
                {
                    string res = www.downloadHandler.text;
                    res = res.Replace("[", "").Replace("]","");

                    Debug.Log(res);

                    JSONNode response = JSON.Parse(res);
                    yield return response;

                    Color col;
                    ColorUtility.TryParseHtmlString((response["success"]) ? "#54CA4D" : "#CA4D56", out col);
                    _statusTxt.text = (response["success"]) ? "OTP Code sent!" : "Something went wrong";

                    _status.GetComponent<Image>().color = col;
                    while (_status.alpha < 1)
                    {
                        _status.alpha += Time.deltaTime / 2f;
                        yield return null;
                    }
                    yield return new WaitForSeconds(3);
                    while (_status.alpha > 0)
                    {
                        _status.alpha -= Time.deltaTime / 2f;
                        yield return null;
                    }
                }
            }
            yield return null;
        }
    }
    public void Signup()
    {
        string jsonForm = "{" +
                            "\"email\":\"" + _email.text + "\"," +
                            "\"otp\":\"" + _code.text + "\"," +
                            "}";
        StartCoroutine(SendRequest());
        IEnumerator SendRequest()
        {
            using (UnityWebRequest www = UnityWebRequest.Put($"{BaseURL}register/verify/otp", jsonForm))
            {
                www.method = UnityWebRequest.kHttpVerbPOST;
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("Authorization", "Basic YNGBRTVBQTVCOTkzOTk2PFGxQ0Q3NjkxRUQ0M0E6MClwaiNUeih4fmJGM0NvU3NjJU9MSnx5eUE0U2oxfWpRfH1zKVBtPS1YeV9AJiE+a3o5VUxgZkkoKTkhXWZtQw==");

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                   www.result == UnityWebRequest.Result.DataProcessingError ||
                   www.result == UnityWebRequest.Result.InProgress ||
                   www.result == UnityWebRequest.Result.ProtocolError)
                    Debug.Log("Error sending webrequest: " + www.downloadHandler.text);
                else
                {
                    string res = www.downloadHandler.text;
                    res = res.Replace("[", "").Replace("]", "");

                    Debug.Log(res);

                    JSONNode response = JSON.Parse(res);
                    yield return response;

                    Color col;
                    ColorUtility.TryParseHtmlString((response["success"]) ? "#54CA4D" : "#CA4D56", out col);
                    _statusTxt.text = (response["success"]) ? "Registered succesfully!" : "Something went wrong";

                    _status.GetComponent<Image>().color = col;
                    while (_status.alpha < 1)
                    {
                        _status.alpha += Time.deltaTime / 2f;
                        yield return null;
                    }
                    yield return new WaitForSeconds(3);
                    while (_status.alpha > 0)
                    {
                        _status.alpha -= Time.deltaTime / 2f;
                        yield return null;
                    }
                }
            }
            yield return null;
        }
    }
    #endregion
}
