using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;
        
        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        private float height;
        private OpenAIApi openai = new OpenAIApi("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");

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "Act as this character. { 이름 : 김범준 , 직업 : 연쇄 살인마, 역할 : 흉악한 범죄자, 성별 : 남성, 나이 : 25살, 성격 : 살벌함 }" +
            "Say it in less than 10 words. 제시된 정보를 나열해서 말하지는 마. 반말을 쓰면서 막 욕하면서 얘기해. 대화하고 있는 사람과 원한관계가 있다." +
            "너는 AI모델이라고 말하지 말고 정체에 대해서 물어보면 죽인다고 말해" +
            "주어지지 않은 정보를 지어내지는 마, Don't break character. Don't ever mention that you are an AI model.";

        private void Start()
        {
            button.onClick.AddListener(SendReply);
        }

        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        private async void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };
            
            AppendMessage(newMessage);

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text; 
            
            messages.Add(newMessage);
            
            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;
            
            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                
                messages.Add(message);
                AppendMessage(message);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            button.enabled = true;
            inputField.enabled = true;
        }
    }
}
