using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TableInteractable : MonoBehaviour
{
    public QuestCollectableData finalDessert;
    [Tooltip("0 is Sweet trophy, 1 is Savory, 2 is sour, 3 is Bitter")]
    public QuestCollectableData[] trophies;
    public CollectableData collectDataForKeyIcon;
    public GameObject keyIcon;
    public KeyCode interactKey;
    public float interactRange;
    public float fadeTime;
    public string endSceneName;
    private bool inRange;
    private void Start()
    {
        if (!Player.Instance.inventory.HasQuestItem(finalDessert))
        {
            Destroy(gameObject);
        }
        keyIcon.GetComponent<SpriteRenderer>().sprite = collectDataForKeyIcon.keySprite;
    }

    private void Update()
    {
        inRange = Vector2.Distance(transform.position, Player.Instance.transform.position) <= interactRange;
        if (inRange)
        {
            bool canInteract = !Player.Instance.InInteraction && !Player.Instance.GetIsAttaching() && Player.Instance.GetIsGrounded() && !Player.Instance.GetIsLocked() && !Player.Instance.GetHookBeingThrown();
            if (canInteract)
            {
                keyIcon.SetActive(true);
                if (Input.GetKeyDown(interactKey))
                {
                    Player.Instance.InInteraction = true;
                    StartCoroutine(OnInteract());
                }
            }
        }
        else
        {
            keyIcon.SetActive(false);
        }
    }
    private IEnumerator OnInteract()
    {
        // Check which trophies have been claimed
        if (Player.Instance.inventory.HasQuestItem(trophies[0]))
        {
            EndGameManager.HAS_SWEET_TROPHY = true;
        }
        if (Player.Instance.inventory.HasQuestItem(trophies[1]))
        {
            EndGameManager.HAS_SAVORY_TROPHY = true;
        }
        if (Player.Instance.inventory.HasQuestItem(trophies[2]))
        {
            EndGameManager.HAS_SOUR_TROPHY = true;
        }
        if (Player.Instance.inventory.HasQuestItem(trophies[3]))
        {
            EndGameManager.HAS_BITTER_TROPHY = true;
        }
        // start cutscene
        UIManager.Instance.FadeOut();
        float volume = AudioListener.volume;
        float timer = fadeTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            AudioListener.volume = Mathf.Lerp(0, volume, timer / fadeTime);
            yield return null;
        }
        yield return null;
        AudioListener.volume = volume;
        GameManager.Instance.ResetGame();
        SceneManager.LoadScene(endSceneName);

        // TODO: Change song to end game scene or something
    }


}
