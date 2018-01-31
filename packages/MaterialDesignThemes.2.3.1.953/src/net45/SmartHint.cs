﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MaterialDesignThemes.Wpf.Converters;

namespace MaterialDesignThemes.Wpf
{

    /// <summary>
    /// A control that implement placeholder behavior. Can work as a simple placeholder either as a floating hint, see <see cref="UseFloating"/> property.
    /// <para/>
    /// To set a target control you should set the HintProxy property. Use the <see cref="HintProxyFabricConverter.Instance"/> converter which converts a control into the IHintProxy interface.
    /// </summary>
    [TemplateVisualState(GroupName = ContentStatesGroupName, Name = ContentEmptyName)]
    [TemplateVisualState(GroupName = ContentStatesGroupName, Name = ContentNotEmptyName)]
    public class SmartHint : Control
    {        
        public const string ContentStatesGroupName = "ContentStates";
        public const string ContentEmptyName = "ContentEmpty";
        public const string ContentNotEmptyName = "ContentNotEmpty";

        private ContentControl _floatingHintPart = null;

        #region ManagedProperty

        public static readonly DependencyProperty HintProxyProperty = DependencyProperty.Register(
            nameof(HintProxy), typeof(IHintProxy), typeof(SmartHint), new PropertyMetadata(default(IHintProxy), HintProxyPropertyChangedCallback));

        public IHintProxy HintProxy
        {
            get { return (IHintProxy)GetValue(HintProxyProperty); }
            set { SetValue(HintProxyProperty, value); }
        }

        #endregion

        #region HintProperty

        public static readonly DependencyProperty HintProperty = DependencyProperty.Register(
            nameof(Hint), typeof(object), typeof(SmartHint), new PropertyMetadata(null));

        public object Hint
        {
            get { return GetValue(HintProperty); }
            set { SetValue(HintProperty, value); }
        }

        #endregion

        #region IsContentNullOrEmpty

        private static readonly DependencyPropertyKey IsContentNullOrEmptyPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "IsContentNullOrEmpty", typeof(bool), typeof(SmartHint),
                new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsContentNullOrEmptyProperty =
            IsContentNullOrEmptyPropertyKey.DependencyProperty;

        public bool IsContentNullOrEmpty
        {
            get { return (bool) GetValue(IsContentNullOrEmptyProperty); }
            private set { SetValue(IsContentNullOrEmptyPropertyKey, value); }
        }

        #endregion

        #region UseFloating

        public static readonly DependencyProperty UseFloatingProperty = DependencyProperty.Register(
            nameof(UseFloating), typeof(bool), typeof(SmartHint), new PropertyMetadata(false));

        public bool UseFloating
        {
            get { return (bool) GetValue(UseFloatingProperty); }
            set { SetValue(UseFloatingProperty, value); }
        }

        #endregion

        #region FloatingScale & FloatingOffset

        public static readonly DependencyProperty FloatingScaleProperty = DependencyProperty.Register(
            nameof(FloatingScale), typeof(double), typeof(SmartHint), new PropertyMetadata(.74));

        public double FloatingScale
        {
            get { return (double)GetValue(FloatingScaleProperty); }
            set { SetValue(FloatingScaleProperty, value); }
        }

        public static readonly DependencyProperty FloatingOffsetProperty = DependencyProperty.Register(
            nameof(FloatingOffset), typeof(Point), typeof(SmartHint), new PropertyMetadata(new Point(1, -16)));

        public Point FloatingOffset
        {
            get { return (Point)GetValue(FloatingOffsetProperty); }
            set { SetValue(FloatingOffsetProperty, value); }
        }        

        #endregion

        #region HintOpacity

        public static readonly DependencyProperty HintOpacityProperty = DependencyProperty.Register(
            nameof(HintOpacity), typeof(double), typeof(SmartHint), new PropertyMetadata(.46));

        public double HintOpacity
        {
            get { return (double) GetValue(HintOpacityProperty); }
            set { SetValue(HintOpacityProperty, value); }
        }

        #endregion

        static SmartHint()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SmartHint), new FrameworkPropertyMetadata(typeof(SmartHint)));
        }

        public SmartHint()
        {
            IsHitTestVisible = false;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        private static void HintProxyPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var smartHint = dependencyObject as SmartHint;
            if (smartHint == null) return;

            var hintProxy = dependencyPropertyChangedEventArgs.OldValue as IHintProxy;

            if (hintProxy != null)
            {
                hintProxy.IsVisibleChanged -= smartHint.OnHintProxyIsVisibleChanged;
                hintProxy.ContentChanged -= smartHint.OnHintProxyContentChanged;
                hintProxy.Loaded -= smartHint.OnHintProxyContentChanged;
                hintProxy.Dispose();
            }

            hintProxy = dependencyPropertyChangedEventArgs.NewValue as IHintProxy;
            if (hintProxy == null) return;

            hintProxy.IsVisibleChanged += smartHint.OnHintProxyIsVisibleChanged;
            hintProxy.ContentChanged += smartHint.OnHintProxyContentChanged;
            hintProxy.Loaded += smartHint.OnHintProxyContentChanged;
            smartHint.RefreshState(false);
        }

        protected virtual void OnHintProxyContentChanged(object sender, EventArgs e)
        {
            IsContentNullOrEmpty = HintProxy.IsEmpty();

            if (HintProxy.IsLoaded)
            {
                RefreshState(true);
            }
            else
            {
                HintProxy.Loaded += HintProxySetStateOnLoaded;
            }
        }

        private void HintProxySetStateOnLoaded(object sender, EventArgs e)
        {
            RefreshState(false);
            HintProxy.Loaded -= HintProxySetStateOnLoaded;
        }

        protected virtual void OnHintProxyIsVisibleChanged(object sender, EventArgs e)
        {
            RefreshState(false);
        }

        private void RefreshState(bool useTransitions)
        {
            var proxy = HintProxy;

            if (proxy == null) return;
            if (!proxy.IsVisible) return;
            
            var action = new Action(() =>
            {
                var isEmpty = proxy.IsEmpty();

                var state = isEmpty
                    ? ContentEmptyName
                    : ContentNotEmptyName;
            
                VisualStateManager.GoToState(this, state, useTransitions);                
            });

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                action();
            }
            else
            {
                Dispatcher.BeginInvoke(action);
            }
        }        
    }
}
