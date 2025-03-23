using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;
using LLMUnity;
using System.Linq;

namespace HuggingFace.API
{
    [Serializable]
    public class SentenceSimilarityRequest
    {
        public string source_sentence;
        public string[] sentences;
    }

    [Serializable]
    public class HuggingFaceRequest
    {
        public string inputs;
    }

    [Serializable]
    public class HuggingFaceResponse
    {
        public List<ResponseItem> generated_text;
    }

    [Serializable]
    public class ResponseItem
    {
        public string generated_text;
    }

    [Serializable]
    public class SpeechRecognitionResponse
    {
        public string text;
    }

    public class HuggingFaceAPI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _responseText;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Button _sendRequestButton;
        [SerializeField] private Button _startRecordingButton;
        [SerializeField] private Button _stopRecordingButton;
        [SerializeField] private LLMCharacter llmCharacter;

        private bool _isProcessing;
        private bool _isRecording;
        private AudioClip _recordingClip;

        private const string API_URL = "https://api-inference.huggingface.co/models/google/gemma-2-2b-it";
        private const string SIMILARITY_API_URL = "https://api-inference.huggingface.co/models/sentence-transformers/all-MiniLM-L6-v2";
        private const string SPEECH_API_URL = "https://api-inference.huggingface.co/models/openai/whisper-large-v3";
        private const string API_KEY = "Bearer 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";

        private void Awake()
        {
            _sendRequestButton.onClick.AddListener(OnButtonClick_SendRequest);
            _startRecordingButton.onClick.AddListener(StartRecording);
            _stopRecordingButton.onClick.AddListener(StopRecording);

            // Initially disable stop button
            _stopRecordingButton.interactable = false;

            if (llmCharacter == null)
            {
                llmCharacter = GetComponent<LLMCharacter>();
                if (llmCharacter == null)
                {
                    Debug.LogError("LLMCharacter component is missing! Please add it to the same GameObject.");
                }
            }
        }

        private void OnDestroy()
        {
            _sendRequestButton.onClick.RemoveListener(OnButtonClick_SendRequest);
            _startRecordingButton.onClick.RemoveListener(StartRecording);
            _stopRecordingButton.onClick.RemoveListener(StopRecording);
        }

        private void Update()
        {
            // Check if recording needs to be stopped due to length
            if (_isRecording && Microphone.GetPosition(null) >= _recordingClip.samples)
            {
                StopRecording();
            }
        }

        private void StartRecording()
        {
            _responseText.text = "Recording...";
            _startRecordingButton.interactable = false;
            _stopRecordingButton.interactable = true;
            _recordingClip = Microphone.Start(null, false, 10, 44100);
            _isRecording = true;
        }

        private void StopRecording()
        {
            if (!_isRecording) return;

            var position = Microphone.GetPosition(null);
            Microphone.End(null);
            var samples = new float[position * _recordingClip.channels];
            _recordingClip.GetData(samples, 0);
            byte[] wavData = EncodeAsWAV(samples, _recordingClip.frequency, _recordingClip.channels);
            _isRecording = false;

            _responseText.text = "Processing speech...";
            SendSpeechToText(wavData);
        }

        private async void SendSpeechToText(byte[] audioData)
        {
            try
            {
                using (UnityWebRequest request = new UnityWebRequest(SPEECH_API_URL, "POST"))
                {
                    request.uploadHandler = new UploadHandlerRaw(audioData);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "audio/wav");
                    request.SetRequestHeader("Authorization", API_KEY);

                    var operation = request.SendWebRequest();
                    while (!operation.isDone)
                        await Task.Yield();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        string response = request.downloadHandler.text;
                        Debug.Log($"Raw Speech API Response: {response}");

                        var speechResponse = JsonUtility.FromJson<SpeechRecognitionResponse>(response);
                        if (speechResponse != null && !string.IsNullOrEmpty(speechResponse.text))
                        {
                            _inputField.text = speechResponse.text;
                           _responseText.text = "Speech recognized! You can now send the request.";
                        }
                        else
                        {
                            _responseText.text = "Failed to recognize speech.";
                        }
                    }
                    else
                    {
                        _responseText.text = $"Error in speech recognition: {request.error}";
                    }
                }
            }
            catch (Exception e)
            {
                _responseText.text = $"Error in speech recognition: {e.Message}";
            }
            finally
            {
                _startRecordingButton.interactable = true;
                _stopRecordingButton.interactable = false;
            }
        }

        private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
        {
            using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
            {
                using (var writer = new BinaryWriter(memoryStream))
                {
                    writer.Write("RIFF".ToCharArray());
                    writer.Write(36 + samples.Length * 2);
                    writer.Write("WAVE".ToCharArray());
                    writer.Write("fmt ".ToCharArray());
                    writer.Write(16);
                    writer.Write((ushort)1);
                    writer.Write((ushort)channels);
                    writer.Write(frequency);
                    writer.Write(frequency * channels * 2);
                    writer.Write((ushort)(channels * 2));
                    writer.Write((ushort)16);
                    writer.Write("data".ToCharArray());
                    writer.Write(samples.Length * 2);

                    foreach (var sample in samples)
                    {
                        writer.Write((short)(sample * short.MaxValue));
                    }
                }
                return memoryStream.ToArray();
            }
        }

        private bool IsCommand(string input)
        {
            string[] commandKeywords = {
                // Offensive actions
                "use", "cast", "attack", "fire", "shoot", "strike", "hit", "blast", "burn", "explode",
                "launch", "throw", "smash", "punch", "kick", "slash", "stab", "pierce", "destroy",
                
                // Defensive actions
                "shield", "protect", "defend", "guard", "cover", "block", "dodge", "evade", "avoid",
                "save", "help", "support", "heal", "healing", "buff", "buffing", "boost", "boosted",
                
                // Targeting
                "target", "focus", "aim", "at", "towards", "against", "on", "onto", "into",
                "enemy", "opponent", "foe", "monster", "boss", "creature", "demon", "dragon",
                
                // Combat state
                "fight", "battle", "combat", "engage", "engage in", "start", "begin", "initiate",
                
                // Numbers and targeting specific enemies
                "one", "two", "1", "2", "first", "second", "primary", "secondary",
                
                // Common combat phrases
                "take down", "take out", "deal with", "handle", "finish off", "eliminate",
                "get rid of", "remove", "destroy", "defeat", "beat", "overcome",
                
                // Support phrases
                "back me up", "cover me", "watch my back", "protect me", "help me",
                "give me cover", "keep me safe", "defend me", "save me"
            };
            string lowered = input.ToLower().Trim();
            return commandKeywords.Any(keyword => lowered.Contains(keyword));
        }

         private string ConstructCommandPrompt(string message)
     {
         return @"Convert this input into a combat command. Only respond with one of these exact commands:
         - enemy1
         - enemy2
         - fireball
         - shield

         Input: " + message + "\n\nCommand:";
     }

        private string ConstructDialoguePrompt(string message)
        {
            return @"Respond to the player's question while staying in character. Keep responses under 3 sentences.

            Question: " + message;
        }

        private async Task ProcessInput(string userInput)
        {
            try
            {
                if (IsCommand(userInput))
                {
                    string commandResponse = await llmCharacter.Chat(ConstructCommandPrompt(userInput));
                    commandResponse = commandResponse.Trim().ToLower();
                    Debug.Log("sys response "+ commandResponse);
                    if (commandResponse.Contains("enemy1")  || 
                            commandResponse.Contains("enemy2") || 
                            commandResponse.Contains("fireball") || 
                            commandResponse.Contains("shield"))
                        {
                        _responseText.text = commandResponse;
                        if (CombatManager.instance != null)
                        {
                            CombatManager.instance.ProcessCommand(commandResponse);
                        }
                    }
                    else
                    {
                        _responseText.text = "I don't understand that command. Try: use fireball, use shield, target enemy 1, or target enemy 2";
                    }
                }
                else
                {
                    string dialogueResponse = await llmCharacter.Chat(ConstructDialoguePrompt(userInput));
                    _responseText.text = dialogueResponse;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error processing input: {e.Message}");
                _responseText.text = "Error processing your request";
            }
        }

        public async Task SendHuggingFaceRequest()
        {
            try
            {
                if (_isProcessing) return;
                _isProcessing = true;
                _sendRequestButton.interactable = false;

                string userInput = _inputField.text;
                if (string.IsNullOrEmpty(userInput))
                {
                    _responseText.text = "Please enter a message or use voice input";
                    return;
                }

                await ProcessInput(userInput);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in request: {e.Message}");
                _responseText.text = "Failed to process request";
            }
            finally
            {
                _isProcessing = false;
                _sendRequestButton.interactable = true;
            }
        }

        public void OnButtonClick_SendRequest()
        {
            _ = SendHuggingFaceRequest();
        }
    }
} 
