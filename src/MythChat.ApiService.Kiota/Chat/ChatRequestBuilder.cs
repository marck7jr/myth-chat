// <auto-generated/>
using Microsoft.Kiota.Abstractions;
using MythChat.ApiService.Kiota.Chat.Agents;
using MythChat.ApiService.Kiota.Chat.Ask;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
namespace MythChat.ApiService.Kiota.Chat {
    /// <summary>
    /// Builds and executes requests for operations under \chat
    /// </summary>
    public class ChatRequestBuilder : BaseRequestBuilder {
        /// <summary>The agents property</summary>
        public AgentsRequestBuilder Agents { get =>
            new AgentsRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The ask property</summary>
        public AskRequestBuilder Ask { get =>
            new AskRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new ChatRequestBuilder and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public ChatRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/chat", pathParameters) {
        }
        /// <summary>
        /// Instantiates a new ChatRequestBuilder and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public ChatRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/chat", rawUrl) {
        }
    }
}
