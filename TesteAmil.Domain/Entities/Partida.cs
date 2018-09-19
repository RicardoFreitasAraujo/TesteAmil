using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteAmil.Domain.Entities
{
    public class Partida
    {
        public string Id { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public List<Jogador> Jogadores { get; set; }

        public Partida()
        {
            this.Id = String.Empty;
            this.Jogadores = new List<Jogador>();
        }

    }
}
