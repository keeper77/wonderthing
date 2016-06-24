using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;
using System.Drawing;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using VkNet;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace Wonderthing
{
	public sealed class Helper
	{
		private static readonly Helper _instance = new Helper();
		private VkApi _apiWorker = null;// = new VkApi();
		static Helper() { }

		private Helper() { }

		public static Helper Instance
		{
			get
			{
				return _instance;
			}
		}

		public void InitApi(string login, string passwd, bool withCode = false)
		{
			_apiWorker = new VkApi();

			Func<string> code = () =>
			{
				Console.Write("Please enter code: ");
				string value = Console.ReadLine();

				return value;
			};
			if (withCode)
				_apiWorker.Authorize(new ApiAuthParams() { ApplicationId = 5394500, Login = login, Password = passwd, Settings = Settings.All, TwoFactorAuthorization = code });
			else
				_apiWorker.Authorize(new ApiAuthParams() { ApplicationId = 5394500, Login = login, Password = passwd, Settings = Settings.All });
		}

		public void InitApi(string token, long userId)
		{
			_apiWorker = new VkApi
			{
				UserId = userId,
				AccessToken = token,
				LastInvokeTime = DateTime.Now.AddMinutes(-5),
				_expireTimer = new Timer(new TimerCallback(state => { }), null, 900001, Timeout.Infinite)
			};

		}

		public List<User> GetUsers(int ageFrom = 16, int ageTo = 24, Sex gender=Sex.Unknown, string city = null, string universityName = null)
		{
			//var city = "Москва";
			//var universityName = "МИФИ";
			var countries = _apiWorker.Database.GetCountries(false, new List<Iso3166> { Iso3166.RU });
			var cities = _apiWorker.Database.GetCities(countries[0].Id, null, city);
			var universities = _apiWorker.Database.GetUniversities(countries[0].Id, cities[0].Id, universityName);

			var cnt = 0;
			var usrParams = new UserSearchParams()
			{
				AgeFrom = 16,
				AgeTo = 24,
				City = (int?)cities[0].Id,
				Sex = gender,
				University = (int?)universities[0].Id,
				UniversityCountry = (int?)countries[0].Id,
				Count = 1000,
				Offset = 0
			};

#warning Получение максимум первых 1000 человек!
			var res = _apiWorker.Users.Search(out cnt, usrParams);
			return res.ToList();
		}

		public static byte[] GetPhoto(Photo photo)
		{
			var url = photo.BigPhotoSrc?.AbsoluteUri ??
			          photo.Photo2560?.AbsoluteUri ??
			          photo.Photo1280?.AbsoluteUri ??
			          photo.Photo807?.AbsoluteUri ??
			          photo.Photo604?.AbsoluteUri ??
			          photo.Photo130?.AbsoluteUri ??
			          photo.Photo75?.AbsoluteUri ?? "";

			if (!string.IsNullOrWhiteSpace(url))
			{
				WebClient client = new WebClient();
				//client.Headers.Add("user-agent",
				//"Mozilla/5.0 (Linux; Android 5.0.1; SM-G9006V Build/AURORA) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/37.0.0.0 Mobile Safari/537.36");
				//client.Headers.Add("Host", "www.povarenok.ru");
				//client.Headers.Add("Connection", "keep-alive");
				client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
				client.Headers.Add("User-Agent",
					//"Mozilla/5.0 (Linux; Android 5.0.1; GT-I9500 Build/LRX22C) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/37.0.0.0 Mobile Safari/537.36");
					"Mozilla / 5.0(Macintosh; Intel Mac OS X 10_11_4) AppleWebKit / 601.5.17(KHTML, like Gecko) Version / 9.1 Safari / 601.5.17");
				client.Headers.Add("Accept-Encoding", "gzip,deflate");
				client.Headers.Add("Accept-Language", "ru-RU,en-US;q=0.8");
				//client.Headers.Add("X-Requested-With", "ru.mediafort.povarenok");
				var data = client.DownloadData(url);
				return data;
			}
			else
			{
				
			}
			return null;
		}

		public static string GetUrl(Photo photo)
		{
			return photo.BigPhotoSrc?.AbsoluteUri ??
					  photo.Photo2560?.AbsoluteUri ??
					  photo.Photo1280?.AbsoluteUri ??
					  photo.Photo807?.AbsoluteUri ??
					  photo.Photo604?.AbsoluteUri ??
					  photo.Photo130?.AbsoluteUri ??
					  photo.Photo75?.AbsoluteUri ?? "";
		}

		public Dictionary<long, Tuple<User, List<Photo>>> GetUsersPhotos(long userId, bool all = false, int maxDepth = 0, int curDepth = 0,  Dictionary<long, Tuple<User, List<Photo>>> curDict = null)
		{
			if (_apiWorker == null)
				return null;
			if (curDepth > maxDepth)
				return curDict;
			if (curDict == null)
				curDict = new Dictionary<long, Tuple<User, List<Photo>>>();

			if (curDict.ContainsKey(userId))
				return curDict;

			//var res = new Dictionary<long, Tuple<User, List<Photo>>>();
			//curDict?.ToList().ForEach(e=>res.Add(e.Key,e.Value));

			try
			{
				int c = 0;
				var usr = _apiWorker.Users.Get(userId, ProfileFields.All);
				List<Photo> photos;
				if (all)
				{
					photos = _apiWorker.Photo.GetAll(out c, new PhotoGetAllParams() {OwnerId = userId, Extended = true}).ToList();
				}
				else
				{
					photos =
					_apiWorker.Photo.GetUserPhotos(out c, new PhotoGetUserPhotosParams() { UserId = (ulong)userId, Extended = true })
						.ToList();
				}
				
				//var tags = _apiWorker.Photo.GetTags(photos[0].)
				var item = new Tuple<User, List<Photo>>(usr, photos);
				curDict.Add(usr.Id, item);

				if (curDepth == maxDepth)
					return curDict;
				var friendsList = _apiWorker.Friends.Get(new FriendsGetParams() {UserId = userId,Fields = ProfileFields.All});
				friendsList?.ToList().ForEach(fr => GetUsersPhotos(fr.Id, all, maxDepth, curDepth + 1, curDict));

			}
			catch (Exception ex)
			{
				
			}


			return curDict;
		}
		public VkApi ApiWorker => _apiWorker;
		private byte[] GetFBPhoto(string login)
		{
			try
			{
				using (var client = new WebClient())
				{
					//качаем только с домена фейсбука
					var path = login.StartsWith("https") && (login.Contains(".fb.com") || login.Contains(".facebook.com")) ? login :
						   login.StartsWith("https") ? "" : "https://www.facebook.com/" + login; //если пришла ссылка не на fb, то ничего не качаем
					if (!String.IsNullOrEmpty(path))
					{
						client.Headers[HttpRequestHeader.UserAgent] = "Chrome/46.0.2490.86";
						var data = client.DownloadData(path);
						string webData = System.Text.Encoding.UTF8.GetString(data);
						var classInd = webData.IndexOf("profilePicThumb");
						var httpsInd = webData.IndexOf("https", classInd);
						var quoteInd = webData.IndexOf("\"", httpsInd);
						string reference = webData.Substring(httpsInd, quoteInd - httpsInd).Replace("&amp;", "&");
						return client.DownloadData(reference);
					}
					else
					{
						return new byte[] { };
					}
				}
			}
			catch (Exception)
			{
				return new byte[] { };
			}

		}

		public static bool CheckPhoto(string path)
		{
			Image<Bgr, Byte> My_Image = new Image<Bgr, byte>(path);
			var cs = new CascadeClassifier("Resources\\new.xml");
			Size s1 = new Size(100, 100);
			Size s2 = new Size(400, 400);
			var rects = cs.DetectMultiScale(new Image<Gray, byte>(path), 1.05, 8, s1, s2).ToArray();
			Bgr a = new Bgr(Color.Fuchsia);
			try
			{
				foreach (var item in rects)
				{
					My_Image.Draw(item, a, 1);

				}
			}
			catch (Exception) { }
			//pictureBox1.Image = My_Image.ToBitmap();
			return rects.Count() != 0;
		}
	}
}
