﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Threading;
using NHibernate.Cache;
using NUnit.Framework;

namespace NHibernate.Caches.Common.Tests
{
	using System.Threading.Tasks;
	public abstract partial class CacheFixture : Fixture
	{

		[Test]
		public async Task TestPutAsync()
		{
			const string key = "keyTestPut";
			const string value = "valuePut";

			var cache = GetDefaultCache();
			// Due to async version, it may already be there.
			await (cache.RemoveAsync(key, CancellationToken.None));

			Assert.That(await (cache.GetAsync(key, CancellationToken.None)), Is.Null, "cache returned an item we didn't add !?!");

			await (cache.PutAsync(key, value, CancellationToken.None));
			var item = await (cache.GetAsync(key, CancellationToken.None));
			Assert.That(item, Is.Not.Null, "Unable to retrieve cached item");
			Assert.That(item, Is.EqualTo(value), "didn't return the item we added");
		}

		[Test]
		public async Task TestRemoveAsync()
		{
			const string key = "keyTestRemove";
			const string value = "valueRemove";

			var cache = GetDefaultCache();

			// add the item
			await (cache.PutAsync(key, value, CancellationToken.None));

			// make sure it's there
			var item = await (cache.GetAsync(key, CancellationToken.None));
			Assert.That(item, Is.Not.Null, "item just added is not there");

			// remove it
			await (cache.RemoveAsync(key, CancellationToken.None));

			// make sure it's not there
			item = await (cache.GetAsync(key, CancellationToken.None));
			Assert.That(item, Is.Null, "item still exists in cache after remove");
		}

		[Test]
		public async Task TestClearAsync()
		{
			const string key = "keyTestClear";
			const string value = "valueClear";

			var cache = GetDefaultCache();

			// add the item
			await (cache.PutAsync(key, value, CancellationToken.None));

			// make sure it's there
			var item = await (cache.GetAsync(key, CancellationToken.None));
			Assert.That(item, Is.Not.Null, "couldn't find item in cache");

			// clear the cache
			await (cache.ClearAsync(CancellationToken.None));

			// make sure we don't get an item
			item = await (cache.GetAsync(key, CancellationToken.None));
			Assert.That(item, Is.Null, "item still exists in cache after clear");
		}

		[Test]
		public void TestNullKeyPutAsync()
		{
			var cache = GetDefaultCache();
			Assert.ThrowsAsync<ArgumentNullException>(() => cache.PutAsync(null, null, CancellationToken.None));
		}

		[Test]
		public void TestNullValuePutAsync()
		{
			var cache = GetDefaultCache();
			Assert.ThrowsAsync<ArgumentNullException>(() => cache.PutAsync("keyTestNullValuePut", null, CancellationToken.None));
		}

		[Test]
		public async Task TestNullKeyGetAsync()
		{
			var cache = GetDefaultCache();
			await (cache.PutAsync("keyTestNullKeyGet", "value", CancellationToken.None));
			var item = await (cache.GetAsync(null, CancellationToken.None));
			Assert.IsNull(item);
		}

		[Test]
		public void TestNullKeyRemoveAsync()
		{
			var cache = GetDefaultCache();
			Assert.ThrowsAsync<ArgumentNullException>(() => cache.RemoveAsync(null, CancellationToken.None));
		}

		[Test]
		public async Task TestRegionsAsync()
		{
			const string key = "keyTestRegions";
			var props = GetDefaultProperties();
			var cache1 = DefaultProvider.BuildCache("TestRegions1", props);
			var cache2 = DefaultProvider.BuildCache("TestRegions2", props);
			const string s1 = "test1";
			const string s2 = "test2";
			await (cache1.PutAsync(key, s1, CancellationToken.None));
			await (cache2.PutAsync(key, s2, CancellationToken.None));
			var get1 = await (cache1.GetAsync(key, CancellationToken.None));
			var get2 = await (cache2.GetAsync(key, CancellationToken.None));
			Assert.That(get1, Is.EqualTo(s1), "Unexpected value in cache1");
			Assert.That(get2, Is.EqualTo(s2), "Unexpected value in cache2");
		}

		[Test]
		public async Task TestNonEqualObjectsWithEqualHashCodeAndToStringAsync()
		{
			if (!SupportsDistinguishingKeysWithSameStringRepresentationAndHashcode)
				Assert.Ignore("Test not supported by provider");

			var obj1 = new SomeObject();
			var obj2 = new SomeObject();

			obj1.Id = 1;
			obj2.Id = 2;

			var cache = GetDefaultCache();

			Assert.That(await (cache.GetAsync(obj2, CancellationToken.None)), Is.Null, "Unexectedly found a cache entry for key obj2");
			await (cache.PutAsync(obj1, obj1, CancellationToken.None));
			Assert.That(await (cache.GetAsync(obj1, CancellationToken.None)), Is.EqualTo(obj1), "Unable to retrieved cached object for key obj1");
			Assert.That(await (cache.GetAsync(obj2, CancellationToken.None)), Is.Null, "Unexectedly found a cache entry for key obj2 after obj1 put");
		}

		[Test]
		public async Task TestObjectExpirationAsync([ValueSource(nameof(ExpirationSettingNames))] string expirationSetting)
		{
			if (!SupportsDefaultExpiration)
				Assert.Ignore("Provider does not support default expiration settings");

			const int expirySeconds = 3;
			const string key = "keyTestObjectExpiration";
			var obj = new SomeObject { Id = 2 };

			var cache = GetCacheForExpiration("TestObjectExpiration", expirationSetting, expirySeconds);

			Assert.That(await (cache.GetAsync(key, CancellationToken.None)), Is.Null, "Unexpected entry for key");
			await (cache.PutAsync(key, obj, CancellationToken.None));
			// Wait up to 1 sec before expiration
			await (Task.Delay(TimeSpan.FromSeconds(expirySeconds - 1)));
			Assert.That(await (cache.GetAsync(key, CancellationToken.None)), Is.Not.Null, "Missing entry for key");

			// Wait expiration
			await (Task.Delay(TimeSpan.FromSeconds(2)));

			// Check it expired
			Assert.That(await (cache.GetAsync(key, CancellationToken.None)), Is.Null, "Unexpected entry for key after expiration");
		}

		[Test]
		public async Task TestObjectExpirationAfterUpdateAsync([ValueSource(nameof(ExpirationSettingNames))] string expirationSetting)
		{
			if (!SupportsDefaultExpiration)
				Assert.Ignore("Provider does not support default expiration settings");

			const int expirySeconds = 3;
			const string key = "keyTestObjectExpirationAfterUpdate";
			var obj = new SomeObject { Id = 2 };

			var cache = GetCacheForExpiration("TestObjectExpirationAfterUpdate", expirationSetting, expirySeconds);

			Assert.That(await (cache.GetAsync(key, CancellationToken.None)), Is.Null, "Unexpected entry for key");
			await (cache.PutAsync(key, obj, CancellationToken.None));
			Assert.That(await (cache.GetAsync(key, CancellationToken.None)), Is.Not.Null, "Missing entry for key");

			// This forces an object update
			await (cache.PutAsync(key, obj, CancellationToken.None));
			Assert.That(await (cache.GetAsync(key, CancellationToken.None)), Is.Not.Null, "Missing entry for key after update");

			// Wait
			await (Task.Delay(TimeSpan.FromSeconds(expirySeconds + 2)));

			// Check it expired
			Assert.That(await (cache.GetAsync(key, CancellationToken.None)), Is.Null, "Unexpected entry for key after expiration");
		}

		[Test]
		public async Task TestSlidingExpirationAsync()
		{
			if (!SupportsSlidingExpiration)
				Assert.Ignore("Provider does not support sliding expiration settings");

			const int expirySeconds = 3;
			const string key = "keyTestSlidingExpiration";
			var obj = new SomeObject { Id = 2 };

			var props = GetPropertiesForExpiration(Cfg.Environment.CacheDefaultExpiration, expirySeconds.ToString());
			props["cache.use_sliding_expiration"] = "true";
			var cache = DefaultProvider.BuildCache("TestObjectExpiration", props);

			await (cache.PutAsync(key, obj, CancellationToken.None));
			// Wait up to 1 sec before expiration
			await (Task.Delay(TimeSpan.FromSeconds(expirySeconds - 1)));
			Assert.That(await (cache.GetAsync(key, CancellationToken.None)), Is.Not.Null, "Missing entry for key");

			// Wait up to 1 sec before expiration again
			await (Task.Delay(TimeSpan.FromSeconds(expirySeconds - 1)));
			Assert.That(await (cache.GetAsync(key, CancellationToken.None)), Is.Not.Null, "Missing entry for key after get and wait less than expiration");

			// Wait expiration
			await (Task.Delay(TimeSpan.FromSeconds(expirySeconds + 1)));

			// Check it expired
			Assert.That(await (cache.GetAsync(key, CancellationToken.None)), Is.Null, "Unexpected entry for key after expiration");
		}

		// NHCH-43
		[Test]
		public async Task TestUnicodeAsync()
		{
			var keyValues = new Dictionary<string, string>
			{
				{"길동", "valuePut1"},
				{"최고", "valuePut2"},
				{"新闻", "valuePut3"},
				{"地图", "valuePut4"},
				{"ます", "valuePut5"},
				{"プル", "valuePut6"}
			};
			var cache = GetDefaultCache();

			// Troubles may specifically arise with long keys, where a hashing algorithm may be used.
			var longKeyPrefix = new string('_', 1000);
			var longKeyValueSuffix = "Long";
			foreach (var kv in keyValues)
			{
				await (cache.PutAsync(kv.Key, kv.Value, CancellationToken.None));
				await (cache.PutAsync(longKeyPrefix + kv.Key, kv.Value + longKeyValueSuffix, CancellationToken.None));
			}

			foreach (var kv in keyValues)
			{
				var item = await (cache.GetAsync(kv.Key, CancellationToken.None));
				Assert.That(item, Is.EqualTo(kv.Value), $"Didn't return the item we added for key {kv.Key}");
				item = await (cache.GetAsync(longKeyPrefix + kv.Key, CancellationToken.None));
				Assert.That(item, Is.EqualTo(kv.Value + longKeyValueSuffix), $"Didn't return the item we added for long key {kv.Key}");
			}
		}
	}
}
