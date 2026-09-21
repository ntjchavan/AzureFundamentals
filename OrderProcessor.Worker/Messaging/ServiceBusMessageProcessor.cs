using Microsoft.Extensions.Options;
using OrderProcessor.Worker.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessor.Worker.Messaging
{
    public class ServiceBusMessageProcessor
    {
        private readonly ServiceBusOptions _options;

        public ServiceBusMessageProcessor(IOptions<ServiceBusOptions> options)
        {
            _options = options.Value;
        }


    }
}
