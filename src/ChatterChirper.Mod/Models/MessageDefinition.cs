using System;
using System.Collections.Generic;

namespace ChatterChirper.Models
{
    [Serializable]
    public class MessageDefinition
    {
        public string id;
        public string category;
        public int severity; // 1-5
        public Dictionary<string, string> conditions;
        public float weight = 1.0f;
        public int cooldown = 600; // seconds
        public List<string> texts;

        public MessageDefinition()
        {
            conditions = new Dictionary<string, string>();
            texts = new List<string>();
        }
    }
}