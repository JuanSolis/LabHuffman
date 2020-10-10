using System;
using System.Collections.Generic;
using System.Linq;
using Heap;
using System.Threading.Tasks;
using API.Models;

namespace API.Data
{
    public class Singleton
    {
        private readonly static Singleton _instance = new Singleton();
        public List<Compression> comp = new List<Compression>();
        private Singleton()
        {
            Compression compPrueva = new Compression();
            compPrueva.originalName =  "";
            compPrueva.compressedFilePath = "";
            compPrueva.compressionFactor =0;
            compPrueva.compressionRatio = 0;
            compPrueva.reductionPercentage = 0;
            comp.Add(compPrueva);


        }
        public static Singleton Instance
        {
            get
            {
                return _instance;
            }
        }

        public Heap<Compression> heap = new Heap<Compression>();
    }
}
