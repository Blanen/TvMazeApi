using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VideolandAssignment.DTOModels
{
    public class ShowListDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<ShowListPersonDto> Cast { get; set; }
    }

    public class ShowListPersonDto
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public string BirthDate { get; set; }
    }
    public partial class ShowDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public partial class CastDto
    {
        [JsonProperty("person", NullValueHandling = NullValueHandling.Ignore)]
        public PersonDto Person { get; set; }
    }


    public partial class PersonDto
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("birthday", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Birthday { get; set; }

    }
}
