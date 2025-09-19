using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.IO;
using System.Configuration;
using r3Take.DataAccessLayer;

namespace r3Take.AMF
{
    public class AMFUtil
    {
        public static Object[] Tree(Hashtable h, HttpContext hc)
        {
            DAL dal = new DAL();
            DataTable dt = new DataTable();
            StringBuilder SB = new StringBuilder();
            int nr = 0;
            int level_act = -1;
            int level_old = -1;
            int level_dif = 0;
            Object[] myResult = new Object[3];
            myResult[0] = "TREE";
            myResult[1] = "mnu1";

            dt = dal.QueryDT("DS_r3", "SELECT ID_SEC,DESC_SEC, PARENT, [LEVEL], ORDEN, TOC, OP, DATA FROM V_MenuXUser WHERE ID_PORTAL = 1 AND account = '" + hc.Session["account"].ToString() + "' ORDER BY TOC ", "", h, hc);
            dt.Rows.RemoveAt(0);
            nr = dt.Rows.Count;

            SB.Append("<root>\n");
            for (int i = 0; i <= nr - 1; i++)
            {
                level_act = Int32.Parse(dt.Rows[i][3].ToString());
                if (level_act <= level_old)
                {
                    level_dif = (level_old - level_act) + 1;
                    for (int j = 0; j <= level_dif - 1; j++)
                    {
                        SB.Append("</item>\n");
                    }
                }
                level_old = level_act;
                SB.Append("<item label=\"").Append(dt.Rows[i][1]).Append("\" op=\"").Append(dt.Rows[i][6]).Append("\" data=\"").Append(dt.Rows[i][7]).Append("\" >\n ");
            }
            for (int j = 0; j <= level_old - 2; j++)
            {
                SB.Append("</item>\n");
            }
            SB.Append("</root>");
            myResult[2] = SB.ToString();
            SB = null;
            dt.Dispose();
            dt = null;
            dal = null;
            return myResult;
        }

        public static Object[] Tree(string dataSource, string sqlQuery, string args, int level, string myTree, Hashtable h, HttpContext hc)
        {
            DAL dal = new DAL();
            DataTable dt = new DataTable();
            StringBuilder SB = new StringBuilder();
            int nr = 0;
            int nc = 0;
            Boolean cerrar = false;
            String[] Levels = new String[level];
            String[] Tabs = new String[level];
            Object[] myResult = new Object[3];
            myResult[0] = "TREE";
            myResult[1] = myTree;

            dt = dal.QueryDT(dataSource, sqlQuery, args, h, hc);
            nr = dt.Rows.Count;
            nc = dt.Columns.Count;

            SB.Append("<root>\n");

            // Inicializar los niveles 
            for (int i = 0; i <= level - 1; i++)
            {
                Levels[i] = "";
                if (i > 0)
                {
                    Tabs[i] += Tabs[i - 1] + "\t";
                }
                else
                {
                    Tabs[0] = "\t";
                }
            }

            // Llenar los datos
            for (int i = 0; i <= nr - 1; i++)
            {
                // Abrir encabezados

                for (int j = 0; j <= level - 1; j += 5)
                {
                    if (Levels[j].CompareTo(dt.Rows[i][j].ToString()) != 0)
                    {
                        // Cerrar  levels
                        if (cerrar)
                        {
                            for (int k = (level - 1); k >= j; k += -5)
                            {
                                SB.Append(Tabs[k]).Append("</item>\n");
                            }
                            cerrar = false;
                        }
                        // Abrir level
                        SB.Append(Tabs[j]).Append("<item label=\"").Append(dt.Rows[i][j]).Append("\"").Append(" level=\"").Append(dt.Rows[i][j + 2]).Append("\"").Append(" icon=\"").Append(dt.Rows[i][j + 3]).Append("\"").Append(" idEmpresa=\"").Append(dt.Rows[i][j + 4]).Append("\"").Append(" value=\"").Append(dt.Rows[i][j + 1]).Append("\" >\n");
                        Levels[j] = dt.Rows[i][j].ToString();
                        for (int k = (j + 1); k <= level - 1; k++)
                        {
                            Levels[k] = "";
                        }
                    }
                }

                // Llenar datos
                SB.Append(Tabs[level - 1]).Append("\t <item ");
                SB.Append(" label=\"").Append(dt.Rows[i][level]).Append("\" ").Append(" level=\"").Append(dt.Rows[i][level + 2]).Append("\" ").Append(" icon=\"").Append(dt.Rows[i][level + 3]).Append("\" ").Append(" idEmpresa=\"").Append(dt.Rows[i][level + 4]).Append("\" ").Append(" value=\"").Append(dt.Rows[i][level + 1]).Append("\" ");
                SB.Append("/>\n");
                cerrar = true;
            }

            // Si no hay datos señalarlo
            if (nr == 0)
            {
                SB.Append("<item label=\"No hay datos disponibles\" >");
            }

            // Cerrar los niveles
            for (int j = level - 1; j >= 0; j += -5)
            {
                SB.Append(Tabs[j]).Append("</item>\n");
            }
            
            SB.Append("</root>");

            myResult[2] = SB.ToString();
            SB = null;
            dt.Dispose();
            dt = null;
            dal = null;
            return myResult;
        }

        public static Object[] TreeAll(string dataSource, string sqlQuery, string args, string myTree, Hashtable h, HttpContext hc)
        {
            DAL dal = new DAL();
            DataTable dt = new DataTable();
            StringBuilder SB = new StringBuilder();
            int nr = 0;
            int level_act = -1;
            int level_old = -1;
            int level_dif = 0;
            Object[] myResult = new Object[3];
            myResult[0] = "TREE";
            myResult[1] = myTree;

            dt = dal.QueryDT(dataSource, sqlQuery, args, h, hc);
            nr = dt.Rows.Count;
            SB.Append("<root>\n");
            for (int i = 0; i <= nr - 1; i++)
            {
                level_act = Int32.Parse(dt.Rows[i][2].ToString());
                if (level_act <= level_old)
                {
                    level_dif = (level_old - level_act) + 1;
                    for (int j = 0; j <= level_dif - 1; j++)
                    {
                        SB.Append("</item>\n");
                    }
                }
                level_old = level_act;
                SB.Append("<item label=\"").Append(dt.Rows[i][1]).Append("\" data =\"").Append(dt.Rows[i][0]).Append("\" >\n");
            }
            for (int j = 0; j <= level_old - 1; j++)
            {
                SB.Append("</item>\n");
            }
            SB.Append("</root>");
            myResult[2] = SB.ToString();
            SB = null;
            dt.Dispose();
            dt = null;
            dal = null;
            return myResult;
        }

        public static DataTable Query(string dataSource, string sqlQuery, string args, Hashtable h, HttpContext hc)
        {
            DAL dal = new DAL();
            DataTable dt = new DataTable();
            DataTable dtResult = new DataTable();
            int numCol = 0;

            dt = dal.QueryDT(dataSource, sqlQuery, args, h, hc);
            numCol = dt.Columns.Count;
            dtResult = dal.QueryDT("DS_r3", "SELECT '' AS Campo, '' AS Valor FROM AMF_Config where idAMFConfig=332", "", h, hc);
            for (int i = 0; i <= (numCol) - 1; i++)
            {
                dtResult.Rows.Add(dt.Columns[i].ColumnName.ToString(), dt.Rows[0][i].ToString());
            }
            dt.Dispose();
            return dtResult;
        }

        public static Object[] ToForm(string dataSource, string sqlQuery, string args, Hashtable h, HttpContext hc)
        {
            Object[] myResult = new Object[2];
            myResult[0] = "TO_FORM";
            //try añadido en caso de que el query no regrese al menos una fila 
            try
            {
                myResult[1] = Query(dataSource, sqlQuery, args, h, hc);
            }
            catch (Exception exc)
            {
                ReturnError(("1. " + exc.Message + "\n2. ") + exc.InnerException.Message);
            }
            return myResult;
        }

        public static Object[] ToGrid(string dataSource, string sqlQuery, string args, string myGrid, Hashtable h, HttpContext hc)
        {
            Object[] myResult = new Object[3];
            DAL dal = new DAL();

            myResult[0] = "DG_DATA";
            myResult[1] = myGrid;
            myResult[2] = dal.QueryDT(dataSource, sqlQuery, args, h, hc);
            return myResult;
        }
        public static Object[] ToID(string dataSource, string sqlQuery, string args, string myGrid, Hashtable h, HttpContext hc)
        {
            Object[] myResult = new Object[3];
            DAL dal = new DAL();

            myResult[0] = "ID_DATA";
            myResult[1] = dal.QueryDT(dataSource, sqlQuery, args, h, hc);
            return myResult;
        }

        public static Object[] ToGI(string dataSource, string sqlQuery, string args, string myGrid, Hashtable h, HttpContext hc)
        {
            Object[] myResult = new Object[3];
            DAL dal = new DAL();

            myResult[0] = "GI_DATA";
            myResult[1] = "containerImages";

            string[] completePathImage = null;
            DataTable dtFileImage = new DataTable();
            dtFileImage.Columns.Add("nameImage");


            completePathImage = Directory.GetFiles(ConfigurationManager.AppSettings["RutaRepositorioCompartido"] + "Images\\\\iconDesktop");

            Int32 numFiles = default(Int32);
            numFiles = completePathImage.Length;
            int i = 0;
            for (i = 0; i <= numFiles - 2; i++)
            {
                dtFileImage.Rows.Add(System.IO.Path.GetFileName(completePathImage[i]));
            }


            myResult[2] = dtFileImage;
            return myResult;
        }

        public static Object[] ToFD(string dataSource, string sqlQuery, string args, string myGrid, Hashtable h, HttpContext hc)
        {
            Object[] myResult = new Object[3];
            DAL dal = new DAL();

            myResult[0] = "FD_DATA";
            myResult[1] = dal.QueryDT(dataSource, sqlQuery, args, h, hc);
            return myResult;
        }


        public static Object[] ToCB(string dataSource, string sqlQuery, string args, string myCB, Hashtable h, HttpContext hc)
        {
            Object[] myResult = new Object[3];
            DAL dal = new DAL();

            myResult[0] = "CB_DATA";
            myResult[1] = myCB;
            myResult[2] = dal.QueryDT(dataSource, sqlQuery, args, h, hc);
            return myResult;
        }

        public static Object[] SQL_Exec(string dataSource, string sqlQuery, string args, string msg, Hashtable h, HttpContext hc)
        {
            Object[] myResult = new Object[3];
            DAL dal = new DAL();
            try
            {
                dal.ExecuteNonQuery(dataSource, sqlQuery, args, h, hc);
                if (msg.Length > 0 && msg.CompareTo("NULL") != 0)
                {
                    myResult = ReturnMsg(msg);
                }
                else
                {
                    myResult = ReturnNull();
                }
            }
            catch (Exception exc)
            {
                myResult = ReturnError(exc.Message);
            }
            return myResult;
        }


        public static Object[] ReturnForm(String handler, String form)
        {
            Object[] myResult = new Object[3];
            myResult[0] = "LOAD_FORM";
            myResult[1] = handler;
            myResult[2] = form;
            return myResult;
        }

        public static Object[] ReturnPathReport(String path, String myFrame)
        {
            Object[] myResult = new Object[3];
            myResult[0] = "PATH_REPORT";
            myResult[1] = path;
            myResult[2] = myFrame;
            return myResult;
        }

        public static Object[] ReturnPathExcel(String path)
        {
            Object[] myResult = new Object[2];
            myResult[0] = "PATH_EXCEL";
            myResult[1] = path;
            return myResult;
        }
        
        public static Object[] ReturnModule(String module)
        {
            Object[] myResult = new Object[2];
            myResult[0] = "MODULE";
            myResult[1] = module;
            return myResult;
        }

        public static Object[] ReturnError(String error)
        {
            Object[] myResult = new Object[2];
            myResult[0] = "ERROR";
            myResult[1] = error;
            return myResult;
        }

        public static Object[] ReturnJS(String myFunction, String myParams)
        {
            Object[] myResult = new Object[3];
            myResult[0] = "JS";
            myResult[1] = myFunction;
            myResult[2] = myParams;
            return myResult;
        }
        
        public static Object[] ReturnNull()
        {
            Object[] myResult = new Object[1];
            myResult[0] = "NULL";
            return myResult;
        }

        public static Object[] ReturnMsg(String msg)
        {
            Object[] myResult = new Object[2];
            myResult[0] = "MSG";
            myResult[1] = msg;
            return myResult;
        }
    }
}

