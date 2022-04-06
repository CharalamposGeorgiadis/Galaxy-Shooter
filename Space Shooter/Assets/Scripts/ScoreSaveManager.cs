using LootLocker.Requests;
using System.Collections;
using UnityEngine;

public class ScoreSaveManager : MonoBehaviour
{
    // Leaderboard ID
    [Tooltip("Leaderboard ID")]
    [SerializeField]
    private int leaderboardID;

    /// <summary>
    /// Coroutine for submiting the Player's score
    /// </summary>
    /// <param name="score">Player score</param>
    /// <returns></returns>
    public IEnumerator SubmitScore(int score)
    {
        bool done = false;
        string username = UsernameManager.username;

        LootLockerSDKManager.SubmitScore(username, score, leaderboardID, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Succesfully Uploaded Score");
                done = true;
            }
            else
            {
                Debug.Log("Failed to Upload Score");
                done = true;
            }
            });
            yield return new WaitWhile(() => done == false);
    }
}
