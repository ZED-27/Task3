using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace RockPaperScissors
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] obj = new string[args.Length];
            Array.Copy(args, obj, args.Length);
            
            if(ErrorProcessing(obj))
            {
                Environment.Exit(0);
            }

            var quantityObjGame = obj.Length;

            var moves = new List<string>();
            foreach (var i in obj)
            {
                moves.Add(i);
            }
            moves.Add("exit");
            moves.Add("help");


            var isExitDown = false;
            int numberUserMove = 0;
            int numberComputerMove = 0;

            do
            {
                var hmac = new HMACGenerator();
                hmac.DataGen(obj.Length);

                Console.WriteLine($"HMAC:{hmac.GetHash()}");

                ShowMenu(moves);
                var userChoice = Console.ReadLine();
                if(userChoice == string.Empty)
                {
                    userChoice = "-1";
                }
                if (userChoice == "0")
                {
                    Console.WriteLine($"Your move: {moves[moves.Count - 2]}");
                    isExitDown = true;
                }
                else if (userChoice == "?")
                {
                    Console.WriteLine($"Your move: {moves[moves.Count - 1]}");
                    Table.CreateTable(obj);
                    isExitDown = false;
                }
                else if (0 < int.Parse(userChoice) && int.Parse(userChoice) <= quantityObjGame)
                {
                    numberUserMove = int.Parse(userChoice) - 1;
                    Console.WriteLine($"Your move: {moves[numberUserMove]}");
                    isExitDown = false;

                    numberComputerMove = int.Parse(hmac.GetData());
                    Console.WriteLine($"Computer move: {moves[numberComputerMove]}");

                    var status = Rules.IsUserWin(numberUserMove, numberComputerMove, quantityObjGame - 1);
                    if (status == "draw")
                    {
                        Console.WriteLine(status);
                    }
                    else
                    {
                        Console.WriteLine($"You {status}");
                    }
                }
                else
                {
                    isExitDown = false;
                }
            }
            while (!isExitDown);
        }

        static void ShowMenu(List<string> moves)
        {
            Console.WriteLine("Available moves: ");
            for (var i = 0; i < moves.Count - 2; i++)
            {
                Console.WriteLine($"{i + 1} - {moves[i]}");
            }
            Console.WriteLine($"0 - {moves[moves.Count - 2]}");
            Console.WriteLine($"? - {moves[moves.Count - 1]}");

            Console.Write("Enter your move: ");
        }

        static bool ErrorProcessing(string[] obj)
        {
            var isErrorFind = ErrorProcessingHonestNumber(obj) | ErrorProcessingIdenticalElements(obj) | ErrorProcessingOneElement(obj);            

            if (isErrorFind)
            {
                ExampleOutput();
            }

            return isErrorFind;
        }

        static bool ErrorProcessingHonestNumber(string[] obj)
        {
            if (obj.Length % 2 == 0)
            {
                Console.WriteLine("ERROR: honest number of parameters entered");
                return true;
            }
            return false;
        }

        static bool ErrorProcessingOneElement (string[] obj)
        {
            if (obj.Length < 3)
            {
                Console.WriteLine("ERROR: number of parameters < 3");
                return true;
            }
            return false;
        }

        static bool ErrorProcessingIdenticalElements(string[] obj)
        {
            if (findDublicate(obj))
            {
                Console.WriteLine("ERROR: identical elements found");
                return true;
            }
            return false;
        }

        static bool findDublicate(string[] obj)
        {
            var set = new HashSet<string>();
            foreach (var item in obj)
            {
                if (!set.Add(item))
                {
                    return true;
                }
            }
            return false;
        }

        static void ExampleOutput()
        {
            Console.WriteLine("Example: rock paper scissors lizard spock");
        }
    }

    class HMACGenerator
    {
        private string key;
        private string data;
        private string hash;

        public string GetKey()
        {
            return key;
        }

        public string GetHash()
        {
            return hash;
        }

        public string GetData()
        {
            return data;
        }

        public void KeyGen()
        {
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();

            byte[] bytes = new byte[64];
            rng.GetBytes(bytes);

            key = Convert.ToBase64String(bytes).ToUpper();
        }

        public void DataGen(int maxValue)
        {
            data = RandomNumberGenerator.GetInt32(maxValue).ToString();
            KeyGen();
            HMACHASH(data, key);
        }

        public void HMACHASH(string data, string key)
        {
            byte[] bkey = Encoding.Default.GetBytes(key);
            using (var hmac = new HMACSHA256(bkey))
            {
                byte[] bstr = Encoding.Default.GetBytes(data);
                var bhash = hmac.ComputeHash(bstr);
                hash = BitConverter.ToString(bhash).Replace("-", string.Empty).ToUpper();
            }
        }
    }

    class Rules
    {
        public static string IsUserWin(int numberUserMove, int numberComputerMove, int quantityMoves)
        {
            var quantityDefeated = quantityMoves / 2;

            if (numberComputerMove == numberUserMove)
            {
                return "draw";
            }
            else
            {
                var number = numberUserMove == 0 ? quantityMoves : numberUserMove - 1;
                for (var i = 0; i < quantityDefeated; i++)
                {
                    if (number == numberComputerMove)
                    {
                        return "win";
                    }

                    if (--number <= 0)
                    {
                        number = quantityMoves;
                    }

                }

                return "lose";
            }
        }
    }

    class Table
    {
        public static void CreateTable(string[] moves)
        {
            int maxLegth = 0;
            foreach (var i in moves)
            {
                maxLegth = Math.Max(i.Length, maxLegth);
            }

            var writeLength = maxLegth + 4;

            for (var i = 0; i < moves.Length + 1; i++)
            {
                for (var j = 0; j < moves.Length + 1; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        Console.Write(string.Empty.PadLeft(writeLength));
                    }
                    else if (i == 0 && j != 0)
                    {
                        Console.Write(moves[j - 1].PadLeft(writeLength));
                    }
                    else if (i != 0 && j == 0)
                    {
                        Console.Write(moves[i - 1].PadLeft(writeLength));
                    }
                    else
                    {
                        Console.Write(Rules.IsUserWin(j - 1, i - 1, moves.Length - 1).PadLeft(writeLength));
                    }


                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
