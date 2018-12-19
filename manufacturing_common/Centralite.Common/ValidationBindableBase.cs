using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Centralite.Common
{
    public class ValidationBindableBase : BindableBase, INotifyDataErrorInfo
    {
        #region INotifyDataErrorInfo
        private ErrorsContainer<ValidationResult> errorsContainer;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors
        {
            get { return this.errorsContainer.HasErrors; }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return this.errorsContainer.GetErrors(propertyName);
        }

        protected void RaiseErrorsChanged([CallerMemberName] string propertyName = null)
        {
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public ValidationBindableBase()
        {
            this.errorsContainer = new ErrorsContainer<ValidationResult>(this.RaiseErrorsChanged);
        }
        #endregion
    }
}
