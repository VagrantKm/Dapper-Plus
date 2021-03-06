﻿using System;
using System.Collections.Concurrent;
using Z.BulkOperations;

namespace Z.Dapper.Plus
{
    /// <summary>Manager for dapper plus.</summary>
    public class DapperPlusManager
    {
        public static bool ThrowErrorIfNotMapped { get; set; }
        /// <summary>The mapper cache.</summary>
        private static ConcurrentDictionary<string, DapperPlusEntityMapper> _mapperCache;

        /// <summary>The mapper factory.</summary>
        private static Action<DapperPlusEntityMapper> _mapperFactory;

        /// <summary>Static constructor.</summary>
        static DapperPlusManager()
        {
            _mapperCache = new ConcurrentDictionary<string, DapperPlusEntityMapper>();
        }

        /// <summary>Gets or sets the mapper cache.</summary>
        /// <value>The mapper cache.</value>
        public static ConcurrentDictionary<string, DapperPlusEntityMapper> MapperCache
        {
            get { return _mapperCache; }
            set { _mapperCache = value; }
        }

        /// <summary>Gets or sets the mapper factory.</summary>
        /// <value>The mapper factory.</value>
        public static Action<DapperPlusEntityMapper> MapperFactory
        {
            get { return _mapperFactory; }
            set { _mapperFactory = value; }
        }

        /// <summary>Adds a PRO License to activate all features.</summary>
        /// <param name="licenseName">The license name.</param>
        /// <param name="licenseKey">The license key.</param>
        public static void AddLicense(string licenseName, string licenseKey)
        {
            LicenseManager.AddLicense(licenseName, licenseKey);
        }

        /// <summary>Get or create a new mapper for the specified type and mapper key</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="mapperKey">The mapper key.</param>
        /// <returns>A DapperPlusConfig&lt;T&gt;</returns>
        public static DapperPlusEntityMapper<T> Entity<T>(string mapperKey = null)
        {
            var config = MapperCache.GetOrAdd(GetFullMapperKey<T>(mapperKey), s => new DapperPlusEntityMapper<T>());

            return (DapperPlusEntityMapper<T>) config;
        }

        /// <summary>Gets the full mapper key used in the mapper cache.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="mapperKey">The mapper key.</param>
        /// <returns>The full mapper key used in the mapped cache.</returns>
        public static string GetFullMapperKey<T>(string mapperKey)
        {
            return GetFullMapperKey(typeof (T), mapperKey);
        }

        public static string GetFullMapperKey(Type type, string mapperKey)
        {
            var fullKey = mapperKey ?? "zzz_null" + ";";

            if (type.FullName.StartsWith("System.Data.Entity.DynamicProxies"))
            {
                // Oops! Entity Framework Proxy Type
                var name = type.FullName.Remove(0, "System.Data.Entity.DynamicProxies.".Length).Split('_')[0];
                fullKey = "zzz_proxy;" + fullKey;
                fullKey += name;
            }
            else
            {
                fullKey += type.FullName;
            }
            
            return fullKey;
        }
    }
}