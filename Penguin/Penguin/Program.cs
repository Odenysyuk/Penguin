using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;
using Penguin;
using System.Xml;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Security.Cryptography;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using System.Collections;

namespace Penguin
{
    class Program
    {
        static public User UserProgram = null;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            ServerClien.InstallServerClien();
            User.initialization();
            ServerClien.UpdateDictionaryEU().Wait();
            User.InputSystem();

            if (UserProgram != null)
            {
                PrintMenu(UserProgram);
            }
            else
            {
                Console.WriteLine("Sorry, we have some problem with Penguin. Try again late.");
            }

        }

        static public void PrintMenu(User UserProgram)
        {
            Console.Clear();
            string choice = "-1";

            Console.WriteLine("\t Hello {0}, You can do:", UserProgram.Login);
            Console.WriteLine("1. Learn new words (Enter -1);");
            Console.WriteLine("2. Repeat learned words Eng -> Ukr (Enter -2);");
            Console.WriteLine("3. Repeat learned words Ukr -> Eng (Enter -3);");
            Console.WriteLine("4. Add new word my dictionary (Enter -4);");
            Console.WriteLine("5. Update base(Enter - 5);");
            Console.WriteLine("6. Setting user(Enter - 6);");
            Console.WriteLine("7. Show statistic(Enter - 7);");
            Console.WriteLine("0. Exit (Enter - 0);");
            choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    UserProgram.LearnNewWords();
                    break;
                case "2":
                    UserProgram.RepeatWordsEngUkr();
                    break;
                case "3":
                    UserProgram.RepeatWordsUkrEng();
                    break;
                case "4":
                    UserProgram.AddNewWords();
                    break;
                case "5":
                    ServerClien.InstallServerClien();
                    break;
                case "6":
                    UserProgram.UpdateUser();
                    break;
                case "7":
                    UserProgram.ShowStatistic();
                    break;
                case "0":
                    UserProgram.AddStatistic();
                    Console.WriteLine("Bye-bye!!!");
                    return;
                default:
                    break;
            }

            

            PrintMenu(UserProgram);

        }

        static public void PressAnyKey(String Message, Int32 times = 10)
        {            
            Console.WriteLine(Message);
            Console.ReadKey();
            Console.Clear();
        }
    }

    class User
    {
        public delegate Task CreateUpdateUsers(User Obj);
        public static event CreateUpdateUsers EventCreateUpdateUsers;

        public static void initialization()
        {
            EventCreateUpdateUsers = new CreateUpdateUsers(ServerClien.InsertUser);
        }

        public static void DoEvent(User Temp)
        {
            if (EventCreateUpdateUsers != null)
                EventCreateUpdateUsers(Temp);

        } 

        [BsonElement("_id")]
        public string Login { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("surname")]
        public string Surname { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        public uint AmountWordsOfDay { get; set; }

        public DateTime DateOfBorned { get; set; }

        public List<DictionaryEngUkr> LearnedWords = new List<DictionaryEngUkr>();

        public Dictionary<DateTime, Int32> Statistic = new Dictionary<DateTime, int>();

        public void ClearLearnedWords()
        {
            if (LearnedWords != null)
            {
                LearnedWords.Clear();
                DoEvent(this);
            }    
        }

        public void UpdateUser()
        {
            Console.Clear();  

            string choice = "-1";

            Console.WriteLine("\t Menu of setting user:");
            Console.WriteLine("1. Change name (Enter -1);");
            Console.WriteLine("2. Change surname (Enter -2);");
            Console.WriteLine("3. Change password (Enter -3);");
            Console.WriteLine("4. Change amount of word to learn (Enter -4);");
            Console.WriteLine("5. Change date of birthday (Enter - 5);");
            Console.WriteLine("6. Clean learning words(Enter - 6);");
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
                    Password = EnterPassword();
                    break;
                case "4":
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
                case "5":
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
                case "6":
                    ClearLearnedWords();
                    break;
                case "0":
                    Program.PrintMenu(this);
                    break;
                default:
                    Console.WriteLine("Incorrect choice. Please, try again!!!");
                    UpdateUser();
                    break;
            }          

            if (EventCreateUpdateUsers != null)
                EventCreateUpdateUsers(this);

            Program.PressAnyKey("Change are saved.Press any key...");
            UpdateUser();
        }

        public void LearnNewWords()
        {

            var ListWord = ServerClien.AddListWords(LearnedWords, AmountWordsOfDay);

            if (LearnedWords == null)
                LearnedWords = new List<DictionaryEngUkr>();

            foreach (var el in ListWord)
            {
                var i = 5;
                Console.WriteLine("{0}", el);
                Console.WriteLine("Write five sentences with new word:");
                while (i-- != 0)
                {
                    var str = Console.ReadLine();
                    if(str.Length < 6)
                    {
                        Console.WriteLine("Sentence is too shot. Please, write more large.");
                        i++;
                    }
                }
                LearnedWords.Add(el);
            }

            DoEvent(this);
        }

        public void RepeatWordsEngUkr()
        {
            Console.Clear();

            var ind = AmountWordsOfDay;
            Int32 ch;
            while (ind-- != 0)
            {
                var el = LearnedWords[new Random().Next(1, LearnedWords.Count - 1)];
                var list = ServerClien.AddListWords(LearnedWords, 3);
                var Ran = new Random().Next(1,4);

                Console.WriteLine("\n{0} find translation:", el.WordEng);
                for (int i = 1, j =0; i <= 4; i++)
                {
                    if(i == Ran)
                    {
                        Console.WriteLine("{0}, {1}", i,  el.WorkUkrToString());
                    }
                    else
                    {
                        Console.WriteLine("{0}, {1}", i, list[j++].WorkUkrToString());
                    }
                }

                EnterChoise: Console.Write("Your choise: ");

                try{
                    ch = Int32.Parse(Console.ReadLine());
                }
                catch(Exception e)
                {
                    Console.WriteLine("Incorrect choise, try again.");
                    goto EnterChoise;
                }
                
                if(ch == Ran)
                {
                    Console.WriteLine("You're right!!!");
                }
                else
                {
                    Console.WriteLine("Right answer is {0}", el.WorkUkrToString());
                }
                Console.ReadLine();
            }
        }

        public void RepeatWordsUkrEng()
        {
            Console.Clear();

            var ind = AmountWordsOfDay;
            Int32 ch;
            while (ind-- != 0)
            {
                var el = LearnedWords[new Random().Next(1, LearnedWords.Count - 1)];
                var list = ServerClien.AddListWords(LearnedWords, 3);
                var Ran = new Random().Next(1, 4);

                Console.WriteLine("\n{0} find translation:", el.WorkUkrToString());
                for (int i = 1, j = 0; i <= 4; i++)
                {
                    if (i == Ran)
                    {
                        Console.WriteLine("{0}, {1}", i, el.WordEng);
                    }
                    else
                    {
                        Console.WriteLine("{0}, {1}", i, list[j++].WordEng);
                    }
                }

                EnterChoise: Console.Write("Your choise: ");

                try
                {
                    ch = Int32.Parse(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Incorrect choise, try again.");
                    goto EnterChoise;
                }

                if (ch == Ran)
                {
                    Console.WriteLine("You're right!!!");
                }
                else
                {
                    Console.WriteLine("Right answer is {0}", el.WordEng);
                }
                Console.ReadLine();
            }
        }

        public void AddNewWords()
        {
            String str;
            Console.WriteLine("Enter English word:");
            str = Console.ReadLine();
            var temp = ServerClien.FindWordByEng(str);
            if(temp == null)
            {
                Program.PressAnyKey("Not found word. Try update dictionary!!!");
            }

            if (LearnedWords == null)
                LearnedWords = new List<DictionaryEngUkr>();
            LearnedWords.Add(temp);
            DoEvent(this);
        }

        public void ShowStatistic()
        {
            Console.Clear();

            var keys = Statistic.Keys;
            foreach (var el in keys)
            {
                Console.WriteLine("{0} : {1}", el, Statistic[el]);
            }
            Program.PressAnyKey("Press any key...");
        }

        public void AddStatistic()
        {
            DateTime thisDay = DateTime.Today.Date;
            if (Statistic == null)
                Statistic = new Dictionary<DateTime, int>();

            
            if (Statistic.ContainsKey(thisDay) == false)
                Statistic.Add(thisDay, LearnedWords.Count());
            else
                Statistic[thisDay] = LearnedWords.Count();

            DoEvent(this);


        }

        public override string ToString()
        {
            return String.Format("{0} {1} ({2})", Name, Surname, Login);
        }

        static public void InputSystem()
        {

            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.CursorSize = 25;
            Console.Clear();

            Console.Title = "Penguin";

            Console.WriteLine("\n\n\n\n\t\t\t\t\t\t Welcome in Penguin!!! ");

            Console.CursorTop = Console.WindowHeight - 2;
            Program.PressAnyKey("\t\t\t\t\t\tPress any key to continue ...", 10);

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

            EnterLogin: Console.WriteLine("Enter your login:");
            login = Console.ReadLine();

            var User = ServerClien.FindUserByLogin(login);
            if (User == null)
            {
                var ListCopy = ServerClien.FindUserByLoginSame(login);
                if(ListCopy.Count == 0)
                {
                    Program.PressAnyKey("\nWrong login. Try again!!!\nPress any key......", 3);
                    goto EnterLogin;
                }

                Console.WriteLine("\nWrong login combination. But maybe you'd say: ");
                foreach (var el in ListCopy)
                {
                    Console.Write("{0} ", el.Login);
                }
                Console.WriteLine();
                goto EnterLogin;
            }

            EnterPassword: password = EnterPassword();
            if (User.Password != password)
            {
                Console.WriteLine("\n Invalid login / password combination. Try again!!!");
                goto EnterPassword;
            }

            Console.Clear();
            Program.UserProgram = User;
            Console.WriteLine("\t \t \t Hello, {0}", User);
        }
        
        static public void MenuSignUp()
        {
            string @login = "";

            Console.WriteLine("Enter login:");
            @login = Console.ReadLine();

            var @user = ServerClien.FindUserByLogin(@login);

            if (@user != null)
            {
                Program.PressAnyKey(String.Format("User with name {0} is already exists Try again!!!\nPress any key......", @login));
                MenuSignUp();
            }

            User temp = new User();
            temp.Login = login;

            temp.Password = EnterPassword();

            Console.WriteLine("Enter name:");
            temp.Name = Console.ReadLine();

            Console.WriteLine("Enter surname:");
            temp.Surname = Console.ReadLine();

            uint @AmountWordsOfDay = 0;
            EnterAmount: Console.WriteLine("Enter amount of word to learn:");
            try
            {
                @AmountWordsOfDay = UInt32.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine("Incorrect param. {0}", e.Message);
                goto EnterAmount;
            }

            temp.AmountWordsOfDay = @AmountWordsOfDay;


            DateTime DB;
            EnterData: Console.WriteLine("Enter date of birthday:");
            try
            {
                DB = DateTime.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine("Incorrect param. {0}", e.Message);
                goto EnterData;
            }

            temp.DateOfBorned = DB;

            if (EventCreateUpdateUsers != null)
                EventCreateUpdateUsers(temp);

            Program.UserProgram = temp;

            Console.WriteLine("Congratulations on your registration!");
            Program.PressAnyKey("Press any key......");

        }

        static public String EnterPassword()
        {

            Console.WriteLine("Enter your password:");

            string str = string.Empty;
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (str.Length != 0)
                    {
                        str = str.Remove(str.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    str += key.KeyChar;
                    Console.Write("*");
                }
            }
            while (true);

            return MD5Helper.ToMD5(str);
        }
    }

    class DictionaryEngUkr
    {
        public ObjectId Id { get; set; }

        [BsonElement("english")]
        public string WordEng { get; set; }

        [BsonElement("ukaine")]
        public List<string> WordUkr {
            get;
            set;
       }

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

        public override string ToString()
        {
            var str = "";
            foreach (var el in WordUkr) { str += el + "; "; }
            return String.Format("{0} - {1}", WordEng, str);
        }

        public string WorkUkrToString()
        {
            var str = "";
            foreach (var el in WordUkr) { str += el + "; "; }
            return str;
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
        static public IMongoCollection<User> col_user { get; set; }

        static public void InstallServerClien()
        {
            ConventionMain.RegisterNewConvetions();
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

        static public Object FindUser(string log, string passw)
        {

            var col = col_user;
            var filter = new BsonDocument();
            var result = col.Find(filter);
            return result;
        }

        static public User FindUserByLogin(String log_)
        {

            var bs = col_user.Find(new BsonDocument("_id", log_)).ToList();
            if (bs.Count == 1)
                return bs.First();

            return null;
        }

        static public List<User> FindUserByLoginSame(String log_)
        {

            List<String> temp = new List<String>();
            temp.Add(log_);
            SplitString(temp, log_, 2);

            var builder =  Builders<User>.Filter;
            var List = new List<FilterDefinition<User>>();

            foreach (var el in temp)
            {
                List.Add(builder.Regex("_id", new BsonRegularExpression(el, "si")));
            }
            var filter = builder.Or(List);

            return col_user.Find(filter).ToList();

        }

        static void SplitString(List<String> temp, String str, Int32 Separator = 2)
        {

            var res = str.Length / Separator;

            if (res > 1)
            {
                temp.Add(str.Substring(0, res));
                temp.Add(str.Substring(res));
                SplitString(temp, str.Substring(0, res), Separator);
                SplitString(temp, str.Substring(res), Separator);
            }

        }

        static public async Task InsertUser(User user_)
        {
            var col = col_user;

            await col.ReplaceOneAsync(Builders<User>.Filter.Eq("_id", user_.Login), user_, new UpdateOptions{IsUpsert = true });
        }

        static public async Task UpdatetUser(User user_)
        {
            var col = col_user;

            await col.InsertOneAsync(user_);
        }

        static public List<DictionaryEngUkr>  AddListWords (List<DictionaryEngUkr>  LearnedWords, UInt32 AmountWordsOfDay)
        {
            var builder = Builders<DictionaryEngUkr>.Filter;
            int number = new Random().Next(1, 50000);

            if (LearnedWords == null)
            {
                var filter_ = new BsonDocument();
                return col_dictionary.Find(filter_).Skip(number).Limit((int)AmountWordsOfDay).ToList(); //.Limit((int)AmountWordsOfDay).ToList();
            }


            var List = new List<ObjectId>();

            foreach (var el in LearnedWords)
            {
                List.Add(el.Id);
            }
           var filter = builder.Nin("_id", List);

            return col_dictionary.Find(filter).Skip(number).Limit((int)AmountWordsOfDay).ToList();
        }

        static public DictionaryEngUkr FindWordByEng(String word)
        {
            var bs = col_dictionary.Find(new BsonDocument("_id", word)).ToList();
            if (bs.Count == 1)
                return bs.First();
            return null;
        }

    }


    public class ConventionMain
    {
        public static void RegisterNewConvetions()
        {
            ConventionRegistry.Register(
            "DictionaryRepresentationConvention",
            new ConventionPack { new DictionaryRepresentationConvention(DictionaryRepresentation.ArrayOfArrays) },
            _ => true);
        }
    }
    class DictionaryRepresentationConvention : ConventionBase, IMemberMapConvention
    {
        private readonly DictionaryRepresentation _dictionaryRepresentation;
        public DictionaryRepresentationConvention(DictionaryRepresentation dictionaryRepresentation)
        {
            _dictionaryRepresentation = dictionaryRepresentation;
        }
        public void Apply(BsonMemberMap memberMap)
        {
            memberMap.SetSerializer(ConfigureSerializer(memberMap.GetSerializer()));
        }
        private IBsonSerializer ConfigureSerializer(IBsonSerializer serializer)
        {
            var dictionaryRepresentationConfigurable = serializer as IDictionaryRepresentationConfigurable;
            if (dictionaryRepresentationConfigurable != null)
            {
                serializer = dictionaryRepresentationConfigurable.WithDictionaryRepresentation(_dictionaryRepresentation);
            }

            var childSerializerConfigurable = serializer as IChildSerializerConfigurable;
            return childSerializerConfigurable == null
                ? serializer
                : childSerializerConfigurable.WithChildSerializer(ConfigureSerializer(childSerializerConfigurable.ChildSerializer));
        }
    }


    internal class MD5Helper
    {
        public static string ToMD5(string str)
        {
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            return BitConverter.ToString(mD5CryptoServiceProvider.ComputeHash(Encoding.Default.GetBytes(str))).Replace("-", "").ToLower();
        }
    }
}