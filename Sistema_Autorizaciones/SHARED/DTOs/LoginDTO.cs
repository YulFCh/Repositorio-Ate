using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class LoginDTO {
        public string user {  get; set; }
        public string pass { get; set; }
    }

    public class rsLogin {
        public string mensaje { get; set; }
        public string ip { get; set; }    
        public string host { get; set; }
        public string id_usuario { get; set; }
        public string login { get; set; }
        public int area { get; set; }
        public int cajero { get; set; }
        public string id_doc { get; set; }
        public string num_doc { get; set; }
        public string nombre { get; set; }
        public string caja { get; set; }
        public int nestado { get; set; }
        public string id_perfil { get; set; }
        public string nomb_perfil { get; set; }
        public string nomb_area { get; set; }
        public string encargado { get; set; }
    }
}
