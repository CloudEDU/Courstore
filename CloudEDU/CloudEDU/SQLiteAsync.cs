//
// Copyright (c) 2012 Krueger Systems, Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SQLite
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SQLiteAsyncConnection
    {
        /// <summary>
        /// The _connection string
        /// </summary>
        SQLiteConnectionString _connectionString;
        /// <summary>
        /// The _open flags
        /// </summary>
        SQLiteOpenFlags _openFlags;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteAsyncConnection"/> class.
        /// </summary>
        /// <param name="databasePath">The database path.</param>
        /// <param name="storeDateTimeAsTicks">if set to <c>true</c> [store date time as ticks].</param>
        public SQLiteAsyncConnection(string databasePath, bool storeDateTimeAsTicks = false)
            : this(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, storeDateTimeAsTicks)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteAsyncConnection"/> class.
        /// </summary>
        /// <param name="databasePath">The database path.</param>
        /// <param name="openFlags">The open flags.</param>
        /// <param name="storeDateTimeAsTicks">if set to <c>true</c> [store date time as ticks].</param>
        public SQLiteAsyncConnection(string databasePath, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = false)
        {
            _openFlags = openFlags;
            _connectionString = new SQLiteConnectionString(databasePath, storeDateTimeAsTicks);
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        SQLiteConnectionWithLock GetConnection()
        {
            return SQLiteConnectionPool.Shared.GetConnection(_connectionString, _openFlags);
        }

        /// <summary>
        /// Creates the table asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<CreateTablesResult> CreateTableAsync<T>()
            where T : new()
        {
            return CreateTablesAsync(typeof(T));
        }

        /// <summary>
        /// Creates the tables asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <returns></returns>
        public Task<CreateTablesResult> CreateTablesAsync<T, T2>()
            where T : new()
            where T2 : new()
        {
            return CreateTablesAsync(typeof(T), typeof(T2));
        }

        /// <summary>
        /// Creates the tables asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <returns></returns>
        public Task<CreateTablesResult> CreateTablesAsync<T, T2, T3>()
            where T : new()
            where T2 : new()
            where T3 : new()
        {
            return CreateTablesAsync(typeof(T), typeof(T2), typeof(T3));
        }

        /// <summary>
        /// Creates the tables asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <returns></returns>
        public Task<CreateTablesResult> CreateTablesAsync<T, T2, T3, T4>()
            where T : new()
            where T2 : new()
            where T3 : new()
            where T4 : new()
        {
            return CreateTablesAsync(typeof(T), typeof(T2), typeof(T3), typeof(T4));
        }

        /// <summary>
        /// Creates the tables asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <returns></returns>
        public Task<CreateTablesResult> CreateTablesAsync<T, T2, T3, T4, T5>()
            where T : new()
            where T2 : new()
            where T3 : new()
            where T4 : new()
            where T5 : new()
        {
            return CreateTablesAsync(typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        }

        /// <summary>
        /// Creates the tables asynchronous.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns></returns>
        public Task<CreateTablesResult> CreateTablesAsync(params Type[] types)
        {
            return Task.Factory.StartNew(() =>
            {
                CreateTablesResult result = new CreateTablesResult();
                var conn = GetConnection();
                using (conn.Lock())
                {
                    foreach (Type type in types)
                    {
                        int aResult = conn.CreateTable(type);
                        result.Results[type] = aResult;
                    }
                }
                return result;
            });
        }

        /// <summary>
        /// Drops the table asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<int> DropTableAsync<T>()
            where T : new()
        {
            return Task.Factory.StartNew(() =>
            {
                var conn = GetConnection();
                using (conn.Lock())
                {
                    return conn.DropTable<T>();
                }
            });
        }

        /// <summary>
        /// Inserts the asynchronous.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Task<int> InsertAsync(object item)
        {
            return Task.Factory.StartNew(() =>
            {
                var conn = GetConnection();
                using (conn.Lock())
                {
                    return conn.Insert(item);
                }
            });
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Task<int> UpdateAsync(object item)
        {
            return Task.Factory.StartNew(() =>
            {
                var conn = GetConnection();
                using (conn.Lock())
                {
                    return conn.Update(item);
                }
            });
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Task<int> DeleteAsync(object item)
        {
            return Task.Factory.StartNew(() =>
            {
                var conn = GetConnection();
                using (conn.Lock())
                {
                    return conn.Delete(item);
                }
            });
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pk">The pk.</param>
        /// <returns></returns>
        public Task<T> GetAsync<T>(object pk)
            where T : new()
        {
            return Task.Factory.StartNew(() =>
            {
                var conn = GetConnection();
                using (conn.Lock())
                {
                    return conn.Get<T>(pk);
                }
            });
        }

        /// <summary>
        /// Finds the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pk">The pk.</param>
        /// <returns></returns>
        public Task<T> FindAsync<T>(object pk)
            where T : new()
        {
            return Task.Factory.StartNew(() =>
            {
                var conn = GetConnection();
                using (conn.Lock())
                {
                    return conn.Find<T>(pk);
                }
            });
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate)
            where T : new()
        {
            return Task.Factory.StartNew(() =>
            {
                var conn = GetConnection();
                using (conn.Lock())
                {
                    return conn.Get<T>(predicate);
                }
            });
        }

        /// <summary>
        /// Finds the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public Task<T> FindAsync<T>(Expression<Func<T, bool>> predicate)
            where T : new()
        {
            return Task.Factory.StartNew(() =>
            {
                var conn = GetConnection();
                using (conn.Lock())
                {
                    return conn.Find<T>(predicate);
                }
            });
        }

        /// <summary>
        /// Executes the asynchronous.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public Task<int> ExecuteAsync(string query, params object[] args)
        {
            return Task<int>.Factory.StartNew(() =>
            {
                var conn = GetConnection();
                using (conn.Lock())
                {
                    return conn.Execute(query, args);
                }
            });
        }

        /// <summary>
        /// Inserts all asynchronous.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public Task<int> InsertAllAsync(IEnumerable items)
        {
            return Task.Factory.StartNew(() =>
            {
                var conn = GetConnection();
                using (conn.Lock())
                {
                    return conn.InsertAll(items);
                }
            });
        }

        /// <summary>
        /// Updates all asynchronous.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public Task<int> UpdateAllAsync(IEnumerable items)
        {
            return Task.Factory.StartNew(() =>
            {
                var conn = GetConnection();
                using (conn.Lock())
                {
                    return conn.UpdateAll(items);
                }
            });
        }

        /// <summary>
        /// Runs the in transaction asynchronous.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        [Obsolete("Will cause a deadlock if any call in action ends up in a different thread. Use RunInTransactionAsync(Action<SQLiteConnection>) instead.")]
        public Task RunInTransactionAsync(Action<SQLiteAsyncConnection> action)
        {
            return Task.Factory.StartNew(() =>
            {
                var conn = this.GetConnection();
                using (conn.Lock())
                {
                    conn.BeginTransaction();
                    try
                    {
                        action(this);
                        conn.Commit();
                    }
                    catch (Exception)
                    {
                        conn.Rollback();
                        throw;
                    }
                }
            });
        }

        /// <summary>
        /// Runs the in transaction asynchronous.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public Task RunInTransactionAsync(Action<SQLiteConnection> action)
        {
            return Task.Factory.StartNew(() =>
            {
                var conn = this.GetConnection();
                using (conn.Lock())
                {
                    conn.BeginTransaction();
                    try
                    {
                        action(conn);
                        conn.Commit();
                    }
                    catch (Exception)
                    {
                        conn.Rollback();
                        throw;
                    }
                }
            });
        }

        /// <summary>
        /// Tables this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public AsyncTableQuery<T> Table<T>()
            where T : new()
        {
            //
            // This isn't async as the underlying connection doesn't go out to the database
            // until the query is performed. The Async methods are on the query iteself.
            //
            var conn = GetConnection();
            return new AsyncTableQuery<T>(conn.Table<T>());
        }

        /// <summary>
        /// Executes the scalar asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public Task<T> ExecuteScalarAsync<T>(string sql, params object[] args)
        {
            return Task<T>.Factory.StartNew(() =>
            {
                var conn = GetConnection();
                using (conn.Lock())
                {
                    var command = conn.CreateCommand(sql, args);
                    return command.ExecuteScalar<T>();
                }
            });
        }

        /// <summary>
        /// Queries the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public Task<List<T>> QueryAsync<T>(string sql, params object[] args)
            where T : new()
        {
            return Task<List<T>>.Factory.StartNew(() =>
            {
                var conn = GetConnection();
                using (conn.Lock())
                {
                    return conn.Query<T>(sql, args);
                }
            });
        }
    }

    //
    // TODO: Bind to AsyncConnection.GetConnection instead so that delayed
    // execution can still work after a Pool.Reset.
    //
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncTableQuery<T>
        where T : new()
    {
        /// <summary>
        /// The _inner query
        /// </summary>
        TableQuery<T> _innerQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTableQuery{T}"/> class.
        /// </summary>
        /// <param name="innerQuery">The inner query.</param>
        public AsyncTableQuery(TableQuery<T> innerQuery)
        {
            _innerQuery = innerQuery;
        }

        /// <summary>
        /// Wheres the specified pred expr.
        /// </summary>
        /// <param name="predExpr">The pred expr.</param>
        /// <returns></returns>
        public AsyncTableQuery<T> Where(Expression<Func<T, bool>> predExpr)
        {
            return new AsyncTableQuery<T>(_innerQuery.Where(predExpr));
        }

        /// <summary>
        /// Skips the specified n.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        public AsyncTableQuery<T> Skip(int n)
        {
            return new AsyncTableQuery<T>(_innerQuery.Skip(n));
        }

        /// <summary>
        /// Takes the specified n.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        public AsyncTableQuery<T> Take(int n)
        {
            return new AsyncTableQuery<T>(_innerQuery.Take(n));
        }

        /// <summary>
        /// Orders the by.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="orderExpr">The order expr.</param>
        /// <returns></returns>
        public AsyncTableQuery<T> OrderBy<U>(Expression<Func<T, U>> orderExpr)
        {
            return new AsyncTableQuery<T>(_innerQuery.OrderBy<U>(orderExpr));
        }

        /// <summary>
        /// Orders the by descending.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="orderExpr">The order expr.</param>
        /// <returns></returns>
        public AsyncTableQuery<T> OrderByDescending<U>(Expression<Func<T, U>> orderExpr)
        {
            return new AsyncTableQuery<T>(_innerQuery.OrderByDescending<U>(orderExpr));
        }

        /// <summary>
        /// To the list asynchronous.
        /// </summary>
        /// <returns></returns>
        public Task<List<T>> ToListAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                using (((SQLiteConnectionWithLock)_innerQuery.Connection).Lock())
                {
                    return _innerQuery.ToList();
                }
            });
        }

        /// <summary>
        /// Counts the asynchronous.
        /// </summary>
        /// <returns></returns>
        public Task<int> CountAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                using (((SQLiteConnectionWithLock)_innerQuery.Connection).Lock())
                {
                    return _innerQuery.Count();
                }
            });
        }

        /// <summary>
        /// Elements at asynchronous.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Task<T> ElementAtAsync(int index)
        {
            return Task.Factory.StartNew(() =>
            {
                using (((SQLiteConnectionWithLock)_innerQuery.Connection).Lock())
                {
                    return _innerQuery.ElementAt(index);
                }
            });
        }

        /// <summary>
        /// Firsts the asynchronous.
        /// </summary>
        /// <returns></returns>
        public Task<T> FirstAsync()
        {
            return Task<T>.Factory.StartNew(() =>
            {
                using (((SQLiteConnectionWithLock)_innerQuery.Connection).Lock())
                {
                    return _innerQuery.First();
                }
            });
        }

        /// <summary>
        /// Firsts the or default asynchronous.
        /// </summary>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync()
        {
            return Task<T>.Factory.StartNew(() =>
            {
                using (((SQLiteConnectionWithLock)_innerQuery.Connection).Lock())
                {
                    return _innerQuery.FirstOrDefault();
                }
            });
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CreateTablesResult
    {
        /// <summary>
        /// Gets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        public Dictionary<Type, int> Results { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTablesResult"/> class.
        /// </summary>
        internal CreateTablesResult()
        {
            this.Results = new Dictionary<Type, int>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    class SQLiteConnectionPool
    {
        /// <summary>
        /// 
        /// </summary>
        class Entry
        {
            /// <summary>
            /// Gets or sets the connection string.
            /// </summary>
            /// <value>
            /// The connection string.
            /// </value>
            public SQLiteConnectionString ConnectionString { get; private set; }
            /// <summary>
            /// Gets or sets the connection.
            /// </summary>
            /// <value>
            /// The connection.
            /// </value>
            public SQLiteConnectionWithLock Connection { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Entry"/> class.
            /// </summary>
            /// <param name="connectionString">The connection string.</param>
            /// <param name="openFlags">The open flags.</param>
            public Entry(SQLiteConnectionString connectionString, SQLiteOpenFlags openFlags)
            {
                ConnectionString = connectionString;
                Connection = new SQLiteConnectionWithLock(connectionString, openFlags);
            }

            /// <summary>
            /// Called when [application suspended].
            /// </summary>
            public void OnApplicationSuspended()
            {
                Connection.Dispose();
                Connection = null;
            }
        }

        /// <summary>
        /// The _entries
        /// </summary>
        readonly Dictionary<string, Entry> _entries = new Dictionary<string, Entry>();
        /// <summary>
        /// The _entries lock
        /// </summary>
        readonly object _entriesLock = new object();

        /// <summary>
        /// The _shared
        /// </summary>
        static readonly SQLiteConnectionPool _shared = new SQLiteConnectionPool();

        /// <summary>
        /// Gets the singleton instance of the connection tool.
        /// </summary>
        /// <value>
        /// The shared.
        /// </value>
        public static SQLiteConnectionPool Shared
        {
            get
            {
                return _shared;
            }
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="openFlags">The open flags.</param>
        /// <returns></returns>
        public SQLiteConnectionWithLock GetConnection(SQLiteConnectionString connectionString, SQLiteOpenFlags openFlags)
        {
            lock (_entriesLock)
            {
                Entry entry;
                string key = connectionString.ConnectionString;

                if (!_entries.TryGetValue(key, out entry))
                {
                    entry = new Entry(connectionString, openFlags);
                    _entries[key] = entry;
                }

                return entry.Connection;
            }
        }

        /// <summary>
        /// Closes all connections managed by this pool.
        /// </summary>
        public void Reset()
        {
            lock (_entriesLock)
            {
                foreach (var entry in _entries.Values)
                {
                    entry.OnApplicationSuspended();
                }
                _entries.Clear();
            }
        }

        /// <summary>
        /// Call this method when the application is suspended.
        /// </summary>
        /// <remarks>
        /// Behaviour here is to close any open connections.
        /// </remarks>
        public void ApplicationSuspended()
        {
            Reset();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    class SQLiteConnectionWithLock : SQLiteConnection
    {
        /// <summary>
        /// The _lock point
        /// </summary>
        readonly object _lockPoint = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteConnectionWithLock"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="openFlags">The open flags.</param>
        public SQLiteConnectionWithLock(SQLiteConnectionString connectionString, SQLiteOpenFlags openFlags)
            : base(connectionString.DatabasePath, openFlags, connectionString.StoreDateTimeAsTicks)
        {
        }

        /// <summary>
        /// Locks this instance.
        /// </summary>
        /// <returns></returns>
        public IDisposable Lock()
        {
            return new LockWrapper(_lockPoint);
        }

        /// <summary>
        /// 
        /// </summary>
        private class LockWrapper : IDisposable
        {
            /// <summary>
            /// The _lock point
            /// </summary>
            object _lockPoint;

            /// <summary>
            /// Initializes a new instance of the <see cref="LockWrapper"/> class.
            /// </summary>
            /// <param name="lockPoint">The lock point.</param>
            public LockWrapper(object lockPoint)
            {
                _lockPoint = lockPoint;
                Monitor.Enter(_lockPoint);
            }

            /// <summary>
            /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
            /// </summary>
            public void Dispose()
            {
                Monitor.Exit(_lockPoint);
            }
        }
    }
}

