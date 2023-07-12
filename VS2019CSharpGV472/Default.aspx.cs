using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

namespace VS2019CSharpGV472
{
    public partial class _Default : Page
    {
        // hold the types so that a trip to the db isn't required on each new row
        private string[] _products;
        private string[,] _categories;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack) {

                GetProducts();
                GetCategories();

                Session["GridViewsCount"] = 1;
                
                FillProductDropDown(this.ddlProducts1);
                AddRowToDatatableForGrid("1");

                // save the arrays in a session var
                Session["Products"] = _products;
                Session["Categories"] = _categories;

            }
            else
            {
                if (Request.Form["__EVENTTARGET"] != null)
				{
                    if (Request.Form["__EVENTTARGET"].IndexOf("btnAddRow") == -1)
					{
                        // if the user isn't adding a row simply redisplay any added grids
                        if (Convert.ToInt32(Session["gridCount"]) >  0)
                        {
                            ReDisplayGridViews();
                        }
                    }
                }
                 
            }
        }

        protected void FillProductDropDown(DropDownList ddlToFill)
		{
            // use the saved array to fill the dropdownlist passed
            int itemCount = 0;

            ddlToFill.Items.Insert(itemCount, "Please select a Product");
            
            if (Session["Products"] != null) {
                _products = (string[])Session["Products"];
            }

            foreach (string item in _products)
            {
                itemCount += 1;
                if (item != null) { ddlToFill.Items.Insert(itemCount -1, item); }
            }
        }

        protected void FillCategoryDropDown(DropDownList ddlToFill, int ProductID)
        {
            // use the saved array to fill the dropdownlist passed
            ddlToFill.Items.Insert(0, "Select a Cateorgy");

            if (Session["Categories"] != null)
            {
                _categories = (string[,])Session["Categories"];
            }

            // for the certain productid
            for (int p = ProductID; p <= ProductID; p++)
            {
                for (int c = 1; c <= _categories.GetUpperBound(1); c++)
				{
                    ddlToFill.Items.Insert(c, _categories[p, c]);
                }
            }

        }
        protected void GetCategories()
		{
            // grab the dropdown contents for the categories dropdown
            // limits the back and fourth to the database
            using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=GridTest;Integrated Security=True;"))
            {
                conn.Open();

                // find the maximum number of categories a product has in the categories table
                SqlCommand cmd = new SqlCommand("select max(NumberOfCategories) from (select count(id) as NumberOfCategories from tblCategories group by ProductID) as tbl1;", conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                int maxNumberOfCategories = dr.GetInt32(0);
                dr.Close();

                // get the catergoies from the table
                cmd = new SqlCommand("Select * from tblCategories Order by ProductID", conn);
                dr = cmd.ExecuteReader();

                int numProducts = _products.Length;

                // scope the array variable that holds the Travel Types for the dropdown
                _categories = new string[numProducts, maxNumberOfCategories + 1];

                int categoryByProductIndex = 1;
                int currentProductID = 0;

                // fill the array with the categories
                while (dr.Read())
				{
                    if (currentProductID != dr.GetInt32(1))
					    { categoryByProductIndex = 1;
                        currentProductID = dr.GetInt32(1);} 
                    else { categoryByProductIndex += 1; }

                    _categories[dr.GetInt32(1), categoryByProductIndex] = dr.GetString(2);
                }

            }
        }

        protected void GetProducts()
        {
            // grab the dropdown contents for the products dropdown
            using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=GridTest;Integrated Security=True;"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select * from tblProducts Order by ID", conn);
                SqlDataReader dr = cmd.ExecuteReader();

                // to get the number of rows the reader has, load it into a table
                DataTable dt = new DataTable();
                dt.Load(dr);
                int numRows = dt.Rows.Count;

                // scope the array variable that holds the Products for the dropdown
                _products = new string[numRows + 1];

                // fill the array with the products
                foreach (DataRow row in dt.Rows)
                {
                    // use the id as the array index
                    _products[row.Field<int>("ID")] = row.Field<string>("Product");
                }
            }
        }


        protected void AddRowToDatatableForGrid(string GridNumber)
		{
            // each GridView needs a DataTable where the new row is actually added
            // these are in session memory and can be saved to a table at any time
            DataTable mytable;
            DataRow dr;
            //GridView gv = (GridView)upGridViews.FindControl($"gv{GridNumber}");
            GridView gv = (GridView)divGridSection.FindControl($"gv{GridNumber}");

            string gridDataName = $"Grid{GridNumber}Datatable";

            if (Session[gridDataName] == null)
            {
                // there isn't an associated datatable with the grid, create a new one
                mytable = new DataTable();
                mytable.TableName = gridDataName;
                mytable.Columns.Add("ID", typeof(Int16));
                mytable.Columns.Add("CategoryID", typeof(Int16));
                mytable.Columns.Add("Amount", typeof(float));
                mytable.Columns.Add("TaxAmount", typeof(float));
                mytable.Columns.Add("DateCreated", typeof(DateTime));
            } else
			{
                // else use the one that exists already
                mytable = Session[$"Grid{GridNumber}Datatable"] as DataTable;

                dr = mytable.Rows[mytable.Rows.Count - 1];

                // write last row from the grid to the previous row in the assiciated table
                DropDownList categoryDDL = gv.Rows[mytable.Rows.Count - 1].FindControl($"ddlCategory{GridNumber}") as DropDownList;
                TextBox txtAmount = gv.Rows[mytable.Rows.Count - 1].FindControl($"txtAmount{GridNumber}") as TextBox;
                //Label lblTaxAmount = gv.Rows[mytable.Rows.Count - 1].FindControl($"lblTaxAmount{GridNumber}") as Label;
                                
                dr["ID"] = mytable.Rows.Count;
                dr["CategoryID"] = categoryDDL.SelectedIndex;
                dr["Amount"] = txtAmount.Text;
                dr["TaxAmount"] = float.Parse(txtAmount.Text) * 1.13;
                //dr["DateCreated"] = DateTime.Now;

            }

            dr = mytable.NewRow();
            dr["CategoryID"] = 0;
            dr["Amount"] = 0;
            dr["TaxAmount"] = 0;
            //dr["DateCreated"] = DateTime.Now;
            mytable.Rows.Add(dr);

            Session[gridDataName] = mytable;

            if (gv.DataSource == null)
            {
                gv.DataSource = mytable;
                gv.DataBind();
            }

			if (mytable.Rows.Count > 1)
			{
				// adding a row to a grid that contains a row, need to now fill the new dropdown list
				DropDownList categoryDDL = gv.Rows[mytable.Rows.Count - 1].FindControl($"ddlCategory{GridNumber}") as DropDownList;
                //DropDownList productDDL = upGridViews.FindControl($"ddlProducts{GridNumber}") as DropDownList;
                DropDownList productDDL = divGridSection.FindControl($"ddlProducts{GridNumber}") as DropDownList;

                // for which product id should the category dropdown be filled with
                int forProductID = productDDL.SelectedIndex;

				FillCategoryDropDown(categoryDDL, forProductID);

			}

			// udpate the panel (update panels stop the page from flashing)
			//upGridViews.Update();

        }

        protected void ReDisplayGridViews()
        {
            GridView gv;
            gv = new GridView();

            // reassign the datatable it used
            // putting the object in front casts the session value as that object
            DataTable mytable = (DataTable)Session["grid2Table"];
                       
            gv.DataSource = mytable;
            gv.DataBind();

            //upGridViews.ContentTemplateContainer.Controls.Add(gv);
            //upGridViews.Update();
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

            //upGridViews.ContentTemplateContainer.Controls.Add(gv);
            //upGridViews.Update();
            Session["gv2"] = gv;

            //lbCurrentGrid.Text = "Current: Second Grid";
        }

        protected void btnAddRow_Click(object sender, EventArgs e)
        {

            // which button is calling
            Button callingBTN = sender as Button;
            string callingBTNNumber = callingBTN.ID.Substring(callingBTN.ID.Length - 1, 1);

            // add a new row to the associated gridview and table
            AddRowToDatatableForGrid(callingBTNNumber);

        }

		protected void ddlProducts_SelectedIndexChanged(object sender, EventArgs e)
		{
            // general event to fill the associated Grids category dropdown
            DropDownList callingDDL = sender as DropDownList;
            
            // which product dropdown list is calling
            string callingDDLNumber = callingDDL.ID.Substring(callingDDL.ID.Length - 1, 1);

            // calculate the category dropdown list to fill and find it on the page
            string fillCategoryDDLName = $"ddlCategory{callingDDLNumber}";
            
            // find the number of rows that are in the table the gridview is connected to
            DataTable dt = Session[$"Grid{callingDDLNumber}Datatable"] as DataTable;

            //GridView gv = (GridView)upGridViews.FindControl($"gv{callingDDLNumber}");
            GridView gv = (GridView)divGridSection.FindControl($"gv{callingDDLNumber}");

            DropDownList categoryDDL = gv.Rows[dt.Rows.Count-1].FindControl(fillCategoryDDLName) as DropDownList;

            // for which product id should the category dropdown be filled with
            int forProductID = callingDDL.SelectedIndex;
                        
            FillCategoryDropDown(categoryDDL, forProductID);
        }
	}
}