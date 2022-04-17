using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using peter_ficsit_api;
using peter_ficsit_api.GraphQL;
using StrawberryShake;
using Ificsit_api = peter_ficsit_api.GraphQL.Ificsit_api;

namespace peter_ficsit_api_implement
{
    public class UploadFileImplem
    {
        public static bool UploadFile(string ModID, string filepath, VersionStabilities stabilities, string Changelog)
        {
            var client = ClientBuilder.GetClient();

            string VersionID = "";

            if (VersionID.Length < 2)
            {
                var create = client.CreateVersion.ExecuteAsync(ModID).GetAwaiter().GetResult();
                create.EnsureNoErrors();
                VersionID = create.Data.CreateVersion;

                if (VersionID == null)
                {
                    return false; //failed
                }
            }

            Console.WriteLine("CreateVersion " + VersionID);

            if (!UploadFileChunked(client, ModID, VersionID, filepath, StaticOptions.APIURL, StaticOptions.APIKEY))
            {
                return false;
            }

            NewVersion NewVersion = new NewVersion()
            {
                Changelog = Changelog,
                Stability = stabilities
            };

            var createFinal = client.FinalizeCreateVersion.ExecuteAsync(ModID, VersionID, NewVersion).GetAwaiter().GetResult();
            createFinal.EnsureNoErrors();
            if (!createFinal.Data.FinalizeCreateVersion)
            {
                return false; //failed
            }
            Console.WriteLine("FinalizeCreateVersion " + createFinal.Data.FinalizeCreateVersion);

            Console.WriteLine($"UPLOADED FILE! {VersionID} was uploaded!");

            return true;
        }

        private static bool UploadFileChunked(Ificsit_api api, string ModID, string VersionID, string inputFile, string APIURL, string AuthHeaderValue)
        {
            const int chunkSize = 10000000;
            byte[] buffer = new byte[chunkSize];

            string path = Directory.CreateDirectory("temp").CreateSubdirectory(VersionID).FullName;
            List<string> files = new List<string>();

            string filename = Path.GetFileName(inputFile);
            long lenRemaining = 0;
            using (Stream input = File.OpenRead(inputFile))
            {
                Console.WriteLine("Length " + input.Length);
                lenRemaining = input.Length;
                int index = 0;
                while (input.Position < input.Length)
                {
                    int chunkBytesRead = 0;
                    {
                        chunkBytesRead = 0;
                        while (chunkBytesRead < chunkSize)
                        {
                            int bytesRead = input.Read(buffer,
                                                       chunkBytesRead,
                                                       chunkSize - chunkBytesRead);

                            if (bytesRead == 0)
                            {
                                break;
                            }
                            chunkBytesRead += bytesRead;
                        }
                    }
                    Console.WriteLine("Part " + (index + 1));
                    Console.WriteLine("Read " + chunkBytesRead);
                    lenRemaining = lenRemaining - chunkBytesRead;
                    Console.WriteLine("lenRemaining " + lenRemaining);

                    string operations = "{ \"query\": \"mutation ($file: Upload!) { uploadVersionPart(modId: \\\""+ ModID +"\\\", versionId: \\\""+ VersionID +"\\\", part: "+ (index + 1) +", file: $file) }\", \"variables\": { \"file\": null } }";
                    string map = "{ \"0\": [\"variables.file\"] }";
                    string filepath = path + "\\" + filename; // + "." + index

                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(AuthHeaderValue);

                    var data = new MultipartFormDataContent();

                    data.Add(new ByteArrayContent(Encoding.ASCII.GetBytes(operations)), "operations");
                    data.Add(new ByteArrayContent(Encoding.ASCII.GetBytes(map)), "map");

                    var fileValue = new StreamContent(new MemoryStream(buffer, 0, chunkBytesRead));
                    // add the name and meta-data 
                    data.Add(fileValue, "0", filename);
                    
                    HttpResponseMessage response =  client.PostAsync(APIURL, data).GetAwaiter().GetResult();
                    var code = response.StatusCode;
                    Console.WriteLine(code.ToString());
                    HttpContent responseContent = response.Content;

                    using (var reader = new StreamReader(responseContent.ReadAsStream()))
                    {
                        string json = reader.ReadToEnd();
                        Console.WriteLine(json);
                        if (code != HttpStatusCode.OK) return false;

                        //Process Response
                        if (json.StartsWith("{\"data\":"))
                        {
                            json = json.Replace("{\"data\":{", "").TrimEnd('}');
                            if (json.StartsWith("\"uploadVersionPart\"") && json.Contains("\"uploadVersionPart\":true"))
                            {
                                //SUCCESS
                            }
                        }
                        else
                        {
                            return false; // Bad Result?
                        }

                    }

                    if (code != HttpStatusCode.OK) return false;

                    index++;
                    Thread.Sleep(500); // experimental; perhaps try it
                }
            }
            return true;
        }

    }
}
