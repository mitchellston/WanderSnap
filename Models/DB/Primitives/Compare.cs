namespace P4_Vacation_photos.Models.DB.Primitives
{
    public enum Compare
    {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual
    }

    /// <summary>Transforms the Compare enum to the right string for usage in a sql database</summary>
    public class TransformCompare
    {
        public string change(Compare compare)
        {
            switch (compare)
            {
                case Compare.Equal:
                    return "=";
                case Compare.NotEqual:
                    return "!=";
                case Compare.GreaterThan:
                    return ">";
                case Compare.LessThan:
                    return "<";
                case Compare.GreaterThanOrEqual:
                    return ">=";
                case Compare.LessThanOrEqual:
                    return "<=";
                default:
                    return "=";
            }
        }
    }
}
