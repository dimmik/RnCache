using System;
using System.Threading.Tasks;

namespace RnCache
{
    /// <summary>
    /// <p>Renewable cache.</p>
    /// <p>
    /// To be used when it is not that important which data snapshot to use - one currently in cache or one that is being updated
    /// Use case: you have editable dictionary item lists on UI
    /// When one user edits the list and saves - it is not essential for others to get this upate immediately, 
    /// but it is essential to get lists immediately instead of waiting for some background update (probably long).
    /// </p>
    /// <p>Generally it is supposed to be simple, easy to use and lightweight interface.</p>
    /// </summary>
    public interface IRnCache
    {
        /// <summary>
        /// Inits entity cache. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityId">Identifier of the cache</param>
        /// <param name="entityRetriever">Function to retrieve the entities. For example, some SQL-based data retrieving</param>
        /// <param name="lazy">if set to true, no real call to the entityRetriever will be performed at the point of initialization</param>
        void InitEntityCache<T>(string entityId, Func<T> entityRetriever, bool lazy = false);
        
        /// <summary>
        /// Returns cached data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityId">Identifier of the cache</param>
        /// <param name="updateIfNull">If set to true - synchronously call to entityRetriever (defined in InitEntityCache) 
        /// <u>if entity in cache is null</u> before returning</param>
        /// <param name="forceUpdate">if set to true - synchronously call to entityRetriever (defined in InitEntityCache) before returning</param>
        /// <returns>Cached entities. May be null</returns>
        T GetCachedEntity<T>(string entityId, bool updateIfNull = true, bool forceUpdate = false);

        /// <summary>
        /// Updates entity in cache. Default behavior - if currently cached entity is null - update it synchronously. If not - update in parallel task.
        /// </summary>
        /// <param name="entityId">Identifier of the cache</param>
        /// <param name="forceSyncUpdate">If set to true - update synchronously</param>
        /// <returns></returns>
        Task UpdateEntityCache(string entityId, bool forceSyncUpdate = false);
    }
}