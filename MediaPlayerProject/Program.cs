using MediaPlayerProject;
using System;
using System.Text;
using WMPLib;

namespace ConsoleMusicPlayer
{
    internal class Program
    {
        public static WMPPlayState playerPlayStatus { get; set; }
        public static PlayerStatus playerStatus { get; set; }

        /// <summary>
        /// Event handler of windows media player PlayStateChange event
        /// </summary>
        /// <param name="NewState"></param>
        private static void Player_PlayStateChange(int NewState)
        {
            playerPlayStatus = (WMPPlayState)NewState;
        }

        /// <summary>
        /// Event handler of windows media player MediaError event
        /// </summary>
        /// <param name="pMediaObject"></param>
        private static void Player_MediaError(object pMediaObject)
        {
            playerStatus = PlayerStatus.Error;
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

        /// <summary>
        /// Simple text parser with cancellation
        /// </summary>
        /// <param name="CancelInputKey"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string SimpleUserInputParser(ConsoleKey CancelInputKey)
        {
            ConsoleKeyInfo cki_Key;
            var inputStr = new StringBuilder();
            int offset = Console.CursorLeft;
            while ((cki_Key = Console.ReadKey(true)).Key != CancelInputKey)
            {
                int location = Console.CursorLeft - offset;
                if (cki_Key.Key == ConsoleKey.Enter)
                {
                    return inputStr.ToString();
                }

                if (cki_Key.Key == ConsoleKey.Backspace && location > 0)
                {
                    inputStr.Remove(location - 1, 1);
                    Console.CursorLeft = offset;
                    Console.Write(new string(' ', inputStr.Length + 1));
                    Console.CursorLeft = offset;
                    Console.Write(inputStr.ToString());
                    Console.CursorLeft = location + offset - 1;
                }

                if (cki_Key.Key == ConsoleKey.LeftArrow && location > 0)
                {
                    Console.CursorLeft--;
                }

                if (cki_Key.Key == ConsoleKey.RightArrow && location < inputStr.Length)
                {
                    Console.CursorLeft++;
                }

                if (cki_Key.Key == ConsoleKey.Delete && location < inputStr.Length)
                {
                    inputStr.Remove(location, 1);
                    Console.CursorLeft = offset;
                    Console.Write(new string(' ', inputStr.Length + 1));
                    Console.CursorLeft = offset;
                    Console.Write(inputStr.ToString());
                    Console.CursorLeft = offset + location;
                }

                if (cki_Key.Key != ConsoleKey.Tab &&
                   (char.IsLetterOrDigit(cki_Key.KeyChar) ||
                    char.IsWhiteSpace(cki_Key.KeyChar) ||
                    cki_Key.Key == ConsoleKey.RightWindows ||
                    char.IsPunctuation(cki_Key.KeyChar))
                    )
                {
                    inputStr.Insert(location, cki_Key.KeyChar);
                    Console.CursorLeft = offset;
                    Console.Write(inputStr.ToString());
                    Console.CursorLeft = location + offset + 1;
                }
            }
            if (cki_Key.Key == CancelInputKey)
            {
                playerStatus = PlayerStatus.Abort;
                throw new Exception($"Processing is gestopt. {StringResources.MessageRepeat} later");
            }

            return inputStr.ToString();
        }

        /// <summary>
        /// Change the volume level of windows media player with recurcive error message handling
        /// </summary>
        /// <param name="player"></param>
        /// <param name="hoffset"></param>
        private static void ChangeVolume(WindowsMediaPlayer player, int hoffset)
        {
            Console.CursorTop = hoffset;
            Console.CursorLeft = 0;
            Console.Write($"{StringResources.InputVolumeLevel} Volume({player.settings.volume}):");
            int offset = Console.CursorLeft;
            string inputLevel = SimpleUserInputParser(ConsoleKey.Escape);

            if (int.TryParse(inputLevel, out int level))
            {
                if (level >= 0 && level <= 100)
                {
                    player.settings.volume = level;
                }
                else
                {
                    Console.CursorLeft = offset;
                    Console.CursorTop = hoffset;
                    Console.WriteLine(new String(' ', inputLevel.Length));
                    Console.Write($"{StringResources.VolumeLevelInfo} {StringResources.MessageRepeat}");
                    ChangeVolume(player, hoffset);
                }
            }
            else
            {
                Console.CursorLeft = offset;
                Console.CursorTop = hoffset;
                Console.WriteLine(new String(' ', inputLevel.Length));
                Console.Write($"{StringResources.VolumeLevelErrorInfo} {StringResources.MessageRepeat}");
                ChangeVolume(player, hoffset);
            }
        }

        /// <summary>
        ///  Display meta data of current media source
        /// </summary>
        /// <param name="media"></param>
        /// <param name="metadatakeys"></param>
        private static void PrintMetaData(IWMPMedia media, string[] metadatakeys)
        {
            foreach (string key in metadatakeys)
            {
                Console.WriteLine($"{key} : {media.getItemInfo(key)}");
            }
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
            int offset = Console.CursorLeft;
            try
            {
                while (playerStatus != PlayerStatus.Abort)
                {
                    if (playerPlayStatus == WMPPlayState.wmppsUndefined ||
                        (playerPlayStatus == WMPPlayState.wmppsReady &&
                        playerStatus == PlayerStatus.Error))
                    {
                        Console.Clear();
                        Console.CursorLeft = offset;
                        Console.WriteLine(StringResources.MediaPlayerLogo);
                        Console.WriteLine(StringResources.MediaPlayerPicture);
                        if (playerStatus == PlayerStatus.Error)
                        {
                            Console.WriteLine(StringResources.MediaErrorInfo);
                        }
                        else
                        {
                            Console.WriteLine();
                        }

                        Console.WriteLine();
                        Console.WriteLine();
                        Console.Write(StringResources.ShortUserMenu);

                        cki_Key = Console.ReadKey(true);
                        userKey = TranslateInputToUserKeys(cki_Key.KeyChar);

                        if ((userKey != UserKeys.Esc) && (userKey != UserKeys.OpenFile))
                        {
                            userKey = UserKeys.Unknown;
                        }
                        Console.Write(userKey.ToString());
                    }
                    else if (playerPlayStatus != WMPPlayState.wmppsUndefined &&
                        playerPlayStatus != WMPPlayState.wmppsTransitioning &&
                        playerStatus != PlayerStatus.None &&
                        playerStatus != PlayerStatus.Error)
                    {
                        Console.Clear();
                        Console.CursorLeft = offset;
                        Console.WriteLine(StringResources.MediaPlayerLogo);

                        if (userKey != UserKeys.None
                            && playerPlayStatus == WMPPlayState.wmppsStopped)
                        {
                            Console.WriteLine(StringResources.MediaPlayerPicture);
                        }
                        else if (userKey == UserKeys.None ||
                             playerPlayStatus == WMPPlayState.wmppsPlaying ||
                             playerPlayStatus == WMPPlayState.wmppsPaused ||
                             playerPlayStatus == WMPPlayState.wmppsScanForward)
                        {
                            Console.WriteLine();
                            PrintMetaData(player.currentMedia, StringResources.metadataquery);
                        }

                        if (player.settings.mute)
                        {
                            Console.WriteLine(StringResources.MutingInfo);
                        }
                        else
                        {
                            Console.WriteLine();
                        }
                        if (player.settings.volume == 0)
                        {
                            Console.WriteLine(StringResources.VolumeNullLevelInfo);
                        }
                        else
                        {
                            Console.WriteLine();
                        }

                        Console.WriteLine();
                        Console.Write(StringResources.LongUserMenu);

                        cki_Key = Console.ReadKey(true);
                        userKey = TranslateInputToUserKeys(cki_Key.KeyChar);
                        Console.Write(userKey.ToString());
                    }
                    switch (userKey)
                    {
                        case UserKeys.Esc:
                            {
                                playerStatus = PlayerStatus.Abort;
                                throw new ApplicationException($"Processing is gestopt. {StringResources.MessageRepeat} later");
                            }

                        case UserKeys.OpenFile:
                            {
                                string musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                                Console.WriteLine();
                                Console.WriteLine(StringResources.InputAudioFile);

                                string inputFileName = SimpleUserInputParser(ConsoleKey.Escape);
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
                                ChangeVolume(player, Console.CursorTop);
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
                                Console.WriteLine($"Een verkeerde toets was gedrukt. {StringResources.MessageRepeat}");
                                break;
                            }
                    }
                }
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
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