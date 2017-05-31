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
        private Chair[] chairs;
        private GameConfig config;
		private int dealerIndex = 0;
        #endregion

        #region Properties
        // TODO: do we need ID for the Room? if so, what type should it be? 'long?' means nullable long.
        //public long? ID { get; }

        /* UNDONE: Tomer - 
            we'll just return the totalRaise field that holds the highest raise so far at the table.
            that will let the next player know what's the amount of money he needs to invest in order to keep playing.
            that's enough, right?
         */

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
        public IDictionary<Chair, Player> TableLocationOfActivePlayers { get; private set; }

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

            chairs = new Chair[NCHAIRS_IN_ROOM];

            for (int i = 0; i < NCHAIRS_IN_ROOM; i++)
            {
                chairs[i] = new Chair(i);
            }

            CurrentHand = null;
            TableLocationOfActivePlayers = new Dictionary<Chair, Player>();

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

        #region Methods
        /// <summary>
        /// Converting passive player to active player.
        /// precondition: the player must be a passive player at the room.
        /// postcondition: the player is active player at the room.
        /// </summary>
        /// <param name="player">a passive player at the room</param>
        public bool JoinPlayerToTable(Player player, double buyIn)
        {
            if (player.CurrentState != Player.State.Passive)
            {
                return false;
            }

            return player.JoinToTable(buyIn);
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

        /// <summary>
        /// UC014 Start (Deal) a New Hand
        /// </summary>
        /// <see cref="https://docs.google.com/document/d/1OTee6BGDWK2usL53jdoeBOI-1Jh8wyNejbQ0ZroUhcA/edit#heading=h.3z6a7b6nlnjj"/>
        public void StartNewHand()
        {
            if (ActivePlayers.Count < 2)
            {
                throw new NotEnoughPlayersException("Its should be at least 2 active players to start new hand!");
            }
            if (this.CurrentHand != null && this.CurrentHand.Active)
            {
                throw new NotEnoughPlayersException("The previous hand hasnt ended");
            }

            Player dealer = ActivePlayers.ElementAt(dealerIndex);
            CurrentHand = new Hand(dealer, ActivePlayers, config);
            CurrentHand.PrepareHand();
            CurrentHand.PlayHand();
        }

        public void EndCurrentHand()
        {
            CurrentHand.EndHand();
            dealerIndex++;
            //TODO: implementation
        }

        public bool TakeChair(Player player, int index)
        {
            try
            {
                var result = PassivePlayers.Where(item => item == player);
                if (result.Count() != 1 || !chairs[index].Take())
                {
                    return false;
                }

                // register player location at the table.
                TableLocationOfActivePlayers.Add(chairs[index], player);

                return true;
            }
            catch (IndexOutOfRangeException)
            {
                throw new RoomRulesException("chair index does not exists");
            }
        }

        public void LeaveChair(Player player)
        {
            if (TableLocationOfActivePlayers.Values.Contains(player))
            {
                foreach (var chair in TableLocationOfActivePlayers.Keys)
                {
                    if(TableLocationOfActivePlayers[chair] == player)
                    {
                        TableLocationOfActivePlayers.Remove(chair);
                        chair.Release();
                        return;
                    }
                }
            }
        }

        public void SendMessage()
        {
            //TODO: 'UC006: Send Message to Room’s Chat' - for the ones that doing that
        }
        #endregion

    }
}
