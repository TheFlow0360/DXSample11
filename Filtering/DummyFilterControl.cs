using System;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;

namespace DXSample11.Filtering
{
    public class DummyFilterControl : ComboBoxEdit
    {
        static DummyFilterControl()
        {
            DummyFilterEditSettings.RegisterEditSettings();
        }

        public DummyFilterControl()
        {
            _items = new ObservableCollection<DummyWrapper>();
            ItemsSource = _items;
            ApplyItemTemplateToSelectedItem = true;
            IsTextEditable = false;
            DisplayMember = nameof(DummyWrapper.DisplayText);
            ValueMember = nameof(DummyWrapper.Value);
        }

        private readonly ObservableCollection<DummyWrapper> _items;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            _items.Clear();
            
            // null item 
            _items.Add(new DummyWrapper(Int32.MinValue));

            // default items
            _items.Add(new DummyWrapper(1));
            _items.Add(new DummyWrapper(2));
        }

        protected override BaseEditSettings CreateEditorSettings()
        {
            return new DummyFilterEditSettings();
        }
    }
}