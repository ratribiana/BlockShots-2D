using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddressableManager : MonoBehaviour
{
    [SerializeField] Slider _loadingBar;
    [SerializeField] TMPro.TMP_Text _progressTxt;
    IEnumerator Start()
    {
        _progressTxt.text = $"Loading... 0%";


        while (_loadingBar.value < 1)
        {
            _loadingBar.value += Time.deltaTime / 4f;
            _progressTxt.text = $"Loading... {Mathf.FloorToInt(_loadingBar.value * 100)}%";
            yield return null;
        }

        yield return new WaitForSeconds(.2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("02 Authentication");

        yield return null;
    }
}
