﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Poker.BE.Domain.Core;
using Poker.BE.Domain.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.BE.Domain.Core.Tests
{
    [TestClass()]
    public class GameCenterTests
    {
        #region Set Up

        public TestContext TestContext { get; set; }
        private GameCenter gameCenter;

        [TestInitialize]
        public void Before()
        {
            gameCenter = GameCenter.Instance;
        }

        [TestCleanup]
        public void After()
        {
            gameCenter.ClearAll();
            gameCenter = null;
        }
        #endregion

        #region Find an Existing Room - Functions Overloading Tests
        [TestMethod()]
        public void FindRoomsByCriteriaTest_level()
        {
            //Arrange

            GameConfig config = new GameConfig();
            int level = 4;
            var expRoom = gameCenter.CreateNewRoom(level, config, out Player creator);
            Exception expE = null;
            expRoom.Name = "test room";


            TestContext.WriteLine("min level {0} max level {1}", gameCenter.Leagues.Single().MinLevel, gameCenter.Leagues.Single().MaxLevel);

            //Act
            var actRoom = gameCenter.FindRoomsByCriteria(45);
            try
            {
                gameCenter.FindRoomsByCriteria(0);

                Assert.Fail("exception missing");
            }
            catch (Utility.Exceptions.RoomNotFoundException e)
            {
                expE = e;
            }

            //Assert
            Assert.IsNotNull(expE);
            TestContext.WriteLine("exception message: {0}", expE.Message);
            Assert.AreEqual(1, actRoom.Count);
            Assert.AreEqual(expRoom, actRoom.Single());
        }

        [TestMethod]
        public void FindRoomsByCriteriaTest_multiple_rooms()
        {
            //Arrange
            GameConfig config = new GameConfig();
            int level = 4;
            var expRoom = gameCenter.CreateNewRoom(level, config, out Player creator).Name = "test room 1";
            var expRoom2 = gameCenter.CreateNewRoom(level, config, out Player creator2).Name = "test room 2";

            //Act
            var actual = gameCenter.FindRoomsByCriteria(25);

            //Assert
            Assert.AreEqual(2, actual.Count);
        }

        [TestMethod()]
        public void FindRoomsByCriteriaTest_player()
        {
            //Arrange
            var expRoom = gameCenter.CreateNewRoom(6, new GameConfig(), out Player expPlayer);

            //Act
            var actual = gameCenter.FindRoomsByCriteria(-1, expPlayer);
            try
            {
                gameCenter.FindRoomsByCriteria(-1, new Player());
                Assert.Fail("room not found exception");
            }
            catch (Utility.Exceptions.RoomNotFoundException e)
            {
                TestContext.WriteLine("exception message " + e.Message);
            }

            //Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(expRoom, actual.Single());
        }

        [TestMethod()]
        public void FindRoomsByCriteriaTest_perf()
        {
            // TODO
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void FindRoomsByCriteriaTest_betsize()
        {
            // TODO
            throw new NotImplementedException();
        }
        #endregion

        [TestMethod()]
        public void EnterRoomTest()
        {
            //Arrange
            var expected = new Player();
            var actual = default(Player);
            Room room = new Room(new Player());

            //Act
            actual = gameCenter.EnterRoom(room);

            //Assert
            Assert.AreEqual(expected, actual, "returned player");
            Assert.IsNotNull(actual.Nickname, "nickname not null");
            Assert.AreEqual(0, actual.Wallet, "wallet = 0 (before buy in to table)");
        }

        [TestMethod()]
        public void CreateNewRoomTest()
        {
            //Arrange
            var expPlayer = new Player();
            var expRoom = new Room(expPlayer);

            GameConfig inConfig = new GameConfig();
            int inLevel = 3;

            //Act
            var actual = gameCenter.CreateNewRoom(inLevel, inConfig, out Player actCreator);

            //Assert
            Assert.AreEqual(expPlayer, actCreator, "eq creators");
            Assert.AreEqual(1, gameCenter.Rooms.Count, "rooms count");
            Assert.AreEqual(1, gameCenter.Players.Count, "players count");
            Assert.AreEqual(1, gameCenter.Leagues.Count, "leagues count");
            Assert.AreEqual(actual, gameCenter.Rooms.First(), "room is registered");
            Assert.AreEqual(expPlayer, gameCenter.Players.First(), "player is registered");
            Assert.IsTrue(gameCenter.Leagues.First().Rooms.Contains(actual), "league is registered and contain room");
        }

        [TestMethod()]
        [ExpectedException(typeof(Utility.Exceptions.NotEnoughMoneyException))]
        public void JoinNextHandTest()
        {
            //Arrange
            var expRoom = gameCenter.CreateNewRoom(1, new GameConfig(), out Player expPlayer);
            double inBuyin = expRoom.BuyInCost - 20.2;
            int inSeatIndex = 2;

            //Act
            gameCenter.JoinNextHand(expPlayer, inSeatIndex, inBuyin);

            //Assert

            // if no exception occurs
            Assert.Fail("not enough money");
        }

        [TestMethod]
        [ExpectedException(typeof(Utility.Exceptions.RoomNotFoundException))]
        public void JoinNextHandTest1()
        {
            //Arrange
            //var expected = ;

            Player expPlayer = new Player();
            double inBuyin = 0;
            int inSeatIndex = 0;

            //Act
            gameCenter.JoinNextHand(expPlayer, inSeatIndex, inBuyin);

            //Assert

            // if no exception
            Assert.Fail("room not found for player");
        }

        [TestMethod]
        [ExpectedException(typeof(Utility.Exceptions.RoomRulesException))]
        public void JoinNextHandTest2()
        {
            //Arrange
            int seatIndex = 200;
            var expRoom = gameCenter.CreateNewRoom(1, new GameConfig(), out Player expPlayer);
            double buyIn = expRoom.BuyInCost + 2.5;

            //Act
            gameCenter.JoinNextHand(expPlayer, seatIndex, buyIn);

            //Assert
            Assert.Fail("seat taken / not exists");
        }

        [TestMethod]
        public void JoinNextHand_success()
        {
            //Arrange
            var expRoom = gameCenter.CreateNewRoom(1, new GameConfig(), out Player expPlayer);
            expPlayer.Nickname = "yosi";
            int seatIndex = 4;
            double buyIn = expRoom.BuyInCost;

            //Act
            gameCenter.JoinNextHand(expPlayer, seatIndex, buyIn);

            //Assert
            Assert.AreEqual(Player.State.ActiveUnfolded, expPlayer.CurrentState);
            Assert.AreEqual(buyIn, expPlayer.Wallet);
            Assert.AreEqual(expPlayer, expRoom.TableLocationOfActivePlayers[expRoom.Chairs.ElementAt(seatIndex)]);
            Assert.AreEqual(1, expRoom.ActivePlayers.Count);
        }

        [TestMethod()]
        [ExpectedException(typeof(Utility.Exceptions.PlayerModeException))]
        public void StandUpToSpactateTest()
        {
            //Arrange
            var expRoom = gameCenter.CreateNewRoom(1, new GameConfig(), out Player actPlayer);
            actPlayer.Nickname = "yossi";
            var expMoney = expRoom.BuyInCost;
            gameCenter.JoinNextHand(actPlayer, 2, expMoney);

            //Act
            var actMoney = gameCenter.StandUpToSpactate(actPlayer);

            //Assert
            Assert.Fail("expected exception: player mode exception");
        }

        [TestMethod]
        public void StandUpToSpactateTest_success()
        {
            //Arrange
            var expRoom = gameCenter.CreateNewRoom(1, new GameConfig(), out Player actPlayer);
            actPlayer.Nickname = "yossi";
            var expMoney = expRoom.BuyInCost;
            gameCenter.JoinNextHand(actPlayer, 2, expMoney);

            actPlayer.CurrentState = Player.State.ActiveFolded;

            //Act
            var actMoney = gameCenter.StandUpToSpactate(actPlayer);

            //Assert
            Assert.AreEqual(expMoney, actMoney);
            Assert.AreEqual(Player.State.Passive, actPlayer.CurrentState);
            Assert.AreEqual(0, expRoom.ActivePlayers.Count);
        }
    }
}