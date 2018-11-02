using System.Windows.Forms;
using T2Tools.Formats;

namespace T2Tools.Controls
{
    class EntityListItem : ListViewItem
    {
        public EIBPoint Entry { get; private set; }

        private ListViewSubItem xItem;
        private ListViewSubItem yItem;
        private ListViewSubItem infoItem;

        public EntityListItem(EIBPoint entry)
        {
            Entry = entry;
            UseItemStyleForSubItems = false;
            
            xItem = SubItems.Add(new ListViewSubItem());
            yItem = SubItems.Add(new ListViewSubItem());
            infoItem = SubItems.Add(new ListViewSubItem());

            updateCaptions();
        }

        public void Update()
        {
            updateCaptions();
        }

        private void updateCaptions()
        {
            //ForeColor = Entry.Dirty ? Color.Red : Color.Black;

            Text = Entry.ID.ToString();
            xItem.Text = Entry.LocalX.ToString();
            yItem.Text = Entry.LocalY.ToString();
            infoItem.Text = "";
        }

        
    }
}
