namespace P4_Vacation_photos.Models.DB.Primitives
{

    ///<summary>The columns and values of a table</summary>
    public class Column
    {
        public string _column { get; set; }
        public dynamic _value { get; set; }
        public Column(string column, dynamic value)
        {
            this._column = column;
            this._value = value;
        }
    }
}