using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OrderProcessor.Worker.Configuration
{
    public class ServiceBusOptions
    {
        [Required]
        public string ConnectionString { get; set; } = string.Empty;

        [Required]
        public string QueueName { get; set; } = string.Empty;

        [Range(1, 100)]
        public int MaxConcurrentCalls { get; set; }

        [Range(0, 200)]
        public int PrefetchCount { get; set; }
    }
}
