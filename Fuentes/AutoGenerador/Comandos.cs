using System;
using System.Data;
using System.Data.Common;

namespace BaseDatos
{
    public class Comandos : Conexion, IDisposable
    {
        #region "Miembros"
        private DataSet m_dataset;
        private DataTable m_datatable;
        private DbCommand m_comando;
        private DbDataAdapter m_adaptador;
        private string m_ultimoerror;
        private string m_nombrecursorsalida;
        #endregion

        #region "Propiedades"
        public string UltimoError
        {
            get { return m_ultimoerror; }
        }
        public string NombreCursorSalida
        {
            get { return m_nombrecursorsalida; }
            set { m_nombrecursorsalida = value; }
        }
        public string proveedor
        {
            get { return MProveedor; }
        }
        #endregion

        #region "Constructores"
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strconexion">Cadena de conexion con la base de datos</param>
        /// <param name="proveedor">Proveedor de datos. Ej:System.Data.SqlClient</param>
        public Comandos(string strconexion, string proveedor)
            : base(strconexion, proveedor)
        {
            m_comando = base.ObtenerFabrica().CreateCommand();
            m_comando.Connection = base.MConexion;
            m_comando.CommandTimeout = 0;
        }
        /// <summary>
        /// Constructor, creado especialmente para trabajar con Oracle.
        /// </summary>
        /// <param name="strconexion">Cadena de conexion con la base de datos<param>
        /// <param name="proveedor">Proveedor de datos. Ej:System.Data.SqlClient</param>
        /// <param name="nombrecursorsalida">Nombre del sursor que se usará para retornar datos</param>
        public Comandos(string strconexion, string proveedor, string nombrecursorsalida)
            : base(strconexion, proveedor)
        {
            m_comando = base.ObtenerFabrica().CreateCommand();
            m_comando.Connection = base.MConexion;
            m_comando.CommandTimeout = 0;
            m_nombrecursorsalida = nombrecursorsalida;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                m_comando.Dispose();
                m_adaptador.Dispose();
                m_datatable.Dispose();
                m_dataset.Dispose();
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region "Metodos Locales"
        /// <summary>
        /// Borra todos los parametros que tenga asociados el ejecutor de comandos
        /// </summary>
        public void limpiarParametros()
        {
            m_comando.Parameters.Clear();
        }
        /// <summary>
        /// Agregar un parametro al ejecutor de comandos
        /// </summary>
        /// <param name="nombre">Nombre del parametro</param>
        /// <param name="valor">Valor del parametro</param>
        public void agregarParametro(string nombre, object valor)
        {
            DbParameter param = base.ObtenerFabrica().CreateParameter();
            param.ParameterName = nombre;
            param.Value = valor;
            m_comando.Parameters.Add(param);
        }
        /// <summary>
        /// Retorna el valor de un parametro.
        /// Normalmente utilizado para recuperar los parametros de salida de los PA
        /// </summary>
        /// <param name="nombre">Nombre del parametro del que se desea obtener el valor</param>
        /// <returns>Una objeto tipo Object con el valor del parametro</returns>
        public object obtenerParametro(string nombre)
        {
            object valor;
            try
            {
                valor = m_comando.Parameters[nombre].Value;
            }
            catch
            {
                valor = null;
            }
            return valor;
        }
        /// <summary>
        /// Agregar un parametro al ejecutor de comandos
        /// </summary>
        /// <param name="param">Parametro que se desea agregar</param>
        public void agregarParametro(DbParameter param)
        {
            m_comando.Parameters.Add(param);
        }
        /// <summary>
        /// Agregar un parametro al ejecutor de comandos
        /// </summary>
        /// <param name="nombre">Nombre del parametro</param>
        /// <param name="valor">Valor del parametro</param>
        /// <param name="direccion">Direccion del Parametro (Entrada o Salida)</param>
        public void agregarParametro(string nombre, object valor, ParameterDirection direccion)
        {
            DbParameter param = base.ObtenerFabrica().CreateParameter();
            param.ParameterName = nombre;
            param.Value = valor;
            param.Direction = direccion;
            m_comando.Parameters.Add(param);
        }
        public void agregarParametro(string nombre, object valor, ParameterDirection direccion, int tamano)
        {
            DbParameter param = base.ObtenerFabrica().CreateParameter();
            param.ParameterName = nombre;
            param.Value = valor;
            param.Direction = direccion;
            param.Size = 100;
            m_comando.Parameters.Add(param);
        }
        /// <summary>
        /// Obtiene un mensaje de error entendible para el usuario a partir de un objeto Exception
        /// </summary>
        /// <param name="e">Excepcion capturada que se dedea "Traducir"</param>
        /// <returns>Mensaje de error entendible</returns>
        private string obtenerError(DbException e)
        {
            string mensaje = e.Message;
            switch (base.MProveedor)
            {
                case "System.Data.SqlClient":
                    {
                        System.Data.SqlClient.SqlException ex = (System.Data.SqlClient.SqlException)e;
                        switch (ex.Number)
                        {
                            case 547:
                                {
                                    mensaje = "El registro se encuentra asociado en otra tabla.";
                                    break;
                                }
                            case 2627:
                                {
                                    mensaje = "Uno de los valores ingresados ya se encuentra registrado.";
                                    break;
                                }
                        }
                        break;
                    }
                case "System.Data.OracleClient":
                    {
                        //System.Data.OracleClient.OracleException ex = (System.Data.OracleClient.OracleException)e;
                        //switch (ex.Code)
                        //{
                        //    case 547:
                        //        {
                        //            mensaje = "El registro se encuentra asociado en otra tabla.";
                        //            break;
                        //        }
                        //}
                        break;
                    }
            }
            return mensaje;
        }
        /// <summary>
        /// Asigna el mensaje de Error.
        /// </summary>
        /// <param name="mensajeError">Mensaje de Error</param>
        public void asignarError(string mensajeError)
        {
            this.m_ultimoerror = mensajeError;
        }
        /// <summary>
        /// Agrega un parametro de tipo cursor usando el nombre de parametro de cursor predeterminado. UNICAMENTE PARA ORACLE
        /// </summary>
        public void agregarParametroCursor()
        {
            //System.Data.OracleClient.OracleParameter cursor = new System.Data.OracleClient.OracleParameter();
            //cursor.ParameterName = m_nombrecursorsalida;
            //cursor.OracleType = System.Data.OracleClient.OracleType.Cursor;
            //cursor.Direction = ParameterDirection.Output;
            //m_comando.Parameters.Add(cursor);
        }
        /// <summary>
        /// Agrega un parametro de tipo cursor. UNICAMENTE PARA ORACLE
        /// </summary>
        /// <param name="nombreCursor">Nombre del parametro cursor</param>
        public void agregarParametroCursor(string nombreCursor)
        {
            //System.Data.OracleClient.OracleParameter cursor = new System.Data.OracleClient.OracleParameter();
            //cursor.ParameterName = nombreCursor;
            //cursor.OracleType = System.Data.OracleClient.OracleType.Cursor;
            //cursor.Direction = ParameterDirection.Output;
            //m_comando.Parameters.Add(cursor);
        }
        #endregion

        #region "Metodos de Base de Datos"
        /// <summary>
        /// Ejecuta un procedimiento almacenado de consulta
        /// </summary>
        /// <param name="nombrePA">Nombre del procedimiento</param>
        /// <returns>Conjunto de datos devuelto por el procedimiento</returns>
        public DataSet ejecutarProcedimiento(string nombrePA)
        {
            m_comando.CommandText = nombrePA;
            m_comando.CommandType = CommandType.StoredProcedure;
            m_dataset = new DataSet();
            m_adaptador = ObtenerFabrica().CreateDataAdapter();
            if (m_adaptador != null) m_adaptador.SelectCommand = m_comando;
            Abrir();
            try
            {
                if (m_adaptador != null) m_adaptador.Fill(m_dataset);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar el Procedimiento Almacenado. " + ex.Message);
            }
            finally
            {
                Cerrar();
            }

            return m_dataset;
        }
        /// <summary>
        /// Ejecuta un procedimiento almacenado que NO es de consulta sino de manipulacion de datos
        /// </summary>
        /// <param name="nombrePA">Nombre del procedimiento</param>
        /// <returns>Numero de filas afectadas por la ejecucion del procedimiento</returns>
        public int ejecutarProcedimientoNQ(string nombrePA)
        {
            int filasafectadas;
            m_comando.CommandText = nombrePA;
            m_comando.CommandType = CommandType.StoredProcedure;
            base.Abrir();
            try
            {
                filasafectadas = m_comando.ExecuteNonQuery();
            }
            catch (DbException ex)
            {
                filasafectadas = -1;
                m_ultimoerror = obtenerError(ex);
            }
            finally
            {
                base.Cerrar();
            }
            return filasafectadas;
        }
        /// <summary>
        /// Ejecuta un procedimiento almacenado en forma Escalar
        /// </summary>
        /// <param name="nombrePA">Nombre del procedimiento</param>
        /// <returns>El valor de la primera fila y la primera columna del conjunto de datos obtenido</returns>
        public object ejecutarProcedimientoE(string nombrePA)
        {
            object datoretorno = new object();
            m_comando.CommandText = nombrePA;
            m_comando.CommandType = CommandType.StoredProcedure;
            base.Abrir();
            try
            {
                datoretorno = m_comando.ExecuteScalar();
            }
            catch (DbException ex)
            {
                datoretorno = null;
                m_ultimoerror = obtenerError(ex);
            }
            finally
            {
                base.Cerrar();
            }
            return datoretorno;
        }
        /// <summary>
        /// Obtiene el conjunto de datos de una tabla o vista
        /// </summary>
        /// <param name="nombre">Nombre de la tabla</param>
        /// <returns>Conjunto de datos obtenido</returns>
        public DataTable obtenerTabla(string nombre)
        {
            m_datatable = new DataTable();
            m_comando.CommandText = nombre;
            m_comando.CommandType = CommandType.TableDirect;
            m_adaptador = base.ObtenerFabrica().CreateDataAdapter();
            m_adaptador.SelectCommand = m_comando;
            base.Abrir();
            try
            {
                m_adaptador.Fill(m_datatable);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar la tabla. " + ex.Message);
            }
            finally
            {
                base.Cerrar();
            }
            return m_datatable;
        }
        /// <summary>
        /// Obtiene un conjunto de datos resultado de una consulta SQL
        /// </summary>
        /// <param name="consultaSQL">Consulta SQL que se desea ejecutar</param>
        /// <returns>Conjunto de datos obtenido</returns>
        public DataTable obtenerConsulta(string consultaSQL)
        {
            consultaSQL.Trim();
            m_datatable = new DataTable();
            m_comando.CommandText = consultaSQL;
            m_comando.CommandType = CommandType.Text;
            m_adaptador = base.ObtenerFabrica().CreateDataAdapter();
            m_adaptador.SelectCommand = m_comando;
            base.Abrir();
            try
            {
                m_adaptador.Fill(m_datatable);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar la consulta. " + ex.Message);
            }
            finally
            {
                base.Cerrar();
            }
            return m_datatable;
        }
        /// <summary>
        /// Ejecuta una instruccion SQL que no es de consulta sino de manipulacion de datos tales como:
        /// INSERT, UPDATE, DELETE, etc.
        /// </summary>
        /// <param name="sentenciaSQL">Intruccion SQL que se va a ejecutar</param>
        /// <returns>Numero de filas afectadas por la ejecucion de la instruccion</returns>
        public int ejecutarSentencia(string sentenciaSQL)
        {
            int filasafectadas;
            m_comando.CommandText = sentenciaSQL;
            m_comando.CommandType = CommandType.Text;
            base.Abrir();
            try
            {
                filasafectadas = m_comando.ExecuteNonQuery();
            }
            catch (DbException ex)
            {
                filasafectadas = -1;
                m_ultimoerror = obtenerError(ex);
            }
            finally
            {
                base.Cerrar();
            }
            return filasafectadas;
        }

       
        #endregion
    }
}
