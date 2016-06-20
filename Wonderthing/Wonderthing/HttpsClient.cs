using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Wonderthing
{
	// serializable class forwarding "feature vector" from Openbr and link to the user id in the social network.
	public class OpbrData
	{
		public string link { get; set; }
		public string base64string { get; set; }

		public OpbrData() { }

		public OpbrData(string link, string str)
		{
			this.link = link;
			base64string = str;
		}
	}

	public class EncruptAlg
	{
		public enum EncAlgType
		{
			AES,
			RSA,
			MAGMA
		}
		public static string keystr = "abcabcabcaabcabc";
		public static byte[] key = Encoding.UTF8.GetBytes(keystr);

		public static byte[] Encrypt(byte[] ENC, byte[] AES_KEY)
		{
			byte[] tmp;
			byte[] buf;
			byte[] AES_IV;
			using (Aes AES = Aes.Create())
			{
				AES.KeySize = 256;
				AES.BlockSize = 128;
				AES.Key = AES_KEY;
				AES.GenerateIV();
				AES_IV = AES.IV;
				AES.Padding = PaddingMode.ANSIX923;  //!!!The ANSIX923 padding string consists of a sequence of bytes filled with zeros before the length. 

				MemoryStream MS = new MemoryStream();
				CryptoStream CS = new CryptoStream(MS, AES.CreateEncryptor(AES.Key, AES.IV), CryptoStreamMode.Write);

				CS.Write(ENC, 0, ENC.Length);
				CS.Close();

				tmp = MS.ToArray();
				buf = new byte[tmp.Length + 16];
				AES_IV.CopyTo(buf, 0);
				tmp.CopyTo(buf, 16);
				return buf;
			}
		}
		public static byte[] Decrypt(byte[] DEC, byte[] AES_KEY)
		{
			byte[] AES_IV = new byte[16];
			byte[] tmp = new byte[DEC.Length - 16];
			for (int i = 0; i < 16; i++)
			{
				AES_IV[i] = DEC[i];
			}
			for (int i = 0; i < DEC.Length - 16; i++)
			{
				tmp[i] = DEC[i + 16];
			}
			using (Aes AES = Aes.Create())
			{
				AES.KeySize = 256;
				AES.BlockSize = 128;
				AES.Key = AES_KEY;
				AES.IV = AES_IV;
				AES.Padding = PaddingMode.ANSIX923;

				MemoryStream MS = new MemoryStream();
				CryptoStream CS = new CryptoStream(MS, AES.CreateDecryptor(AES.Key, AES.IV), CryptoStreamMode.Write);

				CS.Write(tmp, 0, tmp.Length);
				CS.Close();

				return MS.ToArray();
			}
		}

	}

	public static class HttpsClient
	{
		// public static ManualResetEvent allDone = new ManualResetEvent(false);

		const string login = "login";
		const string password = "password";
		const bool flag = false;


		public static void SendVector(string sendadr, string vklink, string FilePath, EncruptAlg.EncAlgType etype)
		{
			WebResponse Subsystem2Ansver;
			string filejson = "";
			string evklink = "";
			try
			{
				switch (etype)
				{
					case EncruptAlg.EncAlgType.AES:
						if (flag)
						{
							AES newaes = new AES();
							filejson = Convert.ToBase64String(newaes.AES_Encrypt(createByteArr(FilePath), EncruptAlg.key));
							evklink = Convert.ToBase64String(newaes.AES_Encrypt(Encoding.UTF8.GetBytes(vklink), EncruptAlg.key));
						}
						else
						{
							filejson = Convert.ToBase64String(EncruptAlg.Encrypt(createByteArr(FilePath), EncruptAlg.key));
							evklink = Convert.ToBase64String(EncruptAlg.Encrypt(Encoding.UTF8.GetBytes(vklink), EncruptAlg.key));
						}
						break;
					case EncruptAlg.EncAlgType.RSA:
						RSA newrsa = new RSA();
						filejson = Convert.ToBase64String(Encoding.UTF8.GetBytes(newrsa.EncryptData(EncruptAlg.keystr, clearText(FilePath))));
						evklink = Convert.ToBase64String(Encoding.UTF8.GetBytes(newrsa.EncryptData(EncruptAlg.keystr, vklink)));
						break;
					//printLog("Error! You try to use an invalid encryption algorithm - RSA");
					//throw new ArgumentNullException("No encription alg");
					case EncruptAlg.EncAlgType.MAGMA:
						printLog("Error! You try to use an invalid encryption algorithm - MAGMA");
						throw new ArgumentNullException("No encription alg");
					default:
						throw new ArgumentNullException("No encription alg");
				}
				OpbrData opbrdata = new OpbrData(evklink, filejson);
				string serialized = JsonConvert.SerializeObject(opbrdata);
				Subsystem2Ansver = HttpPost(sendadr, serialized); // здесь сейчас стоит синхранная передача одного пост запроса
																  // если нужна асинхронная, то надо вместо этой строчки впилить все из функции AsynchPost(). 
																  // ее я не проверяла, ты лучше меня разбираешься со всеми этими делигатами и асинхронностью)))
				printLog("Subsystem2Ansver: authorization " + Subsystem2Ansver.ToString());
			}
			catch (WebException ex)
			{
				WebExceptionStatus status = ex.Status;
				if (status == WebExceptionStatus.ProtocolError)
				{
					HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
					printLog("The error status code:" + (int)httpResponse.StatusCode + " " + httpResponse.StatusDescription);
				}
				throw ex;
			}
			catch (Exception e)
			{
				printLog(e.Message);
				throw e;
			}
		}


		public static string HttpGet(string URI)
		{
			System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
			System.Net.WebResponse resp = req.GetResponse();
			System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
			return sr.ReadToEnd().Trim();
		}

		public static WebResponse HttpPost(string URI, string jsonstr)
		{
			System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
			req.ContentType = "application/json";  //"application/x-www-form-urlencoded"
			req.Method = "POST";
			req.Headers.Add("login:" + Convert.ToBase64String(Encoding.UTF8.GetBytes("login")));
			req.Headers.Add("password:" + Convert.ToBase64String(Encoding.UTF8.GetBytes("password")));
			//req.Headers.Add("login:" + Convert.ToBase64String(EncruptAlg.Encrypt(Encoding.UTF8.GetBytes("login"), EncruptAlg.key)));
			//req.Headers.Add("password:" + Convert.ToBase64String(EncruptAlg.Encrypt(Encoding.UTF8.GetBytes("password"), EncruptAlg.key)));
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonstr);
			req.ContentLength = bytes.Length;
			System.IO.Stream os = req.GetRequestStream();
			os.Write(bytes, 0, bytes.Length);
			os.Close();
			System.Net.WebResponse resp = req.GetResponse();
			System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
			return resp;
		}

		//возвращает размер файла по ссылке если он существует, иначе ошибка.
		public static int findFileSize(string readPath)
		{
			int FileSize = 0;
			FileInfo FileInf = new FileInfo(readPath);
			if (FileInf.Exists)
			{ FileSize = (int)FileInf.Length; }
			else { throw new ArgumentNullException("Opbr file size = NULL"); }
			return FileSize;
		}

		//считывает все данные бинарного файла по ссылке в массив byte[]
		public static byte[] createByteArr(string readPath)
		{
			byte[] FileClearData;
			int fsz = findFileSize(readPath);
			FileClearData = new byte[fsz];
			using (BinaryReader reader = new BinaryReader(File.Open(readPath, FileMode.Open)))
			{
				FileClearData = reader.ReadBytes(fsz);
			}
			return FileClearData;
		}
		public static string clearText(string readPath)
		{
			string FileClearData;
			int fsz = findFileSize(readPath);
			//FileClearData = new byte[fsz];
			using (BinaryReader reader = new BinaryReader(File.Open(readPath, FileMode.Open)))
			{
				FileClearData = reader.ReadChars(fsz).ToString();
			}
			return FileClearData;
		}
		// заносит строку mes в лог файл.
		public static void printLog(string mes)
		{

			string logPath = "log.txt";
			string logMessage = "# " + "HttpsClient: " + DateTime.Now + " ";
			System.IO.File.AppendAllText(logPath, logMessage + mes + "\n");
		}

	}
}
