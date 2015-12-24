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
                if (state == ParserState.None && current == PrefixSeparator)
                {
                    state = ParserState.PrefixSeparator;
                    continue;
                }

                if (state == ParserState.PrefixSeparator && current != OpenVersionRange && current != PrefixSeparator)
                {
                    tokenBeginIndex = i;
                    tokenLength++;
                    state = ParserState.Prefix;
                    continue;
                }

                if (state == ParserState.Prefix && current != PrefixSeparator)
                {
                    tokenLength++;
                    continue;
                }

                if (state == ParserState.Prefix && current == PrefixSeparator)
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
                    continue;
                }

                if (state == ParserState.PrefixSeparator && current == OpenVersionRange)
                {
                    state = ParserState.BeginVersion;
                    continue;
                }

                if (state == ParserState.BeginVersion)
                {
                    tokenBeginIndex = i;
                    tokenLength++;
                    state = ParserState.Version;
                    continue;
                }

                if (state == ParserState.Version && current != CloseVersionRange)
                {
                    tokenLength++;
                    continue;
                }

                if (state == ParserState.Version && current == CloseVersionRange)
                {
                    state = ParserState.EndVersion;
                }

                if (state == ParserState.EndVersion && current == PrefixSeparator)
                {
                    versionRange = VersionRange.Parse(key.Substring(tokenBeginIndex, tokenLength));
                    if (!versionRange.InRange(_version))
                    {
                        configKey = null;
                        return false;
                    }
                    tokenLength = 0;
                    state = ParserState.PrefixSeparator;
                }
            }

            if (state != ParserState.Prefix && state != ParserState.None)
            {
                configKey = null;
                return false;
            }

            key = key.Substring(tokenBeginIndex);

            configKey = new ConfigKey(key, _prefixBuilder.Create(prefixes), versionRange);
            return true;
        }
    }
}
