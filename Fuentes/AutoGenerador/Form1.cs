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
            string propiedades;
            string metodos;

            bool exists = System.IO.Directory.Exists(buttonEdit1.Text + "\\Entidades\\");
            if (!exists)
                System.IO.Directory.CreateDirectory(buttonEdit1.Text + "\\Entidades\\");

            
            DataTable datTablas = cargarEsquemaBD();
            if (datTablas != null)
            {
                foreach (DataRow dr in datTablas.Rows)
                {
                    if (checkEdit1.Checked)
                    {
                        propiedades = "";
                        metodos = "";

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
                                propiedades += string.Format("Private {0} m_{1};{2}", dc.DataType.Name, dc.ColumnName.ToLower(), Environment.NewLine);
                                metodos += string.Format("public bool {0} {{ {1} get {{return m_{2};}} {3} set {{m_{4} = value;}} }}", dc.ColumnName, Environment.NewLine, dc.ColumnName.ToLower(), Environment.NewLine, dc.ColumnName.ToLower());
                            }
                        }

                        archivo.Replace("[PROPIEDADES]", propiedades);
                        archivo.Replace("[METODOS]", metodos);

                        System.IO.StreamWriter writer = new System.IO.StreamWriter(string.Format("{0}{1}cls{2}.cs", buttonEdit1.Text, "Entidades\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
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
