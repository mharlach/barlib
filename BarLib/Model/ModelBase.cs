using System;

namespace BarLib
{
    public abstract class ModelBase
    {
        public string Id { get; set; } = string.Empty;

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}