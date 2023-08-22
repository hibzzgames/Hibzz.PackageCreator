using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.PackageManager.UI;
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
            if (isInitialized) { return; }

            // create the hook
            SetupHook();

            // hook has been setup, update the flag
            isInitialized = true;
        }

        public void SetupHook()
        {
            // let's get a reference to the UnityEditor.CoreModule assembly
            var editorCoreModuleAssembly = Assembly.Load("UnityEditor.CoreModule");
            if (editorCoreModuleAssembly == null) { return; }

            // from the assembly, we need to get the type
            var toolbarType = editorCoreModuleAssembly.GetType("UnityEditor.PackageManager.UI.Internal.PackageManagerToolbar");
            if (toolbarType == null) { return; }

            // get the package manager root and from it a reference to the toolbar
            var root = this.GetRoot().Q<TemplateContainer>();
            var toolbar = ReflectionTools.Call(typeof(UQueryExtensions), "Q",
                new Type[] { toolbarType },  // genericTypes
                root, null, new string[] { } // args
                );

            // make sure the toolbar is found
            if (toolbar == null) { return; }

            // add a create package option to the add the button
            var addMenu = ReflectionTools.Get(toolbar, "addMenu");
            var createPackageItem = ReflectionTools.Call(addMenu, "AddBuiltInDropdownItem");
            ReflectionTools.Set(createPackageItem, "text", "Add new package...");
            ReflectionTools.Set(createPackageItem, "action", new Action(PackageCreatorWindow.OpenPackageCreator));
        }
    }
}
