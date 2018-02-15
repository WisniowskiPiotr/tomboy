// Add an PassEncrypt item to the font styles menu. Just select text and press ctrl+e.
// (C) 2018 Piotr Wiśniowski <contact.wisniowskipiotr@gmail.com>, LGPL 2.1 or later.

using Mono.Unix;
using System;
using Gtk;
using Tomboy;

namespace Tomboy.PassEncrypt
{
	public class PassEncryptNoteAddin : NoteAddin
	{
        /// <summary>
        /// Initializes Addin and adds its tag to Note table.
        /// </summary>
		public override void Initialize ()
		{
            if (!Note.TagTable.IsDynamicTagRegistered(PassEncryptTag.TagName))
            {
                Note.TagTable.RegisterDynamicTag(PassEncryptTag.TagName, typeof(PassEncryptTag));
            }
        }

        /// <summary>
        /// Required by NoteAddin. Does nothing.
        /// </summary>
		public override void Shutdown ()
		{
		}

        /// <summary>
        /// Adds text menu item to newly opened note to be used by user.
        /// </summary>
		public override void OnNoteOpened ()
		{
			// Add here instead of in Initialize to avoid creating unopened
			// notes' windows/buffers.
			AddTextMenuItem (new PassEncryptMenuItem (this));
		}
	}
}
