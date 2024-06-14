
#if ADVENTURE_CREATOR

using System;
using System.Collections.Generic;
using AC;
using Action = AC.Action;
using Char = AC.Char;

namespace stoogebag
{
    public class ConversationStreamActionList : ActionList
    {

        public int indexInConversation;
        public int currentIndex;
        public List<Segment> Segments;
        public Conversation Convo;
        public Char asker;
        public Char responder;

        private void Start()
        {
            Convo = GetComponent<Conversation>();
        }

        public override void Interact(int i, bool addToSkipQueue)
        {


            //make my changes?

            actions = new List<Action>();

            var seg = Segments[currentIndex];

            var speaker = currentIndex == 0 ? asker : responder;


            currentIndex++;
            if (currentIndex > Segments.Count - 1) currentIndex = Segments.Count - 1;



            var qAction = ActionSpeech.CreateNew(asker, seg.DialogueOption.text);
            actions.Add(qAction);
            for (int j = 0; j < seg.Responses.Count; j++)
            {
                var speechAction = ActionSpeech.CreateNew(responder, seg.Responses[j].text);
                actions.Add(speechAction);
            }

            base.Interact(i, addToSkipQueue);

            Convo.options[indexInConversation].label = Segments[currentIndex].DialogueOption.text;
        }
    }


    [Serializable]
    public class Segment
    {
        public DialogueLine DialogueOption;
        public List<DialogueLine> Responses;

        public DialogueLine GetLine(int i)
        {
            if (i == 0) return DialogueOption;
            else return Responses[i - 1];
        }
    }

    [Serializable]
    public class DialogueLine
    {
        public string text;
    }
}
#endif