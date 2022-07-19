using System;

using AppKit;
using Foundation;

namespace DeclarativeTree
{
	public partial class ViewController : NSViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Do any additional setup after loading the view.
			var columns = new[]
			{
				new DeclarativeListColumn
				{
					Id = "col1",
					Title = "Column 1"
				},
				new DeclarativeListColumn
				{
					Id = "col2",
					Title = "Column 2"
				}
			};

			var model = new DeclarativeListModel(columns);
			model.AddRow("string 1", true);
			model.AddRow("row 2", false);

			var listView = new DeclarativeListView(model)
			{
				TranslatesAutoresizingMaskIntoConstraints = false
			};
			View.AddSubview(listView);
			View.LeadingAnchor.ConstraintEqualToAnchor(listView.LeadingAnchor).Active = true;
			View.TrailingAnchor.ConstraintEqualToAnchor(listView.TrailingAnchor).Active = true;
			View.TopAnchor.ConstraintEqualToAnchor(listView.TopAnchor).Active = true;
			View.BottomAnchor.ConstraintEqualToAnchor(listView.BottomAnchor).Active = true;
		}

		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
	}
}
