using System;
using System.Collections.Generic;
using System.Text;

namespace Heap
{
    public class Valor
    {
        public int frecuencia { get; set; }
        public byte letra { get; set; }
        public Valor()
        {

        }
        public Valor(int f, byte l)
        {
            this.frecuencia = f;
            this.letra = l;
        }
    }
}
