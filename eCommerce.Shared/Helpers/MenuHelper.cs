using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Shared.Helpers
{
    public class MenuHelper
    {
        public SqlConnection Con;
        //public string ConStr = "Server=103.92.45.62;Database=LocalDb;User Id=etpl;Password=e1!t2@p3#l";
        public string ConStr = "Server=198.12.255.161;Database=LocalDb;User Id=etpl;Password=etpl#3339";
        DataTable dtMnu;
        public MenuHelper()
        {
            Con = new SqlConnection(@ConStr);
            if (Con.State != ConnectionState.Open)
            {
                Con.Open();
            }
        }
        publi static DataTable GetTable(string sqlStr)
        {
            if (Con.State == ConnectionState.Closed)
            {
                Con.Open();
            }
            DataSet datSet = new DataSet();
            SqlDataAdapter datAdp = new SqlDataAdapter();
            DataTable dtTb = new DataTable();
            datAdp = new SqlDataAdapter(sqlStr, Con);
            datAdp.Fill(datSet);
            dtTb = datSet.Tables[0];
            return dtTb;
        }
        public string CrtMenu()
        {
            string mnuStr = "";
            dtMnu = GetTable("SELECT CTG_CODE CTCD,CTG_NAME CTNM,CTG_TYPE CTTP,ISNULL(CTG_UCTG,0) UCTG,ISNULL(CTG_SURL,'') SURL FROM CATG_MST");
            DataTable dt = new DataTable();
            dt = dtMnu.Select("CTTP='G'").CopyToDataTable();
            foreach (DataRow dr in dt.Rows)
            {
                mnuStr += "<div class='nav-depart'>";
                mnuStr+= "<div class='depart-btn'>";
                mnuStr += "<span>" + dr["CTNM"] + "</span>";
                mnuStr += "<ul class='depart-hover'>";
                mnuStr += GetGrp(dr["CTCD"].ToString());
                mnuStr += "</ul></div></div>";
            }
            return mnuStr;
        }
        public string GetGrp(string gpcd)
        {
            string getStr = "";
            DataTable dt = new DataTable();
            DataTable dtr = new DataTable();
            DataView dv;
            if (dtMnu.Select("UCTG=" + gpcd).Length > 0)
            {
                dt = dtMnu.Select("UCTG=" + gpcd).CopyToDataTable();
                dv = new DataView(dt);
                dv.Sort = "CTNM";
                dt = dv.ToTable();
                foreach (DataRow dr in dt.Rows)
                {
                    if (dtMnu.Select("UCTG=" + dr["CTCD"].ToString()).Length == 0)
                    {
                        getStr += GetMnu(dr["CTNM"].ToString(), dr["SURL"].ToString());
                    }
                    else
                    {
                        dtr = dtMnu.Select("UCTG=" + dr["CTCD"].ToString()).CopyToDataTable();
                        dv = new DataView(dtr);
                        dv.Sort = "CTNM";
                        dtr = dv.ToTable();
                        getStr += "<li><a href='" + dr["SURL"].ToString() + "'>";
                        getStr += dr["CTNM"].ToString() + "<i class='fas fa-angle-double-right ml-1 text-black-50'></i>";
                        getStr += "</a>";
                        getStr += "<ul class='sub-menu'>";
                        foreach (DataRow dr1 in dtr.Rows)
                        {
                            getStr += GetMnu(dr1["CTNM"].ToString(), dr1["SURL"].ToString());
                        }
                        getStr += "</ul></li>";
                    }
                }
            }
            return getStr;
        }
        public string GetMnu(string mnnm, string surl)
        {
            string getStr;
            getStr= "<li><a href='" + surl + "'>" + mnnm + "</a></li>";
            return getStr;
        }
    }
}
