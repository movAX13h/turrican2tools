using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using T2Tools.Turrican;

namespace T2Tools.Controls
{
    class TOCListItem : ListViewItem
    {
        public TOCEntry Entry { get; private set; }

        private ListViewSubItem typeItem;
        private ListViewSubItem sizeItem;

        public TOCListItem(TOCEntry entry)
        {
            Entry = entry;
            UseItemStyleForSubItems = false;
            
            typeItem = SubItems.Add(new ListViewSubItem());
            sizeItem = SubItems.Add(new ListViewSubItem());

            //sizeItem.Font = new System.Drawing.Font("Consolas", 10);

            var handledTypes = new List<TOCEntryType>() {
                TOCEntryType.Text,
                TOCEntryType.Language
            };

            if (!handledTypes.Contains(Entry.Type))
            {
                typeItem.BackColor = Color.LightGray;
            }

            /*
            switch(entry.Type)
            {
                case DatFileEntry.DataType.Image:
                    BackColor = Color.FromArgb(255, 220, 255, 220);
                    break;
                case DatFileEntry.DataType.Map:
                    BackColor = Color.FromArgb(255, 220, 220, 255);
                    break;
                case DatFileEntry.DataType.Music:
                    BackColor = Color.FromArgb(255, 255, 255, 220);
                    break;
                case DatFileEntry.DataType.SoundFX:
                    BackColor = Color.FromArgb(255, 255, 255, 180);
                    break;
                case DatFileEntry.DataType.SpeakerSound:
                    BackColor = Color.FromArgb(255, 255, 255, 140);
                    break;
                case DatFileEntry.DataType.Sprite:
                    BackColor = Color.FromArgb(255, 180, 255, 180);
                    break;
                case DatFileEntry.DataType.Text:
                    BackColor = Color.FromArgb(255, 220, 220, 220);
                    break;
                case DatFileEntry.DataType.Unknown:
                default:
                    BackColor = Color.FromArgb(255, 255, 0, 0);
                    break;
            }
            */

            updateCaptions();
        }

        private void updateCaptions()
        {

            Text = Entry.Name;
            typeItem.Text = Entry.TypeString;
            sizeItem.Text = Entry.Size.ToString();
        }

        
    }
}
