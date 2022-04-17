using StrawberryShake.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace peter_ficsit_api
{

    //dynamic upload = (chunk: blob, chunkId: index, versionID: VersionID);
    //string int string

    public class UploadData
    {
        [JsonPropertyName("chunk")]
        public byte[] chunk;
        [JsonPropertyName("chunkId")]
        public int chunkId;
        [JsonPropertyName("versionID")]
        public string versionID;
    }

    public class UploadSerializer : ScalarSerializer<UploadData>
    {
        public UploadSerializer(): base("Upload")
        {
        }
        //public override UploadData Parse(string serializedValue)
        //{            
        //    // handle the serialization of the JsonElement
        //    //UploadData data = new UploadData();
        //    //data.chunk = serializedValue.GetProperty("chunk").GetBytesFromBase64();
        //    //data.chunkId = serializedValue.GetProperty("chunkId").GetInt32();
        //    //data.versionID = serializedValue.GetProperty("versionID").GetString();
        //    return System.Text.Json.JsonSerializer.Deserialize<UploadData>(serializedValue);
        //}
        //protected override string Format(UploadData runtimeValue)
        //{
        //    // handle the serialization of the runtime representation in case
        //    // the scalar is used as a variable.
        //    return System.Text.Json.JsonSerializer.Serialize(runtimeValue);
        //}
    }
}
