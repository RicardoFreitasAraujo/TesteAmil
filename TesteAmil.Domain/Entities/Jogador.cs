using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteAmil.Domain.Entities
{
    public class Jogador
    {
        public string Nome { get; set; }
        public int NumeroAssassinatos { get; set; }
        public int NumeroMortes { get; set; }
        public string ArmaPreferida { get; set; }

        public List<JogadorArmaAssasinatos> Armas { get; set; }

        public Jogador()
        {
            this.NumeroAssassinatos = 0;
            this.NumeroMortes = 0;
            this.Armas = new List<JogadorArmaAssasinatos>();
        }
    }
}
