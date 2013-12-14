using System;
using PicturesSynchroniser.Common;

namespace PicturesSynchroniser.Models
{
    public class OptionModel : BindableBase
    {
        private string type;

        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
                this.SetProperty(ref this.type, value);
            }
        }
    }
}
