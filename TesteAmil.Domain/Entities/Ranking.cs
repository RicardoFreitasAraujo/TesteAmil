using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteAmil.Domain.Entities
{
    public class Ranking
    {
        public List<Partida> Partidas { get; set; }
        public Ranking()
        {
            this.Partidas = new List<Partida>();
        }
    }
}
