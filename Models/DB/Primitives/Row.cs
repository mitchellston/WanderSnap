namespace P4_Vacation_photos.Models.DB.Primitives
{
    /// <summary>Represents a row in a table</summary>
    public class Row
    {
        public List<Column> _columns { get; set; }

        public Row(List<Column> columns)
        {
            this._columns = columns;
        }
    }
}