﻿

#pragma checksum "C:\Users\k\Documents\GitHub\Courstore\CloudEDU\CloudEDU\CourseStore\Coursing.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "36C03C3A7333C21764DF217E24C3DD2C"
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
    partial class Coursing : global::CloudEDU.Common.GlobalPage, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 38 "..\..\..\CourseStore\Coursing.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.UserProfileButton_Click;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 62 "..\..\..\CourseStore\Coursing.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.NotesText_Tapped;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 58 "..\..\..\CourseStore\Coursing.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.LecturesText_Tapped;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 54 "..\..\..\CourseStore\Coursing.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.HomeText_Tapped;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 48 "..\..\..\CourseStore\Coursing.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.BackButton_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


