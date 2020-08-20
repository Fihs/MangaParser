﻿using MangaParser.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace MangaParser.Core.Models
{
    /// <summary>
    /// Provides an object representation of a manga with some information.
    /// </summary>
    [Serializable]
    public class MangaObject : DataBase<IName>, IMangaObject
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MangaObject"/> class with a <see langword="null"/> parameters.
        /// </summary>
        public MangaObject() : base()
        {
            Covers = new List<ICover>();
            Authors = new List<IDataBase<IName>>();
            Genres = new List<IDataBase<IName>>();
            Illustrators = new List<IDataBase<IName>>();
            Writers = new List<IDataBase<IName>>();
            Magazines = new List<IDataBase<IName>>();
            Publishers = new List<IDataBase<IName>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MangaObject"/> class with a minimum parameters.
        /// </summary>
        /// <param name="value">A data with a manga name.</param>
        /// <param name="url">An url to the manga.</param>
        public MangaObject(IName value, Uri url) : base(value, url)
        {
            Covers = new List<ICover>();
            Authors = new List<IDataBase<IName>>();
            Genres = new List<IDataBase<IName>>();
            Illustrators = new List<IDataBase<IName>>();
            Writers = new List<IDataBase<IName>>();
            Magazines = new List<IDataBase<IName>>();
            Publishers = new List<IDataBase<IName>>();
        }

        /// <inheritdoc cref="MangaObject(IName, Uri)"/>
        public MangaObject(IName value, string url) : base(value, url)
        {
            Covers = new List<ICover>();
            Authors = new List<IDataBase<IName>>();
            Genres = new List<IDataBase<IName>>();
            Illustrators = new List<IDataBase<IName>>();
            Writers = new List<IDataBase<IName>>();
            Magazines = new List<IDataBase<IName>>();
            Publishers = new List<IDataBase<IName>>();
        }

        #endregion Constructors

        #region Properties

        public virtual ICollection<ICover> Covers { get; set; }
        public virtual ICollection<IDataBase<IName>> Authors { get; set; }
        public virtual ICollection<IDataBase<IName>> Genres { get; set; }
        public virtual IDataBase<string> Description { get; set; }
        public virtual ICollection<IDataBase<IName>> Illustrators { get; set; }
        public virtual ICollection<IDataBase<IName>> Writers { get; set; }
        public virtual ICollection<IDataBase<IName>> Magazines { get; set; }
        public virtual ICollection<IDataBase<IName>> Publishers { get; set; }
        public virtual IDataBase<DateTime> ReleaseDate { get; set; }
        public virtual IDataBase<int> Volumes { get; set; }

        public virtual string Source => Url != null ? Url.Host : "Unknown";

        #endregion Properties
    }
}