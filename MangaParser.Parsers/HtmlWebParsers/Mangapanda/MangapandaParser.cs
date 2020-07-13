﻿using MangaParser.Parsers.HtmlWebParsers.Mangareader;

namespace MangaParser.Parsers.HtmlWebParsers.Mangapanda
{
    public class MangapandaParser : MangareaderParser
    {
        #region Constructors

        public MangapandaParser(string baseUri = "https://www.mangapanda.com") : base(baseUri)
        {
        }

        #endregion Constructors
    }
}