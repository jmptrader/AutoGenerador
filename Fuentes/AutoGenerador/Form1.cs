using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoGenerador
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                buttonEdit1.Text = fbd.SelectedPath + "\\";


        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            StringBuilder archivo = new StringBuilder();
            string propiedades = "";
            string metodos = "";

            if (checkEdit1.Checked)
            {
                DataTable datTablas = cargarEsquemaBD();
                if (datTablas != null)
                {
                    foreach (DataRow dr in datTablas.Rows)
                    {
                        System.IO.StreamReader reader = new System.IO.StreamReader("estructuras\\Entidad.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        string NombreTabla = dr[2].ToString();
                        DataTable datCampos = cargarEsquemaTabla(NombreTabla);

                        archivo.Replace("[TABLE]", "cls" + NombreTabla.Substring(2));
                        if (datCampos != null)
                        {
                            foreach (DataColumn dc in datCampos.Columns)
                            {
                                propiedades += string.Format("Private {0}_{1};{2}", dc.DataType.Name, dc.ColumnName.ToLower(),Environment.NewLine);
                                metodos += string.Format("public bool {0} {",dc.ColumnName);

        
        //    get{
        //        return m_despachado;
        //    }
        //    set{
        //        m_despachado = value;
        //    }
        //}

                            }
                        }
                    }
                }
            }
        }

        public static DataTable cargarEsquemaBD()
        {
            DataTable tabla;
            OleDbConnection conexion = new OleDbConnection(ConfigurationManager.ConnectionStrings["BaseDatos"].ConnectionString);

            try
            {
                conexion.Open();
                tabla = conexion.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                return tabla;
            }
            catch (Exception ex)
            { }
            finally
            {
                conexion.Close();
            }

            return null;
        }

        public static DataTable cargarEsquemaTabla(string NombreTabla)
        {
            DataTable tabla = new DataTable();
            OleDbConnection conexion = new OleDbConnection(ConfigurationManager.ConnectionStrings["BaseDatos"].ConnectionString);

            try
            {
                if (NombreTabla != null)
                {
                    OleDbCommand comando = new OleDbCommand("Select * from " + NombreTabla + " Where 0=1", conexion);
                    OleDbDataAdapter adaptador = new OleDbDataAdapter(comando);
                    tabla = new DataTable();
                    adaptador.Fill(tabla);
                    adaptador.FillSchema(tabla, SchemaType.Source);
                }
                return tabla;
            }
            catch (Exception ex)
            { }
            finally
            {
                conexion.Close();
            }

            return null;
        }
    }
}
