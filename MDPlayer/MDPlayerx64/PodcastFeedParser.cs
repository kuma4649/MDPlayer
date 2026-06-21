using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MDPlayerx64
{
    /// <summary>
    /// Podcast RSS feed を解析するクラス
    /// </summary>
    public class PodcastFeedParser
    {
        /// <summary>
        /// Podcast エピソード情報
        /// </summary>
        public class PodcastEpisode
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string AudioUrl { get; set; }
            public DateTime? PubDate { get; set; }
            public string Duration { get; set; }
            public long? AudioLength { get; set; }
            public string AudioType { get; set; }
        }

        /// <summary>
        /// Podcast feed 情報
        /// </summary>
        public class PodcastFeed
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Author { get; set; }
            public string ImageUrl { get; set; }
            public List<PodcastEpisode> Episodes { get; set; } = new List<PodcastEpisode>();
        }

        /// <summary>
        /// RSS feed URL から Podcast 情報を取得・解析
        /// </summary>
        /// <param name="feedUrl">Podcast RSS feed URL</param>
        /// <returns>Podcast feed 情報</returns>
        public static async Task<PodcastFeed> ParseFeedAsync(string feedUrl)
        {
            return await System.Threading.Tasks.Task.Run(() => ParseFeedSync(feedUrl));
        }

        /// <summary>
        /// 同期版フィード取得（スレッドプール上で実行）
        /// </summary>
        public static PodcastFeed ParseFeedSync(string feedUrl)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[Podcast Parser] Starting (sync): {feedUrl}");

                using (var handler = new HttpClientHandler())
                {
                    System.Diagnostics.Debug.WriteLine($"[Podcast Parser] HttpClientHandler created");

                    handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => {
                        System.Diagnostics.Debug.WriteLine($"[Podcast Parser] SSL validation: {errors}");
                        return true;
                    };

                    handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
                    handler.AllowAutoRedirect = true;
                    handler.MaxAutomaticRedirections = 10;
                    handler.UseCookies = true;
                    handler.CookieContainer = new System.Net.CookieContainer();

                    System.Diagnostics.Debug.WriteLine($"[Podcast Parser] HttpClient creating...");

                    using (var client = new HttpClient(handler))
                    {
                        client.Timeout = TimeSpan.FromSeconds(30);

                        System.Diagnostics.Debug.WriteLine($"[Podcast Parser] Setting headers...");

                        // EDGE/Chrome と同じ User-Agent
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0");
                        client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                        client.DefaultRequestHeaders.Add("Accept-Language", "ja-JP,ja;q=0.9");
                        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                        client.DefaultRequestHeaders.Add("DNT", "1");
                        client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                        client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                        client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
                        client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
                        client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
                        client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
                        client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");

                        System.Diagnostics.Debug.WriteLine($"[Podcast Parser] GET request starting: {feedUrl}");

                        // HttpCompletionOption.ResponseHeadersRead でヘッダー取得後すぐに進行
                        var response = client.GetAsync(feedUrl, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();

                        System.Diagnostics.Debug.WriteLine($"[Podcast Parser] Response received: {response.StatusCode}");
                        System.Diagnostics.Debug.WriteLine($"[Podcast Parser] Content-Type: {response.Content.Headers.ContentType}");
                        System.Diagnostics.Debug.WriteLine($"[Podcast Parser] Content-Length: {response.Content.Headers.ContentLength}");

                        if (!response.IsSuccessStatusCode)
                        {
                            System.Diagnostics.Debug.WriteLine($"[Podcast Parser] HTTP Error: {response.StatusCode} {response.ReasonPhrase}");
                            return null;
                        }

                        System.Diagnostics.Debug.WriteLine($"[Podcast Parser] Reading content...");
                        if (response.Content.Headers.ContentType.MediaType != "application/rss+xml")
                        {
                            return null;
                        }
                        string xmlContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        System.Diagnostics.Debug.WriteLine($"[Podcast Parser] Content read: {xmlContent.Length} bytes");

                        System.Diagnostics.Debug.WriteLine($"[Podcast Parser] Parsing XML...");
                        var feed = ParseFeedXml(xmlContent);
                        System.Diagnostics.Debug.WriteLine($"[Podcast Parser] Success: {feed.Episodes.Count} episodes");
                        return feed;
                    }
                }
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine($"[Podcast Parser] TIMEOUT: Request exceeded 30 seconds");
                return null;
            }
            catch (HttpRequestException hex)
            {
                System.Diagnostics.Debug.WriteLine($"[Podcast Parser] HTTP Error: {hex.Message}");
                if (hex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"[Podcast Parser] Inner Exception: {hex.InnerException.GetType().Name} - {hex.InnerException.Message}");
                System.Diagnostics.Debug.WriteLine($"[Podcast Parser] Stack: {hex.StackTrace}");
                return null;
            }
            catch (System.Xml.XmlException xex)
            {
                System.Diagnostics.Debug.WriteLine($"[Podcast Parser] XML Parse Error: {xex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Podcast Parser] Error: {ex.GetType().Name} - {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[Podcast Parser] Stack: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// XML 文字列から Podcast feed を解析
        /// </summary>
        private static PodcastFeed ParseFeedXml(string xmlContent)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(xmlContent))
                {
                    throw new Exception("Empty XML content");
                }

                System.Diagnostics.Debug.WriteLine($"[Podcast Parser] XML Content Length: {xmlContent.Length}");
                var xdoc = XDocument.Parse(xmlContent);
                var feed = new PodcastFeed();

                // RSS または Atom フォーマット判定
                var root = xdoc.Root;
                if (root.Name.LocalName == "rss")
                {
                    System.Diagnostics.Debug.WriteLine($"[Podcast Parser] Detected RSS format");
                    ParseRssFeed(xdoc, feed);
                }
                else if (root.Name.LocalName == "feed")
                {
                    System.Diagnostics.Debug.WriteLine($"[Podcast Parser] Detected Atom format");
                    ParseAtomFeed(xdoc, feed);
                }
                else
                {
                    throw new Exception($"Unknown feed format: {root.Name.LocalName}");
                }

                return feed;
            }
            catch (System.Xml.XmlException xmlex)
            {
                System.Diagnostics.Debug.WriteLine($"[Podcast Parser] XML Parse Error: {xmlex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Podcast Parser] Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// RSS 形式の feed を解析
        /// </summary>
        private static void ParseRssFeed(XDocument xdoc, PodcastFeed feed)
        {
            var channel = xdoc.Descendants("channel").FirstOrDefault();
            if (channel == null) return;

            // Channel 情報
            feed.Title = channel.Element("title")?.Value ?? "Unknown";
            feed.Description = channel.Element("description")?.Value ?? "";
            feed.Author = channel.Element("author")?.Value 
                       ?? channel.Element("managingEditor")?.Value 
                       ?? "";

            // iTunes 名前空間対応
            var itunesNs = XNamespace.Get("http://www.itunes.com/dtds/podcast-1.0.dtd");
            feed.Author = channel.Element(itunesNs + "author")?.Value ?? feed.Author;
            feed.ImageUrl = channel.Element(itunesNs + "image")?.Attribute("href")?.Value ?? "";
            if (string.IsNullOrEmpty(feed.ImageUrl))
            {
                feed.ImageUrl = channel.Element("image")?.Element("url")?.Value ?? "";
            }

            System.Diagnostics.Debug.WriteLine($"[ParseRssFeed] Channel: {feed.Title}, Author: {feed.Author}");

            // エピソード情報を抽出
            foreach (var item in channel.Elements("item"))
            {
                var episode = new PodcastEpisode();

                episode.Title = item.Element("title")?.Value ?? "Unknown";
                episode.Description = item.Element("description")?.Value ?? "";
                episode.Duration = item.Element(itunesNs + "duration")?.Value ?? "";

                // 公開日
                var pubDateStr = item.Element("pubDate")?.Value;
                if (DateTime.TryParse(pubDateStr, out var pubDate))
                {
                    episode.PubDate = pubDate;
                }

                // エンクロージャ（音声ファイル）
                var enclosure = item.Element("enclosure");
                if (enclosure != null)
                {
                    episode.AudioUrl = enclosure.Attribute("url")?.Value ?? "";
                    episode.AudioType = enclosure.Attribute("type")?.Value ?? "";

                    if (long.TryParse(enclosure.Attribute("length")?.Value, out var length))
                    {
                        episode.AudioLength = length;
                    }
                }

                if (!string.IsNullOrEmpty(episode.AudioUrl))
                {
                    feed.Episodes.Add(episode);
                }
            }
        }

        /// <summary>
        /// Atom 形式の feed を解析
        /// </summary>
        private static void ParseAtomFeed(XDocument xdoc, PodcastFeed feed)
        {
            var atomNs = XNamespace.Get("http://www.w3.org/2005/Atom");
            var root = xdoc.Root;

            // Feed 情報
            feed.Title = root.Element(atomNs + "title")?.Value ?? "Unknown";
            feed.Description = root.Element(atomNs + "subtitle")?.Value ?? "";
            feed.Author = root.Element(atomNs + "author")?.Element(atomNs + "name")?.Value ?? "";

            // 画像
            var logo = root.Element(atomNs + "logo")?.Value;
            feed.ImageUrl = !string.IsNullOrEmpty(logo) ? logo : "";

            // エピソード情報を抽出
            foreach (var entry in root.Elements(atomNs + "entry"))
            {
                var episode = new PodcastEpisode();

                episode.Title = entry.Element(atomNs + "title")?.Value ?? "Unknown";
                episode.Description = entry.Element(atomNs + "summary")?.Value ?? "";

                // 公開日
                var publishedStr = entry.Element(atomNs + "published")?.Value;
                if (DateTime.TryParse(publishedStr, out var published))
                {
                    episode.PubDate = published;
                }

                // 音声 URL (link 要素から取得)
                var audioLink = entry.Elements(atomNs + "link")
                    .FirstOrDefault(e => 
                        e.Attribute("type")?.Value?.Contains("audio") == true
                        || e.Attribute("rel")?.Value == "enclosure");

                if (audioLink != null)
                {
                    episode.AudioUrl = audioLink.Attribute("href")?.Value ?? "";
                    episode.AudioType = audioLink.Attribute("type")?.Value ?? "";

                    if (long.TryParse(audioLink.Attribute("length")?.Value, out var length))
                    {
                        episode.AudioLength = length;
                    }
                }

                if (!string.IsNullOrEmpty(episode.AudioUrl))
                {
                    feed.Episodes.Add(episode);
                }
            }
        }
    }
}
