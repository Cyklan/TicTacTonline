﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class MatchHistoryDocument: Document
    {
        public List<MatchHistoryPosition> Matches { get; set; }
    }
}
