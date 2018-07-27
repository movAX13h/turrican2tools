using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using T2Tools.Turrican;

namespace T2Tools.Controls
{
    class TOCListItem : ListViewItem
    {
        private static List<TOCEntryType> handledTypes = new List<TOCEntryType>() {
                TOCEntryType.Text,
                TOCEntryType.Language,
                TOCEntryType.StaticSprite,
                TOCEntryType.Palette,
                TOCEntryType.AnimatedSprite,
                TOCEntryType.Map,
                TOCEntryType.Tileset,
                TOCEntryType.Music
        };

        public TOCEntry Entry { get; private set; }

        private ListViewSubItem typeItem;
        private ListViewSubItem sizeItem;
        private ListViewSubItem startItem;
        private ListViewSubItem endItem;

        public TOCListItem(TOCEntry entry)
        {
            Entry = entry;
            UseItemStyleForSubItems = false;
            
            typeItem = SubItems.Add(new ListViewSubItem());
            sizeItem = SubItems.Add(new ListViewSubItem());
            startItem = SubItems.Add(new ListViewSubItem());
            endItem = SubItems.Add(new ListViewSubItem());

            updateCaptions();
        }

        public void Update()
        {
            updateCaptions();
        }

        private void updateCaptions()
        {
            if (!handledTypes.Contains(Entry.Type)) typeItem.BackColor = Color.LightGray;
            ForeColor = Entry.Dirty ? Color.Red : Color.Black;

            Text = Entry.Name;
            typeItem.Text = Entry.TypeString;
            sizeItem.Text = Entry.Size.ToString();
            startItem.Text = Entry.PackedStart.ToString();
            endItem.Text = Entry.PackedEnd.ToString();
        }

        
    }
}
