using System;
using System.Collections.Generic;

namespace DynamicConfig
{
    internal class ConfigKeyBuilder
    {
        internal const char PrefixSeparator = '-';
        internal const char OpenVersionRange = '(';
        internal const char CloseVersionRange = ')';

        private readonly PrefixBuilder _prefixBuilder;
        private readonly Version _version;

        private enum ParserState
        {
            None,
            PrefixSeparator,
            Prefix,
            BeginVersion,
            Version,
            EndVersion
        }

        public ConfigKeyBuilder(PrefixBuilder prefixBuilder, Version version)
        {
            _prefixBuilder = prefixBuilder;
            _version = version;
        }

        public bool TryCreate(string key, out ConfigKey configKey)
        {
            var prefixes = new List<string>();
            VersionRange versionRange = VersionRange.Empty;

            ParserState state = ParserState.None;

            int tokenBeginIndex = 0;
            int tokenLength = 0;


            for (int i = 0; i < key.Length; i++)
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
                        if (current != OpenVersionRange && current != PrefixSeparator)
                        {
                            tokenBeginIndex = i;
                            tokenLength++;
                            state = ParserState.Prefix;
                        }
                        else if (current == OpenVersionRange)
                        {
                            state = ParserState.BeginVersion;
                        }
                        break;
                    case ParserState.Prefix:
                        if (current != PrefixSeparator)
                        {
                            tokenLength++;
                        }
                        else
                        {
                            string prefix = key.Substring(tokenBeginIndex, tokenLength);
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
                                throw new DynamicConfigException(string.Format("Invalid key format: {0}", key));
                            }
                            if (!versionRange.InRange(_version))
                            {
                                configKey = null;
                                return false;
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
                    configKey = new ConfigKey(key, _prefixBuilder.Create(prefixes), versionRange);
                    return true;
                }
            }

            configKey = null;
            throw new DynamicConfigException(string.Format("Invalid key format: {0}", key));
        }
    }
}
