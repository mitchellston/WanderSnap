
// using P4_Vacation_photos.Models.DB.Providers;
// using System.Linq.Expressions;
// using P4_Vacation_photos.Models.DB.Primitives;
// namespace P4_Vacation_photos.Models.DB
// {
//     public interface IQuery
//     {
//         IQuerySelect<T> Select<T>(Expression<Func<T, object>>? selector = null) where T : class, new();
//         // void Insert<T>(Expression<Func<T, object>>? selector = null);
//         // void Update<T>(Expression<Func<T, object>>? table = null);
//         // void Delete<T>(Expression<Func<T, object>>? table = null);
//     }
//     public interface IQuerySelect<T> where T : class, new()
//     {
//         IQuerySelect<T> Where(string column, Compare @operator, string value);
//         IQuerySelect<T> Limit(int limit);
//         IQuerySelect<T> Offset(int offset);
//         IQuerySelect<T> OrderBy(Expression<Func<T, object>>? selector = null);
//         T[] Execute();
//     }


//     public class Client : IQuery
//     {
//         private IDbProvider _Provider;
//         private string[]? _EffectedColumns = null;
//         private string? _Table = null;
//         /// <summary>Constructor - Start the client for the database</summary>
//         public Client(IDbProvider provider)
//         {
//             this._Provider = provider;
//         }

//         public IQuerySelect<T> Select<T>(Expression<Func<T, object>>? selector = null) where T : class, new()
//         {
//             if (selector != null)
//             {
//                 _EffectedColumns = GetStrings<T, object>(selector).ToArray();
//             }
//             this._Table = typeof(T).Name;
//             return new SelectQuery<T>(this);
//         }
//         private class SelectQuery<T> : IQuerySelect<T> where T : class, new()
//         {
//             private Client _Client;
//             private int? _Limit = null;
//             private int? _Offset = null;
//             private Expression<Func<T, object>>? _OrderBy = null;

//             public SelectQuery(Client client)
//             {
//                 this._Client = client;
//             }
//             public IQuerySelect<T> Limit(int limit)
//             {
//                 this._Limit = limit;
//                 return this;
//             }
//             public IQuerySelect<T> Offset(int offset)
//             {
//                 this._Offset = offset;
//                 return this;
//             }
//             public IQuerySelect<T> OrderBy(Expression<Func<T, object>>? selector = null)
//             {
//                 this._OrderBy = selector;
//                 return this;
//             }
//             public IQuerySelect<T> Where(Expression<Func<T, object>>? selector = null)
//             {


//                 return this;
//             }
//             public T[] Execute()
//             {
//                 if (this._Client._Table == null) throw new System.Exception("Invalid table");
//                 if (this._Client._EffectedColumns == null) throw new System.Exception("Invalid columns");
//                 var data = this._Client._Provider.select(this._Client._Table, this._Client._EffectedColumns);
//                 var returnData = new T[data.Length];
//                 for (int i = 0; i < data.Length; i++)
//                 {
//                     returnData[i] = new T();
//                     var fields = returnData[i].GetType().GetFields();
//                     foreach (var field in fields)
//                     {
//                         field.SetValue(returnData[i], null);
//                     }
//                     foreach (var column in data[i]._columns)
//                     {
//                         var field = returnData[i].GetType().GetField(column._column);
//                         if (field != null)
//                         {
//                             field.SetValue(returnData[i], column._value);
//                         }
//                     }
//                 }
//                 return returnData;

//             }
//         }


//         private IEnumerable<string> GetStrings<T, E>(Expression<Func<T, E>> selector)
//         {
//             var memberExpression = selector.Body as NewExpression;
//             if (memberExpression == null)
//                 throw new ArgumentException("Invalid selector expression.");

//             var columnNames = new List<string>();
//             foreach (var argument in memberExpression.Arguments)
//             {
//                 var columnName = (argument as MemberExpression)?.Member.Name;
//                 if (columnName == null)
//                     throw new ArgumentException("Invalid selector expression.");

//                 columnNames.Add(columnName);
//             }

//             return columnNames;
//         }
//     }

// }