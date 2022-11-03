using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace stoogebag_MonuMental.stoogebag.Extensions
{
    public static class ReflectionExtensions
    {
        private static BindingFlags publicFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
        private static BindingFlags privateFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;

        public static IEnumerable<FieldInfo> GetAllFieldsWithAttribute(this object obj, Type attributeType, bool includePrivate)
        {
            var flags = includePrivate ? privateFlags | publicFlags : publicFlags;
            return obj.GetType().GetFields(flags).Where(
                f => f.GetCustomAttributes(attributeType, false).Any());
        }
        
        public static IEnumerable<PropertyInfo> GetAllPropertiesWithAttribute(this object obj, Type attributeType, bool includePrivate)
        {
            var flags = includePrivate ? privateFlags | publicFlags : publicFlags;
            return obj.GetType().GetProperties(flags).Where(
                f => f.GetCustomAttributes(attributeType, false).Any());
        }
        public static IEnumerable<MemberInfo> GetAllMembersWithAttribute(this object obj, Type attributeType, bool includePrivate)
        {
            var flags = includePrivate ? privateFlags | publicFlags : publicFlags;
            return obj.GetType().GetMembers(flags).Where(
                f => f.GetCustomAttributes(attributeType, false).Any());
        }
        
    }
}