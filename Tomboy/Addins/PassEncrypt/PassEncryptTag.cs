
// (C) 2018 Piotr Wiśniowski <contact.wisniowskipiotr@gmail.com>, LGPL 2.1 or later. 
using Gtk;
using System;
using System.Xml;
using Tomboy;
using System.IO;

namespace Tomboy.PassEncrypt
{
	class PassEncryptTag : DynamicNoteTag
	{
        #region fields
        /// <summary>
        /// PassEncryptTag used in xml for writing tag to file.
        /// </summary>
        public static string TagName  {get;} = "encpass";
        /// <summary>
        /// PassEncryptTag password atribute used in xml for writing tag to file.
        /// </summary>
        protected static string AtrName  {get;} = "password";
        #endregion

        #region methods
        /// <summary>
        /// Initializes PassEncryptTag with element_name in xml.
        /// </summary>
        /// <param name="element_name"> Tag name which will be written in xml. </param>
        public override void Initialize(string element_name)
        {
            base.Initialize(element_name);
            this.Editable = false;
            this.CanSplit = false;
            this.CanSpellCheck = false;
            this.CanGrow = false;
            this.CanActivate = true;
            this.Activated += PassEncryptTag_Activated;
            this.SaveType = TagSaveType.Content;
        }

        /// <summary>
        /// Sets password which will be saved in tag.
        /// </summary>
        /// <param name="encryptedPass"> Enctypred password which will be saved in tag. </param>
        public void SetPassword(string encryptedPass)
        {
            this.Attributes.Add(AtrName, encryptedPass);
        }
        
        /// <summary>
        /// Method used when user clicks on tag. Synchronusly starts DecryptPassAsync().
        /// </summary>
        /// <param name="tag"> NoteTag which was clicked. </param>
        /// <param name="editor"> NoteEditor in which NoteTag was clicked. </param>
        /// <param name="start"> TextIter pointiong to beginning of tag. </param>
        /// <param name="end"> TextIter pointiong to end of tag. </param>
        /// <returns> Returns true if password contained in tag is not empty string. </returns>
        protected bool PassEncryptTag_Activated(NoteTag tag, NoteEditor editor, Gtk.TextIter start, Gtk.TextIter end)
        {
            string encPass = (tag as PassEncryptTag).Attributes[AtrName];
            if (string.IsNullOrWhiteSpace(encPass))
                return false;
            DecryptPassAsync(encPass);
            return true;
        }

        /// <summary>
        /// Method used do decrypt password. 
        /// Shows new PasswordWindow and asks for decrypting passPhrase, then shows decrypted password on that window.
        /// </summary>
        /// <param name="encPass"> Encrypted password which needs to be decrypted by user entered passPhrase. </param>
        protected async void DecryptPassAsync(string encPass)
        {
            PasswordWindow passwordWindow = new PasswordWindow(false);
            passwordWindow.ShowAll();
            string passPhrase = await passwordWindow.GetPassword();
            if (string.IsNullOrEmpty(passPhrase))
                return;

            string decrypted = (new Encrypter()).Decrypt(encPass, passPhrase);
            passwordWindow.ShowNonEditableText(decrypted);
        }

        /// <summary>
        /// Saves PassEncryptTag in additional file near note just in case if tomboy deletes tag when addin is not found.
        /// </summary>
        public void SaveBackupCopy(string noteFilePath)
        {
            string path = noteFilePath + ".enctext";
            int i = 0;
            while( File.Exists(path) )
            {
                i++;
                path = noteFilePath + ".enctext" + i.ToString();
            }
            File.WriteAllText(path,this.Attributes[AtrName]);
        }
        #endregion
    }
}
