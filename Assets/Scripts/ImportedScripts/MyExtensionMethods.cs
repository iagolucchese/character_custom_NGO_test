using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ImportedScripts
{
    public static class MyExtensionMethods
    {
        #region Animator Extensions
        /// <summary>
        /// Check if the animator has this parameter. Returns false if the animator is null.
        /// </summary>
        /// <param name="animator">This animator reference</param>
        /// <param name="paramName">Parameter string name</param>
        /// <returns></returns>
        public static bool HasParameter(this Animator animator, string paramName)
        {
            if (animator == null || animator.runtimeAnimatorController == null) return false;
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == paramName) return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the animator has this parameter. Returns false if the animator is null.
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="paramNameHash"></param>
        /// <returns></returns>
        public static bool HasParameter(this Animator animator, int paramNameHash)
        {
            if (animator == null || animator.runtimeAnimatorController == null) return false;
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.nameHash == paramNameHash) return true;
            }

            return false;
        }

        /// <summary>
        /// Safely checks if the animator is not null, and if it has the parameter passed. Returns true if the parameter was set successfully.
        /// </summary>
        public static bool SafeSetParameter(this Animator animator, int paramHash)
        {
            if (animator.HasParameter(paramHash) == false) return false;

            animator.SetTrigger(paramHash);
            return true;
        }

        public static bool SafeSetParameter(this Animator animator, int paramHash, bool value)
        {
            if (animator.HasParameter(paramHash) == false) return false;

            animator.SetBool(paramHash, value);
            return true;
        }

        public static bool SafeSetParameter(this Animator animator, int paramHash, int value)
        {
            if (animator.HasParameter(paramHash) == false) return false;

            animator.SetInteger(paramHash, value);
            return true;
        }

        public static bool SafeSetParameter(this Animator animator, int paramHash, float value)
        {
            if (animator.HasParameter(paramHash) == false) return false;

            animator.SetFloat(paramHash, value);
            return true;
        }
        #endregion
        
        #region Collection Extensions
        /// <summary>
        /// Checks that the collection is not null and not empty
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>True if not null and not empty</returns>
        public static bool IsValidAndNotEmpty(this ICollection collection)
        {
            return collection != null && collection.Count > 0;
        }
    
        /// <summary>
        /// Checks if the collection is null or empty
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>True if null or empty</returns>
        public static bool IsInvalidOrEmpty(this ICollection collection)
        {
            return collection == null || collection.Count <= 0;
        }
        #endregion
        
        #region IList Extensions
        /// <summary>
        /// Randomizes the list order.
        /// </summary>
        /// <param name="list"></param>
        public static void Shuffle(this IList list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        /// <summary>
        /// Gets a single random element from the list. Uses UnityEngine.Random.
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetRandom<T>(this IList<T> list) => list[Random.Range(0, list.Count)];

        /// <summary>
        /// Get a random elements from the list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="amount">Amount of random elements</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>List of random elements T</returns>
        public static List<T> GetRandom<T>(this IList<T> list, int amount)
        {
            if (amount < 2)
            {
                return new List<T> { list.GetRandom() };
            }

            List<T> results = new List<T>(list);
            results.Shuffle();

            int trueAmount = amount > results.Count ? results.Count : amount;
            if (results.Count > trueAmount)
                results.RemoveRange(trueAmount, results.Count - trueAmount);

            return results;
        }

        /// <summary>
        /// Get the last item on the list.
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetLast<T>(this IList<T> list) => list[list.Count - 1];

        /// <summary>
        /// Removes all null elements from the list.
        /// </summary>
        public static void Sanitize(this IList list)
        {
            if (list.IsInvalidOrEmpty()) return;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] != null) continue;
                list.RemoveAt(i);
            }
        }

        /// <summary>
        /// Returns element at 'index', using Mathf.Repeat to loop the index value.
        /// </summary>
        public static T ElementAtIndexRepeat<T>(this IList<T> list, int index)
        {
            if (list == null || list.Count <= 0) return default;
            int loopedIndex = (int)Mathf.Repeat(index, list.Count);
            return list[loopedIndex];
        }

        /// <summary>
        /// A null-safe version of the Count property. Returns zero if the list is null.
        /// </summary>
        public static int SafeCount<T>(this IList<T> list)
        {
            return list?.Count ?? 0;
        }

        /// <summary>
        /// Adds to list only if the element doesn't exist there yet.
        /// </summary>
        /// <returns>True if an element was added, false otherwise</returns>
        public static bool AddIfNew<T>(this IList<T> list, T newElement)
        {
            if (list == null) return false;
            if (list.Contains(newElement)) return false;

            list.Add(newElement);
            return true;
        }

        /// <summary>
        /// Adds to list all the elements that dont exist there yet.
        /// </summary>
        /// <returns>True if an element was added, false otherwise</returns>
        public static bool AddIfNew<T>(this IList<T> list, IList<T> newElements)
        {
            if (list == null) return false;
            if (newElements == null) return false;
            if (newElements.Count <= 0) return false;

            bool response = false;
            foreach (T newElement in newElements)
            {
                if (newElement == null) continue;
                response |= list.AddIfNew(newElement);
            }
            return response;
        }

        /// <summary>
        /// Moves an element of the list to a new position using its index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            var item = list[oldIndex];
            list.RemoveAt(oldIndex);
            if (newIndex > oldIndex) newIndex--;
            list.Insert(newIndex, item);
        }

        /// <summary>
        /// Finds the element index and then moves it to a new position.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <param name="newIndex"></param>
        public static void Move<T>(this IList<T> list, T item, int newIndex)
        {
            if (item != null)
            {
                var oldIndex = list.IndexOf(item);
                if (oldIndex > -1)
                {
                    list.RemoveAt(oldIndex);
                    if (newIndex > oldIndex) newIndex--;
                    list.Insert(newIndex, item);
                }
            }
        }
        #endregion
        
        #region Vector Extensions
        public static Vector3 ToVector3XZ(this Vector2 vector2)
        {
            return new Vector3(vector2.x, 0, vector2.y);
        }
        #endregion
        
        #region GameObject Extensions
        /// <summary>
        /// Shortcut, editor-only method set this Unity Object as dirty.
        /// <para>Does not require #IF UNITY_EDITOR, as it will do nothing on builds.</para>
        /// </summary>
        public static void SetAsDirty(this UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(obj);
#endif
        }
        
        /// <summary>
        /// In runtime, always returns false. In editor-time, returns true if the editor is in prefab mode.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsAPrefab(this GameObject obj)
        {
#if UNITY_EDITOR
#if UNITY_2020
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
#else
            UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#endif
            bool isValidPrefabStage = prefabStage != null && prefabStage.stageHandle.IsValid();
            bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(obj) == PrefabInstanceStatus.Connected;
            return isValidPrefabStage || !prefabConnected;
#else
            return false;
#endif
        }
        #endregion
    }
}