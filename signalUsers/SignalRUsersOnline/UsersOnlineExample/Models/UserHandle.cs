using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UsersOnlineExample.Models
{
    public class UserHandle
    {

        public int UserHandleId { get; set; }

        public int GooglePlaceId { get; set; }

        public UserHandle(int UserHandleId, int GooglePlaceId)
        {
            this.UserHandleId = UserHandleId;
            this.GooglePlaceId = GooglePlaceId;
        }
    }
}