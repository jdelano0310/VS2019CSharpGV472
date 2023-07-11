using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace VS2019CSharpGV472
{
    public partial class _Default : Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack) {
                // put some data in the existing grid view
                DataTable mytable = new DataTable();
                mytable.Columns.Add("FirstName", typeof(string));
                mytable.Columns.Add("LastName", typeof(string));

                DataRow dr = mytable.NewRow();
                dr["FirstName"] = "Person";
                dr["LastName"] = "Number1";
                mytable.Rows.Add(dr);

                dr = mytable.NewRow();
                dr["FirstName"] = "Person";
                dr["LastName"] = "Number2";
                mytable.Rows.Add(dr);

                Session["grid1Table"] = mytable;
                gv1.DataSource = mytable;
                gv1.DataBind();

            } else
            {
                if (Request.Form["__EVENTTARGET"] != null)
				{
                    if (Request.Form["__EVENTTARGET"].IndexOf("btnAddRow") == -1)
					{
                        // check for the other grid
                        if (Session["grid2Table"] != null)
                        {
                            ReDisplayGridView("grid2Table");
                        }
                    }
                }
                 
            }
        }

        protected void ReDisplayGridView(string gvName)
        {
            GridView gv;
            gv = new GridView();

            // reassign the datatable it used
            // putting the object in front casts the session value as that object
            DataTable mytable = (DataTable)Session["grid2Table"];
                       
            gv.DataSource = mytable;
            gv.DataBind();

            upGridViews.ContentTemplateContainer.Controls.Add(gv);
            upGridViews.Update();
        }
        protected void btnAddGrid_Click(object sender, EventArgs e)
        {
            // create a new gridview programmatically
            GridView gv;
            gv = new GridView();
            gv.ID = "gv2";

            // it needs a datatable
            DataTable mytable = new DataTable();
            mytable.Columns.Add("FirstName", typeof(string));
            mytable.Columns.Add("LastName", typeof(string));

            DataRow dr = mytable.NewRow();
            dr["FirstName"] = "Person";
            dr["LastName"] = "Number3";
            mytable.Rows.Add(dr);

            dr = mytable.NewRow();
            dr["FirstName"] = "Person";
            dr["LastName"] = "Number4";
            mytable.Rows.Add(dr);

            Session["grid2Table"] = mytable;
            gv.DataSource = mytable;
            gv.DataBind();

            upGridViews.ContentTemplateContainer.Controls.Add(gv);
            upGridViews.Update();
            Session["gv2"] = gv;

            lbCurrentGrid.Text = "Current: Second Grid";
        }

        protected void btnAddRow_Click(object sender, EventArgs e)
        {
            GridView gvTemp;
            DataTable mytable;

            // putting the object in front casts the session value as that object
            if (lbCurrentGrid.Text.IndexOf("First") > 0)
            {
                // the original grid on the page
                mytable = (DataTable)Session["grid1Table"];
                gvTemp = gv1;
            } else
            {
                // the added grid on the page
                gvTemp = new GridView();  
                mytable = (DataTable)Session["grid2Table"];
            }

            DataRow dr = mytable.NewRow();
            dr["FirstName"] = "Person";
            dr["LastName"] = "Number6";
            mytable.Rows.Add(dr);

            gvTemp.DataSource = mytable;
            gvTemp.DataBind();

            // if this isn't adding a row to the inital grid then add the grid back to the
            // page
            if (lbCurrentGrid.Text.IndexOf("First") == -1) { 
                upGridViews.ContentTemplateContainer.Controls.Add(gvTemp);
            }

            upGridViews.Update();
        }
    }
}