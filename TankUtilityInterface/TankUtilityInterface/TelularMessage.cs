using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


public enum RequestType
{
    Create,
    Delete,
    Discover,
    Execute,
    Observe,
    Read,
    WriteAttributes,
    Write,
    Register,
    Update,
    Unregister,
    Notify,
    BootstrapWrite,
    BootstrapDelete,
    BootstrapRequest,
    BootstrapFinish
}


namespace DeviceActor.Common.DataContracts
{
    [DataContract]
    public class TelularMessage
    {
        /// <summary>
        /// A unique string that identifies the device model.
        /// This should most likely include the business unit for ease of routing/filtering through the 
        /// architecture of components. This field could be augmented by the gateway.
        /// </summary>
        [DataMember(Name = "devicetype")]
        public string DeviceType { get; set; }

        /// <summary>
        /// A unique string that identifies a specific individual device.
        /// This must be unique through the entire system.
        /// </summary>
        [DataMember(Name = "deviceid")]
        public string DeviceID { get; set; }

        /// <summary>
        /// A unique string that identifies a specific message for the current device.
        /// The combination of device ID and message ID must be unique throughout the entire system.
        /// </summary>
        [DataMember(Name = "messageid")]
        public string MessageID { get; set; }

        /// <summary>
        /// A timestamp, accurate to the millisecond, the message was created
        /// </summary>
        [DataMember(Name = "createdtimestamp")]
        public DateTime CreatedTimestamp { get; set; }

        /// <summary>
        /// A timestamp, accurate to the millisecond, the message was created in UTC format
        /// </summary>
        [DataMember(Name = "utccreatedtimestamp")]
        public DateTime UTCCreatedTimestamp { get; set; }

        /// <summary>
        /// A string indicating what type of message the current instance represents.
        /// This value should be one of several pre-defined message types.
        /// The type of message will give the architecture components the needed context to know 
        /// what additional properties are present in the list of properties.
        /// </summary>
        [DataMember(Name = "messagetype")]
        [JsonConverter(typeof(StringEnumConverter))]
        public RequestType MessageType { get; set; }

        /// <summary>
        /// A string describing the origination source of the message.
        /// This could be the name of the software component or endpoint.
        /// This field needs to be added by the gateway.
        /// </summary>
        [DataMember(Name = "source")]
        public string Source { get; set; }

        /// <summary>
        /// A string describing the destination of the message.
        /// This could be the name of the software component or endpoint.
        /// This field needs to be added by the gateway.
        /// </summary>
        [DataMember(Name = "destination")]
        public string Destination { get; set; }

        /// <summary>
        /// A string that represents the schema of the current message.
        /// This allows updates to the message layout without disrupting
        /// the currently deployed components.
        /// </summary>
        [DataMember(Name = "schema")]
        public string Schema { get; set; }

        /// <summary>
        /// A string representing the high level category this message belongs to.
        /// For example all message types that are considered to be an alarm or alert
        /// would have a value of "Alarm". This would assist all system components 
        /// with routing, filtering and classification. This field needs to be
        /// added by the gateway.
        /// </summary>
        [DataMember(Name = "messagecategory")]
        public string MessageCategory { get; set; }

        [DataMember(Name = "partitionid")]
        public string PartitionId { get; set; }

        /// <summary>
        /// All other properties not explicitly listed in the "header"
        /// section will be in this generic list of string-based name/value pairs.
        /// </summary>
        [DataMember(Name = "properties")]
        public List<TelularMessageProperty> Properties { get; set; }

        public TelularMessage()
        {
            Properties = new List<TelularMessageProperty>();
        }
    }
}
