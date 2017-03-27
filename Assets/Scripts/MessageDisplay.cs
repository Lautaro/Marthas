using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Text))]
public class MessageDisplay : MonoBehaviour
{
    public class Message
    {
        public string message;
        public Color color;
    }

    public List<Message> messageQue = new List<Message>();

   // private Dictionary<string, Color> quedGuiMessages = new Dictionary<string, Color>();
    bool messageIsTweening;
    Text guiMessageText;

    void Awake()
    {
        guiMessageText = GetComponent<Text>();
    }


    public void DisplayGuiMessage(string message, Color color)
    {
        var newMessage = new Message()
        {
            message = message,
            color = color
        };

        // if display is busy with message, store in que
        if (messageIsTweening)
        {
            messageQue.Add(newMessage);
        }
        else
        {// otherwise display
            displayMessageOnScreen(newMessage);
        }
    }


    private void displayMessageOnScreen(Message message)
    {
        // remove from que
        RemoveMessageFromQue(message);

        // If message can be displayed, go ahead: 
        messageIsTweening = true;
        guiMessageText.text = message.message;
        guiMessageText.color = message.color;
        guiMessageText.gameObject.SetActive(true);

        var tweens = DOTween.Sequence();
        var textTf = guiMessageText.transform;


        // ANIMATE TEXT 
        tweens.Append(
            textTf.DOScale(new Vector3(2.0f, 2.0f, 1), 1)
            .SetEase(Ease.OutExpo)
            .SetLoops(2, LoopType.Yoyo));

        // When tween is complete
        tweens.OnComplete(()=> {
            OnMessgeDisplayCompleted();
        });
    }

    private void OnMessgeDisplayCompleted()
    {
        // On Complete!!!
        guiMessageText.gameObject.SetActive(false);
        messageIsTweening = false;

        if (messageQue.Count > 0)
        {
            // If there are messages in que, send next message.
            var nextMessage = messageQue.First();
            displayMessageOnScreen(nextMessage);
        }
    }

    private void RemoveMessageFromQue(Message message)
    {
        if (messageQue.Contains(message))
        {
            messageQue.Remove(message);

        }
    }
}
