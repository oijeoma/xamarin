namespace UsersOnlineExample.Utils
{
    using System.Collections.Generic;

    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;
    using StackExchange.Redis;
    using System;
    using Newtonsoft.Json;
    using UsersOnlineExample.Models;

    [HubName("userActivityHub")]
    public class UserActivityHub : Hub
    {

        ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("parkingFree.redis.cache.windows.net,abortConnect=false,ConnectTimeout=10000,ssl=true,password=KVj4EV+ncyS1Om2QTrxrMVe0SqLn3LAM3Bm0gEWmu9U=");
        /// <summary>
        /// The count of users connected.
        /// </summary>
        public static List<string> Users = new List<string>();
        public static List<string> userHandles = new List<string>();
        IDatabase cache = Connection.GetDatabase();

        /// <summary>
        /// Sends the update user count to the listening view.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// 
        public void receiveList(string myHandle,string  myPlace)
        {
            //List<string> userList = new List<string>();
            // userList.Add(myHandle);
            if (lazyConnection.IsValueCreated)
                {
                cache.StringSet("e25", JsonConvert.SerializeObject(new UserHandle(25, 132424)));

                var e25 = JsonConvert.DeserializeObject<UserHandle>(cache.StringGet("132424"));
                var context = GlobalHost.ConnectionManager.GetHubContext<UserActivityHub>();

                context.Clients.All.returnList(e25);
            }
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect("parkingFree.redis.cache.windows.net,abortConnect=false,ssl=true,password=KVj4EV+ncyS1Om2QTrxrMVe0SqLn3LAM3Bm0gEWmu9U=");
        });

        public static ConnectionMultiplexer Connection
        {


            get
            {
                return lazyConnection.Value;
            }
        }

        public void GetDatabase()
        {

            // Connection refers to a property that returns a ConnectionMultiplexer
            // as shown in the previous example.

            // Perform cache operations using the cache object...
            // Simple put of integral data types into the cache
            cache.StringSet("key1", "value");
            cache.StringSet("key2", 25);

            // Simple get of data types from the cache
            string key1 = cache.StringGet("key1");
            int key2 = (int)cache.StringGet("key2");
        }

        public void AddRedisObject()
        {
            // Store to cache
            cache.StringSet("e25", JsonConvert.SerializeObject(new UserHandle(25, 132424)));

            // Retrieve from cache
           
        }

        public void GetRedisObject()
        {
            //Get User for each google place
            // Store to cache
            var  e25 = JsonConvert.DeserializeObject<UserHandle>(cache.StringGet("132424"));
            // Retrieve from cache

        }

        

        public void RemoveRedisObject()
        {

        }

        public void Send(int count, List<string> userHandle)
        {
            // Call the addNewMessageToPage method to update clients.
            var context = GlobalHost.ConnectionManager.GetHubContext<UserActivityHub>();
            context.Clients.All.updateUsersOnlineCount(count, userHandles);
        }
        
        /// <summary>
        /// The OnConnected event.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override System.Threading.Tasks.Task OnConnected()
        {
            string clientId = GetClientId();

            if (Users.IndexOf(clientId) == -1)
            {
                Users.Add(clientId);
            }

            // Send the current count of users
            Send(Users.Count, userHandles);

            return base.OnConnected();
        }

        /// <summary>
        /// The OnReconnected event.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override System.Threading.Tasks.Task OnReconnected()
        {
            string clientId = GetClientId();
            if (Users.IndexOf(clientId) == -1)
            {
                Users.Add(clientId);
            }

            // Send the current count of users
            Send(Users.Count, userHandles);

            return base.OnReconnected();
        }

        /// <summary>
        /// The OnDisconnected event.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override System.Threading.Tasks.Task OnDisconnected()
        {
            string clientId = GetClientId();

            if (Users.IndexOf(clientId) > -1)
            {
                Users.Remove(clientId);
            }

            // Send the current count of users
            Send(Users.Count, userHandles);

            return base.OnDisconnected();
        }

        /// <summary>
        /// Get's the currently connected Id of the client.
        /// This is unique for each client and is used to identify
        /// a connection.
        /// </summary>
        /// <returns>The client Id.</returns>
        private string GetClientId()
        {
            string clientId = "";
            if (Context.QueryString["clientId"] != null)
            {
                // clientId passed from application 
                clientId = this.Context.QueryString["clientId"];
            }

            if (string.IsNullOrEmpty(clientId.Trim()))
            {
                clientId = Context.ConnectionId;
            }

            return clientId;
        }
    }
}