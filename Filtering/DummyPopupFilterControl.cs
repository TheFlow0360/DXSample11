using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Data.Filtering;

namespace DXSample11.Filtering
{
    public class DummyPopupFilterControl : Control
    {
        public DummyPopupFilterControl()
        {
            Items = new ObservableCollection<Object>();

        }

        public ObservableCollection<Object> Items { get; set; }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
            nameof(Filter),
            typeof(CriteriaOperator),
            typeof(DummyPopupFilterControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, FilterPropertyChangedCallback));

        private static void FilterPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DummyPopupFilterControl)d;
            control?.OnFilterPropertyChanged(e);
        }
        
        public CriteriaOperator Filter
        {
            get => (CriteriaOperator)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        public static readonly DependencyProperty SelectAllStateProperty = DependencyProperty.Register(
            nameof(SelectAllState),
            typeof(Boolean?),
            typeof(DummyPopupFilterControl),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectAllStatePropertyChangedCallback));

        private static void SelectAllStatePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DummyPopupFilterControl)d;
            control?.OnSelectAllPropertyChanged(e);
        }

        public Boolean? SelectAllState
        {
            get => (Boolean?)GetValue(SelectAllStateProperty);
            set => SetValue(SelectAllStateProperty, value);
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(DummyPopupFilterControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly DependencyProperty ColumnFieldNameProperty = DependencyProperty.Register(
            nameof(ColumnFieldName),
            typeof(String),
            typeof(DummyPopupFilterControl),
            new FrameworkPropertyMetadata(null));

        public String ColumnFieldName
        {
            get => (String)GetValue(ColumnFieldNameProperty);
            set => SetValue(ColumnFieldNameProperty, value);
        }

        private void OnSelectionChanged(Object sender, EventArgs eventArgs)
        {
            if (FilterChangeActive)
            {
                return;
            }

            FilterChangeActive = true;
            try
            {
                var selectedCount = Items.Cast<DummyWrapper>().Count(dummyWrapper => dummyWrapper.Checked);
                if (selectedCount > 0)
                {
                    CriteriaOperator filter = null;
                    var operandValues = Items.Cast<DummyWrapper>()
                        .Where(dummyWrapper => dummyWrapper.Checked)
                        .Where(dummyWrapper => dummyWrapper.Value != null)
                        .Select(dummyWrapper => new OperandValue(dummyWrapper.Value)).ToList();
                    if (operandValues.Any())
                    {
                        filter = new InOperator(new OperandProperty(ColumnFieldName), operandValues);
                    }
                    var nullValues = Items.Cast<DummyWrapper>()
                        .Where(dummyWrapper => dummyWrapper.Checked)
                        .Where(dummyWrapper => dummyWrapper.Value == null);
                    if (nullValues.Any())
                    {
                        var newFilter = new NullOperator(ColumnFieldName);
                        if (ReferenceEquals(filter, null))
                        {
                            filter = newFilter;
                        }
                        else
                        {
                            filter = new GroupOperator(GroupOperatorType.Or, filter, newFilter);
                        }
                    }
                    Filter = filter;
                    SelectAllState = selectedCount == Items.Count ? (Boolean?)true : null;
                }
                else
                {
                    Filter = null;
                    SelectAllState = false;
                }
            }
            finally
            {
                FilterChangeActive = false;
            }
        }

        private void OnFilterPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.FilterChangeActive)
            {
                return;
            }
            MatchCheckboxesToFilter();
        }

        private void MatchCheckboxesToFilter()
        {
            if (Items == null || Items.Count <= 0)
            {
                return;
            }
            if (Filter is InOperator)
            {
                var op = (InOperator)Filter;
                foreach (var dummyWrapper in this.Items.Cast<DummyWrapper>())
                {
                    dummyWrapper.Checked = op.Operands.Any(filter => ((OperandValue)filter).Value?.Equals(dummyWrapper.Value) ?? dummyWrapper.Value == null);
                }
            }
            else if (Filter is BinaryOperator)
            {
                var op = (BinaryOperator)Filter;
                if (op.OperatorType != BinaryOperatorType.Equal)
                {
                    return;
                }
                foreach (var dummyWrapper in this.Items.Cast<DummyWrapper>())
                {
                    dummyWrapper.Checked = ((OperandValue)op.RightOperand).Value?.Equals(dummyWrapper.Value) ?? dummyWrapper.Value == null;
                }
            }
        }

        private Boolean FilterChangeActive { get; set; }

        protected void OnSelectAllPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.FilterChangeActive)
            {
                return;
            }
            var newState = (Boolean?)e.NewValue;
            if (!newState.HasValue)
            {
                return;
            }
            this.SelectAll(newState.Value);
        }

        private void SelectAll(Boolean selectAll)
        {
            FilterChangeActive = true;
            foreach (var dummyWrapper in Items.Cast<DummyWrapper>())
            {
                dummyWrapper.Checked = selectAll;
            }
            FilterChangeActive = false;
            OnSelectionChanged(this, EventArgs.Empty);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Items.Clear();

            // null item 
            var wrapper = new DummyWrapper();
            wrapper.CheckedChanged += OnSelectionChanged;
            Items.Add(wrapper);

            // default items
            wrapper = new DummyWrapper(1);
            wrapper.CheckedChanged += OnSelectionChanged;
            Items.Add(wrapper);
            wrapper = new DummyWrapper(2);
            wrapper.CheckedChanged += OnSelectionChanged;
            Items.Add(wrapper);

            MatchCheckboxesToFilter();
        }
    }
}