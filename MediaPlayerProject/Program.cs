using MediaPlayerProject;
using System;
using WMPLib;

namespace ConsoleMusicPlayer
{
    internal class Program
    {
        public static WMPPlayState playerPlayStatus { get; set; }
        public static PlayerStatus playerStatus { get; set; }
        public static string MediaInfo { get; set; }

        private const string messageRepeat = "Probeer het opnieuw...";
        private const string inputAudioFile = "Voer een folder in Music en filename (vb. dir\\file.mp3) in aub of Esc voor abort :";
        private const string inputVolumeLevel = "Voer een volume in aub of Esc voor abort :";

        private static void Player_PlayStateChange(int NewState)
        {
            playerPlayStatus = (WMPPlayState)NewState;
        }

        ///todo mw check object pMediaObject
        private static void Player_MediaError(object pMediaObject)
        {
            playerStatus = PlayerStatus.Error;
            MediaInfo = "Media is afwezig of onbekende mediafout";
        }

        /// <summary>
        /// Translate inputs user keys to processing keys
        /// </summary>
        /// <param name="KeyChar"></param>
        /// <returns></returns>
        public static UserKeys TranslateInputToUserKeys(char KeyChar)
        {
            switch (Char.ToUpper(KeyChar))
            {
                case '\u001b': return UserKeys.Esc;
                case 'F': return UserKeys.Forward;
                case 'V': return UserKeys.Volume;
                case 'M': return UserKeys.Mute;
                case 'O': return UserKeys.OpenFile;
                case 'P': return UserKeys.PlayPause;
                case 'S': return UserKeys.Stop;
                default:
                    {
                        return UserKeys.Unknown;
                    }
            }
        }
        ///todo string edit (left,right,del,backspace etc)
        public static string ReadAndThrowAlarmIfEsc()
        {
            string inputStr="";
            ConsoleKeyInfo cki_KeyError;
            while ((cki_KeyError = Console.ReadKey()).Key != ConsoleKey.Escape)
            {
                if (cki_KeyError.Key == ConsoleKey.Enter)
                {
                    return inputStr;
                }

                inputStr += cki_KeyError.KeyChar.ToString();

            }
            if (cki_KeyError.Key == ConsoleKey.Escape)
            {
                playerStatus = PlayerStatus.Abort;
                throw new Exception($"Processing is gestopt. {messageRepeat} later");
            }
          
            return inputStr;
        }

        private static void ChangeVolume(WindowsMediaPlayer player)
        {
            Console.WriteLine($"{inputVolumeLevel} Volume({player.settings.volume}) :");
            //  ConsoleKeyInfo cki_Key = ReadAndThrowAlarmIfEsc(); //Todo
            //  string inputLevel = cki_Key.KeyChar.ToString() + Console.ReadLine(); //Todo
            string inputLevel = ReadAndThrowAlarmIfEsc();
            if (int.TryParse(inputLevel, out int level))
            {
                if (level >= 0 && level <= 100)
                {
                    player.settings.volume = level;
                }
                else
                {
                    Console.WriteLine($"Volume waarde moet tussen 0 en 100 zijn. {messageRepeat}");
                    ChangeVolume(player);
                }
            }
            else
            {
                Console.WriteLine($"Geen geldig getal. {messageRepeat}");
                ChangeVolume(player);
            }
        }

        private static void PrintMetaData(IWMPMedia media)
        {
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
                        if (playerStatus == PlayerStatus.Error)
                        {
                            Console.WriteLine(MediaInfo);
                        }

                        if (userKey != UserKeys.Unknown)
                        {
                            Console.Write("Wilt een audio file open(o) of ESC om te kunnen stoppen ?");
                        }

                        cki_Key = Console.ReadKey();
                        userKey = TranslateInputToUserKeys(cki_Key.KeyChar);

                        if ((userKey != UserKeys.Esc) && (userKey != UserKeys.OpenFile))
                        {
                            userKey = UserKeys.Unknown;
                        }
                        
                    }
                    else if (playerPlayStatus != WMPPlayState.wmppsUndefined &&
                        playerPlayStatus != WMPPlayState.wmppsTransitioning &&
                        playerStatus != PlayerStatus.None &&
                        playerStatus != PlayerStatus.Error)
                    {
                        if (userKey == UserKeys.None)
                        {
                            PrintMetaData(player.currentMedia);
                        }

                        if (userKey != UserKeys.Unknown)
                        {
                            Console.WriteLine();
                            Console.Write("Wilt audio file Open(o), Play/Pause(p), Forward(f), Stop(s), Volume(v), Mute(m) of ESC om te kunnen stoppen ?");
                        }
                        if (player.settings.mute)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Geluid Uit. Tik op Mute(m) om  geluid terug Aan te doen.");
                        }
                        if (player.settings.volume == 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Volume (0). Zet volume op hoger waarde om geluid terug Aan te doen.");
                        }
                        cki_Key = Console.ReadKey();
                        userKey = TranslateInputToUserKeys(cki_Key.KeyChar);
                    }

                    switch (userKey)
                    {
                        case UserKeys.Esc:
                            {
                                playerStatus = PlayerStatus.Abort;
                                throw new Exception($"Processing is gestopt. {messageRepeat} later");
                            }

                        case UserKeys.OpenFile:
                            {
                                string musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                                Console.WriteLine();
                                Console.WriteLine(inputAudioFile);

                                
                                string inputFileName = ReadAndThrowAlarmIfEsc(); 
                                player.URL = System.IO.Path.Combine(musicFolder, inputFileName);

                                userKey = UserKeys.None;
                                playerStatus = PlayerStatus.Idle;
                                break;
                            }
                        case UserKeys.PlayPause:
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
                        case UserKeys.Forward:
                            {
                                if (playerPlayStatus == WMPPlayState.wmppsPlaying)
                                {
                                    player.controls.fastForward();
                                }

                                break;
                            }
                        case UserKeys.Stop:
                            {
                                player.controls.stop();
                                break;
                            }
                        case UserKeys.Volume:
                            {
                                Console.WriteLine();
                                ChangeVolume(player);
                                break;
                            }
                        case UserKeys.Mute:
                            {
                                player.settings.mute = !player.settings.mute;
                                break;
                            }
                        case UserKeys.None:
                            {
                                break;
                            }
                        default:
                            {
                                Console.WriteLine();
                                Console.WriteLine($"Een verkeerde toets was gedrukt. {messageRepeat}");
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
                if (player != null)
                {
                    player.controls.stop();
                }
            }
        }

    }
}