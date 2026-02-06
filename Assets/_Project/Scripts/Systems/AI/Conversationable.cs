using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Conversationable : MonoBehaviour
{
    public bool IsConversing;
    private bool isSpeaking = false;
    private bool isFinished = false;
    private string speakingMessage;
    private string charactersPrinted = "";
    [SerializeField] private float timeBetweenCharacters = 0.04f;
    private int currentCharacter = 0;
    private float pauseBetweenSentences = 1f;

    public ConversationData CustomConversations; //If we want an enemy to have a custom conversation to them

    private GameObject speechBubble;

    [SerializeField] private EnemyController enemyController;

    private float conversationDelay = 20f; //Time between new conversations
    private float conversationTimer = 0.0f;
    private bool canTalk = true;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }
    public bool StoppedTalking()
    {
        return isFinished;
    }
    public void Talking()
    {
        if (isFinished is true) return;

        if(isSpeaking is false)
        {
            isSpeaking = true;
            StartCoroutine(TalkingCoroutine());
        }
    }
    private IEnumerator TalkingCoroutine()
    {
        canTalk = false;
        //int charactersToPrint = speakingMessage.Length;
        //charactersPrinted += (speakingMessage[currentCharacter]);
        while (charactersPrinted.Length < speakingMessage.Length)
        {
            charactersPrinted += (speakingMessage[currentCharacter]);
            speechBubble.GetComponentInChildren<TMP_Text>().text = charactersPrinted;
            currentCharacter++;
            yield return new WaitForSeconds(timeBetweenCharacters);
        }
    
      
        yield return new WaitForSeconds(pauseBetweenSentences);
        isFinished = true;
        ResetCharacters();
    }


    public void InitializeSpeakingMessage(string speakingMessage)
    {
        this.speakingMessage = speakingMessage;
        speechBubble = ConversationManager.Instance.InstantiateBubble(transform, "");
      
    }
    private void ResetCharacters()
    {
        isSpeaking = false;
        currentCharacter = 0;
        speakingMessage = "";
        charactersPrinted = "";
        Destroy(speechBubble.transform.gameObject);
    }
    public void IsDoneTalking()
    {
        isFinished = false;
        IsConversing = false;
    }
    private void Update()
    {
        if (IsConversing is true) return;

        if(canTalk is false)
        {
            conversationTimer += Time.deltaTime;
            if(conversationTimer > conversationDelay)
            {
                conversationTimer = 0;
                canTalk = true;
            }
            return;
        }

        SearchForConversation();
    }
    public void OverrideTalkDelay()
    {
        conversationTimer = 0;
        canTalk = true;
    }
    private void SearchForConversation()
    {
        if (IsConversing is true) return;

        switch (enemyController.GetCurrentState())
        {
            case EnemyChaseState:
                ConversationManager.Instance.TryAConversation(transform, ConversationTopic.Chase);
                conversationDelay = 1f;
                break;
            case EnemyAttackState:
                conversationDelay = 5f;
                 ConversationManager.Instance.TryAConversation(transform, ConversationTopic.Attack);

                break;
            case EnemyInvestigateState:
           
                break;
            case EnemyPatrolState:
                conversationDelay = 1f;
                ConversationManager.Instance.TryAConversation(transform, ConversationTopic.Alone);
                break;
            case EnemyStationaryState:
                ConversationManager.Instance.TryAConversation(transform, ConversationTopic.Alone);
                break;
        }
    }
}
