using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteAmil.Analisador.Enum;
using TesteAmil.Domain.Entities;

namespace TesteAmil.Analisador
{
    /// <summary>
    /// Classe responsável por Analisar Log
    /// </summary>
    public class AnalisadorRanking
    {


        #region Propriedades Privadas
        private const string NOME_ASSASSINO_DESCONSIDERAR = "<WORLD>";
        private string PartidaId { get; set; }
        private Ranking Ranking {get; set;}
        private List<string> linhas { get; set; }
        #endregion

        #region Propriedades Públicas
        public string CaminhoArquivoLog { get; private set; }
        #endregion

        #region Construtor
        public AnalisadorRanking(string caminhoArquivoLog)
        {
            this.Ranking = new Ranking();
            this.linhas = new List<string>();
            this.CaminhoArquivoLog = caminhoArquivoLog;

            if (!File.Exists(this.CaminhoArquivoLog))
                throw new Exception("O caminho do arquivo não existe");
        }
        #endregion

        #region Métodos Públicos
        public Ranking Analisar()
        {
            this.CarregarLinhas();
            this.PartidaId = String.Empty;

            foreach (string linha in this.linhas)
            {
                TipoEventoLog tipoeventoLog = this.TipoEventoLogPorLinha(linha);

                if (tipoeventoLog == TipoEventoLog.InicioPartida)
                {
                    this.InicioPartida(linha);
                }
                if (tipoeventoLog == TipoEventoLog.JogadorAsssinouPorArma)
                {
                    this.JogadorAsssinouPorArma(linha);
                }
                if (tipoeventoLog == TipoEventoLog.FimPartida)
                {
                    this.FimPartida(linha);
                }
                if (tipoeventoLog == TipoEventoLog.NDA)
                {

                }
            }

            return this.Ranking;
        }
        #endregion

        #region Métodos Privados
        public void CarregarLinhas()
        {
            using (StreamReader sr = File.OpenText(this.CaminhoArquivoLog))
            {
                // Read the stream to a string, and write the string to the console.
                String linha = String.Empty;
                while ((linha = sr.ReadLine()) != null)
                {
                    this.linhas.Add(linha);
                }
            }
        }

        public TipoEventoLog TipoEventoLogPorLinha(string linha)
        {

            if (linha.IndexOf("started") > 0)
                return TipoEventoLog.InicioPartida;

            if (linha.IndexOf("ended") > 0)
                return TipoEventoLog.FimPartida;

            if (linha.IndexOf("killed") > 0)
                return TipoEventoLog.JogadorAsssinouPorArma;

            return TipoEventoLog.NDA;
        }

        public void InicioPartida(string linha)
        {
            //New match 11348965 has started  
            string[] dados = linha.Split(new string[] { " " },StringSplitOptions.None);
            string Id = dados[5];
            string Datainicio = dados[0] + dados[1];

            Partida partida = this.Ranking.Partidas.Where(p => p.Id == Id).FirstOrDefault();
            if (partida == null)
            {
                this.Ranking.Partidas.Add(new Partida { DataInicio = Datainicio, Id = Id });
            }
            else
            {
                partida.DataInicio = Datainicio;
            }

            this.PartidaId = Id;
        }

        public void JogadorAsssinouPorArma(string linha)
        {
            //23/04/2013 15:36:04 - Roman killed Nick using M16  
            string[] dados = linha.Split(new string[] { " " }, StringSplitOptions.None);
            string PartidaId = this.PartidaId; 
            string Data = dados[0] + dados[1];
            string NomeAssasino = dados[3];
            string NomeVitima = dados[5];
            string NomeArma = dados[7];

            Partida partida = this.Ranking.Partidas.Where(p => p.Id == PartidaId).FirstOrDefault();
            if (partida == null)
                return;

            if (NomeAssasino != NOME_ASSASSINO_DESCONSIDERAR)
            {
                //Assasino
                Jogador assassino = partida.Jogadores.Where(j => j.Nome == NomeAssasino).FirstOrDefault();
                if (assassino == null)
                {
                    partida.Jogadores.Add(new Jogador { Nome = NomeAssasino, NumeroAssassinatos = 1, NumeroMortes = 0 });
                }
                else
                {
                    assassino.NumeroAssassinatos = assassino.NumeroAssassinatos + 1;
                }

                //Arma do Assassinato
                Jogador assassinoArma = partida.Jogadores.Where(j => j.Nome == NomeAssasino).FirstOrDefault();
                JogadorArmaAssasinatos arma = assassinoArma.Armas.Where(a => a.NomeArma == NomeArma).FirstOrDefault();
                if (arma == null)
                {
                    assassinoArma.Armas.Add(new JogadorArmaAssasinatos { NomeArma = NomeArma, NumeroAssasinatos = 1 });
                } else
                {
                    arma.NumeroAssasinatos = arma.NumeroAssasinatos;
                }

                //Arma mais utilizada
                assassinoArma.ArmaPreferida = String.Empty;
                if (assassinoArma.Armas.Count > 0)
                {
                    assassinoArma.ArmaPreferida = assassinoArma.Armas.OrderByDescending(o => o.NumeroAssasinatos).First().NomeArma ?? String.Empty;
                }
            }

            //Vítima
            Jogador vitima = partida.Jogadores.Where(j => j.Nome == NomeVitima).FirstOrDefault();
            if (vitima == null)
            {
                partida.Jogadores.Add(new Jogador { Nome = NomeVitima, NumeroAssassinatos = 0, NumeroMortes = 1,ArmaPreferida = String.Empty });
            }
            else
            {
                vitima.NumeroMortes = vitima.NumeroMortes + 1;
            }


        }

        public void FimPartida(string linha)
        {
            this.PartidaId = String.Empty;
            string[] dados = linha.Split(new string[] { " " }, StringSplitOptions.None);
            string Id = dados[4];
            string DataFim = dados[0] + dados[1];

            Partida partida = this.Ranking.Partidas.Where(p => p.Id == Id).FirstOrDefault();
            if (partida != null)
            {
                partida.DataFim = DataFim;
            }
        }

        #endregion


    }
}
