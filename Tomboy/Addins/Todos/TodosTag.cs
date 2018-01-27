
// (C) 2018 Piotr Wiśniowski <contact.wisniowskipiotr@gmail.com>, LGPL 2.1 or later. 
using Gtk;
using System;
using System.Xml;
using Tomboy;

namespace Tomboy.Todos
{
	class TodosTag : NoteTag
	{
        #region fields
        /// <summary>
        /// TodosTag used in xml.
        /// </summary>
        public static string TagName {get;} = "todo";
        /// <summary>
        /// Char used in text when todo is done.
        /// </summary>
        protected char DoneChar {get;} = 'a';
        /// <summary>
        /// Char used in text when todo is not done yet.
        /// </summary>
        protected char NotDoneChar {get;} = 'a';
        /// <summary>
        /// Date indicating deadline to wchich sth should be done.
        /// </summary>
        public DateTime Deadline {get; protected set;}
        /// <summary>
        /// Initial priority of tag.
        /// </summary>
        public int Priority {get;}
        /// <summary>
        /// Text of tag.
        /// </summary>
        public string Text {get; protected set;}
        /// <summary>
        /// Note in which tag is found.
        /// </summary>
        public TodosTagStatus Status {get; protected set;}
        public Note Note {get; protected set;}

        #endregion

        #region methods
        /// <summary>
        /// Initializes TodosTag with element_name in xml.
        /// </summary>
        /// <param name="element_name"> Tag name which will be written in xml. </param>
        public override void Initialize(string element_name)
        {
            base.Initialize(element_name);
            this.Editable = true;
            this.CanSplit = false;
            this.CanSpellCheck = true;
            this.CanGrow = false;
            this.CanActivate = false;
            this.Activated += TodosTag_Activated;
            this.SaveType = TagSaveType.Content;
        }

        /// <summary>
        /// Method used when user clicks on tag. Checks tag done or not.
        /// </summary>
        /// <param name="tag"> NoteTag which was clicked. </param>
        /// <param name="editor"> NoteEditor in which NoteTag was clicked. </param>
        /// <param name="start"> TextIter pointiong to beginning of tag. </param>
        /// <param name="end"> TextIter pointiong to end of tag. </param>
        /// <returns> Returns true if OK. </returns>
        protected bool TodosTag_Activated(NoteTag tag, NoteEditor editor, Gtk.TextIter start, Gtk.TextIter end)
        {
            if(this.Status != TodosTagStatus.Done)
            {
                this.Status = TodosTagStatus.Done;
            }
            else
            {
                this.Status = TodosTagStatus.NotDone;
            }
            SetCheckMark();
            return true;
        }

        /// <summary>
        /// Ensures that Text starts witch 2 chars with apriopriate status.
        /// </summary>
        protected void SetCheckMark()
        {
            if (this.Text.Length >= 2 && this.Text[1] == ' ')
            {
                bool test = false;
                foreach (TodosTagStatus s in Enum.GetValues(typeof(TodosTagStatus)))
                {
                    if (s.ToChar() == this.Text[0])
                    {
                        test=true;
                        break;
                    }
                }
                if (test)
                {
                    this.Text.Remove(0,2);
                }
            }
            this.Text = this.Status.ToChar() + ' ' + this.Text;
        }

        /// <summary>
        /// Sets tag parameters based on text.
        /// </summary>
        /// <param name="text"> Text containing todos parameters. </param>
        public void SetParameters(string text, Note note )
        {
            if (note != this.Note)
            {
                this.Note = note;
                this.Note.TagTable.TagChanged += TodosTag_Changed;
            }
            
        }

        void TodosTag_Changed (object sender, Gtk.TagChangedArgs args)
        {
            // TODO:
        }
        #endregion
    }
}
