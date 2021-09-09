using System;
using System.IO;
using CranchyLib.SaveFile; //CranchyLib.Savefile.dll
using Newtonsoft.Json.Linq; //Included with CranchyLib.Savefile.dll (Merged using ILMerge)

namespace SaveFileReset
{
    class Program
    {
        static void Main()
        {
            Console.Clear();
            IncreaseConsoleBufferSize();
            Console.WriteLine("                SSSSS  EEEEEEE RRRRRR  VV     VV EEEEEEE RRRRRR  NN   NN   AAA   MM    MM EEEEEEE ");
            Console.WriteLine("               SS      EE      RR   RR VV     VV EE      RR   RR NNN  NN  AAAAA  MMM  MMM EE      ");
            Console.WriteLine("                SSSSS  EEEEE   RRRRRR   VV   VV  EEEEE   RRRRRR  NN N NN AA   AA MM MM MM EEEEE   ");
            Console.WriteLine("                    SS EE      RR  RR    VV VV   EE      RR  RR  NN  NNN AAAAAAA MM    MM EE      ");
            Console.WriteLine("                SSSSS  EEEEEEE RR   RR    VVV    EEEEEEE RR   RR NN   NN AA   AA MM    MM EEEEEEE ");
            Console.WriteLine("                                      SaveFile Reset Tool - Zero Progress                         ");
            if(NetServices.Platform == null)
            {
                Console.Write("\n\nSelect Your Platform:\n[1] Steam\n[2] Microsoft Store\n> ");
                switch(Console.ReadLine())
                {
                    case "1":
                        NetServices.Platform = "steam";
                        break;

                    case "2":
                        NetServices.Platform = "grdk";
                        break;

                    default:
                        Console.WriteLine("Please, Specify Valid platform (1 or 2)\nPress ENTER to continue...");
                        Console.ReadLine();
                        Main();
                        break;
                }
            }
            Console.Write("\nbhvrSession=");
            string bhvrSession = Console.ReadLine();

            if (bhvrSession.Length < 256)
            {
                Console.WriteLine("\n\nERROR: bhvrSession Length can't be less then 256 symbols!\nPress ENTER to continue...");
                Console.ReadLine();
                Main();
            }
            else
            {

                bhvrSession = bhvrSession.Replace("bhvrSession", "").Replace("=", "").Replace(" ", "");
                string saveFileVersion = NetServices.REQUEST_GET_HEADER($"https://{NetServices.Platform}.live.bhvrdbd.com/api/v1/players/me/states/FullProfile/binary", $"bhvrSession={bhvrSession}");
                if (saveFileVersion == "ERROR")
                {
                    Console.WriteLine("Something went wrong, make sure bhvrSession is valid & properly pasted\nPress ENTER to continue...");
                    Console.ReadLine();
                    Main();
                }


                Console.WriteLine($"Profile Version: {saveFileVersion}\n\nTrying To Obtain playerUID...");
                string saveFileUserID = NetServices.REQUEST_GET($"https://{NetServices.Platform}.live.bhvrdbd.com/api/v1/players/me/states/FullProfile/binary", $"bhvrSession={bhvrSession}");
                if (saveFileUserID == "ERROR")
                {
                    Console.WriteLine("Something went wrong when program tried to obtain playerUID\nPress ENTER to continue...");
                    Console.ReadLine();
                    Main();
                }
                var JsFullProfile = JObject.Parse(SaveFile.DecryptSavefile(saveFileUserID));
                saveFileUserID = (string)JsFullProfile["playerUId"];


                Console.WriteLine($"UserID: {saveFileUserID}\n\nTrying To Inject SaveFile...");
                string saveFileResponse = NetServices.REQUEST_POST($"https://{NetServices.Platform}.live.bhvrdbd.com/api/v1/players/me/states/binary?schemaVersion=0&stateName=FullProfile&version={(Convert.ToInt32(saveFileVersion) + 1).ToString()}", $"bhvrSession={bhvrSession}", SaveFile.EncryptSavefile(SaveFile.Ressurect_All(Properties.Resources.OFFLINE_SAVEFILE, saveFileUserID)));
                if (saveFileResponse == "ERROR")
                {
                    Console.WriteLine("Something went wrong, make sure that bhvrSession is validated by EAC\nPress ENTER to continue...");
                    Console.ReadLine();
                    Main();
                }
                Console.WriteLine("Success!\nPress ENTER to continue...");
                Console.ReadLine();
                Environment.Exit(0);

            }
        }

        private static void IncreaseConsoleBufferSize()
        {
            Console.SetIn(new StreamReader(Console.OpenStandardInput(),
                               Console.InputEncoding,
                               false,
                               bufferSize: 1024));
        }
    }
}
