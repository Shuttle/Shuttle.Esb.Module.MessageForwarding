﻿using System.Collections.Generic;

namespace Shuttle.Esb.Module.MessageForwarding
{
    public class MessageForwardingOptions
    {
        public const string SectionName = "Shuttle:Modules:MessageForwarding";

        public List<MessageRouteOptions> ForwardingRoutes { get; set; } = new List<MessageRouteOptions>();
    }
}