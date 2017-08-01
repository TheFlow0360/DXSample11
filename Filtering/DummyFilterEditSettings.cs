using System;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;

namespace DXSample11.Filtering
{
    public class DummyFilterEditSettings : ComboBoxEditSettings
    {
        static DummyFilterEditSettings()
        {
            RegisterEditSettings();
        }

        public DummyFilterEditSettings()
        {
            _items = new ObservableCollection<DummyWrapper>();
            ItemsSource = _items;
            ApplyItemTemplateToSelectedItem = true;
            IsTextEditable = false;
            DisplayMember = nameof(DummyWrapper.DisplayText);
            ValueMember = nameof(DummyWrapper.Value);
        }

        private readonly ObservableCollection<DummyWrapper> _items;

        private static Boolean _registered;

        public static void RegisterEditSettings()
        {
            if (_registered)
            {
                return;
            }
            EditorSettingsProvider.Default.RegisterUserEditor2(
                typeof(DummyFilterControl),
                typeof(DummyFilterEditSettings),
                optimized => optimized ? new InplaceBaseEdit() : (IBaseEdit)Activator.CreateInstance(typeof(DummyFilterControl)),
                () => (BaseEditSettings)Activator.CreateInstance(typeof(DummyFilterEditSettings)));
            _registered = true;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            _items.Clear();

            // null item 
            _items.Add(new DummyWrapper());

            // default items
            _items.Add(new DummyWrapper(1));
            _items.Add(new DummyWrapper(2));
        }
    }
}