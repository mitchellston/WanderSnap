namespace WanderSnap.Models.DB.Primitives
{
    /// <summary>Represents the where clause of a database query</summary>
    public class Where : Column
    {

        public Compare _operator { get; set; }

        public Where(string column, Compare @operator, string value) : base(column, value)
        {
            this._operator = @operator;
        }
    }
}