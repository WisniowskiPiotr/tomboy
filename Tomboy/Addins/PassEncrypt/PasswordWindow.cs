
// (C) 2018 Piotr Wiśniowski <contact.wisniowskipiotr@gmail.com>, LGPL 2.1 or later.
using Gtk;
using Mono.Unix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tomboy.PassEncrypt
{
    class PasswordWindow : Window
    {
        #region fields
        /// <summary>
        /// First text field is used mainly for displaying start message and for entering password.
        /// </summary>
        public Entry FirstEntry = new Entry();
        /// <summary>
        /// Second text field is used mainly for displaying futher part of start message and for reentering password or showing non editable message (typicaly decoded password).
        /// </summary>
        public Entry SecondEntry = new Entry();
        /// <summary>
        /// Used as container for text fields.
        /// </summary>
        protected VBox Vbox = new VBox();
        /// <summary>
        /// Task when completed returns password entered by user. Used by await.
        /// </summary>
        protected TaskCompletionSource<string> EnteredPass = new TaskCompletionSource<string>();
        /// <summary>
        /// Flag indicating if user needs to reenter password for lowering probability of typos in password.
        /// </summary>
        protected bool WithConfirmation = true;
        /// <summary>
        /// Cannot enter password which length is less than MinimalPasswordLength.
        /// </summary>
        protected int MinimalPasswordLength = 6;
        #endregion

        #region methods
        /// <summary>
        /// Creates window which is used for password input and validation.
        /// </summary>
        /// <param name="withConfirmation"> If true window requires to reenter password for lowering probability of typos in password. </param>
        public PasswordWindow(bool withConfirmation) : base(WindowType.Toplevel)
        {
            WithConfirmation = withConfirmation;
            FirstEntry.GrabFocus();
            FirstEntry.Text = Catalog.GetString("Please enter password");
            FirstEntry.Changed += Entry_Changed;
            FirstEntry.Activated += Entry_Activated;
            FirstEntry.TooltipText = Catalog.GetString("Its best to use some technique for chosing password. Ex. first letters of phrase - \"Desmond has a barrow in the market place, Molly is the singer in a band.\" becomes \"Dhabitmp,Mitsiab.\"");
            Vbox.PackStart(FirstEntry, false, true, 2);

            if (withConfirmation)
            {
                SecondEntry.Text = Catalog.GetString("Reenter password");
                SecondEntry.Changed += Entry_Changed;
                SecondEntry.Activated += Entry_Activated;
            }
            else
            {
                SecondEntry.Text = Catalog.GetString("Decoded text");
                SecondEntry.Visibility = true;
                SecondEntry.IsEditable = false;
            }
            Vbox.PackStart(SecondEntry, false, true, 2);

            this.Add(Vbox);
            this.Title = Catalog.GetString("Enter Password");
            this.SetPosition(WindowPosition.Mouse);
            this.FocusOutEvent += PasswordWindow_FocusOutEvent;
            this.DeleteEvent += DeleteEvent_handler;
        }

        /// <summary>
        /// Occures when window is closed.
        /// </summary>
        protected void DeleteEvent_handler(object o, DeleteEventArgs args)
        {
            EnteredPass.TrySetResult(string.Empty);
            base.Dispose();
        }

        /// <summary>
        /// Occures when window is destroyed.
        /// </summary>
        public override void Destroy()
        {
            EnteredPass.TrySetResult(string.Empty);
            base.Destroy();
        }

        /// <summary>
        /// Occures when window is disposed.
        /// </summary>
        public override void Dispose()
        {
            EnteredPass.TrySetResult(string.Empty);
            base.Dispose();
        }

        /// <summary>
        /// Occures when window is not in focus. 
        /// When user leaves window in background this window is destroyed. 
        /// This prevents user for leaving window open in background and forgeting about it.
        /// </summary>
        protected void PasswordWindow_FocusOutEvent(object o, FocusOutEventArgs args)
        {
            this.Destroy();
        }

        /// <summary>
        /// Occures only once when input field text of window is changed.
        /// This method is run when user interaction starts.
        /// Texts in input fields are deleted and password chars are hidden from reading.
        /// </summary>
        protected void Entry_Changed(object sender, EventArgs e)
        {
            FirstEntry.Changed -= Entry_Changed;
            FirstEntry.Text = string.Empty;
            FirstEntry.Visibility = false;

            SecondEntry.Changed -= Entry_Changed;
            SecondEntry.Text = string.Empty;
            SecondEntry.Visibility = false;
        }

        /// <summary>
        /// Occures when input field is confirmed by pressing enter on keyboard.
        /// This method validates inputs and moves cursor where appripriate or sets entered text as reasult.
        /// </summary>
        protected void Entry_Activated(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstEntry.Text) || FirstEntry.Text.Length < MinimalPasswordLength)
            {
                FirstEntry.GrabFocus();
                return;
            }
            if (WithConfirmation && (string.IsNullOrWhiteSpace(SecondEntry.Text) || SecondEntry.Text.Trim() != FirstEntry.Text.Trim()))
            {
                SecondEntry.GrabFocus();
                return;
            }
            else
            {
                FirstEntry.IsEditable = false;
                SecondEntry.IsEditable = false;

                FirstEntry.Activated -= Entry_Activated;
                SecondEntry.Activated -= Entry_Activated;

                EnteredPass.SetResult(FirstEntry.Text.Trim());

                if (WithConfirmation)
                    this.Destroy();
                return;
            }
        }

        /// <summary>
        /// Show non editable text on second text field of window and optionaly copies this text to clipboard.
        /// </summary>
        /// <param name="text"> Text to be shown. </param>
        /// <param name="copy"> If true text is also copied to clipboard. </param>
        public void ShowNonEditableText(string text, bool copy = true)
        {
            SecondEntry.Text = text;
            SecondEntry.Visibility = true;
            SecondEntry.IsEditable = false;
            if (copy)
            {
                SecondEntry.GetClipboard(Gdk.Selection.Clipboard).Text = text;
            }
        }

        /// <summary>
        /// Method used for waiting for validated user input of password. This task returns password entered by user.
        /// </summary>
        /// <returns> This task returns password entered by user. </returns>
        public async Task<string> GetPassword()
        {
            return await this.EnteredPass.Task;
        }
        #endregion
    }
}
