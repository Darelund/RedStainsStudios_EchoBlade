using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;



public enum ConversationTopic
{
    Saw,
    Heard,
    Alarm,
    Alone,
    Chase,
    Attack,
    Disappeared
}

//TODO: Use it later
[CreateAssetMenu(menuName = "GameData/ConversationData")]
public class ConversationsData : ScriptableObject
{
    [Serializable]
    public struct ConversationTopicLines
    {
        public ConversationTopic topic;
        public ConversationData[] data;
    }

    public ConversationTopicLines[] conversationPairs;

}
public class ConversationManager : MonoBehaviour
{
    [SerializeField] private GameObject SpeechBubblePrefab;
    private Dictionary<ConversationTopic, ConversationData> Conversations;

    public ConversationData GetConversation(ConversationTopic topic) => Conversations[topic];

    #region Singleton
    private static ConversationManager instance;
    public static ConversationManager Instance => instance;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
            return;
        }

        instance = this;
    }
    #endregion


    private List<EngagedInConversation> ongoingConversations = new List<EngagedInConversation>(); //People currently having a conversation
    public bool ShowDebugs = false;
    private void Start()
    {
        CreatingConversations();
    }

    public GameObject InstantiateBubble(Transform t, string talkText)
    {
        GameObject SpeechBubbleInstance = Instantiate(SpeechBubblePrefab, t);
        SpeechBubbleInstance.transform.GetChild(0).GetComponent<TMP_Text>().text = talkText;
        SpeechBubbleInstance.transform.GetComponent<SpeechBubble>().Init(Camera.main.transform);
  
        return SpeechBubbleInstance;
    }


    private void CreatingConversations()
    {
        Conversations = new Dictionary<ConversationTopic, ConversationData>();

        //Conversation 1 - If they saw something for a second that then disappeared
        Conversations[ConversationTopic.Saw] = new ConversationData(3);
        Conversations[ConversationTopic.Saw].AddTurn(ConversationTurn.Create("Found anything?", true), ConversationTurn.Create("Did you see something?", true));
        Conversations[ConversationTopic.Saw].AddTurn(ConversationTurn.Create("Nothing"), ConversationTurn.Create("No"), ConversationTurn.Create("No, what about you?", true));
        Conversations[ConversationTopic.Saw].AddTurn(ConversationTurn.Create("I am not sure?"), ConversationTurn.Create("No"), ConversationTurn.Create("I don't think so"), ConversationTurn.Create("Nope"));


        //Conversation 2 - If they heard a sound
        Conversations[ConversationTopic.Heard] = new ConversationData(3);
        Conversations[ConversationTopic.Heard].AddTurn(ConversationTurn.Create("You also heard that?", true), ConversationTurn.Create("Did you hear that?", true));
        Conversations[ConversationTopic.Heard].AddTurn(ConversationTurn.Create("I didn't hear anything"), ConversationTurn.Create("No"), ConversationTurn.Create("No, what about you?", true), ConversationTurn.Create("Don't think so"), ConversationTurn.Create("Nope"));
        Conversations[ConversationTopic.Heard].AddTurn(ConversationTurn.Create("I thought I heard something, but I guess not"), ConversationTurn.Create("Maybe it was just my imagination"));

        //Conversation 3 - If an alarm went off
        Conversations[ConversationTopic.Alarm] = new ConversationData(3);
        Conversations[ConversationTopic.Alarm].AddTurn(ConversationTurn.Create("Any idea why the alarm went off?", true), ConversationTurn.Create("See anything?", true));
        Conversations[ConversationTopic.Alarm].AddTurn(ConversationTurn.Create("Probably just a false alarm"), ConversationTurn.Create("No"), ConversationTurn.Create("No, what about you?", true), ConversationTurn.Create("Nope"));
        Conversations[ConversationTopic.Alarm].AddTurn(ConversationTurn.Create("I thought I saw a ghost"), ConversationTurn.Create("I thought I saw a ghost"), ConversationTurn.Create("Nothing"), ConversationTurn.Create("Nope"));


        //Talking with yourself
        Conversations[ConversationTopic.Alone] = new ConversationData(1);
        Conversations[ConversationTopic.Alone].AddTurn(ConversationTurn.Create("Must have been the wind"), ConversationTurn.Create("I am getting old"), ConversationTurn.Create("I swear I saw something"), ConversationTurn.Create("Muttering"));


        //Chasing the player
        Conversations[ConversationTopic.Chase] = new ConversationData(1);
        Conversations[ConversationTopic.Chase].AddTurn(ConversationTurn.Create("Stop there!"), ConversationTurn.Create("Come back here!"), ConversationTurn.Create("I will get you!"), ConversationTurn.Create("You won't get away!"), ConversationTurn.Create("Fight me coward!"), ConversationTurn.Create("Get back here!"), ConversationTurn.Create("Come back here!"));

        Conversations[ConversationTopic.Attack] = new ConversationData(1);
        Conversations[ConversationTopic.Attack].AddTurn(ConversationTurn.Create("Die!"), ConversationTurn.Create("You are done for!"), ConversationTurn.Create("You’re an ugly one, aren’t you?"), ConversationTurn.Create("Eat that!"));

        Conversations[ConversationTopic.Disappeared] = new ConversationData(1);
        Conversations[ConversationTopic.Disappeared].AddTurn(ConversationTurn.Create("Where did you go!"), ConversationTurn.Create("Show yourself!"), ConversationTurn.Create("Where are you hiding?"), ConversationTurn.Create("You can't hide forever!"), ConversationTurn.Create("I'll find you..."), ConversationTurn.Create("Where are you?"));


    }


    //More than 1
    public bool TryAConversation(Transform requester, ConversationTopic topic)
    {
        if (requester.GetComponent<Conversationable>().IsConversing is true) return false; //Can't start a new conversation when you are already in one


        Conversationable requested = FindNearestExtrovert(requester);
        //If requested is null then and the conversation topic is isAloneConversation
        //Only one line will get called, other
        //if (requested == null) return false; //Couldn't find anybody to have a conversation with

        if (GetConversation(topic).turns.Length > 1 && requested == null)
        {
            return false;
        }

        //Add the 2 characters to the list of people having a conversation
        Conversationable personA = requester.GetComponent<Conversationable>();
        Conversationable personB = requested;
        ongoingConversations.Add(new EngagedInConversation(personA, personB, topic));
        return true;
    }
    public bool SelfConversation(Transform requester)
    {
        return true;
    }
    public Conversationable FindNearestExtrovert(Transform requester) //LOL help me with my naming
    {
        float converseDistance = 10f;
        List<Conversationable> closestExtroverts = Physics.OverlapSphere(requester.transform.position, converseDistance, 1 << 9).
            ToList().FindAll(c => c.GetComponent<Conversationable>() != null).ConvertAll(c => c.GetComponent<Conversationable>());  

        Conversationable extrovertFound = null;

        foreach (var extrovert in closestExtroverts)
        {
            if (extrovert.gameObject == requester.gameObject) continue; //Don't add yourself 
            if (extrovert.IsConversing is true) continue;//Damn already conversing

            extrovertFound = extrovert;
            break;
        }
        return extrovertFound;

    }
    private void Update()
    {
        List<EngagedInConversation> ongoingConversationsToRemove = new();
        foreach (var ongoingConversation in ongoingConversations)
        {
            //TODO: Somehow make conversation start and then go between PersonA and PersonB
            ongoingConversation.Converse();
            if(ongoingConversation.isConversationDone is true)
            {
                ongoingConversationsToRemove.Add(ongoingConversation);
            }
        }
        foreach (var ongoingConversation in ongoingConversationsToRemove)
        {
            if(ongoingConversation.PersonA != null)
            {
                ongoingConversation.PersonA.IsDoneTalking();
            }
            if (ongoingConversation.PersonB != null)
            {
                ongoingConversation.PersonB.IsDoneTalking();
            }
            ongoingConversations.Remove(ongoingConversation);

            if(ShowDebugs)
            Debug.Log("Conversation ended");
        }
    }
}
[System.Serializable]
public class EngagedInConversation
{
    public Conversationable PersonA;
    public Conversationable PersonB;
    private ConversationData Conversation;
    private int conversationState = 0;
    private Conversationable currentSpeaker;

    public float converseTime;
    public float currentConverseTime;
    public bool isConversationDone = false;
    private string currentSentence;

    public EngagedInConversation(Conversationable personA, Conversationable personB, ConversationTopic conversationTopic)
    {
        PersonA = personA;
        PersonB = personB;

        Conversation = ConversationManager.Instance.GetConversation(conversationTopic);
        ChooseASpeaker(); //Set default speaker
        currentSpeaker.InitializeSpeakingMessage(GetNextSentence());
        if (personA != null)
            personA.IsConversing = true;
        if (personB != null)
            personB.IsConversing = true;
    }
    private string GetNextSentence()
    {
        currentSentence = Conversation.turns[conversationState][UnityEngine.Random.Range(0, Conversation.turns[conversationState].Length)].Sentence;
        return currentSentence;
    }
    public void Converse()
    {
        if(isConversationDone is true)
        {
            //TODO: Remove conversation
            //Debug.Log("Is done");
            return;
        }
       
        currentSpeaker.Talking();

        if (currentSpeaker.StoppedTalking() is true) //When we stop talking switch
        {
            if (NoResponse())
            {
                ConversationEnded();
                return;
            }

            ChooseASpeaker();
            if (isConversationDone) return;
            conversationState++;
           
            if (conversationState >= Conversation.turns.Length)
            {
                ConversationEnded();
            }
            else
            {
              
                string nextSentence = GetNextSentence();
                currentSpeaker.InitializeSpeakingMessage(nextSentence);
            }
        }
    }
    private bool NoResponse()
    {
        var currentTurn = Conversation.turns[conversationState].ToList().Find(c => c.Sentence == currentSentence);
        var currentTurnIndex = Conversation.turns[conversationState].ToList().IndexOf(currentTurn);
        return (Conversation.turns[conversationState][currentTurnIndex].WantResponse is false); //If we don't want a response return
    }
    private void ChooseASpeaker()
    {
        if (currentSpeaker == null)
        {
            currentSpeaker = PersonA;
        }
        else
        {
            currentSpeaker = currentSpeaker == PersonA ? PersonB : PersonA;
            if(PersonB == null)
            {
                ConversationEnded();
                currentSpeaker = PersonA;
            }
        }
    }
    private void ConversationEnded()
    {
        isConversationDone = true;
    }
    //public void MoveConversation()
    //{
    //    //PersonA starts with saying x
    //    //Then PersonB might reply with y
    //    //And then PersonA will maybe reply with z
    //}
}
//TODO: Should add voice lines to this later
// A class for creating conversations between characters. Responding with different sentences to each other. 


//TODO: Make it use scriptable object later

[System.Serializable]
public class ConversationData
{
    public ConversationTurn[][] turns;
    private int maxTurns;
    private int currentTurns;
    public ConversationData(int amountOfSentencesbetweenThem)
    {
        turns = new ConversationTurn[amountOfSentencesbetweenThem][];
        maxTurns = amountOfSentencesbetweenThem;
    }

    //The sentence/response a character will say to another character. Adding more then one will give it the opportunity to 
    //respond differently each time
    public void AddTurn(params ConversationTurn[] sentence)
    {
        if (maxTurns <= currentTurns)
        {
            Debug.Log("Can't add any more conversations, max conversations has been reached");
            return;
        }
        turns[currentTurns] = new ConversationTurn[sentence.Length];
        for (int i = 0; i < sentence.Length; i++)
        {
            turns[currentTurns][i] = sentence[i];
        }
        currentTurns++;
    }
    public ConversationTurn UseTurn(int turn, int sentence)
    {
        return turns[turn][sentence];
    }
}
[Serializable]
public struct ConversationTurn
{
    public string Sentence;
    public bool WantResponse;
    public ConversationTurn(string sentence, bool wantResponse)
    {
        Sentence = sentence;
        WantResponse = wantResponse;
    }
    public static ConversationTurn Create(string sentence, bool wantResponse = false)
    {
        return new ConversationTurn(sentence, wantResponse);
    }
}
