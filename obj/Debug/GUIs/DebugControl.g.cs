﻿#pragma checksum "..\..\..\GUIs\DebugControl.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "5735B8CE75991604E3EDDACDFF1519D559F71F50"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using AiWPF.GUIs;
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


namespace AiWPF.GUIs {
    
    
    /// <summary>
    /// DebugControl
    /// </summary>
    public partial class DebugControl : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\GUIs\DebugControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtAnimationSpeed;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\GUIs\DebugControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnPlay;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\GUIs\DebugControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnBackward;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\GUIs\DebugControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnForward;
        
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
            System.Uri resourceLocater = new System.Uri("/AiWPF;component/guis/debugcontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\GUIs\DebugControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
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
            this.txtAnimationSpeed = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.btnPlay = ((System.Windows.Controls.Button)(target));
            
            #line 12 "..\..\..\GUIs\DebugControl.xaml"
            this.btnPlay.Click += new System.Windows.RoutedEventHandler(this.btnPlay_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnBackward = ((System.Windows.Controls.Button)(target));
            
            #line 17 "..\..\..\GUIs\DebugControl.xaml"
            this.btnBackward.Click += new System.Windows.RoutedEventHandler(this.btnBackward_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnForward = ((System.Windows.Controls.Button)(target));
            
            #line 22 "..\..\..\GUIs\DebugControl.xaml"
            this.btnForward.Click += new System.Windows.RoutedEventHandler(this.btnForward_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

