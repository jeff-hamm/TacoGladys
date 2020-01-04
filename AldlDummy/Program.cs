using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace AldlDummy
{
    class Program
    {
        static int Main(string[] args)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            while (true)
            {
                try
                {
                    Thread.Sleep(1000);
                    if (args[0] == "-d")
                    {
                        Stream($"{dir}/aldl-def.bin");
                        Console.Read();
                        return 0;
                    }
                    else if (args[0] == "-m")
                    {
                        Stream($"{dir}/aldl-msg.bin");
                        return -1;
                    }

                }
                catch (Exception ex)
                {
                    File.WriteAllText($"{dir}/{DateTime.Now.Ticks}.err", $"{ex}");
                    return -2;
                }
            }
        }


        static void Stream(string fileName)
        {
            var r = new Random();
            byte[] buffer = new byte[512];
            char[] cbuffer = new char[512];
            Console.Error.Write("datastreamer: Logging data to file: stdout\n");
            using var fs = System.IO.File.OpenRead(fileName);
            int read = -1;
            do
            {
                read = fs.Read(buffer, 0, 256);
                buffer.CopyTo(cbuffer,0);
                Console.Out.Write(cbuffer,0,read);
            } while (read > 0);

        }
    }
}
