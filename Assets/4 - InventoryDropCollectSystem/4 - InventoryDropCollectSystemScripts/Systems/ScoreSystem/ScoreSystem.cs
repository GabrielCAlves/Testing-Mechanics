using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public int currentScore = 0;

    public void AddPoints(int points)
    {
        currentScore += points;
        Debug.Log("Score: " + currentScore);
    }
}
