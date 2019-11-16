//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Microsoft.CognitiveServices.Speech;
//using UnityEngine.UI;
//using System.Threading.Tasks;
//using System;

//public class SpeechInput : MonoBehaviour
//{

//	object threadLocker = new object();
//	bool waitingForSpeak;
//	string message;

//	void Start()
//	{
//        RecognizeSpeech().Wait();
//	}
//	public static async Task RecognizeSpeech()
//	{
//		// Creates an instance of a speech config with specified subscription key and service region.
//		// Replace with your own subscription key and service region (e.g., "westus").
//		var config = SpeechConfig.FromSubscription("61c926fcef7c4ad7a6d091640df7e05e", "westus");
//        using (var recognizer = new SpeechRecognizer(config))
//        {
//            // Starts speech recognition, and returns after a single utterance is recognized. The end of a
//            // single utterance is determined by listening for silence at the end or until a maximum of 15
//            // seconds of audio is processed.  The task returns the recognition text as result. 
//            // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
//            // shot recognition like command or query. 
//            // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
//            var result = await recognizer.RecognizeOnceAsync();

//            // Checks result.
//            if (result.Reason == ResultReason.RecognizedSpeech)
//            {
//                Console.WriteLine($"We recognized: {result.Text}");
//            }
//            else if (result.Reason == ResultReason.NoMatch)
//            {
//                Console.WriteLine($"NOMATCH: Speech could not be recognized.");
//            }
//            else if (result.Reason == ResultReason.Canceled)
//            {
//                var cancellation = CancellationDetails.FromResult(result);
//                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

//                if (cancellation.Reason == CancellationReason.Error)
//                {
//                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
//                    Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
//                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
//                }
//            }
//        }
//	}

//}
