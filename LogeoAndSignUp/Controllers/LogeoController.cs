using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using LogeoAndSignUp.Models;

namespace LogeoAndSignUp.Controllers
{
    public class LogeoController : Controller
    {
        //Cadena de coexion a base de datos
        static string cadena = "Server = DESKTOP-CS3EA1C ; database=DB_LOGUEO; Integrated Security = true;";
        // GET: Logeo
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(Usuario oUsuario)
        {
            bool registrado;
            string mensaje;

            if(oUsuario.Contrasenia == oUsuario.ConfirmarContrasenia)
            {
                oUsuario.Contrasenia = EncriptarPass(oUsuario.Contrasenia);
            }
            else
            {
                //viewdata permite enviar mensajes del controlador a nuestra vista
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Mail);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Contrasenia);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar,100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value.ToString();
            }

            ViewData["Mensaje"] = mensaje;

            if (registrado)
            {
                return RedirectToAction("Login", "Logeo");
            }
            else
            {
                return View();
            }
            
        }

        [HttpPost]
        public ActionResult Login(Usuario oUsuario)
        {
            oUsuario.Contrasenia = EncriptarPass(oUsuario.Contrasenia);

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Mail);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Contrasenia);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                oUsuario.IdUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());

            }
            if(oUsuario.IdUsuario != 0)
            {
                Session["usuario"] = oUsuario;
                return RedirectToAction("Index","Home");
            }
            else
            {
                ViewData["Mensaje"] = "usuario no encontrado";
                return View();
            }
        }

        //metdo para encriptar contraseña
        public static string EncriptarPass(string texto)
        {
            StringBuilder Sb = new StringBuilder();
            using(SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach(byte b in result)
                {
                    Sb.Append(b.ToString("x2"));
                }
            }
            return Sb.ToString();
        }
    }
}