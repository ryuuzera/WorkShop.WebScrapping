using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Workshop.Models
{

    public class Equipe
    {
        [JsonPropertyName("escudo")]
        public string? Escudo { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nome_popular")]
        public string? NomePopular { get; set; }

        [JsonPropertyName("sigla")]
        public string? Sigla { get; set; }
    }

    public class Equipes
    {
        [JsonPropertyName("mandante")]
        public Equipe? Mandante { get; set; }

        [JsonPropertyName("visitante")]
        public Equipe? Visitante { get; set; }
    }

    public class Sede
    {
        [JsonPropertyName("nome_popular")]
        public string? NomePopular { get; set; }
    }

    public class Broadcast
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("label")]
        public string? Label { get; set; }
    }

    public class Transmissao
    {
        [JsonPropertyName("broadcast")]
        public Broadcast? Broadcast { get; set; }

        [JsonPropertyName("label")]
        public string? Label { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }

    public class Jogo
    {
        [JsonPropertyName("data_realizacao")]
        public DateTime DataRealizacao { get; set; }

        [JsonPropertyName("equipes")]
        public Equipes? Equipes { get; set; }

        [JsonPropertyName("hora_realizacao")]
        public string? HoraRealizacao { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("jogo_ja_comecou")]
        public bool JogoJaComecou { get; set; }

        [JsonPropertyName("placar_oficial_mandante")]
        public int? PlacarOficialMandante { get; set; }

        [JsonPropertyName("placar_oficial_visitante")]
        public int? PlacarOficialVisitante { get; set; }

        [JsonPropertyName("placar_penaltis_mandante")]
        public int? PlacarPenaltisMandante { get; set; }

        [JsonPropertyName("placar_penaltis_visitante")]
        public int? PlacarPenaltisVisitante { get; set; }

        [JsonPropertyName("sede")]
        public Sede? Sede { get; set; }

        [JsonPropertyName("transmissao")]
        public Transmissao? Transmissao { get; set; }
    }
}
