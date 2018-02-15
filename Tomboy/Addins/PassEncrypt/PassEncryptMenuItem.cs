
// (C) 2018 Piotr Wiśniowski <contact.wisniowskipiotr@gmail.com>, LGPL 2.1 or later.
using Mono.Unix;
using System;
using Gtk;

namespace Tomboy.PassEncrypt
{
    class PassEncryptMenuItem : MenuItem
	{
        /// <summary>
        /// Addin instance from which this PassEncryptMenuItem was created.
        /// </summary>
		protected NoteAddin Addin;

        /// <summary>
        /// Adds PassEncryptMenuItem in text menu of note.
        /// </summary>
		public PassEncryptMenuItem (NoteAddin addin) : base ( Catalog.GetString ("Encrypt Text"))
		{
			((Label) Child).UseMarkup = true;

            Addin = addin;
			AddAccelerator ("activate", Addin.Window.AccelGroup,
				(uint) Gdk.Key.e, Gdk.ModifierType.ControlMask,
				Gtk.AccelFlags.Visible);

			ShowAll();
		}

        /// <summary>
        /// Occures when PassEncryptMenuItem option in text menu is cliced or crtl+e keys pressed.
        /// Gets selected text, shows pasword prompt and encrypts text in PassEncryptTag.
        /// </summary>
		protected async override void OnActivated ()
		{
            Logger.Info("PassEncryptMenuItem: Encrypting Text...");
            if ( Addin.Note.Buffer.HasSelection )
            {
                string selection = Addin.Note.Buffer.Selection.Trim();
                TextIter start, end;
                Addin.Note.Buffer.GetSelectionBounds(out start, out end);
                if (string.IsNullOrWhiteSpace(selection))
                    return;
                PasswordWindow passwordWindow = new PasswordWindow(true);
                passwordWindow.ShowAll();
                string passPhrase = await passwordWindow.GetPassword();
                if (string.IsNullOrWhiteSpace(passPhrase))
                    return;
                string encPassword = (new Encrypter()).Encrypt(selection, passPhrase);
                PassEncryptTag encPassTag = (PassEncryptTag) Addin.Note.TagTable.CreateDynamicTag(PassEncryptTag.TagName);
                encPassTag.SetPassword(encPassword);
                encPassTag.SaveBackupCopy(Addin.Note.FilePath);
                Gtk.TextTag[] tags = { encPassTag };
                Addin.Note.Buffer.DeleteInteractive(ref start, ref end, true);
                Addin.Note.Buffer.InsertWithTags(ref start, Catalog.GetString(" -Encrypted Text- "), tags);
            }
        }
    }
}
