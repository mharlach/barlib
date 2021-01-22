using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BarLib
{
    public abstract class StorageObjectBase : IValidatableObject
    {
        [Required]
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("pk")]
        public string PartitionKey { get; set; } = string.Empty;

        [JsonProperty("created")]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [JsonProperty("updated")]
        public DateTime Updated { get; set; } = DateTime.UtcNow;

        [JsonProperty("type")]
        public abstract string ObjectType {get;set;}

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) => new List<ValidationResult>();

    }

    public abstract class ModelBase : StorageObjectBase
    {
        [Required]
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }

    public abstract class UserModelBase : StorageObjectBase
    {
        [Required]
        [JsonProperty("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonProperty("hashCode")]
        public int HashCode { get; set; }
    }

}