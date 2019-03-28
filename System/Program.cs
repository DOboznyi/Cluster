using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    class Program
    {
        static void Main(string[] args)
        {
            EDF e = new EDF(10, new List<double>() {1, 5, 10 }, new List<int>() { 1,2,3}, 1000, 10, 10, 0.5, 0.5, new List<double>() { 0.8, 0.15, 0.05 }, 3, 2, 1);
            e.run();
        }
    }
}
