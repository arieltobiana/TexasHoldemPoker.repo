﻿using Poker.BE.Domain.Utility.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Poker.BE.Domain.Game
{

    /// <summary> Defined the room that the players are playing at the game </summary>
    /// <remarks>
    /// <author>Idan Izicovich</author>
    /// <lastModified>2017-04-25</lastModified>
    /// </remarks>
    public class Room
    {
        #region Constants
        public const int NCHAIRS_IN_ROOM = 10;
        #endregion

        #region Fields
        private ICollection<Player> activeAndPassivePlayers;
        private Deck deck;
        private Chair[] chairs;
        private GameConfig config;

        #endregion

        #region Properties
        // TODO: do we need ID for the Room? if so, what type should it be? 'long?' means nullable long.
        //public long? ID { get; }

        public ICollection<Chair> Chairs { get { return chairs; } }
        public Hand CurrentHand { get; private set; }
        public ICollection<Player> ActivePlayers
        {
            get
            {
                // TODO clean this comment code
                //return activeAndPassivePlayers.Where(
                //    player => (player.CurrentState == Player.State.ActiveUnfolded | player.CurrentState == Player.State.ActiveFolded))
                //    .ToList();

                var result = from player in activeAndPassivePlayers
                             where player.CurrentState != Player.State.Passive
                             select player;

                return result.ToList();

            }
        }
        public ICollection<Player> PassivePlayers
        {
            get
            {
                var result = from player in activeAndPassivePlayers
                             where player.CurrentState == Player.State.Passive
                             select player;

                return result.ToList();
            }
        }
        public ICollection<Player> Players { get { return activeAndPassivePlayers; } }

        #region GameConfig Properties (8)
        public GamePreferences Preferences
        {
            get { return config.Preferences; }
            set { config.Preferences = value; }
        }

        public string Name
        {
            get { return config.Name; }
            set { config.Name = value; }
        }

        public bool IsSpactatorsAllowed
        {
            get { return config.IsSpactatorsAllowed; }
            set { config.IsSpactatorsAllowed = value; }
        }

        public int MaxNumberOfPlayers
        {
            get { return config.MaxNumberOfPlayers; }
            set { config.MaxNumberOfPlayers = value; }
        }

        public int MinNumberOfPlayers
        {
            get { return config.MinNumberOfPlayers; }
            set { config.MinNumberOfPlayers = value; }
        }

        public int MaxNumberOfActivePlayers
        {
            get { return config.MaxNumberOfActivePlayers; }
            set { config.MaxNumberOfActivePlayers = value; }
        }

        public double MinimumBet
        {
            get { return config.MinimumBet; }
            set { config.MinimumBet = value; }
        }

        public double BuyInCost
        {
            get { return config.BuyInCost; }
            set { config.BuyInCost = value; }
        }
        #endregion

        public bool IsTableFull
        {
            get
            {
                return ActivePlayers.Count == MaxNumberOfActivePlayers;
            }
        }


        #endregion

        #region Constructors
        private Room()
        {
            activeAndPassivePlayers = new List<Player>();
            deck = new Deck();
            chairs = new Chair[NCHAIRS_IN_ROOM];

            for (int i = 0; i < NCHAIRS_IN_ROOM; i++)
            {
                Chairs.ToArray()[i] = new Chair(i);
            }

            CurrentHand = null;

            // Note: default configuration
            config = new GameConfig();
        }

        /// <summary>
        /// UC003 Create a new room 
        /// </summary>
        /// <param name="creator">enter the room as a passive player.</param>
        /// <see cref="https://docs.google.com/document/d/1ob4bSynssE3UOfehUAFNv_VDpPbybhS4dW_O-v-QDiw/edit#heading=h.tzy1eb1jifgr"/>
        public Room(Player creator) : this()
        {
            activeAndPassivePlayers.Add(creator);
        }

        /// <summary>
        /// UC003 Create a new room 
        /// </summary>
        /// <param name="creator">enter the room as a passive player.</param>
        /// <param name="preferences">limit / no limit / pot limit </param>
        /// <see cref="https://docs.google.com/document/d/1ob4bSynssE3UOfehUAFNv_VDpPbybhS4dW_O-v-QDiw/edit#heading=h.tzy1eb1jifgr"/>
        public Room(Player creator, GamePreferences preferences) : this(creator)
        {
            Preferences = preferences;
        }

        public Room(Player creator, GameConfig config) : this(creator, config.Preferences)
        {
            /*Note: 8 configurations */
            this.config = config;
        }

        #endregion

        #region Private Functions

        private void TakeAChair(int index)
        {
            chairs[index].Take();
        }

        private void ReleaseAChair(int index)
        {
            chairs[index].Release();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Converting passive player to active player.
        /// precondition: the player must be a passive player at the room.
        /// postcondition: the player is active player at the room.
        /// </summary>
        /// <param name="player">a passive player at the room</param>
        public bool JoinPlayerToTable(Player player)
        {
            if (player.CurrentState != Player.State.Passive)
            {
                return false;
            }

            return player.JoinToTable(BuyInCost);
        }

        public void RemovePlayer(Player player)
        {
            activeAndPassivePlayers.Remove(player);
        }

        /// <summary>
        /// Method as a destructor - delete all players and other resources from the room.
        /// </summary>
        /// <remarks>
        /// this function used be gameCenter do delete the room.
        /// All players and other resources of room need to be deleted.
        /// </remarks>
        public void ClearAll()
        {
            this.activeAndPassivePlayers.Clear();
            this.CurrentHand = null;
            this.Name = new GameConfig().Name;
        }

        public Player CreatePlayer()
        {
            var result = new Player();
            activeAndPassivePlayers.Add(result);
            return result;
        }

        public void StartNewHand()
        {
            CurrentHand = new Hand(deck, ActivePlayers);
        }

        public bool TakeChair(Player player, int index)
        {
            // TODO
            throw new NotImplementedException();
        }

        public bool LeaveChair(Player player)
        {
            // TODO
            throw new NotImplementedException();
        }

        public void SendMessage()
        {
            //TODO: 'UC006: Send Message to Room’s Chat' - for the ones that doing that
        }
        #endregion

    }//class
}
