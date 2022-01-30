using System;
using WMPLib;

namespace ConsoleMusicPlayer
{
    internal class Program
    {
        private const string messageRepeat = "Probeer het opnieuw";
        private const string inputAudioFile = "\nVoer een folder in Music + filename (vb. dir\\file.mp3) in aub of Esc voor abort :";
        private const string inputVolumeLevel = "\nVoer een volume in aub of Esc voor abort :";

        private static void Player_PlayStateChange(int NewState)
        {
            playerPlayStatus = (WMPLib.WMPPlayState)NewState;
        }

        private static void Player_MediaError(object pMediaObject)
        {
            IWMPMedia media = (IWMPMedia)pMediaObject;
            playerStatus = PlayerStatus.Error;
            MediaInfo = "Media is absent or unknown media error";
        }

        public enum PlayerStatus
        {
            None,
            Idle,
            Error,
            Abort
        }

        /// <summary>
        /// Enum with processing keys from menu ConsoleMusicPlayer
        /// </summary>
        public enum UserKeys
        {
            None = -1,
            Unknown = 0,
            Esc = 27,
            Forward = 70,
            Mute = 77,
            OpenFile = 79,
            PlayPause = 80,
            Reverse = 82,
            Stop = 83,
            Volume = 86
        };

        /// <summary>
        /// Translate inputs user keys to processing keys
        /// </summary>
        /// <param name="KeyChar"></param>
        /// <returns></returns>
        public static UserKeys TranslateInputToUserKeys(char KeyChar)
        {
            switch (KeyChar)
            {
                case ('\u001b'): return UserKeys.Esc;
                case ('f'):
                case ('F'): return UserKeys.Forward;
                case ('v'):
                case ('V'): return UserKeys.Volume;
                case ('m'):
                case ('M'): return UserKeys.Mute;
                case ('o'):
                case ('O'): return UserKeys.OpenFile;
                case ('p'):
                case ('P'): return UserKeys.PlayPause;
                case ('s'):
                case ('S'): return UserKeys.Stop;
                default:
                    {
                        return UserKeys.Unknown;
                    }
            }
            return UserKeys.None;
        }

        public static WMPLib.WMPPlayState playerPlayStatus { get; set; }
        public static PlayerStatus playerStatus { get; set; }
        public static string MediaInfo { get; set; }

        public static ConsoleKeyInfo ReadAndThrowAlarmIfEsc()
        {
            ConsoleKeyInfo cki_KeyError = Console.ReadKey();
            if (cki_KeyError.Key == ConsoleKey.Escape)
            {
                playerStatus = PlayerStatus.Abort;
                throw new Exception("Processing is gestopt. " + messageRepeat + " later");
            }
            return cki_KeyError;
        }

        private static void Main(string[] args)
        {
            ConsoleKeyInfo cki_Key;
            WindowsMediaPlayer player = new WindowsMediaPlayer();
            player.PlayStateChange +=
                new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);
            player.MediaError +=
            new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
            UserKeys userKey = UserKeys.None;
            try
            {
                while (playerStatus != PlayerStatus.Abort)
                {
                    if (playerPlayStatus == WMPPlayState.wmppsUndefined ||
                        (playerPlayStatus == WMPPlayState.wmppsReady &&
                        playerStatus == PlayerStatus.Error))
                    {
                        if (playerStatus == PlayerStatus.Error) Console.WriteLine("Media Error");
                        if (userKey != UserKeys.Unknown) Console.Write("Wilt een audio file open(o) of ESC om te kunnen stoppen ?");

                        if (ReadAndThrowAlarmIfEsc().Key == ConsoleKey.O)
                        { userKey = UserKeys.OpenFile; }
                        else { userKey = UserKeys.Unknown; }
                    }
                    else if (playerPlayStatus != WMPPlayState.wmppsUndefined &&
                        playerPlayStatus != WMPPlayState.wmppsTransitioning &&/*
                        playerPlayStatus != WMPPlayState.wmppsReconnecting &&
                        playerPlayStatus != WMPPlayState.wmppsLast &&*/
                        playerStatus != PlayerStatus.None &&
                        playerStatus != PlayerStatus.Error)
                    {
                        if (userKey == UserKeys.None)
                        {
                            IWMPMedia media = player.currentMedia;
                            Console.WriteLine($"Author : {media.getItemInfo("Author")}");
                            Console.WriteLine($"Bitrate : {media.getItemInfo("Bitrate")}");
                            Console.WriteLine($"Duration : {media.getItemInfo("Duration")}");
                            Console.WriteLine($"FileSize : {media.getItemInfo("FileSize")}");
                            Console.WriteLine($"FileType : {media.getItemInfo("FileType")}");
                            Console.WriteLine($"MediaType : {media.getItemInfo("MediaType")}");
                            Console.WriteLine($"Title : {media.getItemInfo("Title")}");
                            Console.WriteLine($"WM/Genre : {media.getItemInfo("WM/Genre")}");
                            Console.WriteLine($"WM/Track : {media.getItemInfo("WM/Track")}");
                            Console.WriteLine($"WM/Year : {media.getItemInfo("WM/Year")}");
                            if (player.settings.mute) Console.WriteLine("Mute Aan. Tik op m om mute uitzetten.");
                        }

                        if (userKey != UserKeys.Unknown) Console.Write("\nWilt audio file Open(o), Play/Pause(p), Forward(f), Stop(s), Volume(v), Mute(m) of ESC om te kunnen stoppen ?");
                        cki_Key = Console.ReadKey();
                        userKey = TranslateInputToUserKeys(cki_Key.KeyChar);
                    }

                    switch (userKey)
                    {
                        case (UserKeys.Esc):
                            {
                                playerStatus = PlayerStatus.Abort;
                                throw new Exception("Processing is gestopt. " + messageRepeat + " later");
                            }

                        case (UserKeys.OpenFile):
                            {
                                string musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                                Console.WriteLine(inputAudioFile);
                                ConsoleKeyInfo cki_Key_Stop = ReadAndThrowAlarmIfEsc();
                                string inputFileName = cki_Key_Stop.KeyChar.ToString() + Console.ReadLine();
                                player.URL = System.IO.Path.Combine(musicFolder, inputFileName);
                                userKey = UserKeys.None;
                                playerStatus = PlayerStatus.Idle;
                                break;
                            }
                        case (UserKeys.PlayPause):
                            {
                                if (playerPlayStatus == WMPPlayState.wmppsPlaying)
                                {
                                    player.controls.pause();
                                }
                                else
                                {
                                    player.controls.play();
                                }
                                break;
                            }
                        case (UserKeys.Forward):
                            {
                                if (playerPlayStatus == WMPPlayState.wmppsPlaying)
                                {
                                    player.controls.stop();
                                    player.controls.fastForward();
                                }

                                break;
                            }
                        case (UserKeys.Stop):
                            {
                                player.controls.stop();
                                break;
                            }
                        case (UserKeys.Volume):
                            {
                                Console.WriteLine($"{inputVolumeLevel} Volume({player.settings.volume}) :");
                                ConsoleKeyInfo cki_Key_Stop = ReadAndThrowAlarmIfEsc();
                                string inputLevel = cki_Key_Stop.KeyChar.ToString() + Console.ReadLine();
                                player.settings.volume = int.Parse(inputLevel);
                                break;
                            }
                        case (UserKeys.Mute):
                            {
                                player.settings.mute = !player.settings.mute;
                                break;
                            }
                        case (UserKeys.None):
                            {
                                break;
                            }
                        default:
                            {
                                Console.WriteLine();
                                Console.WriteLine("Een verkeerde toets was gedrukt. " + messageRepeat);
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex.Message);
            }
            finally
            {
                player.controls.stop();
            }
        }
    }
}