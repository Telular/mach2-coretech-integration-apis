using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisCacheLib
{
    public static class Constants
    {
        public static class Lwm2mResponseTypes
        {
            public const string DEVICE_CAPABILITY = "DeviceCapability";
        }

        public static class Lwm2mResponseCodes
        {
            public const string CHANGED = "Changed";
            public const string CREATED = "Created";
        }

        public static class Lwm2mResourceNames
        {
            public static class VistaPanel
            {
                public const string ADDRESS = "VistaPanel.Address";
                public const string ZONE_PROGRAM_TYPE = "VistaPanel.ZonePrgmType";
                public const string USER_MANAGEMENT_TYPE = "VistaPanel.UserMgmtType";
                public const string USER_DIGITS = "VistaPanel.UserDigits";
                public const string ZONE_DIGITS = "VistaPanel.ZoneDigits";
            }
        }

        public static class Parsing
        {
            public const char PAYLOAD_DELIMITER = ',';
            public const char OBSERVATION_DELIMITER = '|';
            public const char OBJECT_NAME_DELIMITER = '.';
            public const string OBJECT_DELIMITER = "/";
            public const string OBJECT_START_CHAR = "<";
            public const string OBJECT_END_CHAR = ">";
        }

        public static class DefaultValues
        {
            //Since there is always only one panel, the instance ID is always 0.
            public const int PANEL_INSTANCE_ID = 0;
            public const int MT_MESSAGE_TIMEOUT_MINS = 10;
        }

        public static class Logging
        {
            public const string DATA_WEB_API_DEPENDENCY_NAME = "DataWebApi";
            public const string SENSOR_MESSAGING_COMMAND_NAME = "LookupSensorMessaging";
            public const string PANEL_SETTINGS_COMMAND_NAME = "GetPanelSettings";
        }

        public static class Configuration
        {
            public const string CACHE_DELETE_KEY = "CacheDeleteKey";
            public const string DEVICE_MESSAGING_CONNECT_KEY = "DeviceMessaging";
            public const string CACHE_CONNECT_KEY = "CacheConfigString";
            public const string OBSERVE_OBJECT_IDS = "ObservableObjectIDs";
            public const string ENABLE_DEVICE_VALIDATION_KEY = "ValidateDeviceRegistration";
            public const string APPLICATION_INSIGHTS_KEY = "AppInsightsInstrumentationKey";
            public const string APPLICATION_INSIGHTS_DEVICE_MANAGEMENT_KEY = "AppInsightsDMInstrumentationKey";
            public const string APPLICATION_NAME_KEY = "AppName";
            public const string DATA_WEB_API_URL_KEY = "DataWebApiUrl";
            public const string MT_WEB_API_URL_KEY = "MTWebApiUrl";
            public const string CACHE_TYPE = "cacheType";
        }

        public static class Protocols
        {
            public const string LWM2M_PROTOCOL_PREFIX = "LWM2M";
        }

        public static class DocDBLocations
        {
            public const string DEVICE_COLLECTION = "Devices";
            public const string MESSAGES_COLLECTION = "Messages";
            public const string CLIENT_REGISTRATIONS_COLLECTION = "ClientRegistrations";
            public const string DATABASE_ID = "Items";
            public const string OBSERVATION_COLLECTION = "Observations";
            public const string DEVICE_TRANSACTIONS_DOCUMENT_LOCATION = "/dbs/Items/colls/DeviceTransactions/";
            public const string DEVICE_TRANSACTIONS_COLLECTION = "DeviceTransactions";

            public const string DEVICE_DESIRED_PASS_THROUGH_DATA = "Data";
            public const string DEVICE_DESIRED_PASS_THROUGH_INSTANCE_ID = "InstanceID";
        }

        public static class DocDBQueries
        {
            public const string SELECT_DEVICE_BY_ID = "SELECT * FROM Devices d WHERE d.id = @id";
            public const string SELECT_DEVICE_MESSAGES_BY_DEVICE_ID = "SELECT TOP 50 * FROM Messages m WHERE m.deviceid = @id ORDER BY m._ts DESC";
            public const string SELECT_CLIENT_REGISTRATION_BY_DEVICE_ID = "SELECT * FROM ClientRegistrations c WHERE c.endpoint = @id";
            public const string SELECT_ALL_CLIENT_REGISTRATIONS = "SELECT * FROM ClientRegistrations";
            public const string SELECT_COUNT_CLIENT_REGISTRATIONS = "SELECT VALUE COUNT(1) FROM ClientRegistrations";
            public const string SELECT_OBSERVATIONS_BY_DEVICE_ID = "SELECT * FROM Observations o WHERE o.deviceid = @id";
            public const string SELECT_OBSERVATIONS_BY_OBSERVATION_ID = "SELECT * FROM Observations o WHERE o.id = @id";

            public const string SELECT_DEVICE_TRANSACTION_BY_ID = "SELECT * FROM DeviceTransactions d WHERE d.id = @id";
            public const string SELECT_DEVICE_TRANSACTION_BY_DEVICE_ID = "SELECT TOP 50 * FROM DeviceTransactions d WHERE d.deviceid = @id ORDER BY d._ts DESC";
            public const string SELECT_DEVICE_TRANSACTION_BY_DEVICE_ID_AND_LWPATH = "SELECT TOP 1 * FROM DeviceTransactions d WHERE d.deviceid = @deviceID AND d.message_type = @messageType AND d.lw_path = @lwPath ORDER BY d._ts DESC";
            public const string SELECT_DEVICE_TRANSACTION_BY_DEVICE_ID_AND_LWMESSAGEID = "SELECT TOP 1 * FROM DeviceTransactions d WHERE d.deviceid = @deviceID AND d.lw_message_id = @lwMessageID ORDER BY d._ts DESC";
        }

        public static class DocDBPartitionKeys
        {
            public const string PARTITION_KEY_PROPERTY_NAME = "partitionKey";
            public const string BOOTSTRAP_PARTITION = "bootstrapConfig";
        }

        public static class DependencyType
        {
            public const string KEY_VAULT = "Key Vault";
        }

        public static class DependencyCommand
        {
            public const string GENERATE_BOOTSTRAP_PSK = "Generate Bootstrap PSK";
            public const string RETRIEVE_SECRET = "Retrieve Key Vault Secret";
        }

        public static class DependencyName
        {
            public const string BOOTSTRAP_TO_KEY_VAULT = "Bootstrap Server To Key Vault";
        }

        public static class LoggingProperties
        {
            public const string APN = "APN";
            public const string CLIENT = "Client";
            public const string DEVICE_ID = "Device ID";
            public const string ENDPOINT = "Endpoint";
            public const string IMEI = "IMEI";
            public const string IMSI = "IMSI";
            public const string IP_TYPE = "IP Type";
            public const string ITERATION = "Iteration";
            public const string MESSAGE_CONTENTS = "Message Contents";
            public const string MESSAGE_ID = "Message ID";
            public const string MESSAGE_TYPE = "Message Type";
            public const string PATH = "Path";
            public const string PDP_CONTEXT = "PDP Context";
            public const string PREVIOUS_APN = "Previous APN";
            public const string REGISTRATION_ID = "Registration ID";
            public const string STATUS = "Status";
            public const string TRANSACTION_ID = "Transaction ID";
            public const string DEVICE_CONTENTS = "Device Contents";
            public const string INSTANCE_ID = "Instance ID";
            public const string REQUEST_ID = "Request ID";
            public const string SEARCH_PROPERTY = "Search Property";
            public const string SEARCH_VALUE = "Search Value";
            public const string SECRET_NAME = "Secret Name";
            public const string VERSION = "Version";

            public const string COMMAND_TYPE = "Command type";
            public const string COMMAND_SET_DESIRED_PASS_THROUGH_DATA = "Set Desired Pass Through Data";
            public const string COMMAND_SET_REPORTED_PASS_THROUGH_DATA = "Set Reported Pass Through Data";
            public const string COMMAND_CREATE_PASS_THROUGH_DATA = "Create New Pass Through Data";
            public const string COMMAND_GET_NEW_PASS_THROUGH_DATA = "Get New Pass Through Data";
            public const string COMMAND_REMOVE_NEW_PASS_THROUGH_DATA = "Remove New Pass Through Data";
            public const string COMMAND_REMOVE_NEW_PASS_THROUGH_DATA_FAILED = "Failed to Remove New Pass Through Data";

            public const string EVENT_TRANSACTION_STATUS_UPDATE = "MT Web API - Device Transaction Status Updated";
            public const string EVENT_TRANSACTION_MESSAGE_ID_UPDATE = "LW Gateway - Device Transaction MT Message ID Updated";
            public const string EVENT_TRANSACTION_STATUS_UPDATE_FAILED = "MT Web API - Device Transaction Status Update Failed";

            public static class Carrier
            {
                public const string ACCOUNT_NAME = "Account Name";
                public const string CARRIER_MESSAGE = "Carrier Message";
                public const string CARRIER_NAME = "Carrier Name";
                public const string DEVICE_REQUEST_OBJECT = "Device Request Object";
                public const string ERROR = "Error";
                public const string ERROR_CODE = "Error Code";
                public const string ERROR_MESSAGE = "Error Message";
                public const string FIELD = "Field";
                public const string OPERATION = "Operation";
                public const string STATUS_CODE = "Status Code";
                public const string REQUEST_ID = "Request ID";
                public const string REQUEST_OBJECT = "Request Object";
                public const string VALUE = "Value";
                public const string ERROR_TITLE = "Error Title";

                public const string DEVICE_NOT_FOUND = "Carrier API - Device not found";
                public const string EDIT_DEVICE = "Carrier API - Update Device Request";
                public const string EDIT_DEVICE_ERROR = "Carrier API - Update device returned unexpected result";
                public const string EDIT_DEVICE_SUCCESS = "Carrier API - Update Device Request Success";
                public const string GET_DEVICE = "Carrier API - Get Device Request";
                public const string GET_DEVICE_SUCCESS = "Carrier API - Get Device Request Success";
                public const string GET_DEVICE_ERROR = "Carrier API - Get Device Request returned unexpected result";
                public const string UPLOAD_DEVICES = "Carrier API - Upload Devices";
                public const string UPLOAD_DEVICES_SUCCESS = "Carrier API - Upload Devices Request Success";
                public const string UPLOAD_DEVICES_ERROR = "Carrier API - Upload Devices returned unexpected result";

                public const string VERIZON_REQUEST_XML = "Verizon Request XML";
                public const string VERIZON_GET_DEVICE = "Verizon Get Device Request";
                public const string VERIZON_CHANGE_DEVICE_STATE = "Verizon Change Device State Request";
                public const string VERIZON_CHANGE_CUSTOM_FIELDS = "Verizon Change Custom Fields";

                public const string ASSETLINKGLOBAL_REQUEST_JSON = "Asset Link Global Request JSON";
                public const string ASSETLINKGLOBAL_GET_DEVICE = "Asset Link Global Get Device Request";
                public const string ASSETLINKGLOBAL_CHANGE_DEVICE_STATE = "Asset Link Global Change Device State Request";
                public const string ASSETLINKGLOBAL_API_RESULT = "Asset Link Global API Result";

            }
        }

        public static class LogExceptions
        {
            public const string INVALID_DOC_DB_LOCATION = "Invalid document db location.";
            public const string INVALID_CARRIER_REQUEST = "Invalid Carrier API request";
            public const string DEVICE_UPLOAD_MISMATCH = "Device upload type and device identifiers do not match.";
        }


        public static class JsonPropertyNames
        {
            public static class TransportPath
            {
                public const string PRIORITY = "Priority";
                public const string RETRY_COUNT = "MaxRetryCount";
                public const string PATH = "Path";
                public const string RETRY_INTERVALS = "RetryIntervals";
                public const string FAMILY_CODE = "FamilyCode";
                public const string PROTOCOL = "Protocol";
            }

            public static class DeviceInfo
            {
                public const string IS_VALID = "IsValid";
                public const string DEVICE_ID = "DeviceID";
                public const string REGISTRATION_ID = "RegistrationID";
                public const string DEVICE_JSON_STRING = "DeviceJsonString";
            }

            public static class CommonProperties
            {
                public const string ID = "id";
                public const string NAME = "name";
                public const string SEQUENCE_ID = "sequenceid";
                public const string PARENT_ID = "parentid";
                public const string PARENT_SEQUENCE_ID = "parentsequenceid";
                public const string ORIGINATION_SOURCE_TYPE = "originationsourcetype";
                public const string VALUE = "value";
                public const string DATA_TYPE = "datatype";
            }

            public static class CommonHeader
            {
                // Header fields
                public const string DEVICE_TYPE = "devicetype";
                public const string DEVICE_ID = "deviceid";
                public const string MESSAGE_ID = "messageid";
                public const string CREATED_TIMESTAMP = "createdtimestamp";
                public const string MESSAGE_TYPE = "messagetype";
                public const string SOURCE = "source";
                public const string DESTINATION = "destination";
                public const string SCHEMA = "schema";
                public const string MESSAGE_CATEGORY = "messagecategory";
                public const string PROPERTIES = "properties";
            }

            public static class DataWebApi
            {
                //Sensor Messaging 
                public const string ADD_ON_PROPERTY = "addons";
                public const string SENSOR_MESSAGING_CODE = "SNMSG";
                public const string CODE_PROPERTY = "code";

                //Panel Settings
                public const string PANEL_SETTINGS_PROPERTY = "settings";
                public const string PANEL_NAME_PROPERTY = "panelname";
                public const string PANEL_ADDRESS_PROPERTY = "paneladdress";
                public const string ZONE_NAME_PROPERTY = "zonename";
                public const string USER_MANAGEMENT_TYPE = "description";
                public const string USER_DIGITS_PROPERTY = "userdigits";
                public const string ZONE_DIGITS_PROPERTY = "zonedigits";
            }

            public static class MTWebApi
            {
                public const string SERIAL_NUMBER_PROPERTY = "serialNo";
                public const string PROPERTIES_PROPERTY = "properties";
                public const string PAYLOAD_PROPERTY = "payload";
                public const string SOURCE_PROPERTY = "source";
                public const string PATH_PROPERTY = "path";
                public const string DESTINATION_PROPERTY = "destination";

                public const string CREATE_MESSAGE = "api/device/create";
                public const string EXECUTE_MESSAGE = "api/device/execute";
                public const string OBSERVE_MESSAGE = "api/device/observe";
            }
        }

        public static class JsonParsingDataTypes
        {
            public const string STRING_DATA_TYPE = "String";
            public const string INTEGER_DATA_TYPE = "Integer";
            public const string FLOAT_DATA_TYPE = "Float";
            public const string BOOLEAN_DATA_TYPE = "Boolean";
            public const string OPAQUE_DATA_TYPE = "Opaque";
            public const string TIME_DATA_TYPE = "Time";
        }

        public static class GeoJSONProperties
        {
            public const string GEO_JSON_TYPE_NAME = "type";
            public const string GEO_JSON_GEOMETRY_TYPE_NAME = "type";
            public const string FEATURES_NAME = "features";
            public const string GEOMETRY_NAME = "geometry";
            public const string GEOMETRIES_NAME = "geometries";
            public const string PROPERTIES_NAME = "properties";
            public const string COORDINATES_NAME = "coordinates"; // required element if the GeoJSONGeometryType is something other than GeometryCollection
            public const string ID_NAME = "id";
        }

        public static class FormatStrings
        {
            public const string MT_JSON_PROTOCOL_FORMAT = "json-{0}";
            public const string TRANSPORT_PATH_HASH_KEY_FORMAT = "transportpath-{0}-{1}";
            public const string OBJECT_ID_HASH_KEY_FORMAT = "objectid-{0}";
            public const string DEVICE_FAMILY_PATH_KEY_FORMAT = "devicefamilypaths-{0}";
            public const string SENSOR_MESSAGING_URL = "business/serviceaddons/{0}";
            public const string PANEL_SETTINGS_URL = "business/panelsettings/{0}";
            public const string HUB_REGISTRATION_URL = "business/HubRegistration";
            public const string DEVICE_RESPONSE = "business/DeviceResponse";
            public const string FILE_READ = "business/FileReadComplete";
            public const string DEVICE_DELETED = "business/DeviceDeleted";
            public const string UPDATE_SUBTYPE = "business/UpdateDeviceType";
            public const string OBSERVATION_ID = "{0}|{1}";
        }

        public static class Cache
        {
            public const string PANEL_ID_KEY = "panelids";
            public const string MESSAGE_CATEGORY_KEY = "messagecategories";
            public const string DEVICE_ID_KEY = "DeviceID";
            public const string DEVICE_INFO_KEY = "DeviceInfo";
            public const string DEVICE_AGE_KEY = "DeviceAge";
            public const string CLIENT_REGISTRY_KEY = "ClientRegistry";
            public const string CLIENT_REGISTRATIONS_KEY = "ClientRegistrations";
            public const string OBSERVATION_KEY = "Observations";
            public const string OBSERVATION_REGISTRY_KEY = "ObservationRegistry";
            public const string OBSERVABLEOBJECTS_KEY = "observableobjects";
        }

        public static class CacheType
        {
            public const string REDIS = "redis";
            public const string IN_MEMORY = "inmemory";
            public const string DOC_DB = "docdb";
            public const string NONE = "none";
        }

        public static class EventProcessorActions
        {
            public const string NOT_EQUALS = "notEquals";
            public const string EQUALS = "equals";
            public const string STARTS_WITH = "startsWith";
            public const string ENDS_WITH = "endsWith";
            public const string CONTAINS = "contains";
            public const string STATIC = "static";
            public const string REPLACE = "replace";
        }

        public static class Carrier
        {
            public static class Identifiers
            {
                public const string ESN = "esn";
                public const string ICCID = "iccid";
                public const string IMEI = "imei";
                public const string IMSI = "imsi";
                public const string MDN = "mdn";
                public const string MIN = "min";
                public const string MSISDN = "msisdn";
                public const string URN = "urn";
            }

            public static class Jasper
            {
                public const string CARRIER_NAME = "Jasper";
                public const string CARRIER_NAME_LOWER_CASE = "jasper";

                public const string ACCOUNT_ID = "accountId";
                public const string CUSTOM1 = "custom1";
                public const string CUSTOM2 = "custom2";
                public const string CUSTOM3 = "custom3";
                public const string CUSTOMER = "customer";
                public const string FIELD_NAC_ID = "nacId";
                public const string FIXED_IP_ADDRESS = "fixedIpAddress";
                public const string MODEM_ID = "modemId";
                public const string RATE_PLAN = "ratePlan";
                public const string STATUS = "status";
                public const string TERMINAL_ID = "terminalId";
            }

            public static class Operation
            {
                public const string EDIT_DEVICE = "Edit Device";
                public const string GET_DEVICE = "Get Device";
                public const string GET_DEVICE_NETWORK_CONFIG = "Get Device Network Config";
                public const string STATUS = "Status";
                public const string UPLOAD_DEVICE = "Upload Device";
            }

            public static class Verizon
            {
                public const string CARRIER_NAME = "Verizon";
                public const string CARRIER_NAME_LOWER_CASE = "verizon";

                public const string ACCOUNT_NAME = "AccountName";
                public const string CUSTOM_FIELD_1 = "CustomField1";
                public const string CUSTOM_FIELD_2 = "CustomField2";
                public const string CUSTOM_FIELD_3 = "CustomField3";
                public const string CUSTOM_FIELD_4 = "CustomField4";
                public const string CUSTOM_FIELD_5 = "CustomField5";
                public const string CUSTOM_FIELDS = "CustomFields";
                public const string IP_ADDRESS = "IPAddress";
                public const string LEAD_ID = "LeadId";
                public const string SERVICE_PLAN = "ServicePlan";
                public const string SERVICE_ZIP_CODE = "ServiceZipCode";
                public const string STATE = "State";

                public const string STATE_CHANGE = "State Change";
                public const string STATE_ACTIVATE = "Activate";
                public const string STATE_DEACTIVATE = "Deactivate";
                public const string STATE_SUSPEND = "Suspend";
                public const string STATE_RESTORE = "Restore";

                public const string ERROR_STRING_INPUT_INVALID = "INPUT_INVALID";
                public const string ERROR_STRING_INVALID_INPUT = "INVALID_INPUT";
                public const string ERROR_STRING_INPUT_MISSING = "INPUT_MISSING";
                public const string ERROR_STRING_REQUEST_FAILED = "REQUEST_FAILED";

                public const string ERROR_MESSAGE_DEVICE_NOT_FOUND = "Verizon device not found";
            }

            public static class VerizonThingSpace
            {
                public const string CARRIER_NAME = "VerizonThingSpace";
                public const string CARRIER_NAME_LOWER_CASE = "verizonthingspace";

                public static class UploadDeviceUploadType
                {
                    public const string IMEI = "IMEI";
                    public const string IMEI_ICCID_PAIR = "IMEI ICCID Pair";
                    public const string IMEI_ICCID_EMBEDDED_SIM_PAIR = "IMEI ICCID Embedded SIM Pair";
                    public const string IMEI_EID_PAIR = "IMEI EID Pair";
                }

                public static class Method
                {
                    public const string Get = "GET";
                    public const string Post = "POST";
                    public const string Put = "PUT";
                    public const string Delete = "DELETE";
                }

                public static class PostDataType
                {
                    public const string Json = "JSON";
                    public const string Xml = "Xml";
                    public const string FORM_ENCODED = "application/x-www-form-urlencoded";
                }

                public static class DeviceIdentifiers
                {
                    public const string ICCID = "ICCID";
                    public const string IMEI = "IMEI";
                }
            }

            public static class AssetLinkGlobal
            {
                public const string CARRIER_NAME = "AL-Iridium";
                public const string CARRIER_NAME_LOWER_CASE = "al-iridium";

                public const string TEST_CARRIER_NAME = "Test-AL-Iridium";
                public const string TEST_CARRIER_NAME_LOWER_CASE = "test-al-iridium";

                public const string SERVICE_PLAN = "servicePlan";
                public const string SERVICE_PLAN_STARTED = "servicePlanStarted";
                public const string STATUS = "status";
                public const string DEVICE_ID = "deviceid";
                public const string ESN = "ESN";

                public const string PLAN_PROVISIONED_7_6_KB = "SBD Track 7.6 KB";
                public const string PLAN_PROVISIONED_3_2_KB = "SBD Track 3.2 KB";
                public const string PLAN_DEPROVISIONED = "DEACTIVE";
                public const string DEVICE_ACTIVATED = "ACTIVATED";
                public const string DEVICE_DEACTIVATED = "DEACTIVATED";

                public static class ApiFileIdentifiers
                {
                    public const string DEVICE = "device";
                    public const string MOMMENT = "moment";
                    public const string GROUP = "grp";
                    public const string REMOTE = "remote";
                }

                public static class ApiActionsIdentifiers
                {
                    public const string GET = "get";
                    public const string PROVISION = "provision";
                    public const string DEPROVISION = "deprovision";
                    public const string REPORT = "report";

                }

            }
        }
    }
}
