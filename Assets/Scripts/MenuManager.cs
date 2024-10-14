using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject Main;
    public GameObject AR;
    public GameObject AR1;
    public GameObject MainPanel;
    public GameObject xrOrigin;

    // Reference to the ARTrackedImageManager
    private ARTrackedImageManager arTrackedImageManager;
    private static MenuManager instance;

    // Start is called before the first frame update
    void Start()
    {
        Main.SetActive(true);
        AR.SetActive(false);
        AR1.SetActive(false);
        // Get the ARTrackedImageManager component from the XR Origin
        arTrackedImageManager = xrOrigin.GetComponent<ARTrackedImageManager>();

        if (arTrackedImageManager == null)
        {
            Debug.LogError("ARTrackedImageManager component not found on XR Origin!");
        }
        // Toggle the script enabled/disabled state
        //arTrackedImageManager.SetActive(false);
        //xrOrigin.ARTrackedImageManager.SetActive(false);
        arTrackedImageManager.enabled = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void ArButton()
    {
        Main.SetActive(false);
        AR.SetActive(true);
    }

    public void ArButton1()
    {
        AR.SetActive(false);
        MainPanel.SetActive(false);
        AR1.SetActive(true);
    }
    public void ScanQR()
    {
        // Toggle the script enabled/disabled state
        arTrackedImageManager.enabled = true;
        //xrOrigin.ARTrackedImageManager.SetActive(true);

        MainPanel.SetActive(false);
    }

    public void BackButton()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        ArButton();
        ArButton1();
    }
    public void BackButton1()
    {
        MainPanel.SetActive(true);
        AR.SetActive(true);
        AR1.SetActive(false);
    }

    public void BackButton2()
    {
        AR.SetActive(false);
        Main.SetActive(true);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Call ArButton and ArButton1 after the scene loads
        ArButton();
        ArButton1();
    }
}
