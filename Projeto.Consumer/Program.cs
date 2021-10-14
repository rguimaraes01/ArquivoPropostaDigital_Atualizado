using Projeto.Domain;
using Projeto.Infra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading;

namespace Projeto.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                //Console.SetWindowSize(50, 100);
                Console.WriteLine();
                Console.WriteLine("############################################################");
                Console.WriteLine("##       Iniciando Processo da Geração do Arquivo         ##");
                Console.WriteLine("############################################################");
                Console.WriteLine();
                args = new string[1];
                args[0] = "informatica";

                //var diretorio = Environment.CurrentDirectory + @"\ArquivoKitDigital";
                var diretorioAtual = Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString());
                var diretorio = diretorioAtual + @"\ArquivoPropostaDigital";

                if (!Directory.Exists(diretorio))
                {
                    Directory.CreateDirectory(diretorio);
                }
                string[] arquivos = Directory.GetFiles(diretorio);
                if (arquivos.Count() > 0)
                {
                    var subDiretorio = Path.Combine(diretorio, "Old");
                    if (!Directory.Exists(subDiretorio))
                    {
                        Directory.CreateDirectory(subDiretorio);
                    }
                    foreach (var item in arquivos)
                    {
                        File.Copy(item, Path.Combine(subDiretorio, Path.GetFileName(item)), true);
                        File.Delete(item);
                    }
                }
                var lista = new ConcurrentBag<Proposta>();
                string nomeArquivo = "PropostaDigital" + DateTime.Now.ToString("ddMMyyyyHHmmss");
                if (args.Length > 0)
                {
                    string usuario = args[0];
                    var propostas = new PropostaDao().GetPropostas(usuario, nomeArquivo);
                    if (propostas.Count() > 0)
                    {
                        Parallel.ForEach(propostas, item =>
                        {
                            lista.Add(item);
                        });
                        propostas = null;
                    }
                }
                Console.WriteLine();
                Console.WriteLine("Criando Arquivo Json...");

                var path = Path.Combine(diretorio, nomeArquivo + ".json");

                //REMOVE BOM
                Encoding utf8WithoutBom = new UTF8Encoding(false);
                //FIM REMOVE BOM
                                
                using (StreamWriter file = new StreamWriter(File.Open(path, FileMode.CreateNew), utf8WithoutBom))
                {
                    var jsonListaIncluded = JsonConvert.SerializeObject(lista, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });
                    file.Write(jsonListaIncluded);
                }               
                
                Console.WriteLine();
                Console.WriteLine("############################################################");
                Console.WriteLine("##                 FIM Processo do Arquivo                ##");
                Console.WriteLine("############################################################");
                Console.WriteLine();
                stopwatch.Stop();
                Console. WriteLine($"Tempo passado: {stopwatch.Elapsed}");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}