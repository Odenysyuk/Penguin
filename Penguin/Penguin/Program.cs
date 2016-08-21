using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client;
using Server;
using System.Xml;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace Penguin
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerClien.InstallServerClien();
            ServerClien.UpdateDictionaryEU().Wait();
            User.InputSystem();





         //   Console.InputEncoding = Encoding.UTF8;
         //   User Client = new User();
         //   Client.Name = "Olena";
         //  Console.WriteLine(Client.Name);

            //  ParseDictionary();
        }

       
    }

}

namespace Client
{
    class User
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }  

        public string Surname { get; set; }
        
        public string Login { get; set; }

        public string Password{ get; set; }

        public uint AmountWordsOfDay { get; set; }

        public DateTime DateOfBorned { get; set; }

        public List<DictionaryEngUkr> LearnedWords;

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

        static public void InputSystem()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
         //   Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.Clear();

            Console.Title = "Penguin";

            Console.WriteLine("\n\n\n\n\t\t\t\t\t\t Welcome in Penguin!!! ");

            Console.CursorTop = Console.WindowHeight - 2;
            Console.WriteLine("\t\t\t\t\t\tPress any key to continue ...");

            Console.ReadKey();
            Console.Clear();

            User.MenuSign();      
        }

        static public void MenuSign()
        {
            string choice;

            Console.WriteLine("\t Menu:");
            Console.WriteLine("1. Sign In (Enter -1);");
            Console.WriteLine("2. No account yet? Sign up. (Enter -2);");

            choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    MenuSignIn();
                    break;
                case "2":
                    MenuSignUp();
                    break;
                default:
                    Console.WriteLine("Invalid combination. Try again!!!\n Press any key...");
                    Console.ReadLine();
                    MenuSign();
                    break;
            }
            Console.Clear();   
        }

        static public void MenuSignIn()
        {
            string login, password;

            Console.WriteLine("Enter your login:");
            login = Console.ReadLine();

            Console.WriteLine("Enter your password:");
            password = Console.ReadLine();

            var User = ServerClien.FindUser(login, password);

            if(User != null)
            {
                Console.WriteLine("Hello, {0}", User);
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Invalid login / password and password combination. Try again!!!\nPress any key......");
                Console.ReadKey();
                Console.Clear();
                MenuSignIn();
            }



        }

        static public void MenuSignUp()
        {

            Console.WriteLine("Enter name;");
            Console.WriteLine("Enter surname");
            Console.WriteLine("Enter login;");
            Console.WriteLine("Enter password;");
            Console.WriteLine("Enter amount of word to learn;");
            Console.WriteLine("Enter date of birthday;");
            Console.WriteLine("Enter clean learning words;");

            Console.WriteLine("Congratulations on your registration!");
            Console.Clear();
        }

       
    }

    class DictionaryEngUkr
    {
        public ObjectId Id { get; set; }

        [BsonElement("english")]
        public string WordEng { get; set; }

        [BsonElement("ukaine")]
        public List<string> WordUkr { get; set; }

        static public List<DictionaryEngUkr> ParseDictionary()
        {
            List<DictionaryEngUkr> listEU = new List<DictionaryEngUkr>();

            XmlTextReader reader = new XmlTextReader("dictionary.xdxf");
            reader.WhitespaceHandling = WhitespaceHandling.None;
            while (reader.Read())
            {
                if (reader.Name == "ar" && reader.NodeType == XmlNodeType.Element)
                {
                    string Eng = "";

                    Boolean translation = false;
                    while (reader.Read() && reader.Name != "ar")
                    {

                        if (reader.NodeType == XmlNodeType.Text)
                        {
                            string temp = reader.Value;
                            if (translation == false)
                            {
                                Eng = temp;
                                translation = true;
                            }
                            else
                            {
                                temp = temp.Replace("\n", "");                                                 
                                var Ukr = temp.Split(',').ToList();
                                listEU.Add(new DictionaryEngUkr { WordEng = Eng, WordUkr = Ukr });
                            }


                        }

                    }

                }

            }

            return listEU;

        }

       

    }

}



namespace Server
{
   
    static class ServerClien
    {
        static public MongoClient Client { get; set; }
        static public IMongoDatabase database { get; set; }
        static public IMongoCollection<DictionaryEngUkr> col_dictionary { get; set; }
        static public IMongoCollection<User> col_user{ get; set; }

        static public void InstallServerClien(){
            Client = new MongoClient("mongodb://localhost:27017");
            database = Client.GetDatabase("Penguit");
            col_dictionary = database.GetCollection<DictionaryEngUkr>("dictionary");
            col_user = database.GetCollection<User>("users");
        }

        static public async Task UpdateDictionaryEU()
        {
            var col = col_dictionary;
            var result = await col.DeleteManyAsync(new BsonDocument());
            var parse = DictionaryEngUkr.ParseDictionary();
            await col.InsertManyAsync(parse);
        }

        static public async Task FindUser(string log, string passw)
        {

            var col = col_dictionary;
            
            // return new User();

        }
    }
}

//var client = new MongoClient("mongodb://localhost:27017");
//var db = client.GetDatabase("Penguit");
//var col = db.GetCollection<DictionaryEngUkr>("dictionary");

//var filter = new BsonDocument();
//var result = await col.DeleteManyAsync(filter);

//var filter = new BsonDocument();
//var result = await col.DeleteManyAsync(filter);

//using (var writer = new JsonWriter(Console.Out))
//{
//    BsonSerializer.Serialize(writer, Dict);
//}
