using System;
using System.Data.Common;

namespace BaseDatos
{
    /// <summary>
    /// Clase para crear y manejar la conexion con la Base de Datos
    /// </summary>
    public abstract class Conexion
    {
        #region "Miembros"
        protected string MStrconexion;
        protected string MProveedor;
        protected DbConnection MConexion;
        private const string Defproveedor = "System.Data.SqlClient";
        #endregion

        #region "Constructores"
        /// <summary>
        /// COnstructor por defecto
        /// </summary>
        public Conexion()
        {
            MStrconexion = "";
            MConexion = null;
            MProveedor = Defproveedor;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strconexion">Cadena de conexion con la base de datos</param>
        /// <param name="proveedor">Proveedor de datos. Ej:System.Data.SqlClient</param>
        public Conexion(string strconexion, string proveedor)
        {
            if (string.IsNullOrEmpty(strconexion))
                strconexion = "";
            if (string.IsNullOrEmpty(proveedor))
                proveedor = Defproveedor;
            MStrconexion = strconexion;
            MProveedor = proveedor;
            MConexion = CrearConexion();
            MConexion.ConnectionString = MStrconexion;
        }
        #endregion

        #region "Metodos"
        /// <summary>
        /// Retorna la fabrica del proveedor
        /// </summary>
        /// <returns></returns>
        protected DbProviderFactory ObtenerFabrica()
        {
            return DbProviderFactories.GetFactory(MProveedor);
        }
        /// <summary>
        /// Retorna la nueva conexion
        /// </summary>
        /// <returns></returns>
        private DbConnection CrearConexion()
        {
            return ObtenerFabrica().CreateConnection();
        }
        /// <summary>
        /// Abre la conexion con la Base de Datos
        /// </summary>
        protected void Abrir()
        {
            try
            {
                if (MConexion.State == System.Data.ConnectionState.Closed)
                    MConexion.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("No se logró abrir la conexión con la Base de Datos. " + ex.Message);
            }
        }
        /// <summary>
        /// Cierra la conexion con la Base de Datos
        /// </summary>
        protected void Cerrar()
        {
            try
            {
                if (MConexion.State != System.Data.ConnectionState.Closed)
                    MConexion.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("No se  logró cerrar la conexion con la Base de Datos. " + ex.Message);
            }
        }
        #endregion
    }
}
