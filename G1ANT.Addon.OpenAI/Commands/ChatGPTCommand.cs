using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using G1ANT.Language;
using Newtonsoft.Json;
using System.Net.Http.Formatting;

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

            [Argument(Tooltip = "If set to true, full JSON response will be returned. If false, only message content will be returned")]
            public BooleanStructure ReturnJson { get; set; } = new BooleanStructure(false);

            [Argument(Tooltip = "Name of a variable where the provided text will be stored")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public ChatGPTCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            var model = "gpt - 3.5 - turbo";
            var token = arguments.ApiKey.Value;
            var requestBodyModel = "";

            var requestBodyJson = JsonConvert.SerializeObject(requestBodyModel);
            var requestContent = new StringContent(requestBodyJson);
            



        }

        static async Task ChatGPT(string token, string requestContent)
        {
            var messages = new List<GptMessage>
            {
                new GptMessage { Role = "user", Content = "Hello!" }
            };

            var jsonModel = new GptJsonModel
            {
                Model = "gpt-3.5-turbo",
                Messages = messages
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.openai.com/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string apiKey = token;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                HttpResponseMessage response = await client.PostAsJsonAsync("v1/chat/completions", jsonModel);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var formattedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(result), Formatting.Indented);
                    Console.WriteLine("Response:");
                    Console.WriteLine(formattedJson);
                }
                else
                {
                    Console.WriteLine($"Request failed. Status code: {response.StatusCode}");
                }
            }
            //var client = new HttpClient();
            //// Set the authorization header
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"\"Bearer\", {token}");

            //// Send the POST request and get the response
            //HttpResponseMessage response = await client.PostAsync(url, requestContent);
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
