using TMPro;
using UnityEngine;

public class QuestLog : MonoBehaviour
{
    #region Singleton
    public static QuestLog instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    #endregion

    [SerializeField] private TextMeshProUGUI _textMeshPro;

    [SerializeField] private Quest[] quests;

    private int questLine = 0;
    private int quest = 0;

    private void Start()
    {
        _textMeshPro.text = quests[questLine].m_Quest[quest];
    }

    public void ProgressQuest()
    {
        quest++;

        if (quest >= quests[questLine].m_Quest.Length)
        {
            questLine++;
            quest = 0;

            if(questLine < quests.Length)
            _textMeshPro.text = quests[questLine].m_Quest[quest];
            else
            {
                Debug.Log("No quests left!");
            }
        }
        else
        {
            _textMeshPro.text = quests[questLine].m_Quest[quest];
        }
    }

}
