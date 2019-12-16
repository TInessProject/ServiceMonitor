using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;


public enum HttpVerb
{
    GET,
    POST,
    PUT,
    DELETE
}


namespace ServiceMonitor.Models
{

    public class RestClient
    {
        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }
        public string AuthorizationBearer { get; set; }

        

        public RestClient()
        {
            EndPoint = "";
            Method = HttpVerb.GET;
            ContentType = "text/xml";
            PostData = "";
        }
        public RestClient(string endpoint)
        {
            EndPoint = endpoint;
            Method = HttpVerb.GET;
            ContentType = "text/xml";
            PostData = "";
        }
        public RestClient(string endpoint, HttpVerb method)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "text/xml";
            PostData = "";
        }

        public RestClient(string endpoint, HttpVerb method, string postData)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "text/xml";
            PostData = postData;
        }

        
        public string MakeRequest()
        {
            return MakeRequest("");
        }
        

        public string MakeRequest(string parameters)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);
                HttpWebRequest.DefaultWebProxy = null;

                /*inicio: utilizando certificado digital*/
                //string certificateFile = @"C:\Certificado\certunimedbelemrkp.pfx";
                string certificateFile = @"Certificado/certunimedbelemrkp.pfx";
                certificateFile = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, certificateFile));

                /*inicio: salva e um diretório*/

                string path = @"C:\ServiceMonitor\DiretorioCertificado.txt";
                var file = new FileInfo(path);
                file.Directory.Create(); //If the directory already exists, this method does nothing.
                //salva o arquivo fisicamente
                System.IO.File.WriteAllText(path, certificateFile);
                /*fim: salva e um diretório*/

                string certificateFilePass = "Un1m3d088@";
                request.ClientCertificates.Add(new X509Certificate2(certificateFile, certificateFilePass));
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                /*fim: utilizando certificado digital*/

                /*inicio: Barear Token */
                if (AuthorizationBearer != "")
                {
                    //request.PreAuthenticate = true;
                    request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + AuthorizationBearer);
                }
                /*fim: Barear Token*/

                request.Method = Method.ToString();
                request.ContentLength = 0;
                request.ContentType = ContentType;
                
                if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
                {
                    var encoding = new UTF8Encoding();
                    var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
                    request.ContentLength = bytes.Length;
                    request.ServicePoint.Expect100Continue = true;

                    using (var writeStream = request.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var responseValue = string.Empty;
                    
                    if ( (response.StatusCode != HttpStatusCode.OK) && response.StatusCode != HttpStatusCode.Accepted)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        throw new ApplicationException(message);
                    }

                    // grab the response
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                    }

                    return responseValue;
                }

            }catch (Exception ex)
            {
                Dictionary<string, object> error = new Dictionary<string, object>()
                                                        {
                                                            { "OutErro", 1 },
                                                            { "OutMensagem", "" + ex.Message + " (cod(" + "ND" + "):RestClient)" }
                                                        };
                string json = JsonConvert.SerializeObject(error);
                //string json = @"{ ""status"" : ""ERROR"", ""mensage"" : """+ex.Message+ """" + "}";
                return json;
            }

        }


        public string MakeRequest2(string parameters)
        {
            //host = @"https://localhost/";
            string host = EndPoint + parameters;
            //string certName = @"C:\\Certificado\\synergyius_santander.pem";
            string certName = @"E:\Projetos\Synergius-Backend\M0\Models\synergyius_santander.pem";
            string password = @"sy#Int@Sant##23";

            try
            {
                X509Certificate2Collection certificates = new X509Certificate2Collection();
                certificates.Import(certName, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

                ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(host);
                req.AllowAutoRedirect = true;
                req.ClientCertificates = certificates;
                req.Method = "POST";
                req.ContentType = "application/json";
                string postData = "login-form-type=cert";
                byte[] postBytes = Encoding.UTF8.GetBytes(postData);
                req.ContentLength = postBytes.Length;

                Stream postStream = req.GetRequestStream();
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Flush();
                postStream.Close();
                WebResponse resp = req.GetResponse();

                Stream stream = resp.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        Console.WriteLine(line);
                        line = reader.ReadLine();
                    }
                }

                stream.Close();

                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }








        

        /*

        public HttpResponseDto MakeHttpsGetRequest(HttpRequestDto requestDto)
        {
            HttpResponseDto responseDto = new HttpResponseDto();
            try
            {
                //Creating the X.509 certificate.
                X509Certificate2 certificate = new X509Certificate2(requestDto.CertificatePath, requestDto.CertificatePassword);
                //Initialize HttpWebRequest.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestDto.RequestUri);
                //Set the Timeout.
                request.Timeout = requestDto.TimeoutMilliseconds;
                //Add certificate to request.
                request.ClientCertificates.Add(certificate);
                //UserAgent.
                request.UserAgent = requestDto.UserAgent;
                //HTTP verb.
                request.Method = "GET";
                //Get HttpWebResponse.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                GetHttpResponseProperties(response, responseDto);
                responseDto.IsException = false;
            }
            catch (Exception ex)
            {
                responseDto.IsException = true;
                responseDto.ExceptionType = ex.GetType().ToString();
                responseDto.ExceptionMessage = ex.Message;
                responseDto.ExceptionToString = ex.ToString();
                //Used for debugging certificate path.
                //responseDto.ExceptionMessage += " Certificate path: "+requestDto.CertificatePath;
                if (ex.InnerException != null)
                    responseDto.ExceptionMessage += " InnerException: " + ex.InnerException.Message;
                //It seems likes HttpWebRequest.GetResponse() throws an exception a to anyhthing else then a StatusCode 200, OK.
                //"Generally HttpWebRequest treats all non-success (200) codes to be exceptions by design."
                //http://connect.microsoft.com/VisualStudio/feedback/details/258168/httpwebrequest-throws-inappropriate-exceptions
                //http://stackoverflow.com/questions/692342/net-httpwebrequest-getresponse-raises-exception-when-http-status-code-400-ba
                //So to (400) Bad Request, (404) Not Found, etc. HttpWebRequest.GetResponse() will, by default, throw a System.Net.WebException.
                //WebException contains an HttpWebResponse object, (WebException.Response Property), with all the normal HTTP Response properties
                //like: Headers, StatusCode, Message Body, etc. So we extract it below.
                if (ex is WebException)
                {
                    //Casting Exception to webException.
                    WebException webException = (WebException)ex;
                    //We can extract HttpWebResponse from WebResponse.
                    WebResponse webResponse = webException.Response;
                    if (webResponse != null)
                    {
                        //Casting WebException.Response to HttpWebResponse.
                        HttpWebResponse httpWebResponse = (HttpWebResponse)webException.Response;
                        GetHttpResponseProperties(httpWebResponse, responseDto);
                    }
                }
                //As discussed above, HttpWebRequest.GetResponse() throws an exception (WebException) to anyhthing else then a StatusCode 200.
                //But we want to handle other exceptions out in the code, therefore we rethrow the exception if it's not WebException.
                else
                {
                    throw ex;
                }
            }
            return responseDto;
        }



        */




        public void MakeRequest3(string parameters)
        {
            /*
            var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(request.Content.OuterXml.ToString());
            string password = "XXXX";
            X509Certificate2 cert = new X509Certificate2("c:\\zzzz.p12", password);
            string key = cert.GetPublicKeyString();
            string certData = Encoding.ASCII.GetString(cert.Export(X509ContentType.Cert));

            Uri uri = new Uri(request.Url);
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(uri);
            myRequest.Credentials = new NetworkCredential(request.User, request.Password.ToString());
            myRequest.Method = "PUT";
            myRequest.ContentType = request.ContentType;
            myRequest.ContentLength = data.Length;
            myRequest.ClientCertificates.Add(cert);

            Stream newStream = myRequest.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            System.IO.StreamReader st = new StreamReader(((HttpWebResponse)myRequest.GetResponse()).GetResponseStream());
            */
        }
        






    } // class




}
