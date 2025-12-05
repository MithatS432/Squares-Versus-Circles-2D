using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuUI : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;

    public GameObject startEffectObj;
    public GameObject quitEffectObj;

    private const float ROTATION_INTERVAL = 3f;

    void Start()
    {
        StartCoroutine(EffectCycle());
    }

    IEnumerator EffectCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(ROTATION_INTERVAL);

            if (startEffectObj != null) startEffectObj.SetActive(true);
            if (quitEffectObj != null) quitEffectObj.SetActive(true);

            yield return StartCoroutine(RotateButton360(startButton.transform, 1f));
            yield return StartCoroutine(RotateButton360(quitButton.transform, 1f));


            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator RotateButton360(Transform buttonTransform, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float angle = Mathf.Lerp(0f, 360f, elapsed / duration);
            buttonTransform.localRotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
        buttonTransform.localRotation = Quaternion.identity;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Arena");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;     
#else
        Application.Quit();
#endif
    }
}
