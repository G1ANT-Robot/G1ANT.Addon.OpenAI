using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using G1ANT.Language;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace G1ANT.Addon.OpenAI.Commands
{
    [Command(Name = "chatgpt", Tooltip = "This command send prompt to ChatGPT, model 'gpt - 3.5 - turbo'")]
    public class ChatGPTCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Tooltip = "Your OpenAI API Key")]
            public TextStructure ApiKey { get; set; }

            [Argument(Tooltip = "Prompt for ChatGPT", Required = true)]
            public TextStructure Prompt { get; set; }
            // Information about range is from api documentation, but it works also with values outside the range
            [Argument(Tooltip = "Default 1. Range between 0 - 2.  will make the output more random, lower values will make it more focused and deterministic.", Required = true)]
            public IntegerStructure Temperature { get; set; } = new IntegerStructure(1);

            [Argument(Tooltip = "If set to true, full JSON response will be returned. If false, only message content will be returned")]
            public BooleanStructure ReturnJson { get; set; } = new BooleanStructure(false);

            [Argument(Tooltip = "Name of a variable where the result will be stored")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public ChatGPTCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            var model = "gpt-3.5-turbo";
            var apiKey = arguments.ApiKey.Value;
            var prompt = arguments.Prompt.Value;
            var temperature = arguments.Temperature.Value;

            var chatResponse = ChatGPT(apiKey, prompt, model, temperature);
            string result = chatResponse.Content.ReadAsStringAsync().Result;
            var formattedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(result), Formatting.Indented);
            var jsonChatResponse = JObject.Parse(formattedJson);

            if (chatResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (arguments.ReturnJson.Value)
                    Scripter.Variables.SetVariableValue(arguments.Result.Value, new JsonStructure(jsonChatResponse));
                else
                    Scripter.Variables.SetVariableValue(arguments.Result.Value, new TextStructure(jsonChatResponse["choices"][0]["message"]["content"].ToString()));
            }
            else
            {
                throw new HttpException((int)chatResponse.StatusCode, jsonChatResponse["error"].ToString());
            }
        }

        static HttpResponseMessage ChatGPT(string apiKey, string content, string model, int temperature = 1)
        {
            var messages = new List<GptMessage>
            {
                new GptMessage { Role = "user", Content = content }
            };

            var jsonModel = new GptJsonModel
            {
                Model = model,
                Messages = messages
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.openai.com/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                HttpResponseMessage response = client.PostAsJsonAsync("v1/chat/completions", jsonModel).Result;
                return response;
            }
        }

        public class GptMessage
        {
            [JsonProperty("role")]
            public string Role { get; set; }

            [JsonProperty("content")]
            public string Content { get; set; }
        }

        public class GptJsonModel
        {
            [JsonProperty("model")]
            public string Model { get; set; }

            [JsonProperty("messages")]
            public List<GptMessage> Messages { get; set; }
        }
    }
}
