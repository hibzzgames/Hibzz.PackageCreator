using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hibzz.PackageCreator
{
    internal static class VisualElementExtension
    {
        public static VisualElement GetRoot(this VisualElement element)
        {
            // given element is null (how?)
            if (element == null) { return null; }

            // recursively look at parents
            while (element.parent != null)
            {
                element = element.parent;
            }

            // the parent element is null
            return element;
        }
    }
}
