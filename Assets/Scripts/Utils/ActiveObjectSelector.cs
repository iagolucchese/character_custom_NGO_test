using System;
using System.Collections.Generic;
using ImportedScripts;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CharacterCustomNGO.Utils
{
    public class ActiveObjectSelector : MonoBehaviour
    {
        private enum SelectorBehavior { RepeatsAfterMax, CapsAtMax, Randomized }
        private enum AmountActive { OnlySelected, SelectedAndBefore, SelectedAndAfter }
        
        [SerializeField] private int selectedObject;
        [SerializeField] private bool autoUpdateObject = true;
        [SerializeField] private bool invertBehavior;
        [SerializeField] private SelectorBehavior selectorBehavior = SelectorBehavior.RepeatsAfterMax;
        [SerializeField] private AmountActive amountActive = AmountActive.OnlySelected;
        [SerializeField] private List<GameObject> objectList;
        [Header("Unity Events")]
        [SerializeField] private UnityEvent<int> onObjectSelected;

        public GameObject CurrentSelectedObject => objectList[SelectedObject];
        public int ObjectCount => objectList.SafeCount();
        public int SelectedObject
        {
            get => selectedObject;
            set
            {
                if (objectList.IsInvalidOrEmpty())
                {
                    selectedObject = 0;
                    return;
                }
                
                selectedObject = selectorBehavior switch
                {
                    SelectorBehavior.RepeatsAfterMax => (int)Mathf.Repeat(value, ObjectCount),
                    SelectorBehavior.CapsAtMax => Mathf.Clamp(value, 0, ObjectCount - 1),
                    SelectorBehavior.Randomized => UnityEngine.Random.Range(0, ObjectCount),
                    _ => throw new ArgumentOutOfRangeException()
                };
                if (autoUpdateObject)
                    UpdateSelectedObject();
            }
        }
        
        private void OnValidate()
        {
            if (gameObject.IsAPrefab()) return;
            SelectObject(selectedObject);
        }

        public void SelectObject(int index)
        {
            SelectedObject = index;
            if (!autoUpdateObject)
                UpdateSelectedObject();
        }
        
        [Button]
        public void UpdateSelectedObject()
        {
            if (objectList.IsInvalidOrEmpty()) return;

            for (int index = 0; index < objectList.Count; index++)
            {
                GameObject obj = objectList[index];
                if (obj == null) continue;

                bool activateObject = amountActive switch
                {
                    AmountActive.OnlySelected => index == SelectedObject,
                    AmountActive.SelectedAndBefore => index <= SelectedObject,
                    AmountActive.SelectedAndAfter => index >= SelectedObject,
                    _ => index == SelectedObject
                };
                activateObject = invertBehavior ? !activateObject : activateObject;
                obj.SetActive(activateObject);
                obj.SetAsDirty();
            }
            onObjectSelected?.Invoke(selectedObject);
        }
        
        [Button]
        public void SelectNext()
        {
            SelectObject(SelectedObject + 1);
        }
        [Button]
        public void SelectPrevious()
        {
            SelectObject(SelectedObject - 1);
        }

        [Button]
        private void AddChildrenToList()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                objectList.Add(child);
            }
        }
    }
}