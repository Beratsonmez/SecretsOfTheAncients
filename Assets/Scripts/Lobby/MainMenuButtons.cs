using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField, Header("Buttons Objects Refference")]
    public Button startButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button quitButton;

    [SerializeField] private GameObject NetworkManagerPrefab;
    private GameObject spawnedObject;

    [Header("Canvas Objects Refference")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject optionsMenuCanvas;
    [SerializeField] private GameObject creditsMenuCanvas;

    private void Start()
    {
        if(NetworkManagerPrefab != null)
        {
            spawnedObject = Instantiate(NetworkManagerPrefab);

            if (startButton != null && spawnedObject != null)
            {
                SteamLobby hostScript = spawnedObject.GetComponent<SteamLobby>();

                if (hostScript != null)
                {
                    startButton.onClick.AddListener(hostScript.HostLobby);
                }
                else
                {
                    Debug.LogError("Script bile�eni bulunamad�.");
                }
            }
            
        }
        else
        {
            Debug.Log("Prefab Yarat�lamad�.");
        }

        mainMenuCanvas.SetActive(true);
    }
 
    public void OpenCanvas(GameObject selectedCanvas)
    {
        // �nce t�m canvaslar� kapat
        mainMenuCanvas.SetActive(false);
        optionsMenuCanvas.SetActive(false);
        creditsMenuCanvas.SetActive(false);

        // Se�ili olan� a�
        selectedCanvas.SetActive(true);
        Debug.Log("Changed Canvas to" +  selectedCanvas);
    }

    public void OptionsMenu()
    {
        OpenCanvas(optionsMenuCanvas);
    }

    public void CreditsMenu()
    {
        OpenCanvas(creditsMenuCanvas);
    }

    public void MainMenuBack()
    {
        OpenCanvas(mainMenuCanvas);
    }

    public void QuitGame()
    {
        QuitGame();
    }
}
