﻿using Poker.BE.Domain.Core;
using Poker.BE.Domain.Game;
using Poker.BE.Domain.Security;
using Poker.BE.Domain.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.BE.Service.Modules.Caches
{
    public sealed class CommonCache : ICache
    {

        #region Properties
        public UserManager UserManager { get; set; }

        public GameCenter GameCenter { get; set; }

        /// <summary>
        /// Map for the player (user session) ID -> player at the given session.
        /// using the player.GetHashCode() for generating this ID.
        /// </summary>
        /// <remarks>
        /// session ID is the ID we give for a screen the user opens.
        /// this is a need because the user can play several screen at once.
        /// thus, to play with different players at the same time.
        /// 
        /// for now - the user cannot play as several players, at the same room.
        ///    - this option is blocked.
        /// </remarks>
        public IDictionary<int, Player> Players { get; set; }

        public IDictionary<int, Room> Rooms { get; set; }

        public IDictionary<string, User> Users { get; set; }

        public IDictionary<int, League> Leagues { get; set; }

        #endregion

        #region Singleton Constructor
        // Note: for c# implementation
        static CommonCache() { }

        /* Note: Singleton private constructor
         * -----------------------------------
         * 
         * For every Cached Property we should initiate its value here.
         * */
        private CommonCache()
        {
            UserManager = UserManager.Instance;
            GameCenter = GameCenter.Instance;
            Players = new Dictionary<int, Player>();
            Rooms = new Dictionary<int, Room>();
            Users = new Dictionary<string, User>();
            Leagues = new Dictionary<int, League>();
        }

        private static readonly CommonCache _instance = new CommonCache();
        public static CommonCache Instance { get { return _instance; } }

        #endregion

        #region ICache Methods

        private bool UpdateEnumerables<S, D>(IEnumerable<S> source, IEnumerable<D> destination)
        {
            var isUpdated = false;
            // TODO ? - idan.
            return isUpdated;
        }

        public bool Refresh()
        {
            /** 
             * Note: checking if cache has changed after refresh
             *  
             *  Algorithm:
             *      checking if the sequence had changed (by sequenceEqual).
             *      if it had changed - an update occur. and isChanged = true;
             *      
             *  Author:
             *      Idan Izicovich.
             * */
            bool isChanged = false;

            // Users
            if (!UserManager.Users.SequenceEqual(Users))
            {
                UserManager.Users.ToList().ForEach(userPair => Users.Add(userPair.Key, userPair.Value));
                isChanged = true;
            }

            // Players
            if (!GameCenter.Players.SequenceEqual(Players.Values, new AddressComparer<Player>()))
            {
                GameCenter.Players.ToList().ForEach(player => Players.Add(player.GetHashCode(), player));
                isChanged = true;
            }

            // Rooms
            if (!GameCenter.Rooms.SequenceEqual(Rooms.Values))
            {
                GameCenter.Rooms.ToList().ForEach(room => Rooms.Add(room.GetHashCode(), room));
                isChanged = true;
            }

            // Leagues
            //Undone: idan - make sure this is the right dictionary key needed for mapping the leagues at the cache
            if (!GameCenter.Leagues.SequenceEqual(Leagues.Values))
            {
                GameCenter.Leagues.ToList().ForEach(league => Leagues.Add(league.GetHashCode(), league));
                isChanged = true;
            }

            return isChanged;
        }

        public TValue RefreshAndGet<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, Exception e)
        {
            var resultObject = default(TValue);
            while (!dictionary.TryGetValue(key, out resultObject))
            {
                if (Refresh()) continue;

                throw e;
            }

            return resultObject;
        }

        public void Clear()
        {
            GameCenter.ClearAll();
            UserManager.Clear();
            Leagues.Clear();
            Players.Clear();
            Rooms.Clear();
            Users.Clear();
        }
        #endregion

    }
}
