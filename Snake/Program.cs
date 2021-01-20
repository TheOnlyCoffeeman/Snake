using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine eng = new Engine();
            eng.Run();
        }
    }
}
