using UnityEngine.UI;
using UnityEngine;
using LootLocker.Requests;
using System.Collections;

public class LeaderboardManager : MonoBehaviour
{

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    void Start()
    {
        StartCoroutine(Login());
    }

    /// <summary>
    /// Coroutine for Player Login
    /// </summary>
    /// <returns></returns>
    public IEnumerator Login()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession("LeaderboardPlayer", (response) =>
        {
            if (response.success)
            {
                Debug.Log("Succesfully started LootLocker session");
                PlayerPrefs.SetString("PlayerID", response.player_id.ToString());
                done = true;
            }
            else
            {
                Debug.Log("Error starting LootLocker session");
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }
}
