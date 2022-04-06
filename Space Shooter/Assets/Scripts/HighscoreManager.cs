using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighscoreManager : MonoBehaviour
{
    // Leaderboard usernames
    [Tooltip("Leaderboard usernames")]
    [SerializeField]
    private GameObject[] usernameObjects;

    // Leaderboard scores
    [Tooltip("Leaderboard scores")]
    [SerializeField]
    private GameObject[] scoreObjects;

    // Leaderboard ID
    [Tooltip("Leaderboard ID")]
    [SerializeField]
    private int leaderboardID;

    // Nubmer of highscores that will appear on the Leaderboard screen
    [Tooltip("Nubmer of highscores that will appear on the Leaderboard screen")]
    [SerializeField]
    private int numberOfHighscores;

    // 1st, 2nd and 3rd place star icons
    [Tooltip("1st, 2nd and 3rd place star icons")]
    [SerializeField]
    private GameObject[] starIcons;

    // Username Texts
    private List<TextMeshProUGUI> usernameTexts = new();

    // Score Texts
    private List<TextMeshProUGUI> scoreTexts = new();

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    void Start()
    {
        for (int i = 0; i < usernameObjects.Length; i++)
        {
            usernameTexts.Add(usernameObjects[i].GetComponent<TextMeshProUGUI>());
            scoreTexts.Add(scoreObjects[i].GetComponent<TextMeshProUGUI>());
        }
        StartCoroutine(FetchTopHighscores(numberOfHighscores));
    }

    /// <summary>
    /// Gets the top x highscores
    /// </summary>
    /// <param name="numberOfHighscores">Value of x</param>
    /// <returns></returns>
    public IEnumerator FetchTopHighscores(int numberOfHighscores)
    {
        LootLockerSDKManager.GetScoreList(leaderboardID, numberOfHighscores, (response) => { 
            if (response.success)
            {
                LootLockerLeaderboardMember[] members = response.items;

                for (int i = 0; i < members.Length; i++)
                {
                    if (i < starIcons.Length)
                        starIcons[i].SetActive(true);
                    usernameTexts[i].text = members[i].player.name;
                    scoreTexts[i].text = members[i].score + "\n";
                }
            }
            else
                Debug.Log("Failed to load highscores");
        });

        yield return new WaitWhile(() => false);
    }
}
