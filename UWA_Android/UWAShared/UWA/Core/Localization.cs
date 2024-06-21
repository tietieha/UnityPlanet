using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace UWA.Core
{
	// Token: 0x0200005C RID: 92
	[ComVisible(false)]
	public class Localization
	{
		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000416 RID: 1046 RVA: 0x0002564C File Offset: 0x0002384C
		// (set) Token: 0x06000417 RID: 1047 RVA: 0x00025654 File Offset: 0x00023854
		public Localization.eWebSite WebSite { get; private set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000418 RID: 1048 RVA: 0x00025660 File Offset: 0x00023860
		// (set) Token: 0x06000419 RID: 1049 RVA: 0x00025668 File Offset: 0x00023868
		public Localization.eLocale Locale { get; private set; }

		// Token: 0x0600041A RID: 1050 RVA: 0x00025674 File Offset: 0x00023874
		public static string StreamToString(Stream stream)
		{
			stream.Position = 0L;
			string result;
			using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
			{
				result = streamReader.ReadToEnd();
			}
			return result;
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x000256C4 File Offset: 0x000238C4
		private string LoadLocale(string locale)
		{
			string result;
			if (!(locale == "zh_CN"))
			{
				if (!(locale == "ja_JP"))
				{
					if (!(locale == "en_US"))
					{
						result = null;
					}
					else
					{
						result = "\r\n{\r\n  \"Review reports\": \"Review reports\",\r\n  \"Login to sync\": \"Login to sync data\",\r\n  \"Sync all data\": \"Sync all data\",\r\n  \"Task in list\": \" task(s) in waiting list\",\r\n  \"New version available\": \"UWA GOT v{0} is Released.\",\r\n  \"Username\": \"Account\",\r\n  \"Password\": \"Password\",\r\n  \"AuthCode\": \"Auth Code\",\r\n  \"Forgot?\": \"Forget password?\",\r\n  \"Remember me\": \"Remember me\",\r\n  \"Sign up\": \"Sign up\",\r\n  \"Log in\": \"Login\",\r\n  \"Logout\": \"Logout\",\r\n  \"View\": \"View\",\r\n  \"Offline Activation User Guide\": \"Offline Activation User Guide:\",\r\n  \"UWA GOT User Guide\": \"UWA_GOT_User_Guide(EN)\",\r\n  \"Device ID\": \"Device ID:\",\r\n  \"Offline Tips\": \"For offline access device.\",\r\n  \"Device\": \"Current device:\",\r\n  \"Generate License Request\": \"Generate License Request file\",\r\n  \"License file\": \"License File:\",\r\n  \"Import\": \"Import\",\r\n  \"Active License Tips\": \"Valid License File, Expiration Date:\",\r\n  \"Invalid License Tips\": \"License File is invalid.\",\r\n  \"Null License Tips\": \"Please import valid License File.\",\r\n  \"Waining License Tips\": \"Abnormal behavior was detected. Please contact us. Exception code:\",\r\n  \"Start Offline\": \"Start using UWA GOT\",\r\n  \"Active\": \"Be Active\",\r\n  \"Valid Time\": \"Expiration Date\",\r\n  \"Try for free\": \"Trial\",\r\n  \"Renewal\": \"Renewal\",\r\n  \"Unbinding\": \"Unbinding\",\r\n  \"Unbindable\": \"Unbind is not supported for offline activation.\",\r\n  \"Trial\": \"On Trial\",\r\n  \"Trial Intro\": \"New account has 15-day of trial.\",\r\n  \"Trial Date1\": \" \",\r\n  \"Trial Date2\": \"days left\",\r\n  \"Activate\": \"Active\",\r\n  \"Inactivated\": \"Inactive\",\r\n  \"Serial Code\": \"Serial Code\",\r\n  \"Active Tips\": \"Success! Please login again if the state is not refreshed.\",\r\n  \"Activate Submit\": \"Submit\",\r\n  \"Trial Submit\": \"Submit\",\r\n  \"Back\": \"Back\",\r\n  \"Buy\": \"No Serial Code? Click here.\",\r\n  \"Try Tips\": \"Success! Please login again if the state is not refreshed.\",\r\n  \"Already synced\": \"Already synced\",\r\n  \"Sync to GOT OL\": \"Sync to UWA GOT Online\",\r\n  \"Note\": \"Note\",\r\n  \"OK\": \"Confirm\",\r\n  \"Cancel\": \"Cancel\",\r\n  \"Check net\": \"Please check the network.\",\r\n  \"Failed to login\": \"Failed to login, Please try again.\",\r\n  \"Failed to send:\": \"Failed to send:\",\r\n  \"Failed to send, try again\": \"Failed to send, please check the network and try again\",\r\n  \"Local Server not connected\": \"Local Server is not connected.\",\r\n  \"Internal not connected\": \"Internet is not connected.\",\r\n  \"Data not found\": \"Data is not found.\",\r\n  \"Pkg not match\": \"Package name of the data does not match with the project.\",\r\n  \"Project not select\": \"Please select a project\\n for data uploaded to online. \",\r\n  \"Lua for online\": \"Lua is only for UWA GOT Online.\",\r\n  \"GPU for online\": \"GPU is only for UWA GOT Online.\",\r\n  \"GPM for online\": \"GPM is only for UWA GPM (Online).\",\r\n  \"User Balance Not Enough\": \"Your balance is not enough.\\nPlease go to Account settings->Purchase->UWA GOT Online.\",\r\n  \"Project Balance Not Enough\": \"Your balance is not enough.\\nPlease go to Account settings->Purchase->UWA GOT Online.\",\r\n  \"Cannot Get Balance\": \"Failed to get balance information, please wait a moment.\",\r\n  \"Balance Reduce Tip\": \"This data will be charged {0:F2} minutes.\",\r\n  \"Project Balance Tip\": \"Balance: {0} min {1} s.\",\r\n  \"New Project Balance Tip\": \"Create project, balance: {0} min {1} s.\",\r\n  \"Time Left Tip\": \"Balance:{0}m{1}s\",\r\n  \"License Rsp 31101\": \"Failed, this user has already applied for trial or activation.\",\r\n  \"License Rsp 31102\": \"Failed, this device has already applied for trial or activation.\",\r\n  \"License Rsp 31103\": \"Failed, this user has already applied for trial or activation.\",\r\n  \"License Rsp 31201\": \"Failed, this serial code is invalid.\",\r\n  \"License Rsp 31202\": \"Failed, License has expired.\",\r\n  \"License Rsp 31203\": \"Failed, this license is activated already.\",\r\n  \"License Rsp 31204\": \"Failed, this device has already applied for trial or activation.\",\r\n  \"License NULL\": \"Please input the serial code.\",\r\n  \"License Rsp -1\": \"Please check the network.\",\r\n  \"License Rsp 0\": \"Success! Please re-login if the state is not updated.\",\r\n  \"License Rsp 20001\": \"Wrong parameters\",\r\n  \"License Rsp 30001\": \"Out of service\",\r\n  \"Login Rsp 10001\": \"Please try again after login\",\r\n  \"Login Rsp 20001\": \"Wrong parameters\",\r\n  \"Login Rsp 20313\": \"Account Name/Password is invalid\",\r\n  \"Login Rsp 20601\": \"Auth code is wrong\",\r\n  \"Login Rsp 20603\": \"Please enter auth code\",\r\n  \"Login Rsp 20606\": \"Account is not activated\",\r\n  \"Login Rsp 30001\": \"Out of service\",\r\n  \"Uploading Tip 1\": \"Welcome to UWA Blog, let's explore and share knowledge!\",\r\n  \"Uploading Tip 2\": \"Upgrade to VIP, for a deeper and more comprehensive performance analysis.\",\r\n  \"Uploading Tip 3\": \"Speed. Precision. Consideration.\",\r\n  \"Uploading Tip 4\": \" \",\r\n  \"Upload to Online\": \"Upload to UWA website.\",\r\n  \"Upload to Local Server\": \"Upload to Local Server.\",\r\n  \"Select Project\": \"Select Project\",\r\n  \"Refresh Project List\": \"Refresh Project List\",\r\n  \"Data Submit\": \"Data Submit\",\r\n  \"Completed\": \"Completed\",\r\n  \"Uploading...\": \"Uploading...\",\r\n  \"Upload Screen\": \"Upload Screenshot to GOT Online.\",\r\n  \"Unuploaded\": \"Fail to upload\",\r\n  \"StatNotFound\": \"UEStats file not found\",\r\n  \"Unsubmit\": \"Fail to submit\",\r\n  \"Resubmit\": \"Data resubmit\",\r\n  \"BalanceNoEnough\": \"Balance is not enough\",\r\n  \"DataBroken\": \"Data is broken\",\r\n  \"InvalidProjectId\": \"Project ID is invalid\",\r\n  \"UserNotActive\": \"Account is not Activated.\"\r\n}\r\n";
					}
				}
				else
				{
					result = "\r\n{\r\n  \"Review reports\": \"レポートを確認する\",\r\n  \"Login to sync\": \"ログインしてデータを同期する\",\r\n  \"Sync all data\": \"全データを同期する\",\r\n  \"Task in list\": \"処理待ちタスクのリスト\",\r\n  \"New version available\": \"UWA GOT V{0} がリリースしました。\",\r\n  \"Username\": \"アカウント\",\r\n  \"Password\": \"パスワード\",\r\n  \"AuthCode\": \"認証コード\",\r\n  \"Forgot?\": \"パスワードを忘れた場合\",\r\n  \"Remember me\": \"ログイン情報を保持\",\r\n  \"Sign up\": \"登録\",\r\n  \"Log in\": \"ログイン\",\r\n  \"Logout\": \"ログアウト\",\r\n  \"View\": \"確認\",\r\n  \"Offline Activation User Guide\": \"オフライン認証のユーザーガイド:\",\r\n  \"UWA GOT User Guide\": \"UWA GOT User Guide (JP)\",\r\n  \"Device ID\": \"デバイス ID:\",\r\n  \"Offline Tips\": \"オフラインアクセス用のデバイス。\",\r\n  \"Device\": \"現在のデバイス:\",\r\n  \"Generate License Request\": \"License Requestファイルを生成する\",\r\n  \"License file\": \"Licenseファイル:\",\r\n  \"Import\": \"インポート\",\r\n  \"Active License Tips\": \"有効Licenseファイル、有効期間：\",\r\n  \"Invalid License Tips\": \"Licenseファイルは無効です。\",\r\n  \"Null License Tips\": \"有効なLicenseファイルをインポートしてください。\",\r\n  \"Waining License Tips\": \"異常なエラーを検出しました。サポート窓口へお問い合わせください。異常コード：\",\r\n  \"Start Offline\": \"UWA GOTを使用開始\",\r\n  \"Active\": \"認証済み\",\r\n  \"Valid Time\": \"有効期間\",\r\n  \"Try for free\": \"トライアル\",\r\n  \"Renewal\": \"更新\",\r\n  \"Unbinding\": \"デバイスとの紐付きを解除する\",\r\n  \"Unbindable\": \"オフライン認証は紐付き解除できません。\",\r\n  \"Trial\": \"試用中\",\r\n  \"Trial Intro\": \"新しいアカウントは15日の試用を申請できる。\",\r\n  \"Trial Date1\": \" \",\r\n  \"Trial Date2\": \"日の試用期間\",\r\n  \"Activate\": \"認証\",\r\n  \"Inactivated\": \"未認証\",\r\n  \"Serial Code\": \"ライセンスキー\",\r\n  \"Active Tips\": \"認証完了！アカウントステータスが更新されないばあいは再度ログインしてください。\",\r\n  \"Activate Submit\": \"申請\",\r\n  \"Trial Submit\": \"申請\",\r\n  \"Back\": \"戻る\",\r\n  \"Buy\": \"Licenseを購入？クリック\",\r\n  \"Try Tips\": \"試用開始！アカウントステータスが更新されない場合は再度ログインしてください。\",\r\n  \"Already synced\": \"同期済み\",\r\n  \"Sync to GOT OL\": \"UWA GOT Onlineへ同期します\",\r\n  \"Note\": \"Note\",\r\n  \"OK\": \"確認\",\r\n  \"Cancel\": \"キャンセル\",\r\n  \"Check net\": \"ネットワークを確認してください。\",\r\n  \"Failed to login\": \"ログイン失敗、もう一度お試しください。\",\r\n  \"Failed to send:\": \"送信失敗：\",\r\n  \"Failed to send, try again\": \"送信失敗、ネットワークを確認して、再度お試しください。\",\r\n  \"Local Server not connected\": \"ローカルサーバーは接続されていません。\",\r\n  \"Pkg not match\": \"Package name of the data\\n does not match with the project.\",\r\n  \"Project not select\": \"Please select a project\\n for data uploaded to online. \",\r\n  \"Internal not connected\": \"インターネットは接続されていません。\",\r\n  \"Data not found\": \"データを見つかりません。\",\r\n  \"Lua for online\": \"LuaはUWA GOT Onlineのみ向けです。\",\r\n  \"GPU for online\": \"GPUはUWA GOT Onlineのみ向けです。\",\r\n  \"GPM for online\": \"GPM is only for UWA GPM (Online).\",\r\n  \"User Balance Not Enough\": \"残り時間が不足しています、ご購入後、再度提出してください。\\n ホームページ->価格->UWA GOT Onlineへ移動してください\",\r\n  \"Project Balance Not Enough\": \"プロジェクトの残存時間が不足しています、管理者へご連絡ください。ご購入後、再度提出してください。\\n ホームページ->価格->UWA GOT Onlineへ移動してください\",\r\n  \"Cannot Get Balance\": \"残り時間情報を取得できませんでした、あとでもう一度お試しください。\",\r\n  \"Balance Reduce Tip\": \"本データは残り時間から{0:F2}分を差し引きます。\",\r\n  \"Project Balance Tip\": \"本プロジェクトの残り時間：{0}分{1}秒\",\r\n  \"New Project Balance Tip\": \"プロジェクトを作成します、アカウント残り時間：{0}分{1}秒\",\r\n  \"Time Left Tip\": \"残り時間：{0}分{1}秒\",\r\n  \"License Rsp 31101\": \"申請エラー、本アカウントは試用または認証をすでに申請済みです。\",\r\n  \"License Rsp 31102\": \"申請エラー、本デバイスは試用または認証をすでに申請済みです。\",\r\n  \"License Rsp 31103\": \"申請エラー、本アカウントは試用または認証をすでに申請済みです。\",\r\n  \"License Rsp 31201\": \"認証エラー、有効なライセンスキーをご入力ください。\",\r\n  \"License Rsp 31202\": \"認証エラー、Licenseは有効期間を過ぎました。\",\r\n  \"License Rsp 31203\": \"認証エラー、ライセンスは紐付き済み。\",\r\n  \"License Rsp 31204\": \"認証エラー、ライセンスは紐付き済み。\",\r\n  \"License NULL\": \"有効なライセンスキーをご入力ください。\",\r\n  \"License Rsp -1\": \"ネットワークを確認してください。\",\r\n  \"License Rsp 0\": \"完了しました! もしアカウントのステータスが未更新の場合は、もう一度ログインしてください。\",\r\n  \"License Rsp 20001\": \"リクエストエラー\",\r\n  \"License Rsp 30001\": \"服务暂时不可用\",\r\n  \"Login Rsp 10001\": \"もう一度ログインして、再度操作してください\",\r\n  \"Login Rsp 20001\": \"リクエストエラー\",\r\n  \"Login Rsp 20313\": \"無効なアカウントまたはパスワード\",\r\n  \"Login Rsp 20601\": \"画像認証コードエラー\",\r\n  \"Login Rsp 20603\": \"認証コードをご入力ください。\",\r\n  \"Login Rsp 20606\": \"アカウントの認証は完了しておりません\",\r\n  \"Login Rsp 30001\": \"服务暂时不可用\",\r\n  \"Uploading Tip 1\": \"Speed. Precision. Consideration.\",\r\n  \"Uploading Tip 2\": \"UWAの代理店へ問合せする：sales@nexas-tec.co.jp\",\r\n  \"Uploading Tip 3\": \"データの可視化により、さらに具体的な最適化提案およびパラメーター変化情報で開発チームは高度なプロジェクト品質管理ができる。\",\r\n  \"Uploading Tip 4\": \" \",\r\n  \"Upload to Online\": \"テストデータを UWA ウェブサイトにアップロードします。\",\r\n  \"Upload to Local Server\": \"テストデータをローカルサーバーにアップロードし、GOT Editor のパネルでデータを確認します。\",\r\n  \"Select Project\": \"Select Project\",\r\n  \"Refresh Project List\": \"Refresh Project List\",\r\n  \"Data Submit\": \"データ提出\",\r\n  \"Completed\": \"完成\",\r\n  \"Uploading...\": \"アップロード中、、、、\",\r\n  \"Upload Screen\": \"スクリーンショットをGOT Onlineにアップロードします。\",\r\n  \"Unuploaded\": \"Fail to upload\",\r\n  \"StatNotFound\": \"UEStats file not found\",\r\n  \"Unsubmit\": \"Fail to submit\",\r\n  \"Resubmit\": \"Data resubmit\",\r\n  \"BalanceNoEnough\": \"Balance is not enough\",\r\n  \"DataBroken\": \"Data is broken\",\r\n  \"InvalidProjectId\": \"Project ID is invalid\",\r\n  \"UserNotActive\": \"Account is not Activated.\"\r\n}\r\n";
				}
			}
			else
			{
				result = "\r\n{\r\n  \"Review reports\": \"查看报告\",\r\n  \"Login to sync\": \"登录进行同步\",\r\n  \"Sync all data\": \"同步所有数据\",\r\n  \"Task in list\": \" 个任务在列表中...\",\r\n  \"New version available\": \"UWA GOT v{0} 已发布。\",\r\n  \"Username\": \"账号\",\r\n  \"Password\": \"密码\",\r\n  \"AuthCode\": \"验证码\",\r\n  \"Forgot?\": \"忘记密码？\",\r\n  \"Remember me\": \"记住我\",\r\n  \"Sign up\": \"注册\",\r\n  \"Log in\": \"登录\",\r\n  \"Logout\": \"注销\",\r\n  \"View\": \"查看\",\r\n  \"Offline Activation User Guide\": \"离线激活说明文档:\",\r\n  \"UWA GOT User Guide\": \"UWA GOT 使用说明\",\r\n  \"Device ID\": \"设备ID:\",\r\n  \"Offline Tips\": \"适用于无法连接Internet的内网机使用\",\r\n  \"Device\": \"当前设备\",\r\n  \"Generate License Request\": \"创建 License Request 文件\",\r\n  \"License file\": \"License 文件\",\r\n  \"Import\": \"导入\",\r\n  \"Active License Tips\": \"License File有效，有效期至：\",\r\n  \"Invalid License Tips\": \"导入文件不是有效的 License 文件。\",\r\n  \"Null License Tips\": \"请导入有效的LicenseFile文件。\",\r\n  \"Waining License Tips\": \"检测到使用行为异常，请联系工作人员。异常码：\",\r\n  \"Start Offline\": \"开始使用 UWA GOT\",\r\n  \"Active\": \"已激活\",\r\n  \"Valid Time\": \"有效期\",\r\n  \"Try for free\": \"试用\",\r\n  \"Renewal\": \"续费\",\r\n  \"Unbinding\": \"解绑\",\r\n  \"Unbindable\": \"离线激活 License 不可解绑\",\r\n  \"Trial\": \"试用中\",\r\n  \"Trial Intro\": \"首次使用 GOT 的用户可申请 15 天的试用期。\",\r\n  \"Trial Date1\": \"还有\",\r\n  \"Trial Date2\": \"天试用期\",\r\n  \"Activate\": \"激活\",\r\n  \"Inactivated\": \"未激活\",\r\n  \"Serial Code\": \"序列号\",\r\n  \"Active Tips\": \"激活成功！如账号状态未刷新请重新登陆。\",\r\n  \"Activate Submit\": \"申请激活\",\r\n  \"Trial Submit\": \"申请试用\",\r\n  \"Back\": \"返回\",\r\n  \"Buy\": \"没有License序列号？前往购买\",\r\n  \"Try Tips\": \"试用成功！账号状态未刷新请重新登陆\",\r\n  \"Already synced\": \"已经同步\",\r\n  \"Sync to GOT OL\": \"同步到 GOT Online\",\r\n  \"Note\": \"备注\",\r\n  \"OK\": \"确定\",\r\n  \"Cancel\": \"取消\",\r\n  \"Check net\": \"请检查网络连接。\",\r\n  \"Failed to send:\": \"发送失败：\",\r\n  \"Failed to send, try again\": \"发送失败，请检查网络后重试。\",\r\n  \"Local Server not connected\": \"没有连接到本地服务器。请确认：\\n1、是否和GOT Editor在同一个局域网内；\\n2、是否有开启防火墙、VPN和杀毒软件。\",\r\n  \"Internal not connected\": \"没有连接到互联网。\",\r\n  \"Data not found\": \"未找到数据。\",\r\n  \"Pkg not match\": \"测试数据中的包名与项目包名不一致。\\n请先在网页端创建对应的项目。\",\r\n  \"Project not select\": \"请为上传 Online 的数据指定项目。\",\r\n  \"Lua for online\": \"Lua 模式只支持 GOT Online 上传。\",\r\n  \"GPU for online\": \"GPU 模式只支持 GOT Online 上传。\",\r\n  \"GPM for online\": \"GPM 数据只支持 UWA GPM 服务（Online）。\",\r\n  \"User Balance Not Enough\": \"您的剩余时间不足，请购买后提交。\\n请前往 官网\\\"价格\\\"页面 了解并购买。\",\r\n  \"Project Balance Not Enough\": \"项目的剩余时间不足，请提醒拥有者购买后提交。\\n请前往 官网\\\"价格\\\"页面 了解并购买。\",\r\n  \"Cannot Get Balance\": \"无法获取剩余时间，请稍后重试。\",\r\n  \"Balance Reduce Tip\": \"本次提交将扣除购买者约{0:F2}分钟的剩余时间。\",\r\n  \"Project Balance Tip\": \"该项目剩余时间：{0}分{1}秒\",\r\n  \"New Project Balance Tip\": \"创建项目，账号剩余时间：{0}分{1}秒\",\r\n  \"Time Left Tip\": \"剩余时间：{0}分{1}秒\",\r\n  \"License Rsp 31101\": \"申请失败，该账号已申请过试用或激活。\",\r\n  \"License Rsp 31102\": \"申请失败，该设备已申请过试用或激活。\",\r\n  \"License Rsp 31103\": \"申请失败，该账号已申请过试用或激活。\",\r\n  \"License Rsp 31201\": \"激活失败，请填写有效的License序列号。\",\r\n  \"License Rsp 31202\": \"激活失败，License已过期。\",\r\n  \"License Rsp 31203\": \"激活失败，License已绑定。\",\r\n  \"License Rsp 31204\": \"激活失败，License已绑定。\",\r\n  \"License NULL\": \"请输入License序列号。\",\r\n  \"License Rsp -1\": \"请检查网络连接。\",\r\n  \"License Rsp 0\": \"操作成功！如账号状态未刷新请重新登录。\",\r\n  \"License Rsp 20001\": \"请求参数错误\",\r\n  \"License Rsp 30001\": \"服务暂时不可用\",\r\n  \"Login Rsp 10001\": \"请登陆后重新操作\",\r\n  \"Login Rsp 20001\": \"请求参数错误\",\r\n  \"Login Rsp 20313\": \"无效的用户账号或密码\",\r\n  \"Login Rsp 20601\": \"图片验证码错误\",\r\n  \"Login Rsp 20603\": \"请输入验证码\",\r\n  \"Login Rsp 20606\": \"账户尚未激活\",\r\n  \"Login Rsp 30001\": \"服务暂时不可用\",\r\n  \"Uploading Tip 1\": \"还在为 Lua 的性能困扰吗？来用用 GOT Online 吧！\",\r\n  \"Uploading Tip 2\": \"每次学堂上新均可享优惠价，\\n盘它就对了！\",\r\n  \"Uploading Tip 3\": \"GPM 是监控性能的好帮手，当你发现问题时，可结合 GOT Online 或真人真机深入探查。\",\r\n  \"Uploading Tip 4\": \"SDK 最新版本为 2.2.2，大家记得随时更新，不要错过实用的新功能喔！\",\r\n  \"Uploading Tip 5\": \"本地资源检测已经支持 Luacheck 功能，小伙伴们快耍起来！\",\r\n  \"Uploading Tip 6\": \" \",\r\n  \"Upload to Online\": \"将测试数据上传至UWA网站。\",\r\n  \"Upload to Local Server\": \"将测试数据上传至本地服务器，在 UWA GOT 面板中查看数据。\",\r\n  \"Select Project\": \"选择项目\",\r\n  \"Refresh Project List\": \"刷新项目列表\",\r\n  \"Data Submit\": \"提交数据\",\r\n  \"Completed\": \"完成\",\r\n  \"Uploading...\": \"上传中...\",\r\n  \"Upload Screen\": \"上传屏幕截图至 GOT Online。\",\r\n  \"Unuploaded\": \"上传失败\",\r\n  \"StatNotFound\": \"UEStats文件查找失败\",\r\n  \"Unsubmit\": \"提交失败\",\r\n  \"Resubmit\": \"数据重复上传\",\r\n  \"BalanceNoEnough\": \"余额不足\",\r\n  \"DataBroken\": \"数据损坏\",\r\n  \"InvalidProjectId\": \"项目ID无效\",\r\n  \"UserNotActive\": \"您的UWA账号未完成认证，请先登录网站进行认证操作。\"\r\n}";
			}
			return result;
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x00025740 File Offset: 0x00023940
		private Localization()
		{
			this.Locale = Localization.eLocale.zh_CN;
			I18n instance = I18n.Instance;
			I18n.Configure("Locales/", this.Locale.ToString(), true, new string[]
			{
				"zh_CN",
				"en_US",
				"ja_JP"
			}, new I18n.GetLocaleFile(this.LoadLocale));
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x000257B4 File Offset: 0x000239B4
		public void SetWebSite(Localization.eWebSite site)
		{
			this.WebSite = site;
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x000257C0 File Offset: 0x000239C0
		public void SetLocale(Localization.eLocale locale)
		{
			this.Locale = locale;
			I18n.SetLocale(locale.ToString());
			bool flag = this.OnLocalize != null;
			if (flag)
			{
				this.OnLocalize();
			}
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x0002580C File Offset: 0x00023A0C
		public string Get(string key)
		{
			return I18n.Instance.__(key, new object[0]);
		}

		// Token: 0x0400029F RID: 671
		public Localization.Action OnLocalize;

		// Token: 0x040002A0 RID: 672
		public static readonly Localization Instance = new Localization();

		// Token: 0x0200013D RID: 317
		// (Invoke) Token: 0x06000A7B RID: 2683
		public delegate void Action();

		// Token: 0x0200013E RID: 318
		public enum eWebSite
		{
			// Token: 0x0400076A RID: 1898
			CN,
			// Token: 0x0400076B RID: 1899
			US,
			// Token: 0x0400076C RID: 1900
			JP
		}

		// Token: 0x0200013F RID: 319
		public enum eLocale
		{
			// Token: 0x0400076E RID: 1902
			zh_CN,
			// Token: 0x0400076F RID: 1903
			en_US,
			// Token: 0x04000770 RID: 1904
			ja_JP
		}
	}
}
