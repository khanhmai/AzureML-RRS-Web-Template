using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using AzureMLInterface.Model;
using System.Web.UI;
using Newtonsoft.Json.Linq;
using ParameterIO;

namespace AzureMLInterface.Controlers
{
    public static class GenerateControl
    {
        static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        
        /// <summary>
        /// Show list of Output to user by OutputParameter and value
        /// </summary>
        /// <param name="outputPlace">div of output</param>
        /// <param name="sciprtPlaceHolde"> div of script if need </param>
        /// <param name="outputParam"> list of output Prameter </param>
        /// <param name="listOutput"> list of Output Object (name, list value) </param>
        /// <param name="page"> Main page </param>
        static public void ShowOutput(PlaceHolder outputPlace, PlaceHolder sciprtPlaceHolde, List<AMLParam> outputParam, List<OutputObject> listOutput, Page page)
        {
            Dictionary<string, double> listOutputNumber = new Dictionary<string, double>();
            outputPlace.Controls.Add(new LiteralControl("<p class=\"titleresult\">Result</p>"));
            int count = 0;
            outputPlace.Controls.Add(new LiteralControl("<table class=\"table table-hover table-bordered\">"));
            outputPlace.Controls.Add(new LiteralControl("<tr class=\"info\"><th>Label</th><th>Value</th></tr>"));

            foreach (OutputObject outputObj in listOutput)
            {
                bool showOutputLabel = false;
                for (int i = 0; i < outputObj.Values.Count; i++)
                {
                    if (outputParam[count + i].Enable) { showOutputLabel = true; break; }
                }
               
                if(showOutputLabel)
                    outputPlace.Controls.Add(new LiteralControl("<tr><td colspan=\"2\" style=\"background-color: azure\">" + outputObj.Name + "</td></tr>"));

                for (int i = 0; i < outputObj.Values.Count; i++)
                {
                    AMLParam param = outputParam[count];
                    // Check value, if empty, skip this field, don't show on output region
                    
                    if (param.Enable)
                    {
                        //if (string.IsNullOrEmpty(outputObj.Values[i])) continue;
                        // Show image from output
                        if (param.Name.Contains("R Output JSON"))
                        {
                            outputPlace.Controls.Add(new LiteralControl("<tr>"));
                            outputPlace.Controls.Add(new LiteralControl(string.Format("<td class=\"textresult\">{0}</td>", textInfo.ToTitleCase(param.Name))));
                            if (!string.IsNullOrEmpty(outputObj.Values[i])) 
                                outputPlace.Controls.Add(new LiteralControl(string.Format("<td>{0}</td>", "Please check Image")));
                            else outputPlace.Controls.Add(new LiteralControl(string.Format("<td></td>")));
                            outputPlace.Controls.Add(new LiteralControl("</tr>"));

                            if (!string.IsNullOrEmpty(outputObj.Values[i]))
                            {
                                Image img = new Image();
                                img.ImageUrl = @"data:image/png;base64," + getImageData(outputObj.Values[i]);
                                img.Style["width"] = "100%";
                                img.Style["max-width"] = "480px";

                                // add picture outsite of table (script path)

                                sciprtPlaceHolde.Controls.Add(new LiteralControl("<div style=\"text-align:center\">"));
                                sciprtPlaceHolde.Controls.Add(img);
                                sciprtPlaceHolde.Controls.Add(new LiteralControl("</div>"));
                            }
                        }
                        else if (param.Name.Equals("Graphics"))
                        {
                            outputPlace.Controls.Add(new LiteralControl("<tr>"));
                            outputPlace.Controls.Add(new LiteralControl(string.Format("<td class=\"textresult\">{0}</td>", textInfo.ToTitleCase(param.Name))));
                            if (!string.IsNullOrEmpty(outputObj.Values[i]))
                                outputPlace.Controls.Add(new LiteralControl(string.Format("<td>{0}</td>", "Please check Image")));
                            else outputPlace.Controls.Add(new LiteralControl(string.Format("<td></td>")));
                            outputPlace.Controls.Add(new LiteralControl("</tr>"));

                            if (!string.IsNullOrEmpty(outputObj.Values[i]))
                            {
                                Image img = new Image();
                                img.ImageUrl = @"data:image/png;base64," + outputObj.Values[i];
                                img.Style["width"] = "100%";
                                img.Style["max-width"] = "480px";

                                // add picture outsite of table (script path)

                                sciprtPlaceHolde.Controls.Add(new LiteralControl("<div style=\"text-align:center\">"));
                                sciprtPlaceHolde.Controls.Add(img);
                                sciprtPlaceHolde.Controls.Add(new LiteralControl("</div>"));
                            }
                        }

                        else
                        {
                            outputPlace.Controls.Add(new LiteralControl("<tr>"));
                            outputPlace.Controls.Add(new LiteralControl(string.Format("<td class=\"textresult\">{0}</td>", string.IsNullOrEmpty(param.Alias) ? textInfo.ToTitleCase(param.Name) : param.Alias)));
                            outputPlace.Controls.Add(new LiteralControl(string.Format("<td>{0}</td>", outputObj.Values[i].Trim('\"'))));
                            if (param.Type == "number")
                            {
                                double tmp = 0;
                                try
                                {
                                    tmp = Math.Round(Convert.ToDouble(outputObj.Values[i].Trim('\"')), 2);
                                    listOutputNumber.Add(!string.IsNullOrEmpty(param.Alias) ? param.Alias : param.Name, tmp);
                                }
                                catch (Exception) { };

                            }
                            outputPlace.Controls.Add(new LiteralControl("</tr>"));
                        }
                    }
                    count++;
                }
                
            }
            outputPlace.Controls.Add(new LiteralControl("</table>"));
            //if (listOutputNumber.Count == 1)
            //    ScriptManager.RegisterStartupScript(page, page.GetType(), "script", "loadPercentage(" + listOutputNumber.First().Value + ");", true);
            //else if (listOutputNumber.Count > 1) sciprtPlaceHolde.Controls.Add(GenerateScriptBarGraph(listOutputNumber));

        }
        
        /// <summary>
        /// Extrack base64 data from output of R Script Output
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        static public string getImageData(string jsonStr)
        {
            try
            {
                var objects = JObject.Parse(jsonStr);
                return objects.SelectToken("['Graphics Device']")[0].ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Show list of input
        /// </summary>
        /// <param name="inputPlace1"> div of clolumn 1 </param>
        /// <param name="inputPlace2"> div of clolumn 2 </param>
        /// <param name="inputParam"> list of Parameter Object </param>
        /// <param name="browser"> Detect Browser </param>
        static public void ShowInput(PlaceHolder inputPlace1, PlaceHolder inputPlace2, List<AMLParam> inputParam, HttpBrowserCapabilities browser)
        {
            if (inputParam == null || inputParam.Count == 0)
            {
                inputPlace1.Controls.Add(new LiteralControl("<h3>No web service input</h3>"));
                return;
            }
            int count = 0;
            for (int i = 0; i < inputParam.Count; i++) //MLParameter param in inputParam)
            {
                Literal lcssFormGroup = new Literal();
                lcssFormGroup.Text = "<div class=\"form-group\">";
                Literal ldiv = new Literal();
                ldiv.Text = "</div>";
                

                AMLParam param = inputParam[i];
                Label lbl = new Label();
                if(string.IsNullOrEmpty(param.Alias))
                    lbl.Text = textInfo.ToTitleCase(param.Name);
                else lbl.Text = textInfo.ToTitleCase(param.Alias);
                lbl.ID = "lbl" + param + count++;
                lbl.Attributes.Add("Class", "fieldname");
                
                if (i <= inputParam.Count / 2)
                {
                    inputPlace1.Controls.Add(lcssFormGroup);
                    inputPlace1.Controls.Add(lbl);

                    foreach (var control in GenerateInputControl(param, browser))
                    {
                        inputPlace1.Controls.Add(control);
                    }

                    if (!string.IsNullOrEmpty(param.Description)) inputPlace1.Controls.Add(new LiteralControl(string.Format("<div style=\"width=:100%\"><samp>{0}</samp></div>", param.Description)));
                    inputPlace1.Controls.Add(ldiv);
                    
                }
                else
                {
                    inputPlace2.Controls.Add(lcssFormGroup);
                    inputPlace2.Controls.Add(lbl);
                    foreach (var control in GenerateInputControl(param, browser))
                    {
                        inputPlace2.Controls.Add(control);
                    }
                    if (!string.IsNullOrEmpty(param.Description)) inputPlace2.Controls.Add(new LiteralControl(string.Format("<div style=\"width=:100%\"><samp>{0}</samp></div>", param.Description)));
                    inputPlace2.Controls.Add(ldiv);
                }
            }
        }

        static IEnumerable<Control> GenerateInputControl(AMLParam param, HttpBrowserCapabilities browser)
        {
            if (param.StrEnum != null && param.StrEnum.Count > 3)           // Drop list
            {
                yield return GenerateDropdownList(param);
            }
            else if (param.StrEnum != null && param.StrEnum.Count <= 3 && param.StrEnum.Count > 0)     // Radio Button
            {
                yield return GenerateRadio(param);
            }
            else if (param.Type.ToLower() == "integer")   // slider number Integer
            {
                TextBox txt = new TextBox();
                txt.ID = param.Name;
                txt.Text = param.DefaultValue;
                txt.Attributes.Add("Class", "amount form-control form-control-number");
                if (param.Type == "integer")
                    txt.TextMode = TextBoxMode.Number;
                if(browser.Browser.ToLower() == "internetexplorer")
                {
                    yield return GenerateSliderHTML(param, "1");
                    yield return txt;
                    yield return new LiteralControl("</div>");  // end of div
                    yield return GenerateScriptSlideIE(param);
                }

                else
                {
                    yield return GenerateSliderHTML(param, "1");
                    yield return txt;
                    yield return new LiteralControl("</div>");  // end of div
                    yield return GenerateScriptSlideHTML(param);
                }
            }
            else if (param.Type.ToLower() == "number")   // slider number Integer
            {
                TextBox txt = new TextBox();
                txt.ID = param.Name;
                txt.Text = param.DefaultValue;
                txt.Attributes.Add("Class", "amount form-control form-control-number");
                if (browser.Browser.ToLower() == "internetexplorer")
                {
                    yield return GenerateSliderHTML(param, "0.01");
                    yield return txt;
                    yield return new LiteralControl("</div>");  // end of div
                    yield return GenerateScriptSlideIE(param);
                }

                else
                {
                    yield return GenerateSliderHTML(param, "0.01");
                    yield return txt;
                    yield return new LiteralControl("</div>");  // end of div
                    yield return GenerateScriptSlideHTML(param);
                }
            }
            else
            {
                TextBox txt = new TextBox();
                txt.ID = param.Name;
                txt.Text = param.DefaultValue;
                txt.Attributes.Add("Class", "form-control");
                if (param.Type == "integer")
                    txt.TextMode = TextBoxMode.Number;

                yield return txt;
            }
        }

        static private Control GenerateDropdownList(AMLParam param)
        {
            DropDownList drop = new DropDownList();
            drop.ID = param.Name;
            drop.Attributes.Add("Class", "form-control");
            foreach (string ele in param.StrEnum)
            {
                ListItem litem = new ListItem();
                litem.Text = ele;
                if (litem.Text.ToLower() == param.DefaultValue.ToLower()) litem.Selected = true;
                drop.Items.Add(litem);
            }
            return drop;
        }

        static private Control GenerateRadio(AMLParam param)
        {
            RadioButtonList rabtnList = new RadioButtonList();
            rabtnList.ID = param.Name;
            var listEmum = param.StrEnum.OrderBy(x => x.Length).ToList();
            foreach (string ele in listEmum)
            {
                ListItem litem = new ListItem();
                litem.Text = ele;
                if (litem.Text.ToLower() == param.DefaultValue.ToLower()) litem.Selected = true;
                rabtnList.Items.Add(litem);
            }
            rabtnList.RepeatLayout = RepeatLayout.Flow;
            rabtnList.RepeatDirection = RepeatDirection.Horizontal;
            return rabtnList;
        }

        static private Control GenerateSliderIE(AMLParam param, string stepvalue)
        {
            LiteralControl lit = new LiteralControl();
            lit.Text += string.Format("{0:#,###0}",int.Parse(param.MinValue));
            lit.Text += string.Format("<div id=\"IEslider_{0}\" class = \"IEslider slider-displayIE\" data-begin=\"{1}\" data-end=\"{2}\" data-def=\"{3}\" data-step=\"{4}\"></div>",
                                                param.Name, param.MinValue, param.MaxValue, param.DefaultValue, stepvalue);
            lit.Text += string.Format("{0:#,###0}", int.Parse(param.MaxValue));
            return lit;
        }

        /// <summary>
        /// Create slider by HTML5, we don't need default value because jQuery will update slider's value from textbox
        /// </summary>
        /// <param name="param"></param>
        /// <param name="stepvalue"></param>
        /// <returns></returns>
        static private Control GenerateSliderHTML(AMLParam param, string stepvalue)
        {
            LiteralControl lit = new LiteralControl();
            lit.Text += "<div class=\"input-group\">";
            lit.Text += "<span class=\"input-group-addon\">" + string.Format("{0:#,###0}",int.Parse(param.MinValue)) + "</span>";
            lit.Text += string.Format("<input id=\"slider_{0}\" type=\"range\" min=\"{1}\" max=\"{2}\" step=\"{3}\" class=\"slider slider-displayHTML form-control\"/>",
                                                param.Name, param.MinValue, param.MaxValue, stepvalue);
            lit.Text += "<span class=\"input-group-addon\">" + string.Format("{0:#,###0}",int.Parse(param.MaxValue)) + "</span>";
            //lit.Text += "</div>";             // sitll miss this end div for txt control
            //lit.Text += param.MaxValue;
            return lit;
        }

        static private Control GenerateScriptSlideHTML(AMLParam param)
        {
            LiteralControl lit = new LiteralControl();
            lit.Text += "<script>";
            lit.Text += "document.getElementById(\"slider_{0}\").oninput = function(event){document.getElementById(\"{0}\").value = event.target.value};".Replace("{0}", param.Name);
            lit.Text += "document.getElementById(\"{0}\").onchange = function(event){document.getElementById(\"slider_{0}\").value = event.target.value};".Replace("{0}", param.Name);
            lit.Text += "</script>";

            return lit;
        }

        static private Control GenerateScriptSlideIE(AMLParam param)
        {
            LiteralControl lit = new LiteralControl();
            lit.Text += "<script>";
            lit.Text += "document.getElementById(\"slider_{0}\").onchange = function(event){document.getElementById(\"{0}\").value = event.target.value};".Replace("{0}", param.Name);
            lit.Text += "document.getElementById(\"{0}\").onchange = function(event){document.getElementById(\"slider_{0}\").value = event.target.value};".Replace("{0}", param.Name);
            lit.Text += "</script>";

            return lit;
        }

        static private Control GenerateScriptBarGraph(Dictionary<string, double> listOutputNumber)
        {
            LiteralControl lit = new LiteralControl();
            lit.Text += "<script>";
            lit.Text += "var freqData=[";
            int count = 1;
            foreach (var output in listOutputNumber)
            {
                if(string.IsNullOrEmpty(output.Key))
                    lit.Text += "{State:'" + count + "',freq:{low:" + output.Value.ToString() + "}},";
                else lit.Text += "{State:'" + output.Key + "',freq:{low:" + output.Value.ToString() + "}},";

                count++;
            }
            
            lit.Text += "];";
            lit.Text += "dashboard('#topLoader',freqData);";
            lit.Text += "</script>";

            return lit;
        }
    }
}