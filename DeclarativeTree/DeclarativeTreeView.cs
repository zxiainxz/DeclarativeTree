using System;
using System.Collections.Generic;
using AppKit;

namespace DeclarativeTree
{
    public class DeclarativeListColumn
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }

    public interface IDeclarativeListModel
    {
        DeclarativeListColumn[] Columns { get; }
        int Length { get; }
        object ObjectForColumn(int rowIndex, string columnId);
    }

    public class DeclarativeListModel : IDeclarativeListModel
    {
        Dictionary<string, List<object>> model = new Dictionary<string, List<object>>();

        public DeclarativeListColumn[] Columns { get; private set; }
        public int Length { get; private set; } = 0;

        public object ObjectForColumn(int rowIndex, string columnId)
        {
            if (rowIndex >= Length)
            {
                throw new IndexOutOfRangeException();
            }

            if (!model.TryGetValue(columnId, out var bucket))
            {
                // Need better exception?
                throw new IndexOutOfRangeException();
            }

            return bucket[rowIndex];
        }

        public DeclarativeListModel(params DeclarativeListColumn[] columns)
        {
            Columns = columns;
            foreach (var c in columns)
            {
                model[c.Id] = new List<object>();
            }
        }

        public void AddRow(params object[] rowObjects)
        {
            if (rowObjects.Length != Columns.Length)
            {
                throw new IndexOutOfRangeException();
            }

            int i = 0;
            foreach (var bucket in model.Values)
            {
                bucket.Add(rowObjects[i]);
                i++;
            }

            Length++;
        }

        public void RemoveRow(int rowIndex)
        {
            if (rowIndex >= Length)
            {
                throw new IndexOutOfRangeException();
            }

            foreach (var bucket in model.Values)
            {
                bucket.RemoveAt(rowIndex);
            }

            Length--;
        }

        public void InsertRow(int rowIndex, params object[] rowObjects)
        {
            if (rowIndex >= Length)
            {
                throw new IndexOutOfRangeException();
            }

            int i = 0;
            foreach (var bucket in model.Values)
            {
                bucket.Insert(rowIndex, rowObjects[i]);
                i++;
            }

            Length++;
        }

        public void Update(int rowIndex, params object[] rowObjects)
        {
            if (rowIndex >= Length)
            {
                throw new IndexOutOfRangeException();
            }

            int i = 0;
            foreach (var bucket in model.Values)
            {
                bucket.RemoveAt(rowIndex);
                bucket.Insert(rowIndex, rowObjects[i]);
                i++;
            }
        }

        public void UpdateColumn(int rowIndex, int columnIndex, object columnObject)
        {
            if (rowIndex >= Length)
            {
                throw new IndexOutOfRangeException();
            }

            if (columnIndex >= Columns.Length)
            {
                throw new IndexOutOfRangeException();
            }

            var bucket = model[Columns[columnIndex].Id];
            bucket.RemoveAt(rowIndex);
            bucket.Insert(rowIndex, columnObject);
        }
    }

    public class DeclarativeListView : NSTableView
	{
		public DeclarativeListView(IDeclarativeListModel model)
		{
            foreach (var c in model.Columns)
            {
                var tableColumn = new NSTableColumn(c.Id)
                {
                    Title = c.Title
                };
                AddColumn(tableColumn);
            }

            Delegate = new DLVDelegate(model);
            DataSource = new DLVDataSource(model);
		}
	}

    class DLVDelegate : NSTableViewDelegate
    {
        IDeclarativeListModel model;
        public DLVDelegate(IDeclarativeListModel model)
        {
            this.model = model;
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            var obj = model.ObjectForColumn((int)row, tableColumn.Identifier);
            if (obj == null)
            {
                throw new Exception();
            }

            var t = obj.GetType();
            if (t == typeof(string))
            {
                return NSTextField.CreateLabel((string)obj);
            }
            else if (t == typeof(bool))
            {
                var button = NSButton.CreateCheckbox(String.Empty, () =>
                {

                });

                button.State = (bool)obj ? NSCellStateValue.On : NSCellStateValue.Off;
                return button;
            }
            else if (t == typeof(NSImage))
            {
                return new NSImageView()
                {
                    Image = (NSImage)obj,
                };
            }
            else
            {
                throw new Exception();
            }

            return base.GetViewForItem(tableView, tableColumn, row);
        }
    }

    class DLVDataSource : NSTableViewDataSource
    {
        IDeclarativeListModel model;
        public DLVDataSource(IDeclarativeListModel model)
        {
            this.model = model;
        }

        public override nint GetRowCount(NSTableView tableView)
        {
            return model.Length;
        }
    }
}

