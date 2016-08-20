using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client;
using System.Xml;

namespace Penguin
{
    class Program
    {
        static void Main(string[] args)
        {
            User Client = new User();
            Client.Name = "Olena";
            Console.WriteLine(Client.Name);
        }

        static void ParseDictionary()
        {
            XmlTextReader reader = new XmlTextReader("dictionary.xdxf");
            reader.WhitespaceHandling = WhitespaceHandling.None;
            while (reader.Read())
            {
                if (reader.Name == "ar" && reader.NodeType == XmlNodeType.Element)
                {
                    Boolean translation = false;
                    while (reader.Read() && reader.Name != "ar")
                    {

                        if (reader.NodeType == XmlNodeType.Text)
                        {
                            string temp = reader.Value;
                            if (translation == false)
                            {
                                Console.WriteLine("Eng :{0}", temp);
                                translation = true;
                            }
                            else
                            {
                                temp = temp.Replace("\n", "");
                                Console.WriteLine("UKR :{0}", temp);
                            }

                        }

                    }

                }

            }



        }

    }

}

namespace Client
{
    class User
    {
        public string Name
        {
            set;
            get;
        }

        public string Surname
        {
            set;
            get;
        }

        public string Login
        {
            set;
            get;
        }

        public string Password
        {
            set;
            get;
        }

        public uint AmountWordsOfDay
        {
            set;
            get;
        }

        public DateTime DateOfBorned
        {
            set;
            get;
        }

        public Dictionary<string, string> LearnedWords;

        public void UpdateUser()
        {
            string choice = "-1";

            Console.WriteLine("\t Menu:");
            Console.WriteLine("1. Change name (Enter -1);");
            Console.WriteLine("2. Change surname (Enter -2);");
            Console.WriteLine("3. Change login (Enter -3);");
            Console.WriteLine("4. Change password (Enter -4);");
            Console.WriteLine("5. Change amount of word to learn (Enter -5);");
            Console.WriteLine("6. Change date of birthday (Enter - 6);");
            Console.WriteLine("7. Change clean learning words(Enter - 7);");
            Console.WriteLine("0. Exit (Enter - 0);");
            choice = Console.ReadLine();


            switch (choice)
            {
                case "1":
                    Console.WriteLine("Enter new name:");
                    Name = Console.ReadLine();
                    break;
                case "2":
                    Console.WriteLine("Enter surname name:");
                    Surname = Console.ReadLine();
                    break;
                case "3":
                    Console.WriteLine("Enter new login:");
                    Login = Console.ReadLine();
                    break;
                case "4":
                    Console.WriteLine("Enter new password:");
                    Password = Console.ReadLine();
                    break;
                case "5":
                    Console.WriteLine("Enter new amount of words to learn:");

                    try
                    {
                        AmountWordsOfDay = UInt32.Parse(Console.ReadLine());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Incorrect choice. Please, try again!!!");
                        UpdateUser();
                    }
                    break;
                case "6":
                    Console.WriteLine("Enter new date of birthday:");

                    try
                    {
                        DateOfBorned = DateTime.Parse(Console.ReadLine());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Incorrect choice. Please, try again!!!");
                        UpdateUser();
                    }
                    break;
                case "7":
                    LearnedWords.Clear();
                    break;
                case "0":
                    Console.WriteLine("Bye-bye!!!");
                    break;
                default:
                    Console.WriteLine("Incorrect choice. Please, try again!!!");
                    UpdateUser();
                    break;
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1} ({2})", Name, Surname, Login);
        }

    }


}

namespace Server
{

}