using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ParameterIO
{
    public class AMLParameterObject
    {
        [XmlElement("Url")]
        public string Url;

        [XmlElement("APIKey")]
        public string APIKey;

        [XmlElement("Title")]
        public string Title;

        [XmlElement("Description")]
        public string Description;

        [XmlElement("InputParameter")]
        public List<AMLParam> listInputParameter = new List<AMLParam>();

        [XmlElement("OutputParameter")]
        public List<AMLParam> listOutputParameter = new List<AMLParam>();

        [XmlElement("Copyright")]
        public string Copyright = "";

        public string ReadSwagger()
        {
            try
            {
                string workspaceID = ExtractString(Url, "/workspaces/", "/").Trim('/');
                if (string.IsNullOrEmpty(workspaceID))
                    return "Extract workspaces ID Error!!! Please check the API Post URL";
                string serviceID = ExtractString(Url, "/services/", "/").Trim('/');
                if (string.IsNullOrEmpty(serviceID))
                    return "Extract service ID Error!!! Please check the API Post URL";

                

                return ReadSwagger(workspaceID, serviceID);
            }
            catch (Exception)
            {
                return "Read Swagger Error !!!";
            }
        }

        List<string> GetPaths(string json, string ExecuteionPath)
        {
            List<string> listOutputPath = new List<string>();
            JToken outputToken = JObject.Parse(json).SelectToken(ExecuteionPath);//"definitions.ExecutionOutputs");
            JToken outputTokenPropertise = outputToken["properties"];
            if (outputTokenPropertise == null) return null;

            foreach (var outputvar in JObject.Parse(outputTokenPropertise.ToString()))
            {
                if (outputvar.Value["items"]["$ref"] != null)
                    listOutputPath.Add(outputvar.Value["items"]["$ref"].ToString().Replace("#/", "").Replace("/", "."));
            }
            return listOutputPath;
        }

        public string ReadSwagger(string workspace, string webservice)
        {
            this.listInputParameter.Clear();
            this.listOutputParameter.Clear();
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                System.Net.WebClient wc = new System.Net.WebClient();
                wc.Encoding = System.Text.Encoding.UTF8;

                //string jsonString = wc.DownloadString(string.Format("https://management-tm.azureml.net/workspaces/{0}/webservices/{1}/endpoints/{2}/apidocument", workspace, webservice, "default"));
                //string jsonString = wc.DownloadString(string.Format("https://ussouthcentral.services.azureml-int.net/workspaces/{0}/services/{1}/swagger.json", workspace, webservice));
                string jsonString = wc.DownloadString(string.Format("https://ussouthcentral.services.azureml.net/workspaces/{0}/services/{1}/swagger.json", workspace, webservice));

                try { this.Title = JObject.Parse(jsonString).SelectToken("info.title").ToString(); }
                catch (Exception) { return "Cannot get Service Name !!!"; }
                try { this.Description = JObject.Parse(jsonString).SelectToken("info.description").ToString(); }
                catch (Exception) { }

                List<string> inputPaths, outputPaths;
                try { inputPaths = GetPaths(jsonString, "definitions.ExecutionInputs"); }
                catch (Exception) { return "Cannot get List of Input Paramters !!!"; }
                try { outputPaths = GetPaths(jsonString, "definitions.ExecutionOutputs"); }
                catch (Exception) { return "Cannot get List of Output Paramters !!!"; }


                if (inputPaths != null)
                    foreach (string inputP in inputPaths)
                        this.listInputParameter.AddRange(ParseMLParmeter(jsonString, inputP));

                if (outputPaths != null)
                    foreach (string outputP in outputPaths)
                        this.listOutputParameter.AddRange(ParseMLParmeter(jsonString, outputP));

                if (listInputParameter == null || listOutputParameter == null)
                    return "";

                if (listOutputParameter.Count <= listInputParameter.Count)
                    return "";
                // Diable repeat input parameter from Output
                for (int i = 0; i < listInputParameter.Count; i++)
                    this.listOutputParameter[i].Enable = false;

                return "";
            }
            catch (Exception)
            {
                return "Get Service Document Error !!! Please check the API Post URL";
            }

        }

        string ExtractString(string full, string start, string end)
        {
            int id1 = full.IndexOf(start);
            if (id1 == -1) return "";
            int id2 = full.IndexOf(end, id1 + start.Length + 1);
            if (id2 == -1) return "";
            return full.Substring(id1 + start.Length, id2 - id1 - start.Length);
        }

        List<AMLParam> ParseMLParmeter(string jsonStr, string ParameterXPath)
        {
            try
            {
                List<AMLParam> result = new List<AMLParam>();
                var objects = JObject.Parse(jsonStr);
                JToken jtoken = objects.SelectToken(ParameterXPath);

                JArray jarrRequired = JArray.Parse(jtoken["required"].ToString());
                JObject ParameterObj = JObject.Parse(jtoken["properties"].ToString());

                var listRequired = jarrRequired.ToList();

                foreach (var obj in ParameterObj)
                {
                    AMLParam param = new AMLParam();
                    param.Name = obj.Key;
                    param.Type = obj.Value["type"].ToString();
                    param.Format = obj.Value["format"] != null ? obj.Value["format"].ToString() : "";
                    param.Description = obj.Value["description"] != null ? obj.Value["description"].ToString() : "";
                    param.DefaultValue = obj.Value["default"] != null ? obj.Value["default"].ToString() : "";


                    // Parse Description string to get Alias
                    string[] desSplit = param.Description.Split('|');
                    if (desSplit.Length == 2)
                    {
                        param.Alias = desSplit[0];
                        param.Description = desSplit[1];
                    }

                    if (param.Type == "bool")
                    {
                        param.StrEnum.Add("true");
                        param.StrEnum.Add("false");
                    }

                    // Fill list Emum and set Default Value
                    if (obj.Value["enum"] != null)
                    {
                        JArray strEnum = JArray.Parse(obj.Value["enum"].ToString());
                        param.StrEnum = strEnum.ToObject<List<string>>();
                        if (string.IsNullOrEmpty(param.DefaultValue)) param.DefaultValue = param.StrEnum[0];
                    }
                    else
                    {
                        if (param.Type == "integer" || param.Type == "number")
                            if (string.IsNullOrEmpty(param.DefaultValue)) param.DefaultValue = "0";
                            else if (param.Type == "bool")
                                if (string.IsNullOrEmpty(param.DefaultValue)) param.DefaultValue = "true";
                    }

                    result.Add(param);
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool ExportInputParameter(string OutputPath)
        {
            try
            {

                var appendMode = false;
                var encoding = Encoding.GetEncoding("UTF-8");
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                XmlSerializer serializer = new XmlSerializer(typeof(AMLParameterObject));
                using (StreamWriter writer = new StreamWriter(OutputPath, appendMode, encoding))
                {
                    serializer.Serialize(writer, this, ns);
                    //System.IO.File.WriteAllText(sf.FileName,writer.ToString());
                };
                return true;
            }
            catch (Exception)
            { return false; }
        }

        public bool ImportInputParameter(string InputPath)
        {
            TextReader reader = new StreamReader(InputPath);
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(AMLParameterObject));

                object obj = deserializer.Deserialize(reader);
                var amlObj = (AMLParameterObject)obj;

                this.Url = amlObj.Url;
                this.APIKey = amlObj.APIKey;
                this.Title = amlObj.Title;
                this.Description = amlObj.Description;
                this.Copyright = amlObj.Copyright;
                this.listInputParameter = amlObj.listInputParameter;
                this.listOutputParameter = amlObj.listOutputParameter;

                reader.Close();
                return true;
            }
            catch (Exception)
            {
                reader.Close();
                return false;
            }


        }

    }
}