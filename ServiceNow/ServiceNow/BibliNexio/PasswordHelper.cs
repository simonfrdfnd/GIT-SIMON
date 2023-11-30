namespace ServiceNow.BibliNexio
{
  using System;
  using System.Security;
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// Helper class to allow PasswordBox to bind password on viewmodel
  /// </summary>
  public static class PasswordHelper
  {
    /// <summary>
    /// the attach property
    /// </summary>
    public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached(
        "Attach",
        typeof(bool),
        typeof(PasswordHelper),
        new PropertyMetadata(false, Attach));

    /// <summary>
    /// the password property
    /// </summary>
    public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached(
        "Password",
        typeof(SecureString),
        typeof(PasswordHelper),
        new FrameworkPropertyMetadata(null, OnPasswordPropertyChanged));

    /// <summary>
    /// indicates if is updating
    /// </summary>
    private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached(
        "IsUpdating",
        typeof(bool),
        typeof(PasswordHelper));

    /// <summary>
    /// Gets the attach.
    /// </summary>
    /// <param name="dp">The dp.</param>
    /// <returns>Attach property</returns>
    public static bool GetAttach(DependencyObject dp)
    {
      return (bool)dp.GetValue(AttachProperty);
    }

    /// <summary>
    /// Gets the password.
    /// </summary>
    /// <param name="dp">The dp.</param>
    /// <returns>the password</returns>
    public static SecureString GetPassword(DependencyObject dp)
    {
      return (SecureString)dp.GetValue(PasswordProperty);
    }

    /// <summary>
    /// Sets the attach.
    /// </summary>
    /// <param name="dp">The dp.</param>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public static void SetAttach(DependencyObject dp, bool value)
    {
      dp.SetValue(AttachProperty, value);
    }

    /// <summary>
    /// Sets the password.
    /// </summary>
    /// <param name="dp">The dp.</param>
    /// <param name="value">The value.</param>
    public static void SetPassword(DependencyObject dp, SecureString value)
    {
      dp.SetValue(PasswordProperty, value);
    }

    /// <summary>
    /// Attaches the specified sender.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      PasswordBox passwordBox = sender as PasswordBox;

      if (passwordBox == null)
      {
        return;
      }

      if ((bool)e.OldValue)
      {
        passwordBox.PasswordChanged -= PasswordChanged;
      }

      if ((bool)e.NewValue)
      {
        passwordBox.PasswordChanged += PasswordChanged;
      }
    }

    /// <summary>
    /// Gets the is updating.
    /// </summary>
    /// <param name="dp">The dp.</param>
    /// <returns>A value indicating if this is updating</returns>
    private static bool GetIsUpdating(DependencyObject dp)
    {
      return (bool)dp.GetValue(IsUpdatingProperty);
    }

    /// <summary>
    /// Called when [password property changed].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      PasswordBox passwordBox = sender as PasswordBox;
      passwordBox.PasswordChanged -= PasswordChanged;

      if (!(bool)GetIsUpdating(passwordBox))
      {
        if (e.NewValue != null)
        {
          IntPtr unmanagedString = System.Runtime.InteropServices.Marshal.SecureStringToGlobalAllocUnicode((SecureString)e.NewValue);
          passwordBox.Password = System.Runtime.InteropServices.Marshal.PtrToStringUni(unmanagedString);
        }
        else
        {
          passwordBox.Password = string.Empty;
        }
      }

      passwordBox.PasswordChanged += PasswordChanged;
    }

    /// <summary>
    /// Passwords the changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    private static void PasswordChanged(object sender, RoutedEventArgs e)
    {
      PasswordBox passwordBox = sender as PasswordBox;
      SetIsUpdating(passwordBox, true);
      SetPassword(passwordBox, passwordBox.SecurePassword);
      SetIsUpdating(passwordBox, false);
    }

    /// <summary>
    /// Sets the is updating.
    /// </summary>
    /// <param name="dp">The dp.</param>
    /// <param name="value">if set to <c>true</c> [value].</param>
    private static void SetIsUpdating(DependencyObject dp, bool value)
    {
      dp.SetValue(IsUpdatingProperty, value);
    }
  }
}

