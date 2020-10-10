using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using System.Linq;
using System.Security.Cryptography;

namespace Heap
{
    public class Heap<T> where T: new()

    {
        public int i = 0;
        nodoColaPrioridad<T> head = null;
        Dictionary<byte, string> prefijos = new Dictionary<byte, string>();
        Dictionary<byte, int> diccionario = new Dictionary<byte, int>();
       
        Dictionary<byte, List<string>> diccionarioBytes = new Dictionary<byte, List<string>>();
        Queue<nodoColaPrioridad<T>> pila = new Queue<nodoColaPrioridad<T>>();
        public List<int> listaCaracteres = new List<int>();
        List<string> listaBinaria = new List<string>();
        string final;
        int caracteres = 0;
        int bytes = 0;
         string codigoDescifrado = "";
        public string codigoNuevoDescifrado = "";
        string codigo = "";
        string metadata;
        int cant = 0;
        DateTime now = DateTime.Now;
        string path = @"\Files\";

        public void Compression(string ruta, string nombre)
        {
            path += nombre+".txt";
            string file = Environment.CurrentDirectory;
            string root = Directory.GetParent(file).Parent.Parent.Parent.FullName;
            path = root + path;
            

            using var fileWritten = new FileStream(ruta, FileMode.OpenOrCreate);
            using var reader = new BinaryReader(fileWritten);
            var buffer = new byte[5000];
            while (fileWritten.Position < fileWritten.Length)
            {
                buffer = reader.ReadBytes(5000);
                generarTabla(buffer);
                
            }
            ObtenerTablaGeneral();

            reader.Close();
            fileWritten.Close();
            //generarCodigo(prefijos, diccionario );


            caracteres = diccionario.Count;
            CreateMetaData();

            using var fileWrittenCode = new FileStream(ruta, FileMode.OpenOrCreate);
            using var readerCode = new BinaryReader(fileWrittenCode);
            var bufferCode = new byte[5000];
            

            while (fileWrittenCode.Position < fileWrittenCode.Length)
            {
                bufferCode = readerCode.ReadBytes(5000);
                generarCodigo(prefijos, bufferCode);
                Compress(codigo);
            }

            readerCode.Close();
            fileWrittenCode.Close();

           
            

            //WriteToFile();
        }
        public void Decompression(string ruta)
        {
            
            using var fileWritten = new FileStream(ruta, FileMode.OpenOrCreate);

      
            using var reader = new StreamReader(fileWritten);
            
            var buffer = new byte[5000];


            string contenidoMeta = reader.ReadToEnd();

            string frecuencias = contenidoMeta.Substring(2, (bytes * caracteres));
            

            char[]contenido = contenidoMeta.Substring((2 + (caracteres* bytes)), (contenidoMeta.Length - (frecuencias.Length) - 2 )).ToCharArray();



            //byte[] allText = Convert.FromBase64String(contenido);

            

            if (fileWritten.Position == 0)
                {
                    caracteres = Convert.ToInt32(Convert.ToString(reader.Read(), 2),2);
                    
                }
                if (fileWritten.Position == 1)
                {
                    bytes = Convert.ToInt32(Convert.ToString(reader.Read(), 2), 2);
                }

            //buffer = reader.ReadBytes(5000);


            
                
                generarTablaDescompresion(Encoding.UTF8.GetBytes(frecuencias.ToCharArray()));
            //ObtenerTablaGeneral();
            //byte[] codificado = Encoding.UTF8.GetBytes(allText.Substring(((bytes * caracteres) + 2), (int)(allText.Length - frecuencias.Length - 2)));

            //generarDescifrado(codificado);

            byte[] coleccion = contenido.SelectMany(BitConverter.GetBytes).ToArray();
            byte[] num = new byte[(coleccion.Length) / 2];


            int j = 0;
            for (int i = 0; i < num.Length; i++)
            {
                num[i] = coleccion[j];
                j += 2;
            }

            generarDescifrado(num);




            reader.Close();
                fileWritten.Close();

            DeCompress(codigoDescifrado);
            //ObtenerTablaGeneral();
            //generarCodigo(prefijos, diccionario);
            //caracteres = diccionario.Count;
            //reader.Close();
            //fileWritten.Close();
            //Compress(codigo);
            //WriteToFile();
        }
        public void generarTabla (byte[] bytes)
        {

            foreach (var caracter in bytes)
            {
                //if (!diccionario.ContainsKey(caracter))
                //{
                //    int cantidad = 0;
                //    for (int i = 0; i < bytes.Length; i++)
                //    {
                //        if (bytes[i].Equals(caracter))
                //        {
                //            cantidad++;
                //        }
                //    }
                //    diccionario.Add(caracter, cantidad);
                //    int numIdx = Array.IndexOf(bytes, caracter);
                //    List<byte> tmp = new List<byte>(bytes);
                //    tmp.RemoveAll(x => x.Equals(caracter));
                //    bytes = tmp.ToArray();
                //    if (bytes.Length == 0)
                //    {
                //        break;
                //    }

                //}
                //else {
                //    diccionario[caracter] = diccionario[caracter]+1;
                //}

                if (!diccionario.ContainsKey(caracter))
                {
                    diccionario.Add(caracter, 1);
                    int numIdx = Array.IndexOf(bytes, caracter);
                    List<byte> tmp = new List<byte>(bytes);
                    tmp.RemoveAt(numIdx);
                    bytes = tmp.ToArray();
                    
                }
                else
                {
                    int cantidad = 0;
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        if (bytes[i].Equals(caracter))
                        {
                            cantidad++;
                        }
                    }
                    diccionario[caracter] = diccionario[caracter] + cantidad;
                    int numIdx = Array.IndexOf(bytes, caracter);
                    List<byte> tmp = new List<byte>(bytes);
                    tmp.RemoveAll(x => x.Equals(caracter));
                    bytes = tmp.ToArray();
                    if (bytes.Length == 0)
                    {
                        break;
                    }

                }

                
                

            }

        }

        public void generarTablaDescompresion(byte[] arregloBytes) {

            diccionario.Clear();
            while (arregloBytes.Length > 0)
            {

                byte[] texto;
                byte[] nuevo;
                texto = arregloBytes.Take<byte>(bytes).ToArray<byte>();

                byte simbolo;
                byte primerFrecuencia;
                byte segundaFrecuencia;
                int sumaFrecuencia;

                if (texto.Length > 2)
                {
                    simbolo = texto[0];
                    primerFrecuencia = texto[1];
                    segundaFrecuencia = texto[2];
                    string frec1 = Convert.ToString(primerFrecuencia, 2);
                    string frec2 = Convert.ToString(segundaFrecuencia, 2);
                    string suma = frec1 +""+frec2;              
                    sumaFrecuencia = Convert.ToInt32(suma, 2);  
                }
                else {
                    simbolo = texto[0];
                   
                    segundaFrecuencia = texto[1];
                    sumaFrecuencia = segundaFrecuencia;
                }
                if (!diccionario.ContainsKey(simbolo))
                {
                    diccionario.Add(simbolo, sumaFrecuencia);
                    nuevo = arregloBytes.Skip<byte>((bytes)).ToArray<byte>();
                    arregloBytes = nuevo;
                }
               

            }
        
        }
        public void ObtenerTablaGeneral() {

            pila.Clear();
            prefijos.Clear();


            foreach (var item in diccionario)
            {
                Valor val = new Valor();
                nodoColaPrioridad<T> nuevo = new nodoColaPrioridad<T>();
                nuevo.valor = val;
                nuevo.valor.frecuencia = item.Value;
                nuevo.valor.letra = item.Key;
                pila.Enqueue(nuevo);
            }
            OrdenarCola(pila);
            crearArbol(pila);
            asignarPrefijos(head);

        }
        public void Compress(string text)
        {


            while (text.Length > 0) {
                text = SplitByBytes(text);
            }


            foreach (var item in listaBinaria)
            {
                int num = Convert.ToInt32(item, 2);
                listaCaracteres.Add(num);
            }
            foreach (var item in listaCaracteres)
            {
                final += Convert.ToChar((Convert.ToByte(item))).ToString();

            }

            byte[] arrayByte = new byte[final.Length];
            for (int i = 0; i < final.Length; i++)
            {
                arrayByte[i] = Convert.ToByte(final.ToArray()[i]);
            }


            //writer.(metadata + finalNuevo);
            //byte[] arrayMeta = Encoding.UTF8.GetBytes(metadata);
            //byte[] final454 = new byte[(arrayByte.Length + arrayMeta.Length)];

            //for (int i = 0; i < arrayMeta.Length; i++)
            //{
            //    final454[i] = arrayMeta[i];
            //}

            //int j = 0;
            //for (int i = arrayMeta.Length; j < arrayByte.Length; i++)
            //{
            //    final454[i] = arrayByte[j];
            //    j++;
            //}


            if (File.Exists(path))
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.Write(final.ToString());

                }
            }
            
            


        }

        public void CreateMetaData() {
            metadata = ((char)caracteres).ToString();
            int maxValue = diccionario.Values.Max();
            if (maxValue > 255)
            {
                bytes = 3;
            }
            else {
                bytes = 2;
            }
      

            
            metadata += ((char)bytes).ToString();
            foreach (var element in diccionario)
            {
                metadata += Convert.ToChar(element.Key);
                if (bytes > 2)
                {
                    string binario = Convert.ToString(element.Value);
                    byte frecuencia = (byte)element.Value;
                    //dividirBinario(binario);
                    ConvertToMoreThanOneByte(element);
                    foreach (var item in diccionarioBytes[element.Key])
                    {
                        metadata += Convert.ToChar(Convert.ToInt32(item, 2));
                    }
                }
                else
                {
                    metadata += ((char)element.Value).ToString();
                    
                }

            }

            using (StreamWriter sw = new StreamWriter(path)) {
                sw.Write(metadata);
            }



        }

        public void generarDescifrado(byte[] arregloBytes) {
            foreach (var bytes in arregloBytes) {
                codigoDescifrado += Convert.ToString(bytes, 2).PadLeft(8,'0');
            }
        }
        public void DeCompress(string text)
        {
            
            string cadena = "";
            while (text.Length > 0) {
                foreach (var item in text)
                {
                    int longitud = text.Length;
                    cadena += item.ToString();
                    if (prefijos.ContainsValue(cadena.ToString()))
                    {
                        codigoNuevoDescifrado += Convert.ToString((char)prefijos.FirstOrDefault(x => x.Value == cadena.ToString()).Key);

                       
                        text = text.Substring(cadena.Length, longitud-cadena.Length);
                        cadena = "";
                    }
                }
            }

        }
        public string SplitByBytes(string text) {
            string resto = "";
            string texto = "";
            string nuevo = "";
            if (text.Length > 0)
            {
                if (text.Length > 8)
                {
                    texto = text.Substring(0, 8);
                    nuevo = text.Remove(0, 8);
                    listaBinaria.Add(texto);
                }
                else
                {
                    resto = text.Substring(0, text.Length);
                    while (resto.Length < 8)
                    {
                        resto += "0";
                    }
                    listaBinaria.Add(resto);
                    nuevo = resto.Remove(0, 8);
                }

                return nuevo;
            }
            else {
                return "";
            }

           

        }
        
        public void WriteToFile() // falta
        {
            
          
       
            byte[] arrayByte = new byte[final.Length];
            for (int i = 0; i < final.Length; i++)
            {
                arrayByte[i] = Convert.ToByte(final.ToArray()[i]);
            }

            
            //writer.(metadata + finalNuevo);
            byte[] arrayMeta = Encoding.UTF8.GetBytes(metadata);
            byte[] final454 = new Byte[(arrayByte.Length + arrayMeta.Length)];
            for (int i = 0; i < arrayMeta.Length; i++)
            {
                final454[i] = arrayMeta[i];
            }

            int j = 0;
            for (int i = arrayMeta.Length; j < arrayByte.Length; i++)
            {
                final454[i] = arrayByte[j];
                j++;
            }

       

            File.WriteAllText(path, metadata+final.ToString(), Encoding.UTF8);

          
        }

        public void ConvertToMoreThanOneByte( KeyValuePair<byte,int> frecuencia) {
            String frecuenciaBinaria = Convert.ToString(frecuencia.Value, 2).PadLeft((bytes-1)*8, '0');
            List<string> bytesFrecuencia = new List<string>();
          
            while (frecuenciaBinaria.Length > 0)
            {
                frecuenciaBinaria = SplitByBytes2(frecuenciaBinaria, bytesFrecuencia);
                
            }

            diccionarioBytes.Add(frecuencia.Key, bytesFrecuencia); 
        }

        public string SplitByBytes2(string text, List<string> bytesFrecuencia)
        {
            string resto = "";
            string texto = "";
            string nuevo = "";
            if (text.Length > 0)
            {
                if (text.Length > 8)
                {
                    texto = text.Substring(0, 8);
                    nuevo = text.Remove(0, 8);
                    bytesFrecuencia.Add(texto);
                }
                else
                {
                    resto = text.Substring(0, text.Length);
                    while (resto.Length < 8)
                    {
                        resto += "0";
                    }
                    bytesFrecuencia.Add(resto);
                    nuevo = resto.Remove(0, 8);
                }

                return nuevo;
            }
            else
            {
                return "";
            }



        }

        public void dividirBinario(string text)
        {
            string nuevo ="";
            string resto = "";
            string left = "";
            while (text.Length > 0)
            {
                if (text.Length > 8)
                {
                    text = text.Substring(text.Length-8, 8);
                    nuevo = text.Remove(text.Length - 8, 8);
                    int num = Convert.ToInt32(text, 2);
                    metadata+= ((char)num).ToString();
                    dividirBinario(nuevo);
                }
                else
                {
                    resto = text.Substring(i, text.Length);
                    while (left.Length < 8-resto.Length)
                    {
                        left += "0";
                    }
                    resto = left + resto;
                    nuevo = resto.Remove(i, 8);
                    int num = Convert.ToInt32(resto, 2);
                    metadata += ((char)num).ToString();
                    dividirBinario(nuevo);
                }
            }
        }




        public void generarCodigo (Dictionary<byte, string> losPrefijos, byte[] arreglo)
        {
            foreach(var bytes in arreglo)
            {
                for (int i =0; i<losPrefijos.Count;i++)
                {
                    if (bytes == losPrefijos.ElementAt(i).Key)
                    {
                        codigo += losPrefijos.ElementAt(i).Value;
                    }
                }
            }
        }
        public void OrdenarCola(Queue<nodoColaPrioridad<T>> cola)
        {
            int o = 0;
            nodoColaPrioridad<T>[] vector = new nodoColaPrioridad<T>[cola.Count];
            for (int i = 0; cola.Count != 0; i++)
            {
                vector[i] = cola.Dequeue();
                o++;
            }
            for (int i = 0; i < vector.Length; i++)
            {
                for (int j = i + 1; j < vector.Length; j++)
                {
                    if (vector[j].valor.frecuencia < vector[i].valor.frecuencia)
                    {
                        nodoColaPrioridad<T> aux = new nodoColaPrioridad<T>();
                        aux = vector[i];
                        vector[i] = vector[j];
                        vector[j] = aux;
                    }
                }
            }
            for (int i = 0; i < o; i++)
            {
                cola.Enqueue(vector[i]);
            }
        }
        public nodoColaPrioridad<T> crearNodo (nodoColaPrioridad<T> n1, nodoColaPrioridad<T> n2)
        {
            nodoColaPrioridad<T> nuevo = new nodoColaPrioridad<T>();
            Valor val = new Valor();
            nuevo.valor = val;
            nuevo.valor.frecuencia = n1.valor.frecuencia + n2.valor.frecuencia;
            nuevo.valor.letra = 0;
            n1.padre = nuevo;
            n2.padre = nuevo;
            if (n1.valor.frecuencia>n2.valor.frecuencia)
            {
                nuevo.hijoDer = n2;
                nuevo.hijoIzq = n1;
            }
            else
            {
                nuevo.hijoDer = n1;
                nuevo.hijoIzq = n2;
            }
            return nuevo;
        }
        public void crearArbol (Queue<nodoColaPrioridad<T>> cola)
        {
            while (cola.Count>1)
            {
                nodoColaPrioridad<T> aux = new nodoColaPrioridad<T>();
                nodoColaPrioridad<T> nuevo = new nodoColaPrioridad<T>();
                aux = cola.Dequeue();
                nuevo= crearNodo(aux, cola.Dequeue());
                cola.Enqueue(nuevo);
                OrdenarCola(cola);
                crearArbol(cola);
            }
            if (cola.Count==1)
            {
                head = cola.Dequeue();
            }
        }
        public void asignarPrefijos(nodoColaPrioridad<T> raiz)
        {
                if (raiz == head)
                {
                    if (head.hijoIzq != null && head.hijoDer != null)
                    {
                        raiz.hijoIzq.prefijo = "0";
                        raiz.hijoDer.prefijo = "1";
                    if (raiz.hijoIzq.valor.letra != 0)
                         {
                        if (!prefijos.ContainsKey(raiz.hijoIzq.valor.letra))
                        {
                            prefijos.Add(raiz.hijoIzq.valor.letra, raiz.hijoIzq.prefijo);
                        }
                         }
                        if (raiz.hijoDer.valor.letra != 0)
                        {
                        if (!prefijos.ContainsKey(raiz.hijoDer.valor.letra))
                        {
                            prefijos.Add(raiz.hijoDer.valor.letra, raiz.hijoDer.prefijo);
                        }
                    }
                        asignarPrefijos(raiz.hijoDer);
                        asignarPrefijos(raiz.hijoIzq);
                    }
                    if (head.hijoIzq!=null && head.hijoDer==null)
                    {
                        raiz.hijoIzq.prefijo = "0";
                    if (raiz.hijoIzq.valor.letra != 0)
                    {
                        if (!prefijos.ContainsKey(raiz.hijoIzq.valor.letra))
                        {
                            prefijos.Add(raiz.hijoIzq.valor.letra, raiz.hijoIzq.prefijo);
                        }
                    }
                    asignarPrefijos(raiz.hijoIzq);
                    }
                    if (head.hijoIzq == null && head.hijoDer != null)
                    {
                        raiz.hijoDer.prefijo = "1";
                         if (raiz.hijoDer.valor.letra != 0)
                        {
                        if (!prefijos.ContainsKey(raiz.hijoDer.valor.letra))
                        {
                            prefijos.Add(raiz.hijoDer.valor.letra, raiz.hijoDer.prefijo);
                        }
                    }
                    asignarPrefijos(raiz.hijoDer);
                    }
                    
                }
                else
                {
                    if (raiz.hijoDer != null && raiz.hijoIzq != null)
                    {
                        raiz.hijoDer.prefijo = raiz.prefijo + "1";
                        raiz.hijoIzq.prefijo = raiz.prefijo + "0";
                    if (raiz.hijoIzq.valor.letra != 0)
                    {
                        if (!prefijos.ContainsKey(raiz.hijoIzq.valor.letra))
                        {
                            prefijos.Add(raiz.hijoIzq.valor.letra, raiz.hijoIzq.prefijo);
                        }
                    }
                    if (raiz.hijoDer.valor.letra != 0)
                    {
                        if (!prefijos.ContainsKey(raiz.hijoDer.valor.letra))
                        {
                            prefijos.Add(raiz.hijoDer.valor.letra, raiz.hijoDer.prefijo);
                        }
                    }
                    asignarPrefijos(raiz.hijoDer);
                        asignarPrefijos(raiz.hijoIzq);

                    }
                    if (raiz.hijoDer!=null && raiz.hijoIzq == null)
                    {
                        raiz.hijoDer.prefijo = raiz.prefijo + "1";
                    if (raiz.hijoDer.valor.letra != 0)
                    {
                        if (!prefijos.ContainsKey(raiz.hijoDer.valor.letra)) {
                            prefijos.Add(raiz.hijoDer.valor.letra, raiz.hijoDer.prefijo);
                        }
                    }
                    asignarPrefijos(raiz.hijoDer);
                    }
                    if (raiz.hijoDer == null && raiz.hijoIzq != null)
                    {
                        raiz.hijoIzq.prefijo = raiz.prefijo + "0";
                    if (raiz.hijoIzq.valor.letra != 0)
                    {
                        if (!prefijos.ContainsKey(raiz.hijoIzq.valor.letra))
                        {
                            prefijos.Add(raiz.hijoIzq.valor.letra, raiz.hijoIzq.prefijo);
                        }
                    }
                    asignarPrefijos(raiz.hijoIzq);
                    }
                 

                }
        }

        




    }

}
