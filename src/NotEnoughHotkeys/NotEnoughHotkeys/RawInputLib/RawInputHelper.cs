using Microsoft.Win32;
using System;
using System.Management;

namespace NotEnoughHotkeys.RawInputLib
{
    public static class RawInputHelper
    {
        public static Tuple<string,string> GetKeyboardInfo(string hwid)
        {
            Tuple<string, string> result = new Tuple<string, string>("","");
            ManagementObjectSearcher win32Monitor = new ManagementObjectSearcher("select * from Win32_Keyboard");
            hwid = hwid.Replace("#", "\\");


            foreach (ManagementObject obj in win32Monitor.Get())
            {
                string chwid = ((string)obj["DeviceID"]);
                chwid = chwid.Remove(chwid.LastIndexOf("\\"));
                chwid = chwid.Remove(chwid.LastIndexOf("\\"));

                if (hwid.Contains(chwid))
                {
                    result = new Tuple<string,string>((string)obj["Description"], KbdLayoutFromId((string)obj["Layout"]));
                }
            }
            return result;
        }

        private static string KbdLayoutFromId(string code)
        {
            switch (code)
            {
                case "0000041C":
                    return "Albanian";
                case "00000401":
                    return "Arabic (101)";
                case "00010401":
                    return "Arabic (102)";
                case "00020401":
                    return "Arabic (102) Azerty";
                case "0000042B":
                    return "Armenian eastern";
                case "0001042B":
                    return "Armenian Western";
                case "0000044D":
                    return "Assamese - inscript";
                case "0000082C":
                    return "Azeri Cyrillic";
                case "0000042C":
                    return "Azeri Latin";
                case "0000046D":
                    return "Bashkir";
                case "00000423":
                    return "Belarusian";
                case "0000080C":
                    return "Belgian French";
                case "00000813":
                    return "Belgian (period)";
                case "0001080C":
                    return "Belgian (comma)";
                case "00000445":
                    return "Bengali";
                case "00010445":
                    return "Bengali - inscript (legacy)";
                case "00020445":
                    return "Bengali - inscript";
                case "0000201A":
                    return "Bosnian (cyrillic)";
                case "00030402":
                    return "Bulgarian";
                case "00000402":
                    return "Bulgarian(typewriter)";
                case "00010402":
                    return "Bulgarian (latin)";
                case "00020402":
                    return "Bulgarian (phonetic)";
                case "00040402":
                    return "Bulgarian (phonetic traditional)";
                case "00011009":
                    return "Canada Multilingual";
                case "00001009":
                    return "Canada French";
                case "00000C0C":
                    return "Canada French (legacy)";
                case "00000404":
                    return "Chinese (traditional) - us keyboard";
                case "00000804":
                    return "Chinese (simplified) -us keyboard";
                case "00000C04":
                    return "Chinese (traditional, hong kong s.a.r.) - us keyboard";
                case "00001004":
                    return "Chinese (simplified, singapore) - us keyboard";
                case "00001404":
                    return "Chinese (traditional, macao s.a.r.) - us keyboard";
                case "00000405":
                    return "Czech";
                case "00020405":
                    return "Czech programmers";
                case "00010405":
                    return "Czech (qwerty)";
                case "0000041A":
                    return "Croatian";
                case "00000439":
                    return "Deanagari - inscript";
                case "00000406":
                    return "Danish";
                case "00000465":
                    return "Divehi phonetic";
                case "00010465":
                    return "Divehi typewriter";
                case "00000413":
                    return "Dutch";
                case "00000425":
                    return "Estonian";
                case "00000438":
                    return "Faeroese";
                case "0000040B":
                    return "Finnish";
                case "0001083B":
                    return "Finnish with sami";
                case "0000040C":
                    return "French";
                case "00011809":
                    return "Gaelic";
                case "00000437":
                    return "Georgian";
                case "00020437":
                    return "Georgian (ergonomic)";
                case "00010437":
                    return "Georgian (qwerty)";
                case "00000407":
                    return "German";
                case "00010407":
                    return "German (ibm)";
                case "0000046F":
                    return "Greenlandic";
                case "00000468":
                    return "Hausa";
                case "0000040D":
                    return "Hebrew";
                case "00010439":
                    return "Hindi traditional";
                case "00000408":
                    return "Greek";
                case "00010408":
                    return "Greek (220)";
                case "00030408":
                    return "Greek (220) latin";
                case "00020408":
                    return "Greek (319)";
                case "00040408":
                    return "Greek (319) latin";
                case "00050408":
                    return "Greek latin";
                case "00060408":
                    return "Greek polyonic";
                case "00000447":
                    return "Gujarati";
                case "0000040E":
                    return "Hungarian";
                case "0001040E":
                    return "Hungarian 101 key";
                case "0000040F":
                    return "Icelandic";
                case "00000470":
                    return "Igbo";
                case "0000085D":
                    return "Inuktitut - latin";
                case "0001045D":
                    return "Inuktitut - naqittaut";
                case "00001809":
                    return "Irish";
                case "00000410":
                    return "Italian";
                case "00010410":
                    return "Italian (142)";
                case "00000411":
                    return "Japanese";
                case "0000044B":
                    return "Kannada";
                case "0000043F":
                    return "Kazakh";
                case "00000453":
                    return "Khmer";
                case "00000412":
                    return "Korean";
                case "00000440":
                    return "Kyrgyz cyrillic";
                case "00000454":
                    return "Lao";
                case "0000080A":
                    return "Latin america";
                case "00000426":
                    return "Latvian";
                case "00010426":
                    return "Latvian (qwerty)";
                case "00010427":
                    return "Lithuanian";
                case "00000427":
                    return "Lithuanian ibm";
                case "00020427":
                    return "Lithuanian standard";
                case "0000046E":
                    return "Luxembourgish";
                case "0000042F":
                    return "Macedonian (fyrom)";
                case "0001042F":
                    return "Macedonian (fyrom) - standard";
                case "0000044C":
                    return "Malayalam";
                case "0000043A":
                    return "Maltese 47-key";
                case "0001043A":
                    return "Maltese 48-key";
                case "0000044E":
                    return "Marathi";
                case "00000481":
                    return "Maroi";
                case "00000450":
                    return "Mongolian cyrillic";
                case "00000850":
                    return "Mongolian (mongolian script)";
                case "00000461":
                    return "Nepali";
                case "00000414":
                    return "Norwegian";
                case "0000043B":
                    return "Norwegian with sami";
                case "00000448":
                    return "Oriya";
                case "00000463":
                    return "Pashto (afghanistan)";
                case "00000429":
                    return "Persian";
                case "00000415":
                    return "Polish (programmers)";
                case "00010415":
                    return "Polish (214)";
                case "00000816":
                    return "Portuguese";
                case "00000416":
                    return "Portuguese (brazillian abnt)";
                case "00010416":
                    return "Portuguese (brazillian abnt2)";
                case "00000446":
                    return "Punjabi";
                case "00010418":
                    return "Romanian (standard)";
                case "00000418":
                    return "Romanian (legacy)";
                case "00020418":
                    return "Romanian (programmers)";
                case "00000419":
                    return "Russian";
                case "00010419":
                    return "Russian (typewriter)";
                case "0002083B":
                    return "Sami extended finland-sweden";
                case "0001043B":
                    return "Sami extended norway";
                case "00000C1A":
                    return "Serbian (cyrillic)";
                case "0000081A":
                    return "Serbian (latin)";
                case "0000046C":
                    return "Sesotho sa Leboa";
                case "00000432":
                    return "Setswana";
                case "0000045B":
                    return "Sinhala";
                case "0001045B":
                    return "Sinhala -Wij 9";
                case "0000041B":
                    return "Slovak";
                case "0001041B":
                    return "Slovak (qwerty)";
                case "00000424":
                    return "Slovenian";
                case "0001042E":
                    return "Sorbian extended";
                case "0002042E":
                    return "Sorbian standard";
                case "0000042E":
                    return "Sorbian standard (legacy)";
                case "0000040A":
                    return "Spanish";
                case "0001040A":
                    return "Spanish variation";
                case "0000041D":
                    return "Swedish";
                case "0000083B":
                    return "Swedish with sami";
                case "00000807":
                    return "Swiss german";
                case "0000100C":
                    return "Swiss french";
                case "0000045A":
                    return "Syriac";
                case "0001045A":
                    return "Syriac phonetic";
                case "00000428":
                    return "Tajik";
                case "00000449":
                    return "Tamil";
                case "00000444":
                    return "Tatar";
                case "0000044A":
                    return "Telugu";
                case "0000041E":
                    return "Thai Kedmanee";
                case "0002041E":
                    return "Thai Kedmanee (non-shiftlock)";
                case "0001041E":
                    return "Thai Pattachote";
                case "0003041E":
                    return "Thai Pattachote (non-shiftlock)";
                case "00000451":
                    return "Tibetan (prc)";
                case "0001041F":
                    return "Turkish F";
                case "0000041F":
                    return "Turkish Q";
                case "00000442":
                    return "Turkmen";
                case "00000422":
                    return "Ukrainian";
                case "00020422":
                    return "Ukrainian (enhanced)";
                case "00000809":
                    return "United Kingdom";
                case "00000452":
                    return "United Kingdom Extended";
                case "00000409":
                    return "United States";
                case "00010409":
                    return "United States - dvorak";
                case "00030409":
                    return "United States - dvorak left hand";
                case "00050409":
                    return "United States - dvorak right hand";
                case "00004009":
                    return "United States - india";
                case "00020409":
                    return "United States - international";
                case "00000420":
                    return "Urdu";
                case "00010480":
                    return "Uyghur";
                case "00000480":
                    return "Uyghur (legacy)";
                case "00000843":
                    return "Uzbek cyrillic";
                case "0000042A":
                    return "Vietnamese";
                case "00000485":
                    return "Yakut";
                case "0000046A":
                    return "Yoruba";
                case "00000488":
                    return "Wolof";

                default:
                    return "unknown";
            }
        }

        public static string GetDeviceName(string hwid)
        {
            try
            {
                var split = hwid.Substring(4).Split('#');
                RegistryKey reg = Registry.LocalMachine.OpenSubKey($"System\\CurrentControlSet\\Enum\\{split[0]}\\{split[1]}\\{split[2]}");
                var desc = reg.GetValue("DeviceDesc").ToString();
                return desc.Substring(desc.IndexOf(';')+1);
            }
            catch
            {
                return "";
            }
        }
    }
}
