using System;
using System.Collections.Generic;
using System.Text;

namespace Heap
{
    public class nodoColaPrioridad<T>
    {
        public  nodoColaPrioridad()
        {

        }       
        public int posicion { get; set; }
        public nodoColaPrioridad<T> hijoIzq { get; set; }
        public nodoColaPrioridad<T> hijoDer { get; set; }
        public nodoColaPrioridad<T> padre { get; set; }
        public Valor valor { get; set; }
        public string prefijo { get; set; }
     
    }
}
