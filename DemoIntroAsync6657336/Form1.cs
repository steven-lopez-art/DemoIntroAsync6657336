using System.Diagnostics;

namespace DemoIntroAsync6657336
{
    public partial class Form1 : Form
    {

        HttpClient httpClient = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        //Peligro async void deve ser evitado, EXCEPTO en eventos.
        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;

            var directorioActual = AppDomain.CurrentDomain.BaseDirectory;
            var destinoBaseSecuencial = Path.Combine(directorioActual, @"Imagenes/resultado-secuencial");
            var destinoBaseParalelo = Path.Combine(directorioActual, @"Imagenes/resultado-paralelo");
            PrepararEjecucion(destinoBaseSecuencial, destinoBaseParalelo);

            Console.WriteLine("Inicio");
            List<Imagen> imagenes = ObtenerImagenes();


            //Parte Secuencial
            var sw = new Stopwatch();
            sw.Start();

            foreach (var imagen in imagenes)
            {
                await ProcesarImagen(destinoBaseSecuencial, imagen);
            }

            Console.WriteLine("Secuencial - duración en segundos: {0}", sw.ElapsedMilliseconds / 1000.0);

            sw.Reset();

            sw.Start();

            var tareasEnumerables = imagenes.Select(async imagen =>
            {
                await ProcesarImagen(destinoBaseParalelo, imagen);
            });

            await Task.WhenAll(tareasEnumerables);

            Console.WriteLine("Paralelos - duracion en segundos: {0}", sw.ElapsedMilliseconds / 1000.0);

            sw.Stop();

            pictureBox1.Visible = false;
        }

        private async Task ProcesarImagen(string directorio, Imagen imagen)
        {
            var respuesta = await httpClient.GetAsync(imagen.URL);
            var contenido = await respuesta.Content.ReadAsByteArrayAsync();

            Bitmap bitmap;

            using (var ms = new MemoryStream(contenido))
            {
                bitmap = new Bitmap(ms);
            }

            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var destino = Path.Combine(directorio, imagen.Nombre);
            bitmap.Save(destino);

        }

        private static List<Imagen> ObtenerImagenes()
        {
            var imagenes = new List<Imagen>();

            for (int i = 0; i < 7; i++)
            {
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"SALVADORENOS {i}.png",
                        URL = "https://2.bp.blogspot.com/-aiDnCFYhqOo/XA81jLNJqjI/AAAAAAAAD88/Y_539Ddz8qkONh8NwlXwedQMA81GFWdqQCLcBGAs/s1600/mapa-el-salvador.jpg"
                    });
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"NAHUALES {i}.jpg",
                        URL = "https://i.pinimg.com/originals/4e/0d/cc/4e0dcc5161703a8468ceb914c263b000.jpg"
                    });
               imagenes.Add(
                   new Imagen()
                    {
                        Nombre = $"MONOS {i}.jpg",
                        URL = "https://th.bing.com/th/id/R.8c3cf7f9bbdce04060b94913aec1b8dd?rik=YLcRN0ebtFSb4A&pid=ImgRaw&r=0"
                    });
            }
            return imagenes;
        }

        private void BorrarArchivos(string directorio)
        {
            var archivos = Directory.EnumerateFiles(directorio);
            foreach (var archivo in archivos)
            {
                File.Delete(archivo);
            }
        }

        private void PrepararEjecucion(string destinoBaseParalelo, string destinoBaseSecuncial)
        {
            if (!Directory.Exists(destinoBaseParalelo))
            {
                Directory.CreateDirectory(destinoBaseParalelo);
            }

            if (!Directory.Exists(destinoBaseSecuncial))
            {
                Directory.CreateDirectory(destinoBaseSecuncial);
            }

            BorrarArchivos(destinoBaseParalelo);
            BorrarArchivos(destinoBaseSecuncial);

        }

        private async Task<string> ProcesamientoLargo()
        {
            await Task.Delay(5000); //asincrono
            return "Felipe";
        }

        private async Task RealizarProcesamientoLargoA()
        {
            await Task.Delay(1000); //asincrona
            Console.WriteLine("Proceso A Finalizado");
        }

        private async Task RealizarProcesamientoLargoB()
        {
            await Task.Delay(1000); //asincrona
            Console.WriteLine("Proceso B Finalizado");
        }
        
        private async Task RealizarProcesamientoLargoC()
        {
            await Task.Delay(1000); //asincrona
            Console.WriteLine("Proceso C Finalizado");
        }
    }
}