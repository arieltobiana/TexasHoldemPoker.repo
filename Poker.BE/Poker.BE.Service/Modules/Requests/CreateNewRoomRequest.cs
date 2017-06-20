﻿using Poker.BE.Domain.Game;

namespace Poker.BE.Service.Modules.Requests
{
	public class CreateNewRoomRequest : IRequest
	{
		public int Level { get; set; }
		/// <summary>
		/// user Name
		/// </summary>
		public string User { get; set; }
	}
}