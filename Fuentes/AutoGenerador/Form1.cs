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
            string campos;
            string entidad;
            string entidadCombo;

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

            exists = System.IO.Directory.Exists(buttonEdit1.Text + "\\Controles\\");
            if (!exists)
                System.IO.Directory.CreateDirectory(buttonEdit1.Text + "\\Controles\\");

            exists = System.IO.Directory.Exists(buttonEdit1.Text + "\\Paginas\\");
            if (!exists)
                System.IO.Directory.CreateDirectory(buttonEdit1.Text + "\\Paginas\\");

            DataTable datTablas = cargarEsquemaBD();
            if (datTablas != null)
            {
                propiedadesFachada = "";
                foreach (DataRow dr in datTablas.Rows)
                {
                    #region Entidad
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
                            string nulo;
                            foreach (DataColumn dc in datCampos.Columns)
                            {
                                nulo = "";
                                if (dc.DataType.Name.IndexOf("Int") >= 0 || dc.DataType.Name.IndexOf("Byte") >= 0)
                                    nulo = "?";
                                propiedades += string.Format("private {0}{3} m_{1};{2}", dc.DataType.Name, dc.ColumnName.ToLower(), Environment.NewLine, nulo);
                                metodos += string.Format("public {0}{6} {1} {{ {2} get {{return m_{3};}} {4} set {{m_{5} = value;}} }}", dc.DataType.Name, dc.ColumnName, Environment.NewLine, dc.ColumnName.ToLower(), Environment.NewLine, dc.ColumnName.ToLower(), nulo);
                                enumCampos += string.Format("{0},", dc.ColumnName);
                            }
                        }

                        archivo.Replace("[PROPIEDADES]", propiedades);
                        archivo.Replace("[METODOS]", metodos);
                        archivo.Replace("[ENUMCAMPOS]", enumCampos);

                        System.IO.StreamWriter writer = new System.IO.StreamWriter(string.Format("{0}{1}cls{2}.cs", buttonEdit1.Text, "Entidades\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                    }
                    #endregion

                    #region DALC
                    if (checkEdit2.Checked) //DALC
                    {
                        propiedades = "";
                        metodos = "";
                        campos = "";

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
                                campos += "," + dc.ColumnName;
                                propiedades += string.Format("private {0} m_{1};{2}", dc.DataType.Name, dc.ColumnName.ToLower(), Environment.NewLine);
                                metodos += string.Format("public {0} {1} {{ {2} get {{return m_{3};}} {4} set {{m_{5} = value;}} }}", dc.DataType.Name, dc.ColumnName, Environment.NewLine, dc.ColumnName.ToLower(), Environment.NewLine, dc.ColumnName.ToLower());
                            }

                            campos = campos.Substring(1);
                        }

                        archivo.Replace("[PROPIEDADES]", propiedades);
                        archivo.Replace("[METODOS]", metodos);
                        archivo.Replace("[CAMPOS]", campos);

                        System.IO.StreamWriter writer = new System.IO.StreamWriter(string.Format("{0}{1}cls{2}DALC.cs", buttonEdit1.Text, "DALC\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                    }
                    #endregion

                    #region Fabrica
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
                                    case "String":
                                        propiedades += string.Format("obj.{0} = fila[\"{1}\"].ToString();{2}", dc.ColumnName, dc.ColumnName, Environment.NewLine);
                                        break;
                                    default:
                                        propiedades += string.Format("obj.{0} = Convert.To{1}(fila[\"{0}\"]);{2}", dc.ColumnName, dc.DataType.Name, Environment.NewLine);
                                        break;
                                }
                            }
                        }

                        archivo.Replace("[PROPIEDADES]", propiedades);

                        System.IO.StreamWriter writer = new System.IO.StreamWriter(string.Format("{0}{1}clsFabrica{2}.cs", buttonEdit1.Text, "Fabricas\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                    }
                    #endregion

                    #region Fachada Hija
                    if (checkEdit3.Checked) //Fachada Hija
                    {
                        metodos = "";

                        //archivo hija fachada
                        System.IO.StreamReader reader = new System.IO.StreamReader("estructuras\\FachadaHija.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        string NombreTabla = dr[2].ToString();
                        propiedadesFachada += string.Format("private cls{0}DALC m_cls{1}DALC;{2}", NombreTabla.Substring(2), NombreTabla.Substring(2), Environment.NewLine);

                        //consultas
                        //metodos = string.Format("#region {0}{1}", NombreTabla.Substring(2), Environment.NewLine);
                        //metodos += string.Format("public cls{0} consultarEntidad{1}(SentenciaSQL sql)", NombreTabla.Substring(2), NombreTabla.Substring(2));
                        //metodos += "{" + Environment.NewLine;
                        //metodos += string.Format("m_cls{0}DALC = new cls{1}DALC(m_EjecutorBaseDatos);{2}", NombreTabla.Substring(2), NombreTabla.Substring(2), Environment.NewLine);
                        //metodos += string.Format("return m_cls{0}DALC.Consultar(sql);{1}", NombreTabla.Substring(2), Environment.NewLine);
                        //metodos += string.Format("}} {0}{1}", Environment.NewLine, Environment.NewLine);

                        //metodos += string.Format("public DataTable consultarDatos{0}(SentenciaSQL sql)", NombreTabla.Substring(2));
                        //metodos += "{" + Environment.NewLine;
                        //metodos += string.Format("m_cls{0}DALC = new cls{1}DALC(m_EjecutorBaseDatos);{2}", NombreTabla.Substring(2), NombreTabla.Substring(2), Environment.NewLine);
                        //metodos += string.Format("return m_cls{0}DALC.datatableConsultar(sql);{1}", NombreTabla.Substring(2), Environment.NewLine);
                        //metodos += string.Format("}} {0}{1}", Environment.NewLine, Environment.NewLine);

                        //metodos += string.Format("public List<cls{0}> consultarLista{1}(SentenciaSQL sql)", NombreTabla.Substring(2), NombreTabla.Substring(2));
                        //metodos += "{" + Environment.NewLine;
                        //metodos += string.Format("m_cls{0}DALC = new cls{1}DALC(m_EjecutorBaseDatos);{2}", NombreTabla.Substring(2), NombreTabla.Substring(2), Environment.NewLine);
                        //metodos += string.Format("return m_cls{0}DALC.listConsultar(sql);{1}", NombreTabla.Substring(2), Environment.NewLine);
                        //metodos += string.Format("}} {0}{1}", Environment.NewLine, Environment.NewLine);                        
                        //metodos += "#endregion" + Environment.NewLine;

                        archivo.Replace("[TABLE]", NombreTabla.Substring(2));
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);

                        System.IO.StreamWriter writer = new System.IO.StreamWriter(string.Format("{0}{1}clsFachada{2}.cs", buttonEdit1.Text, "Fachada\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();

                    }
                    #endregion

                    #region Controles
                    if (checkEdit5.Checked) //controles form
                    {
                        propiedades = "";
                        entidad = "";
                        entidadCombo = "";

                        #region Pagina
                        System.IO.StreamReader reader = new System.IO.StreamReader("estructuras\\ctrForm_Pagina.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        string NombreTabla = dr[2].ToString();
                        DataTable datCampos = cargarEsquemaTabla(NombreTabla);
                        if (datCampos != null)
                        {
                            foreach (DataColumn dc in datCampos.Columns)
                            {
                                string entidadTemporal = "";
                                if (!dc.ReadOnly)
                                    entidadTemporal = string.Format("{0}.{1} = txt{1}.Text;{2}", NombreTabla.Substring(2), dc.ColumnName, Environment.NewLine);

                                string componente = string.Format("<dx:ASPxTextBox ID=\"txt{0}\" runat=\"server\" CssClass=\"TextBox\">{1}</dx:ASPxTextBox>{1}", dc.ColumnName, Environment.NewLine);

                                if (dc.DataType.Name.IndexOf("Boolean") >= 0)
                                {
                                    componente = string.Format("<dx:ASPxCheckBox ID=\"ck{0}\" runat=\"server\" CheckState=\"Unchecked\" CssClass=\"CheckBox\">{1}</dx:ASPxCheckBox>", dc.ColumnName, Environment.NewLine);
                                    entidadTemporal = string.Format("{0}.{1} = ck{1}.Checked ? true : false;{2}", NombreTabla.Substring(2), dc.ColumnName, Environment.NewLine);
                                }

                                if (dc.DataType.Name.IndexOf("Int") >= 0 || dc.DataType.Name.IndexOf("Decimal") >= 0 || dc.DataType.Name.IndexOf("Double") >= 0)
                                {
                                    if (dc.ColumnName != "Id")
                                        entidadTemporal = string.Format("{0} valor{1} = 0;{3}{0}.TryParse(txt{1}.Text, out valor{1});{3}{2}.{1} = valor{1};{3}", dc.DataType.Name, dc.ColumnName, NombreTabla.Substring(2), Environment.NewLine);
                                    componente = string.Format("<dx:ASPxTextBox ID=\"txt{0}\" runat=\"server\" CssClass=\"TextBox\" DisplayFormatString=\"N0\">{1}<MaskSettings Mask=\"&lt;0..10000000&gt;\"/>{1}</dx:ASPxTextBox>{1}", dc.ColumnName, Environment.NewLine);
                                }

                                if (dc.ColumnName == "Id")
                                    componente = string.Format("<dx:ASPxLabel ID=\"lb{0}\" runat=\"server\" Text=\"0\">{1}</dx:ASPxLabel>{1}", dc.ColumnName, Environment.NewLine);

                                if (dc.ColumnName.Substring(0, 2) == "id" || dc.ColumnName == "Estado")
                                {
                                    componente = string.Format("<dx:ASPxComboBox ID=\"cb{0}\" runat=\"server\" CssClass=\"ComboBox\">{1}</dx:ASPxComboBox>", dc.ColumnName, Environment.NewLine);
                                    entidadTemporal = string.Format("if (cb{0}.Value != null){1}{2}.{0} = Convert.To{3}(cb{0}.Value);{1}", dc.ColumnName, Environment.NewLine, NombreTabla.Substring(2), dc.DataType.Name);
                                }

                                if (dc.ColumnName.Substring(0, 2) == "id") // llenar combos
                                {
                                    entidadCombo += string.Format("cbid{0}.DataSource = fachadaCore.consultarDatos{0}(sql);{1}", dc.ColumnName.Substring(2), Environment.NewLine);
                                    entidadCombo += string.Format("cbid{0}.ValueField = \"Id\";{1}", dc.ColumnName.Substring(2), Environment.NewLine);
                                    entidadCombo += string.Format("cbid{0}.TextField = \"Nombre\";{1}", dc.ColumnName.Substring(2), Environment.NewLine);
                                    entidadCombo += string.Format("cbid{0}.DataBind();{1}", dc.ColumnName.Substring(2), Environment.NewLine);
                                }

                                propiedades += string.Format("<dx:LayoutItem Caption=\"{0}\" FieldName=\"{0}\">{1}", dc.ColumnName, Environment.NewLine);
                                propiedades += string.Format("<LayoutItemNestedControlCollection>{0}", Environment.NewLine);
                                propiedades += string.Format("<dx:LayoutItemNestedControlContainer runat=\"server\">{0}", Environment.NewLine);
                                propiedades += componente;
                                propiedades += string.Format("</dx:LayoutItemNestedControlContainer>{0}</LayoutItemNestedControlCollection>{0}</dx:LayoutItem>{0}", Environment.NewLine);

                                entidad += entidadTemporal;
                            }
                        }

                        archivo.Replace("[TABLE]", NombreTabla.Substring(2));
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);
                        archivo.Replace("[PROPIEDADES]", propiedades);

                        System.IO.StreamWriter writer = new System.IO.StreamWriter(string.Format("{0}{1}ctr{2}Form.ascx", buttonEdit1.Text, "Controles\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                        #endregion

                        #region Codigo
                        reader = new System.IO.StreamReader("estructuras\\ctrForm_Codigo.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        archivo.Replace("[TABLE]", NombreTabla.Substring(2));
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);
                        archivo.Replace("[ENTIDAD]", entidad);
                        archivo.Replace("[ENTIDADCOMBO]", entidadCombo);

                        writer = new System.IO.StreamWriter(string.Format("{0}{1}ctr{2}Form.ascx.cs", buttonEdit1.Text, "Controles\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                        #endregion

                        #region Diseño
                        reader = new System.IO.StreamReader("estructuras\\ctrForm_Diseno.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        archivo.Replace("[TABLE]", NombreTabla.Substring(2));
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);

                        writer = new System.IO.StreamWriter(string.Format("{0}{1}ctr{2}Form.ascx.designer.cs", buttonEdit1.Text, "Controles\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                        #endregion

                    }

                    if (checkEdit6.Checked) //controles list
                    {
                        propiedades = "";
                        entidad = "";

                        #region Pagina
                        System.IO.StreamReader reader = new System.IO.StreamReader("estructuras\\ctrList_Pagina.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        string NombreTabla = dr[2].ToString();
                        DataTable datCampos = cargarEsquemaTabla(NombreTabla);
                        if (datCampos != null)
                        {
                            foreach (DataColumn dc in datCampos.Columns)
                            {
                                string componente = string.Format("<dx:GridViewDataTextColumn FieldName=\"{0}\" Caption=\"{0}\">{1}</dx:GridViewDataTextColumn>", dc.ColumnName, Environment.NewLine);

                                if (dc.DataType.Name.IndexOf("Boolean") >= 0)
                                {
                                    componente = string.Format("<dx:GridViewDataCheckColumn Caption=\"{0}\" FieldName=\"{0}\" UnboundType=\"Boolean\">{1}</dx:GridViewDataCheckColumn>", dc.ColumnName, Environment.NewLine);
                                }
                                propiedades += componente + Environment.NewLine;
                            }
                        }

                        archivo.Replace("[TABLE]", NombreTabla.Substring(2));
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);
                        archivo.Replace("[PROPIEDADES]", propiedades);

                        System.IO.StreamWriter writer = new System.IO.StreamWriter(string.Format("{0}{1}ctr{2}List.ascx", buttonEdit1.Text, "Controles\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                        #endregion

                        #region Codigo
                        reader = new System.IO.StreamReader("estructuras\\ctrList_Codigo.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        archivo.Replace("[TABLE]", NombreTabla.Substring(2));
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);

                        writer = new System.IO.StreamWriter(string.Format("{0}{1}ctr{2}List.ascx.cs", buttonEdit1.Text, "Controles\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                        #endregion

                        #region Diseño
                        reader = new System.IO.StreamReader("estructuras\\ctrList_Diseno.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        archivo.Replace("[TABLE]", NombreTabla.Substring(2));
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);

                        writer = new System.IO.StreamWriter(string.Format("{0}{1}ctr{2}List.ascx.designer.cs", buttonEdit1.Text, "Controles\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                        #endregion

                    }
                    #endregion

                    #region Formularios
                    if (checkEdit7.Checked) //List 
                    {
                        propiedades = "";
                        metodos = "";

                        #region Pagina
                        System.IO.StreamReader reader = new System.IO.StreamReader("estructuras\\frm_Pagina.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        string NombreTabla = dr[2].ToString();
                        propiedades = string.Format("<dx:ASPxButton ID=\"btCrear\" runat=\"server\" Text= \"Nuevo\" OnClick=\"btCrear_Click\"></dx:ASPxButton>{0}", Environment.NewLine);

                        archivo.Replace("[TABLE]", NombreTabla.Substring(2));
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);
                        archivo.Replace("[TIPO]", "List");
                        archivo.Replace("[PROPIEDADES]", propiedades);

                        System.IO.StreamWriter writer = new System.IO.StreamWriter(string.Format("{0}{1}frm{2}List.aspx", buttonEdit1.Text, "Paginas\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                        #endregion

                        #region Diseño
                        reader = new System.IO.StreamReader("estructuras\\frm_Diseno.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        archivo.Replace("[TABLE]", NombreTabla.Substring(2));
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);
                        archivo.Replace("[TIPO]", "List");

                        writer = new System.IO.StreamWriter(string.Format("{0}{1}frm{2}List.aspx.designer.cs", buttonEdit1.Text, "Paginas\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                        #endregion

                        #region Codigo
                        reader = new System.IO.StreamReader("estructuras\\frm_Codigo.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        metodos += "protected void btCrear_Click(object sender, EventArgs e)" + Environment.NewLine;
                        metodos += "{";
                        metodos += string.Format("{0}Response.Redirect(\"frm{1}Form.aspx\");{0}", Environment.NewLine, NombreTabla.Substring(2));
                        metodos += "}" + Environment.NewLine;

                        archivo.Replace("[TABLE]", NombreTabla.Substring(2));
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);
                        archivo.Replace("[TIPO]", "List");
                        archivo.Replace("[METODOS]", metodos);

                        writer = new System.IO.StreamWriter(string.Format("{0}{1}frm{2}List.aspx.cs", buttonEdit1.Text, "Paginas\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                        #endregion
                    }

                    if (checkEdit8.Checked) //Form 
                    {
                        propiedades = "";
                        metodos = "";

                        #region Pagina
                        System.IO.StreamReader reader = new System.IO.StreamReader("estructuras\\frm_Pagina.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        string NombreTabla = dr[2].ToString();
                        propiedades = "";

                        archivo.Replace("[TABLE]", NombreTabla.Substring(2));
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);
                        archivo.Replace("[TIPO]", "Form");
                        archivo.Replace("[PROPIEDADES]", propiedades);

                        System.IO.StreamWriter writer = new System.IO.StreamWriter(string.Format("{0}{1}frm{2}Form.aspx", buttonEdit1.Text, "Paginas\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                        #endregion

                        #region Diseño
                        reader = new System.IO.StreamReader("estructuras\\frm_Diseno.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        archivo.Replace("[TABLE]", NombreTabla.Substring(2));
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);
                        archivo.Replace("[TIPO]", "Form");

                        writer = new System.IO.StreamWriter(string.Format("{0}{1}frm{2}Form.aspx.designer.cs", buttonEdit1.Text, "Paginas\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                        #endregion

                        #region Codigo
                        reader = new System.IO.StreamReader("estructuras\\frm_Codigo.txt");
                        archivo = new StringBuilder(reader.ReadToEnd());
                        reader.Close();

                        archivo.Replace("[TABLE]", NombreTabla.Substring(2));
                        archivo.Replace("[NAMESPACE]", textEdit1.Text);
                        archivo.Replace("[TIPO]", "Form");
                        archivo.Replace("[METODOS]", metodos);

                        writer = new System.IO.StreamWriter(string.Format("{0}{1}frm{2}Form.aspx.cs", buttonEdit1.Text, "Paginas\\", NombreTabla.Substring(2)), false, Encoding.Unicode);
                        writer.Write(archivo.ToString());
                        writer.Close();
                        #endregion
                    }
                    #endregion
                }

                #region Fachada Padre
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
                #endregion

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
