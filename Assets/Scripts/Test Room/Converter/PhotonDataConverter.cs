using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

public static class PhotonDataConverter
{
	public static byte[] Serialize_ObjTypeToByte<T>(T obj)
	{
		string json = JsonSerializer.Serialize(obj);
		return Encoding.UTF8.GetBytes(json);
	}

	public static T Deserialize_ByteToObjType<T>(byte[] data)
	{
		string json = Encoding.UTF8.GetString(data);
		return JsonSerializer.Deserialize<T>(json);
	}
}
