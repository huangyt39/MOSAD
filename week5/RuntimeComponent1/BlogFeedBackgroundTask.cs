using App4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.Web.Syndication;

namespace BackgroundTasks
{
    public sealed class BlogFeedBackgroundTask : IBackgroundTask
    {
//        //首先，我们处理一下获取IT追梦园的RSS订阅，返回XML文档的方法。前两个是设置一下网络请求头的信息。（可以忽略）
//        static string customHeaderName = "User-Agent";
//        static string customHeaderValue = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; 
//         WOW64; Trident/6.0)"; 
////这里是IT追梦园的RSS地址：
//         static string feedUrl = @"http://www.zmy123.cn/?feed=rss2";
        //这里定义一个textElementName,用来在后面显示该节点请求到的数据
        static string textElementName = "text";


        //注意：这里是后台任务的开始，等我们写完代码，在这里打断点调试，看后台任务是否可以进行到这里！
        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            //var feed = await GetBlogFeed();

            static var feed = MainPage.ViewModel;

            UpdateTile(feed);

            deferral.Complete();
        }

        private static async Task<SyndicationFeed> GetBlogFeed()
        {
            SyndicationFeed feed = null;

            try
            {
                //这里都是请求最XML地址的方法，并获取到XML文档。
                SyndicationClient client = new SyndicationClient();
                client.BypassCacheOnRetrieve = true;
                client.SetRequestHeader(customHeaderName, customHeaderValue);
                //这里我们获取到了XML文档 feed
                feed = await client.RetrieveFeedAsync(new Uri(feedUrl));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return feed;
        }

        //更新磁贴的方法
        private static void UpdateTile(SyndicationFeed feed)
        {
            //通过这个方法，我们就可以为动态磁贴的添加做基础。
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();

            //这里设置的是所以磁贴都可以为动态
            updater.EnableNotificationQueue(true);
            updater.Clear();
            int itemCount = 0;

            //然后这里是重点：记得分3步走：
            foreach (var item in feed.Items)
            {
                //1：创建xml对象，这里看你想显示几种动态磁贴，如果想显示正方形和长方形的，那就分别设置一个动态磁贴类型即可。
                //下面这两个分别是矩形的动态磁贴，和方形的动态磁贴，具体样式，自己可以去微软官网查一查。我这里用到的是换行的文字形式。
                XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideText03);
                XmlDocument tilexml2 = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquareText04);
                var title = item.Title;
                string titleText = title.Text == null ? String.Empty : title.Text;
                //2.接着给这个xml对象赋值
                tileXml.GetElementsByTagName(textElementName)[0].InnerText = titleText;

                //3.然后用Update方法来更新这个磁贴
                updater.Update(new TileNotification(tileXml));

                //4.最后这里需要注意的是微软规定动态磁贴的队列数目小于5个，所以这里做出判断。
                if (itemCount++ > 5) break;
            }
        }

    }
}