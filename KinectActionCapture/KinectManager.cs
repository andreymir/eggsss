using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KinectActionCapture.Calculation;
using KinectActionCapture.Game;
using KinectActionCapture.VoiceManipulation;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.IO;

namespace KinectActionCapture
{
    public class KinectManager : IKinectManager
    {
        private const double MIN_ENABLED_DISTANCE = 0.35;
        private const double DEATH_AREA_RADIUS = 0.2;

        private Skeleton[] skeletons = null;
        private KinectSensor kinectSensor;
        private GameState lastCapturedGameState = GameState.NoAreaSelected;
        private VoiceCommand lastRecognizedVoiceCommand = VoiceCommand.None;
        private RecognizerInfo kinectRecognizerInfo;
        private SpeechRecognitionEngine recognizer;
        private KinectAudioSource kinectSource;
        private Stream audioStream;
        private bool stopCalled = false;

        public StartResult StartKinect()
        {
            if (KinectSensor.KinectSensors.Count == 0)
            {
                return StartResult.NoKinectDetected;
            }

            if (KinectSensor.KinectSensors.Count > 1)
            {
                return StartResult.ManyKinectsDetected;
            }

            kinectSensor = KinectSensor.KinectSensors.Single();
            
            if (kinectSensor.Status != KinectStatus.Connected)
            {
                return StartResult.KinectError;
            }

            try
            {
                kinectSensor.SkeletonStream.Enable();

                kinectSensor.Start();
            }
            catch (InvalidOperationException)
            {
                return StartResult.KinectError;
            }

            kinectSensor.SkeletonFrameReady +=
                new EventHandler<SkeletonFrameReadyEventArgs>(
                myKinect_SkeletonFrameReady);

            kinectRecognizerInfo = findKinectRecognizerInfo();

            if (kinectRecognizerInfo != null)
            {
                recognizer = new SpeechRecognitionEngine(kinectRecognizerInfo);

                SetVoiceRecognizer();

                kinectSource = kinectSensor.AudioSource;
                kinectSource.BeamAngleMode = BeamAngleMode.Adaptive;
                audioStream = kinectSource.Start();
                recognizer.SetInputToAudioStream(audioStream, new SpeechAudioFormatInfo(
                                                                  EncodingFormat.Pcm, 16000, 16, 1,
                                                                  32000, 2, null));
                recognizer.RecognizeAsync(RecognizeMode.Multiple);

                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);
            }

            return StartResult.KinectStarted;
        }

        void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > 0.75f)
            {
                switch (e.Result.Text)
                {
                    case "Новая игра":
                    //case "New Game" :
                        lastRecognizedVoiceCommand = VoiceCommand.NewGame;
                        break;
                    case "Выход":
                    //case "Exit" :
                        lastRecognizedVoiceCommand = VoiceCommand.Exit;
                        break;
                    case "Пауза":
                    //case "Pause" :
                        lastRecognizedVoiceCommand = VoiceCommand.Pause;
                        break;
                    case "Продолжить":
                    //case "Continue" :
                        lastRecognizedVoiceCommand = VoiceCommand.Continue;
                        break;
                    default:
                        lastRecognizedVoiceCommand = VoiceCommand.None;
                        break;
                }   
            }
        }

        public GameState GetCurrentGameState()
        {
            return lastCapturedGameState;
        }

        public VoiceCommand GetCurrentVoiceCommand()
        {
            VoiceCommand command = lastRecognizedVoiceCommand;

            lastRecognizedVoiceCommand = VoiceCommand.None;

            return command;
        }


        public void StopKinect()
        {
            ShutdownSpeechRecognition();

            stopCalled = true;
            if (kinectSensor != null)
            {
                kinectSensor.Stop();
            }
        }

        private void ShutdownSpeechRecognition()
        {
            if (kinectSource != null)
            {
                kinectSource.Stop();
            }

            if (recognizer != null)
            {
                recognizer.RecognizeAsyncCancel();
                recognizer.RecognizeAsyncStop();
            }
        }

        private void SetVoiceRecognizer()
        {
            Choices commands = new Choices();

            //commands.Add("New Game", "Exit", "Pause", "Continue");
            commands.Add("Новая игра", "Выход", "Пауза", "Продолжить");

            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.Culture = kinectRecognizerInfo.Culture;
            grammarBuilder.Append(commands);
            Grammar grammar = new Grammar(grammarBuilder);

            recognizer.LoadGrammar(grammar);
        }

        private RecognizerInfo findKinectRecognizerInfo()
        {
            var recognizers = SpeechRecognitionEngine.InstalledRecognizers();
            foreach (RecognizerInfo recInfo in recognizers)
            {
                // look at each recognizer info value to find the one that works for Kinect
                //if (recInfo.AdditionalInfo.ContainsKey("Kinect"))
                {
                    //string details = recInfo.AdditionalInfo["Kinect"];
                    if (recInfo.Culture.Name == "ru-RU")
                        // If we get here we have found the info we want to use
                        return recInfo;
                }
            }
            return null;
        }

        private void myKinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (stopCalled)
            {
                return;
            }

            using (SkeletonFrame currentSkeletonFrame = e.OpenSkeletonFrame())
            {
                if (currentSkeletonFrame == null)
                {
                    lastCapturedGameState = GameState.NoAreaSelected;
                    return;
                }

                if (skeletons == null)
                {
                    skeletons = new Skeleton[currentSkeletonFrame.SkeletonArrayLength];
                }

                currentSkeletonFrame.CopySkeletonDataTo(skeletons);

                if (skeletons == null)
                {
                    lastCapturedGameState = GameState.NoAreaSelected;
                    return;
                }

                Skeleton foundSkeleton = skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);

                if (foundSkeleton == null)
                {
                    foundSkeleton = skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.PositionOnly);
                }

                if (foundSkeleton != null)
                {
                    lastCapturedGameState = GetGameState(foundSkeleton);
                    return;
                }

                lastCapturedGameState = GameState.NoAreaSelected;
            }
        }

        private GameState GetGameState(Skeleton skeleton)
        {
            Joint? leftHand = GetAvailableJoint(skeleton.Joints[JointType.HandLeft], skeleton.Joints[JointType.WristLeft]);
            Joint? rightHand = GetAvailableJoint(skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.WristRight]);

            if (leftHand.HasValue && rightHand.HasValue)
            {
                if (MathProcessor.CalculateDistance(leftHand.Value.Position, rightHand.Value.Position) <= MIN_ENABLED_DISTANCE)
                {
                    Point centralPoint = MathProcessor.CalculateMiddlePoint(leftHand.Value.Position,
                                                                            rightHand.Value.Position);

                    centralPoint = GetCorrectedCentralPoint(centralPoint, skeleton.Joints[JointType.Spine]);

                    if (!IsPointInDeathZone(centralPoint))
                    {
                        if (centralPoint.X < 0.0 && centralPoint.Y < 0.0)
                        {
                            return GameState.FirstAreaSelected;
                        }

                        if (centralPoint.X < 0.0 && centralPoint.Y > 0.0)
                        {
                            return GameState.SecondAreaSelected;
                        }

                        if (centralPoint.X > 0.0 && centralPoint.Y > 0.0)
                        {
                            return GameState.ThirdAreaSelected;
                        }

                        if (centralPoint.X > 0.0 && centralPoint.Y < 0.0)
                        {
                            return GameState.FourthAreaSelected;
                        }
                    }

                    return GameState.NoAreaSelected;
                }
            }

            return GameState.NoAreaSelected;
        }

        private bool IsPointInDeathZone(Point centralPoint)
        {
            return MathProcessor.CalculateDistance(new Point(centralPoint.X, centralPoint.Y, 0.0), new Point()) < DEATH_AREA_RADIUS ||
                    Math.Abs(centralPoint.X) < DEATH_AREA_RADIUS ||
                    Math.Abs(centralPoint.Y) < DEATH_AREA_RADIUS / 2.0;
        }

        private Joint? GetAvailableJoint(Joint primary, Joint alternative)
        {
            if (primary.TrackingState != JointTrackingState.NotTracked)
            {
                return primary;
            }

            if (alternative.TrackingState == JointTrackingState.Tracked)
            {
                return alternative;
            }

            return null;
        }

        private Point GetCorrectedCentralPoint(Point currentCentralPoint, Joint spine)
        {
            Point correctedCentralPoint = new Point(currentCentralPoint.X, currentCentralPoint.Y, currentCentralPoint.Z);

            double dX = spine.Position.X;
            double dY = spine.Position.Y;

            if (spine.TrackingState != JointTrackingState.NotTracked)
            {
                correctedCentralPoint.X -= dX;
                correctedCentralPoint.Y -= dY;
            }

            return correctedCentralPoint;
        }
    }
}
