using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace DataSystem
{
    public class CharacterProgressData : IHaveId
    {
        [JsonProperty] public string Id { get; set; }
        [JsonProperty] public string Meaning { get; set; }

        public List<string> RelatedPuzzles;

        public CharacterProgressData (string _id)
        {
            Id = _id;
            Meaning = "";
            RelatedPuzzles = new List<string>();
        }

        public CharacterProgressData (string _id, string _meaning)
        {
            Id = _id;
            Meaning = _meaning;
            RelatedPuzzles = new List<string>();
        }
    }
}