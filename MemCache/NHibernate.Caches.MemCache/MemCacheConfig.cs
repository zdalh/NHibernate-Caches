#region License

//
//  MemCache - A cache provider for NHibernate using the .NET client
//  (http://sourceforge.net/projects/memcacheddotnet) for memcached,
//  which is located at http://www.danga.com/memcached/.
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// CLOVER:OFF
//

#endregion

using System;

namespace NHibernate.Caches.MemCache
{
	public class MemCacheConfig
	{
		public MemCacheConfig(string host, int port) : this(host, port, 1) {}

		public MemCacheConfig(string host, int port, int weight)
		{
			if (string.IsNullOrEmpty(host))
			{
				throw new ArgumentNullException("host");
			}
			if (port <= 0)
			{
				throw new ArgumentOutOfRangeException("port", "port must be greater than 0.");
			}
			Host = host;
			Port = port;
			Weight = weight;
		}

		public string Host { get; private set; }

		public int Port { get; private set; }

		public int Weight { get; private set; }
	}
}