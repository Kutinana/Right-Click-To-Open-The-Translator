using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace DataSystem
{
    [Serializable]
    public class CharacterProgressData : IHaveId
    {
        [JsonProperty] public string Id { get; set; }
        [JsonProperty] public string Meaning { get; set; } = "";
        [JsonProperty] public List<string> RelatedPuzzles { get; set; } = new();

        public CharacterProgressData(string _id)
        {
            Id = _id;
            Meaning = "";
            RelatedPuzzles = new List<string>();
        }

        public CharacterProgressData(string _id, string _meaning)
        {
            Id = _id;
            Meaning = _meaning;
            RelatedPuzzles = new List<string>();
        }

        [JsonConstructor] public CharacterProgressData(string _id, List<string> _relatedPuzzles, string _meaning)
        {
            Id = _id;
            Meaning = _meaning;
            RelatedPuzzles = _relatedPuzzles;
        }
    }
}