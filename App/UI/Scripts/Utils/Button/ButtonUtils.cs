using UnityEngine.Events;
using UnityEngine.UI;

namespace OregoFramework.Util
{
    /// <summary>
    ///     <para>Button extensions.</para>
    /// </summary>
    public static class ButtonUtils
    {
        public static void AddListener(this Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
        }

        public static void RemoveListeners(this Button button)
        {
            button.onClick.RemoveAllListeners();
        }

        public static void RemoveListener(this Button button, UnityAction action)
        {
            button.onClick.RemoveListener(action);
        }
    }
}