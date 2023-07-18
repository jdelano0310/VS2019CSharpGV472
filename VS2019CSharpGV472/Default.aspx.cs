using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace VS2019CSharpGV472
{
    public partial class _Default : Page
    {
        // hold the types so that a trip to the db isn't required on each new row
        private string[] _products;
        private string[,] _categories;
        private bool _reDisplayingPrevious = false;

        protected void Page_Load(object sender, EventArgs e)
        {

            _reDisplayingPrevious = false;

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

                // if there are more than one gridview in session, then recreate the controls
                // for the grid
                if ((int)Session["GridViewsCount"] > 1)
				{
                    if (Request.Form["__EVENTTARGET"].IndexOf("ddlProducts") > -1)
                    {
                        // the post back was caused by a programmatically created products dropdown
                        // findout which it was and place it in a session var for later use
                        string ddlPostback = Request.Form["__EVENTTARGET"].ToString();
                        ddlPostback = ddlPostback.Split(new string[] { "$" }, StringSplitOptions.None)[2];

                        Session["DropDownListPostBack"] = ddlPostback;

                        FindTheProductDDLValue();
                    }

                    ReDisplayGridViews((int)Session["GridViewsCount"]);

                }

            }
        }

        protected void FindTheProductDDLValue()
		{
            // look in the Request Form object for the value of the product dropdownlist
            // extract the value from the FORM viewstate *** there are BETTER ways to do this ***
            string postbackDDL = Session["DropDownListPostBack"].ToString();

            string postBackDDLValue = Request.Form.ToString().Substring(0, Request.Form.ToString().IndexOf("__EVENTTARGET")-1);
            string[] postBackFormValues = postBackDDLValue.Split(new string[] { "%24" }, StringSplitOptions.None);

            foreach (string fv in postBackFormValues)
			{
                if (fv.Contains(postbackDDL + "="))
				{
                    // take the value to the right of the equal sign
                    postBackDDLValue = fv.Split(new string[] { "=" }, StringSplitOptions.None)[1];
                    postBackDDLValue = postBackDDLValue.Split(new string[] { "&" }, StringSplitOptions.None)[0];
                    break;
                }
			}

            // using the array, find what index the text retrieved from the FORM data is at.
            _products = (string[])Session["Products"];
            for (int p = 1; p <= _products.GetUpperBound(0); p++)
			{
                if (_products[p] == postBackDDLValue)
				{
                    Session[$"{postbackDDL}SelectionIndex"] = p;
                    break;
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
            ddlToFill.Items.Clear();
            ddlToFill.Items.Insert(0, "Select a Cateorgy");

            if (Session["Categories"] != null)
            {
                _categories = (string[,])Session["Categories"];
            }

            // for the certain productid
            for (int c = 1; c <= _categories.GetUpperBound(1); c++)
			{
                ddlToFill.Items.Insert(c, _categories[ProductID, c]);
            }

            ddlToFill.Enabled = true;

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
            GridView gv = (GridView)upGridViews.FindControl($"gv{GridNumber}");
            
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

                
                // write last row from the grid to the previous row in the associated table
                DropDownList categoryDDL = gv.Rows[mytable.Rows.Count - 1].FindControl("ddlCategory") as DropDownList;
                TextBox txtAmount = gv.Rows[mytable.Rows.Count - 1].FindControl($"txtAmount") as TextBox;

                // this happens when a new grid is added 
                if (txtAmount.Text.Length == 0) txtAmount.Text = "0";

                dr["ID"] = mytable.Rows.Count;
                dr["CategoryID"] = categoryDDL.SelectedIndex;
                dr["Amount"] = txtAmount.Text;
                dr["TaxAmount"] = float.Parse(txtAmount.Text) * 1.13;
                //dr["DateCreated"] = DateTime.Now;
                
            }

            if (!_reDisplayingPrevious)
			{
                // only add the row if this was not called to just redisplay what was already there
                dr = mytable.NewRow();
                dr["CategoryID"] = 0;
                dr["Amount"] = 0;
                dr["TaxAmount"] = 0;
                //dr["DateCreated"] = DateTime.Now;
                mytable.Rows.Add(dr);
            }

            Session[gridDataName] = mytable; // save the associated table (with the grid data) in session

            // reapply the binding
            gv.DataSource = mytable;
    		gv.DataBind();

            if (Session[$"ddlProduct{GridNumber}SelectionIndex"] != null)
            {
                // do this if there is a selection in the product dropdown so the category dropdown is filled
                int forProductID = (int)Session[$"ddlProduct{GridNumber}SelectionIndex"];

                // re-populate the dropdown lists or populate the first 
                foreach (GridViewRow gr in gv.Rows)
                {
                    DropDownList categoryDDL = gr.FindControl("ddlCategory") as DropDownList;
                    FillCategoryDropDown(categoryDDL, forProductID);

                    if (gr.Cells[1].Text != "0")
                    {
                        // reselect the item that was selected before postback
                        categoryDDL.SelectedIndex = int.Parse(gr.Cells[1].Text);

                        // this has already been entered
                        TextBox txtAmount = gr.FindControl($"txtAmount") as TextBox;
                        txtAmount.Text = mytable.Rows[gr.RowIndex].ItemArray[2].ToString();
                        txtAmount.Enabled = false;

                    } 
                }
            }

        }

        protected void ReDisplayGrid1Rows()
		{
            //with GridView1 we just need to make sure all the rows have the dropdown displayed
            DataTable mytable = Session["Grid1Datatable"] as DataTable;
            GridView gv = (GridView)upGridViews.FindControl("gv1");

            int forProductID = (int)Session["ddlProduct1SelectionIndex"];

            // re-populate the dropdown lists or populate the first 
            foreach (GridViewRow gr in gv.Rows)
            {
                DropDownList categoryDDL = gr.FindControl("ddlCategory") as DropDownList;
                FillCategoryDropDown(categoryDDL, forProductID);

                if (gr.Cells[1].Text != "0")
                {
                    // reselect the item that was selected before postback
                    categoryDDL.SelectedIndex = int.Parse(gr.Cells[1].Text);

                    // this has already been entered
                    TextBox txtAmount = gr.FindControl($"txtAmount") as TextBox;
                    txtAmount.Text = mytable.Rows[gr.RowIndex].ItemArray[2].ToString();
                    txtAmount.Enabled = (txtAmount.Text == "0");  // the textbox is enabled if it contains "0"

                }
            }

        }

        protected void ReDisplayGridViews(int gridViewCount)
        {
            // there are more than 1 grid views, recreate them
            _reDisplayingPrevious = true;

            // with GridView1 we just need to make sure all the rows have the dropdown displayed
            ReDisplayGrid1Rows();

            for (int gc = 2; gc <= gridViewCount; gc++)
			{
                CreateOneSet(gc.ToString());
            }

            _reDisplayingPrevious = false;
        }

        class DropDownListColumn : ITemplate
        {
            public void InstantiateIn(System.Web.UI.Control container)
            {
                DropDownList ddl = new DropDownList();
                ddl.ID = "ddlCategory";
                container.Controls.Add(ddl);
            }
        }

        class TextBoxColumn : ITemplate
        {
            public void InstantiateIn(System.Web.UI.Control container)
            {
                TextBox tb = new TextBox();
                tb.ID = "txtAmount";
                container.Controls.Add(tb);
            }
        }

        protected void btnAddGrid_Click(object sender, EventArgs e)
        {
            if (Session["GridViewsCount"].ToString() == "1")
            {
                // run the Tax Amount calc on the current row of the initial gridview
                // before adding a new grid
                DataTable mytable = Session["Grid1Datatable"] as DataTable;
                DataRow dr = mytable.Rows[mytable.Rows.Count - 1];

                // write last row from the grid to the previous row in the associated table
                DropDownList categoryDDL = gv1.Rows[mytable.Rows.Count - 1].FindControl("ddlCategory") as DropDownList;
                TextBox txtAmount = gv1.Rows[mytable.Rows.Count - 1].FindControl($"txtAmount") as TextBox;

                // this happens when a new grid is added 
                if (txtAmount.Text.Length == 0) txtAmount.Text = "0";

                dr["ID"] = mytable.Rows.Count;
                dr["CategoryID"] = categoryDDL.SelectedIndex;
                dr["Amount"] = txtAmount.Text;
                dr["TaxAmount"] = float.Parse(txtAmount.Text) * 1.13;

                Session["Grid1Datatable"] = mytable;

                gv1.DataSource = mytable;
                gv1.DataBind();

                // have to reattach to the category dropdown list because it was recreated during the datasource
                // assignment above
                int selectedProductId = (int)Session[$"ddlProduct1SelectionIndex"];
                categoryDDL = gv1.Rows[mytable.Rows.Count - 1].FindControl("ddlCategory") as DropDownList;
                FillCategoryDropDown(categoryDDL, selectedProductId);

                // select the previously selected item using the data saved in the datatable that now
                // resides in the second cell of the grid
                categoryDDL.SelectedIndex = int.Parse(gv1.Rows[mytable.Rows.Count - 1].Cells[1].Text);

                // make sure any other rows are still showing the proper category dropdown
                if (mytable.Rows.Count > 1)
                {
                    for (int gv1rw = 0; gv1rw <= mytable.Rows.Count - 2; gv1rw++)
                    {
                        categoryDDL = gv1.Rows[gv1rw].FindControl("ddlCategory") as DropDownList;
                        FillCategoryDropDown(categoryDDL, selectedProductId);
                        categoryDDL.SelectedIndex = int.Parse(gv1.Rows[gv1rw].Cells[1].Text);
                    }
                }

            }

            // increment the number of grids the page is displaying count
            Session["GridViewsCount"] = (int)Session["GridViewsCount"] + 1;

            // call the sub that will create the new section which contains a new grid
            CreateOneSet(Session["GridViewsCount"].ToString());

            // update the updatepanel
            upGridViews.Update();
            
        }

        protected void CreateOneSet(string setNumber)
		{
            // this creates a new set of Add Row & Add Grid buttons, a Product Dropdown, and a Gridview
            // assigns them a number

            // the div that surrounds each set of buttons / product dropdown and grid
            Panel divCS = new Panel();
            divCS.ID = "divControlSet" + setNumber;
            divCS.Attributes.Add("style", "margin-bottom: 10px");

            // the div that surrounds the bottuns and product dropdown
            Panel divBtnsAndDropdown = new Panel();
            divBtnsAndDropdown.ID = "divBtnsAndDropdown" + setNumber;
            divBtnsAndDropdown.CssClass = "row";

            Button btn;
            btn = new Button();
            btn.ID = "btnAddRow" + setNumber;
            btn.Text = "Add Row";
            btn.Click += new EventHandler(this.btnAddRow_Click);
            btn.UseSubmitBehavior = false;

            divBtnsAndDropdown.Controls.Add(btn);
            //upGridViews.ContentTemplateContainer.Controls.Add(btn);

            btn = new Button();
            btn.ID = "btnAddGrid" + setNumber;
            btn.Text = "Add Grid";
            btn.Click += new EventHandler(this.btnAddGrid_Click);
            btn.UseSubmitBehavior = false;

            divBtnsAndDropdown.Controls.Add(btn);
            //upGridViews.ContentTemplateContainer.Controls.Add(btn);

            DropDownList newProductDDL = new DropDownList();
            newProductDDL.ID = "ddlProducts" + setNumber;
            newProductDDL.SelectedIndexChanged += new EventHandler(ddlProducts_SelectedIndexChanged);
            newProductDDL.AutoPostBack = true;

            divBtnsAndDropdown.Controls.Add(newProductDDL);
            //upGridViews.ContentTemplateContainer.Controls.Add(newProductDDL);

            FillProductDropDown(newProductDDL);

            if (Session[$"ddlProducts{setNumber}SelectionIndex"] != null)
            {
                newProductDDL.SelectedIndex = (int)Session[$"ddlProducts{setNumber}SelectionIndex"];
            }

            divCS.Controls.Add(divBtnsAndDropdown);

            GridView gv = new GridView();
            gv.ID = "gv" + setNumber;
            gv.AutoGenerateColumns = false;

            TemplateField tfield;
            BoundField bfield;

            // add the template fields
            tfield = new TemplateField();
            tfield.HeaderText = "Category";
            tfield.ItemTemplate = new DropDownListColumn();
            gv.Columns.Add(tfield);

            bfield = new BoundField();
            bfield.DataField = "CategoryID";
            bfield.HeaderText = "";
            bfield.ItemStyle.Font.Size = 0;
            gv.Columns.Add(bfield);

            tfield = new TemplateField();
            tfield.HeaderText = "Amount";
            tfield.ItemTemplate = new TextBoxColumn();
            gv.Columns.Add(tfield);

            bfield = new BoundField();
            bfield.DataField = "TaxAmount";
            bfield.HeaderText = "Tax Amount";
            bfield.ItemStyle.Width = 200;
            gv.Columns.Add(bfield);

            divCS.Controls.Add(gv);
            upGridViews.ContentTemplateContainer.Controls.Add(divCS);
            //upGridViews.ContentTemplateContainer.Controls.Add(gv);

            AddRowToDatatableForGrid(setNumber);
            ddlProducts_SelectedIndexChanged(newProductDDL, null);
        }


        protected void btnAddRow_Click(object sender, EventArgs e)
        {

            // which button is calling - need this so you know which grid to add thw new row to
            Button callingBTN = sender as Button;
            string callingBTNNumber = callingBTN.ID.Substring(callingBTN.ID.Length - 1);

            // add a new row to the associated gridview and table
            AddRowToDatatableForGrid(callingBTNNumber);

            // having a number greater than one assigned to the button that caused this event
            // to trigger, means there are mulitple gridviews displayed on the page. 
            // tell the update panel to, well, update to diplay their newest state
            if (int.Parse(callingBTNNumber) > 1)
			{
                upGridViews.Update();
			}
        }

		protected void ddlProducts_SelectedIndexChanged(object sender, EventArgs e)
		{
            // general event to fill the associated Grids category dropdown
            DropDownList callingDDL = sender as DropDownList;
            
            // which product dropdown list is calling
            string callingDDLNumber = callingDDL.ID.Substring(callingDDL.ID.Length - 1, 1);

            // find the number of rows that are in the table the gridview is connected to
            DataTable dt = Session[$"Grid{callingDDLNumber}Datatable"] as DataTable;

            // get the gridview the products dropdown is associated with
            GridView gv = (GridView)upGridViews.FindControl($"gv{callingDDLNumber}");

            // now get the category dropdown list in the last row of the gridview from above
            DropDownList categoryDDL = gv.Rows[dt.Rows.Count - 1].FindControl("ddlCategory") as DropDownList;

            // for which product id should the category dropdown be filled with
            int forProductID = callingDDL.SelectedIndex;

            // remember what the product dropdown selection was
            Session[$"ddlProduct{callingDDLNumber}SelectionIndex"] = forProductID;

            // call the routine that fills the category dropdown lists
            FillCategoryDropDown(categoryDDL, forProductID);
        }

	}
}