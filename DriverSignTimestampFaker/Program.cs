using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSignTimestampFaker
{
    class Program
    {
        static int findDateOffset(byte[] data)
        {
            byte[] sig = new byte[] { 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x09, 0x05, 0x31 };
            int currentAddress = 0;

            for (int i = 0; i < data.Length - sig.Length; i++)      //Signature scan for string that comes a few bytes before date timestamp.
            {
                currentAddress++;
                bool lastMatched = true;
                for(int j = 0; j < sig.Length; j++)
                {
                    if (data[i + j] != sig[j] || !lastMatched)
                    {
                        lastMatched = false;
                        continue;
                    }

                    if (j == sig.Length - 1)
                        return i;
                }
            }

            return -1;
        }

        static void Main(string[] args)
        {
            Console.Title = "Driver Signing Timestamp Faker";

            if (args.Length < 1)
            {
                Console.WriteLine("No input file specified.");
                System.Threading.Thread.Sleep(2000);
                Environment.Exit(666);
            }

            string filename = Path.GetFileName(args[0]);
            byte[] file = File.ReadAllBytes(args[0]);

            int dateoffset = findDateOffset(file) + 13;
            
            Console.WriteLine("Desired Date (YYDDMM): ");
            string desiredDate = Console.ReadLine();

            if (desiredDate.Length < 6)                                 	//Idiot checks
            {
                Console.WriteLine("Invalid date: " + desiredDate);
            }

            if(desiredDate.Length == 8)
                desiredDate = desiredDate.Remove(0, 2);                 	//Replace YYYY with YY if user didnt follow *YY* instruction

            int desiredDateIndex = 0;
            for(int i = dateoffset; i < dateoffset + desiredDate.Length; i++)	//Cry about it.
            {
                file[i] = (byte)desiredDate[desiredDateIndex];
                desiredDateIndex++;
            }

            File.WriteAllBytes(desiredDate + "_" + filename, file);
            Console.Clear();
            Console.WriteLine("File written: " + desiredDate + "_" + filename);
            Console.ReadKey();
        }
    }
}
