#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace stoogebag.Editor.Tools
{
    [Serializable]
    public class BookmarkCategory
    {
        public string name = "New Category";
        public List<UnityEngine.Object> items = new List<UnityEngine.Object>();
    }

    public class BookmarkData : ScriptableObject
    {
        public List<BookmarkCategory> categories = new List<BookmarkCategory>();
    }
}
#endif
