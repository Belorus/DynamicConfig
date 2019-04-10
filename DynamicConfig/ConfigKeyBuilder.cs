using System.Collections.Generic;

namespace DynamicConfig
{
    internal class ConfigKeyBuilder
    {
        internal const char PrefixSeparator = '-';
        internal const char OpenVersionRange = '(';
        internal const char CloseVersionRange = ')';
        internal const char OpenSegmentation = '<';
        internal const char CloseSegmentation = '>';

        private readonly IPrefixBuilder _prefixBuilder;

        private enum ParserState
        {
            None,
            PrefixSeparator,
            Prefix,
            BeginVersion,
            Version,
            EndVersion,
            BeginSegmentation,
            Segmentation,
            EndSegmentation,
        }

        public ConfigKeyBuilder(IPrefixBuilder prefixBuilder)
        {
            _prefixBuilder = prefixBuilder;
        }

        public bool TryCreate(string key, out ConfigKey configKey)
        {
            var prefixes = new List<string>();
            var versionRange = VersionRange.Empty;
            var segmentation = Segment.Default;

            var state = ParserState.None;

            var tokenBeginIndex = 0;
            var tokenLength = 0;


            for (var i = 0; i < key.Length; i++)
            {
                char current = key[i];

                switch (state)
                {
                    case ParserState.None:
                        if (current == PrefixSeparator)
                        {
                            state = ParserState.PrefixSeparator;
                        }
                        break;
                    case ParserState.PrefixSeparator:
                        switch (current)
                        {
                            case OpenVersionRange:
                                state = ParserState.BeginVersion;
                                break;
                            case OpenSegmentation:
                                state = ParserState.BeginSegmentation;
                                break;
                            default:
                                if (current != PrefixSeparator)
                                {
                                    tokenBeginIndex = i;
                                    tokenLength++;
                                    state = ParserState.Prefix;
                                }

                                break;
                        }
                        break;
                    case ParserState.Prefix:
                        if (current != PrefixSeparator)
                        {
                            tokenLength++;
                        }
                        else
                        {
                            var prefix = key.Substring(tokenBeginIndex, tokenLength);
                            if (_prefixBuilder.Contains(prefix))
                            {
                                prefixes.Add(prefix);
                                tokenLength = 0;
                                state = ParserState.PrefixSeparator;
                            }
                            else
                            {
                                configKey = null;
                                return false;
                            }
                        }
                        break;
                    case ParserState.BeginVersion:
                        tokenBeginIndex = i;
                        tokenLength++;
                        state = ParserState.Version;
                        break;
                    case ParserState.Version:
                        if (current != CloseVersionRange)
                        {
                            tokenLength++;
                        }
                        else
                        {
                            state = ParserState.EndVersion;
                        }
                        break;
                    case ParserState.EndVersion:
                        if (current == PrefixSeparator)
                        {
                            if (!VersionRange.TryParse(key.Substring(tokenBeginIndex, tokenLength), out versionRange))
                            {
                                configKey = null;
                                throw new DynamicConfigException($"Invalid version range format: {key}");
                            }
                            tokenLength = 0;
                            state = ParserState.PrefixSeparator;
                        }
                        break;
                    case ParserState.BeginSegmentation:
                        tokenBeginIndex = i;
                        tokenLength++;
                        state = ParserState.Segmentation;
                        break;
                    case ParserState.Segmentation:
                        if (current != CloseSegmentation)
                        {
                            tokenLength++;
                        }
                        else
                        {
                            state = ParserState.EndSegmentation;
                        }
                        break;
                    case ParserState.EndSegmentation:
                        if (current == PrefixSeparator)
                        {
                            if (!Segment.TryParse(key.Substring(tokenBeginIndex, tokenLength), out segmentation))
                            {
                                configKey = null;
                                throw new DynamicConfigException($"Invalid segment format: {key}");
                            }
                            tokenLength = 0;
                            state = ParserState.PrefixSeparator;
                        }
                        break;
                }
            }

            if (state == ParserState.Prefix || state == ParserState.None)
            {
                key = key.Substring(tokenBeginIndex);

                if (!string.IsNullOrEmpty(key))
                {
                    configKey = new ConfigKey(key, _prefixBuilder.Create(prefixes), versionRange, segmentation);
                    return true;
                }
            }

            configKey = null;
            throw new DynamicConfigException($"Invalid key format: {key}");
        }
    }
}
