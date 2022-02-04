namespace MediaPlayerProject
{
    public static class StringResources
    {
        public static string MessageRepeat = "Probeer het opnieuw...";
        public static string InputAudioFile = "Voer een folder in MyMusic en filename (vb. dir\\file.mp3) of ESC voor abort :";
        public static string InputVolumeLevel = "Voer een <V>olume in of ESC voor abort :";
        public static string MediaErrorInfo = "Media is afwezig of onbekende mediafout";
        public static string ShortUserMenu = "Wilt een audio file <O>pen of ESC om te kunnen stoppen ?";
        public static string LongUserMenu = "Wilt audio file <O>pen, <P>lay/<P>ause, <F>orward, <S>top, <V>olume, <M>ute of ESC om te kunnen stoppen ?";
        public static string VolumeLevelInfo = "Volume waarde moet tussen 0 en 100 zijn.";
        public static string VolumeLevelErrorInfo = "Geen geldig volume level waarde.";
        public static string MutingInfo = "Muteren. Tik op <M>ute om  geluid terug Aan te doen.";
        public static string VolumeNullLevelInfo = "Volume (0). Zet <V>olume op hoger waarde om geluid terug Aan te doen.";

        public static string[] metadataquery = { "Author", "UserRating"
                , "Bitrate", "Duration", "FileSize", "FileType", "MediaType"
                , "Title", "WM/AlbumArtist", "WM/AlbumTitle", "WM/Genre"
                , "WM/Track", "WM/TrackNumber", "WM/Year" };

        public static string MediaPlayerLogo = @"
     |**\/**|        |*(*)             |*|
     |*\**/*| ___  __|*|_  __ _   _ __ |*| __ _ _   _  ___ _ __
     |*|\/|*|/***\/****|*|/**`*| |*'**\|*|/**`*|*| |*|/***\*'**|
     |*|  |*|* **/*(_|*|*|*(_|*| |*|_)*|*|*(_|*|*|_|*|* **/*|
     |*|  |*|\***|\**,*|*|\**,*| |*.**/|*|\**,*|\**,*|\*** *|";

        public static string MediaPlayerPicture = @"
                    ___                                          ___
 __________________/  /                       __________________/  /
| _    _______    /  /  Use <ESC> for Exit   | _    _______    /  /
|(_) .d########b.// )| _____________________ |(_) .d########b.// )|
|  .d############//  ||        _____        ||  .d############//  |
| .d######++####//b. ||() || [PIONEER] || ()|| .d######++####//b. |
| 9######(  )#_//##P ||()|__|  | = |  |__|()|| 9######(  )#_//##P |
| 'b######++#/_/##d' ||() ||   | = |   || ()|| 'b######++#/_/##d' |
|  ^9############P^  ||   ||   |___|   ||   ||  ^9############P^  |
|  _^9a#######aP^    || _   _____..__   _   ||  _^9a#######aP^    |
| |_|  `------`      || (_)|_____||__| (_)  || |_|  `______'      |
|  ___..___________  ||_____________________||  ___..___________  |
| |___||___________| |       Console         | |___||___________| |
|____________________|     Media Player      |____________________|";
    }
}