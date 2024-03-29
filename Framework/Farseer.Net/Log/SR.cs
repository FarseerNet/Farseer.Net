// <auto-generated>
using System.Reflection;
#pragma warning disable 649

 namespace FxResources.System.Text.Json
 {
     internal static class SR { }
 }

namespace FS.Log
{
    internal static partial class SR
    {
        private static global::System.Resources.ResourceManager s_resourceManager;
        internal static global::System.Resources.ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new global::System.Resources.ResourceManager(typeof(FxResources.System.Text.Json.SR)));

        /// <summary>The maximum configured depth of {0} has been exceeded. Cannot read next JSON array.</summary>
        internal static string @ArrayDepthTooLarge => GetResourceString("ArrayDepthTooLarge");
        /// <summary>The JSON writer needs to be flushed before getting the current state. There are {0} bytes that have not been committed to the output.</summary>
        internal static string @CallFlushToAvoidDataLoss => GetResourceString("CallFlushToAvoidDataLoss");
        /// <summary>Cannot read incomplete UTF-16 JSON text as string with missing low surrogate.</summary>
        internal static string @CannotReadIncompleteUTF16 => GetResourceString("CannotReadIncompleteUTF16");
        /// <summary>Cannot read invalid UTF-16 JSON text as string. Invalid surrogate value: '{0}'.</summary>
        internal static string @CannotReadInvalidUTF16 => GetResourceString("CannotReadInvalidUTF16");
        /// <summary>Cannot write the start of an object/array after a single JSON value or outside of an existing closed object/array. Current token type is '{0}'.</summary>
        internal static string @CannotStartObjectArrayAfterPrimitiveOrClose => GetResourceString("CannotStartObjectArrayAfterPrimitiveOrClose");
        /// <summary>Cannot write the start of an object or array without a property name. Current token type is '{0}'.</summary>
        internal static string @CannotStartObjectArrayWithoutProperty => GetResourceString("CannotStartObjectArrayWithoutProperty");
        /// <summary>Cannot transcode invalid UTF-8 JSON text to UTF-16 string.</summary>
        internal static string @CannotTranscodeInvalidUtf8 => GetResourceString("CannotTranscodeInvalidUtf8");
        /// <summary>Cannot decode JSON text that is not encoded as valid Base64 to bytes.</summary>
        internal static string @CannotDecodeInvalidBase64 => GetResourceString("CannotDecodeInvalidBase64");
        /// <summary>Cannot transcode invalid UTF-16 string to UTF-8 JSON text.</summary>
        internal static string @CannotTranscodeInvalidUtf16 => GetResourceString("CannotTranscodeInvalidUtf16");
        /// <summary>Cannot encode invalid UTF-16 text as JSON. Invalid surrogate value: '{0}'.</summary>
        internal static string @CannotEncodeInvalidUTF16 => GetResourceString("CannotEncodeInvalidUTF16");
        /// <summary>Cannot encode invalid UTF-8 text as JSON. Invalid input: '{0}'.</summary>
        internal static string @CannotEncodeInvalidUTF8 => GetResourceString("CannotEncodeInvalidUTF8");
        /// <summary>Cannot write a JSON property within an array or as the first JSON token. Current token type is '{0}'.</summary>
        internal static string @CannotWritePropertyWithinArray => GetResourceString("CannotWritePropertyWithinArray");
        /// <summary>Cannot write a JSON property name following another property name. A JSON value is missing.</summary>
        internal static string @CannotWritePropertyAfterProperty => GetResourceString("CannotWritePropertyAfterProperty");
        /// <summary>Cannot write a JSON value after a single JSON value or outside of an existing closed object/array. Current token type is '{0}'.</summary>
        internal static string @CannotWriteValueAfterPrimitiveOrClose => GetResourceString("CannotWriteValueAfterPrimitiveOrClose");
        /// <summary>Cannot write a JSON value within an object without a property name. Current token type is '{0}'.</summary>
        internal static string @CannotWriteValueWithinObject => GetResourceString("CannotWriteValueWithinObject");
        /// <summary>CurrentDepth ({0}) is equal to or larger than the maximum allowed depth of {1}. Cannot write the next JSON object or array.</summary>
        internal static string @DepthTooLarge => GetResourceString("DepthTooLarge");
        /// <summary>Writing an empty JSON payload (excluding comments) is invalid.</summary>
        internal static string @EmptyJsonIsInvalid => GetResourceString("EmptyJsonIsInvalid");
        /// <summary>Expected end of comment, but instead reached end of data.</summary>
        internal static string @EndOfCommentNotFound => GetResourceString("EndOfCommentNotFound");
        /// <summary>Expected end of string, but instead reached end of data.</summary>
        internal static string @EndOfStringNotFound => GetResourceString("EndOfStringNotFound");
        /// <summary>'{0}' is invalid after a single JSON value. Expected end of data.</summary>
        internal static string @ExpectedEndAfterSingleJson => GetResourceString("ExpectedEndAfterSingleJson");
        /// <summary>'{0}' is an invalid end of a number. Expected a delimiter.</summary>
        internal static string @ExpectedEndOfDigitNotFound => GetResourceString("ExpectedEndOfDigitNotFound");
        /// <summary>'{0}' is an invalid JSON literal. Expected the literal 'false'.</summary>
        internal static string @ExpectedFalse => GetResourceString("ExpectedFalse");
        /// <summary>The input does not contain any JSON tokens. Expected the input to start with a valid JSON token, when isFinalBlock is true.</summary>
        internal static string @ExpectedJsonTokens => GetResourceString("ExpectedJsonTokens");
        /// <summary>The input does not contain any complete JSON tokens. Expected the input to have at least one valid, complete, JSON token.</summary>
        internal static string @ExpectedOneCompleteToken => GetResourceString("ExpectedOneCompleteToken");
        /// <summary>'{0}' is an invalid end of a number. Expected 'E' or 'e'.</summary>
        internal static string @ExpectedNextDigitEValueNotFound => GetResourceString("ExpectedNextDigitEValueNotFound");
        /// <summary>'{0}' is an invalid JSON literal. Expected the literal 'null'.</summary>
        internal static string @ExpectedNull => GetResourceString("ExpectedNull");
        /// <summary>'{0}' is invalid after a property name. Expected a ':'.</summary>
        internal static string @ExpectedSeparatorAfterPropertyNameNotFound => GetResourceString("ExpectedSeparatorAfterPropertyNameNotFound");
        /// <summary>'{0}' is an invalid start of a property name. Expected a '"'.</summary>
        internal static string @ExpectedStartOfPropertyNotFound => GetResourceString("ExpectedStartOfPropertyNotFound");
        /// <summary>Expected start of a property name or value, but instead reached end of data.</summary>
        internal static string @ExpectedStartOfPropertyOrValueNotFound => GetResourceString("ExpectedStartOfPropertyOrValueNotFound");
        /// <summary>'{0}' is an invalid start of a value.</summary>
        internal static string @ExpectedStartOfValueNotFound => GetResourceString("ExpectedStartOfValueNotFound");
        /// <summary>'{0}' is an invalid JSON literal. Expected the literal 'true'.</summary>
        internal static string @ExpectedTrue => GetResourceString("ExpectedTrue");
        /// <summary>Expected a value, but instead reached end of data.</summary>
        internal static string @ExpectedValueAfterPropertyNameNotFound => GetResourceString("ExpectedValueAfterPropertyNameNotFound");
        /// <summary>The 'IBufferWriter' could not provide an output buffer that is large enough to continue writing.</summary>
        internal static string @FailedToGetLargerSpan => GetResourceString("FailedToGetLargerSpan");
        /// <summary>'{0}' is invalid after a value. Expected either ',', '}}', or ']'.</summary>
        internal static string @FoundInvalidCharacter => GetResourceString("FoundInvalidCharacter");
        /// <summary>Cannot get the value of a token type '{0}' as a {1}.</summary>
        internal static string @InvalidCast => GetResourceString("InvalidCast");
        /// <summary>'{0}' is an invalid escapable character within a JSON string. The string should be correctly escaped.</summary>
        internal static string @InvalidCharacterAfterEscapeWithinString => GetResourceString("InvalidCharacterAfterEscapeWithinString");
        /// <summary>'{0}' is invalid within a JSON string. The string should be correctly escaped.</summary>
        internal static string @InvalidCharacterWithinString => GetResourceString("InvalidCharacterWithinString");
        /// <summary>'{0}' is an invalid token type for the end of the JSON payload. Expected either 'EndArray' or 'EndObject'.</summary>
        internal static string @InvalidEndOfJsonNonPrimitive => GetResourceString("InvalidEndOfJsonNonPrimitive");
        /// <summary>'{0}' is not a hex digit following '\u' within a JSON string. The string should be correctly escaped.</summary>
        internal static string @InvalidHexCharacterWithinString => GetResourceString("InvalidHexCharacterWithinString");
        /// <summary>Comments cannot be stored in a JsonDocument, only the Skip and Disallow comment handling modes are supported.</summary>
        internal static string @JsonDocumentDoesNotSupportComments => GetResourceString("JsonDocumentDoesNotSupportComments");
        /// <summary>The requested operation requires an element of type '{0}', but the target element has type '{1}'.</summary>
        internal static string @JsonElementHasWrongType => GetResourceString("JsonElementHasWrongType");
        /// <summary>Max depth must be positive.</summary>
        internal static string @MaxDepthMustBePositive => GetResourceString("MaxDepthMustBePositive");
        /// <summary>The JsonCommentHandling enum must be set to one of the supported values.</summary>
        internal static string @CommentHandlingMustBeValid => GetResourceString("CommentHandlingMustBeValid");
        /// <summary>'{0}' is invalid without a matching open.</summary>
        internal static string @MismatchedObjectArray => GetResourceString("MismatchedObjectArray");
        /// <summary>'{0}' is invalid following a property name.</summary>
        internal static string @CannotWriteEndAfterProperty => GetResourceString("CannotWriteEndAfterProperty");
        /// <summary>The maximum configured depth of {0} has been exceeded. Cannot read next JSON object.</summary>
        internal static string @ObjectDepthTooLarge => GetResourceString("ObjectDepthTooLarge");
        /// <summary>The JSON property name of length {0} is too large and not supported.</summary>
        internal static string @PropertyNameTooLarge => GetResourceString("PropertyNameTooLarge");
        /// <summary>The JSON value is either too large or too small for a Decimal.</summary>
        internal static string @FormatDecimal => GetResourceString("FormatDecimal");
        /// <summary>Either the JSON value is not in a supported format, or is out of bounds for a Double.</summary>
        internal static string @FormatDouble => GetResourceString("FormatDouble");
        /// <summary>Either the JSON value is not in a supported format, or is out of bounds for an Int32.</summary>
        internal static string @FormatInt32 => GetResourceString("FormatInt32");
        /// <summary>Either the JSON value is not in a supported format, or is out of bounds for an Int64.</summary>
        internal static string @FormatInt64 => GetResourceString("FormatInt64");
        /// <summary>Either the JSON value is not in a supported format, or is out of bounds for a Single.</summary>
        internal static string @FormatSingle => GetResourceString("FormatSingle");
        /// <summary>Either the JSON value is not in a supported format, or is out of bounds for a UInt32.</summary>
        internal static string @FormatUInt32 => GetResourceString("FormatUInt32");
        /// <summary>Either the JSON value is not in a supported format, or is out of bounds for a UInt64.</summary>
        internal static string @FormatUInt64 => GetResourceString("FormatUInt64");
        /// <summary>'{0}' is invalid within a number, immediately after a decimal point ('.'). Expected a digit ('0'-'9').</summary>
        internal static string @RequiredDigitNotFoundAfterDecimal => GetResourceString("RequiredDigitNotFoundAfterDecimal");
        /// <summary>'{0}' is invalid within a number, immediately after a sign character ('+' or '-'). Expected a digit ('0'-'9').</summary>
        internal static string @RequiredDigitNotFoundAfterSign => GetResourceString("RequiredDigitNotFoundAfterSign");
        /// <summary>Expected a digit ('0'-'9'), but instead reached end of data.</summary>
        internal static string @RequiredDigitNotFoundEndOfData => GetResourceString("RequiredDigitNotFoundEndOfData");
        /// <summary>.NET number values such as positive and negative infinity cannot be written as valid JSON.</summary>
        internal static string @SpecialNumberValuesNotSupported => GetResourceString("SpecialNumberValuesNotSupported");
        /// <summary>The JSON value of length {0} is too large and not supported.</summary>
        internal static string @ValueTooLarge => GetResourceString("ValueTooLarge");
        /// <summary>Expected depth to be zero at the end of the JSON payload. There is an open JSON object or array that should be closed.</summary>
        internal static string @ZeroDepthAtEnd => GetResourceString("ZeroDepthAtEnd");
        /// <summary>The JSON value could not be converted to {0}.</summary>
        internal static string @DeserializeUnableToConvertValue => GetResourceString("DeserializeUnableToConvertValue");
        /// <summary>The specified type {0} must derive from the specific value's type {1}.</summary>
        internal static string @DeserializeWrongType => GetResourceString("DeserializeWrongType");
        /// <summary>The value must be greater than zero.</summary>
        internal static string @SerializationInvalidBufferSize => GetResourceString("SerializationInvalidBufferSize");
        /// <summary>Cannot advance past the end of the buffer, which has a size of {0}.</summary>
        internal static string @BufferWriterAdvancedTooFar => GetResourceString("BufferWriterAdvancedTooFar");
        /// <summary>Cannot compare the value of a token type '{0}' to text.</summary>
        internal static string @InvalidComparison => GetResourceString("InvalidComparison");
        /// <summary>The JSON value is not in a supported DateTime format.</summary>
        internal static string @FormatDateTime => GetResourceString("FormatDateTime");
        /// <summary>The JSON value is not in a supported DateTimeOffset format.</summary>
        internal static string @FormatDateTimeOffset => GetResourceString("FormatDateTimeOffset");
        /// <summary>The JSON value is not in a supported Guid format.</summary>
        internal static string @FormatGuid => GetResourceString("FormatGuid");
        /// <summary>'{0}' is an invalid start of a property name or value, after a comment.</summary>
        internal static string @ExpectedStartOfPropertyOrValueAfterComment => GetResourceString("ExpectedStartOfPropertyOrValueAfterComment");
        /// <summary>The JSON array contains a trailing comma at the end which is not supported in this mode. Change the reader options.</summary>
        internal static string @TrailingCommaNotAllowedBeforeArrayEnd => GetResourceString("TrailingCommaNotAllowedBeforeArrayEnd");
        /// <summary>The JSON object contains a trailing comma at the end which is not supported in this mode. Change the reader options.</summary>
        internal static string @TrailingCommaNotAllowedBeforeObjectEnd => GetResourceString("TrailingCommaNotAllowedBeforeObjectEnd");
        /// <summary>Serializer options cannot be changed once serialization or deserialization has occurred.</summary>
        internal static string @SerializerOptionsImmutable => GetResourceString("SerializerOptionsImmutable");
        /// <summary>Stream is not writable.</summary>
        internal static string @StreamNotWritable => GetResourceString("StreamNotWritable");
        /// <summary>Cannot write a comment value which contains the end of comment delimiter.</summary>
        internal static string @CannotWriteCommentWithEmbeddedDelimiter => GetResourceString("CannotWriteCommentWithEmbeddedDelimiter");
        /// <summary>The JSON property name for '{0}.{1}' collides with another property.</summary>
        internal static string @SerializerPropertyNameConflict => GetResourceString("SerializerPropertyNameConflict");
        /// <summary>The JSON property name for '{0}.{1}' cannot be null.</summary>
        internal static string @SerializerPropertyNameNull => GetResourceString("SerializerPropertyNameNull");
        /// <summary>An item with the same property name '{0}' has already been added.</summary>
        internal static string @DeserializeDuplicateKey => GetResourceString("DeserializeDuplicateKey");
        /// <summary>The data extension property '{0}.{1}' does not match the required signature of IDictionary&lt;string, JsonElement&gt; or IDictionary&lt;string, object&gt;.</summary>
        internal static string @SerializationDataExtensionPropertyInvalid => GetResourceString("SerializationDataExtensionPropertyInvalid");
        /// <summary>The type '{0}' cannot have more than one property that has the attribute '{1}'.</summary>
        internal static string @SerializationDuplicateTypeAttribute => GetResourceString("SerializationDuplicateTypeAttribute");
        /// <summary>The type '{0}' is not supported.</summary>
        internal static string @SerializationNotSupportedType => GetResourceString("SerializationNotSupportedType");
        /// <summary>'{0}' is invalid after '/' at the beginning of the comment. Expected either '/' or '*'.</summary>
        internal static string @InvalidCharacterAtStartOfComment => GetResourceString("InvalidCharacterAtStartOfComment");
        /// <summary>Unexpected end of data while reading a comment.</summary>
        internal static string @UnexpectedEndOfDataWhileReadingComment => GetResourceString("UnexpectedEndOfDataWhileReadingComment");
        /// <summary>Cannot skip tokens on partial JSON. Either get the whole payload and create a Utf8JsonReader instance where isFinalBlock is true or call TrySkip.</summary>
        internal static string @CannotSkip => GetResourceString("CannotSkip");
        /// <summary>There is not enough data to read through the entire JSON array or object.</summary>
        internal static string @NotEnoughData => GetResourceString("NotEnoughData");
        /// <summary>Found invalid line or paragraph separator character while reading a comment.</summary>
        internal static string @UnexpectedEndOfLineSeparator => GetResourceString("UnexpectedEndOfLineSeparator");
        /// <summary>Comments cannot be stored when deserializing objects, only the Skip and Disallow comment handling modes are supported.</summary>
        internal static string @JsonSerializerDoesNotSupportComments => GetResourceString("JsonSerializerDoesNotSupportComments");
        /// <summary>Deserialization of types without a parameterless constructor, a singular parameterized constructor, or a parameterized constructor annotated with '{0}' is not supported. Type '{1}'.</summary>
        internal static string @DeserializeNoConstructor => GetResourceString("DeserializeNoConstructor");
        /// <summary>Deserialization of interface types is not supported. Type '{0}'.</summary>
        internal static string @DeserializePolymorphicInterface => GetResourceString("DeserializePolymorphicInterface");
        /// <summary>The converter specified on '{0}' is not compatible with the type '{1}'.</summary>
        internal static string @SerializationConverterOnAttributeNotCompatible => GetResourceString("SerializationConverterOnAttributeNotCompatible");
        /// <summary>The converter specified on '{0}' does not derive from JsonConverter or have a public parameterless constructor.</summary>
        internal static string @SerializationConverterOnAttributeInvalid => GetResourceString("SerializationConverterOnAttributeInvalid");
        /// <summary>The converter '{0}' read too much or not enough.</summary>
        internal static string @SerializationConverterRead => GetResourceString("SerializationConverterRead");
        /// <summary>The converter '{0}' is not compatible with the type '{1}'.</summary>
        internal static string @SerializationConverterNotCompatible => GetResourceString("SerializationConverterNotCompatible");
        /// <summary>The converter '{0}' wrote too much or not enough.</summary>
        internal static string @SerializationConverterWrite => GetResourceString("SerializationConverterWrite");
        /// <summary>The naming policy '{0}' cannot return null.</summary>
        internal static string @NamingPolicyReturnNull => GetResourceString("NamingPolicyReturnNull");
        /// <summary>The attribute '{0}' cannot exist more than once on '{1}'.</summary>
        internal static string @SerializationDuplicateAttribute => GetResourceString("SerializationDuplicateAttribute");
        /// <summary>The object or value could not be serialized.</summary>
        internal static string @SerializeUnableToSerialize => GetResourceString("SerializeUnableToSerialize");
        /// <summary>Either the JSON value is not in a supported format, or is out of bounds for an unsigned byte.</summary>
        internal static string @FormatByte => GetResourceString("FormatByte");
        /// <summary>Either the JSON value is not in a supported format, or is out of bounds for an Int16.</summary>
        internal static string @FormatInt16 => GetResourceString("FormatInt16");
        /// <summary>Either the JSON value is not in a supported format, or is out of bounds for a signed byte.</summary>
        internal static string @FormatSByte => GetResourceString("FormatSByte");
        /// <summary>Either the JSON value is not in a supported format, or is out of bounds for a UInt16.</summary>
        internal static string @FormatUInt16 => GetResourceString("FormatUInt16");
        /// <summary>A possible object cycle was detected. This can either be due to a cycle or if the object depth is larger than the maximum allowed depth of {0}. Consider using ReferenceHandler.Preserve on JsonSerializerOptions to support cycles.</summary>
        internal static string @SerializerCycleDetected => GetResourceString("SerializerCycleDetected");
        /// <summary>Expected a number, but instead got empty string.</summary>
        internal static string @EmptyStringToInitializeNumber => GetResourceString("EmptyStringToInitializeNumber");
        /// <summary>Property with name '{0}' already exists.</summary>
        internal static string @JsonObjectDuplicateKey => GetResourceString("JsonObjectDuplicateKey");
        /// <summary>Property with name '{0}' not found.</summary>
        internal static string @PropertyNotFound => GetResourceString("PropertyNotFound");
        /// <summary>Property with name '{0}' has a different type than expected.</summary>
        internal static string @PropertyTypeMismatch => GetResourceString("PropertyTypeMismatch");
        /// <summary>The DuplicatePropertyNameHandling enum must be set to one of the supported values.</summary>
        internal static string @InvalidDuplicatePropertyNameHandling => GetResourceString("InvalidDuplicatePropertyNameHandling");
        /// <summary>Invalid leading zero before '{0}'.</summary>
        internal static string @InvalidLeadingZeroInNumber => GetResourceString("InvalidLeadingZeroInNumber");
        /// <summary>The JSON array was modified during iteration.</summary>
        internal static string @ArrayModifiedDuringIteration => GetResourceString("ArrayModifiedDuringIteration");
        /// <summary>This JsonElement instance was not built from a JsonNode and is immutable.</summary>
        internal static string @NotNodeJsonElementParent => GetResourceString("NotNodeJsonElementParent");
        /// <summary>Cannot parse a JSON object containing metadata properties like '$id' into an array or immutable collection type. Type '{0}'.</summary>
        internal static string @MetadataCannotParsePreservedObjectToImmutable => GetResourceString("MetadataCannotParsePreservedObjectToImmutable");
        /// <summary>The value of the '$id' metadata property '{0}' conflicts with an existing identifier.</summary>
        internal static string @MetadataDuplicateIdFound => GetResourceString("MetadataDuplicateIdFound");
        /// <summary>The metadata property '$id' must be the first property in the JSON object.</summary>
        internal static string @MetadataIdIsNotFirstProperty => GetResourceString("MetadataIdIsNotFirstProperty");
        /// <summary>Invalid reference to value type '{0}'.</summary>
        internal static string @MetadataInvalidReferenceToValueType => GetResourceString("MetadataInvalidReferenceToValueType");
        /// <summary>The '$values' metadata property must be a JSON array. Current token type is '{0}'.</summary>
        internal static string @MetadataInvalidTokenAfterValues => GetResourceString("MetadataInvalidTokenAfterValues");
        /// <summary>Deserialization failed for one of these reasons:
        /// 1. {0}
        /// 2. {1}</summary>
        internal static string @MetadataPreservedArrayFailed => GetResourceString("MetadataPreservedArrayFailed");
        /// <summary>Invalid property '{0}' found within a JSON object that must only contain metadata properties and the nested JSON array to be preserved.</summary>
        internal static string @MetadataPreservedArrayInvalidProperty => GetResourceString("MetadataPreservedArrayInvalidProperty");
        /// <summary>One or more metadata properties, such as '$id' and '$values', were not found within a JSON object that must only contain metadata properties and the nested JSON array to be preserved.</summary>
        internal static string @MetadataPreservedArrayPropertyNotFound => GetResourceString("MetadataPreservedArrayPropertyNotFound");
        /// <summary>A JSON object that contains a '$ref' metadata property must not contain any other properties.</summary>
        internal static string @MetadataReferenceCannotContainOtherProperties => GetResourceString("MetadataReferenceCannotContainOtherProperties");
        /// <summary>Reference '{0}' not found.</summary>
        internal static string @MetadataReferenceNotFound => GetResourceString("MetadataReferenceNotFound");
        /// <summary>The '$id' and '$ref' metadata properties must be JSON strings. Current token type is '{0}'.</summary>
        internal static string @MetadataValueWasNotString => GetResourceString("MetadataValueWasNotString");
        /// <summary>Properties that start with '$' are not allowed on preserve mode, either escape the character or turn off preserve references by setting ReferenceHandler to null.</summary>
        internal static string @MetadataInvalidPropertyWithLeadingDollarSign => GetResourceString("MetadataInvalidPropertyWithLeadingDollarSign");
        /// <summary>Members '{0}' and '{1}' on type '{2}' cannot both bind with parameter '{3}' in constructor '{4}' on deserialization.</summary>
        internal static string @MultipleMembersBindWithConstructorParameter => GetResourceString("MultipleMembersBindWithConstructorParameter");
        /// <summary>Each parameter in constructor '{0}' on type '{1}' must bind to an object property or field on deserialization. Each parameter name must match with a property or field on the object. The match can be case-insensitive.</summary>
        internal static string @ConstructorParamIncompleteBinding => GetResourceString("ConstructorParamIncompleteBinding");
        /// <summary>The constructor '{0}' on type '{1}' may not have more than 64 parameters for deserialization.</summary>
        internal static string @ConstructorMaxOf64Parameters => GetResourceString("ConstructorMaxOf64Parameters");
        /// <summary>Reference metadata is not honored when deserializing types using parameterized constructors. See type '{0}'.</summary>
        internal static string @ObjectWithParameterizedCtorRefMetadataNotHonored => GetResourceString("ObjectWithParameterizedCtorRefMetadataNotHonored");
        /// <summary>The converter '{0}' cannot return a null value.</summary>
        internal static string @SerializerConverterFactoryReturnsNull => GetResourceString("SerializerConverterFactoryReturnsNull");
        /// <summary>The unsupported member type is located on type '{0}'.</summary>
        internal static string @SerializationNotSupportedParentType => GetResourceString("SerializationNotSupportedParentType");
        /// <summary>The extension data property '{0}' on type '{1}' cannot bind with a parameter in constructor '{2}'.</summary>
        internal static string @ExtensionDataCannotBindToCtorParam => GetResourceString("ExtensionDataCannotBindToCtorParam");
        /// <summary>Cannot allocate a buffer of size {0}.</summary>
        internal static string @BufferMaximumSizeExceeded => GetResourceString("BufferMaximumSizeExceeded");
        /// <summary>The type '{0}' is invalid for serialization or deserialization because it is a pointer type, is a ref struct, or contains generic parameters that have not been replaced by specific types.</summary>
        internal static string @CannotSerializeInvalidType => GetResourceString("CannotSerializeInvalidType");
        /// <summary>Serialization and deserialization of 'System.Type' instances are not supported and should be avoided since they can lead to security issues.</summary>
        internal static string @SerializeTypeInstanceNotSupported => GetResourceString("SerializeTypeInstanceNotSupported");
        /// <summary>The non-public property '{0}' on type '{1}' is annotated with 'JsonIncludeAttribute' which is invalid.</summary>
        internal static string @JsonIncludeOnNonPublicInvalid => GetResourceString("JsonIncludeOnNonPublicInvalid");
        /// <summary>The type '{0}' of property '{1}' on type '{2}' is invalid for serialization or deserialization because it is a pointer type, is a ref struct, or contains generic parameters that have not been replaced by specific types.</summary>
        internal static string @CannotSerializeInvalidMember => GetResourceString("CannotSerializeInvalidMember");
        /// <summary>The collection type '{0}' is abstract, an interface, or is read only, and could not be instantiated and populated.</summary>
        internal static string @CannotPopulateCollection => GetResourceString("CannotPopulateCollection");
        /// <summary>'IgnoreNullValues' and 'DefaultIgnoreCondition' cannot both be set to non-default values.</summary>
        internal static string @DefaultIgnoreConditionAlreadySpecified => GetResourceString("DefaultIgnoreConditionAlreadySpecified");
        /// <summary>The value cannot be 'JsonIgnoreCondition.Always'.</summary>
        internal static string @DefaultIgnoreConditionInvalid => GetResourceString("DefaultIgnoreConditionInvalid");
        /// <summary>The JSON value is not in a supported Boolean format.</summary>
        internal static string @FormatBoolean => GetResourceString("FormatBoolean");
        /// <summary>The type '{0}' is not a supported Dictionary key type.</summary>
        internal static string @DictionaryKeyTypeNotSupported => GetResourceString("DictionaryKeyTypeNotSupported");
        /// <summary>The ignore condition 'JsonIgnoreCondition.WhenWritingNull' is not valid on value-type member '{0}' on type '{1}'. Consider using 'JsonIgnoreCondition.WhenWritingDefault'.</summary>
        internal static string @IgnoreConditionOnValueTypeInvalid => GetResourceString("IgnoreConditionOnValueTypeInvalid");
        /// <summary>'JsonNumberHandlingAttribute' cannot be placed on a property, field, or type that is handled by a custom converter. See usage(s) of converter '{0}' on type '{1}'.</summary>
        internal static string @NumberHandlingConverterMustBeBuiltIn => GetResourceString("NumberHandlingConverterMustBeBuiltIn");
        /// <summary>When 'JsonNumberHandlingAttribute' is placed on a property or field, the property or field must be a number or a collection. See member '{0}' on type '{1}'.</summary>
        internal static string @NumberHandlingOnPropertyTypeMustBeNumberOrCollection => GetResourceString("NumberHandlingOnPropertyTypeMustBeNumberOrCollection");
        /// <summary>The converter '{0}' handles type '{1}' but is being asked to convert type '{2}'. Either create a separate converter for type '{2}' or change the converter's 'CanConvert' method to only return 'true' for a single type.</summary>
        internal static string @ConverterCanConvertNullableRedundant => GetResourceString("ConverterCanConvertNullableRedundant");
        /// <summary>The object with reference id '{0}' of type '{1}' cannot be assigned to the type '{2}'.</summary>
        internal static string @MetadataReferenceOfTypeCannotBeAssignedToType => GetResourceString("MetadataReferenceOfTypeCannotBeAssignedToType");
        /// <summary>Unable to cast object of type '{0}' to type '{1}'.</summary>
        internal static string @DeserializeUnableToAssignValue => GetResourceString("DeserializeUnableToAssignValue");
        /// <summary>Unable to assign 'null' to the property or field of type '{0}'.</summary>
        internal static string @DeserializeUnableToAssignNull => GetResourceString("DeserializeUnableToAssignNull");
        private static readonly bool s_usingResourceKeys;
        private static          bool UsingResourceKeys() => SR.s_usingResourceKeys;
        internal static string GetResourceString(string resourceKey, string defaultString = null)
        {
            if (SR.UsingResourceKeys())
                return defaultString ?? resourceKey;
            string str = (string) null;
            try
            {
                str = SR.ResourceManager.GetString(resourceKey);
            }
            catch (System.Resources.MissingManifestResourceException)
            {
            }
            return defaultString != null && resourceKey.Equals(str) ? defaultString : str;
        }
    }
}
