﻿

#pragma checksum "C:\Users\k\Documents\GitHub\Courstore\CloudEDU\CloudEDU\Common\AppbarContent.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1A1F40519D6109EF2B0D26A998FC5A29"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CloudEDU.Common
{
    partial class AppbarContent : global::Windows.UI.Xaml.Controls.UserControl, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 30 "..\..\..\Common\AppbarContent.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.LogoutButton_Click_1;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 15 "..\..\..\Common\AppbarContent.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.CourstoreButton_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 17 "..\..\..\Common\AppbarContent.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.MyCoursesButton_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 19 "..\..\..\Common\AppbarContent.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.UploadCourseButton_Click;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 23 "..\..\..\Common\AppbarContent.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).KeyUp += this.Enter_Clicked;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


