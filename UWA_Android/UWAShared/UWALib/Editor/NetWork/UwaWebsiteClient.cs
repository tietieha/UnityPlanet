using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UWA.Core;
using UWA.SDK;

namespace UWALib.Editor.NetWork
{
	// Token: 0x02000058 RID: 88
	internal static class UwaWebsiteClient
	{
		// Token: 0x060003C5 RID: 965 RVA: 0x00021A6C File Offset: 0x0001FC6C
		public static void Logout()
		{
			UwaWebsiteClient.userProfile = null;
			UwaWebsiteClient.paUploadInfo.Clear();
			UwaWebsiteClient.gotDataSubmitInfo.Clear();
			UwaWebsiteClient.ActiveSessionID = string.Empty;
			UwaWebsiteClient.SavedSessionID = string.Empty;
			UwaWebsiteClient.sLoginState = UwaWebsiteClient.LoginState.LOGGED_OUT;
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060003C6 RID: 966 RVA: 0x00021AA8 File Offset: 0x0001FCA8
		// (set) Token: 0x060003C7 RID: 967 RVA: 0x00021AC8 File Offset: 0x0001FCC8
		public static string LogPath
		{
			get
			{
				return UwaWebsiteClient._logPath;
			}
			set
			{
				bool flag = UwaWebsiteClient._logPath != value;
				if (flag)
				{
					bool flag2 = string.IsNullOrEmpty(value) && !Directory.Exists(Path.GetDirectoryName(value));
					if (flag2)
					{
						UwaWebsiteClient._logPath = null;
					}
					else
					{
						bool flag3 = !File.Exists(value);
						if (flag3)
						{
							try
							{
								File.WriteAllText(value, "");
							}
							catch (Exception ex)
							{
							}
						}
						UwaWebsiteClient._logPath = (File.Exists(value) ? value : null);
					}
				}
			}
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x00021B70 File Offset: 0x0001FD70
		[Conditional("DEBUG_LOG")]
		public static void Log(string content)
		{
			bool flag = UwaWebsiteClient.Debug || UwaWebsiteClient.DebugLog;
			if (flag)
			{
				UnityEngine.Debug.Log(content);
			}
			UwaWebsiteClient.WriteLogToFile("LOG", content);
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x00021BB4 File Offset: 0x0001FDB4
		[Conditional("DEBUG_LOG")]
		public static void LogError(string content)
		{
			bool flag = UwaWebsiteClient.Debug || UwaWebsiteClient.DebugLog;
			if (flag)
			{
				UnityEngine.Debug.LogError(content);
			}
			UwaWebsiteClient.WriteLogToFile("ERROR", content);
		}

		// Token: 0x060003CA RID: 970 RVA: 0x00021BF8 File Offset: 0x0001FDF8
		[Conditional("DEBUG_LOG")]
		public static void LogWarning(string content)
		{
			bool flag = UwaWebsiteClient.Debug || UwaWebsiteClient.DebugLog;
			if (flag)
			{
				UnityEngine.Debug.LogWarning(content);
			}
			UwaWebsiteClient.WriteLogToFile("WARNING", content);
		}

		// Token: 0x060003CB RID: 971 RVA: 0x00021C3C File Offset: 0x0001FE3C
		private static void WriteLogToFile(string Type, string content)
		{
			bool flag = UwaWebsiteClient.LogPath == null || !File.Exists(UwaWebsiteClient.LogPath);
			if (!flag)
			{
				object obj = UwaWebsiteClient.logObj;
				lock (obj)
				{
					try
					{
						StreamWriter streamWriter = File.AppendText(UwaWebsiteClient.LogPath);
						streamWriter.WriteLine("{0} [{1}]:", Type, DateTime.Now);
						streamWriter.WriteLine(content);
						streamWriter.WriteLine("");
						streamWriter.Close();
					}
					catch (Exception ex)
					{
					}
				}
			}
		}

		// Token: 0x060003CC RID: 972 RVA: 0x00021CF8 File Offset: 0x0001FEF8
		public static string StrToMD5(string str)
		{
			byte[] bytes = Encoding.GetEncoding("ASCII").GetBytes(str);
			MD5 md = new MD5CryptoServiceProvider();
			byte[] array = md.ComputeHash(bytes);
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text += array[i].ToString("x2");
			}
			return text.ToLower();
		}

		// Token: 0x060003CD RID: 973 RVA: 0x00021D74 File Offset: 0x0001FF74
		internal static void LoginWithUserIdToken(string userId, string token, UwaWebsiteClient.OnDoneCallback onCallback)
		{
			UwaWebsiteClient.Log("LoginWithUserIdToken " + userId + "," + token);
			int userId2 = 0;
			bool flag = !int.TryParse(userId, out userId2);
			if (flag)
			{
				onCallback(0, "userId (" + userId + ") is not a number.");
			}
			else
			{
				UwaWebsiteClient.cookcontanerInstance = new CookieContainer(100);
				Cookie cookie = new Cookie("uwa_user_id", userId);
				cookie.Domain = UwaWebsiteClient.UwaDomain;
				cookie.Path = "/";
				UwaWebsiteClient.cookcontanerInstance.Add(cookie);
				UwaWebsiteClient.Log("ADD Cookie {uwa_user_id," + userId + "}");
				Cookie cookie2 = new Cookie("uwa_login_token", token);
				cookie2.Domain = UwaWebsiteClient.UwaDomain;
				cookie2.Path = "/";
				UwaWebsiteClient.cookcontanerInstance.Add(cookie2);
				UwaWebsiteClient.Log("ADD Cookie {uwa_login_token," + token + "}");
				UwaWebsiteClient.LoginToken = token;
				UwaWebsiteClient.userProfile = new UwaWebsiteClient.UserProfile();
				UwaWebsiteClient.userProfile.Name = "unknown";
				UwaWebsiteClient.userProfile.IconUrl = "unknown";
				UwaWebsiteClient.userProfile.UserId = userId2;
				UwaWebsiteClient.userProfile.Email = "unknown";
				UwaWebsiteClient.sLoginState = UwaWebsiteClient.LoginState.LOGGED_IN;
				onCallback(0, null);
			}
		}

		// Token: 0x060003CE RID: 974 RVA: 0x00021EC0 File Offset: 0x000200C0
		internal static void NewLoginWithCredentials(string username, string password, string authCode, bool rememberMe, UwaWebsiteClient.OnDoneCallback onCallback)
		{
			bool flag = UwaWebsiteClient.sLoginState == UwaWebsiteClient.LoginState.IN_PROGRESS;
			if (flag)
			{
				UwaWebsiteClient.LogWarning("Tried to login with credentials while already in progress of logging in");
			}
			else
			{
				UwaWebsiteClient.sLoginState = UwaWebsiteClient.LoginState.IN_PROGRESS;
				UwaWebsiteClient.RememberSession = rememberMe;
				UwaWebsiteClient.sErrorMessage = null;
				string text = string.Format("{0}/api/m/signin", UwaWebsiteClient.UwaWebsiteUrl);
				UwaWebsiteClient.Log(text);
				Uri address = new Uri(text);
				UwaWebsiteWebClient uwaWebsiteWebClient = new UwaWebsiteWebClient();
				JSONValue jsonvalue = default(JSONValue);
				string s = username;
				int s2 = 1;
				bool flag2 = Regex.IsMatch(username, "[A-Za-z0-9!#$%&'*+\\/=?^_`{|}~-]+(?:\\.[A-Za-z0-9!#$%&'*+\\/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?");
				if (flag2)
				{
					s2 = 0;
				}
				else
				{
					bool flag3 = Regex.IsMatch(username, "^(?:(?:(?:\\+86)?1[34578]\\d(-?)\\d{4}\\1\\d{4})|(?:(?:\\+886)?0?9\\d{8})|(?:(?:\\+852)?[9|6]\\d{7}))$");
					if (flag3)
					{
						bool flag4 = Regex.IsMatch(username, "^(\\+(?:86|852|886))");
						if (flag4)
						{
							string value = Regex.Match(username, "^(\\+(?:86|852|886))").Groups[1].Value;
							jsonvalue["country_code"] = value;
							s = Regex.Replace(username, "^(\\+(?:86|852|886))", "");
						}
						s2 = 1;
					}
				}
				jsonvalue["account"] = s;
				jsonvalue["type"] = s2;
				jsonvalue["password"] = UwaWebsiteClient.StrToMD5(password);
				jsonvalue["keep_login"] = UwaWebsiteClient.RememberSession;
				bool flag5 = authCode != null;
				if (flag5)
				{
					jsonvalue["auth_code"] = authCode;
				}
				string text2 = jsonvalue.ToString();
				UwaWebsiteClient.Log(text2);
				UwaWebsiteClient.Pending pending = new UwaWebsiteClient.Pending();
				pending.conn = uwaWebsiteWebClient;
				pending.id = "login";
				pending.callback = UwaWebsiteClient.WrapLoginCallback(onCallback);
				UwaWebsiteClient.Pendings.Add(pending);
				uwaWebsiteWebClient.Headers.Add("Accept", "application/json");
				uwaWebsiteWebClient.Headers.Add("Content-type", "application/json");
				uwaWebsiteWebClient.UploadStringCompleted += UwaWebsiteClient.UploadStringCallback;
				try
				{
					uwaWebsiteWebClient.UploadStringAsync(address, "POST", text2, pending);
				}
				catch (WebException ex)
				{
					pending.ex = ex;
					UwaWebsiteClient.sLoginState = UwaWebsiteClient.LoginState.LOGIN_ERROR;
				}
			}
		}

		// Token: 0x060003CF RID: 975 RVA: 0x00022108 File Offset: 0x00020308
		internal static void GARecord(string json, UwaWebsiteClient.OnDoneCallback onCallback)
		{
			string text = UwaWebsiteClient.GAUrl + "?api_secret=odjFDhkiRheRIQktYNBrlw&measurement_id=G-3HB2XWQZ6H";
			UwaWebsiteClient.Log(text);
			Uri address = new Uri(text);
			UwaWebsiteWebClient uwaWebsiteWebClient = new UwaWebsiteWebClient();
			UwaWebsiteClient.Log(json);
			UwaWebsiteClient.Pending pending = new UwaWebsiteClient.Pending();
			pending.conn = uwaWebsiteWebClient;
			pending.id = "ga";
			UwaWebsiteClient.Pendings.Add(pending);
			uwaWebsiteWebClient.Headers.Add("Accept", "application/json");
			uwaWebsiteWebClient.Headers.Add("Content-type", "application/json");
			uwaWebsiteWebClient.UploadStringCompleted += UwaWebsiteClient.UploadStringCallback;
			try
			{
				uwaWebsiteWebClient.UploadStringAsync(address, "POST", json, pending);
			}
			catch (WebException ex)
			{
				pending.ex = ex;
			}
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x000221DC File Offset: 0x000203DC
		internal static void LoginWithCredentials(string username, string password, string authCode, bool rememberMe, UwaWebsiteClient.OnDoneCallback onCallback)
		{
			UwaWebsiteClient.NewLoginWithCredentials(username, password, authCode, rememberMe, onCallback);
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x000221EC File Offset: 0x000203EC
		internal static void TryLogin(UwaWebsiteClient.OnDoneCallback5 onCallback)
		{
			bool flag = UwaWebsiteClient.sLoginState != UwaWebsiteClient.LoginState.LOGGED_IN;
			if (flag)
			{
				UwaWebsiteClient.LogWarning("trylogin when loginstate == " + UwaWebsiteClient.sLoginState.ToString());
			}
			else
			{
				string text = string.Format("{0}/api/m/tryLogin", UwaWebsiteClient.UwaWebsiteUrl);
				UwaWebsiteClient.Log(text);
				Uri address = new Uri(text);
				UwaWebsiteWebClient uwaWebsiteWebClient = new UwaWebsiteWebClient();
				UwaWebsiteClient.Pending pending = new UwaWebsiteClient.Pending();
				pending.conn = uwaWebsiteWebClient;
				pending.id = "trylogin";
				pending.callback = UwaWebsiteClient.WrapTryLoginCallback(onCallback);
				UwaWebsiteClient.Pendings.Add(pending);
				uwaWebsiteWebClient.Headers.Add("Accept", "application/json");
				uwaWebsiteWebClient.DownloadDataCompleted += UwaWebsiteClient.DownloadDataCallback;
				bool flag2 = UwaWebsiteClient.cookcontanerInstance != null;
				if (flag2)
				{
					uwaWebsiteWebClient.CookieContainer = UwaWebsiteClient.cookcontanerInstance;
				}
				try
				{
					uwaWebsiteWebClient.DownloadDataAsync(address, pending);
				}
				catch (WebException ex)
				{
					pending.ex = ex;
				}
			}
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x00022308 File Offset: 0x00020508
		internal static Dictionary<string, string> ParseUrl(string url)
		{
			bool flag = url == null;
			if (flag)
			{
				throw new ArgumentNullException("url");
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			bool flag2 = url == "";
			Dictionary<string, string> result;
			if (flag2)
			{
				result = dictionary;
			}
			else
			{
				int num = url.IndexOf('?');
				bool flag3 = num == -1;
				if (flag3)
				{
					result = dictionary;
				}
				else
				{
					bool flag4 = num == url.Length - 1;
					if (flag4)
					{
						result = dictionary;
					}
					else
					{
						string text = url.Substring(num + 1);
						string[] array = text.Split(new char[]
						{
							'&'
						});
						for (int i = 0; i < array.Length; i++)
						{
							string[] array2 = array[i].Split(new char[]
							{
								'='
							});
							bool flag5 = array2.Length == 2;
							if (flag5)
							{
								dictionary.Add(array2[0], "\"" + array2[1] + "\"");
							}
						}
						result = dictionary;
					}
				}
			}
			return result;
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00022428 File Offset: 0x00020628
		private static string SHA512(string input)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(input);
			string result;
			using (SHA512 sha = System.Security.Cryptography.SHA512.Create())
			{
				byte[] array = sha.ComputeHash(bytes);
				StringBuilder stringBuilder = new StringBuilder(128);
				foreach (byte b in array)
				{
					stringBuilder.Append(b.ToString("X2"));
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x000224C0 File Offset: 0x000206C0
		internal static void AddSign(UwaWebsiteWebClient client, string url, JSONValue body)
		{
			DateTime d = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
			long num = (long)(DateTime.Now - d).TotalMilliseconds;
			client.Headers.Add("x-uwa-timestamp", num.ToString());
			client.Headers.Add("x-uwa-sign-algorithm", "UWA-SHA512");
			client.Headers.Add("x-uwa-sign-app-category", "UWA-WEB-DEFAULT");
			Dictionary<string, string> dictionary = UwaWebsiteClient.ParseUrl(url);
			dictionary.Add("x-uwa-timestamp", num.ToString());
			dictionary.Add("x-uwa-sign-algorithm", "UWA-SHA512");
			dictionary.Add("x-uwa-sign-app-category", "UWA-WEB-DEFAULT");
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			bool flag = !body.IsNull();
			if (flag)
			{
				bool flag2 = body.IsDict();
				if (flag2)
				{
					foreach (KeyValuePair<string, JSONValue> keyValuePair in body.AsDict(false))
					{
						dictionary2[keyValuePair.Key] = keyValuePair.Value.ToString().Replace("\\/", "/");
					}
				}
				else
				{
					bool flag3 = body.IsList();
					if (flag3)
					{
						for (int i = 0; i < body.AsList(false).Count; i++)
						{
							dictionary2["uwa-body-array-" + i.ToString()] = body.AsList(false)[i].ToString().Replace("\\/", "/");
						}
					}
				}
			}
			foreach (KeyValuePair<string, string> keyValuePair2 in dictionary2)
			{
				bool flag4 = !dictionary.ContainsKey(keyValuePair2.Key);
				if (flag4)
				{
					dictionary.Add(keyValuePair2.Key, keyValuePair2.Value);
				}
			}
			Dictionary<string, string> dictionary3 = (from p in dictionary
			orderby p.Key descending
			select p).ToDictionary((KeyValuePair<string, string> p) => p.Key, (KeyValuePair<string, string> o) => o.Value);
			StringBuilder stringBuilder = new StringBuilder();
			bool flag5 = true;
			foreach (KeyValuePair<string, string> keyValuePair3 in dictionary3)
			{
				bool flag6 = flag5;
				if (flag6)
				{
					flag5 = false;
				}
				else
				{
					stringBuilder.Append("&");
				}
				stringBuilder.Append(keyValuePair3.Key + "=" + keyValuePair3.Value);
			}
			string text = stringBuilder.ToString();
			string text2 = UwaWebsiteClient.SHA512(text).ToLower();
			UwaWebsiteClient.Log("Sign:" + text);
			UwaWebsiteClient.Log("Sign SHA512:" + text2);
			client.Headers.Add("x-uwa-signature", text2);
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00022870 File Offset: 0x00020A70
		internal static void ProjectBalance(string serviceMetaType, int projectId, UwaWebsiteClient.OnDoneCallback onGetBalanceCallback)
		{
			UwaWebsiteClient.sErrorMessage = null;
			string text = string.Format("{0}/project/v1/balance/owner?serviceMetaType={1}&projectGroupId={2}", UwaWebsiteClient.UwaGotServerUrl, serviceMetaType, projectId);
			UwaWebsiteClient.Log(text);
			Uri address = new Uri(text);
			UwaWebsiteWebClient uwaWebsiteWebClient = new UwaWebsiteWebClient();
			UwaWebsiteClient.AddSign(uwaWebsiteWebClient, text, JSONValue.NewNull());
			UwaWebsiteClient.Pending pending = new UwaWebsiteClient.Pending();
			pending.conn = uwaWebsiteWebClient;
			pending.id = "project_balance(projects/balance)";
			pending.callback = UwaWebsiteClient.WrapProjectBalanceCallback(projectId, onGetBalanceCallback);
			UwaWebsiteClient.Pendings.Add(pending);
			uwaWebsiteWebClient.Headers.Add("Accept", "application/json");
			uwaWebsiteWebClient.Headers.Add("x-uwa-api-minor-version", "v1.0.1");
			uwaWebsiteWebClient.CookieContainer = UwaWebsiteClient.cookcontanerInstance;
			uwaWebsiteWebClient.DownloadDataCompleted += UwaWebsiteClient.DownloadDataCallback;
			try
			{
				uwaWebsiteWebClient.DownloadDataAsync(address, pending);
			}
			catch (WebException ex)
			{
				pending.ex = ex;
			}
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x0002296C File Offset: 0x00020B6C
		internal static void GetCreateProjectCheck(UwaWebsiteClient.OnDoneCallback5 onCheckCreateProjCallback)
		{
			bool flag = !UwaWebsiteClient.IsLoggedIn;
			if (!flag)
			{
				UwaWebsiteClient.sErrorMessage = null;
				string text = string.Format("{0}/project/v1/group/create/check", UwaWebsiteClient.UwaGotServerUrl);
				UwaWebsiteClient.Log(text);
				Uri address = new Uri(text);
				UwaWebsiteWebClient uwaWebsiteWebClient = new UwaWebsiteWebClient();
				UwaWebsiteClient.AddSign(uwaWebsiteWebClient, text, JSONValue.NewNull());
				UwaWebsiteClient.Pending pending = new UwaWebsiteClient.Pending();
				pending.conn = uwaWebsiteWebClient;
				pending.id = "get_create_project_check(project/v1/group/create/check)";
				pending.callback = UwaWebsiteClient.WrapGetCreateProjectCheckCallback(onCheckCreateProjCallback);
				UwaWebsiteClient.Pendings.Add(pending);
				uwaWebsiteWebClient.Headers.Add("Accept", "application/json");
				uwaWebsiteWebClient.Headers.Add("x-uwa-api-minor-version", "v1.0.1");
				uwaWebsiteWebClient.CookieContainer = UwaWebsiteClient.cookcontanerInstance;
				uwaWebsiteWebClient.DownloadDataCompleted += UwaWebsiteClient.DownloadDataCallback;
				try
				{
					uwaWebsiteWebClient.DownloadDataAsync(address, pending);
				}
				catch (WebException ex)
				{
					pending.ex = ex;
				}
			}
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x00022A78 File Offset: 0x00020C78
		internal static void CreateProject(int gameSubType, int gameType, string projectName, string pkgName, UwaWebsiteClient.OnDoneCallback2 onCreateProj)
		{
			UwaWebsiteClient.sErrorMessage = null;
			string text = string.Format("{0}/project/v1/group", UwaWebsiteClient.UwaGotServerUrl);
			Uri address = new Uri(text);
			UwaWebsiteClient.Log(text);
			UwaWebsiteWebClient uwaWebsiteWebClient = new UwaWebsiteWebClient();
			JSONValue jsonvalue = default(JSONValue);
			jsonvalue["engine"] = 1;
			jsonvalue["gameSubtype"] = gameSubType;
			jsonvalue["gameType"] = gameType;
			jsonvalue["projectGroupName"] = projectName;
			jsonvalue["projectGroupPackageName"] = pkgName;
			string text2 = jsonvalue.ToString();
			UwaWebsiteClient.Log(text2);
			UwaWebsiteClient.Pending pending = new UwaWebsiteClient.Pending();
			pending.conn = uwaWebsiteWebClient;
			pending.id = "create_project(project/v1/group)";
			pending.callback = UwaWebsiteClient.WrapOnCreateProjectCallback(onCreateProj);
			UwaWebsiteClient.Pendings.Add(pending);
			uwaWebsiteWebClient.Headers.Add("Accept", "application/json");
			uwaWebsiteWebClient.Headers.Add("x-uwa-api-minor-version", "v1.0.1");
			uwaWebsiteWebClient.Headers.Add("Content-type", "application/json");
			uwaWebsiteWebClient.CookieContainer = UwaWebsiteClient.cookcontanerInstance;
			uwaWebsiteWebClient.UploadStringCompleted += UwaWebsiteClient.UploadStringCallback;
			try
			{
				uwaWebsiteWebClient.UploadStringAsync(address, "POST", text2, pending);
			}
			catch (WebException ex)
			{
				pending.ex = ex;
			}
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x00022C00 File Offset: 0x00020E00
		internal static void GotProjectAll(UwaWebsiteClient.OnDoneCallback onGetAllCallback)
		{
			bool flag = !UwaWebsiteClient.IsLoggedIn;
			if (!flag)
			{
				UwaWebsiteClient.sErrorMessage = null;
				string text = string.Format("{0}/project/v1/group/all/infos", UwaWebsiteClient.UwaGotServerUrl);
				UwaWebsiteClient.Log(text);
				Uri address = new Uri(text);
				UwaWebsiteWebClient uwaWebsiteWebClient = new UwaWebsiteWebClient();
				UwaWebsiteClient.AddSign(uwaWebsiteWebClient, text, JSONValue.NewNull());
				UwaWebsiteClient.Pending pending = new UwaWebsiteClient.Pending();
				pending.conn = uwaWebsiteWebClient;
				pending.id = "get_all_group_project(project/v1/group/all/infos)";
				pending.callback = UwaWebsiteClient.WrapGotProjectAllCallback(onGetAllCallback);
				UwaWebsiteClient.Pendings.Add(pending);
				uwaWebsiteWebClient.Headers.Add("Accept", "application/json");
				uwaWebsiteWebClient.Headers.Add("x-uwa-api-minor-version", "v1.0.1");
				uwaWebsiteWebClient.CookieContainer = UwaWebsiteClient.cookcontanerInstance;
				uwaWebsiteWebClient.DownloadDataCompleted += UwaWebsiteClient.DownloadDataCallback;
				try
				{
					uwaWebsiteWebClient.DownloadDataAsync(address, pending);
				}
				catch (WebException ex)
				{
					pending.ex = ex;
				}
			}
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x00022D0C File Offset: 0x00020F0C
		internal static void GotUploadInfo(UwaWebsiteClient.OnDoneCallback callback, UwaWebsiteClient.ServiceDataInfo dataInfo)
		{
			UwaWebsiteClient.sErrorMessage = null;
			string text = string.Format("{0}/gotonline/v1/record/oss/role", UwaWebsiteClient.UwaGotServerUrl);
			bool flag = dataInfo.dataType == 9 || dataInfo.dataType == 8;
			bool flag2 = dataInfo.dataType == 7;
			bool flag3 = dataInfo.dataType == 50;
			bool flag4 = flag;
			if (flag4)
			{
				text = string.Format("{0}/gpm/v1/record/oss/role", UwaWebsiteClient.UwaGotServerUrl);
			}
			bool flag5 = flag2;
			if (flag5)
			{
				text = string.Format("{0}/lt/v1/record/oss/role?projectGroupId={1}&dataKey={2}", UwaWebsiteClient.UwaGotServerUrl, dataInfo.selectedProjectId, dataInfo.dataHash);
			}
			bool flag6 = flag3;
			if (flag6)
			{
				text = string.Format("{0}/ab/v1/record/oss/role/datafile?projectGroupId={1}", UwaWebsiteClient.UwaGotServerUrl, dataInfo.selectedProjectId);
			}
			string value = "v1.0.1";
			bool flag7 = !flag3 && !flag2;
			if (flag7)
			{
				value = "v1.0.2";
			}
			UwaWebsiteClient.Log(text);
			string text2 = "";
			JSONValue body = JSONValue.NewNull();
			bool flag8 = !flag2 && !flag3;
			if (flag8)
			{
				JSONValue item = default(JSONValue);
				item["dataKey"] = dataInfo.dataHash;
				bool flag9 = !flag;
				if (flag9)
				{
					item["duration"] = dataInfo.duration.ToString();
					item["serviceSubtype"] = UwaWebsiteClient.dataTypeDic[dataInfo.dataType];
					item["durationUnitOfMeasure"] = "second";
				}
				body = default(JSONValue);
				body["checkDetails"] = new List<JSONValue>();
				body["checkDetails"].AsList(false).Add(item);
				body["projectGroupId"] = dataInfo.selectedProjectId;
				bool flag10 = !flag && dataInfo.selectProject.SelectedBalance != null;
				if (flag10)
				{
					body["serviceTypeCode"] = dataInfo.selectProject.SelectedBalance.serviceTypeCode.ToString();
				}
				text2 = body.ToString();
			}
			UwaWebsiteClient.Log(text2);
			Uri address = new Uri(text);
			UwaWebsiteWebClient uwaWebsiteWebClient = new UwaWebsiteWebClient();
			UwaWebsiteClient.AddSign(uwaWebsiteWebClient, text, body);
			UwaWebsiteClient.gotDataSubmitInfo.selected = dataInfo;
			UwaWebsiteClient.Pending pending = new UwaWebsiteClient.Pending();
			pending.conn = uwaWebsiteWebClient;
			pending.id = "get_got_uploadInfo(gotonline/v1/record/oss/role)";
			pending.callback = UwaWebsiteClient.WrapGotUploadInfoCallback(callback);
			UwaWebsiteClient.Pendings.Add(pending);
			uwaWebsiteWebClient.Headers.Add("Accept", "application/json");
			uwaWebsiteWebClient.Headers.Add("Content-Type", "application/json");
			uwaWebsiteWebClient.Headers.Add("x-uwa-api-minor-version", value);
			uwaWebsiteWebClient.CookieContainer = UwaWebsiteClient.cookcontanerInstance;
			bool flag11 = flag2 || flag3;
			if (flag11)
			{
				uwaWebsiteWebClient.DownloadDataCompleted += UwaWebsiteClient.DownloadDataCallback;
			}
			else
			{
				uwaWebsiteWebClient.UploadStringCompleted += UwaWebsiteClient.UploadStringCallback;
			}
			UwaWebsiteClient.gotDataSubmitInfo.state = UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Request;
			UwaWebsiteClient.gotDataSubmitInfo.lastPercent = 0f;
			try
			{
				bool flag12 = flag2 || flag3;
				if (flag12)
				{
					uwaWebsiteWebClient.DownloadDataAsync(address, pending);
				}
				else
				{
					uwaWebsiteWebClient.UploadStringAsync(address, "POST", text2, pending);
				}
			}
			catch (WebException ex)
			{
				pending.ex = ex;
				UwaWebsiteClient.gotDataSubmitInfo.state = UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Idle;
			}
		}

		// Token: 0x060003DA RID: 986 RVA: 0x000230C0 File Offset: 0x000212C0
		internal static void JenkinsNotify(string buildGuid, string url, UwaWebsiteClient.OnDoneCallback onTest)
		{
			string[] array = buildGuid.Split(new string[]
			{
				"#$*"
			}, StringSplitOptions.RemoveEmptyEntries);
			int num = 0;
			bool flag = array.Length != 3;
			if (flag)
			{
				onTest(1, "build id length != 3.");
			}
			else
			{
				bool flag2 = !int.TryParse(array[2], out num);
				if (flag2)
				{
					onTest(2, "deploySequence can not parse.");
				}
				else
				{
					string text = array[0];
					string text2 = array[1];
					string text3 = string.Format("{0}/api/user/balance?branchDisplayName={1}&companyIdentification={2}&deploySequence={3}", new object[]
					{
						url,
						text2,
						text,
						num
					});
					UwaWebsiteClient.Log(text3);
					Uri address = new Uri(text3);
					UwaWebsiteWebClient uwaWebsiteWebClient = new UwaWebsiteWebClient();
					UwaWebsiteClient.Pending pending = new UwaWebsiteClient.Pending();
					pending.conn = uwaWebsiteWebClient;
					pending.id = "jenkins_nofify(balance)";
					pending.callback = UwaWebsiteClient.WrapJenkinsNotifyCallback(onTest);
					UwaWebsiteClient.Pendings.Add(pending);
					uwaWebsiteWebClient.Headers.Add("Accept", "application/json");
					uwaWebsiteWebClient.CookieContainer = UwaWebsiteClient.cookcontanerInstance;
					uwaWebsiteWebClient.DownloadDataCompleted += UwaWebsiteClient.DownloadDataCallback;
					try
					{
						uwaWebsiteWebClient.DownloadDataAsync(address, pending);
					}
					catch (WebException ex)
					{
						pending.ex = ex;
					}
				}
			}
		}

		// Token: 0x060003DB RID: 987 RVA: 0x0002322C File Offset: 0x0002142C
		internal static void TestAPI(UwaWebsiteClient.OnDoneCallback onTest)
		{
			UwaWebsiteClient.sErrorMessage = null;
			string text = string.Format("http://192.168.1.47/api/aws/s3/test", new object[0]);
			Uri address = new Uri(text);
			UwaWebsiteClient.Log(text);
			UwaWebsiteWebClient uwaWebsiteWebClient = new UwaWebsiteWebClient();
			JSONValue jsonvalue = default(JSONValue);
			jsonvalue["commonProjectId"] = 3;
			jsonvalue["dataKey"] = "20180627185841ssj5e774199";
			string text2 = jsonvalue.ToString();
			UwaWebsiteClient.Log(text2);
			UwaWebsiteClient.Pending pending = new UwaWebsiteClient.Pending();
			pending.conn = uwaWebsiteWebClient;
			pending.id = "test api(test)";
			pending.callback = UwaWebsiteClient.WrapOnTestCallback(onTest);
			UwaWebsiteClient.Pendings.Add(pending);
			uwaWebsiteWebClient.Headers.Add("Accept", "application/json");
			uwaWebsiteWebClient.Headers.Add("Content-type", "application/json");
			uwaWebsiteWebClient.CookieContainer = UwaWebsiteClient.cookcontanerInstance;
			uwaWebsiteWebClient.UploadStringCompleted += UwaWebsiteClient.UploadStringCallback;
			try
			{
				uwaWebsiteWebClient.UploadStringAsync(address, "POST", text2, pending);
			}
			catch (WebException ex)
			{
				pending.ex = ex;
			}
		}

		// Token: 0x060003DC RID: 988 RVA: 0x00023368 File Offset: 0x00021568
		internal static void NewMainAuthcodeImage(UwaWebsiteClient.OnDoneCallback onDone)
		{
			UwaWebsiteClient.sErrorMessage = null;
			Uri address = new Uri(string.Format("{0}/api/m/authcode/image?width=120&height=32&_cache=" + DateTime.UtcNow.ToString(), UwaWebsiteClient.UwaWebsiteUrl));
			UwaWebsiteWebClient uwaWebsiteWebClient = new UwaWebsiteWebClient();
			UwaWebsiteClient.Pending pending = new UwaWebsiteClient.Pending();
			pending.conn = uwaWebsiteWebClient;
			pending.id = "get_authcode";
			pending.callback = UwaWebsiteClient.WrapMainAuthCodeImageForLoginCallback(onDone);
			UwaWebsiteClient.Pendings.Add(pending);
			uwaWebsiteWebClient.Headers.Add("Accept", "image/jpeg");
			uwaWebsiteWebClient.CookieContainer = UwaWebsiteClient.cookcontanerInstance;
			uwaWebsiteWebClient.DownloadDataCompleted += UwaWebsiteClient.DownloadDataCallback;
			try
			{
				uwaWebsiteWebClient.DownloadDataAsync(address, pending);
			}
			catch (WebException ex)
			{
				pending.ex = ex;
			}
		}

		// Token: 0x060003DD RID: 989 RVA: 0x00023440 File Offset: 0x00021640
		internal static void MainAuthcodeImage(bool project, UwaWebsiteClient.OnDoneCallback onDone)
		{
			if (project)
			{
				UwaWebsiteClient.OldMainAuthcodeImage(project, onDone);
			}
			else
			{
				UwaWebsiteClient.NewMainAuthcodeImage(onDone);
			}
		}

		// Token: 0x060003DE RID: 990 RVA: 0x00023470 File Offset: 0x00021670
		internal static void OldMainAuthcodeImage(bool project, UwaWebsiteClient.OnDoneCallback onDone)
		{
			UwaWebsiteClient.sErrorMessage = null;
			Uri address = new Uri(string.Format("{0}/authcode/image?width=176&height=38&_cache=" + DateTime.UtcNow.ToString(), UwaWebsiteClient.UwaWebsiteUrl));
			UwaWebsiteWebClient uwaWebsiteWebClient = new UwaWebsiteWebClient();
			UwaWebsiteClient.Pending pending = new UwaWebsiteClient.Pending();
			pending.conn = uwaWebsiteWebClient;
			pending.id = "get_authcode";
			if (project)
			{
				pending.callback = UwaWebsiteClient.WrapMainAuthCodeImageForProjectCallback(onDone);
			}
			else
			{
				pending.callback = UwaWebsiteClient.WrapMainAuthCodeImageForLoginCallback(onDone);
			}
			UwaWebsiteClient.Pendings.Add(pending);
			uwaWebsiteWebClient.Headers.Add("Accept", "image/jpeg");
			uwaWebsiteWebClient.CookieContainer = UwaWebsiteClient.cookcontanerInstance;
			uwaWebsiteWebClient.DownloadDataCompleted += UwaWebsiteClient.DownloadDataCallback;
			try
			{
				uwaWebsiteWebClient.DownloadDataAsync(address, pending);
			}
			catch (WebException ex)
			{
				pending.ex = ex;
			}
		}

		// Token: 0x060003DF RID: 991 RVA: 0x00023564 File Offset: 0x00021764
		internal static void MainUserIcon(UwaWebsiteClient.OnDoneCallback onGetAllCallback)
		{
			UwaWebsiteClient.sErrorMessage = null;
			Uri address = new Uri(UwaWebsiteClient.userProfile.IconUrl);
			UwaWebsiteWebClient uwaWebsiteWebClient = new UwaWebsiteWebClient();
			UwaWebsiteClient.Pending pending = new UwaWebsiteClient.Pending();
			pending.conn = uwaWebsiteWebClient;
			pending.id = "get_usericon";
			pending.callback = UwaWebsiteClient.WrapMainUserIconCallback(onGetAllCallback);
			UwaWebsiteClient.Pendings.Add(pending);
			uwaWebsiteWebClient.Headers.Add("Accept", "image/jpeg");
			uwaWebsiteWebClient.DownloadDataCompleted += UwaWebsiteClient.DownloadDataCallback;
			try
			{
				uwaWebsiteWebClient.DownloadDataAsync(address, pending);
			}
			catch (WebException ex)
			{
				pending.ex = ex;
			}
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x00023618 File Offset: 0x00021818
		internal static void GotCheck(UwaWebsiteClient.OnDoneCallback2 onDone)
		{
			bool flag = UwaWebsiteClient.gotDataSubmitInfo.state != UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Uploaded;
			if (!flag)
			{
				UwaWebsiteClient.sErrorMessage = null;
				bool flag2 = UwaWebsiteClient.gotDataSubmitInfo.selected.dataType == 9 || UwaWebsiteClient.gotDataSubmitInfo.selected.dataType == 8;
				bool flag3 = UwaWebsiteClient.gotDataSubmitInfo.selected.dataType == 7;
				bool flag4 = UwaWebsiteClient.gotDataSubmitInfo.selected.dataType == 50;
				UwaWebsiteClient.gotDataSubmitInfo.lastRecordType = (UwaWebsiteClient.ProjectType)UwaWebsiteClient.gotDataSubmitInfo.selected.dataType;
				string text = string.Format("{0}/gotonline/v1/record/batch", UwaWebsiteClient.UwaGotServerUrl);
				bool flag5 = flag2;
				if (flag5)
				{
					text = string.Format("{0}/gpm/v1/record/batch", UwaWebsiteClient.UwaGotServerUrl);
				}
				bool flag6 = flag3;
				if (flag6)
				{
					text = string.Format("{0}/lt/v1/record", UwaWebsiteClient.UwaGotServerUrl);
				}
				bool flag7 = flag4;
				if (flag7)
				{
					text = string.Format("{0}/ab/v1/record/batch", UwaWebsiteClient.UwaGotServerUrl);
				}
				Uri address = new Uri(text);
				UwaWebsiteWebClient uwaWebsiteWebClient = new UwaWebsiteWebClient();
				string value = "v1.0.3";
				bool flag8 = flag4;
				if (flag8)
				{
					value = "v1.0.1";
				}
				bool flag9 = flag2;
				if (flag9)
				{
					value = "v1.0.2";
				}
				JSONValue jsonvalue = default(JSONValue);
				JSONValue body = default(JSONValue);
				bool flag10 = flag3;
				if (flag10)
				{
					jsonvalue["dataKey"] = UwaWebsiteClient.gotDataSubmitInfo.selected.dataHash;
					jsonvalue["fileKey"] = UwaWebsiteClient.gotDataSubmitInfo.fileUrl;
					jsonvalue["platform"] = UwaWebsiteClient.gotDataSubmitInfo.selected.platform;
					jsonvalue["uploadSource"] = UwaWebsiteClient.GotDataSubmitInfo.uploadSource;
					jsonvalue["projectGroupId"] = UwaWebsiteClient.gotDataSubmitInfo.selected.selectedProjectId;
					jsonvalue["userNote"] = UwaWebsiteClient.gotDataSubmitInfo.selected.userNote;
					bool flag11 = UwaWebsiteClient.gotDataSubmitInfo.selected.license != null;
					if (flag11)
					{
						jsonvalue["license"] = UwaWebsiteClient.gotDataSubmitInfo.selected.license;
					}
					body = jsonvalue;
				}
				else
				{
					bool flag12 = flag4;
					if (flag12)
					{
						jsonvalue["dataFilePath"] = UwaWebsiteClient.gotDataSubmitInfo.fileUrl;
						jsonvalue["engineVersion"] = UwaWebsiteClient.gotDataSubmitInfo.selected.engineVersion;
						jsonvalue["platform"] = UwaWebsiteClient.gotDataSubmitInfo.selected.platform;
						jsonvalue["userNote"] = UwaWebsiteClient.gotDataSubmitInfo.selected.userNote;
						body["recordList"] = new List<JSONValue>();
						body["recordList"].AsList(false).Add(jsonvalue);
						body["projectGroupId"] = UwaWebsiteClient.gotDataSubmitInfo.selected.selectedProjectId;
					}
					else
					{
						jsonvalue["dataKey"] = UwaWebsiteClient.gotDataSubmitInfo.selected.dataHash;
						jsonvalue["fileKey"] = UwaWebsiteClient.gotDataSubmitInfo.fileUrl;
						jsonvalue["platform"] = UwaWebsiteClient.gotDataSubmitInfo.selected.platform;
						jsonvalue["duration"] = UwaWebsiteClient.gotDataSubmitInfo.selected.duration.ToString();
						jsonvalue["durationUnitOfMeasure"] = "second";
						jsonvalue["deviceModel"] = UwaWebsiteClient.gotDataSubmitInfo.selected.deviceModel;
						jsonvalue["serviceSubtype"] = UwaWebsiteClient.dataTypeDic[UwaWebsiteClient.gotDataSubmitInfo.selected.dataType];
						jsonvalue["userNote"] = UwaWebsiteClient.gotDataSubmitInfo.selected.userNote;
						bool flag13 = UwaWebsiteClient.gotDataSubmitInfo.selected.license != null;
						if (flag13)
						{
							jsonvalue["license"] = UwaWebsiteClient.gotDataSubmitInfo.selected.license;
						}
						body["projectGroupId"] = UwaWebsiteClient.gotDataSubmitInfo.selected.selectedProjectId;
						bool flag14 = flag2;
						if (flag14)
						{
							body["createDetailList"] = new List<JSONValue>();
							body["createDetailList"].AsList(false).Add(jsonvalue);
						}
						else
						{
							body["gotOnlineCreateDetail"] = new List<JSONValue>();
							body["gotOnlineCreateDetail"].AsList(false).Add(jsonvalue);
							bool flag15 = UwaWebsiteClient.gotDataSubmitInfo.selected.selectProject.SelectedBalance != null;
							if (flag15)
							{
								body["serviceTypeCode"] = UwaWebsiteClient.gotDataSubmitInfo.selected.selectProject.SelectedBalance.serviceTypeCode.ToString();
							}
						}
					}
				}
				string text2 = body.ToString();
				UwaWebsiteClient.Log(text2);
				UwaWebsiteClient.AddSign(uwaWebsiteWebClient, text, body);
				UwaWebsiteClient.Pending pending = new UwaWebsiteClient.Pending();
				pending.conn = uwaWebsiteWebClient;
				pending.id = "submit_got_data(check)";
				pending.callback = UwaWebsiteClient.WrapGotCheckCallback(onDone);
				UwaWebsiteClient.Pendings.Add(pending);
				uwaWebsiteWebClient.Headers.Add("Accept", "application/json");
				uwaWebsiteWebClient.Headers.Add("Content-type", "application/json");
				uwaWebsiteWebClient.Headers.Add("x-uwa-api-minor-version", value);
				uwaWebsiteWebClient.CookieContainer = UwaWebsiteClient.cookcontanerInstance;
				uwaWebsiteWebClient.UploadStringCompleted += UwaWebsiteClient.UploadStringCallback;
				try
				{
					uwaWebsiteWebClient.UploadStringAsync(address, "POST", text2, pending);
					UwaWebsiteClient.gotDataSubmitInfo.state = UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Submitting;
				}
				catch (WebException ex)
				{
					pending.ex = ex;
				}
			}
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x00023C94 File Offset: 0x00021E94
		private static string NameValueCollectionToString(NameValueCollection c)
		{
			string[] allKeys = c.AllKeys;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < allKeys.Length; i++)
			{
				stringBuilder.Append(allKeys[i]).Append(":").Append(c[allKeys[i]]).AppendLine();
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x00023D08 File Offset: 0x00021F08
		internal static void JenkinsDeploy(string buildGuid, UwaWebsiteClient.ProjectType type, int projectId, int recordId, UwaWebsiteClient.OnDoneCallback onDone)
		{
			UwaWebsiteClient.Log(string.Concat(new string[]
			{
				"JenkinsDeploy(",
				buildGuid,
				",",
				type.ToString(),
				",",
				projectId.ToString(),
				",",
				recordId.ToString(),
				")"
			}));
			bool flag = type != UwaWebsiteClient.ProjectType.PA && type != UwaWebsiteClient.ProjectType.RT && type != UwaWebsiteClient.ProjectType.PIPELINE;
			if (flag)
			{
				onDone(1, "only support pa or rt or pipeline");
			}
			else
			{
				UwaWebsiteClient.sErrorMessage = null;
				string text = string.Format("{0}/pipeline/v1/deployment/record", UwaWebsiteClient.UwaGotServerUrl);
				UwaWebsiteClient.Log(text);
				Uri address = new Uri(text);
				UwaWebsiteWebClient uwaWebsiteWebClient = new UwaWebsiteWebClient();
				string[] array = buildGuid.Split(new string[]
				{
					"#$*"
				}, StringSplitOptions.RemoveEmptyEntries);
				int s = 0;
				bool flag2 = array.Length != 4;
				if (flag2)
				{
					onDone(1, "build id length != 4.");
				}
				else
				{
					bool flag3 = !int.TryParse(array[3], out s);
					if (flag3)
					{
						onDone(1, "deploySequence can not parse.");
					}
					else
					{
						string s2 = type.ToString();
						bool flag4 = type == UwaWebsiteClient.ProjectType.PIPELINE;
						if (flag4)
						{
							s2 = "LT";
						}
						bool flag5 = type == UwaWebsiteClient.ProjectType.ASSET || type == UwaWebsiteClient.ProjectType.LUA || type == UwaWebsiteClient.ProjectType.MONO || type == UwaWebsiteClient.ProjectType.PERF;
						if (flag5)
						{
							s2 = "GOT_ONLINE";
						}
						JSONValue body = default(JSONValue);
						body["version"] = 2;
						body["pipelineId"] = array[0];
						body["pipelineName"] = array[1];
						body["branchName"] = array[2];
						body["deploySequence"] = s;
						body["projectGroupId"] = projectId;
						body["recordId"] = recordId;
						body["serviceMetaType"] = s2;
						string text2 = body.ToString();
						UwaWebsiteClient.Log(text2);
						UwaWebsiteClient.AddSign(uwaWebsiteWebClient, text, body);
						UwaWebsiteClient.Pending pending = new UwaWebsiteClient.Pending();
						pending.conn = uwaWebsiteWebClient;
						pending.id = "jenkins_deploy";
						pending.callback = UwaWebsiteClient.WrapJenkinsDeployCallback(onDone);
						UwaWebsiteClient.Pendings.Add(pending);
						uwaWebsiteWebClient.Headers.Add("Accept", "application/json");
						uwaWebsiteWebClient.Headers.Add("Content-type", "application/json");
						uwaWebsiteWebClient.Headers.Add("x-uwa-api-minor-version", "v1.0.1");
						uwaWebsiteWebClient.CookieContainer = UwaWebsiteClient.cookcontanerInstance;
						uwaWebsiteWebClient.UploadStringCompleted += UwaWebsiteClient.UploadStringCallback;
						try
						{
							uwaWebsiteWebClient.UploadStringAsync(address, "POST", text2, pending);
						}
						catch (WebException ex)
						{
							pending.ex = ex;
						}
					}
				}
			}
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x00024044 File Offset: 0x00022244
		private static void DownloadDataCallback(object sender, DownloadDataCompletedEventArgs e)
		{
			UwaWebsiteClient.Pending pending = (UwaWebsiteClient.Pending)e.UserState;
			bool flag = e.Error != null;
			if (flag)
			{
				pending.ex = e.Error;
			}
			else
			{
				bool flag2 = !e.Cancelled;
				if (flag2)
				{
					pending.data = Encoding.UTF8.GetString(e.Result);
					pending.binData = e.Result;
				}
				else
				{
					pending.data = string.Empty;
				}
			}
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x000240CC File Offset: 0x000222CC
		private static void UploadStringCallback(object sender, UploadStringCompletedEventArgs e)
		{
			UwaWebsiteClient.Pending pending = (UwaWebsiteClient.Pending)e.UserState;
			bool flag = e.Error != null;
			if (flag)
			{
				pending.ex = e.Error;
			}
			else
			{
				bool flag2 = !e.Cancelled;
				if (flag2)
				{
					pending.data = e.Result;
				}
				else
				{
					pending.data = string.Empty;
				}
			}
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x0002413C File Offset: 0x0002233C
		private static void UploadValuesCallback(object sender, UploadValuesCompletedEventArgs e)
		{
			UwaWebsiteClient.Pending pending = (UwaWebsiteClient.Pending)e.UserState;
			bool flag = e.Error != null;
			if (flag)
			{
				pending.ex = e.Error;
			}
			else
			{
				bool flag2 = !e.Cancelled;
				if (flag2)
				{
					pending.data = Encoding.UTF8.GetString(e.Result);
				}
				else
				{
					pending.data = string.Empty;
				}
			}
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x000241B8 File Offset: 0x000223B8
		private static UwaWebsiteClient.OnResponseCallback WrapMainUserIconCallback(UwaWebsiteClient.OnDoneCallback onDone)
		{
			return delegate(UwaWebResponse resp)
			{
				bool flag = !UwaWebsiteClient.CheckRespStateFailed(resp, out UwaWebsiteClient.sErrorMessage);
				if (flag)
				{
					try
					{
						bool flag2 = UwaWebsiteClient.userProfile != null;
						if (flag2)
						{
							bool flag3 = UwaWebsiteClient.userProfile.IconImage != null;
							if (flag3)
							{
								Object.DestroyImmediate(UwaWebsiteClient.userProfile.IconImage);
							}
							UwaWebsiteClient.userProfile.IconImage = new Texture2D(2, 2);
							UwaWebsiteClient.LoadTexture(UwaWebsiteClient.userProfile.IconImage, resp.binData);
							bool flag4 = UwaWebsiteClient.userProfile.IconImage.width == 8 && UwaWebsiteClient.userProfile.IconImage.height == 8;
							if (flag4)
							{
								UwaWebsiteClient.LoadTexture(UwaWebsiteClient.userProfile.IconImage, UwaWebsiteClient.defaultUserIcon);
							}
						}
					}
					catch (Exception ex)
					{
						UwaWebsiteClient.sErrorMessage = "Usericon image not loaded.";
						UwaWebsiteClient.Log(ex.ToString());
					}
				}
				bool flag5 = onDone != null;
				if (flag5)
				{
					onDone(0, UwaWebsiteClient.sErrorMessage);
				}
			};
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x000241EC File Offset: 0x000223EC
		public static void LoadTexture(Texture2D tex, byte[] data)
		{
			bool isStatic = UwaWebsiteClient.LoadImageMethod.IsStatic;
			if (isStatic)
			{
				UwaWebsiteClient.LoadImageMethod.Invoke(null, new object[]
				{
					tex,
					data
				});
			}
			else
			{
				UwaWebsiteClient.LoadImageMethod.Invoke(tex, new object[]
				{
					data
				});
			}
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x00024244 File Offset: 0x00022444
		public static void LoadTexture(Texture2D tex, string filePath)
		{
			byte[] data = File.ReadAllBytes(filePath);
			UwaWebsiteClient.LoadTexture(tex, data);
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060003E9 RID: 1001 RVA: 0x00024268 File Offset: 0x00022468
		private static MethodInfo LoadImageMethod
		{
			get
			{
				try
				{
					bool flag = UwaWebsiteClient._loadImageMethod == null;
					if (flag)
					{
						Type type = Assembly.GetAssembly(typeof(GameObject)).GetType("UnityEngine.ImageConversion");
						bool flag2 = type != null;
						if (flag2)
						{
							UwaWebsiteClient._loadImageMethod = type.GetMethod("LoadImage", BindingFlags.Static | BindingFlags.Public, null, new Type[]
							{
								typeof(Texture2D),
								typeof(byte[])
							}, null);
						}
						bool flag3 = UwaWebsiteClient._loadImageMethod == null;
						if (flag3)
						{
							Assembly assembly = null;
							Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
							for (int i = 0; i < assemblies.Length; i++)
							{
								bool flag4 = assemblies[i].FullName.Contains("UnityEngine.ImageConversion");
								if (flag4)
								{
									assembly = assemblies[i];
									break;
								}
							}
							bool flag5 = assembly != null;
							if (flag5)
							{
								type = assembly.GetType("UnityEngine.ImageConversion");
								UwaWebsiteClient._loadImageMethod = type.GetMethod("LoadImage", BindingFlags.Static | BindingFlags.Public, null, new Type[]
								{
									typeof(Texture2D),
									typeof(byte[])
								}, null);
							}
						}
						bool flag6 = UwaWebsiteClient._loadImageMethod == null;
						if (flag6)
						{
							type = typeof(Texture2D);
							UwaWebsiteClient._loadImageMethod = type.GetMethod("LoadImage", BindingFlags.Instance | BindingFlags.Public, null, new Type[]
							{
								typeof(byte[])
							}, null);
						}
					}
				}
				catch (Exception ex)
				{
					string str = "error in LoadTexture : ";
					Exception ex2 = ex;
					UwaWebsiteClient.Log(str + ((ex2 != null) ? ex2.ToString() : null));
				}
				return UwaWebsiteClient._loadImageMethod;
			}
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x00024444 File Offset: 0x00022644
		private static UwaWebsiteClient.OnResponseCallback WrapMainAuthCodeImageForLoginCallback(UwaWebsiteClient.OnDoneCallback onDone)
		{
			return delegate(UwaWebResponse resp)
			{
				bool flag = !UwaWebsiteClient.CheckRespStateFailed(resp, out UwaWebsiteClient.sErrorMessage);
				if (flag)
				{
					try
					{
						bool flag2 = UwaWebsiteClient.LoginAuthImage != null;
						if (flag2)
						{
							Object.DestroyImmediate(UwaWebsiteClient.LoginAuthImage);
						}
						UwaWebsiteClient.LoginAuthImage = new Texture2D(2, 2, 3, false);
						UwaWebsiteClient.LoadTexture(UwaWebsiteClient.LoginAuthImage, resp.binData);
					}
					catch (Exception ex)
					{
						UwaWebsiteClient.sErrorMessage = "Authcode image not loaded.";
						UwaWebsiteClient.Log(ex.ToString());
					}
				}
				bool flag3 = onDone != null;
				if (flag3)
				{
					onDone(0, UwaWebsiteClient.sErrorMessage);
				}
			};
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00024478 File Offset: 0x00022678
		private static UwaWebsiteClient.OnResponseCallback WrapMainAuthCodeImageForProjectCallback(UwaWebsiteClient.OnDoneCallback onDone)
		{
			return delegate(UwaWebResponse resp)
			{
				bool flag = !UwaWebsiteClient.CheckRespStateFailed(resp, out UwaWebsiteClient.sErrorMessage);
				if (flag)
				{
					try
					{
						bool flag2 = UwaWebsiteClient.paUploadInfo.authCodeImage != null;
						if (flag2)
						{
							Object.DestroyImmediate(UwaWebsiteClient.paUploadInfo.authCodeImage);
						}
						UwaWebsiteClient.paUploadInfo.authCodeImage = new Texture2D(2, 2);
						UwaWebsiteClient.LoadTexture(UwaWebsiteClient.paUploadInfo.authCodeImage, resp.binData);
					}
					catch (Exception ex)
					{
						UwaWebsiteClient.sErrorMessage = "Authcode image not loaded.";
						UwaWebsiteClient.Log(ex.ToString());
					}
				}
				bool flag3 = onDone != null;
				if (flag3)
				{
					onDone(0, UwaWebsiteClient.sErrorMessage);
				}
			};
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x000244AC File Offset: 0x000226AC
		private static bool CheckError(JSONValue jsonvalue, out int errorCode, out string errorInfo)
		{
			errorInfo = null;
			errorCode = 0;
			bool flag = !jsonvalue.ContainsKey("status");
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = jsonvalue["status"].AsString(false).Equals("success");
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = jsonvalue["status"].AsString(false).Equals("failed");
					if (flag3)
					{
						try
						{
							errorCode = (int)jsonvalue["code"].AsFloat(false);
							errorInfo = jsonvalue["message"].AsString(false);
							UwaWebsiteClient.Log("CHECK ERROR: [" + errorCode.ToString() + "] " + errorInfo);
						}
						catch (Exception)
						{
						}
					}
					bool flag4 = jsonvalue.ContainsKey("error");
					if (flag4)
					{
						errorCode = (int)jsonvalue["error"]["code"].AsFloat(false);
						errorInfo = jsonvalue["error"]["message"].AsString(false);
						UwaWebsiteClient.Log("CHECK ERROR: [" + errorCode.ToString() + "] " + errorInfo);
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x00024624 File Offset: 0x00022824
		private static bool CheckRespStateFailed(UwaWebResponse resp, out string errorInfo)
		{
			errorInfo = null;
			bool flag = !resp.ok;
			bool result;
			if (flag)
			{
				UwaWebsiteClient.iErrorCode = -1;
				errorInfo = "Network Error : " + (resp.HttpErrorMessage ?? "Unknown http error");
				UwaWebsiteClient.LogError(errorInfo);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x00024688 File Offset: 0x00022888
		private static void DoPostUpload(JSONValue jsonvalue, string zipPath, bool sync)
		{
			UwaWebsiteClient.Log("DoPostUpload");
			JSONValue jsonvalue2 = jsonvalue["data"];
			string arg = "uwa-private";
			string text = jsonvalue2["fileKey"];
			bool flag = UwaWebsiteClient.gotDataSubmitInfo.selected.dataType == 50;
			if (flag)
			{
				UwaWebsiteClient.gotDataSubmitInfo.fileUrl = text;
				text += Path.GetFileName(zipPath);
				arg = "uwa-rc-report";
			}
			else
			{
				text = text + UwaWebsiteClient.gotDataSubmitInfo.selected.dataHash + ".zip";
				UwaWebsiteClient.gotDataSubmitInfo.fileUrl = text;
			}
			jsonvalue2["uploadPath"] = text;
			jsonvalue2["meta"] = UwaWebsiteClient.gotDataSubmitInfo.selected.meta;
			string url = string.Format("https://{0}.oss-cn-beijing.aliyuncs.com/", arg);
			PostUploader postUploader = new PostUploader(PostUploader.Target.OSS);
			postUploader.UploadMultipartCompleted += UwaWebsiteClient.WrapUploaderPartCallback;
			postUploader.UploadCompleted += UwaWebsiteClient.WrapUploaderCompletedCallback;
			if (sync)
			{
				UwaWebsiteClient.Log("PostUploader sync start");
				postUploader.MultipartUploadProgress(url, jsonvalue2, zipPath, null);
			}
			else
			{
				UwaWebsiteClient.Log("PostUploader async start");
				postUploader.MultipartUploadProgressAsync(url, jsonvalue2, zipPath, null);
			}
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x000247E8 File Offset: 0x000229E8
		private static UwaWebsiteClient.OnResponseCallback WrapGotUploadInfoCallback(UwaWebsiteClient.OnDoneCallback onDone)
		{
			return delegate(UwaWebResponse resp)
			{
				UwaWebsiteClient.iErrorCode = 0;
				UwaWebsiteClient.gotDataSubmitInfo.state = UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Idle;
				bool flag = !UwaWebsiteClient.CheckRespStateFailed(resp, out UwaWebsiteClient.sErrorMessage);
				if (flag)
				{
					try
					{
						JSONValue jsonvalue = JSONParser.SimpleParse(resp.data);
						bool flag2 = !UwaWebsiteClient.CheckError(jsonvalue, out UwaWebsiteClient.iErrorCode, out UwaWebsiteClient.sErrorMessage);
						if (flag2)
						{
							bool flag3 = jsonvalue.ContainsKey("message") && jsonvalue["message"] == "已经上传过了！";
							if (flag3)
							{
								UwaWebsiteClient.iErrorCode = 1000;
								UwaWebsiteClient.sErrorMessage = "Already synced";
							}
							else
							{
								bool sync = UwaWebsiteClient.gotDataSubmitInfo.selected.zipPath.Count > 1;
								for (int i = 0; i < UwaWebsiteClient.gotDataSubmitInfo.selected.zipPath.Count; i++)
								{
									UwaWebsiteClient.gotDataSubmitInfo.state = UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Uploading;
									string text = UwaWebsiteClient.gotDataSubmitInfo.selected.zipPath[i];
									bool flag4 = File.Exists(text);
									if (!flag4)
									{
										UwaWebsiteClient.sErrorMessage = "File not found.";
										List<string> zipPath = UwaWebsiteClient.gotDataSubmitInfo.selected.zipPath;
										throw new FileNotFoundException(((zipPath != null) ? zipPath.ToString() : null) + " is not found.");
									}
									bool flag5 = UwaWebsiteClient.WebSite == Localization.eWebSite.US || UwaWebsiteClient.WebSite == Localization.eWebSite.JP;
									if (flag5)
									{
										string fileUrl = jsonvalue["signature"]["key"];
										string text2 = jsonvalue["signature"]["bucket"];
										UwaWebsiteClient.gotDataSubmitInfo.fileUrl = fileUrl;
										string url = null;
										bool flag6 = UwaWebsiteClient.WebSite == Localization.eWebSite.US;
										if (flag6)
										{
											url = string.Format("https://{0}.s3-us-east-2.amazonaws.com/", text2.Replace("\"", ""));
										}
										else
										{
											bool flag7 = UwaWebsiteClient.WebSite == Localization.eWebSite.JP;
											if (flag7)
											{
												url = string.Format("https://{0}.s3-ap-northeast-1.amazonaws.com/", text2.Replace("\"", ""));
											}
										}
										PostUploader postUploader = new PostUploader(PostUploader.Target.AWS);
										postUploader.UploadMultipartCompleted += UwaWebsiteClient.WrapUploaderPartCallback;
										postUploader.UploadCompleted += UwaWebsiteClient.WrapUploaderCompletedCallback;
										postUploader.MultipartUploadProgressAsync(url, resp.data, text, null);
										UwaWebsiteClient.Log("PostUploader start");
									}
									else
									{
										UwaWebsiteClient.Log(jsonvalue.ToString());
										UwaWebsiteClient.DoPostUpload(jsonvalue, text, sync);
									}
								}
							}
						}
					}
					catch (Exception ex)
					{
						UwaWebsiteClient.Log(ex.ToString());
					}
				}
				bool flag8 = onDone != null;
				if (flag8)
				{
					onDone(UwaWebsiteClient.iErrorCode, UwaWebsiteClient.sErrorMessage);
				}
			};
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x0002481C File Offset: 0x00022A1C
		private static UwaWebsiteClient.OnResponseCallback WrapGotCommonInfoCallback(int type, UwaWebsiteClient.OnDoneCallback3 onDone)
		{
			return delegate(UwaWebResponse resp)
			{
				UwaWebsiteClient.iErrorCode = 0;
				int projectId = -1;
				bool flag = !UwaWebsiteClient.CheckRespStateFailed(resp, out UwaWebsiteClient.sErrorMessage);
				if (flag)
				{
					try
					{
						JSONValue jsonvalue = JSONParser.SimpleParse(resp.data);
						bool flag2 = !UwaWebsiteClient.CheckError(jsonvalue, out UwaWebsiteClient.iErrorCode, out UwaWebsiteClient.sErrorMessage);
						if (flag2)
						{
							bool flag3 = jsonvalue.ContainsKey("Id");
							if (flag3)
							{
								projectId = (int)jsonvalue["Id"].AsFloat(false);
							}
						}
						else
						{
							UwaWebsiteClient.iErrorCode = 1;
							UwaWebsiteClient.sErrorMessage = "Can not create";
						}
					}
					catch (Exception ex)
					{
						UwaWebsiteClient.Log(ex.ToString());
					}
				}
				bool flag4 = onDone != null;
				if (flag4)
				{
					onDone(type, projectId, UwaWebsiteClient.iErrorCode, UwaWebsiteClient.sErrorMessage);
				}
			};
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x00024858 File Offset: 0x00022A58
		private static void WrapUploaderPartCallback(object sender, UploadTool.UploadMultipartCompletedEventArgs arg)
		{
			UwaWebsiteClient.gotDataSubmitInfo.lastPercent = arg.Percent;
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x0002486C File Offset: 0x00022A6C
		private static void WrapUploaderCompletedCallback(object sender, UploadTool.UploadCompletedEventArgs arg)
		{
			UwaWebsiteClient.gotDataSubmitInfo.fileETag = arg.ETag;
			UwaWebsiteClient.gotDataSubmitInfo.skip = arg.Skip;
			UwaWebsiteClient.gotDataSubmitInfo.state = ((arg.ETag != null) ? UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Uploaded : UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Idle);
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x000248AC File Offset: 0x00022AAC
		private static UwaWebsiteClient.OnResponseCallback WrapProjectBalanceCallback(int projectId, UwaWebsiteClient.OnDoneCallback onDone)
		{
			return delegate(UwaWebResponse resp)
			{
				UwaWebsiteClient.iErrorCode = 0;
				bool flag = !UwaWebsiteClient.CheckRespStateFailed(resp, out UwaWebsiteClient.sErrorMessage);
				if (flag)
				{
					try
					{
						JSONValue jsonvalue = JSONParser.SimpleParse(resp.data);
						bool flag2 = !UwaWebsiteClient.CheckError(jsonvalue, out UwaWebsiteClient.iErrorCode, out UwaWebsiteClient.sErrorMessage);
						if (flag2)
						{
							List<JSONValue> list = jsonvalue["data"];
							UwaWebsiteClient.GroupProjectInfo groupProjectInfo = UwaWebsiteClient.userProfile.FindById(projectId);
							bool flag3 = groupProjectInfo != null;
							if (flag3)
							{
								groupProjectInfo.Balances = new List<UwaWebsiteClient.BalanceInfo>();
								groupProjectInfo.SelectedBalance = null;
								for (int i = 0; i < list.Count; i++)
								{
									UwaWebsiteClient.BalanceInfo balanceInfo = new UwaWebsiteClient.BalanceInfo();
									list[i].Deserialize(balanceInfo);
									groupProjectInfo.Balances.Add(balanceInfo);
								}
								groupProjectInfo.UpdateDefaultBalance();
							}
						}
					}
					catch (Exception ex)
					{
						UwaWebsiteClient.Log(ex.ToString());
					}
				}
				bool flag4 = onDone != null;
				if (flag4)
				{
					onDone(UwaWebsiteClient.iErrorCode, UwaWebsiteClient.sErrorMessage);
				}
			};
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x000248E8 File Offset: 0x00022AE8
		private static UwaWebsiteClient.OnResponseCallback WrapGetCreateProjectCheckCallback(UwaWebsiteClient.OnDoneCallback5 onDone)
		{
			return delegate(UwaWebResponse resp)
			{
				bool available = false;
				UwaWebsiteClient.iErrorCode = 0;
				bool flag = !UwaWebsiteClient.CheckRespStateFailed(resp, out UwaWebsiteClient.sErrorMessage);
				if (flag)
				{
					try
					{
						JSONValue jsonvalue = JSONParser.SimpleParse(resp.data);
						available = !UwaWebsiteClient.CheckError(jsonvalue, out UwaWebsiteClient.iErrorCode, out UwaWebsiteClient.sErrorMessage);
					}
					catch (Exception ex)
					{
						UwaWebsiteClient.Log(ex.ToString());
					}
				}
				bool flag2 = onDone != null;
				if (flag2)
				{
					onDone(available, UwaWebsiteClient.iErrorCode, UwaWebsiteClient.sErrorMessage);
				}
			};
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x0002491C File Offset: 0x00022B1C
		private static UwaWebsiteClient.OnResponseCallback WrapOnCreateProjectCallback(UwaWebsiteClient.OnDoneCallback2 onDone)
		{
			return delegate(UwaWebResponse resp)
			{
				UwaWebsiteClient.iErrorCode = 0;
				int mode = -1;
				bool flag = !UwaWebsiteClient.CheckRespStateFailed(resp, out UwaWebsiteClient.sErrorMessage);
				if (flag)
				{
					try
					{
						JSONValue jsonvalue = JSONParser.SimpleParse(resp.data);
						bool flag2 = !UwaWebsiteClient.CheckError(jsonvalue, out UwaWebsiteClient.iErrorCode, out UwaWebsiteClient.sErrorMessage);
						if (flag2)
						{
							JSONValue jsonvalue2 = jsonvalue["data"];
							bool flag3 = jsonvalue2.IsFloat();
							if (flag3)
							{
								mode = (int)jsonvalue2.AsFloat(false);
							}
						}
					}
					catch (Exception ex)
					{
						UwaWebsiteClient.Log(ex.ToString());
					}
				}
				bool flag4 = onDone != null;
				if (flag4)
				{
					onDone(mode, UwaWebsiteClient.iErrorCode, UwaWebsiteClient.sErrorMessage);
				}
			};
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00024950 File Offset: 0x00022B50
		private static UwaWebsiteClient.OnResponseCallback WrapGotProjectAllCallback(UwaWebsiteClient.OnDoneCallback onDone)
		{
			return delegate(UwaWebResponse resp)
			{
				UwaWebsiteClient.iErrorCode = 0;
				bool flag = !UwaWebsiteClient.CheckRespStateFailed(resp, out UwaWebsiteClient.sErrorMessage);
				if (flag)
				{
					try
					{
						JSONValue jsonvalue = JSONParser.SimpleParse(resp.data);
						bool flag2 = !UwaWebsiteClient.CheckError(jsonvalue, out UwaWebsiteClient.iErrorCode, out UwaWebsiteClient.sErrorMessage);
						if (flag2)
						{
							UwaWebsiteClient.userProfile.temp_got_project_array = new Dictionary<int, UwaWebsiteClient.GroupProjectInfo>();
							List<JSONValue> list = jsonvalue["data"];
							for (int i = 0; i < list.Count; i++)
							{
								UwaWebsiteClient.GroupProjectInfo groupProjectInfo = new UwaWebsiteClient.GroupProjectInfo();
								list[i].Deserialize(groupProjectInfo);
								UwaWebsiteClient.userProfile.temp_got_project_array.Add(groupProjectInfo.projectGroupId, groupProjectInfo);
							}
						}
					}
					catch (Exception ex)
					{
						UwaWebsiteClient.Log(ex.ToString());
					}
				}
				bool flag3 = onDone != null;
				if (flag3)
				{
					onDone(UwaWebsiteClient.iErrorCode, UwaWebsiteClient.sErrorMessage);
				}
			};
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x00024984 File Offset: 0x00022B84
		private static UwaWebsiteClient.OnResponseCallback WrapJenkinsDeployCallback(UwaWebsiteClient.OnDoneCallback onDone)
		{
			return delegate(UwaWebResponse resp)
			{
				UwaWebsiteClient.submitting = false;
				UwaWebsiteClient.iErrorCode = 0;
				bool flag = !UwaWebsiteClient.CheckRespStateFailed(resp, out UwaWebsiteClient.sErrorMessage);
				if (flag)
				{
					try
					{
						JSONValue jsonvalue = JSONParser.SimpleParse(resp.data);
						bool flag2 = !UwaWebsiteClient.CheckError(jsonvalue, out UwaWebsiteClient.iErrorCode, out UwaWebsiteClient.sErrorMessage);
						if (flag2)
						{
							UwaWebsiteClient.sErrorMessage = "Success.";
						}
					}
					catch (Exception ex)
					{
						UwaWebsiteClient.Log(ex.ToString());
					}
				}
				bool flag3 = onDone != null;
				if (flag3)
				{
					onDone(UwaWebsiteClient.iErrorCode, UwaWebsiteClient.sErrorMessage);
				}
			};
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x000249B8 File Offset: 0x00022BB8
		private static UwaWebsiteClient.OnResponseCallback WrapGotCheckCallback(UwaWebsiteClient.OnDoneCallback2 onDone)
		{
			return delegate(UwaWebResponse resp)
			{
				UwaWebsiteClient.iErrorCode = 0;
				UwaWebsiteClient.gotDataSubmitInfo.state = UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Idle;
				int mode = 0;
				bool flag = !UwaWebsiteClient.CheckRespStateFailed(resp, out UwaWebsiteClient.sErrorMessage);
				if (flag)
				{
					try
					{
						JSONValue jsonvalue = JSONParser.SimpleParse(resp.data);
						bool flag2 = !UwaWebsiteClient.CheckError(jsonvalue, out UwaWebsiteClient.iErrorCode, out UwaWebsiteClient.sErrorMessage);
						if (flag2)
						{
							UwaWebsiteClient.sErrorMessage = "Success.";
							UwaWebsiteClient.gotDataSubmitInfo.state = UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Submit;
							try
							{
								mode = (int)jsonvalue["data"]["recordId"].AsFloat(false);
							}
							catch (Exception)
							{
							}
							try
							{
								List<JSONValue> list = jsonvalue["data"].AsList(false);
								bool flag3 = list.Count == 1;
								if (flag3)
								{
									mode = (int)list[0].AsFloat(false);
								}
							}
							catch (Exception)
							{
							}
						}
					}
					catch (Exception ex)
					{
						UwaWebsiteClient.Log(ex.ToString());
					}
				}
				bool flag4 = onDone != null;
				if (flag4)
				{
					onDone(mode, UwaWebsiteClient.iErrorCode, UwaWebsiteClient.sErrorMessage);
				}
			};
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x000249EC File Offset: 0x00022BEC
		private static UwaWebsiteClient.OnResponseCallback WrapTryLoginCallback(UwaWebsiteClient.OnDoneCallback5 onDone)
		{
			return delegate(UwaWebResponse resp)
			{
				UwaWebsiteClient.iErrorCode = 0;
				bool available = true;
				bool flag = !UwaWebsiteClient.CheckRespStateFailed(resp, out UwaWebsiteClient.sErrorMessage);
				if (flag)
				{
					try
					{
						JSONValue jsonvalue = JSONParser.SimpleParse(resp.data);
						bool flag2 = !UwaWebsiteClient.CheckError(jsonvalue, out UwaWebsiteClient.iErrorCode, out UwaWebsiteClient.sErrorMessage);
						if (flag2)
						{
							JSONValue jsonvalue2 = jsonvalue["data"]["user"]["active"];
							bool flag3 = jsonvalue2.IsNull();
							if (flag3)
							{
								available = false;
							}
							else
							{
								bool flag4 = jsonvalue2.IsFloat();
								if (flag4)
								{
									available = ((int)jsonvalue2.AsFloat(false) == 1);
								}
							}
						}
					}
					catch (Exception ex)
					{
						UwaWebsiteClient.Log(ex.ToString());
					}
				}
				bool flag5 = onDone != null;
				if (flag5)
				{
					onDone(available, UwaWebsiteClient.iErrorCode, UwaWebsiteClient.sErrorMessage);
				}
			};
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00024A20 File Offset: 0x00022C20
		private static UwaWebsiteClient.OnResponseCallback WrapJenkinsNotifyCallback(UwaWebsiteClient.OnDoneCallback onDone)
		{
			return delegate(UwaWebResponse resp)
			{
				UwaWebsiteClient.iErrorCode = 0;
				bool flag = !UwaWebsiteClient.CheckRespStateFailed(resp, out UwaWebsiteClient.sErrorMessage);
				if (flag)
				{
					try
					{
						JSONValue jsonvalue = JSONParser.SimpleParse(resp.data);
						bool flag2 = !UwaWebsiteClient.CheckError(jsonvalue, out UwaWebsiteClient.iErrorCode, out UwaWebsiteClient.sErrorMessage);
						if (flag2)
						{
						}
					}
					catch (Exception ex)
					{
						UwaWebsiteClient.Log(ex.ToString());
					}
				}
				bool flag3 = onDone != null;
				if (flag3)
				{
					onDone(UwaWebsiteClient.iErrorCode, UwaWebsiteClient.sErrorMessage);
				}
			};
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x00024A54 File Offset: 0x00022C54
		private static UwaWebsiteClient.OnResponseCallback WrapOnTestCallback(UwaWebsiteClient.OnDoneCallback onDone)
		{
			return delegate(UwaWebResponse resp)
			{
				UwaWebsiteClient.iErrorCode = 0;
				bool flag = !UwaWebsiteClient.CheckRespStateFailed(resp, out UwaWebsiteClient.sErrorMessage);
				if (flag)
				{
					try
					{
						JSONValue jsonvalue = JSONParser.SimpleParse(resp.data);
						UwaWebsiteClient.Log(resp.data);
						bool flag2 = !UwaWebsiteClient.CheckError(jsonvalue, out UwaWebsiteClient.iErrorCode, out UwaWebsiteClient.sErrorMessage);
						if (flag2)
						{
							string text = jsonvalue["data"]["key"];
							string text2 = string.Format("https://uwa-private.s3-us-east-2.amazonaws.com/", new object[0]);
							UwaWebsiteClient.Log(text2);
							PostUploader postUploader = new PostUploader(PostUploader.Target.OSS);
							postUploader.UploadCompleted += UwaWebsiteClient.WrapUploaderCompletedCallback;
							postUploader.MultipartUploadProgressAsync(text2, resp.data, Application.persistentDataPath + "/uwa-config.zip", null);
							UwaWebsiteClient.Log("PostUploader start");
						}
					}
					catch (Exception ex)
					{
						UwaWebsiteClient.Log(ex.ToString());
					}
				}
				bool flag3 = onDone != null;
				if (flag3)
				{
					onDone(UwaWebsiteClient.iErrorCode, UwaWebsiteClient.sErrorMessage);
				}
			};
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x00024A88 File Offset: 0x00022C88
		private static UwaWebsiteClient.OnResponseCallback WrapLoginCallback(UwaWebsiteClient.OnDoneCallback onDone)
		{
			return delegate(UwaWebResponse resp)
			{
				UwaWebsiteClient.userProfile = null;
				UwaWebsiteClient.iErrorCode = 0;
				UwaWebsiteClient.NeedAuthCode = false;
				bool flag = resp.HttpHeaders != null;
				if (flag)
				{
					UwaWebsiteClient.ActiveSessionID = resp.HttpHeaders[HttpResponseHeader.SetCookie];
					bool flag2 = UwaWebsiteClient.NotSavedSessionId != null;
					if (flag2)
					{
						UwaWebsiteClient.cookcontanerInstance = new CookieContainer(100);
						UwaWebsiteClient.LoginToken = null;
						try
						{
							UwaWebsiteClient.<>c__DisplayClass96_1 CS$<>8__locals2 = new UwaWebsiteClient.<>c__DisplayClass96_1();
							CS$<>8__locals2.lines = UwaWebsiteClient.NotSavedSessionId.Split(new char[]
							{
								';',
								','
							});
							int j;
							int i;
							for (i = 0; i < CS$<>8__locals2.lines.Length; i = j + 1)
							{
								bool flag3 = UwaWebsiteClient.cookieHeaders.Any((string x) => CS$<>8__locals2.lines[i].Contains(x));
								if (flag3)
								{
									int num = CS$<>8__locals2.lines[i].IndexOf("=");
									bool flag4 = num != -1;
									if (flag4)
									{
										bool flag5 = UwaWebsiteClient.WebSite == Localization.eWebSite.CN && CS$<>8__locals2.lines[i].Substring(0, num) == "uwa_login_token";
										if (flag5)
										{
											UwaWebsiteClient.LoginToken = CS$<>8__locals2.lines[i].Substring(num + 1);
										}
										bool flag6 = UwaWebsiteClient.WebSite == Localization.eWebSite.US && CS$<>8__locals2.lines[i].Substring(0, num) == "uwa_login_token_aws";
										if (flag6)
										{
											UwaWebsiteClient.LoginToken = CS$<>8__locals2.lines[i].Substring(num + 1);
										}
										bool flag7 = UwaWebsiteClient.WebSite == Localization.eWebSite.JP && CS$<>8__locals2.lines[i].Substring(0, num) == "uwa_login_token";
										if (flag7)
										{
											UwaWebsiteClient.LoginToken = CS$<>8__locals2.lines[i].Substring(num + 1);
										}
										Cookie cookie = new Cookie(CS$<>8__locals2.lines[i].Substring(0, num), CS$<>8__locals2.lines[i].Substring(num + 1));
										cookie.Domain = UwaWebsiteClient.UwaDomain;
										cookie.Path = "/";
										UwaWebsiteClient.cookcontanerInstance.Add(cookie);
										UwaWebsiteClient.Log(string.Concat(new string[]
										{
											"ADD Cookie {",
											CS$<>8__locals2.lines[i].Substring(0, num),
											",",
											CS$<>8__locals2.lines[i].Substring(num + 1),
											"}"
										}));
									}
								}
								j = i;
							}
						}
						catch (Exception ex)
						{
							UwaWebsiteClient.ActiveSessionID = null;
							UwaWebsiteClient.sLoginState = UwaWebsiteClient.LoginState.LOGIN_ERROR;
							UwaWebsiteClient.sErrorMessage = "Cookie not parsered.";
							UwaWebsiteClient.LogError(ex.ToString());
						}
					}
				}
				bool flag8 = !resp.ok;
				if (flag8)
				{
					UwaWebsiteClient.sLoginState = UwaWebsiteClient.LoginState.LOGIN_ERROR;
					UwaWebsiteClient.LogError(resp.HttpErrorMessage ?? "Unknown http error");
				}
				else
				{
					try
					{
						JSONValue jsonvalue = JSONParser.SimpleParse(resp.data);
						bool flag9 = !UwaWebsiteClient.CheckError(jsonvalue, out UwaWebsiteClient.iErrorCode, out UwaWebsiteClient.sErrorMessage);
						if (flag9)
						{
							JSONValue jsonvalue2 = jsonvalue["data"];
							UwaWebsiteClient.userProfile = new UwaWebsiteClient.UserProfile();
							UwaWebsiteClient.userProfile.Name = (jsonvalue2.ContainsKey("userName") ? jsonvalue2["userName"] : jsonvalue2["user_name"]);
							UwaWebsiteClient.userProfile.IconUrl = (jsonvalue2.ContainsKey("headUrl") ? jsonvalue2["headUrl"] : jsonvalue2["hd_url"]);
							UwaWebsiteClient.userProfile.UserId = (jsonvalue2.ContainsKey("userId") ? jsonvalue2["userId"] : jsonvalue2["user_id"]);
							UwaWebsiteClient.userProfile.Email = (jsonvalue2.ContainsKey("userEmail") ? jsonvalue2["userEmail"].AsString(false) : null);
							CoreUtils.RefreshOnlineConfig();
							UwaWebsiteClient.sLoginState = UwaWebsiteClient.LoginState.LOGGED_IN;
							bool rememberSession = UwaWebsiteClient.RememberSession;
							if (rememberSession)
							{
								UwaWebsiteClient.SavedSessionID = UwaWebsiteClient.NotSavedSessionId;
							}
						}
						else
						{
							bool flag10 = UwaWebsiteClient.iErrorCode == 20603 || (UwaWebsiteClient.AuthCode != null && UwaWebsiteClient.iErrorCode == 20313);
							if (flag10)
							{
								UwaWebsiteClient.NeedAuthCode = true;
							}
							bool flag11 = UwaWebsiteClient.iErrorCode == 20603;
							if (flag11)
							{
								UwaWebsiteClient.NeedAuthCode = true;
							}
							bool flag12 = jsonvalue.ContainsKey("data");
							if (flag12)
							{
								JSONValue jsonvalue3 = jsonvalue["data"];
								bool flag13 = jsonvalue3.ContainsKey("request") && jsonvalue3["request"];
								if (flag13)
								{
									UwaWebsiteClient.NeedAuthCode = true;
								}
							}
							UwaWebsiteClient.sLoginState = UwaWebsiteClient.LoginState.LOGIN_ERROR;
						}
					}
					catch (Exception ex2)
					{
						UwaWebsiteClient.sLoginState = UwaWebsiteClient.LoginState.LOGIN_ERROR;
						UwaWebsiteClient.LogError(ex2.ToString());
					}
				}
				onDone(UwaWebsiteClient.iErrorCode, UwaWebsiteClient.sErrorMessage);
			};
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x00024ABC File Offset: 0x00022CBC
		private static UwaWebResponse ParseUwaWebResponse(UwaWebsiteClient.Pending p)
		{
			Exception ex = p.ex;
			WebHeaderCollection httpHeaders = (p.conn != null) ? p.conn.ResponseHeaders : null;
			UwaWebResponse uwaWebResponse = default(UwaWebResponse);
			uwaWebResponse.data = p.data;
			uwaWebResponse.binData = p.binData;
			uwaWebResponse.ok = true;
			uwaWebResponse.HttpErrorMessage = null;
			uwaWebResponse.HttpStatusCode = -1;
			bool flag = ex != null;
			if (flag)
			{
				WebException ex2 = null;
				try
				{
					ex2 = (WebException)ex;
				}
				catch (Exception)
				{
				}
				bool flag2 = ex2 == null || ex2.Response == null || ex2.Response.Headers == null;
				if (flag2)
				{
					string str = "Invalid server response ";
					Exception ex3 = ex;
					UwaWebsiteClient.LogError(str + ((ex3 != null) ? ex3.ToString() : null));
					UwaWebsiteClient.LogError("Stacktrace:" + ex.StackTrace);
				}
				else
				{
					uwaWebResponse.HttpHeaders = ex2.Response.Headers;
					uwaWebResponse.HttpStatusCode = (int)((HttpWebResponse)ex2.Response).StatusCode;
					uwaWebResponse.HttpErrorMessage = ex2.Message;
					bool flag3 = uwaWebResponse.HttpStatusCode != 401 && UwaWebsiteClient.Debug;
					if (flag3)
					{
						WebHeaderCollection headers = ex2.Response.Headers;
						StringBuilder stringBuilder = new StringBuilder("\nDisplaying ex the response headers\n");
						for (int i = 0; i < headers.Count; i++)
						{
							stringBuilder.Append("\t" + headers.GetKey(i) + " = " + headers.Get(i));
						}
						stringBuilder.Append("status code: " + uwaWebResponse.HttpStatusCode.ToString());
						UwaWebsiteClient.Log(stringBuilder.ToString());
					}
				}
			}
			else
			{
				uwaWebResponse.HttpStatusCode = 200;
				uwaWebResponse.HttpHeaders = httpHeaders;
			}
			bool flag4 = uwaWebResponse.HttpStatusCode / 100 != 2;
			if (flag4)
			{
				uwaWebResponse.ok = false;
				UwaWebsiteClient.LogError("Request statusCode: " + uwaWebResponse.HttpStatusCode.ToString());
				bool flag5 = ex != null;
				if (flag5)
				{
					uwaWebResponse.HttpErrorMessage = ex.Message;
				}
				else
				{
					uwaWebResponse.HttpErrorMessage = "Request status: " + uwaWebResponse.HttpStatusCode.ToString();
				}
			}
			bool flag6 = ex != null;
			if (flag6)
			{
				uwaWebResponse.ok = false;
				string str2 = "Request exception: ";
				Type type = ex.GetType();
				UwaWebsiteClient.LogError(str2 + ((type != null) ? type.ToString() : null) + " - " + ex.Message);
				uwaWebResponse.HttpErrorMessage = ex.Message;
			}
			return uwaWebResponse;
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x00024DBC File Offset: 0x00022FBC
		public static void Update()
		{
			List<UwaWebsiteClient.Pending> pendings = UwaWebsiteClient.Pendings;
			List<UwaWebsiteClient.Pending> obj = pendings;
			lock (obj)
			{
				UwaWebsiteClient.Pendings.RemoveAll(delegate(UwaWebsiteClient.Pending p)
				{
					bool flag = p.conn == null;
					bool result;
					if (flag)
					{
						result = true;
					}
					else
					{
						bool flag2 = !p.conn.IsBusy;
						if (flag2)
						{
							bool flag3 = p.ex == null && p.data == null && p.binData == null;
							if (flag3)
							{
								result = false;
							}
							else
							{
								try
								{
									UwaWebResponse uwaWebResponse = UwaWebsiteClient.ParseUwaWebResponse(p);
									UwaWebsiteClient.Log("Pending done: " + p.id + " " + (uwaWebResponse.data ?? "<nodata>"));
									bool flag4 = p.callback != null;
									if (flag4)
									{
										p.callback(uwaWebResponse);
									}
								}
								catch (Exception ex)
								{
									UwaWebsiteClient.LogError("Uncaught exception in async net callback: " + ex.Message);
									UwaWebsiteClient.LogError(ex.StackTrace);
								}
								p.conn = null;
								result = true;
							}
						}
						else
						{
							result = false;
						}
					}
					return result;
				});
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060003FF RID: 1023 RVA: 0x00024E28 File Offset: 0x00023028
		private static string UwaDomain
		{
			get
			{
				string result;
				switch (UwaWebsiteClient.WebSite)
				{
				case Localization.eWebSite.CN:
					result = ".uwa4d.com";
					break;
				case Localization.eWebSite.US:
					result = ".uwa4d.com";
					break;
				case Localization.eWebSite.JP:
					result = ".uwa4d.com";
					break;
				default:
					result = null;
					break;
				}
				return result;
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000400 RID: 1024 RVA: 0x00024E88 File Offset: 0x00023088
		private static string UwaWebsiteUrl
		{
			get
			{
				string result;
				switch (UwaWebsiteClient.WebSite)
				{
				case Localization.eWebSite.CN:
					result = (UwaWebsiteClient.Debug ? "https://sandbox-api-proxy.uwa4d.com/i2odUs15zQ03e3oB" : "https://www.uwa4d.com");
					break;
				case Localization.eWebSite.US:
					result = (UwaWebsiteClient.Debug ? "https://testen.uwa4d.com" : "https://en.uwa4d.com");
					break;
				case Localization.eWebSite.JP:
					result = (UwaWebsiteClient.Debug ? "https://testjp.uwa4d.com" : "https://jp.uwa4d.com");
					break;
				default:
					result = null;
					break;
				}
				return result;
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000401 RID: 1025 RVA: 0x00024F24 File Offset: 0x00023124
		private static string DebugUrl
		{
			get
			{
				return "http://192.168.1.248:9000";
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000402 RID: 1026 RVA: 0x00024F44 File Offset: 0x00023144
		private static string UwaGotServerUrl
		{
			get
			{
				string result;
				switch (UwaWebsiteClient.WebSite)
				{
				case Localization.eWebSite.CN:
					result = (UwaWebsiteClient.Debug ? "https://sandbox-api-proxy.uwa4d.com/i2odUs15zQ03e3oB" : "https://secure-api.uwa4d.com");
					break;
				case Localization.eWebSite.US:
					result = (UwaWebsiteClient.Debug ? "https://testen.uwa4d.com" : "https://en.uwa4d.com");
					break;
				case Localization.eWebSite.JP:
					result = (UwaWebsiteClient.Debug ? "https://testjp.uwa4d.com" : "https://jp.uwa4d.com");
					break;
				default:
					result = null;
					break;
				}
				return result;
			}
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x00024FE0 File Offset: 0x000231E0
		private static string GetUwaOssUrl(string bucket)
		{
			bool flag = bucket == "uwa-test";
			string result;
			if (flag)
			{
				result = "http://oss-accelerate.aliyuncs.com/";
			}
			else
			{
				result = "http://oss-cn-beijing.aliyuncs.com/";
			}
			return result;
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000404 RID: 1028 RVA: 0x0002501C File Offset: 0x0002321C
		public static bool IsLoggedIn
		{
			get
			{
				return UwaWebsiteClient.sLoginState == UwaWebsiteClient.LoginState.LOGGED_IN;
			}
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x00025040 File Offset: 0x00023240
		public static bool LoginInProgress()
		{
			return UwaWebsiteClient.sLoginState == UwaWebsiteClient.LoginState.IN_PROGRESS;
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000406 RID: 1030 RVA: 0x00025064 File Offset: 0x00023264
		// (set) Token: 0x06000407 RID: 1031 RVA: 0x000250B0 File Offset: 0x000232B0
		private static string ActiveSessionID
		{
			get
			{
				string text = UwaWebsiteClient.SavedSessionID;
				bool flag = string.IsNullOrEmpty(text);
				if (flag)
				{
					text = UwaWebsiteClient.NotSavedSessionId;
				}
				bool flag2 = text != null;
				string result;
				if (flag2)
				{
					result = text;
				}
				else
				{
					result = string.Empty;
				}
				return result;
			}
			set
			{
				UwaWebsiteClient.NotSavedSessionId = value;
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000408 RID: 1032 RVA: 0x000250BC File Offset: 0x000232BC
		// (set) Token: 0x06000409 RID: 1033 RVA: 0x000250FC File Offset: 0x000232FC
		private static string SavedSessionID
		{
			get
			{
				bool rememberSession = UwaWebsiteClient.RememberSession;
				string result;
				if (rememberSession)
				{
					result = PlayerPrefs.GetString("uwa4d.sessionid", string.Empty);
				}
				else
				{
					result = string.Empty;
				}
				return result;
			}
			set
			{
				PlayerPrefs.SetString("uwa4d.sessionid", value);
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x0600040A RID: 1034 RVA: 0x0002510C File Offset: 0x0002330C
		// (set) Token: 0x0600040B RID: 1035 RVA: 0x00025114 File Offset: 0x00023314
		public static string LoginToken { get; set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x0600040C RID: 1036 RVA: 0x0002511C File Offset: 0x0002331C
		// (set) Token: 0x0600040D RID: 1037 RVA: 0x00025160 File Offset: 0x00023360
		public static string Username
		{
			get
			{
				bool flag = UwaWebsiteClient._username == null;
				if (flag)
				{
					UwaWebsiteClient._username = PlayerPrefs.GetString("uwa4d.username", "");
				}
				return UwaWebsiteClient._username;
			}
			set
			{
				bool flag = UwaWebsiteClient._username == null || UwaWebsiteClient._username != value;
				if (flag)
				{
					UwaWebsiteClient._username = value;
					PlayerPrefs.SetString("uwa4d.username", value);
				}
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600040E RID: 1038 RVA: 0x000251A8 File Offset: 0x000233A8
		// (set) Token: 0x0600040F RID: 1039 RVA: 0x000251E8 File Offset: 0x000233E8
		public static string Password
		{
			get
			{
				bool rememberSession = UwaWebsiteClient.RememberSession;
				if (rememberSession)
				{
					UwaWebsiteClient._password = PlayerPrefs.GetString("uwa4d.password", "");
				}
				return UwaWebsiteClient._password;
			}
			set
			{
				bool flag = UwaWebsiteClient._password != value;
				if (flag)
				{
					UwaWebsiteClient._password = value;
					bool rememberSession = UwaWebsiteClient.RememberSession;
					if (rememberSession)
					{
						PlayerPrefs.SetString("uwa4d.password", value);
					}
				}
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000410 RID: 1040 RVA: 0x0002522C File Offset: 0x0002342C
		// (set) Token: 0x06000411 RID: 1041 RVA: 0x00025288 File Offset: 0x00023488
		public static bool RememberSession
		{
			get
			{
				bool flag = UwaWebsiteClient._remember == null;
				if (flag)
				{
					UwaWebsiteClient._remember = new bool?(PlayerPrefs.GetString("uwa4d.remember", "0") == "1");
				}
				return UwaWebsiteClient._remember.Value;
			}
			set
			{
				bool flag = UwaWebsiteClient._remember == null || UwaWebsiteClient._remember.Value != value;
				if (flag)
				{
					if (value)
					{
						PlayerPrefs.SetString("uwa4d.password", UwaWebsiteClient.Password);
					}
					else
					{
						PlayerPrefs.DeleteKey("uwa4d.password");
					}
					UwaWebsiteClient._remember = new bool?(value);
					PlayerPrefs.SetString("uwa4d.remember", value ? "1" : "0");
				}
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000412 RID: 1042 RVA: 0x00025320 File Offset: 0x00023520
		// (set) Token: 0x06000413 RID: 1043 RVA: 0x0002537C File Offset: 0x0002357C
		public static bool? UploadScreen
		{
			get
			{
				bool flag = !PlayerPrefs.HasKey("uwa4d.uploadscreen");
				bool? result;
				if (flag)
				{
					result = null;
				}
				else
				{
					UwaWebsiteClient._uploadScreen = new bool?(PlayerPrefs.GetInt("uwa4d.uploadscreen", 1) == 1);
					result = UwaWebsiteClient._uploadScreen;
				}
				return result;
			}
			set
			{
				bool? uploadScreen = UwaWebsiteClient._uploadScreen;
				bool? flag = value;
				bool flag2 = !(uploadScreen.GetValueOrDefault() == flag.GetValueOrDefault() & uploadScreen != null == (flag != null)) && value != null;
				if (flag2)
				{
					UwaWebsiteClient._uploadScreen = value;
					PlayerPrefs.SetInt("uwa4d.uploadscreen", value.Value ? 1 : 0);
				}
			}
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x000253F8 File Offset: 0x000235F8
		public static bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			bool result = true;
			bool flag = sslPolicyErrors > SslPolicyErrors.None;
			if (flag)
			{
				for (int i = 0; i < chain.ChainStatus.Length; i++)
				{
					bool flag2 = chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown;
					if (!flag2)
					{
						chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
						chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
						chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
						chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
						bool flag3 = chain.Build((X509Certificate2)certificate);
						bool flag4 = !flag3;
						if (flag4)
						{
							result = false;
							break;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0400027B RID: 635
		[NonSerialized]
		private static CookieContainer cookcontanerInstance = new CookieContainer(100);

		// Token: 0x0400027C RID: 636
		[NonSerialized]
		private static UwaWebsiteClient.LoginState sLoginState = UwaWebsiteClient.LoginState.LOGGED_OUT;

		// Token: 0x0400027D RID: 637
		private static string _logPath = null;

		// Token: 0x0400027E RID: 638
		private static object logObj = new object();

		// Token: 0x0400027F RID: 639
		public static UwaWebsiteClient.UserProfile userProfile = null;

		// Token: 0x04000280 RID: 640
		public static UwaWebsiteClient.UserProfile.GotLicenseInfo licenseInfo = null;

		// Token: 0x04000281 RID: 641
		public static Texture2D LoginAuthImage = null;

		// Token: 0x04000282 RID: 642
		public static readonly UwaWebsiteClient.GotDataSubmitInfo gotDataSubmitInfo = new UwaWebsiteClient.GotDataSubmitInfo();

		// Token: 0x04000283 RID: 643
		[NonSerialized]
		private static bool submitting;

		// Token: 0x04000284 RID: 644
		public static readonly UwaWebsiteClient.PaUploadInfo paUploadInfo = new UwaWebsiteClient.PaUploadInfo();

		// Token: 0x04000285 RID: 645
		public static readonly UwaWebsiteClient.UserProfile.GotLicenseInfo gotLicense = new UwaWebsiteClient.UserProfile.GotLicenseInfo();

		// Token: 0x04000286 RID: 646
		private const string regnums = "^(?:(?:(?:\\+86)?1[34578]\\d(-?)\\d{4}\\1\\d{4})|(?:(?:\\+886)?0?9\\d{8})|(?:(?:\\+852)?[9|6]\\d{7}))$";

		// Token: 0x04000287 RID: 647
		private const string regemail = "[A-Za-z0-9!#$%&'*+\\/=?^_`{|}~-]+(?:\\.[A-Za-z0-9!#$%&'*+\\/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?";

		// Token: 0x04000288 RID: 648
		private const string ctrycode = "^(\\+(?:86|852|886))";

		// Token: 0x04000289 RID: 649
		private static Dictionary<int, string> dataTypeDic = new Dictionary<int, string>
		{
			{
				3,
				"mono"
			},
			{
				4,
				"overview"
			},
			{
				5,
				"resource"
			},
			{
				6,
				"lua"
			},
			{
				8,
				"release"
			},
			{
				9,
				"development"
			},
			{
				10,
				"gpu"
			}
		};

		// Token: 0x0400028A RID: 650
		private static readonly List<UwaWebsiteClient.Pending> Pendings = new List<UwaWebsiteClient.Pending>();

		// Token: 0x0400028B RID: 651
		private static byte[] defaultUserIcon = new byte[]
		{
			137,
			80,
			78,
			71,
			13,
			10,
			26,
			10,
			0,
			0,
			0,
			13,
			73,
			72,
			68,
			82,
			0,
			0,
			0,
			40,
			0,
			0,
			0,
			40,
			8,
			6,
			0,
			0,
			0,
			140,
			254,
			184,
			109,
			0,
			0,
			0,
			1,
			115,
			82,
			71,
			66,
			0,
			174,
			206,
			28,
			233,
			0,
			0,
			6,
			123,
			73,
			68,
			65,
			84,
			88,
			9,
			205,
			89,
			75,
			143,
			20,
			85,
			20,
			62,
			245,
			236,
			119,
			79,
			79,
			207,
			131,
			25,
			200,
			136,
			175,
			128,
			209,
			5,
			36,
			182,
			17,
			197,
			205,
			184,
			96,
			54,
			78,
			136,
			184,
			112,
			49,
			97,
			161,
			137,
			186,
			144,
			144,
			248,
			11,
			104,
			127,
			130,
			19,
			88,
			40,
			137,
			108,
			96,
			227,
			2,
			67,
			102,
			53,
			46,
			36,
			49,
			10,
			33,
			78,
			34,
			62,
			80,
			153,
			168,
			16,
			102,
			96,
			222,
			61,
			175,
			126,
			85,
			215,
			203,
			243,
			85,
			79,
			53,
			69,
			85,
			245,
			80,
			51,
			2,
			206,
			73,
			58,
			117,
			171,
			234,
			220,
			115,
			190,
			58,
			247,
			222,
			115,
			207,
			119,
			91,
			160,
			109,
			72,
			177,
			104,
			139,
			7,
			143,
			252,
			118,
			72,
			20,
			148,
			65,
			178,
			173,
			2,
			9,
			194,
			62,
			54,
			211,
			207,
			191,
			12,
			217,
			100,
			146,
			64,
			235,
			220,
			158,
			39,
			219,
			158,
			20,
			68,
			225,
			154,
			105,
			91,
			151,
			175,
			143,
			191,
			248,
			99,
			177,
			40,
			88,
			91,
			117,
			39,
			108,
			165,
			195,
			216,
			149,
			155,
			123,
			108,
			209,
			58,
			65,
			182,
			48,
			146,
			140,
			201,
			3,
			153,
			100,
			156,
			146,
			113,
			149,
			226,
			170,
			76,
			178,
			36,
			145,
			36,
			10,
			140,
			143,
			200,
			178,
			44,
			50,
			76,
			139,
			234,
			13,
			131,
			42,
			53,
			141,
			214,
			249,
			87,
			211,
			140,
			41,
			18,
			236,
			11,
			130,
			37,
			158,
			30,
			126,
			125,
			byte.MaxValue,
			221,
			168,
			126,
			35,
			1,
			188,
			248,
			195,
			207,
			189,
			146,
			168,
			20,
			21,
			73,
			122,
			191,
			43,
			155,
			138,
			117,
			102,
			147,
			148,
			80,
			149,
			168,
			62,
			28,
			189,
			90,
			67,
			167,
			229,
			181,
			42,
			45,
			173,
			85,
			52,
			221,
			52,
			191,
			52,
			45,
			189,
			120,
			236,
			240,
			129,
			249,
			135,
			25,
			121,
			40,
			192,
			75,
			223,
			223,
			24,
			145,
			100,
			105,
			180,
			59,
			151,
			206,
			239,
			234,
			76,
			59,
			145,
			10,
			51,
			42,
			75,
			98,
			51,
			130,
			182,
			77,
			134,
			101,
			115,
			20,
			17,
			203,
			160,
			24,
			166,
			73,
			115,
			203,
			101,
			90,
			92,
			41,
			151,
			76,
			195,
			60,
			121,
			244,
			141,
			151,
			46,
			4,
			181,
			238,
			63,
			105,
			11,
			240,
			243,
			137,
			9,
			165,
			223,
			72,
			157,
			142,
			201,
			242,
			135,
			207,
			244,
			119,
			81,
			34,
			22,
			140,
			152,
			40,
			8,
			148,
			73,
			40,
			148,
			228,
			119,
			24,
			94,
			175,
			52,
			12,
			147,
			202,
			117,
			157,
			170,
			154,
			225,
			125,
			220,
			106,
			215,
			52,
			157,
			110,
			205,
			44,
			81,
			93,
			55,
			206,
			206,
			42,
			149,
			143,
			63,
			42,
			20,
			244,
			214,
			75,
			79,
			227,
			65,
			171,
			27,
			47,
			190,
			186,
			50,
			149,
			136,
			137,
			229,
			175,
			59,
			83,
			137,
			161,
			189,
			187,
			242,
			36,
			250,
			156,
			67,
			77,
			149,
			37,
			234,
			202,
			196,
			3,
			192,
			60,
			182,
			157,
			166,
			198,
			64,
			151,
			214,
			234,
			100,
			113,
			100,
			253,
			130,
			40,
			223,
			153,
			43,
			81,
			169,
			82,
			27,
			23,
			23,
			42,
			199,
			134,
			135,
			11,
			85,
			191,
			78,
			0,
			32,
			34,
			215,
			103,
			164,
			198,
			186,
			51,
			169,
			161,
			129,
			222,
			28,
			47,
			208,
			128,
			10,
			41,
			178,
			72,
			189,
			217,
			68,
			232,
			59,
			191,
			3,
			220,
			235,
			188,
			96,
			22,
			86,
			107,
			161,
			32,
			109,
			6,
			62,
			53,
			191,
			66,
			139,
			235,
			149,
			241,
			89,
			185,
			50,
			236,
			143,
			164,
			232,
			55,
			216,
			167,
			167,
			206,
			32,
			114,
			237,
			192,
			1,
			110,
			87,
			58,
			30,
			25,
			28,
			236,
			43,
			60,
			63,
			59,
			211,
			49,
			191,
			43,
			231,
			30,
			1,
			128,
			175,
			60,
			251,
			132,
			111,
			191,
			210,
			3,
			0,
			47,
			93,
			189,
			49,
			18,
			87,
			228,
			15,
			48,
			172,
			97,
			145,
			67,
			231,
			100,
			92,
			225,
			133,
			242,
			64,
			55,
			191,
			205,
			208,
			251,
			4,
			167,
			34,
			149,
			35,
			31,
			38,
			240,
			245,
			20,
			251,
			132,
			111,
			96,
			240,
			234,
			180,
			122,
			56,
			169,
			68,
			144,
			70,
			177,
			32,
			194,
			230,
			156,
			219,
			41,
			21,
			147,
			221,
			230,
			150,
			175,
			88,
			76,
			237,
			4,
			62,
			225,
			91,
			98,
			12,
			192,
			226,
			234,
			181,
			0,
			34,
			207,
			33,
			149,
			132,
			173,
			86,
			87,
			25,
			87,
			12,
			215,
			118,
			37,
			166,
			72,
			155,
			118,
			133,
			111,
			96,
			80,
			24,
			139,
			171,
			232,
			120,
			195,
			14,
			129,
			36,
			140,
			60,
			183,
			153,
			32,
			149,
			180,
			27,
			250,
			205,
			250,
			185,
			239,
			252,
			169,
			200,
			125,
			238,
			189,
			2,
			131,
			196,
			88,
			128,
			9,
			207,
			29,
			128,
			156,
			86,
			79,
			96,
			135,
			192,
			118,
			245,
			56,
			37,
			152,
			15,
			130,
			222,
			128,
			1,
			88,
			120,
			91,
			60,
			137,
			183,
			34,
			54,
			126,
			18,
			133,
			17,
			108,
			95,
			59,
			69,
			242,
			140,
			133,
			243,
			230,
			113,
			96,
			115,
			170,
			146,
			164,
			42,
			15,
			108,
			117,
			111,
			125,
			156,
			31,
			19,
			231,
			125,
			158,
			179,
			69,
			byte.MaxValue,
			203,
			71,
			38,
			11,
			188,
			120,
			148,
			65,
			84,
			37,
			59,
			77,
			50,
			9,
			206,
			155,
			34,
			189,
			41,
			162,
			158,
			67,
			201,
			180,
			211,
			36,
			197,
			0,
			109,
			178,
			95,
			197,
			252,
			219,
			135,
			122,
			110,
			167,
			137,
			131,
			201,
			182,
			246,
			139,
			100,
			9,
			187,
			57,
			197,
			236,
			52,
			124,
			156,
			111,
			25,
			147,
			45,
			246,
			139,
			188,
			156,
			211,
			155,
			237,
			28,
			byte.MaxValue,
			23,
			114,
			7,
			19,
			176,
			49,
			0,
			51,
			88,
			8,
			61,
			30,
			88,
			219,
			241,
			3,
			128,
			235,
			224,
			16,
			81,
			196,
			228,
			210,
			40,
			164,
			172,
			139,
			210,
			213,
			209,
			105,
			87,
			101,
			135,
			25,
			112,
			116,
			109,
			161,
			204,
			67,
			76,
			243,
			32,
			56,
			145,
			132,
			67,
			192,
			124,
			34,
			146,
			106,
			152,
			18,
			138,
			215,
			168,
			130,
			138,
			156,
			39,
			225,
			52,
			71,
			80,
			152,
			4,
			251,
			138,
			42,
			204,
			206,
			162,
			170,
			6,
			244,
			218,
			149,
			byte.MaxValue,
			1,
			69,
			126,
			160,
			233,
			236,
			71,
			160,
			155,
			216,
			254,
			175,
			129,
			26,
			70,
			149,
			50,
			115,
			9,
			51,
			226,
			148,
			240,
			218,
			100,
			238,
			193,
			78,
			163,
			71,
			16,
			152,
			4,
			65,
			156,
			16,
			77,
			219,
			184,
			12,
			222,
			218,
			78,
			80,
			94,
			129,
			123,
			116,
			36,
			155,
			21,
			49,
			230,
			96,
			169,
			172,
			241,
			92,
			140,
			62,
			229,
			241,
			65,
			203,
			220,
			7,
			2,
			162,
			213,
			219,
			145,
			160,
			108,
			66,
			101,
			0,
			237,
			188,
			242,
			194,
			96,
			76,
			192,
			38,
			130,
			241,
			87,
			53,
			125,
			186,
			206,
			188,
			213,
			47,
			29,
			41,
			149,
			141,
			129,
			3,
			203,
			14,
			123,
			139,
			111,
			212,
			115,
			136,
			196,
			82,
			57,
			156,
			8,
			249,
			109,
			232,
			60,
			151,
			230,
			153,
			143,
			152,
			27,
			52,
			52,
			199,
			133,
			10,
			8,
			87,
			54,
			169,
			82,
			95,
			46,
			73,
			97,
			53,
			34,
			166,
			92,
			181,
			174,
			207,
			0,
			27,
			87,
			51,
			130,
			197,
			95,
			114,
			190,
			196,
			164,
			218,
			21,
			247,
			43,
			51,
			188,
			5,
			122,
			191,
			50,
			207,
			145,
			116,
			11,
			214,
			122,
			131,
			249,
			237,
			74,
			149,
			152,
			134,
			135,
			70,
			211,
			224,
			168,
			173,
			86,
			53,
			154,
			243,
			128,
			67,
			212,
			248,
			68,
			194,
			117,
			195,
			140,
			80,
			164,
			110,
			38,
			95,
			120,
			238,
			149,
			210,
			90,
			133,
			111,
			237,
			243,
			192,
			230,
			104,
			235,
			181,
			198,
			153,
			37,
			177,
			242,
			73,
			47,
			51,
			27,
			85,
			150,
			169,
			135,
			135,
			192,
			5,
			226,
			237,
			8,
			224,
			61,
			108,
			16,
			209,
			67,
			20,
			17,
			21,
			12,
			221,
			10,
			105,
			204,
			244,
			154,
			71,
			31,
			108,
			152,
			87,
			58,
			147,
			119,
			95,
			102,
			232,
			224,
			136,
			101,
			124,
			64,
			96,
			27,
			163,
			140,
			104,
			34,
			16,
			171,
			213,
			6,
			247,
			3,
			77,
			173,
			104,
			124,
			12,
			240,
			25,
			222,
			59,
			5,
			235,
			59,
			131,
			7,
			166,
			77,
			62,
			142,
			152,
			103,
			198,
			223,
			149,
			189,
			31,
			37,
			40,
			248,
			5,
			25,
			190,
			59,
			147,
			160,
			28,
			15,
			191,
			187,
			3,
			97,
			54,
			34,
			45,
			212,
			120,
			104,
			106,
			28,
			89,
			47,
			56,
			12,
			39,
			230,
			92,
			24,
			56,
			175,
			109,
			188,
			79,
			49,
			33,
			195,
			169,
			3,
			167,
			178,
			115,
			238,
			249,
			77,
			43,
			222,
			186,
			165,
			23,
			23,
			87,
			203,
			239,
			106,
			13,
			35,
			207,
			165,
			181,
			183,
			111,
			160,
			141,
			175,
			77,
			243,
			240,
			167,
			152,
			67,
			212,
			56,
			146,
			220,
			199,
			225,
			190,
			136,
			40,
			222,
			97,
			232,
			192,
			224,
			154,
			76,
			110,
			115,
			91,
			94,
			227,
			18,
			71,
			223,
			57,
			18,
			177,
			244,
			83,
			222,
			231,
			173,
			54,
			40,
			223,
			183,
			63,
			253,
			109,
			27,
			6,
			239,
			25,
			79,
			88,
			224,
			19,
			190,
			253,
			180,
			179,
			5,
			206,
			109,
			140,
			93,
			253,
			227,
			139,
			137,
			201,
			187,
			54,
			111,
			127,
			79,
			12,
			34,
			124,
			193,
			39,
			124,
			187,
			56,
			220,
			171,
			51,
			7,
			221,
			27,
			92,
			239,
			241,
			65,
			206,
			221,
			210,
			218,
			248,
			47,
			183,
			230,
			66,
			87,
			167,
			87,
			247,
			81,
			180,
			57,
			10,
			4,
			95,
			240,
			9,
			223,
			126,
			155,
			88,
			68,
			1,
			25,
			155,
			152,
			72,
			90,
			70,
			234,
			226,
			158,
			124,
			118,
			232,
			224,
			179,
			125,
			76,
			3,
			3,
			223,
			17,
			232,
			179,
			157,
			7,
			38,
			175,
			244,
			235,
			byte.MaxValue,
			204,
			210,
			189,
			197,
			213,
			111,
			4,
			53,
			247,
			246,
			112,
			97,
			247,
			253,
			92,
			183,
			97,
			48,
			212,
			243,
			112,
			161,
			80,
			197,
			65,
			206,
			204,
			210,
			218,
			217,
			239,
			126,
			189,
			77,
			235,
			156,
			207,
			30,
			181,
			192,
			38,
			108,
			195,
			199,
			140,
			90,
			125,
			43,
			12,
			28,
			124,
			134,
			70,
			208,
			11,
			230,
			210,
			213,
			63,
			71,
			120,
			65,
			142,
			62,
			221,
			215,
			153,
			127,
			174,
			63,
			207,
			153,
			191,
			181,
			240,
			189,
			106,
			145,
			219,
			13,
			222,
			147,
			byte.MaxValue,
			154,
			41,
			209,
			237,
			217,
			229,
			146,
			97,
			209,
			201,
			163,
			175,
			189,
			176,
			189,
			3,
			76,
			175,
			199,
			230,
			185,
			141,
			242,
			169,
			170,
			202,
			239,
			237,
			237,
			201,
			197,
			6,
			122,
			58,
			40,
			29,
			146,
			116,
			189,
			125,
			252,
			237,
			114,
			173,
			65,
			83,
			11,
			171,
			116,
			103,
			97,
			69,
			227,
			74,
			229,
			156,
			105,
			169,
			167,
			142,
			29,
			126,
			254,
			191,
			31,
			1,
			123,
			29,
			57,
			199,
			17,
			204,
			248,
			65,
			170,
			115,
			233,
			120,
			127,
			23,
			19,
			236,
			46,
			78,
			218,
			169,
			214,
			65,
			122,
			115,
			198,
			32,
			81,
			35,
			105,
			87,
			24,
			84,
			169,
			92,
			227,
			157,
			161,
			74,
			43,
			229,
			250,
			12,
			182,
			47,
			67,
			211,
			71,
			177,
			49,
			120,
			237,
			110,
			214,
			126,
			232,
			16,
			135,
			117,
			118,
			254,
			134,
			24,
			250,
			253,
			21,
			73,
			144,
			7,
			45,
			203,
			60,
			196,
			201,
			121,
			31,
			8,
			14,
			3,
			200,
			52,
			245,
			5,
			254,
			27,
			194,
			158,
			70,
			61,
			135,
			146,
			9,
			85,
			201,
			118,
			byte.MaxValue,
			134,
			248,
			23,
			56,
			55,
			150,
			189,
			59,
			197,
			168,
			226,
			0,
			0,
			0,
			0,
			73,
			69,
			78,
			68,
			174,
			66,
			96,
			130
		};

		// Token: 0x0400028C RID: 652
		private static MethodInfo _loadImageMethod = null;

		// Token: 0x0400028D RID: 653
		private static string[] cookieHeaders = new string[]
		{
			"uwa_login_token_aws",
			"uwa_user_id_aws",
			"uwa_login_token",
			"uwa_user_id",
			"uwa_remember_me",
			"usid",
			"JSESSIONID"
		};

		// Token: 0x0400028E RID: 654
		public static bool DebugLog = false;

		// Token: 0x0400028F RID: 655
		public static bool Debug = false;

		// Token: 0x04000290 RID: 656
		public static Localization.eWebSite WebSite = Localization.eWebSite.CN;

		// Token: 0x04000291 RID: 657
		public static string GAUrl = "https://www.google-analytics.com/mp/collect";

		// Token: 0x04000292 RID: 658
		private static string sErrorMessage = null;

		// Token: 0x04000293 RID: 659
		private static int iErrorCode = -1;

		// Token: 0x04000294 RID: 660
		private static string NotSavedSessionId;

		// Token: 0x04000296 RID: 662
		[NonSerialized]
		private static string _username = null;

		// Token: 0x04000297 RID: 663
		[NonSerialized]
		private static string _password = "";

		// Token: 0x04000298 RID: 664
		public static string AuthCode;

		// Token: 0x04000299 RID: 665
		public static bool NeedAuthCode = false;

		// Token: 0x0400029A RID: 666
		[NonSerialized]
		private static bool? _remember;

		// Token: 0x0400029B RID: 667
		[NonSerialized]
		private static bool? _uploadScreen = null;

		// Token: 0x02000117 RID: 279
		internal class Pending
		{
			// Token: 0x040006EF RID: 1775
			public UwaWebsiteWebClient conn;

			// Token: 0x040006F0 RID: 1776
			public Exception ex;

			// Token: 0x040006F1 RID: 1777
			public string data;

			// Token: 0x040006F2 RID: 1778
			public byte[] binData;

			// Token: 0x040006F3 RID: 1779
			public string id;

			// Token: 0x040006F4 RID: 1780
			public UwaWebsiteClient.OnResponseCallback callback;
		}

		// Token: 0x02000118 RID: 280
		// (Invoke) Token: 0x06000A1D RID: 2589
		public delegate void OnDoneCallback4(int pid, int balance, string errorMessage);

		// Token: 0x02000119 RID: 281
		// (Invoke) Token: 0x06000A21 RID: 2593
		public delegate void OnDoneCallback3(int mode, int projectId, int errorCode, string errorMessage);

		// Token: 0x0200011A RID: 282
		// (Invoke) Token: 0x06000A25 RID: 2597
		public delegate void OnDoneCallback6(int recordId, int projectId, UwaWebsiteClient.ProjectType recordType, int errorCode, string errorMessage);

		// Token: 0x0200011B RID: 283
		// (Invoke) Token: 0x06000A29 RID: 2601
		public delegate void OnDoneCallback2(int mode, int errorCode, string errorMessage);

		// Token: 0x0200011C RID: 284
		// (Invoke) Token: 0x06000A2D RID: 2605
		public delegate void OnDoneCallback(int errorCode, string errorMessage);

		// Token: 0x0200011D RID: 285
		// (Invoke) Token: 0x06000A31 RID: 2609
		public delegate void OnDoneCallback5(bool available, int errorCode, string errorMessage);

		// Token: 0x0200011E RID: 286
		// (Invoke) Token: 0x06000A35 RID: 2613
		public delegate void OnServiceDataInfoCallback(int errorCode, string errorMessage, UwaWebsiteClient.ServiceDataInfo dataInfo);

		// Token: 0x0200011F RID: 287
		// (Invoke) Token: 0x06000A39 RID: 2617
		internal delegate void OnResponseCallback(UwaWebResponse job);

		// Token: 0x02000120 RID: 288
		// (Invoke) Token: 0x06000A3D RID: 2621
		internal delegate void OnJsonGotCallback(JSONValue json);

		// Token: 0x02000121 RID: 289
		private enum LoginState
		{
			// Token: 0x040006F6 RID: 1782
			LOGGED_OUT,
			// Token: 0x040006F7 RID: 1783
			IN_PROGRESS,
			// Token: 0x040006F8 RID: 1784
			LOGGED_IN,
			// Token: 0x040006F9 RID: 1785
			LOGIN_ERROR
		}

		// Token: 0x02000122 RID: 290
		public class ServiceDataInfo
		{
			// Token: 0x170001D7 RID: 471
			// (get) Token: 0x06000A40 RID: 2624 RVA: 0x000487CC File Offset: 0x000469CC
			public UwaWebsiteClient.GroupProjectInfo selectProject
			{
				get
				{
					return UwaWebsiteClient.userProfile.FindById(this.selectedProjectId);
				}
			}

			// Token: 0x040006FA RID: 1786
			public int engine = 0;

			// Token: 0x040006FB RID: 1787
			public string engineVersion = "";

			// Token: 0x040006FC RID: 1788
			public int dataType = 0;

			// Token: 0x040006FD RID: 1789
			public string dataHash = "";

			// Token: 0x040006FE RID: 1790
			public int selectedProjectId = -1;

			// Token: 0x040006FF RID: 1791
			public List<string> zipPath = new List<string>();

			// Token: 0x04000700 RID: 1792
			public int duration = 0;

			// Token: 0x04000701 RID: 1793
			public string deviceModel = "";

			// Token: 0x04000702 RID: 1794
			public int platform = 0;

			// Token: 0x04000703 RID: 1795
			public string userNote = "";

			// Token: 0x04000704 RID: 1796
			public string license = "";

			// Token: 0x04000705 RID: 1797
			public string meta = "";
		}

		// Token: 0x02000123 RID: 291
		public class BalanceInfo
		{
			// Token: 0x170001D8 RID: 472
			// (get) Token: 0x06000A42 RID: 2626 RVA: 0x00048880 File Offset: 0x00046A80
			public int BalanceFinal
			{
				get
				{
					bool flag = this.metaServiceType == "GOT_ONLINE_FREE_USAGE" || this.metaServiceType == "LT_TEST_FREE_USAGE";
					int result;
					if (flag)
					{
						result = int.MaxValue;
					}
					else
					{
						bool flag2 = this.metaServiceType == "GOT_ONLINE_AMOUNT";
						if (flag2)
						{
							result = this.serviceBalance;
						}
						else
						{
							result = 0;
						}
					}
					return result;
				}
			}

			// Token: 0x04000706 RID: 1798
			public int serviceBalance;

			// Token: 0x04000707 RID: 1799
			public int serviceBalanceTotal;

			// Token: 0x04000708 RID: 1800
			public string balanceUM;

			// Token: 0x04000709 RID: 1801
			public bool isDurationLimit;

			// Token: 0x0400070A RID: 1802
			public string startDate;

			// Token: 0x0400070B RID: 1803
			public string endDate;

			// Token: 0x0400070C RID: 1804
			public int serviceTypeCode;

			// Token: 0x0400070D RID: 1805
			public int serviceDetailId;

			// Token: 0x0400070E RID: 1806
			public string serviceMetaType;

			// Token: 0x0400070F RID: 1807
			public string metaServiceType;

			// Token: 0x04000710 RID: 1808
			public int serviceLevel;

			// Token: 0x04000711 RID: 1809
			public string content;
		}

		// Token: 0x02000124 RID: 292
		public enum BalancePriorityType
		{
			// Token: 0x04000713 RID: 1811
			LatestEndDate,
			// Token: 0x04000714 RID: 1812
			MaxLeftCount,
			// Token: 0x04000715 RID: 1813
			Unknown
		}

		// Token: 0x02000125 RID: 293
		public class BalancePriority
		{
			// Token: 0x06000A44 RID: 2628 RVA: 0x00048908 File Offset: 0x00046B08
			public BalancePriority(UwaWebsiteClient.BalancePriorityType type, float priority)
			{
				this.Type = type;
				this.Priority = priority;
				this.CompareTo = null;
				switch (type)
				{
				case UwaWebsiteClient.BalancePriorityType.LatestEndDate:
					this.CompareTo = delegate(UwaWebsiteClient.BalanceInfo x, UwaWebsiteClient.BalanceInfo y)
					{
						bool flag = !x.isDurationLimit && !y.isDurationLimit;
						int result;
						if (flag)
						{
							result = 0;
						}
						else
						{
							bool flag2 = !x.isDurationLimit && y.isDurationLimit;
							if (flag2)
							{
								result = 1;
							}
							else
							{
								bool flag3 = x.isDurationLimit && !y.isDurationLimit;
								if (flag3)
								{
									result = -1;
								}
								else
								{
									DateTime dateTime = DateTime.Parse(x.endDate);
									DateTime value = DateTime.Parse(y.endDate);
									result = dateTime.CompareTo(value);
								}
							}
						}
						return result;
					};
					break;
				case UwaWebsiteClient.BalancePriorityType.MaxLeftCount:
					this.CompareTo = ((UwaWebsiteClient.BalanceInfo x, UwaWebsiteClient.BalanceInfo y) => y.serviceBalance.CompareTo(x.serviceBalance));
					break;
				}
			}

			// Token: 0x04000716 RID: 1814
			public UwaWebsiteClient.BalancePriorityType Type;

			// Token: 0x04000717 RID: 1815
			public float Priority;

			// Token: 0x04000718 RID: 1816
			public Func<UwaWebsiteClient.BalanceInfo, UwaWebsiteClient.BalanceInfo, int> CompareTo;
		}

		// Token: 0x02000126 RID: 294
		public class GroupProjectInfo
		{
			// Token: 0x06000A45 RID: 2629 RVA: 0x000489B8 File Offset: 0x00046BB8
			public bool UpdateDefaultBalance()
			{
				this.SelectedBalance = null;
				bool flag = this.Balances == null;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					for (int i = 0; i < this.Balances.Count; i++)
					{
						bool flag2 = this.Balances[i].metaServiceType == "GOT_ONLINE_FREE_USAGE" || this.Balances[i].metaServiceType == "LOCAL_TEST_FREE_USAGE";
						if (flag2)
						{
							this.SelectedBalance = this.Balances[i];
							return true;
						}
					}
					for (int j = 0; j < this.Balances.Count; j++)
					{
						bool flag3 = this.Balances[j].metaServiceType == "GOT_ONLINE_AMOUNT";
						if (flag3)
						{
							bool flag4 = this.SelectedBalance == null || this.SelectedBalance.serviceBalance < this.Balances[j].serviceBalance;
							if (flag4)
							{
								this.SelectedBalance = this.Balances[j];
							}
						}
					}
					result = true;
				}
				return result;
			}

			// Token: 0x06000A46 RID: 2630 RVA: 0x00048B10 File Offset: 0x00046D10
			public bool UpdateBalance(List<UwaWebsiteClient.BalancePriority> balancePriorities)
			{
				this.SelectedBalance = null;
				bool flag = this.Balances == null;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					balancePriorities.Sort((UwaWebsiteClient.BalancePriority x, UwaWebsiteClient.BalancePriority y) => x.Priority.CompareTo(y.Priority));
					this.Balances.Sort(delegate(UwaWebsiteClient.BalanceInfo x, UwaWebsiteClient.BalanceInfo y)
					{
						for (int i = 0; i < balancePriorities.Count; i++)
						{
							int num = balancePriorities[i].CompareTo(x, y);
							bool flag2 = num != 0;
							if (flag2)
							{
								return num;
							}
						}
						return 0;
					});
					this.SelectedBalance = this.Balances[0];
					result = true;
				}
				return result;
			}

			// Token: 0x04000719 RID: 1817
			public int engine;

			// Token: 0x0400071A RID: 1818
			public int projectGroupId;

			// Token: 0x0400071B RID: 1819
			public string projectGroupName;

			// Token: 0x0400071C RID: 1820
			public string packageName;

			// Token: 0x0400071D RID: 1821
			public int userRole;

			// Token: 0x0400071E RID: 1822
			public List<UwaWebsiteClient.BalanceInfo> Balances;

			// Token: 0x0400071F RID: 1823
			public UwaWebsiteClient.BalanceInfo SelectedBalance;
		}

		// Token: 0x02000127 RID: 295
		public class UserProfile
		{
			// Token: 0x06000A48 RID: 2632 RVA: 0x00048BBC File Offset: 0x00046DBC
			public UwaWebsiteClient.GroupProjectInfo FindById(int id)
			{
				bool flag = this.temp_got_project_array == null;
				UwaWebsiteClient.GroupProjectInfo result;
				if (flag)
				{
					result = null;
				}
				else
				{
					bool flag2 = this.temp_got_project_array.ContainsKey(id);
					if (flag2)
					{
						result = this.temp_got_project_array[id];
					}
					else
					{
						result = null;
					}
				}
				return result;
			}

			// Token: 0x06000A49 RID: 2633 RVA: 0x00048C14 File Offset: 0x00046E14
			public UwaWebsiteClient.GroupProjectInfo FindByName(string name)
			{
				foreach (KeyValuePair<int, UwaWebsiteClient.GroupProjectInfo> keyValuePair in this.temp_got_project_array)
				{
					bool flag = keyValuePair.Value.projectGroupName == name;
					if (flag)
					{
						return keyValuePair.Value;
					}
				}
				return null;
			}

			// Token: 0x06000A4A RID: 2634 RVA: 0x00048C9C File Offset: 0x00046E9C
			public void Clear()
			{
				bool flag = this.IconImage != null;
				if (flag)
				{
					Object.DestroyImmediate(this.IconImage);
					this.IconImage = null;
				}
			}

			// Token: 0x04000720 RID: 1824
			public string Name;

			// Token: 0x04000721 RID: 1825
			public int UserId;

			// Token: 0x04000722 RID: 1826
			public string Email;

			// Token: 0x04000723 RID: 1827
			public string IconUrl;

			// Token: 0x04000724 RID: 1828
			public Texture2D IconImage;

			// Token: 0x04000725 RID: 1829
			public Dictionary<int, UwaWebsiteClient.GroupProjectInfo> temp_got_project_array;

			// Token: 0x0200016B RID: 363
			public class GotLicenseInfo
			{
				// Token: 0x040007DD RID: 2013
				public int status;

				// Token: 0x040007DE RID: 2014
				public string licenseFile;

				// Token: 0x040007DF RID: 2015
				public string info;
			}
		}

		// Token: 0x02000128 RID: 296
		public class GotDataSubmitInfo
		{
			// Token: 0x06000A4C RID: 2636 RVA: 0x00048CE4 File Offset: 0x00046EE4
			public void Clear()
			{
				this.state = UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Idle;
				this.selected = null;
				this.fileUrl = null;
				this.projectName = null;
				this.fileETag = null;
				this.skip = false;
				this.lastPercent = 0f;
			}

			// Token: 0x04000726 RID: 1830
			public static string uploadSource = "website";

			// Token: 0x04000727 RID: 1831
			public UwaWebsiteClient.ServiceDataInfo selected;

			// Token: 0x04000728 RID: 1832
			public string fileUrl;

			// Token: 0x04000729 RID: 1833
			public string projectName;

			// Token: 0x0400072A RID: 1834
			public string fileETag;

			// Token: 0x0400072B RID: 1835
			public bool skip;

			// Token: 0x0400072C RID: 1836
			public float lastPercent;

			// Token: 0x0400072D RID: 1837
			public int lastProjectId;

			// Token: 0x0400072E RID: 1838
			public int lastRecordId;

			// Token: 0x0400072F RID: 1839
			public UwaWebsiteClient.ProjectType lastRecordType;

			// Token: 0x04000730 RID: 1840
			public UwaWebsiteClient.GotDataSubmitInfo.SubmitState state = UwaWebsiteClient.GotDataSubmitInfo.SubmitState.Idle;

			// Token: 0x0200016C RID: 364
			public enum SubmitState
			{
				// Token: 0x040007E1 RID: 2017
				Idle,
				// Token: 0x040007E2 RID: 2018
				GetBalance,
				// Token: 0x040007E3 RID: 2019
				Request,
				// Token: 0x040007E4 RID: 2020
				Uploading,
				// Token: 0x040007E5 RID: 2021
				Uploaded,
				// Token: 0x040007E6 RID: 2022
				Submitting,
				// Token: 0x040007E7 RID: 2023
				Submit
			}
		}

		// Token: 0x02000129 RID: 297
		public class PaUploadInfo
		{
			// Token: 0x06000A4F RID: 2639 RVA: 0x00048D38 File Offset: 0x00046F38
			public void Clear()
			{
				this.selected = null;
				this.fileUrl = null;
				this.scriptUrl = null;
				this.fileETag = null;
				this.skip = false;
				this.description = null;
				this.authCode = null;
				this.uploading = false;
				this.userNote = "";
				this.percent = 0f;
				this.deviceMaps.Clear();
				this.normalSelected = 0;
				this.monoSelected = 0;
				bool flag = this.authCodeImage != null;
				if (flag)
				{
					Object.DestroyImmediate(this.authCodeImage);
					this.authCodeImage = null;
				}
				this.ClearProgress();
			}

			// Token: 0x06000A50 RID: 2640 RVA: 0x00048DE4 File Offset: 0x00046FE4
			public void ClearProgress()
			{
				this.lastPercent = -1f;
				this.lastTime = DateTime.MinValue;
				this.estimateTime = null;
				this.estimateSpeed = null;
			}

			// Token: 0x04000731 RID: 1841
			public static string uploadSource = "pipeline";

			// Token: 0x04000732 RID: 1842
			public UwaWebsiteClient.GroupProjectInfo selected;

			// Token: 0x04000733 RID: 1843
			public Texture2D authCodeImage;

			// Token: 0x04000734 RID: 1844
			public int recordId;

			// Token: 0x04000735 RID: 1845
			public string fileUrl;

			// Token: 0x04000736 RID: 1846
			public string scriptUrl;

			// Token: 0x04000737 RID: 1847
			public string description;

			// Token: 0x04000738 RID: 1848
			public string authCode;

			// Token: 0x04000739 RID: 1849
			public bool uploading;

			// Token: 0x0400073A RID: 1850
			public float percent;

			// Token: 0x0400073B RID: 1851
			public string fileETag;

			// Token: 0x0400073C RID: 1852
			public string userNote;

			// Token: 0x0400073D RID: 1853
			public bool skip;

			// Token: 0x0400073E RID: 1854
			public int serviceLevel;

			// Token: 0x0400073F RID: 1855
			public float lastPercent;

			// Token: 0x04000740 RID: 1856
			public DateTime lastTime;

			// Token: 0x04000741 RID: 1857
			public string estimateTime;

			// Token: 0x04000742 RID: 1858
			public string estimateSpeed;

			// Token: 0x04000743 RID: 1859
			public readonly Dictionary<int, List<UwaWebsiteClient.PaUploadInfo.DeviceInfo>> deviceMaps = new Dictionary<int, List<UwaWebsiteClient.PaUploadInfo.DeviceInfo>>();

			// Token: 0x04000744 RID: 1860
			public int normalSelected = 0;

			// Token: 0x04000745 RID: 1861
			public int monoSelected = 0;

			// Token: 0x0200016D RID: 365
			public class DeviceInfo
			{
				// Token: 0x040007E8 RID: 2024
				public int Id;

				// Token: 0x040007E9 RID: 2025
				public string Name;

				// Token: 0x040007EA RID: 2026
				public bool Selected;
			}
		}

		// Token: 0x0200012A RID: 298
		internal enum ProjectType
		{
			// Token: 0x04000747 RID: 1863
			PA,
			// Token: 0x04000748 RID: 1864
			RC,
			// Token: 0x04000749 RID: 1865
			MONO = 3,
			// Token: 0x0400074A RID: 1866
			PERF,
			// Token: 0x0400074B RID: 1867
			ASSET,
			// Token: 0x0400074C RID: 1868
			LUA,
			// Token: 0x0400074D RID: 1869
			PIPELINE,
			// Token: 0x0400074E RID: 1870
			RT = 101,
			// Token: 0x0400074F RID: 1871
			Unknown = 1000
		}
	}
}
