using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Heap;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class compressController : ControllerBase
    {

        List<Compression> compresiones = new List<Compression>();
        [HttpGet]
        public IEnumerable<Compression> Get()
        {

            return Singleton.Instance.comp;
        }

        [HttpPost("{nombre}")]
        public ActionResult compress([FromForm] IFormFile name, string nombre)
        {
            try {
               
                string path = @"\Files\";
                path += nombre + ".txt";
                string file = Environment.CurrentDirectory;
                string root = Directory.GetParent(file).Parent.Parent.Parent.FullName;
                path = root + path;
                Compression compression = new Compression();
                Heap<Compression> heap = new Heap<Compression>();
                string ruta = Path.GetFullPath(name.FileName);
                compression.originalName = name.FileName;                
                heap.Compression(Path.GetFullPath(name.FileName),nombre);
                compression.compressedFilePath = path;
                FileInfo fileSin = new FileInfo(Path.GetFullPath(name.FileName));
                FileInfo fileCon= new FileInfo(path);
                compression.compressionFactor = fileSin.Length/fileCon.Length;
                compression.compressionRatio = fileCon.Length / fileSin.Length;
                compression.reductionPercentage = (fileCon.Length * 100) / fileSin.Length;
                compresiones.Add(compression);
                return StatusCode(200);
            
            }
            catch
            {
                return StatusCode(500);
            }
        }
        [HttpPost("decompress")]
        public void decompress ([FromForm] IFormFile name)
        {
            try
            {
                Heap<Compression> heap = new Heap<Compression>();
                heap.Decompression(Path.GetFullPath(name.FileName));

            }
            catch
            {

            }
        }
        [HttpGet("compressions")]
        public IEnumerable<Compression> compressions()
        {
            var result = compresiones;
           return result;
        }
    }
}

