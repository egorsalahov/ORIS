using GameAndDot.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameAndDot.Shared.Models
{
    public class EventMessage
    {
        public EventType type {  get; set; }
        public string Id { get; set; }
        public string Username { get; set; }
        public string Color { get; set; }

        //координаты точки
        public int X { get; set; }
        public int Y { get; set; }

        public List<PlayerInfo> Players { get; set; } = new();
    }
}
