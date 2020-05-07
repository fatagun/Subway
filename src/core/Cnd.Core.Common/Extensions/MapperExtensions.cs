using System;
using AutoMapper;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

namespace Cnd.Core.Common
{
    [DebuggerStepThrough]
    public static class MapperExtensions
    {
        private static readonly object Sync = new object();
        private static IConfigurationProvider _config;

        public static object GetPropertyValue(this MemberInfo member, object instance)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            return instance.GetType().GetProperty(member.Name)?.GetValue(instance);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return MapTo<TDestination>(source, destination);
        }

        public static TDestination MapTo<TDestination>(this object source) where TDestination : new()
        {
            return MapTo(source, new TDestination());
        }

        private static TDestination MapTo<TDestination>(object source, TDestination destination)
        {
            try
            {
                if (source == null)
                    return default(TDestination);
                if (destination == null)
                    return default(TDestination);
                var sourceType = GetType(source);
                var destinationType = GetType(destination);
                return GetResult(sourceType, destinationType, source, destination);
            }
            catch (AutoMapperMappingException ex)
            {
                return GetResult(GetType(ex.MemberMap.SourceType), GetType(ex.MemberMap.DestinationType), source, destination);
            }
        }

        private static Type GetType(object obj)
        {
            return GetType(obj.GetType());
        }

        private static TDestination GetResult<TDestination>(Type sourceType, Type destinationType, object source, TDestination destination)
        {
            if (Exists(sourceType, destinationType))
                return GetResult(source, destination);
            lock (Sync)
            {
                if (Exists(sourceType, destinationType))
                    return GetResult(source, destination);
                Init(sourceType, destinationType);
            }
            return GetResult(source, destination);
        }

        private static bool Exists(Type sourceType, Type destinationType)
        {
            return _config?.FindTypeMapFor(sourceType, destinationType) != null;
        }

        private static void Init(Type sourceType, Type destinationType)
        {
            if (_config == null)
            {
                _config = new MapperConfiguration(t => t.CreateMap(sourceType, destinationType));
                return;
            }
            var maps = _config.GetAllTypeMaps();
            _config = new MapperConfiguration(t => t.CreateMap(sourceType, destinationType));
            foreach (var map in maps)
                _config.RegisterTypeMap(map);
        }

        private static TDestination GetResult<TDestination>(object source, TDestination destination)
        {
            return new Mapper(_config).Map(source, destination);
        }

        public static List<TDestination> MapToList<TDestination>(this System.Collections.IEnumerable source)
        {
            return MapTo<List<TDestination>>(source);
        }
    }
}