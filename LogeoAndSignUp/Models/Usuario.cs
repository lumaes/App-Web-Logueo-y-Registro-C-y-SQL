using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogeoAndSignUp.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Contrasenia { get; set; }
        public string Mail { get; set; }
        public string ConfirmarContrasenia { get; set; }

    }
}