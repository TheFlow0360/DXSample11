using System;

namespace DXSample11.Filtering
{
    public class DummyWrapper
    {
        public DummyWrapper(int? value = null)
        {
            Value = value;
        }

        public string DisplayText => Value == null ? "No Reference!" : $"Reference to {Value}";

        public int? Value { get; }

        private Boolean _checked;
        public Boolean Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                if (_checked == value)
                {
                    return;
                }
                _checked = value;
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler CheckedChanged;
    }
}