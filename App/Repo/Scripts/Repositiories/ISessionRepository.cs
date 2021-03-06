using System.Collections;

namespace OregoFramework.App
{
    /// <summary>
    ///     <para>Listens user session events.</para>
    /// </summary>
    public interface ISessionRepository : IRepository
    {
        /// <summary>
        ///     <para>Loads user data asynchronously when starts a user session.</para>
        /// </summary>
        IEnumerator BeginSession();
        
        /// <summary>
        ///     <para>Unloads user data asynchronously when finishes a user session.</para>
        /// </summary>
        IEnumerator EndSession();
    }
}