using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AzureMLInterface.Model;
using ParameterIO;

namespace AzureMLRRSWebTemplate
{
    public partial class Setting : System.Web.UI.Page
    {
        AMLParameterObject paramObj = new AMLParameterObject();
        string inputPost = "input";
        string outputPost = "output";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (paramObj.ImportInputParameter(Server.MapPath("~\\Resources\\AMLParameter.xml")))
            {
                lblTitle.Text = paramObj.Title;
                if (!Page.IsPostBack)
                {
                    txtURL.ReadOnly = true;
                    txtAPI.ReadOnly = true;
                    btnSubmit.Visible = false;
                    btnSave.Visible = false;

                    lblResult.Text = "Please Refresh page to enter Key API and access to Setting.";
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "EnterKeyModal", "$('#EnterKeyModal').modal();", true);
                }

                else if (btnSave.Visible)
                {
                    GenerateParameterList();
                }
            }
            else
            {
                btnSave.Visible = false;
                return;
            }
        }

        private void Script_HideServiceInfo()
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "hideServiceInfo", "hideServiceInfo();", true);
        }

        private void ResetParameter()
        {
            paramObj.Url = txtURL.Text.Trim();
            paramObj.APIKey = (txtAPI.Text).Trim();
            string result = paramObj.ReadSwagger();
            if (string.IsNullOrEmpty(result))
            {
                paramObj.ExportInputParameter(Server.MapPath("~\\Resources\\AMLParameter.xml"));
                lblResult.Text = "Press Submit to load parameters";
                GenerateParameterList();

                Script_HideServiceInfo();
            }
            else
            {
                lblResult.Text = result;
                parameterRegion.Controls.Clear();
                btnSave.Visible = false;
                lblTitle.Text = "";
            }
        }

        protected void btnResetParameter_Click(object sender, EventArgs e)
        {
            ResetParameter();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(paramObj.Url))
                ResetParameter();
            else ScriptManager.RegisterStartupScript(Page, Page.GetType(), "submitModal", "$('#submitModal').modal();", true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < paramObj.listInputParameter.Count; i++)
                {
                    var param = paramObj.listInputParameter[i];
                    param.Alias = ((TextBox)FindControl(string.Format("input{0}_alias_{1}", i, inputPost))).Text;
                    param.Description = ((TextBox)FindControl(string.Format("input{0}_description_{1}", i, inputPost))).Text;
                    if (param.Type == "integer" || param.Type == "number")
                    {
                        param.MinValue = ((TextBox)FindControl(string.Format("input{0}_min_{1}", i, inputPost))).Text;
                        param.MaxValue = ((TextBox)FindControl(string.Format("input{0}_max_{1}", i, inputPost))).Text;
                    }

                    if (param.StrEnum != null && param.StrEnum.Count > 0)
                    {
                        param.DefaultValue = ((DropDownList)FindControl(string.Format("input{0}_default_{1}", i, inputPost))).SelectedValue;
                    }
                    else
                    {

                        param.DefaultValue = ((TextBox)FindControl(string.Format("input{0}_default_{1}", i, inputPost))).Text;
                    }
                }

                for (int i = 0; i < paramObj.listOutputParameter.Count; i++)
                {
                    var param = paramObj.listOutputParameter[i];
                    param.Alias = ((TextBox)FindControl(string.Format("output{0}_alias_{1}", i, outputPost))).Text;
                    param.Enable = ((CheckBox)FindControl(string.Format("output{0}_enable_{1}", i, outputPost))).Checked;
                }

                paramObj.Title = ((TextBox)FindControl("serviceTitle")).Text;
                paramObj.Description = ((TextBox)FindControl("serviceDescription")).Text;
                //paramObj.Copyright = ((TextBox)FindControl("serviceCopyright")).Text;

                if(paramObj.ExportInputParameter(Server.MapPath("~\\Resources\\AMLParameter.xml")))
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "saveChangeModalSuccess", "$('#saveChangeModalSuccess').modal();", true);
                else ScriptManager.RegisterStartupScript(Page, Page.GetType(), "saveChangeModalError", "$('#saveChangeModalError').modal();", true);

            }
            catch(Exception ex)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "saveChangeModalError", "$('#saveChangeModalError').modal();", true);
            }
            
        }

        private void GenerateParameterList()
        {
            parameterRegion.Controls.Clear();

            GenarateInformation();

            GenerateInputParameterList();

            GenerateOutputParameterList();

            btnSave.Visible = true;
        }

        private void GenarateInformation()
        {
            lblTitle.Text = paramObj.Title;

            TextBox txtName = new TextBox();
            txtName.ID = "serviceTitle";
            txtName.Text = paramObj.Title;
            txtName.Attributes.Add("Class", "amount form-control");

            TextBox txtDescription = new TextBox();
            txtDescription.ID = "serviceDescription";
            txtDescription.Text = paramObj.Description;
            txtDescription.Attributes.Add("Class", "amount form-control");
            txtDescription.Attributes.Add("placeholder", "Leave blank to hide");

            TextBox txtCopyright = new TextBox();
            txtCopyright.ID = "serviceCopyright";
            txtCopyright.Text = paramObj.Copyright;
            txtCopyright.Attributes.Add("Class", "amount form-control");
            txtCopyright.Attributes.Add("placeholder", "Default is Microsoft 2015");

            parameterRegion.Controls.Add(new LiteralControl("<p class=\"titleresult\">Web Service Information</p>"));
            parameterRegion.Controls.Add(new LiteralControl("<table class=\"table table-hover table-bordered\">"));
            parameterRegion.Controls.Add(new LiteralControl("<tr><td style=\"width:25%\">Service Name</td>"));
            parameterRegion.Controls.Add(new LiteralControl(string.Format("<td>")));
            parameterRegion.Controls.Add(txtName);
            parameterRegion.Controls.Add(new LiteralControl("</td></tr>"));

            parameterRegion.Controls.Add(new LiteralControl("<tr><td style=\"width:25%\">Service Description</td>"));
            parameterRegion.Controls.Add(new LiteralControl(string.Format("<td>")));
            parameterRegion.Controls.Add(txtDescription);
            parameterRegion.Controls.Add(new LiteralControl(string.Format("</td></tr>")));


            //parameterRegion.Controls.Add(new LiteralControl("<tr><td style=\"width:25%\">Copyright</td>"));
            //parameterRegion.Controls.Add(new LiteralControl(string.Format("<td>")));
            //parameterRegion.Controls.Add(txtCopyright);
            //parameterRegion.Controls.Add(new LiteralControl(string.Format("</td></tr>")));

            parameterRegion.Controls.Add(new LiteralControl("</table>"));
        }


        private void GenerateOutputParameterList()
        {
            //parameterRegion.Controls.Clear();
            parameterRegion.Controls.Add(new LiteralControl("<p class=\"titleresult\">List of Output Parameters</p>"));
            parameterRegion.Controls.Add(new LiteralControl("<table class=\"table table-hover table-bordered\">"));
            parameterRegion.Controls.Add(new LiteralControl("<tr class=\"info\"><th>#</th><th>Name</th><th>Type</th><th>Alias</th><th>Eable</th></tr>"));

            for (int i = 0; i < paramObj.listOutputParameter.Count; i++)
            {
                var param = paramObj.listOutputParameter[i];
                parameterRegion.Controls.Add(new LiteralControl("<tr>"));
                parameterRegion.Controls.Add(new LiteralControl(string.Format("<td>{0}</td>", i + 1)));
                parameterRegion.Controls.Add(new LiteralControl(string.Format("<td>{0}</td>", param.Name)));
                parameterRegion.Controls.Add(new LiteralControl(string.Format("<td>{0}</td>", param.Type)));

                ////////// Alias value control //////
                parameterRegion.Controls.Add(new LiteralControl(string.Format("<td class=\"mytextbox\">")));
                parameterRegion.Controls.Add(GenerateControlAlias(param, string.Format("output{0}_alias_{1}", i, outputPost)));
                parameterRegion.Controls.Add(new LiteralControl(string.Format("</td>")));
                //////////////////////////////////////

                ////////// Enable control //////
                parameterRegion.Controls.Add(new LiteralControl(string.Format("<td class=\"mytextbox\">")));
                parameterRegion.Controls.Add(GenerateCOntrolEnable(param, string.Format("output{0}_enable_{1}", i, outputPost)));
                parameterRegion.Controls.Add(new LiteralControl(string.Format("<script>$(\"input[name='{0}']\").bootstrapSwitch();</script>", string.Format("output{0}_enable_{1}", i, outputPost))));
                parameterRegion.Controls.Add(new LiteralControl(string.Format("</td>")));
                //////////////////////////////////////


                parameterRegion.Controls.Add(new LiteralControl("</tr>"));                
            }

            parameterRegion.Controls.Add(new LiteralControl("</table>"));
        }
        private void GenerateInputParameterList()
        {
            //parameterRegion.Controls.Clear();
            parameterRegion.Controls.Add(new LiteralControl("<p class=\"titleresult\">List of Input Parameters</p>"));
            parameterRegion.Controls.Add(new LiteralControl("<table class=\"table table-hover table-bordered\">"));
            parameterRegion.Controls.Add(new LiteralControl("<tr class=\"info\"><th>#</th><th>Name</th><th>Type</th><th>Alias</th><th>Description</th><th>Default</th><th>Min</th><th>Max</th></tr>"));

            if(paramObj.listInputParameter != null)
                for (int i = 0; i < paramObj.listInputParameter.Count; i++ )
                {
                    var param = paramObj.listInputParameter[i];
                    parameterRegion.Controls.Add(new LiteralControl("<tr>"));
                    parameterRegion.Controls.Add(new LiteralControl(string.Format("<td>{0}</td>", i+1)));
                    parameterRegion.Controls.Add(new LiteralControl(string.Format("<td>{0}</td>", param.Name)));
                    parameterRegion.Controls.Add(new LiteralControl(string.Format("<td>{0}</td>", param.Type)));
                    //parameterRegion.Controls.Add(new LiteralControl(string.Format("<td>{0}</td>", param.Format)));

                    ////////// Alias value control //////
                    parameterRegion.Controls.Add(new LiteralControl(string.Format("<td class=\"mytextbox\">")));
                    parameterRegion.Controls.Add(GenerateControlAlias(param, string.Format("input{0}_alias_{1}",i,inputPost)));
                    parameterRegion.Controls.Add(new LiteralControl(string.Format("</td>")));
                    //////////////////////////////////////

                    ////////// Description value control //////
                    parameterRegion.Controls.Add(new LiteralControl(string.Format("<td class=\"mytextbox\">")));
                    parameterRegion.Controls.Add(GenerateControlDescription(param, string.Format("input{0}_description_{1}", i, inputPost)));
                    parameterRegion.Controls.Add(new LiteralControl(string.Format("</td>")));
                    //////////////////////////////////////

                    ////////// Default value control //////
                    parameterRegion.Controls.Add(new LiteralControl(string.Format("<td class=\"mydropdown\">")));
                    parameterRegion.Controls.Add(GenerateControlDefault(param, string.Format("input{0}_default_{1}", i, inputPost)));
                    parameterRegion.Controls.Add(new LiteralControl(string.Format("</td>")));
                    //////////////////////////////////////

                    ////////// Min value control //////
                    parameterRegion.Controls.Add(new LiteralControl(string.Format("<td class=\"mynumber\">")));
                    parameterRegion.Controls.Add(GenerateControlMin(param, string.Format("input{0}_min_{1}", i, inputPost)));
                    parameterRegion.Controls.Add(new LiteralControl(string.Format("</td>")));
                    //////////////////////////////////////

                    ////////// Max value control //////
                    parameterRegion.Controls.Add(new LiteralControl(string.Format("<td class=\"mynumber\">")));
                    parameterRegion.Controls.Add(GenerateControlMax(param, string.Format("input{0}_max_{1}", i, inputPost)));
                    parameterRegion.Controls.Add(new LiteralControl(string.Format("</td>")));
                    //////////////////////////////////////

                    parameterRegion.Controls.Add(new LiteralControl("</tr>"));           
                }

            parameterRegion.Controls.Add(new LiteralControl("</table>"));

        }

        private Control GenerateCOntrolEnable(AMLParam param, string id)
        {
            CheckBox chb = new CheckBox();
            chb.ID = id;
            chb.Checked = param.Enable;
            return chb;
        }
        private Control GenerateControlAlias(AMLParam param, string id)
        {
            TextBox txt = new TextBox();
            txt.ID = id;
            txt.Text = param.Alias;
            txt.Attributes.Add("Class", "amount form-control");
            return txt;
        }

        private Control GenerateControlDescription(AMLParam param, string id)
        {
            TextBox txt = new TextBox();
            txt.ID = id;
            txt.Text = param.Description;
            txt.Attributes.Add("Class", "amount form-control");
            txt.TextMode = TextBoxMode.MultiLine;
            return txt;
        }

        private Control GenerateControlDefault(AMLParam param, string id)
        {
            TextBox txt = new TextBox();
            txt.ID = id;
            txt.Text = param.DefaultValue;
            txt.Attributes.Add("Class", "amount form-control form-control-number");
            if (param.StrEnum != null && param.StrEnum.Count > 0)      // Droplist
                return GenerateDropdownList(param, id);
            else if (param.Type == "integer")
            {
                txt.TextMode = TextBoxMode.Number;
                return txt;
            }            
            else return txt;
        }

        private Control GenerateControlMin(AMLParam param, string id)
        {
            TextBox txt = new TextBox();
            txt.ID = id;
            txt.Text = param.MinValue;
            txt.Attributes.Add("Class", "amount form-control form-control-number");

            if (param.StrEnum != null && param.StrEnum.Count > 0)      // Droplist
            {
                txt.Text = "";
                txt.ReadOnly = true;
                return txt;
            }

            if (param.Type == "integer")
            {
                txt.TextMode = TextBoxMode.Number;
                return txt;
            }
            
            if (param.Type == "number")
            {
                return txt;
            }
            else
            {
                txt.Text = "";
                txt.ReadOnly = true;
                return txt;
            }
        }

        private Control GenerateControlMax(AMLParam param, string id)
        {
            TextBox txt = new TextBox();
            txt.ID = id;
            txt.Text = param.MaxValue;
            txt.Attributes.Add("Class", "amount form-control form-control-number");

            if (param.StrEnum != null && param.StrEnum.Count > 0)      // Droplist
            {
                txt.Text = "";
                txt.ReadOnly = true;
                return txt;
            }

            if (param.Type == "integer")
            {
                txt.TextMode = TextBoxMode.Number;
                return txt;
            }
            else if (param.Type == "number")
            {
                return txt;
            }
            else
            {
                txt.Text = "";
                txt.ReadOnly = true;
                return txt;
            }
        }

        private Control GenerateDropdownList(AMLParam param, string id)
        {
            DropDownList drop = new DropDownList();
            drop.ID = id;
            drop.Attributes.Add("Class", "amount form-control");
            foreach (string ele in param.StrEnum)
            {
                ListItem litem = new ListItem();
                litem.Text = ele;
                if (litem.Text.ToLower() == param.DefaultValue.ToLower()) litem.Selected = true;
                drop.Items.Add(litem);
            }
            return drop;
        }

        


        #region Modal event
        protected void btnSubmitKey_Click(object sender, EventArgs e)
        {
            string keyinput = txtKeyInput.Text;

            if (keyinput == (paramObj.APIKey))
            {
                //if (!Page.IsPostBack)
                {
                    txtURL.Text = paramObj.Url;
                    txtAPI.Text = keyinput;
                }
                GenerateParameterList();

                txtURL.ReadOnly = false;
                txtAPI.ReadOnly = false;
                btnSubmit.Visible = true;

                lblResult.Text = "Press Submit to load parameters";

                Script_HideServiceInfo();
            }
            else
            {
                Response.Redirect("Setting.aspx");
            }
            
        }

        protected void btnGoToHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }
        #endregion
    }
}