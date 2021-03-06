﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using VkNet;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace Wonderthing
{
	class Program
	{
		private static void Send(string path, long id)
		{
			string sendadr = "http://192.168.1.130:8080/signs";
			//string sendadr = "https://192.168.1.130:8080/signs";

			string FilePath = "";
			string vklink = "";

			FilePath = "C:/Users/Nata/Documents/Visual Studio 2015/Projects/laba_http_client/2";
			vklink = $"https://vk.com/id{id}";
			HttpsClient.SendVector(sendadr, vklink, path, EncruptAlg.EncAlgType.AES);
		}
		static void Main(string[] args)
		{
			Helper.Instance.InitApi("2c6caa918c7c3a20807d0d6d069c8b19fc830c7d35de8b9956351bcc89841773debd5ee7eb87fefc33588", 41558253);
			//Helper.Instance.InitApi("ten-ten-kun@yandex.ru", Console.ReadLine());
			Console.Clear();
			var res = Helper.Instance.GetUsersPhotos(28140268, true, 1);//26206689
			var client = new ServiceReference1.EightBallClient();
			foreach (var item in res)
			{
				var lst = item.Value;
				//lst.Item2.ForEach(e =>
				foreach(var e in lst.Item2)
				{
					var bytes = Helper.GetPhoto(e);
					File.WriteAllBytes($"photo\\{e.Id}.jpg", bytes);

					if (true)//(Helper.CheckPhoto($"photo\\{e.Id}.jpg"))
					{
						Console.WriteLine("get artem");
						var bytes2 = client.DownloadAndProcessPhoto(Helper.GetUrl(e), $"{e.Id}");
						if(bytes2 == null)
							continue;
						File.WriteAllBytes($"res\\{e.Id}", bytes2);
						try
						{
							Console.WriteLine("send kirill");
							Send($"res\\{e.Id}", item.Key);
						}
						catch (Exception ex)
						{
							Console.WriteLine($"Произошла ошибка при отправке данных\r\n{ex.Message}\r\nНажмите на любую кнопку для выхода...");
							Console.ReadKey();
							throw ex;
						}

					}
				}
			}

			#region r1
			//Process.Start("qt_cons.exe", "\"photo\\\" \"res\\\"").WaitForExit();


			//foreach (var item in res)
			//{
			//	var lst = item.Value;
			//	try
			//	{
			//		//lst.Item2.ForEach(e =>
			//		foreach (var e in lst.Item2)
			//		{
			//			if (Helper.CheckPhoto($"photo\\{e.Id}.jpg"))
			//			{
			//				try
			//				{
			//					Send($"photo\\{e.Id}.jpg", item.Key);
			//				}
			//				catch (Exception ex)
			//				{
			//					Console.WriteLine($"Произошла ошибка при отправке данных\r\n{ex.Message}\r\nНажмите на любую кнопку для выхода...");
			//					Console.ReadKey();
			//					break;
			//				}

			//			}

			//		}
			//	}
			//	catch (Exception ex)
			//	{
			//		break;
			//	}
			//}
#endregion
		}
	}
}
