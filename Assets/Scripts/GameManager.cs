using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Rigidbody ball;
    public Transform startPoint;
    public PlatformTilt platform;
    public GameObject[] collectibles;
    public GameObject finish;
    public Text counterText;
    public Text messageText;

    public bool HasWon { get; private set; }
    public int Collected { get; private set; }

    void Start()
    {
        counterText.text = "Objetos: 0 / 3";
        messageText.text = "";
    }

    public void TouchObject(GameObject touched)
    {
        if (HasWon) return;

        foreach (GameObject item in collectibles)
        {
            if (touched == item && item.activeSelf)
            {
                item.SetActive(false);
                Collected++;
                counterText.text = "Objetos: " + Collected + " / 3";
                messageText.text = "";
                return;
            }
        }

        if (touched == finish)
        {
            if (Collected == 3)
            {
                HasWon = true;
                messageText.text = "You win!";
                ball.linearVelocity = Vector3.zero;
                ball.angularVelocity = Vector3.zero;
                ball.isKinematic = true;
                platform.enabled = false;
            }
            else
                messageText.text = "Recoge los 3 objetos antes de llegar a la meta.";
        }
    }

    public void RespawnBall()
    {
        if (HasWon) return;
        ball.linearVelocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;
        ball.position = startPoint.position;
        ball.rotation = startPoint.rotation;
        messageText.text = "";
    }

    public void RestartGame()
    {
        // Recargar la escena restaura todos los objetos a su estado inicial.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
