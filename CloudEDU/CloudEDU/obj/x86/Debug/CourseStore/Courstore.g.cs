﻿

#pragma checksum "C:\Users\k\Documents\GitHub\Courstore\CloudEDU\CloudEDU\CourseStore\Courstore.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2D0509DFE00B160559D6BDD6612C5D20"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CloudEDU.CourseStore
{
    partial class Courstore : global::CloudEDU.Common.GlobalPage, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 15 "..\..\..\CourseStore\Courstore.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).KeyUp += this.CourstoreGrid_KeyUp;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 23 "..\..\..\CourseStore\Courstore.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).KeyUp += this.SearchKey_KeyUp;
                 #line default
                 #line hidden
                #line 23 "..\..\..\CourseStore\Courstore.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).GotFocus += this.SearchBox_GotFocus;
                 #line default
                 #line hidden
                #line 23 "..\..\..\CourseStore\Courstore.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).LostFocus += this.SearchBox_LostFocus;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 108 "..\..\..\CourseStore\Courstore.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.UserProfileButton_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 63 "..\..\..\CourseStore\Courstore.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.Course_ItemClick;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 75 "..\..\..\CourseStore\Courstore.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.CategoryButton_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


