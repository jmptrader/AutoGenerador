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
            string propiedadesFachada;
            string metodos;
            string enumCampos;

            bool exists = System.IO.Directory.Exists(buttonEdit1.Text + "\\Entidades\\");
            if (!exists)
                System.IO.Directory.CreateDirectory(buttonEdit1.Text + "\\Entidades\\");

            exists = System.IO.Directory.Exists(buttonEdit1.Text + "\\Fabricas\\");
            if (!exists)
                System.IO.Directory.CreateDirectory(buttonEdit1.Text + "\\Fabricas\\");

            exists = System.IO.Directory.Exists(buttonEdit1.Text + "\\DALC\\");
            if (!exists)
                System.IO.Directory.CreateDirectory(buttonEdit1.Text + "\\DALC\\");

            exists = System.IO.Directory.Exists(buttonEdit1.Text + "\\Fachada\\");
            if (!exists)
                System.IO.Directory.CreateDirectory(buttonEdit1.Text + "\\Fachada\\");
            
            DataTable datTablas = cargarEsquemaBD();
            if (datTablas != null)
            {
                propiedadesFachada = "";
                foreach (DataRow dr in datTablas.Rows)
                {
                    if (checkEdit1.Checked) //Entidad
                    {
                        propiedades = "";
                        metodos = "";
                        enumCampos = "";

                        System.IO.StreamReader reader = new System.IO.StreamReader("estructuras\\Entidad.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        string NombreTabla = dr[2].ToString();
                        DataTable datCampos = cargarEsquemaTabla(NombreTabla);

                        archivo.Replace("[TABLE]", NombreTabla.Substring(2));
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);
                        if (datCampos != null)
                        {
                            foreach (DataColumn dc in datCampos.Columns)
                            {
                                propiedades += string.Format("private {0} m_{1};{2}", dc.DataType.Name, dc.ColumnName.ToLower(), Environment.NewLine);
                                metodos += string.Format("public {0} {1} {{ {2} get {{return m_{3};}} {4} set {{m_{5} = value;}} }}", dc.DataType.Name, dc.ColumnName, Environment.NewLine, dc.ColumnName.ToLower(), Environment.NewLine, dc.ColumnName.ToLower());
                                enumCampos += string.Format("{0},", dc.ColumnName.ToLower());
                            }
                        }

                        archivo.Replace("[PROPIEDADES]", propiedades);
                        archivo.Replace("[METODOS]", metodos);
                        archivo.Replace("[ENUMCAMPOS]", enumCampos);

                        System.IO.StreamWriter writer = new System.IO.StreamWriter(string.Format("{0}{1}cls{2}.cs", buttonEdit1.Text, "Entidades\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                    }

                    if (checkEdit2.Checked) //DALC
                    {
                        propiedades = "";
                        metodos = "";

                        System.IO.StreamReader reader = new System.IO.StreamReader("estructuras\\DALC.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        string NombreTabla = dr[2].ToString();
                        DataTable datCampos = cargarEsquemaTabla(NombreTabla);

                        archivo.Replace("[TABLE]", NombreTabla.Substring(2));
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);
                        if (datCampos != null)
                        {
                            foreach (DataColumn dc in datCampos.Columns)
                            {
                                propiedades += string.Format("private {0} m_{1};{2}", dc.DataType.Name, dc.ColumnName.ToLower(), Environment.NewLine);
                                metodos += string.Format("public {0} {1} {{ {2} get {{return m_{3};}} {4} set {{m_{5} = value;}} }}", dc.DataType.Name, dc.ColumnName, Environment.NewLine, dc.ColumnName.ToLower(), Environment.NewLine, dc.ColumnName.ToLower());
                            }
                        }

                        archivo.Replace("[PROPIEDADES]", propiedades);
                        archivo.Replace("[METODOS]", metodos);

                        System.IO.StreamWriter writer = new System.IO.StreamWriter(string.Format("{0}{1}cls{2}DALC.cs", buttonEdit1.Text, "DALC\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                    }

                    if (checkEdit4.Checked) //Fabrica
                    {
                        propiedades = "";

                        System.IO.StreamReader reader = new System.IO.StreamReader("estructuras\\Fabrica.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        string NombreTabla = dr[2].ToString();
                        DataTable datCampos = cargarEsquemaTabla(NombreTabla);

                        archivo.Replace("[TABLE]", NombreTabla.Substring(2));
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);
                        if (datCampos != null)
                        {
                            foreach (DataColumn dc in datCampos.Columns)
                            {
                                switch (dc.DataType.Name)
                                {
                                    case "Int16":
                                        propiedades += string.Format("obj.{0} = Convert.ToInt16(fila[\"{1}\"]);{2}", dc.ColumnName, dc.ColumnName, Environment.NewLine);
                                        break;
                                    case "Int32":
                                        propiedades += string.Format("obj.{0} = Convert.ToInt16(fila[\"{1}\"]);{2}", dc.ColumnName, dc.ColumnName, Environment.NewLine);
                                        break;
                                    case "Int64":
                                        propiedades += string.Format("obj.{0} = Convert.ToInt16(fila[\"{1}\"]);{2}", dc.ColumnName, dc.ColumnName, Environment.NewLine);
                                        break;
                                    case "String":
                                        propiedades += string.Format("obj.{0} = fila[\"{1}\"].ToString();{2}", dc.ColumnName, dc.ColumnName, Environment.NewLine);
                                        break;
                                    case "Boolean":
                                        propiedades += string.Format("obj.{0} = Convert.ToBoolean(fila[\"{1}\"]);{2}", dc.ColumnName, dc.ColumnName, Environment.NewLine);
                                        break;
                                }
                            }
                        }

                        archivo.Replace("[PROPIEDADES]", propiedades);

                        System.IO.StreamWriter writer = new System.IO.StreamWriter(string.Format("{0}{1}clsFabrica{2}.cs", buttonEdit1.Text, "Fabricas\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                    }

                    if (checkEdit3.Checked) //Fachada Hijas
                    {
                        metodos = "";

                        //archivo padre
                        System.IO.StreamReader reader = new System.IO.StreamReader("estructuras\\FachadaHija.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        string NombreTabla = dr[2].ToString();
                        propiedadesFachada += string.Format("private cls{0}DALC m_cls{1}DALC;{2}", NombreTabla.Substring(2), NombreTabla.Substring(2), Environment.NewLine);

                        //consultas
                        metodos = string.Format("#region {0}{1}", NombreTabla.Substring(2), Environment.NewLine);
                        metodos += string.Format("public cls{0} consultarEntidad{1}(SentenciaSQL sql)", NombreTabla.Substring(2), NombreTabla.Substring(2));
                        metodos += "{" + Environment.NewLine;
                        metodos += string.Format("m_cls{0}DALC = new cls{1}DALC(m_EjecutorBaseDatos);{2}", NombreTabla.Substring(2), NombreTabla.Substring(2), Environment.NewLine);
                        metodos += string.Format("return m_cls{0}DALC.Consultar(sql);{1}", NombreTabla.Substring(2), Environment.NewLine);
                        metodos += string.Format("}} {0}{1}", Environment.NewLine, Environment.NewLine);

                        metodos += string.Format("public DataTable consultarDatos{0}(SentenciaSQL sql)", NombreTabla.Substring(2));
                        metodos += "{" + Environment.NewLine;
                        metodos += string.Format("m_cls{0}DALC = new cls{1}DALC(m_EjecutorBaseDatos);{2}", NombreTabla.Substring(2), NombreTabla.Substring(2), Environment.NewLine);
                        metodos += string.Format("return m_cls{0}DALC.datatableConsultar(sql);{1}", NombreTabla.Substring(2), Environment.NewLine);
                        metodos += string.Format("}} {0}{1}", Environment.NewLine, Environment.NewLine);

                        metodos += string.Format("public List<cls{0}> consultarLista{1}(SentenciaSQL sql)", NombreTabla.Substring(2), NombreTabla.Substring(2));
                        metodos += "{" + Environment.NewLine;
                        metodos += string.Format("m_cls{0}DALC = new cls{1}DALC(m_EjecutorBaseDatos);{2}", NombreTabla.Substring(2), NombreTabla.Substring(2), Environment.NewLine);
                        metodos += string.Format("return m_cls{0}DALC.listConsultar(sql);{1}", NombreTabla.Substring(2), Environment.NewLine);
                        metodos += string.Format("}} {0}{1}", Environment.NewLine, Environment.NewLine);
                        metodos += "#endregion" + Environment.NewLine;

                        //insertar

                        //modificar

                        //eliminar

                        archivo.Replace("[METODOS]", metodos);
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);

                        System.IO.StreamWriter writer = new System.IO.StreamWriter(string.Format("{0}{1}clsFachada{2}.cs", buttonEdit1.Text, "Fachada\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();

                    }
                }

                
                

                if (checkEdit3.Checked) //Fachada Padre
                {
                    //archivo padre fachada
                    System.IO.StreamReader reader = new System.IO.StreamReader("estructuras\\Fachada.txt");
                    archivo = new StringBuilder(reader.ReadToEnd());
                    reader.Close();

                    archivo.Replace("[MIEMBROS]", propiedadesFachada);
                    archivo.Replace("[NAMESPACE]", textEdit1.Text);

                    System.IO.StreamWriter writer = new System.IO.StreamWriter(string.Format("{0}{1}clsFachada.cs", buttonEdit1.Text, "Fachada\\"), false, Encoding.Unicode);
                    writer.Write(archivo.ToString());
                    writer.Close();
                }

                MessageBox.Show("Finalizo");
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
                    OleDbCommand comando = new OleDbCommand("Select * from " + NombreTabla + " Where 0>1", conexion);
                    OleDbDataAdapter adaptador = new OleDbDataAdapter(comando);
                    tabla = new DataTable();
                    adaptador.Fill(tabla);
                    adaptador.FillSchema(tabla, SchemaType.Source);
                }
                return tabla;
            }
            catch (Exception)
            { }
            finally
            {
                conexion.Close();
            }

            return null;
        }
    }
}
