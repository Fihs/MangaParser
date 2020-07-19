﻿using HtmlAgilityPack;
using MangaParser.Core.Interfaces;
using MangaParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaParser.Parsers.HtmlWebParsers.MangaFox
{
    public class MangaFoxParser : CoreParser
    {
        #region Constructors

        public MangaFoxParser(string baseUri = "https://fanfox.net") : base(baseUri)
        {
        }

        #endregion Constructors

        #region Methods

        #region Search

        public override IEnumerable<IMangaBase> SearchManga(string query)
        {
            query = query is null ? String.Empty : query.Replace(' ', '+');

            var htmlDoc = Web.Load(BaseUrl + $"/search?name={query}");

            return SearchMangaCore(htmlDoc);
        }

        protected override IEnumerable<IMangaBase> SearchMangaCore(HtmlDocument htmlDoc)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='container']/div[@class='line-list']/div[@class='manga-list-4 mt15']/ul[@class='manga-list-4-list line']");

            return GetSearchResult(mainNode);
        }

        #endregion Search

        #region GetManga

        public override IManga GetManga(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            ChangeUrlToMobile(ref url);

            var htmlDoc = Web.Load(url);

            var manga = GetMangaCore(htmlDoc);

            (manga as MangaObject).Url = url;

            return manga;
        }

        private static void ChangeUrlToMobile(ref Uri url)
        {
            if (!url.Host.Contains("m."))
            {
                UriBuilder uriBuilder = new UriBuilder(url);
                uriBuilder.Host = "m." + uriBuilder.Host;
                url = uriBuilder.Uri;
            }
        }

        public override async Task<IManga> GetMangaAsync(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            ChangeUrlToMobile(ref url);

            var htmlDoc = await Web.LoadFromWebAsync(url.OriginalString).ConfigureAwait(false);

            var manga = GetMangaCore(htmlDoc);

            (manga as MangaObject).Url = url;

            return manga;
        }

        protected override IManga GetMangaCore(HtmlDocument htmlDoc)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='container']/div[@class='detail-info']");

            return GetMangaData(mainNode);
        }

        #endregion GetManga

        #region GetChapters

        public override IEnumerable<IChapter> GetChapters(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            ChangeUrlToMobile(ref url);

            return base.GetChapters(url);
        }

        public override Task<IEnumerable<IChapter>> GetChaptersAsync(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            ChangeUrlToMobile(ref url);

            return base.GetChaptersAsync(url);
        }

        protected override IEnumerable<IChapter> GetChaptersCore(HtmlDocument htmlDoc)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/section/div/div[3]/div[1]");

            return GetChapters(mainNode);
        }

        #endregion GetChapters

        #region GetPages

        public override IEnumerable<IPage> GetPages(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            ChangeUrlToMobile(ref url);

            return base.GetPages(url);
        }

        public override Task<IEnumerable<IPage>> GetPagesAsync(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            ChangeUrlToMobile(ref url);

            return base.GetPagesAsync(url);
        }

        protected override IEnumerable<IPage> GetPagesCore(HtmlDocument htmlDoc)
        {
            if (htmlDoc is null)
            {
                throw new ArgumentNullException(nameof(htmlDoc));
            }

            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='site-content']/section[@class='main']/div[@class='mangaread-main']");

            return GetMangaPages(mainNode);
        }

        #endregion GetPages

        #region Private Methods

        private Chapter[] GetChapters(HtmlNode mainNode)
        {
            var chapters = mainNode?.SelectNodes(".//a");

            Chapter[] Chapters;

            if (chapters != null)
            {
                Chapters = new Chapter[chapters.Count];

                for (int i = chapters.Count - 1; i >= 0; i--)
                {
                    DateTime.TryParse(Decode(chapters[i].SelectSingleNode("./span")?.InnerText), out DateTime date);

                    Chapters[chapters.Count - 1 - i] = new Chapter(Decode(chapters[i].InnerText), "http:" + chapters[i].Attributes["href"]?.Value, date);
                }
            }
            else
            {
                Chapters = new Chapter[0];
            }

            return Chapters;
        }

        private MangaObject GetMangaData(HtmlNode mainNode)
        {
            var infoNode = mainNode?.SelectSingleNode(".//div[@class='detail-info-right']");

            var manga = new MangaObject
            {
                Name = GetName(infoNode),
                Description = GetDescription(infoNode),
                Covers = GetCovers(mainNode),
                Genres = GetGenres(infoNode),
                Autors = GetAutors(infoNode),
            };

            return manga;
        }

        private async Task<MangaPage[]> GetMangaPagesAsync(HtmlNode mainNode)
        {
            string imgNodePath = mainNode.XPath + "/div[@class='mangaread-img']/a/img";

            var Counters = mainNode.SelectNodes("./div[@class='mangaread-operate  clearfix']/select/option");

            MangaPage[] pages;

            if (Counters != null)
            {
                pages = new MangaPage[Counters.Count];

                pages[0] = new MangaPage(mainNode.SelectSingleNode("./div[@class='mangaread-img']/a/img")?.Attributes["src"]?.Value);

                HtmlDocument htmlDoc;

                for (int i = 1; i < Counters.Count; i++)
                {
                    htmlDoc = await Web.LoadFromWebAsync("http:" + Counters[i].Attributes["value"]?.Value);

                    pages[i] = new MangaPage(htmlDoc.DocumentNode.SelectSingleNode(imgNodePath)?.Attributes["src"]?.Value);
                }
            }
            else
                pages = new MangaPage[0];

            return pages;
        }

        private IEnumerable<MangaPage> GetMangaPages(HtmlNode mainNode)
        {
            string imgNodePath = mainNode.XPath + "/div[@class='mangaread-img']/a/img";

            var Counters = mainNode.SelectNodes("./div[@class='mangaread-operate  clearfix']/select/option");

            if (Counters != null)
            {
                yield return new MangaPage(mainNode.SelectSingleNode("./div[@class='mangaread-img']/a/img")?.Attributes["src"]?.Value);

                HtmlDocument htmlDoc;

                for (int i = 1; i < Counters.Count; i++)
                {
                    htmlDoc = Web.Load("http:" + Counters[i].Attributes["value"]?.Value);

                    yield return new MangaPage(htmlDoc.DocumentNode.SelectSingleNode(imgNodePath)?.Attributes["src"]?.Value);
                }
            }
        }

        private IEnumerable<MangaObject> GetSearchResult(HtmlNode mainNode)
        {
            var thumbs = mainNode?.SelectNodes("./li");

            if (thumbs != null)
            {
                for (int i = 0; i < thumbs.Count; i++)
                {
                    var manga = new MangaObject
                    {
                        Autors = new DataBase[] { new DataBase(thumbs[i].SelectSingleNode("./p[@class='manga-list-4-item-tip']/a")?.Attributes["title"]?.Value, thumbs[i].SelectSingleNode("./p[@class='manga-list-4-item-tip']/a")?.Attributes["href"]?.Value) },
                        Genres = new DataBase[] { new DataBase(Decode(thumbs[i].SelectSingleNode("./p[@class='manga-list-4-show-tag-list-2']/a")?.InnerText), default(Uri)) },
                        Covers = new Cover[] { new Cover(thumbs[i].SelectSingleNode("./a/img")?.Attributes["src"]?.Value, null, null) },
                        Url = new Uri(BaseUrl, thumbs[i].SelectSingleNode("./a")?.Attributes["href"]?.Value),
                        Description = Decode(thumbs[i].LastChild?.InnerText),
                        Name = new Name(null, thumbs[i].SelectSingleNode("./a")?.Attributes["title"]?.Value, null),
                    };

                    yield return manga;
                }
            }
        }

        #region Data Getting Methods

        private DataBase[] GetAutors(HtmlNode infoNode)
        {
            var autorsNode = infoNode?.SelectNodes("./p[@class='detail-info-right-say']/a");

            DataBase[] autors;

            if (autorsNode != null)
            {
                autors = new DataBase[autorsNode.Count];

                for (int i = 0; i < autorsNode.Count; i++)
                {
                    autors[i] = new DataBase(autorsNode[i].Attributes["title"]?.Value, autorsNode[i].Attributes["href"]?.Value);
                }
            }
            else
                autors = new DataBase[0];

            return autors;
        }

        private Cover[] GetCovers(HtmlNode mainNode)
        {
            var imageNode = mainNode?.SelectSingleNode("./div[@class='detail-info-cover']/img");

            Cover[] images;

            if (imageNode != null)
            {
                images = new Cover[] { new Cover(imageNode.Attributes["src"]?.Value, null, null) };
            }
            else
                images = new Cover[0];

            return images;
        }

        private string GetDescription(HtmlNode infoNode)
        {
            var descNode = infoNode?.SelectSingleNode("./p[@class='fullcontent']");

            if (descNode != null)
            {
                return Decode(descNode.InnerText);
            }
            else
                return null;
        }

        private DataBase[] GetGenres(HtmlNode infoNode)
        {
            var genreNode = infoNode?.SelectNodes("./p[@class='detail-info-right-tag-list']/a");

            DataBase[] genres;

            if (genreNode != null)
            {
                genres = new DataBase[genreNode.Count];

                for (int i = 0; i < genreNode.Count; i++)
                {
                    genres[i] = new DataBase(genreNode[i].Attributes["title"]?.Value, genreNode[i].Attributes["href"]?.Value);
                }
            }
            else
                genres = new DataBase[0];

            return genres;
        }

        private Name GetName(HtmlNode infoNode)
        {
            var nameNode = infoNode?.SelectSingleNode("./p[@class='detail-info-right-title']/span[@class='detail-info-right-title-font']");

            if (nameNode != null)
            {
                return new Name(null, Decode(nameNode.InnerText), null);
            }
            else
                return new Name();
        }

        #endregion Data Getting Methods

        #endregion Private Methods

        #endregion Methods
    }
}