﻿#pragma checksum "..\..\..\..\..\Control\Comm\Option\CommSerialConfig.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "8EA9E0572A1EA7BE4C08F63C3ECE917F"
//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.34209
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

using DY.WPF;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace DY.WPF {
    
    
    /// <summary>
    /// CommSerialConfig
    /// </summary>
    public partial class CommSerialConfig : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\..\..\Control\Comm\Option\CommSerialConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DY.WPF.CommSerialConfig UserControl;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\..\..\Control\Comm\Option\CommSerialConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DY.WPF.ComboBoxWithBar NCom;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\..\..\Control\Comm\Option\CommSerialConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DY.WPF.ComboBoxWithBar NBaud;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\..\..\Control\Comm\Option\CommSerialConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DY.WPF.ComboBoxWithBar NParity;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\..\Control\Comm\Option\CommSerialConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DY.WPF.ComboBoxWithBar NDataBit;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\..\..\Control\Comm\Option\CommSerialConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DY.WPF.ComboBoxWithBar NStopBit;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/DY.WPF;component/control/comm/option/commserialconfig.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Control\Comm\Option\CommSerialConfig.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.UserControl = ((DY.WPF.CommSerialConfig)(target));
            return;
            case 2:
            this.NCom = ((DY.WPF.ComboBoxWithBar)(target));
            return;
            case 3:
            this.NBaud = ((DY.WPF.ComboBoxWithBar)(target));
            return;
            case 4:
            this.NParity = ((DY.WPF.ComboBoxWithBar)(target));
            return;
            case 5:
            this.NDataBit = ((DY.WPF.ComboBoxWithBar)(target));
            return;
            case 6:
            this.NStopBit = ((DY.WPF.ComboBoxWithBar)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

