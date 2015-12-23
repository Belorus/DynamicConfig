using System.Collections.Generic;

namespace DynamicConfig
{
    partial class ConfigKey
    {
        internal const char PrefixSeparator = '-';
        internal const char OpenVersionRange = '(';
        internal const char CloseVersionRange = ')';

        private enum ParserState
        {
            None,
            PrefixSeparator,
            Prefix,
            BeginVersion,
            Version,
            EndVersion
        }

        internal static ConfigKey Parse(string key, PrefixConfig prefixConfig)
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
                    if (prefixConfig.Contains(prefix))
                    {
                        prefixes.Add(prefix);
                        tokenLength = 0;
                        state = ParserState.PrefixSeparator;
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
                    tokenLength = 0;
                    state = ParserState.PrefixSeparator;
                }
            }

            key = key.Substring(tokenBeginIndex);

            return new ConfigKey(key, new Prefix(prefixConfig, prefixes), versionRange);
        }
    }
}
