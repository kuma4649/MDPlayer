using System;
using System.IO;

namespace mdc
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                DispUsage();
                return;
            }

            KumaCom mmf = new FileCom(true, "MDPlayer", "MDPlayer", 1024 * 4);
            try
            {
                mmf.SendMessage(string.Join(" ", args));
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("メッセージが長すぎ");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("共有メモリがみつからない");
            }
        }

        private static void DispUsage()
        {
            Console.WriteLine("MDPlayer control");
            Console.WriteLine("Usage : mdc.exe command [option]");
            Console.WriteLine("  command : ");
            Console.WriteLine("    PLAY [filename]");
            Console.WriteLine("    STOP");
            Console.WriteLine("    NEXT");
            Console.WriteLine("    PREV");
            Console.WriteLine("    FADEOUT");
            Console.WriteLine("    FAST");
            Console.WriteLine("    SLOW");
            Console.WriteLine("    PAUSE");
            Console.WriteLine("    CLOSE");
            Console.WriteLine("    LOOP");
            Console.WriteLine("    MIXER");
            Console.WriteLine("    INFO");
        }
    }
}
