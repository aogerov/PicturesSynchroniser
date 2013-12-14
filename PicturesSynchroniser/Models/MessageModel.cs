using System;
using System.Linq;
using System.Threading.Tasks;
using PicturesSynchroniser.Common;

namespace PicturesSynchroniser.Models
{
    public class MessageModel : BindableBase
    {
        private string text;
        private bool isAllReady;
        private bool isVisible;

        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.SetProperty(ref this.text, value);
            }
        }

        public bool IsAllReady
        {
            get
            {
                return this.isAllReady;
            }
            set
            {
                this.SetProperty(ref this.isAllReady, value);
            }
        }

        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }
            private set
            {
                this.SetProperty(ref this.isVisible, value);
            }
        }

        public async void SetTimeVisibility(int millisecondsDelay)
        {
            this.IsVisible = true;
            if (millisecondsDelay < 10000)
            {
                await Task.Delay(millisecondsDelay);
                this.IsVisible = false;
            }
        }
    }
}