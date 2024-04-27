using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ServiceNow.ViewModel
{
  public abstract class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
  {
    /// <summary>
    /// bool permetant de savoir si l'on doit effectuer la commande de validation
    /// </summary>
    private bool executeCommand = true;

    /// <summary>
    /// Permet de savoir si l'on a une erreur
    /// </summary>
    private bool hasError = false;

    /// <summary>
    /// Liste des commandes a executer lors de la validation
    /// </summary>
    private Dictionary<string, Func<string>> listCommand = new Dictionary<string, Func<string>>();

    /// <summary>
    /// Liste des erreurs obtenues
    /// </summary>
    private Dictionary<string, string> listError = new Dictionary<string, string>();

    /// <summary>
    /// Raised when a property on this object has a new value.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// first error
    /// </summary>
    public string Error
    {
      get
      {
        if (this.listError.Count > 0)
        {
          return this.listError.First().Value;
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Permet de savoir si l'on a une erreur
    /// </summary>
    public bool HasError
    {
      get
      {
        return this.hasError;
      }

      private set
      {
        if (this.hasError != value)
        {
          this.hasError = value;
          this.OnPropertyChanged("HasError");
        }
      }
    }

    /// <summary>
    /// Lance la validation sur un champs
    /// </summary>
    /// <param name="columnName">nom du champ a valider (propertyname)</param>
    /// <returns>retourne string.Empty si pas d'erreur, sinon le message a afficher</returns>
    public string this[string columnName]
    {
      get
      {
        this.listError.Remove(columnName);

        string error = string.Empty;
        if (this.executeCommand == false)
        {
          this.HasError = this.listError.Count > 0;
          return error;
        }

        if (this.listCommand.ContainsKey(columnName) == true)
        {
          Func<string> command = this.listCommand[columnName];
          if (command != null)
          {
            error = command();
          }

          if (error != string.Empty)
          {
            this.listError.Add(columnName, error);
          }
        }

        this.HasError = (this.listError.Count > 0) || (error != string.Empty);
        return error;
      }
    }

    /// <summary>
    /// Permet de savoir si l'on a une erreur
    /// </summary>
    /// <returns>retourne true si il y a une erreur</returns>
    public bool ContainsError()
    {
      bool result = false;

      PropertyInfo[] properties = GetType().GetProperties();
      this.OnPropertyChanged(string.Empty);

      foreach (PropertyInfo propertyInfo in properties)
      {
        string error = this[propertyInfo.Name];
        if ((propertyInfo.CanRead == true) && (propertyInfo.Name != "Item"))
        {
          try
          {
            object value = propertyInfo.GetValue(this, null);
            if ((value is ViewModelBase) && (value != this))
            {
              result |= ((ViewModelBase)value).ContainsError();
            }
          }
          catch
          {
          }
        }
      }

      result |= this.listError.Count > 0;
      this.HasError = result;

      return result;
    }

    /// <summary>
    /// Supprime toutes les erreurs a l'ecran
    /// </summary>
    public void DeleteAllErrors()
    {
      this.executeCommand = false;
      PropertyInfo[] properties = GetType().GetProperties();
      Dictionary<string, string> listParam = new Dictionary<string, string>();
      foreach (PropertyInfo propertyInfo in properties)
      {
        if ((propertyInfo.CanRead == true) && (propertyInfo.CanWrite == true))
        {
          this.OnPropertyChanged(propertyInfo.Name);
        }
      }

      this.HasError = false;
      this.executeCommand = true;
    }

    /// <summary>
    /// Warns the developer if this object does not have a public property with the specified name.
    /// This method does not exist in a Release build.
    /// </summary>
    /// <param name="propertyName">Name of the property</param>
    [Conditional("DEBUG")]
    [DebuggerStepThrough]
    public void VerifyPropertyName(string propertyName)
    {
      // verify that the property name matches a real, public, instance property on this object.
      if ((TypeDescriptor.GetProperties(this)[propertyName] == null) && (propertyName != string.Empty))
      {
        Debug.Fail("Invalid property name: " + propertyName);
      }
    }

    /// <summary>
    /// Ajout d'une validation
    /// </summary>
    /// <param name="propertyName">nom de la propriété</param>
    /// <param name="command">commande a executée</param>
    protected void AddValidation(string propertyName, Func<string> command)
    {
      if (this.listCommand.ContainsKey(propertyName) == false)
      {
        this.listCommand.Add(propertyName, command);
      }
      else
      {
        this.listCommand[propertyName] = command;
      }
    }

    /// <summary>
    /// Raises this object's PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The name of the property that has a new value.</param>
    public void OnPropertyChanged(string propertyName)
    {
      this.VerifyPropertyName(propertyName);
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}