using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Cross platform INotificationService Resolver.
    /// </summary>

    public static partial class NotificationCenter
    {
        private static INotificationService _current;

        /// <summary>
        /// Platform specific INotificationService.
        /// </summary>
        public static INotificationService Current
        {
            get =>
                _current ?? throw new InvalidOperationException(Properties.Resources.PluginNotFound);
            set => _current = value;
        }

        /// <summary>
        /// Return Notification Key.
        /// </summary>
        public static string ReturnRequest => "Plugin.LocalNotification.RETURN_REQUEST";

        /// <summary>
        ///
        /// </summary>
        public static JsonSerializerOptions MyJsonSerializerOptions { get; } = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonValueConverterTimeSpan()
            }
        };

        internal static NotificationRequest GetRequest(string serializedRequest)
        {
            System.Diagnostics.Debug.WriteLine($"Serialized Request [{serializedRequest}]");
            if (string.IsNullOrWhiteSpace(serializedRequest))
            {
                return null;
            }

            var request = JsonSerializer.Deserialize<NotificationRequest>(serializedRequest, MyJsonSerializerOptions);
            return request;
        }

        internal static List<NotificationRequest> GetRequestList(string serializedRequestList)
        {
            if (string.IsNullOrWhiteSpace(serializedRequestList))
            {
                return new List<NotificationRequest>();
            }
            try
            {
                var requestList = JsonSerializer.Deserialize<List<NotificationRequest>>(serializedRequestList, MyJsonSerializerOptions);
                return requestList;
            } catch(Exception ex)
            {
                Debug.Write(serializedRequestList);
                Debug.Write("Fail to deserialized");
                var requestNewton = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NotificationRequest>>(serializedRequestList);
                Debug.Write("Newtonsoft works\r\n-------------------");
                return requestNewton;                
            }            
        }

        internal static string GetRequestListSerialize(List<NotificationRequest> requestList)
        {
            foreach (var request in requestList)
            {
                if (request.Image.Binary != null && request.Image.Binary.Length > 90000)
                {
                    request.Image.Binary = null;
                }
            }
            var serializedRequestList = JsonSerializer.Serialize(requestList, MyJsonSerializerOptions);
            return serializedRequestList;
        }

        internal static string GetRequestSerialize(NotificationRequest request)
        {
            if (request.Image.Binary != null && request.Image.Binary.Length > 90000)
            {
                request.Image.Binary = null;
            }
            var serializedRequest = JsonSerializer.Serialize(request, MyJsonSerializerOptions);

            System.Diagnostics.Debug.WriteLine($"Serialized Request [{serializedRequest}]");

            return serializedRequest;
        }

        internal static Dictionary<string, string> GetRequestSerializeDictionary(NotificationRequest request)
        {
            var dictionary = new Dictionary<string, string>();

            if (request.Image.Binary != null && request.Image.Binary.Length > 90000)
            {
                request.Image.Binary = null;
            }
            var serializedRequest = GetRequestSerialize(request);
            dictionary.Add(ReturnRequest, serializedRequest);
            return dictionary;
        }
    }
}