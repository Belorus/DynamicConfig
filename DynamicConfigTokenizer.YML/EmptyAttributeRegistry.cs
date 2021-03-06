﻿using System;
using System.Collections.Generic;
using System.Reflection;
using SharpYaml.Serialization;
using SharpYaml.Serialization.Descriptors;

namespace DynamicConfigTokenizer.YML
{
    internal class EmptyAttributeRegistry : IAttributeRegistry
    {
        private readonly List<Attribute> _empty = new List<Attribute>(0);
        public List<Attribute> GetAttributes(MemberInfo memberInfo, bool inherit = true)
        {
            return _empty;
        }

        public void Register(MemberInfo memberInfo, Attribute attribute)
        {
        }

        public Func<Attribute, Attribute> AttributeRemap { get; set; }
        public Action<ObjectDescriptor, List<IMemberDescriptor>> PrepareMembersCallback { get; set; }
    }
}
