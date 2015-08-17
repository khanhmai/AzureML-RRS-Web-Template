using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AzureMLInterface.Model;
using AzureMLInterface.Controlers;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Net;
using System.Net.Security;
using Newtonsoft.Json.Linq;
using ParameterIO;

namespace AzureMLRRSWebTemplate
{
    public partial class Default : System.Web.UI.Page
    {
        AMLParameterObject paramObj = new AMLParameterObject();
        string webServicePostUrl;
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Web.HttpBrowserCapabilities browser = Request.Browser;
            if (paramObj.ImportInputParameter(Server.MapPath("~\\Resources\\AMLParameter.xml")))
            {
                Page.Title = paramObj.Title;
                lblTitle.Text = paramObj.Title;                
                webServicePostUrl = paramObj.Url;
            }
            else RequireInfor();

            GenerateControl.ShowInput(InputPlaceHolder1, InputPlaceHolder2, paramObj.listInputParameter, browser);
        }

        private void RequireInfor()
        {
            Response.Redirect("Setting.aspx");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> featureList = getColumnsAndValues();

            //if (featureList == null || featureList.Count == 0) return;

            InvokeRequestResponseService_A(featureList).Wait();
        }


        //Check and adjust URL version if necessary
        private string setPostURLString()
        {
            //version 2 ends with exeucte. If passed in v2 URL, change to end with Score
            int idx = webServicePostUrl.IndexOf("/execute");
            if (idx > 0)
            {
                //remove everything after and including '/execute' and replace with '/score'
                string temp = webServicePostUrl.Substring(0, (webServicePostUrl.Length - (webServicePostUrl.Length - idx)));
                webServicePostUrl = temp + "/score";
            }
            return webServicePostUrl;
        }


        //Get the controls(columns) from the placeholder and get their values to submit to the API
        private Dictionary<string, string> getColumnsAndValues()
        {
            var dict = new Dictionary<string, string>();
            List<Control> listInput = new List<Control>();
            foreach (Control control in InputPlaceHolder1.Controls)
                listInput.Add(control);
            foreach (Control control in InputPlaceHolder2.Controls)
                listInput.Add(control);

            foreach (Control control in listInput)
            {
                string columnName = "";
                string columnValue = "";

                if (control.ID != null)
                {
                    columnName = control.ID;
                    if (control is TextBox || control is DropDownList || control is RadioButtonList)
                    {
                        if (control is TextBox)
                        {
                            TextBox txt = control as TextBox;
                            if (txt.Text != "")
                                columnValue = txt.Text;
                        }
                        else if (control is DropDownList)
                        {
                            DropDownList lb = control as DropDownList;
                            if (lb.SelectedIndex != -1)
                                columnValue = lb.SelectedValue;
                        }
                        else if(control is RadioButtonList)
                        {
                            RadioButtonList ct = control as RadioButtonList;
                            if (ct.SelectedIndex != -1)
                                columnValue = ct.SelectedValue;
                        }
                        //add to list
                        dict.Add(columnName, columnValue);
                    }
                }
            }


            return dict;
        }
        //******************************************RRS Client code to submit request to server***************************************
        //This is copied from the help page sample code and modified for ASP.NEt
        async Task InvokeRequestResponseService_A(Dictionary<string, string> columnsDictionary)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            string[] colums = new string[columnsDictionary.Count];
            columnsDictionary.Keys.CopyTo(colums, 0);

            string[] values = new string[columnsDictionary.Count];
            columnsDictionary.Values.CopyTo(values, 0);

            StringTable strtable = new StringTable();
            strtable.ColumnNames = colums;
            strtable.Values = new string[2, values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                strtable.Values[0, i] = values[i];
                strtable.Values[1, i] = values[i];
            }

            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable>() { 
                        { 
                            "input1", 
                            strtable
                        },
                                        },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };

                string apiKey = (paramObj.APIKey);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri(webServicePostUrl);

                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)


                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest).ConfigureAwait(false); ;

                if (response.IsSuccessStatusCode)
                {
                    string apiResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    //List<string> listValues = ExtractValues(apiResult);
                    //GenerateControl.ShowOutput(OutputPlaceHolder, bottomsciprtPlaceHolde, paramObj.listOutputParameter, listValues, this);

                    List<OutputObject> listOutputObject = ExtractValuesObject(apiResult);
                    GenerateControl.ShowOutput(OutputPlaceHolder, bottomsciprtPlaceHolde, paramObj.listOutputParameter, listOutputObject, this);

                    FocusControlOnPageLoad("divResult");
                }
                else
                {
                    divResult.InnerText = string.Format("The request failed with status code: {0}", response.StatusCode);
                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                    divResult.InnerText = response.Headers.ToString();
                    string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    divResult.InnerText = responseContent;
                }
            }
        }        
        static List<OutputObject> ExtractValuesObject(string jsonStr)
        {
            try
            {
                List<OutputObject> listOutput = new List<OutputObject>();
                var objects = JObject.Parse(JObject.Parse(jsonStr)["Results"].ToString());

                foreach (var output in objects)
                {
                    OutputObject tmpOutput = new OutputObject();
                    tmpOutput.Name = output.Key;
                    JArray outputArray = JArray.Parse(output.Value["value"]["Values"][0].ToString());
                    foreach (var outputValue in outputArray)
                        tmpOutput.Values.Add(outputValue.ToString());

                    listOutput.Add(tmpOutput);
                }
                return listOutput;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public class StringTable
        {
            public string[] ColumnNames { get; set; }
            public string[,] Values { get; set; }
        }

        public Dictionary<string, string> FeatureVector { get; set; }
        public Dictionary<string, string> GlobalParameters { get; set; }

        public class ScoreData
        {
            public Dictionary<string, string> FeatureVector { get; set; }
            public Dictionary<string, string> GlobalParameters { get; set; }
        }

        public class ScoreRequest
        {
            public string Id { get; set; }
            public ScoreData Instance { get; set; }
        }


        public void FocusControlOnPageLoad(string ClientID)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "CtrlFocus", "ScrollView(\"" + ClientID +"\")", true);
        }
    }
}