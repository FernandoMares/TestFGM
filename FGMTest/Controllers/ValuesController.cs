using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using FGMTest.Models;
using Microsoft.Ajax.Utilities;

namespace FGMTest.Controllers
{


    public class ValuesController : ApiController
    {

        public DB_FGMTESTEntities1 db = new DB_FGMTESTEntities1();
        public clsMensajesRetorno response = new clsMensajesRetorno();


        [System.Web.Http.AcceptVerbs("POST")]
        public clsMensajesRetorno Login(clsLogin _login)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    response.registroOK = false;
                    response.detalle = "Usuario y contraseña son campos obligatorios";
                    response.fechaIncidencia = DateTime.Now;
                    registrarLog(response);
                    return response;
                }
                USUARIO usuario = db.USUARIO.Find(_login.correo);
                if (usuario is null)
                {
                    response.registroOK = false;
                    response.detalle = "El correo '" + _login.correo + "' no existe en el sistema.";
                    response.fechaIncidencia = DateTime.Now;
                    registrarLog(response);
                    return response;
                }

                string hash = sha256encrypt(_login.correo, _login.password);//Valor encriptado de la contraseña.
                if (!usuario.USU_CONTRA.Equals(hash))
                {
                    response.registroOK = false;
                    response.detalle = "Correo y/o contraseña incorrectos.";
                    response.fechaIncidencia = DateTime.Now;
                    registrarLog(response);
                    return response;
                }

                //Pasó todo...
                response.registroOK = true;
                response.detalle = "¡Bienvenido " + usuario.USU_NOMBRE + "(" + usuario.USU_CORREO + ")!";
                response.fechaIncidencia = DateTime.Now;
                registrarLog(response);

                //Registrar en histórico de logins...
                HISTORICO_LOGIN histLogin = new HISTORICO_LOGIN();
                histLogin.HIST_LOG_FECHA = DateTime.Now;
                histLogin.USU_CORREO = usuario.USU_CORREO;
                db.HISTORICO_LOGIN.Add(histLogin);
                db.SaveChanges();


                return response;
            }
            catch(Exception e)
            {
                response.registroOK = false;
                response.detalle = e.Message;
                response.fechaIncidencia = DateTime.Now;
                registrarLog(response, "Manejo de excepciones");
                return response;
            }

        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        // GET api/values
        public IHttpActionResult ObtenerListaUsuarios()
        {
            try
            {
                string[] lstRetornar;
                using (DB_FGMTESTEntities1 db = new DB_FGMTESTEntities1())
                {
                    var catalogo = db.USUARIO.ToList();

                    List<object> Usuarios = new List<object>();
                    foreach (var item in catalogo)
                    {
                        Usuarios.Add(new { ID = item.USU_ID, Nombre = item.USU_NOMBRE, Correo = item.USU_CORREO, FechaCreacion = item.USU_FECHA_CREACION });
                    }
                    //return new string[] { "value1", "value2" };
                    return Ok<object>(new { Usuarios });
                }
            }
            catch (Exception e)
            {
                response.registroOK = false;
                response.detalle = e.Message;
                response.fechaIncidencia = DateTime.Now;
                registrarLog(response, "Manejo de excepciones");
                return BadRequest("Manejo de excepciones");
            }
          

        }

        [System.Web.Http.AcceptVerbs("POST","PUT")]
        public clsMensajesRetorno RegistrarUsuario([FromBody] clsUsuario _usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    response.registroOK = false;
                    response.detalle = "Nombre, Correo y Contraseña son campos obligatorios; (Faltan " + ModelState.Count.ToString() + " campo(s))";
                    response.fechaIncidencia = DateTime.Now;
                    registrarLog(response);
                    return response;
                }
                USUARIO usu = db.USUARIO.Find(_usuario.correo);
                if (usu != null)
                {
                    response.registroOK = false;
                    response.detalle = "El correo " + _usuario.correo + " se encuentra en el sistema. Intenta con uno nuevo.";
                    response.fechaIncidencia = DateTime.Now;
                    registrarLog(response);
                    return response;
                }
                usu = new USUARIO();
                usu.USU_CORREO = _usuario.correo;
                usu.USU_FECHA_CREACION = DateTime.Now;
                usu.USU_NOMBRE = _usuario.nombre;
                usu.USU_SALT = CreateSalt(_usuario.correo);
                usu.USU_CONTRA = sha256encrypt(_usuario.correo, _usuario.password);
                usu.USU_ACTIVO = true;

                db.USUARIO.Add(usu);

                db.SaveChanges();


                response.registroOK = true;
                response.detalle = "Usuario " + _usuario.correo + " agregado correctamente";
                response.fechaIncidencia = DateTime.Now;
                registrarLog(response);
                return response;
            }
            catch (Exception e)
            {
                response.registroOK = false;
                response.detalle = e.Message;
                response.fechaIncidencia = DateTime.Now;
                registrarLog(response, "Manejo de excepciones");
                return response;
            }
            
        }


        [System.Web.Http.AcceptVerbs("POST","PUT")]
        public clsMensajesRetorno agregarProducto([FromBody] clsProducto _producto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    response.registroOK = false;
                    response.detalle = "Nombre, Descripción y precio son campos obligatorios; (Faltan " + ModelState.Count.ToString() + " campo(s))";
                    response.fechaIncidencia = DateTime.Now;
                    registrarLog(response);
                    return response;
                }

                PRODUCTO prod = db.PRODUCTO.Find(_producto.nombre);
                if (prod != null)
                {
                    response.registroOK = false;
                    response.detalle = "El producto: " + _producto.nombre + " se encuentra en el sistema. Intenta con uno nuevo.";
                    response.fechaIncidencia = DateTime.Now;
                    registrarLog(response);
                    return response;
                }
                prod = new PRODUCTO();

                prod.PROD_PRECIO = _producto.precio;
                prod.PROD_DESCRIPCION = _producto.descripcion;
                prod.PROD_NOMBRE = _producto.nombre;

                db.PRODUCTO.Add(prod);
                db.SaveChangesAsync();

                response.registroOK = true;
                response.detalle = "Producto: " + _producto.nombre + " agregado correctamente";
                response.fechaIncidencia = DateTime.Now;
                registrarLog(response);
                return response;
            }
            catch (Exception e)
            {
                response.registroOK = false;
                response.detalle = e.Message;
                response.fechaIncidencia = DateTime.Now;
                registrarLog(response, "Manejo de excepciones");
                return response;
            }
            
        }


        [System.Web.Http.AcceptVerbs("POST","PUT")]
        public clsMensajesRetorno editarProducto(string nombreProducto, [FromBody] clsProducto _producto)
        {
            try
            {
                if (nombreProducto is null)
                {
                    response.detalle = "El nombre de producto es obligatorio para actualizarlo.";
                    response.registroOK = false;
                    response.fechaIncidencia = DateTime.Now;
                    registrarLog(response);
                    return response;
                }
                PRODUCTO prod = db.PRODUCTO.Find(nombreProducto);
                if (prod is null)
                {
                    response.detalle = "El producto '" + nombreProducto + "' no se encuentra en el sistema.";
                    response.registroOK = false;
                    response.fechaIncidencia = DateTime.Now;
                    registrarLog(response);
                    return response;
                }

                if (!_producto.descripcion.IsNullOrWhiteSpace())
                {
                    prod.PROD_DESCRIPCION = _producto.descripcion;

                }
                if (_producto.precio != 0)
                {
                    prod.PROD_PRECIO = _producto.precio;
                }
                db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                response.registroOK = true;
                response.detalle = nombreProducto + " modificado correctamente";
                response.fechaIncidencia = DateTime.Now;
                registrarLog(response);
                return response;
            }
            catch (Exception e)
            {
                response.registroOK = false;
                response.detalle = e.Message;
                response.fechaIncidencia = DateTime.Now;
                registrarLog(response, "Manejo de excepciones");
                return response;
            }
            
        }


        [System.Web.Http.AcceptVerbs("DELETE")]
        public clsMensajesRetorno eliminarProducto([FromBody] clsProducto _producto)
        {
            try
            {
                if (_producto.nombre.IsNullOrWhiteSpace())
                {
                    response.registroOK = false;
                    response.detalle = "Ingresar el nombre del producto para eliminarlo.";
                    response.fechaIncidencia = DateTime.Now;
                    registrarLog(response);
                    return response;
                }

                PRODUCTO prod = db.PRODUCTO.Find(_producto.nombre);
                if (prod == null)
                {
                    response.registroOK = false;
                    response.detalle = "Producto '" + _producto.nombre + "' no se encuentra en el sistema, intentar nuevamente.";
                    response.fechaIncidencia = DateTime.Now;
                    registrarLog(response);
                    return response;
                }

                if (prod.PROD_ELIMINADO is true)
                {
                    response.registroOK = false;
                    response.detalle = "Producto '" + _producto.nombre + "' ya se ha eliminado previamente del sistema. Intenta con uno existente.";
                    response.fechaIncidencia = DateTime.Now;
                    registrarLog(response);
                    return response;
                }

                prod.PROD_ELIMINADO = true;
                db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                response.registroOK = true;
                response.detalle = "Producto '" + _producto.nombre + "' eliminado correctamente.";
                response.fechaIncidencia = DateTime.Now;
                registrarLog(response);
                return response;
            }
            catch (Exception e)
            {
                response.registroOK = false;
                response.detalle = e.Message;
                response.fechaIncidencia = DateTime.Now;
                registrarLog(response, "Manejo de excepciones");
                return response;
            }
            
        }

        [System.Web.Http.AcceptVerbs("POST")]
        // GET api/values
        public IHttpActionResult ObtenerListaProductos()
        {
            try
            {
                string[] lstRetornar;
                using (DB_FGMTESTEntities1 db = new DB_FGMTESTEntities1())
                {
                    var catalogo = db.PRODUCTO.ToList();

                    List<object> Productos = new List<object>();
                    foreach (var item in catalogo)
                    {
                        Productos.Add(new { ID = item.PROD_ID, Nombre = item.PROD_NOMBRE, Descripcion = item.PROD_DESCRIPCION, Eliminado = item.PROD_ELIMINADO });
                    }
                    return Ok<object>(new { Productos });
                }
            }
            catch (Exception e)
            {
                response.registroOK = false;
                response.detalle = e.Message;
                response.fechaIncidencia = DateTime.Now;
                registrarLog(response, "Manejo de excepciones");
                return BadRequest("Manejo de Excepciones");
            }
           

        }



        bool registrarLog(clsMensajesRetorno _mensajeRetorno, string aux1 = null, string aux2 = null, string aux3 = null)
        {
            //Procedimiento para guardar en tabla de logs los movimientos...
            bool registrado = false;
            try
            {
                LOGS log = new LOGS();

                log.LOG_FECHA = DateTime.Now;
                log.LOG_ERROR = _mensajeRetorno.registroOK;
                log.LOG_MENSAJE = _mensajeRetorno.detalle;
                log.LOG_CAMPO_AUX1 = aux1;
                log.LOG_CAMPO_AUX2 = aux2;
                log.LOG_CAMPO_AUX3 = aux3;

                db.LOGS.Add(log);
                db.SaveChanges();
                registrado = true;
            }
            catch(Exception e)
            {

            }



            return registrado;
        }


        /*-----------INICIO ENCRIPTADO---------------------*/
        public static string sha256encrypt(string Usuario, string Pass)
        {
            string salt = CreateSalt(Pass);
            string saltAndPwd = String.Concat(Usuario, salt);
            UTF8Encoding encoder = new UTF8Encoding();
            SHA256Managed sha256hasher = new SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(saltAndPwd));
            string hashedPwd = String.Concat(byteArrayToString(hashedDataBytes), salt);
            return hashedPwd;
        }

        public static string byteArrayToString(byte[] inputArray)
        {
            StringBuilder output = new StringBuilder("");
            for (int i = 0; i < inputArray.Length; i++)
            {
                output.Append(inputArray[i].ToString("X2"));
            }
            return output.ToString();
        }

        private static string CreateSalt(string UserName)
        {
            string username = UserName;
            byte[] userBytes;
            string salt;
            userBytes = ASCIIEncoding.ASCII.GetBytes(username);
            long XORED = 0x00;

            foreach (int x in userBytes)
                XORED = XORED ^ x;

            Random rand = new Random(Convert.ToInt32(XORED));
            salt = rand.Next().ToString();
            salt += rand.Next().ToString();
            salt += rand.Next().ToString();
            salt += rand.Next().ToString();
            return salt;
        }
        /*-----------FIN ENCRIPTADO---------------------*/

    }
}
