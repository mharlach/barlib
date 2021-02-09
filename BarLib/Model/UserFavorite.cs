using System;
using System.Collections.Generic;

namespace BarLib
{
    public class UserFavorite
    {
        public int Rating { get; set; }
        public string DrinkId { get; set; } = string.Empty;
    }

    public class UserFavorites : UserModelBase
    {
        public List<UserFavorite> Favorites {get;set;} = new List<UserFavorite>();
        public override string ObjectType {get;set;} = "userFavorites";
    }
}