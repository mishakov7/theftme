﻿using System.Collections;
using static System.Console;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class ReplyEvent : UnityEvent<Reply> { }

public class DisplayDialogue : MonoBehaviour
{
    public Conversation conversation;
    public ReplyEvent replyEvent;

    public GameObject leftSpeaker;
    public GameObject rightSpeaker;

    private SpeakerUI leftSpeakerUI;
    private SpeakerUI rightSpeakerUI;

    private int activeLineIndex;
    private bool conversationStarted = false;

    public void ChangeConversation(Conversation nextConversation) {
        conversationStarted = false;
        conversation = nextConversation;
        AdvanceLine();
    }

    private void Start()
    {
        leftSpeakerUI = leftSpeaker.GetComponent<SpeakerUI>();
        leftSpeakerUI.Speaker = conversation.leftSpeaker;

        rightSpeakerUI = rightSpeaker.GetComponent<SpeakerUI>();
        rightSpeakerUI.Speaker = conversation.rightSpeaker;
    }

    private void Update()
    {
        if (Input.GetKeyDown("space")) {
            AdvanceLine();

        } else if (Input.GetKeyDown(KeyCode.Escape)) {
            EndConversation();
        }
    }
    void OnCollisionEnter(Collision col)  //Unity function called when a collision is detected, and the object collided is put into the variable 'col' to be used later
    {
        if (col.gameObject.name == "RightHand")   //if the object you collided with is the enemy
        {
            //transform.localScale += new Vector3(0, 0, 1); //increase the size of the ball
            AdvanceLine();

            //Destroy(col.gameObject);  //Destroy the enemy
        }

    }




    private void EndConversation() {
        conversation = null;
        conversationStarted = false;
        leftSpeakerUI.Hide();
        rightSpeakerUI.Hide();
    }

    private void Initialize() {
        conversationStarted = true;
        activeLineIndex = 0;
        leftSpeakerUI.Speaker = conversation.leftSpeaker;
        rightSpeakerUI.Speaker = conversation.rightSpeaker;
    }

    private void AdvanceLine() {
        if (conversation == null) {
            return;
        }

        if (!conversationStarted) {
            Initialize();
        }

        if (activeLineIndex < conversation.lines.Length) {
            DisplayLine();

        } else {
            AdvanceConversation();
        }
    }

    private void AdvanceConversation() {
        if (conversation.reply != null) {
            replyEvent.Invoke(conversation.reply);

        } else if (conversation.nextConversation != null) {
            ChangeConversation(conversation.nextConversation);

        } else {
            EndConversation();
        }
    }

    void DisplayLine() {
        Line line = conversation.lines[activeLineIndex];
        Character character = line.character;

        if (leftSpeakerUI.SpeakerIs(character)) {
            SetDialogue(leftSpeakerUI, rightSpeakerUI, line.dialogue);

        } else {
            SetDialogue(rightSpeakerUI, leftSpeakerUI, line.dialogue);

        }

        activeLineIndex += 1;
    }

    void SetDialogue(
        SpeakerUI activeSpeaker,
        SpeakerUI inactiveSpeaker,
        string text
    ) {

        activeSpeaker.Dialogue = text;

        /*StopAllCoroutines();
        StartCoroutine(TypeSentence(activeSpeaker, text));*/

        activeSpeaker.Show();
        inactiveSpeaker.Hide();
    }

    /*IEnumerator TypeSentence(SpeakerUI speaker, string sentence) {
        speaker.dialogue.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            speaker.dialogue.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }*/
}