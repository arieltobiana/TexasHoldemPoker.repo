﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.BE.Service.Modules.Results
{
	public class LogoutResult : IResult
	{
		public int User { get; internal set; }
	}
}
