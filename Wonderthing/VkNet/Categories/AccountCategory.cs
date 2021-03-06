﻿using System;
using System.Collections.ObjectModel;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;

namespace VkNet.Categories
{
	using System.Collections.Generic;
	using JetBrains.Annotations;
	using Model;
	using Model.RequestParams;
	using Utils;

	/// <summary>
	/// Методы этого класса позволяют производить действия с аккаунтом пользователя.
	/// </summary>
	[Serializable]
	public partial class AccountCategory
	{
		/// <summary>
		/// API.
		/// </summary>
		readonly VkApi _vk;

		/// <summary>
		/// Методы для работы с аккаунтом пользователя.
		/// </summary>
		/// <param name="vk">API.</param>
		internal AccountCategory(VkApi vk)
		{
			_vk = vk;
		}

		/// <summary>
		/// Возвращает ненулевые значения счетчиков пользователя.
		/// </summary>
		/// <param name="filter">Счетчики, информацию о которых нужно вернуть (friends, messages, photos, videos, notes, gifts, events, groups, notifications, sdk, app_requests).
		/// sdk - возвращает количество запросов в приложениях.
		/// app_requests - возвращает количество непрочитанных запросов в приложениях. список слов, разделенных через запятую (Список слов, разделенных через запятую).</param>
		/// <returns>
		/// Возвращает объект, который может содержать поля friends, messages, photos, videos, notes, gifts, events, groups, notifications, sdk, app_requests.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.getCounters" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public Counters GetCounters(CountersFilter filter)
		{
			return _vk.Call("account.getCounters", new VkParameters { { "filter", filter } }, true);
		}

		/// <summary>
		/// Устанавливает короткое название приложения (до 17 символов), которое выводится пользователю в левом меню.
		/// </summary>
		/// <param name="userId">Идентификатор пользователя. положительное число, по умолчанию идентификатор текущего пользователя, обязательный параметр (Положительное число, по умолчанию идентификатор текущего пользователя, обязательный параметр).</param>
		/// <param name="name">Короткое название приложения. строка (Строка).</param>
		/// <returns>
		/// Возвращает 1 в случае успешной установки короткого названия.
		/// Если пользователь не установил приложение в левое меню, метод вернет ошибку 148 (Access to the menu of the user denied). Избежать этой ошибки можно с помощью метода account.getAppPermissions.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.setNameInMenu" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public bool SetNameInMenu([NotNull] string name, long? userId = null)
		{
			VkErrors.ThrowIfNullOrEmpty(() => name);
			var parameters = new VkParameters
			{
				{ "name", name },
				{ "user_id", userId}
			};
			return _vk.Call("account.setNameInMenu", parameters, true);
		}

		/// <summary>
		/// Помечает текущего пользователя как online на 15 минут.
		/// </summary>
		/// <param name="voip">Возможны ли видеозвонки для данного устройства флаг, может принимать значения 1 или 0 (Флаг, может принимать значения 1 или 0).</param>
		/// <returns>
		/// В случае успешного выполнения метода будет возвращён код 1.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.setOnline" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public bool SetOnline(bool? voip = null)
		{
			var parameters = new VkParameters { { "voip", voip } };
			return _vk.Call("account.setOnline", parameters);
		}

		/// <summary>
		/// Помечает текущего пользователя как offline.
		/// </summary>
		/// <returns>
		/// В случае успешного выполнения возвращает <c>true</c>.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.setOffline" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public bool SetOffline()
		{
			return _vk.Call("account.setOffline", VkParameters.Empty);
		}

		/// <summary>
		/// Позволяет искать пользователей ВКонтакте, используя телефонные номера, email-адреса, и идентификаторы пользователей в других сервисах. Найденные пользователи могут быть также в дальнейшем получены методом friends.getSuggestions.
		/// </summary>
		/// <param name="contacts">Список контактов, разделенных через запятую. список слов, разделенных через запятую (Список слов, разделенных через запятую).</param>
		/// <param name="service">Строковой идентификатор сервиса, по контактам которого производится поиск. Может принимать следующие значения: (email, phone, twitter, facebook, odnoklassniki, instagram, google) строка, обязательный параметр (Строка, обязательный параметр).</param>
		/// <param name="mycontact">Контакт текущего пользователя в заданном сервисе. строка (Строка).</param>
		/// <param name="returnAll">1 – возвращать также контакты, найденные ранее с использованием этого сервиса, 0 – возвращать только контакты, найденные с использованием поля contacts. флаг, может принимать значения 1 или 0 (Флаг, может принимать значения 1 или 0).</param>
		/// <param name="fields">Список дополнительных полей, которые необходимо вернуть.
		/// Доступные значения: nickname, domain, sex, bdate, city, country, timezone, photo_50, photo_100, photo_200_orig, has_mobile, contacts, education, online, relation, last_seen, status, can_write_private_message, can_see_all_posts, can_post, universities список слов, разделенных через запятую (Список слов, разделенных через запятую).</param>
		/// <returns>
		/// В качестве результата метод возвращает два списка:
		/// found – список объектов пользователей, расширенных полями contact – контакт, по которому был найден пользователь (не приходит если пользователь был найден при предыдущем использовании метода), request_sent – запрос на добавление в друзья уже был выслан, либо пользователь уже является другом, common_count если этот контакт также был импортирован друзьями или контактами текущего пользователя. Метод также возвращает найденные ранее контакты.
		/// other – список контактов, которые не были найдены. Объект содержит поля contact и common_count если этот контакт также был импортирован друзьями или контактами текущего пользователя.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.lookupContacts" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public LookupContactsResult LookupContacts(List<string> contacts, Services service, string mycontact = null, bool? returnAll = null, UsersFields fields = null)
		{
			var parameters = new VkParameters
			{
				{ "contacts", contacts },
				{ "service", service },
				{ "mycontact", mycontact },
				{ "return_all", returnAll },
				{ "fields", fields }
			};
			return _vk.Call("account.lookupContacts", parameters);
		}

		/// <summary>
		/// Подписывает устройство на базе iOS, Android или Windows Phone на получение Push-уведомлений.
		/// </summary>
		/// <param name="params">Параметры запроса.</param>
		/// <returns>
		/// Возвращает 1 в случае успешного выполнения метода.
		/// На iOS и Windows Phone push-уведомления будут отображены без какой либо обработки.
		/// На Android будут приходить события в следующем формате.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.registerDevice" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public bool RegisterDevice(AccountRegisterDeviceParams @params)
		{
			VkErrors.ThrowIfNullOrEmpty(() => @params.Token);

			return _vk.Call("account.registerDevice", @params);
		}

		/// <summary>
		/// Отписывает устройство от Push уведомлений.
		/// </summary>
		/// <param name="deviceId">Уникальный идентификатор устройства. строка, доступен начиная с версии 5.31 (Строка, доступен начиная с версии 5.31).</param>
		/// <param name="sandbox">Флаг предназначен для iOS устройств. 1 — отписать устройство, использующего sandbox сервер для отправки push-уведомлений, 0 — отписать устройство, не использующее sandbox сервер флаг, может принимать значения 1 или 0, по умолчанию 0 (Флаг, может принимать значения 1 или 0, по умолчанию 0).</param>
		/// <returns>
		/// Возвращает <c>true</c> в случае успешного выполнения метода.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.unregisterDevice" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public bool UnregisterDevice(string deviceId, bool? sandbox = null)
		{
			VkErrors.ThrowIfNullOrEmpty(() => deviceId);

			var parameters = new VkParameters
			{
				{ "device_id", deviceId },
				{ "sandbox", sandbox }
			};

			return _vk.Call("account.unregisterDevice", parameters);
		}

		/// <summary>
		/// Отключает push-уведомления на заданный промежуток времени.
		/// </summary>
		/// <param name="deviceId">Идентификатор устройства для сервиса push уведомлений.</param>
		/// <param name="time">Время в секундах на которое требуется отключить уведомления. (-1 - отключить навсегда)</param>
		/// <param name="peerId">Идентификатор чата, для которого следует отключить уведомления.</param>
		/// <param name="sound">1 - включить звук в данном диалоге, 0 - отключить звук (параметр работает только если указан в peer_id передан идентификатор групповой беседы или пользователя)</param>
		/// <returns>
		/// Возвращает результат выполнения метода.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <seealso cref="https://vk.com/dev/account.setSilenceMode" />.
		/// </remarks>
		[ApiVersion("5.50")]
		public bool SetSilenceMode([NotNull] string deviceId, int? time = null, int? peerId = null, bool? sound = null)
		{
			VkErrors.ThrowIfNullOrEmpty(() => deviceId);

			var parameters = new VkParameters
			{
				{ "device_id", deviceId },
				{ "time", time },
				{ "peer_id", peerId },
				{ "sound", sound }
			};

			return _vk.Call("account.setSilenceMode", parameters);
		}

		/// <summary>
		/// Позволяет получать настройки Push уведомлений.
		/// </summary>
		/// <param name="deviceId">Уникальный идентификатор устройства. строка, доступен начиная с версии 5.31 (Строка, доступен начиная с версии 5.31).</param>
		/// <returns>
		/// Возвращает объект, содержащий поля:
		/// disabled — отключены ли уведомления.
		/// disabled_until — unixtime-значение времени, до которого временно отключены уведомления.
		/// conversations — список, содержащий настройки конкретных диалогов, и их количество первым элементом.
		/// settings — объект с настройками Push-уведомлений в специальном формате.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.getPushSettings" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public AccountPushSettings GetPushSettings(string deviceId)
		{
			var parameters = new VkParameters
			{
				{ "device_id", deviceId }
			};

			return _vk.Call("account.getPushSettings", parameters);
		}

		/// <summary>
		/// Изменяет настройку Push-уведомлений.
		/// </summary>
		/// <param name="deviceId">Уникальный идентификатор устройства. строка, обязательный параметр (Строка, обязательный параметр).</param>
		/// <param name="settings">Сериализованный JSON-объект, описывающий настройки уведомлений в специальном формате данные в формате JSON (Данные в формате JSON).</param>
		/// <param name="key">Ключ уведомления. строка (Строка).</param>
		/// <param name="value">Новое значение уведомления в специальном формате. список слов, разделенных через запятую (Список слов, разделенных через запятую).</param>
		/// <returns>
		/// Возвращает 1 в случае успешного выполнения метода.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.setPushSettings" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public bool SetPushSettings(string deviceId, PushSettings settings, string key, List<string> value)
		{
			var parameters = new VkParameters
			{
				{ "device_id", deviceId },
				{ "settings", settings },
				{ "key", key },
				{ "value", value }
			};
			return _vk.Call("account.setPushSettings", parameters);
		}

		/// <summary>
		/// Получает настройки текущего пользователя в данном приложении.
		/// </summary>
		/// <param name="userId">Идентификатор пользователя, информацию о настройках которого необходимо получить. По умолчанию — текущий пользователь. положительное число, по умолчанию идентификатор текущего пользователя, обязательный параметр (Положительное число, по умолчанию идентификатор текущего пользователя, обязательный параметр).</param>
		/// <returns>
		/// После успешного выполнения возвращает битовую маску настроек текущего пользователя в данном приложении.
		///
		/// Пример Если Вы хотите получить права на Доступ к друзьям и Доступ к статусам пользователя, то Ваша битовая маска будет равна: 2   1024 = 1026.
		/// Если, имея битовую маску 1026, Вы хотите проверить, имеет ли она доступ к друзьям — Вы можете сделать 1026 &amp; 2. Например alert(1026 &amp; 2);
		/// см. Список возможных настроек прав доступа.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.getAppPermissions" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public long GetAppPermissions(long userId)
		{
			var parameters = new VkParameters
			{
				{ "user_id", userId}
			};
			return _vk.Call("account.getAppPermissions", parameters, true);
		}

		/// <summary>
		/// Возвращает список активных рекламных предложений (офферов), выполнив которые пользователь сможет получить соответствующее количество голосов на свой счёт внутри приложения.
		/// </summary>
		/// <param name="offset">Смещение, необходимое для выборки определенного подмножества офферов. положительное число, по умолчанию 0 (Положительное число, по умолчанию 0).</param>
		/// <param name="count">Количество офферов, которое необходимо получить положительное число, по умолчанию 100, максимальное значение 100 (Положительное число, по умолчанию 100, максимальное значение 100).</param>
		/// <returns>
		/// Возвращает массив, состоящий из общего количества старгетированных на текущего пользователя специальных предложений (первый элемент), и списка объектов с информацией о предложениях.
		/// В случае, если на пользователя не старгетировано ни одного специального предложения, массив будет содержать элемент 0 (количество специальных предложений).
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.getActiveOffers" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public InformationAboutOffers GetActiveOffers(ulong? offset = null, ulong? count = null)
		{
			var parameters = new VkParameters
			{
				{ "offset", offset },
				{ "count", count }
			};
			return _vk.Call("account.getActiveOffers", parameters, true);
		}

		/// <summary>
		/// Добавляет пользователя в черный список.
		/// </summary>
		/// <param name="userId">Идентификатор пользователя, которого нужно добавить в черный список. положительное число, обязательный параметр (Положительное число, обязательный параметр).</param>
		/// <returns>
		/// В случае успеха метод вернет <c>true</c>.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.banUser" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public bool BanUser(long userId)
		{
			VkErrors.ThrowIfNumberIsNegative(() => userId);
			var parameters = new VkParameters {
				{ "user_id", userId }
			};

			return _vk.Call("account.banUser", parameters);
		}

		/// <summary>
		/// Убирает пользователя из черного списка.
		/// </summary>
		/// <param name="userId">Идентификатор пользователя, которого нужно убрать из черного списка. положительное число, обязательный параметр (Положительное число, обязательный параметр).</param>
		/// <returns>
		/// В случае успеха метод вернет <c>true</c>.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.unbanUser" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public bool UnbanUser(long userId)
		{
			VkErrors.ThrowIfNumberIsNegative(() => userId);
			var parameters = new VkParameters {
				{ "user_id", userId }
			};

			return _vk.Call("account.unbanUser", parameters);
		}

		/// <summary>
		/// Возвращает список пользователей, находящихся в черном списке.
		/// </summary>
		/// <param name="total">Возвращает общее количество находящихся в черном списке пользователей.</param>
		/// <param name="offset">Смещение необходимое для выборки определенного подмножества черного списка. положительное число (Положительное число).</param>
		/// <param name="count">Количество записей, которое необходимо вернуть. положительное число, по умолчанию 20, максимальное значение 200 (Положительное число, по умолчанию 20, максимальное значение 200).</param>
		/// <returns>
		/// Возвращает набор объектов пользователей, находящихся в черном списке.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.getBanned" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public ReadOnlyCollection<User> GetBanned(out int total, int? offset = null, int? count = null)
		{
			VkErrors.ThrowIfNumberIsNegative(() => offset);
			VkErrors.ThrowIfNumberIsNegative(() => count);

			var parameters = new VkParameters
			{
				{ "offset", offset },
				{ "count", count }
			};
			var response = _vk.Call("account.getBanned", parameters);

			total = response["count"];

			return response["items"].ToReadOnlyCollectionOf<User>(vkResponse => vkResponse);
		}

		/// <summary>
		/// Возвращает информацию о текущем аккаунте.
		/// </summary>
		/// <param name="fields">Список полей, которые необходимо вернуть. Возможные значения: (country, https_required, own_posts_default, no_wall_replies, intro, lang, По умолчанию будут возвращены все поля. список слов, разделенных через запятую (Список слов, разделенных через запятую).</param>
		/// <returns>
		/// Метод возвращает объект, содержащий следующие поля:
		/// country – строковой код страны, определенный по IP адресу, с которого сделан запрос;
		/// https_required – 1 - пользователь установил на сайте настройку "Всегда использовать безопасное соединение"; 0 - безопасное соединение не требуется;
		/// own_posts_default – 1 - на стене пользователя по-умолчанию должны отображаться только собственные записи. Соответствует настройке на сайте "Показывать только мои записи", 0 - на стене пользователя должны отображаться все записи;
		/// no_wall_replies – 1 - пользователь отключил комментирование записей на стене, 0 - комментирование записей разрешено;
		/// intro – битовая маска отвечающая за прохождение обучения использованию приложения;
		/// lang – числовой идентификатор текущего языка пользователя.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.getInfo" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public AccountInfo GetInfo(AccountFields fields = null)
		{
			return _vk.Call("account.getInfo", new VkParameters { { "fields", fields } });
		}

		/// <summary>
		/// Позволяет редактировать информацию о текущем аккаунте.
		/// </summary>
		/// <param name="name">Имя настройки.</param>
		/// <param name="value">Значение настройки.</param>
		/// <returns>
		/// В результате успешного выполнения возвращает <c>true</c>.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.setInfo" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public bool SetInfo([NotNull]string name, [NotNull]string value)
		{
			var parameters = new VkParameters
			{
				{ "name", name },
				{ "value", value }
			};
			return _vk.Call("account.setInfo", parameters);
		}

		/// <summary>
		/// Позволяет сменить пароль пользователя после успешного восстановления доступа к аккаунту через СМС, используя метод auth.restore.
		/// </summary>
		/// <param name="restoreSid">Идентификатор сессии, полученный при восстановлении доступа используя метод auth.restore. (В случае если пароль меняется сразу после восстановления доступа) строка (Строка).</param>
		/// <param name="changePasswordHash">Хэш, полученный при успешной OAuth авторизации по коду полученному по СМС (В случае если пароль меняется сразу после восстановления доступа) строка (Строка).</param>
		/// <param name="oldPassword">Текущий пароль пользователя. строка (Строка).</param>
		/// <param name="newPassword">Новый пароль, который будет установлен в качестве текущего. строка, минимальная длина 6, обязательный параметр (Строка, минимальная длина 6, обязательный параметр).</param>
		/// <returns>
		/// В результате выполнения этого метода будет возвращен объект с полем token, содержащим новый токен, и полем secret в случае, если токен был nohttps.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <see href="http://vk.com/dev/account.changePassword" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public AccountChangePasswordResult ChangePassword(string oldPassword, string newPassword, string restoreSid = null, string changePasswordHash = null)
		{
			var parameters = new VkParameters
			{
				{ "restore_sid", restoreSid },
				{ "change_password_hash", changePasswordHash },
				{ "old_password", oldPassword },
				{ "new_password", newPassword }
			};
			return _vk.Call("account.сhangePassword", parameters);
		}

		/// <summary>
		/// Возвращает информацию о текущем профиле.
		/// </summary>
		/// <returns>Информация о текущем профиле в виде <see cref="Model.User"/></returns>
		/// <remarks>
		/// Страница документации ВКонтакте <seealso cref="https://vk.com/dev/account.getProfileInfo" />.
		/// </remarks>
		[Pure]
		[ApiVersion("5.45")]
		public AccountSaveProfileInfoParams GetProfileInfo()
		{
			User user = _vk.Call("account.getProfileInfo", VkParameters.Empty);
			return new AccountSaveProfileInfoParams
			{
				City = user.City,
				Country = user.Country,
				BirthDate = user.BirthDate,
				BirthdayVisibility = user.BirthdayVisibility,
				FirstName = user.FirstName,
				LastName = user.LastName,
				HomeTown = user.HomeTown,
				MaidenName = user.MaidenName,
				Relation = user.Relation,
				Sex = user.Sex,
				RelationPartner = user.RelationPartner,
				ScreenName = user.ScreenName,
				Status = user.Status,
				Phone = user.MobilePhone
			};
		}

		/// <summary>
		/// Редактирует информацию текущего профиля.
		/// </summary>
		/// <param name="cancelRequestId">Идентификатор заявки на смену имени, которую необходимо отменить.</param>
		/// <returns>Результат отмены заявки.</returns>
		/// <remarks>Метод вынесен как отдельный, потому что если в запросе передан параметр <paramref name="cancelRequestId"/>, все остальные параметры игнорируются.</remarks>
		/// <remarks>
		/// Страница документации ВКонтакте <seealso cref="https://vk.com/dev/account.saveProfileInfo" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public bool SaveProfileInfo(int cancelRequestId)
		{
			VkErrors.ThrowIfNumberIsNegative(() => cancelRequestId);
			return _vk.Call("account.saveProfileInfo", new VkParameters { { "cancel_request_id", cancelRequestId } })["changed"];
		}

		/// <summary>
		/// Редактирует информацию текущего профиля.
		/// </summary>
		/// <param name="changeNameRequest">Если в параметрах передавалось имя или фамилия пользователя,
		/// в этом параметре будет возвращен объект типа <see cref="ChangeNameRequest" />, содержащий информацию о заявке на смену имени.</param>
		/// <param name="params">The parameters.</param>
		/// <returns>
		/// Результат отмены заявки.
		/// </returns>
		/// <remarks>
		/// Страница документации ВКонтакте <seealso cref="https://vk.com/dev/account.saveProfileInfo" />.
		/// </remarks>
		[ApiVersion("5.45")]
		public bool SaveProfileInfo(out ChangeNameRequest changeNameRequest, AccountSaveProfileInfoParams @params)
		{
			var response = _vk.Call("account.saveProfileInfo", @params);

			changeNameRequest = null;

			if (response.ContainsKey("name_request"))
			{
				changeNameRequest = response["name_request"];
			}

			return response["changed"];
		}
	}
}