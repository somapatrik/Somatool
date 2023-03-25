using Android.Content;
using AndroidX.Core.Content;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veverka.Platforms.Android
{
    public class MyShellRenderer : ShellRenderer
    {
        public MyShellRenderer(Context context) : base(context)
        {

        }
        protected override IShellToolbarAppearanceTracker CreateToolbarAppearanceTracker()
        {
            return new MyToolbarAppearanceTracker();
        }
    }
    internal class MyToolbarAppearanceTracker : IShellToolbarAppearanceTracker
    {
        public void Dispose()
        {

        }

        public void ResetAppearance(AndroidX.AppCompat.Widget.Toolbar toolbar, IShellToolbarTracker toolbarTracker)
        {

        }
        public void SetAppearance(AndroidX.AppCompat.Widget.Toolbar toolbar, IShellToolbarTracker toolbarTracker, ShellAppearance appearance)
        {
            toolbar.OverflowIcon.SetTint(ContextCompat.GetColor(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity, Resource.Color.m3_ref_palette_white));
        }
    }
}
