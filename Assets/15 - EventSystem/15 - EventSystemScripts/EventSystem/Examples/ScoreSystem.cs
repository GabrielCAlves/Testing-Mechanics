// Sistema de Pontuação
using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem1 : MonoBehaviour
{
    private int currentScore = 0;

    public void AddScore(int points)
    {
        currentScore += points;
        EventManager.Instance.RaiseEvent("PlayerScore", currentScore);
    }
}

// Sistema de UI
public class UISystem : MonoBehaviour
{
    public EventListener scoreListener;
    public Text scoreText;

    private void Start()
    {
        // Configuração via Inspector ou código
        scoreListener.EventSystem = EventManager.Instance.PlayerScore;
        scoreListener.onEventRaisedWithData.AddListener(UpdateScoreUI);
    }

    private void UpdateScoreUI(object data)
    {
        int score = (int)data;
        scoreText.text = $"Score: {score}";
    }
}

// Sistema de Áudio
public class AudioSystem : MonoBehaviour
{
    public EventListener gameOverListener;
    public AudioClip gameOverSound;

    private void Start()
    {
        gameOverListener.EventSystem = EventManager.Instance.GameOver;
        gameOverListener.onEventRaised.AddListener(PlayGameOverSound);
    }

    private void PlayGameOverSound()
    {
        AudioSource.PlayClipAtPoint(gameOverSound, Camera.main.transform.position);
    }
}