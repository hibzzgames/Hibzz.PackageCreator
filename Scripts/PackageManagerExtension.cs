using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.PackageManager.UI.Internal;
using UnityEngine.UIElements;

using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Hibzz.PackageCreator
{
    internal class PackageManagerExtension : VisualElement, IPackageManagerExtension
    {
        private bool isInitialized = false;

        [InitializeOnLoadMethod]
        internal static void Initialize()
        {
            // Register this extension when the package reloads
            var extension = new PackageManagerExtension();
            PackageManagerExtensions.RegisterExtension(extension);
        }

        public VisualElement CreateExtensionUI()
        {
            // can't setup hook here yet because the extension hasn't been created yet
            isInitialized = false;
            return this;
        }

        public void OnPackageAddedOrUpdated(PackageInfo packageInfo) { }

        public void OnPackageRemoved(PackageInfo packageInfo) { }

        public void OnPackageSelectionChange(PackageInfo packageInfo) 
        {
            // already initialized, skip
            if(isInitialized) { return; }

            // create the hook
            SetupHook();

            // hook has been setup, update the flag
            isInitialized = true;
        }

        public void SetupHook()
        {
            // get the package manager root and from it a reference to the toolbar
            var root = this.GetRoot().Q<TemplateContainer>();
            var toolbar = root.Q<PackageManagerToolbar>();

            // add a create package option to the add the button
            var createPackageItem = toolbar.addMenu.AddBuiltInDropdownItem();
            createPackageItem.text = "Add new package...";
            createPackageItem.action = PackageCreatorWindow.OpenPackageCreator;
        }
    }

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
