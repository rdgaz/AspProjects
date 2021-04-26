using System;

namespace AppVotacao.Models
{
    public class Voto
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public int RestauranteId { get; set; }
        public int UsuarioId { get; set; }

    }

}