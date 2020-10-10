using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heap;

namespace testHeap
{
    class Program
    {
        static void Main(string[] args)
        {
            Heap<objeto> heap = new Heap<objeto>();
            //heap.Compress("Holaaaaaaaaa");
            heap.Compression("C:\\Users\\Juan-Gtsk\\Desktop\\CompresionHuffman-Lab03\\CompresionHuffman\\Files\\Prueba.txt");
            Console.WriteLine("COMPRESS DONE!");
            heap.Decompression("c:\\Users\\Juan-Gtsk\\Desktop\\CompresionHuffman-lab03\\CompresionHuffman\\Files\\CompresionTEST.txt");
            Console.WriteLine(heap.codigoNuevoDescifrado);
        }
    }
}
