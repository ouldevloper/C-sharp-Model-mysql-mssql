using System;
using System.Data;
using MySql.Data;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OlDeveloper.Data;
using OlDeveloper.Data.Common;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;

namespace OlDeveloper
{
        /// <summary>
        /// author : AB_OLDEVELOPER
        /// </summary>
        namespace Data
        {
            namespace Common
            {
                public enum OpType
                {
                    LessThan,
                    GreatThan,
                    Equals,
                    lessOrEquals,
                    GreatOrEquals,
                    NotEquals
                }
                public enum ConditionName
                {
                    WHERE,
                    HAVING
                }
                public enum DBType
                {
                    BigInt,
                    Binary,
                    Bit,
                    Char,
                    DateTime,
                    Decimal,
                    Float,
                    Image,
                    Int,
                    Money,
                    NChar,
                    NText,
                    NVarChar,
                    Real,
                    UniqueIdentifier,
                    SmallDateTime,
                    SmallInt,
                    SmallMoney,
                    Text,
                    Timestamp,
                    TinyInt,
                    VarBinary,
                    VarChar,
                    Variant,
                    Xml,
                    Udt,
                    Structured,
                    Date,
                    Time,
                    DateTime2,
                    DateTimeOffset,
                }
                public enum RelationName
                {
                    AND,
                    OR
                }
                public enum GroupType
                {
                    ASC, DESC
                }
                public enum JoinType
                {
                    INNER,
                    LEFT,
                    RIGHT,
                    FULL
                }
                public enum Constarint
                {
                    primarykey,
                    foreignkey,
                    defualt,
                    notNull,
                    _Null,
                    check
                }
                public enum Providers
                {
                    SQLSERVER,
                    MYSQL
                }
                public class ForeingKey
                {
                    public string referenceTableName;
                    public string columnName;
                    public ForeingKey(string a,string b)
                    {
                         referenceTableName=a;
                         columnName=b;
                    }
                }
                public class check
                {
                    public List<column> col;
                    public OpType[] op;
                    public RelationName[] relation;
                    
                    public check(List<column> col, OpType[] op,RelationName[] relation)
                    {
                         this.col=col;
                         this.op = op;
                         this.relation = relation;
                    }
                }
                public class Table
                {
                    public string colNam;
                    public DBType coltyp;
                    public int colsiz;
                    public bool notnull;
                    public bool auto_increment;
                    public bool primary_key;
                    public bool unique;
                    public string Defaulte;
                    public ForeingKey Foreing_Key;
                    public check Check;
                    public Table(string colNam, DBType coltyp, int colsize, bool notnull,
                        bool autoincrement, bool primarykey, string Defaulte, ForeingKey Foreing_Key,bool unique,check Check)
                    {
                        this.colNam = colNam;
                        this.coltyp = coltyp;
                        this.colsiz = colsize;
                        this.notnull = notnull;
                        this.auto_increment = autoincrement;
                        this.primary_key = primarykey;
                        this.Defaulte = Defaulte;
                        this.Foreing_Key = Foreing_Key;
                        this.unique = unique;
                        this.Check = Check;
                    }
                }
                public class column
                {
                    private string columnName;
                    private object columnValue;
                    private DBType columntype;
                    public List<column> columns = new List<column>();
                    public object ColumnValue
                    {
                        get
                        {
                            return this.ColumnValue;
                        }
                    }
                    public string ColumnName
                    {
                        get
                        {
                            return this.ColumnName;
                        }
                    }
                    public DBType ColumnType
                    {
                        get
                        {
                            return this.ColumnType;
                        }
                    }
                    public column() { }
                    public column(string columnName)
                    {
                        this.columnName = columnName;
                    }
                    public column(object columnData)
                    {
                        this.columnValue = columnData;
                    }
                    public column(string columnName, object columnData)
                    {
                        this.columnName = columnName;
                        this.columnValue = columnData;
                    }
                    public column(string columnName, DBType columntype, object columnData)
                    {
                        this.columnName = columnName;
                        this.columntype = columntype;
                        this.columnValue = columnData;
                    }
                }
                class SqlCodeGenerator
                {
                    private SqlCodeGenerator(){}
                    private static SqlCodeGenerator instance = null;
                    private static object obj = new object();
                    public static SqlCodeGenerator GetInstance()
                    {
                        lock (obj)
                        {
                            if (instance == null)
                            {
                                instance = new SqlCodeGenerator();
                            }
                            return instance;
                        }
                    }
                    public string CreateSelect(string tableName)
                    {
                        return "SELECT * FROM " + tableName;
                    }
                    public string CreateSelect(string tableName, int NbRecord, Providers prov)
                    {
                        if (prov == Providers.SQLSERVER)
                            return "SELECT Top " + NbRecord + " * FROM " + tableName;
                        if (prov == Providers.MYSQL)
                            return "SELECT " + " * FROM " + tableName + " LIMIT  " + NbRecord;
                        return "";
                    }
                    public string CreateSelect(string tableName, int NbRecord, bool NotRepetedRecord, Providers prov)
                    {
                        if (prov == Providers.SQLSERVER)
                        {
                            if (NotRepetedRecord)
                                return "SELECT DISTINCT Top " + NbRecord + " * FROM " + tableName;
                            else
                                return "SELECT Top " + NbRecord + " * FROM " + tableName;
                        }
                        else if (prov == Providers.MYSQL)
                        {
                            if (NotRepetedRecord)
                                return "SELECT DISTINCT " + " * FROM " + tableName + " LIMIT " + NbRecord;
                            else
                                return "SELECT  " + " * FROM " + tableName + " LIMIT " + NbRecord; ;
                        }
                        return "";

                    }
                    public string CreateSelect(string tableName, bool NotRepetedRecords)
                    {
                        if (NotRepetedRecords)
                            return "SELECT DISTINCT  * FROM " + tableName;
                        else
                            return "SELECT   * FROM " + tableName;
                    }
                    public string CreateLimtedSelect(string TableName, int nbRecord, Providers prov, List<column> ConditionColumns, OpType[] op)
                    {
                        StringBuilder str = new StringBuilder("SELECT ");
                        if (prov == Providers.SQLSERVER)
                        {
                            str.Append(" TOP " + nbRecord);
                            str.Append(GetColumnName(ConditionColumns) + " FROM " + TableName + " WHERE ");
                            str.Append(GetCondition(ConditionColumns, op));
                        }
                        else if (prov == Providers.MYSQL)
                        {

                            str.Append(GetColumnName(ConditionColumns) + " FROM " + TableName + " WHERE ");
                            str.Append(GetCondition(ConditionColumns, op));
                            str.Append(" LIMIT " + nbRecord);
                        }

                        return str.ToString();
                    }
                    public string SelectInnerJoin(string table1, string table2, string primarykeyColumnName, string foreignKeyColumn, OpType op)
                    {
                        StringBuilder str = new StringBuilder("SELECT * from " + table1 + " INNER JOIN " + table2 + " ON ");
                        str.Append(GetJoinCondition(primarykeyColumnName, foreignKeyColumn, table1, table2, op));
                        return str.ToString();
                    }
                    public string SelectInnerJoin(string table1, string table2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumn, OpType op)
                    {
                        StringBuilder str = new StringBuilder("SELECT " + GetColumnName(ColumnsName) + " from " + table1 + " INNER JOIN " + table2 + " ON ");
                        str.Append(GetJoinCondition(primarykeyColumnName, foreignKeyColumn, table1, table2, op));
                        return str.ToString();
                    }
                    public string SelectInnerJoin(string table1, string table2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumn, OpType op, List<column> columnofcondion, OpType[] ops, RelationName[] relations)
                    {
                        StringBuilder str = new StringBuilder("SELECT " + GetColumnName(ColumnsName) + " from " + table1 + " INNER JOIN " + table2 + " ON ");
                        str.Append(GetJoinCondition(primarykeyColumnName, foreignKeyColumn, table1, table2, op));
                        str.Append(GetCondition(columnofcondion, ops, relations));
                        return str.ToString();
                    }
                    public string SelectLeftJoin(string table1, string table2, string primarykeyColumnName, string foreignKeyColumn, OpType op)
                    {
                        StringBuilder str = new StringBuilder("SELECT * from " + table1 + " LEFT JOIN " + table2 + " ON ");
                        str.Append(GetJoinCondition(primarykeyColumnName, foreignKeyColumn, table1, table2, op));
                        return str.ToString();
                    }
                    public string SelectLeftJoin(string table1, string table2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumn, OpType op)
                    {
                        StringBuilder str = new StringBuilder("SELECT " + GetColumnName(ColumnsName) + " from " + table1 + " LEFT JOIN " + table2 + " ON ");
                        str.Append(GetJoinCondition(primarykeyColumnName, foreignKeyColumn, table1, table2, op));
                        return str.ToString();
                    }
                    public string SelectLeftJoin(string table1, string table2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumn, OpType op, List<column> columnofcondion, OpType[] ops, RelationName[] relations)
                    {
                        StringBuilder str = new StringBuilder("SELECT " + GetColumnName(ColumnsName) + " from " + table1 + " LEFT JOIN " + table2 + " ON ");
                        str.Append(GetJoinCondition(primarykeyColumnName, foreignKeyColumn, table1, table2, op));
                        str.Append(GetCondition(columnofcondion, ops, relations));
                        return str.ToString();
                    }
                    public string SelectRightJoin(string table1, string table2, string primarykeyColumnName, string foreignKeyColumn, OpType op)
                    {
                        StringBuilder str = new StringBuilder("SELECT * from " + table1 + " RIGHT JOIN " + table2 + " ON ");
                        str.Append(GetJoinCondition(primarykeyColumnName, foreignKeyColumn, table1, table2, op));
                        return str.ToString();
                    }
                    public string SelectRightJoin(string table1, string table2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumn, OpType op)
                    {
                        StringBuilder str = new StringBuilder("SELECT " + GetColumnName(ColumnsName) + " from " + table1 + " RIGHT JOIN " + table2 + " ON ");
                        str.Append(GetJoinCondition(primarykeyColumnName, foreignKeyColumn, table1, table2, op));
                        return str.ToString();
                    }
                    public string SelectRightJoin(string table1, string table2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumn, OpType op, List<column> columnofcondion, OpType[] ops, RelationName[] relations)
                    {
                        StringBuilder str = new StringBuilder("SELECT " + GetColumnName(ColumnsName) + " from " + table1 + " RIGHT JOIN " + table2 + " ON ");
                        str.Append(GetJoinCondition(primarykeyColumnName, foreignKeyColumn, table1, table2, op));
                        str.Append(GetCondition(columnofcondion, ops, relations));
                        return str.ToString();
                    }
                    public string SelectFulltJoin(string table1, string table2, string primarykeyColumnName, string foreignKeyColumn, OpType op)
                    {
                        StringBuilder str = new StringBuilder("SELECT * from " + table1 + " FULL JOIN " + table2 + " ON ");
                        str.Append(GetJoinCondition(primarykeyColumnName, foreignKeyColumn, table1, table2, op));
                        return str.ToString();
                    }
                    public string SelectFullJoin(string table1, string table2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumn, OpType op)
                    {
                        StringBuilder str = new StringBuilder("SELECT " + GetColumnName(ColumnsName) + " from " + table1 + " FULL JOIN " + table2 + " ON ");
                        str.Append(GetJoinCondition(primarykeyColumnName, foreignKeyColumn, table1, table2, op));
                        return str.ToString();
                    }
                    public string SelectFullJoin(string table1, string table2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumn, OpType op, List<column> columnofcondion, OpType[] ops, RelationName[] relations)
                    {
                        StringBuilder str = new StringBuilder("SELECT " + GetColumnName(ColumnsName) + " from " + table1 + " FULL JOIN " + table2 + " ON ");
                        str.Append(GetJoinCondition(primarykeyColumnName, foreignKeyColumn, table1, table2, op));
                        str.Append(GetCondition(columnofcondion, ops, relations));
                        return str.ToString();
                    }
                    public string SelectGroupBy(string tableName, List<column> Columns, List<column> columnForGroupBy)
                    {
                        StringBuilder str = new StringBuilder("SELECT ");
                        if (CheckGroupByColumns(Columns, columnForGroupBy))
                        {
                            str.Append(GetColumnName(Columns) + " FROM " + tableName + " GROUP BY ");
                            str.Append(GetColumnName(columnForGroupBy));
                        }
                        else
                        {
                            return null;
                        }
                        return str.ToString();
                    }
                    public string SelectGroupBy(string[] tablesName, List<column> Columns, List<column> columnForGroupBy)
                    {
                        StringBuilder str = new StringBuilder("SELECT ");
                        if (CheckGroupByColumns(Columns, columnForGroupBy))
                        {
                            str.Append(GetColumnName(Columns) + " FROM ");
                            str.Append(GetTablesName(tablesName));
                            str.Append(" GROUP BY ");
                            str.Append(GetColumnName(columnForGroupBy));
                        }
                        else
                        {
                            return null;
                        }
                        return str.ToString();
                    }
                    public string SelectGroupBy(string[] tablesName, List<column> Columns, List<column> columnForGroupBy, List<column> HavingconditionColumn, OpType[] ops)
                    {
                        StringBuilder str = new StringBuilder("SELECT ");
                        if (CheckGroupByColumns(Columns, columnForGroupBy))
                        {
                            str.Append(GetColumnName(Columns) + " FROM ");
                            str.Append(GetTablesName(tablesName));
                            str.Append(" GROUP BY ");
                            str.Append(GetColumnName(columnForGroupBy));
                            str.Append(" HAVING ");
                            str.Append(GetCondition(HavingconditionColumn, ops));
                        }
                        else
                        {
                            return null;
                        }
                        return str.ToString();
                    }
                    public string SelectGroupBy(string tableName, List<column> Columns, List<column> columnForGroupBy, List<column> HavingconditionColumn, OpType[] ops)
                    {
                        StringBuilder str = new StringBuilder("SELECT ");
                        if (CheckGroupByColumns(Columns, columnForGroupBy))
                        {
                            str.Append(GetColumnName(Columns) + " FROM " + tableName);
                            str.Append(" GROUP BY ");
                            str.Append(GetColumnName(columnForGroupBy));
                            str.Append(" HAVING ");
                            str.Append(GetCondition(HavingconditionColumn, ops));
                        }
                        else
                        {
                            return null;
                        }
                        return str.ToString();
                    }
                    public string SelectGroupBy(string[] tablesName, List<column> Columns, List<column> columnForGroupBy, List<column> HavingconditionColumn, OpType[] havingops, List<column> WhereConditionColumn, OpType[] whereops)
                    {
                        StringBuilder str = new StringBuilder("SELECT ");
                        if (CheckGroupByColumns(Columns, columnForGroupBy))
                        {
                            str.Append(GetColumnName(Columns) + " FROM ");
                            str.Append(GetTablesName(tablesName));
                            str.Append(" GROUP BY ");
                            str.Append(GetColumnName(columnForGroupBy));
                            str.Append(" HAVING ");
                            str.Append(GetCondition(HavingconditionColumn, havingops));
                            str.Append(" WHERE ");
                            str.Append(GetCondition(WhereConditionColumn, whereops));
                        }
                        else
                        {
                            return null;
                        }
                        return str.ToString();
                    }
                    public string SelectGroupBy(string tableName, List<column> Columns, List<column> columnForGroupBy, List<column> HavingconditionColumn, OpType[] havingops, List<column> WhereConditionColumn, OpType[] whereops)
                    {
                        StringBuilder str = new StringBuilder("SELECT ");
                        if (CheckGroupByColumns(Columns, columnForGroupBy))
                        {
                            str.Append(GetColumnName(Columns) + " FROM " + tableName);
                            str.Append(" GROUP BY ");
                            str.Append(GetColumnName(columnForGroupBy));
                            str.Append(" HAVING ");
                            str.Append(GetCondition(HavingconditionColumn, havingops));
                            str.Append(" WHERE ");
                            str.Append(GetCondition(WhereConditionColumn, whereops));
                        }
                        else
                        {
                            return null;
                        }
                        return str.ToString();
                    }
                    public string SelectOrderBy(string tableName, List<column> Columns, List<column> columnForGroupBy)
                    {
                        StringBuilder str = new StringBuilder("SELECT ");
                        if (CheckOrderByColumns(Columns, columnForGroupBy))
                        {
                            str.Append(GetColumnName(Columns) + " FROM " + tableName + " ORDER BY ");
                            str.Append(GetColumnName(columnForGroupBy));
                        }
                        else
                        {
                            return null;
                        }
                        return str.ToString();
                    }
                    public string SelectOrderBy(string[] tablesName, List<column> Columns, List<column> columnForGroupBy)
                    {
                        StringBuilder str = new StringBuilder("SELECT ");
                        if (CheckOrderByColumns(Columns, columnForGroupBy))
                        {
                            str.Append(GetColumnName(Columns) + " FROM ");
                            str.Append(GetTablesName(tablesName));
                            str.Append(" ORDER BY");
                            str.Append(GetColumnName(columnForGroupBy));
                        }
                        else
                        {
                            return null;
                        }
                        return str.ToString();
                    }
                    public string SelectOrderBy(string tableName, List<column> Columns, List<column> columnForGroupBy, GroupType grouptype)
                    {
                        StringBuilder str = new StringBuilder("SELECT ");
                        if (CheckOrderByColumns(Columns, columnForGroupBy))
                        {
                            str.Append(GetColumnName(Columns) + " FROM " + tableName + " ORDER BY ");
                            str.Append(GetColumnName(columnForGroupBy) + " " + grouptype);
                        }
                        else
                        {
                            return null;
                        }
                        return str.ToString();
                    }
                    public string SelectOrderBy(string[] tablesName, List<column> Columns, List<column> columnForGroupBy, GroupType grouptype)
                    {
                        StringBuilder str = new StringBuilder("SELECT ");
                        if (CheckOrderByColumns(Columns, columnForGroupBy))
                        {
                            str.Append(GetColumnName(Columns) + " FROM ");
                            str.Append(GetTablesName(tablesName));
                            str.Append(" ORDER BY");
                            str.Append(GetColumnName(columnForGroupBy) + " " + grouptype);
                        }
                        else
                        {
                            return null;
                        }
                        return str.ToString();
                    }
                    public string Insert(string tableName, List<column> col)
                    {
                        StringBuilder str = new StringBuilder("INSERT INTO " + tableName + " (");
                        str.Append(GetColumnName(col));
                        str.Append(" VALUES");
                        str.Append(GetColumnValue(col));
                        return str.ToString();
                    }
                    public string Delete(string TableName, column column, OpType op)
                    {
                        return "DELETE FROM " + TableName + " WHERE " + column.ColumnName + " " + op + " " + column.ColumnValue;
                    }
                    public string Delete(string TableName, List<column> columns, OpType[] op, RelationName[] relation)
                    {
                        StringBuilder str = new StringBuilder("DELETE FROM " + TableName + " WHERE ");
                        str.Append(GetCondition(columns, op, relation));
                        return str.ToString();
                    }
                    public string UpdateALL(string TableName, List<column> columns)
                    {
                        StringBuilder str = new StringBuilder("UPDATE " + TableName + " SET ");
                        str.Append(GetSetColumns(columns));
                        return str.ToString();
                    }
                    public string Update(string TableName, List<column> columns, OpType[] op, RelationName[] relation)
                    {
                        StringBuilder str = new StringBuilder("UPDATE " + TableName + " SET ");
                        str.Append(GetSetColumns(columns));
                        str.Append(" WHERE ");
                        str.Append(GetCondition(columns, op, relation));
                        return str.ToString();
                    }



                    public string CreateDB(string DBNAME)
                    {
                        return "CREATE DATABASE " + DBNAME;
                    }
                    public string USEDB(string DBNAME)
                    {
                        return "USE " + DBNAME;
                    }
                    public string CreateTable(string TableName, Table table, Providers prov)
                    {
                        StringBuilder str = new StringBuilder("CREATE TABLE " + TableName + " ( " + table.colNam + " " + table.coltyp);
                        if (table.colsiz != null)
                            str.Append("(" + table.colsiz + ")");
                        if (table.notnull)
                            str.Append(" NOT NULL ");
                        if (table.primary_key)
                        {
                            str.Append(" PRIMARY KEY ");
                        }
                        if (table.unique)
                        {
                            str.Append(" UNIQUE ");
                        }
                        if (table.auto_increment || table.unique)
                        {
                            switch (prov)
                            {
                                case Providers.SQLSERVER:
                                    {
                                        str.Append(" IDENTITY(1,1) ");
                                        break;
                                    }
                                case Providers.MYSQL:
                                    {
                                        str.Append(" AUTO_INCREMENT ");
                                        break;
                                    }
                            }

                            if (!string.IsNullOrEmpty(table.Defaulte))
                            {
                                str.Append(" DEFAULT (" + table.Defaulte + ") ");
                            }
                            if (!string.IsNullOrEmpty(table.Foreing_Key.columnName) && !string.IsNullOrEmpty(table.Foreing_Key.referenceTableName))
                            {
                                str.Append(" FORIGN KEY REFERENCES " + table.Foreing_Key.referenceTableName + " ( " + table.Foreing_Key.columnName + " ");
                            }
                        }
                        if(table.Check!=null)
                        {
                            str.Append(GetCheckCode(table.Check));
                        }

                        str.Append(" ) ");
                        return str.ToString();
                    }
                    public string DeleteDB(string DBName)
                    {
                        return "DROP DATABASE "+DBName;
                    }
                    public string DeleteTable(string TableNeme)
                    {
                        return "DROP DATABASE " + TableNeme;
                    }




/*==========================================================================================================================================*/
                    private string GetCheckCode(check chk)
                    {
                        StringBuilder str = new StringBuilder(" CHECK (");
                        if (chk.relation.Length == 0 && chk.col.Count == 1 && chk.op.Length == 1)
                        {
                            str.Append(GetCondition(chk.col[0], chk.op[0]) + " ) ");
                        }
                        if (chk.relation.Length + 1 == chk.op.Length && chk.col.Count == chk.op.Length)
                        {
                            str.Append(GetCondition(chk.col,(OpType[])chk.op,chk.relation)+" ) ");
                        }

                        return str.ToString();
                    }
                    private string GetCondition(List<column> CondiotionColumns, OpType[] Operators)
                    {
                        StringBuilder str = new StringBuilder();
                        if (CondiotionColumns.Count != Operators.Length)
                        {
                            return null;
                        }
                        else
                        {
                            for (int i = 0; i < CondiotionColumns.Count && i < Operators.Length; i++)
                            {
                                str.Append(" " + CondiotionColumns[i].ColumnName);
                                switch (Operators[i])
                                {
                                    case OpType.LessThan: str.Append(" > "); break;
                                    case OpType.GreatThan: str.Append(" < "); break;
                                    case OpType.Equals: str.Append(" = "); break;
                                    case OpType.lessOrEquals: str.Append(" >= "); break;
                                    case OpType.GreatOrEquals: str.Append(" <= "); break;
                                    case OpType.NotEquals: str.Append(" <> "); break;
                                    default: goto Error;
                                }
                                if (i + 1 == CondiotionColumns.Count)
                                    str.Append(CondiotionColumns[i].ColumnValue);
                                else
                                    str.Append(CondiotionColumns[i].ColumnValue + " AND ");
                            }
                        }
                        return str.ToString();
                    Error:
                        return null;
                    }
                    
                    private string GetSetColumns(List<column> CondiotionColumns)
                    {
                        StringBuilder str = new StringBuilder();
                        if (CondiotionColumns == null) return null;
                        for (int i = 0; i < CondiotionColumns.Count; i++)
                        {
                            str.Append(" " + CondiotionColumns[i].ColumnName);
                            if (i + 1 == CondiotionColumns.Count)
                                str.Append(" = " + CondiotionColumns[i].ColumnValue);
                            else
                                str.Append(" = " + CondiotionColumns[i].ColumnValue + " AND ");
                        }
                        return str.ToString();
                    }
                    private string GetCondition(List<column> CondiotionColumns, OpType[] Operators, RelationName[] relation)
                    {
                        StringBuilder str = new StringBuilder();
                        if (CondiotionColumns.Count != Operators.Length || Operators.Length != relation.Length)
                        {
                            return null;
                        }
                        else
                        {
                            for (int i = 0; i < CondiotionColumns.Count && i < Operators.Length; i++)
                            {
                                switch (relation[i])
                                {
                                    case RelationName.AND: str.Append(" AND "); break;
                                    case RelationName.OR: str.Append(" OR "); break;
                                    default: goto Error;
                                }
                                str.Append(" " + CondiotionColumns[i].ColumnName);
                                switch (Operators[i])
                                {
                                    case OpType.LessThan: str.Append(" > "); break;
                                    case OpType.GreatThan: str.Append(" < "); break;
                                    case OpType.Equals: str.Append(" = "); break;
                                    case OpType.lessOrEquals: str.Append(" >= "); break;
                                    case OpType.GreatOrEquals: str.Append(" <= "); break;
                                    case OpType.NotEquals: str.Append(" <> "); break;
                                    default: goto Error;
                                }
                                if (i + 1 == CondiotionColumns.Count)
                                    str.Append(CondiotionColumns[i].ColumnValue);
                                else
                                    str.Append(CondiotionColumns[i].ColumnValue + " AND ");
                            }
                        }
                        return str.ToString();
                    Error:
                        return null;
                    }
                    private string GetWhereConditionOnJoin(IDictionary<string, column> cols, OpType[] Operators, RelationName[] re)
                    {
                        StringBuilder str = new StringBuilder();
                        var table = cols.Keys.ToList();
                        var col = cols.Values.ToList();

                        if (cols.Count != Operators.Length)
                        {
                            return null;
                        }
                        else
                        {
                            for (int i = 0; i < cols.Count && i < Operators.Length; i++)
                            {
                                str.Append(" " + table[i] + "." + col[i].ColumnName);
                                switch (Operators[i])
                                {
                                    case OpType.LessThan: str.Append(" > "); break;
                                    case OpType.GreatThan: str.Append(" < "); break;
                                    case OpType.Equals: str.Append(" = "); break;
                                    case OpType.lessOrEquals: str.Append(" >= "); break;
                                    case OpType.GreatOrEquals: str.Append(" <= "); break;
                                    case OpType.NotEquals: str.Append(" <> "); break;
                                    default: goto Error;
                                }
                                if (i + 1 == cols.Count)
                                    str.Append(col[i].ColumnValue);
                                else
                                {
                                    str.Append(col[i].ColumnValue);
                                    switch (re[i])
                                    {
                                        case RelationName.AND: str.Append(" AND "); break;
                                        case RelationName.OR: str.Append(" OR "); break;
                                    }
                                }
                            }
                        }
                        return str.ToString();
                    Error:
                        return null;
                    }
                    private string GetCondition(column Column, OpType Operator)
                    {
                        StringBuilder str = new StringBuilder();
                        if (Column == null || Operator.GetType() != typeof(OpType))
                        {
                            return null;
                        }
                        else
                        {
                            str.Append(" " + Column.ColumnName);
                            switch (Operator)
                            {
                                case OpType.LessThan: str.Append(" > "); break;
                                case OpType.GreatThan: str.Append(" < "); break;
                                case OpType.Equals: str.Append(" = "); break;
                                case OpType.lessOrEquals: str.Append(" >= "); break;
                                case OpType.GreatOrEquals: str.Append(" <= "); break;
                                case OpType.NotEquals: str.Append(" <> "); break;
                                default: goto Error;
                            }
                            str.Append(Column.ColumnValue);
                        }
                        return str.ToString();
                    Error:
                        return null;
                    }
                    private string GetJoinCondition(string Column1, string column2, string table1, string table2, OpType Operator)
                    {
                        StringBuilder str = new StringBuilder();
                        if (Column1 == null || column2 == null || Operator.GetType() != typeof(OpType))
                        {
                            return null;
                        }
                        else
                        {
                            str.Append(" " + table1 + "." + Column1);
                            switch (Operator)
                            {
                                case OpType.LessThan: str.Append(" > "); break;
                                case OpType.GreatThan: str.Append(" < "); break;
                                case OpType.Equals: str.Append(" = "); break;
                                case OpType.lessOrEquals: str.Append(" >= "); break;
                                case OpType.GreatOrEquals: str.Append(" <= "); break;
                                case OpType.NotEquals: str.Append(" <> "); break;
                                default: goto Error;
                            }
                            str.Append(" " + table2 + "." + column2);
                        }
                        return str.ToString();
                    Error:
                        return null;
                    }
                    private string GetColumnName(List<column> col)
                    {
                        StringBuilder str = new StringBuilder(" ");
                        for (int i = 0; i < col.Count; i++)
                        {
                            if (col.Count == 1)
                            {
                                str.Append(col[i].ColumnName + " ");
                                break;
                            }
                            if (i + 1 == col.Count)
                            {
                                str.Append(col[i].ColumnName + " ");
                                break;
                            }
                            else
                            {
                                str.Append(col[i].ColumnName + " , ");
                                continue;
                            }
                        }
                        return str.ToString();
                    }
                    private string GetTablesName(string[] tables)
                    {
                        StringBuilder str = new StringBuilder(" ");
                        for (int i = 0; i < tables.Length; i++)
                        {
                            if (tables.Length == 1)
                            {
                                str.Append(tables[i] + " ");
                                break;
                            }
                            if (i + 1 == tables.Length)
                            {
                                str.Append(tables[i] + " ");
                                break;
                            }
                            else
                            {
                                str.Append(tables[i] + " , ");
                                continue;
                            }
                        }
                        return str.ToString();
                    }
                    private string GetColumnValue(List<column> col)
                    {
                        StringBuilder str = new StringBuilder("( ");
                        for (int i = 0; i < col.Count; i++)
                        {
                            if (i < col.Count - 2)
                            {
                                str.Append(col[i].ColumnValue + " )");
                            }
                            else
                            {
                                str.Append(col[i].ColumnValue + " , ");
                            }
                        }
                        return str.ToString();
                    }
                    private bool CheckGroupByColumns(List<column> col1, List<column> col2)
                    {
                        bool x = false;
                        List<string> str1 = new List<string>();
                        List<string> str2 = new List<string>();
                        foreach (var item in col1)
                        {
                            str1.Add(item.ColumnName.ToLower());
                        }
                        foreach (var item in col2)
                        {
                            str2.Add(item.ColumnName.ToLower());
                        }
                        foreach (var item in str2)
                        {
                            if (str1.Contains(item))
                            {
                                x = true;
                            }
                            else
                            {
                                x = false;
                                break;
                            }
                        }
                        return x;
                    }
                    private bool CheckOrderByColumns(List<column> col1, List<column> col2)
                    {
                        bool x = false;
                        List<string> str1 = new List<string>();
                        List<string> str2 = new List<string>();
                        foreach (var item in col1)
                        {
                            str1.Add(item.ColumnName.ToLower());
                        }
                        foreach (var item in col2)
                        {
                            str2.Add(item.ColumnName.ToLower());
                        }
                        foreach (var item in str2)
                        {
                            if (str1.Contains(item))
                            {
                                x = true;
                                str1.Remove(item);
                            }
                            else
                            {
                                x = false;
                                break;
                            }
                        }
                        return x;
                    }
                }
            }
            namespace SqlServer
            {
                public class SqlServer
                {
                    /// <summary>
                    /// Fields
                    /// </summary>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    /// 
                    #region fields
                    private static SqlServer Instance = null;
                    private SqlCodeGenerator sql = SqlCodeGenerator.GetInstance();
                    private static object o = new object();
                    private static SqlConnection cnx = null;
                    private static SqlCommand cmd = null;
                    private static SqlDataReader read = null;
                    private DataTable table=new DataTable();
                    #endregion 
                    /// <summary>
                    /// class methods
                    /// </summary>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    /// 
                    #region class Metods
                    private SqlServer() { }
                    public static SqlServer GetInstance(string path)
                    {
                        lock (o)
                        {
                            if (Instance == null)
                                Instance = new SqlServer();
                            string connectionString = getConnectionString(path);
                            cnx = new SqlConnection(connectionString);
                            return Instance;
                        }
                    }
                    private static string getConnectionString(string path)
                    {
                        string connectionString = File.ReadAllText(path).ToString();
                        return connectionString;
                    }
                    #endregion
                    /// <summary>
                    /// connection methods
                    /// </summary>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    /// 
                    #region about connection
                    public void Open()
                    {
                        if (cnx.State != ConnectionState.Open)
                        {
                            cnx.Open();
                        }
                    }
                    public void Close()
                    {
                        if (cnx.State != ConnectionState.Closed)
                        {
                            cnx.Close();
                        }
                    }
                    public void CloseReading()
                    {
                        read.Close();
                        cnx.Close();
                    }
                    #endregion
                    /// <summary>
                    /// Read From Data Base
                    /// </summary>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    /// 
                    #region Methods of READING FROM DATABASE
                    public SqlDataReader ReadAll(string tableName)
                    {

                        cmd = new SqlCommand(sql.CreateSelect(tableName), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadAllLimeted(string tableName, int NbRecord)
                    {
                        cmd = new SqlCommand(sql.CreateSelect(tableName, NbRecord, Providers.SQLSERVER), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadAllLimetedAndNoRepete(string tableName, int NbRecord, bool NotRepetedRecord)
                    {
                        cmd = new SqlCommand(sql.CreateSelect(tableName, NbRecord, NotRepetedRecord, Providers.SQLSERVER), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadAllAndNoRepete(string tableName, bool NotRepetedRecords)
                    {
                        cmd = new SqlCommand(sql.CreateSelect(tableName, NotRepetedRecords), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader Read(string tableName, int nbRecord, Providers prov, List<column> ConditionColumns, OpType[] op)
                    {
                        cmd = new SqlCommand(sql.CreateLimtedSelect(tableName, nbRecord, Providers.SQLSERVER, ConditionColumns, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadWithInnerJoin(string tableName1, string tableName2, string primarykeyColumnName, string foreignKeyColumn, OpType op)
                    {
                        cmd = new SqlCommand(sql.SelectInnerJoin(tableName1, tableName2, primarykeyColumnName, foreignKeyColumn, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadWithInnerJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new SqlCommand(sql.SelectInnerJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadWithInnerJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op, List<column> columnofcondion, OpType[] ops, RelationName[] relations)
                    {
                        cmd = new SqlCommand(sql.SelectInnerJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op, columnofcondion, ops, relations), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadWithLeftJoin(string tableName1, string tableName2, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new SqlCommand(sql.SelectLeftJoin(tableName1, tableName2, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadWithLeftJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new SqlCommand(sql.SelectLeftJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadWithLeftJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op, List<column> columnofcondion, OpType[] ops, RelationName[] relations)
                    {
                        cmd = new SqlCommand(sql.SelectLeftJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op, columnofcondion, ops, relations), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadWithRightJoin(string tableName1, string tableName2, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new SqlCommand(sql.SelectRightJoin(tableName1, tableName2, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadWithRightJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new SqlCommand(sql.SelectRightJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadWithRightJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op, List<column> columnofcondion, OpType[] ops, RelationName[] relations)
                    {
                        cmd = new SqlCommand(sql.SelectRightJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op, columnofcondion, ops, relations), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadWithFullJoin(string tableName1, string tableName2, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new SqlCommand(sql.SelectFulltJoin(tableName1, tableName2, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadWithFullJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new SqlCommand(sql.SelectFullJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadWithFullJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op, List<column> columnofcondion, OpType[] ops, RelationName[] relations)
                    {
                        cmd = new SqlCommand(sql.SelectFullJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op, columnofcondion, ops, relations), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadWithGroupBy(string tableName, List<column> Columns, List<column> columnForGroupBy)
                    {
                        cmd = new SqlCommand(sql.SelectGroupBy(tableName, Columns, columnForGroupBy), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;

                    }
                    public SqlDataReader ReadWithGroupBy(string[] tablesName, List<column> Columns, List<column> columnForGroupBy)
                    {
                        cmd = new SqlCommand(sql.SelectGroupBy(tablesName, Columns, columnForGroupBy), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;

                    }
                    public SqlDataReader ReadWithGroupBy(string[] tablesName, List<column> Columns, List<column> columnForGroupBy, List<column> HavingconditionColumn, OpType[] ops)
                    {
                        cmd = new SqlCommand(sql.SelectGroupBy(tablesName, Columns, columnForGroupBy, HavingconditionColumn, ops), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;

                    }
                    public SqlDataReader ReadWithGroupBy(string tableName, List<column> Columns, List<column> columnForGroupBy, List<column> HavingconditionColumn, OpType[] ops)
                    {
                        cmd = new SqlCommand(sql.SelectGroupBy(tableName, Columns, columnForGroupBy, HavingconditionColumn, ops), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadWithGroupBy(string[] tablesName, List<column> Columns, List<column> columnForGroupBy, List<column> HavingconditionColumn, OpType[] havingops, List<column> WhereConditionColumn, OpType[] whereops)
                    {
                        cmd = new SqlCommand(sql.SelectGroupBy(tablesName, Columns, columnForGroupBy, HavingconditionColumn, havingops, WhereConditionColumn, whereops), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadWithGroupBy(string tableName, List<column> Columns, List<column> columnForGroupBy, List<column> HavingconditionColumn, OpType[] havingops, List<column> WhereConditionColumn, OpType[] whereops)
                    {
                        cmd = new SqlCommand(sql.SelectGroupBy(tableName, Columns, columnForGroupBy, HavingconditionColumn, havingops, WhereConditionColumn, whereops), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader ReadWithOrderBy(string tableName, List<column> Columns, List<column> columnForGroupBy)
                    {
                        cmd = new SqlCommand(sql.SelectOrderBy(tableName, Columns, columnForGroupBy), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader Read(string[] tablesName, List<column> Columns, List<column> columnForOrderBy)
                    {
                        cmd = new SqlCommand(sql.SelectOrderBy(tablesName, Columns, columnForOrderBy), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader Read(string tableName, List<column> Columns, List<column> columnForOrderBy, GroupType grouptype)
                    {
                        cmd = new SqlCommand(sql.SelectOrderBy(tableName, Columns, columnForOrderBy, grouptype), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public SqlDataReader Read(string[] tablesName, List<column> Columns, List<column> columnForOrderBy, GroupType grouptype)
                    {
                        cmd = new SqlCommand(sql.SelectOrderBy(tablesName, Columns, columnForOrderBy, grouptype), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    #endregion
                    /// <summary>
                    ///    operatoin methods 
                    /// </summary>
                    /// <param name="dt"></param>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    /// 
                    #region Methods to do some operation on database
                    //Insert 
                    public void Insert(string tableName, List<column> col)
                    {
                        cmd = new SqlCommand(sql.Insert(tableName, col), cnx);
                        try
                        {
                            Open();
                            cmd.ExecuteNonQuery();
                            Close();
                        }
                        catch { }

                    }
                    //Delete
                    public void Delete(string tableName, column column, OpType op)
                    {
                        cmd = new SqlCommand(sql.Delete(tableName, column, op), cnx);
                        try
                        {
                            Open();
                            cmd.ExecuteNonQuery();
                            Close();
                        }
                        catch { }


                    }
                    public void Delete(string tableName, List<column> columns, OpType[] op, RelationName[] relation)
                    {
                        cmd = new SqlCommand(sql.Delete(tableName, columns, op, relation), cnx);
                        try
                        {
                            Open();
                            cmd.ExecuteNonQuery();
                            Close();
                        }
                        catch { }
                    }
                    //Update
                    public void UpdateAll(string tableName, List<column> columns)
                    {
                        cmd = new SqlCommand(sql.UpdateALL(tableName, columns), cnx);
                        try
                        {
                            Open();
                            cmd.ExecuteNonQuery();
                            Close();
                        }
                        catch { }
                    }
                    public void Update(string tableName, List<column> columns, OpType[] op, RelationName[] relation)
                    {
                        cmd = new SqlCommand(sql.Update(tableName, columns, op, relation), cnx);
                        try
                        {
                            Open();
                            cmd.ExecuteNonQuery();
                            Close();
                        }
                        catch { }
                    }
                    #endregion
                    /// <summary>
                    ///   methods that fill controls
                    /// </summary>
                    /// <param name="dt"></param>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    /// 
                    #region methods to fill bouth of atagrid view and combobox
                    public void FillDataGridViewByOneTable(System.Windows.Forms.DataGridView dt,string tableName)
                    {
                        table = new DataTable();
                        table.Load(ReadAll(tableName));
                        dt.DataSource = table;
                        CloseReading();
                        
                    }
                    public void FillDataGridViewByTowTable(DataGridView dt, string tableName1, string tableName2, string primaryKeyColumnName, string foreignKeyColumnName, OpType op,JoinType jtype)
                    {
                        table = new DataTable();
                        switch(jtype)
                        {
                            case JoinType.INNER: table.Load(ReadWithInnerJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.LEFT: table.Load(ReadWithLeftJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.RIGHT: table.Load(ReadWithRightJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.FULL: table.Load(ReadWithFullJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            default: table = null; break;
                        }
                        
                        CloseReading();
                        dt.DataSource = table;
                    }

                    public void FillComboBoxByOneTable(System.Windows.Forms.ComboBox cb, string tableName)
                    {
                        table = new DataTable();
                        table.Load(ReadAll(tableName));
                        cb.DataSource = table;
                        CloseReading();

                    }
                    public void FillComboBoxByTowTable(ComboBox cb, string tableName1, string tableName2, string primaryKeyColumnName, string foreignKeyColumnName, OpType op, JoinType jtype)
                    {
                        table = new DataTable();
                        switch (jtype)
                        {
                            case JoinType.INNER: table.Load(ReadWithInnerJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.LEFT: table.Load(ReadWithLeftJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.RIGHT: table.Load(ReadWithRightJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.FULL: table.Load(ReadWithFullJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            default: table = null; break;
                        }

                        CloseReading();
                        cb.DataSource = table;
                    }

                    public void FillComboBoxByOneTable(System.Windows.Forms.ComboBox cb, string tableName,string displayMemberName,string valueMemberName)
                    {
                        table = new DataTable();
                        table.Load(ReadAll(tableName));
                        cb.DataSource = table;
                        CloseReading();
                        cb.ValueMember = valueMemberName;
                        cb.DisplayMember = displayMemberName;

                    }
                    public void FillComboBoxByTowTable(ComboBox cb, string tableName1, string tableName2, string primaryKeyColumnName, string foreignKeyColumnName, OpType op, JoinType jtype, string displayMemberName, string valueMemberName)
                    {
                        table = new DataTable();
                        switch (jtype)
                        {
                            case JoinType.INNER: table.Load(ReadWithInnerJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.LEFT: table.Load(ReadWithLeftJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.RIGHT: table.Load(ReadWithRightJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.FULL: table.Load(ReadWithFullJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            default: table = null; break;
                        }
                        cb.DataSource = table;
                        CloseReading();
                        cb.ValueMember = valueMemberName;
                        cb.DisplayMember = displayMemberName;
                    }
                    #endregion
                }
                public class SqlServerDeconnectionMode
                {
                    #region fields
                    SqlCodeGenerator code = SqlCodeGenerator.GetInstance();
                    private static SqlServerDeconnectionMode instance = null;
                    private static object o = new object();
                    private static SqlConnection cnx;
                    private SqlCommand cmd;
                    private SqlDataAdapter adapter;
                    private SqlDataReader reader;
                    private DataSet set;
                    private DataTable table;
                    private SqlCommandBuilder build;
                    #endregion
                    #region Class methods
                    private SqlServerDeconnectionMode() { }
                    public static SqlServerDeconnectionMode GetInstance(string path)
                    {
                        if (instance == null)
                            instance = new SqlServerDeconnectionMode();
                        string connectionString = getConnectionString(path);
                        cnx = new SqlConnection(connectionString);
                        return instance;
                    }
                    private static string getConnectionString(string path)
                    {
                        return File.ReadAllLines(path).ToString();
                    }
                    #endregion
                    #region connection methods
                    private void Open()
                    {
                        if (cnx.State != ConnectionState.Open)
                        {
                            cnx.Open();
                        }
                    }
                    private void Close()
                    {
                        if (cnx.State != ConnectionState.Closed)
                        {
                            cnx.Close();
                        }
                    }
                    private void CloseReading()
                    {
                        reader.Close();
                        cnx.Close();
                    }
                    #endregion
                    #region methods to do some operation
                    public void FillDataSetByOneTable(DataSet ds,string TableName)
                    {
                        Open();
                        adapter = new SqlDataAdapter(code.CreateSelect(TableName),cnx);
                        adapter.Fill(ds,TableName);
                        Close();
                    }
                    public void FillDataSetByTowTable(DataSet ds, string TableName, string TableName2, string primarykeyName, string foreignkeyName, OpType op)
                    {
                        Open();
                        adapter = new SqlDataAdapter(code.SelectInnerJoin(TableName, TableName2, primarykeyName, foreignkeyName,op), cnx);
                        adapter.Fill(ds, TableName);
                        Close();
                    }
                    public void FillDataTabletByOneTable(DataTable dt, string TableName)
                    {
                        Open();
                        adapter = new SqlDataAdapter(code.CreateSelect(TableName), cnx);
                        adapter.Fill(dt);
                        Close();
                    }
                    public void FillDataTableByTowTable(DataTable ds, string TableName, string TableName2, string primarykeyName, string foreignkeyName, OpType op)
                    {
                        Open();
                        adapter = new SqlDataAdapter(code.SelectInnerJoin(TableName, TableName2, primarykeyName, foreignkeyName, op), cnx);
                        adapter.Fill(dt);
                        Close();
                    }
                    public void UpdateDataBase(DataSet ds)
                    {
                        adapter =new SqlDataAdapter();
                        build = new SqlCommandBuilder(adapter);
                        adapter.Update(ds);

                    }
                    public void UpdateDataBase(DataSet ds,string tableName)
                    {
                        adapter = new SqlDataAdapter();
                        build = new SqlCommandBuilder(adapter);
                        adapter.Update(ds,tableName);

                    }
                    public void UpdateDataBase(DataTable dt)
                    {
                        adapter = new SqlDataAdapter();
                        build = new SqlCommandBuilder(adapter);
                        adapter.Update(dt);

                    }
                    public void ExecuteStoredProcAndFuc(string StoredProcOrFuncName,SqlParameter[] param)
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = cnx;
                        cmd.CommandText = StoredProcOrFuncName;
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (param != null)
                        {
                            cmd.Parameters.AddRange(param);
                        }
                        try
                        {
                            Open();
                            cmd.ExecuteNonQuery();
                            Close();
                        }
                        catch { }

                    }
                    #endregion
                }
            }
            namespace MySql
            {
                public class MySqlConnectionMode
                {
                    /// <summary>
                    /// fields
                    /// </summary>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    /// 
                    #region Fields
                    private static MySqlConnectionMode Instance = null;
                    private SqlCodeGenerator Sql = SqlCodeGenerator.GetInstance();
                    private static object o = new object();
                    private static MySqlConnection cnx = null;
                    private static MySqlCommand cmd = null;
                    private static MySqlDataReader read = null;
                    private DataTable table = new DataTable();
                    #endregion
                    /// <summary>
                    /// class methods
                    /// </summary>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    /// 
                    #region class methods
                    private MySqlConnectionMode() { }
                    public static MySqlConnectionMode GetInstance(string path)
                    {
                        lock (o)
                        {
                            if (Instance == null)
                                Instance = new MySqlConnectionMode();
                            string connectionString = getConnectionString(path);
                            cnx = new MySqlConnection(connectionString);
                            return Instance;
                        }
                    }
                    private static string getConnectionString(string path)
                    {
                        string connectionString = File.ReadAllText(path).ToString();
                        return connectionString;
                    }
                    #endregion
                    /// <summary>
                    /// connection methods
                    /// </summary>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    ///
                    #region connection methods
                    public void Open()
                    {
                        if (cnx.State != ConnectionState.Open)
                        {
                                cnx.Open();
                        }
                    }
                    public void Close()
                    {
                        if (cnx.State != ConnectionState.Closed)
                        {
                            cnx.Close();
                        }
                    }
                    public void CloseReading()
                    {
                        read.Close();
                        cnx.Close();
                    }
                    #endregion
                    /// <summary>
                    /// reading from database
                    /// </summary>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    ///
                    #region methods of Reading From database
                    public MySqlDataReader ReadAll(string tableName)
                    {
                        cmd = new MySqlCommand(Sql.CreateSelect(tableName), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadAllLimeted(string tableName, int NbRecord)
                    {
                        cmd = new MySqlCommand(Sql.CreateSelect(tableName, NbRecord, Providers.MYSQL), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadAllLimetedAndNoRepete(string tableName, int NbRecord, bool NotRepetedRecord)
                    {
                        cmd = new MySqlCommand(Sql.CreateSelect(tableName, NbRecord, NotRepetedRecord, Providers.MYSQL), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadAllAndNoRepete(string tableName, bool NotRepetedRecords)
                    {
                        cmd = new MySqlCommand(Sql.CreateSelect(tableName, NotRepetedRecords), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader Read(string tableName, int nbRecord, Providers prov, List<column> ConditionColumns, OpType[] op)
                    {
                        cmd = new MySqlCommand(Sql.CreateLimtedSelect(tableName, nbRecord, Providers.MYSQL, ConditionColumns, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadWithInnerJoin(string tableName1, string tableName2, string primarykeyColumnName, string foreignKeyColumn, OpType op)
                    {
                        cmd = new MySqlCommand(Sql.SelectInnerJoin(tableName1, tableName2, primarykeyColumnName, foreignKeyColumn, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadWithInnerJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new MySqlCommand(Sql.SelectInnerJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadWithInnerJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op, List<column> columnofcondion, OpType[] ops, RelationName[] relations)
                    {
                        cmd = new MySqlCommand(Sql.SelectInnerJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op, columnofcondion, ops, relations), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadWithLeftJoin(string tableName1, string tableName2, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new MySqlCommand(Sql.SelectLeftJoin(tableName1, tableName2, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadWithLeftJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new MySqlCommand(Sql.SelectLeftJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadWithLeftJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op, List<column> columnofcondion, OpType[] ops, RelationName[] relations)
                    {
                        cmd = new MySqlCommand(Sql.SelectLeftJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op, columnofcondion, ops, relations), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadWithRightJoin(string tableName1, string tableName2, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new MySqlCommand(Sql.SelectRightJoin(tableName1, tableName2, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadWithRightJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new MySqlCommand(Sql.SelectRightJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadWithRightJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op, List<column> columnofcondion, OpType[] ops, RelationName[] relations)
                    {
                        cmd = new MySqlCommand(Sql.SelectRightJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op, columnofcondion, ops, relations), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadWithFullJoin(string tableName1, string tableName2, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new MySqlCommand(Sql.SelectFulltJoin(tableName1, tableName2, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadWithFullJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new MySqlCommand(Sql.SelectFullJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadWithFullJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op, List<column> columnofcondion, OpType[] ops, RelationName[] relations)
                    {
                        cmd = new MySqlCommand(Sql.SelectFullJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op, columnofcondion, ops, relations), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadWithGroupBy(string tableName, List<column> Columns, List<column> columnForGroupBy)
                    {
                        cmd = new MySqlCommand(Sql.SelectGroupBy(tableName, Columns, columnForGroupBy), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;

                    }
                    public MySqlDataReader ReadWithGroupBy(string[] tablesName, List<column> Columns, List<column> columnForGroupBy)
                    {
                        cmd = new MySqlCommand(Sql.SelectGroupBy(tablesName, Columns, columnForGroupBy), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;

                    }
                    public MySqlDataReader ReadWithGroupBy(string[] tablesName, List<column> Columns, List<column> columnForGroupBy, List<column> HavingconditionColumn, OpType[] ops)
                    {
                        cmd = new MySqlCommand(Sql.SelectGroupBy(tablesName, Columns, columnForGroupBy, HavingconditionColumn, ops), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;

                    }
                    public MySqlDataReader ReadWithGroupBy(string tableName, List<column> Columns, List<column> columnForGroupBy, List<column> HavingconditionColumn, OpType[] ops)
                    {
                        cmd = new MySqlCommand(Sql.SelectGroupBy(tableName, Columns, columnForGroupBy, HavingconditionColumn, ops), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadWithGroupBy(string[] tablesName, List<column> Columns, List<column> columnForGroupBy, List<column> HavingconditionColumn, OpType[] havingops, List<column> WhereConditionColumn, OpType[] whereops)
                    {
                        cmd = new MySqlCommand(Sql.SelectGroupBy(tablesName, Columns, columnForGroupBy, HavingconditionColumn, havingops, WhereConditionColumn, whereops), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadWithGroupBy(string tableName, List<column> Columns, List<column> columnForGroupBy, List<column> HavingconditionColumn, OpType[] havingops, List<column> WhereConditionColumn, OpType[] whereops)
                    {
                        cmd = new MySqlCommand(Sql.SelectGroupBy(tableName, Columns, columnForGroupBy, HavingconditionColumn, havingops, WhereConditionColumn, whereops), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader ReadWithOrderBy(string tableName, List<column> Columns, List<column> columnForGroupBy)
                    {
                        cmd = new MySqlCommand(Sql.SelectOrderBy(tableName, Columns, columnForGroupBy), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader Read(string[] tablesName, List<column> Columns, List<column> columnForOrderBy)
                    {
                        cmd = new MySqlCommand(Sql.SelectOrderBy(tablesName, Columns, columnForOrderBy), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader Read(string tableName, List<column> Columns, List<column> columnForOrderBy, GroupType grouptype)
                    {
                        cmd = new MySqlCommand(Sql.SelectOrderBy(tableName, Columns, columnForOrderBy, grouptype), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public MySqlDataReader Read(string[] tablesName, List<column> Columns, List<column> columnForOrderBy, GroupType grouptype)
                    {
                        cmd = new MySqlCommand(Sql.SelectOrderBy(tablesName, Columns, columnForOrderBy, grouptype), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    #endregion
                    /// <summary>
                    /// reading from database
                    /// </summary>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    ///
                    #region some methods to do some operation on database
                    //Insert 
                    public void Insert(string tableName, List<column> col)
                    {
                        cmd = new MySqlCommand(Sql.Insert(tableName, col), cnx);
                        try
                        {
                            Open();
                            cmd.ExecuteNonQuery();
                            Close();
                        }
                        catch { }

                    }
                    //Delete
                    public void Delete(string tableName, column column, OpType op)
                    {
                        cmd = new MySqlCommand(Sql.Delete(tableName, column, op), cnx);
                        try
                        {
                            Open();
                            cmd.ExecuteNonQuery();
                            Close();
                        }
                        catch { }


                    }
                    public void Delete(string tableName, List<column> columns, OpType[] op, RelationName[] relation)
                    {
                        cmd = new MySqlCommand(Sql.Delete(tableName, columns, op, relation), cnx);
                        try
                        {
                            Open();
                            cmd.ExecuteNonQuery();
                            Close();
                        }
                        catch { }
                    }
                    //Update
                    public void UpdateAll(string tableName, List<column> columns)
                    {
                        cmd = new MySqlCommand(Sql.UpdateALL(tableName, columns), cnx);
                        try
                        {
                            Open();
                            cmd.ExecuteNonQuery();
                            Close();
                        }
                        catch { }
                    }
                    public void Update(string tableName, List<column> columns, OpType[] op, RelationName[] relation)
                    {
                        cmd = new MySqlCommand(Sql.Update(tableName, columns, op, relation), cnx);
                        try
                        {
                            Open();
                            cmd.ExecuteNonQuery();
                            Close();
                        }
                        catch { }
                    }
                    #endregion
                    /// <summary>
                    ///  Fill some windows from Controlds
                    /// </summary>
                    /// <param name="dt"></param>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    /// 
                    #region Fill some windows from Controlds
                    public void FillDataGridViewByOneTable(System.Windows.Forms.DataGridView dt, string tableName)
                    {
                        table = new DataTable();
                        table.Load(ReadAll(tableName));
                        dt.DataSource = table;
                        CloseReading();

                    }
                    public void FillDataGridViewByTowTable(DataGridView dt, string tableName1, string tableName2, string primaryKeyColumnName, string foreignKeyColumnName, OpType op, JoinType jtype)
                    {
                        table = new DataTable();
                        switch (jtype)
                        {
                            case JoinType.INNER: table.Load(ReadWithInnerJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.LEFT: table.Load(ReadWithLeftJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.RIGHT: table.Load(ReadWithRightJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.FULL: table.Load(ReadWithFullJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            default: table = null; break;
                        }

                        CloseReading();
                        dt.DataSource = table;
                    }

                    public void FillComboBoxByOneTable(System.Windows.Forms.ComboBox cb, string tableName)
                    {
                        table = new DataTable();
                        table.Load(ReadAll(tableName));
                        cb.DataSource = table;
                        CloseReading();

                    }
                    public void FillComboBoxByTowTable(ComboBox cb, string tableName1, string tableName2, string primaryKeyColumnName, string foreignKeyColumnName
                                                        , OpType op, JoinType jtype)
                    {
                        table = new DataTable();
                        switch (jtype)
                        {
                            case JoinType.INNER: table.Load(ReadWithInnerJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.LEFT: table.Load(ReadWithLeftJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.RIGHT: table.Load(ReadWithRightJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.FULL: table.Load(ReadWithFullJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            default: table = null; break;
                        }

                        CloseReading();
                        cb.DataSource = table;
                    }

                    public void FillComboBoxByOneTable(System.Windows.Forms.ComboBox cb, string tableName, string displayMemberName, string valueMemberName)
                    {
                        table = new DataTable();
                        table.Load(ReadAll(tableName));
                        cb.DataSource = table;
                        CloseReading();
                        cb.ValueMember = valueMemberName;
                        cb.DisplayMember = displayMemberName;

                    }
                    public void FillComboBoxByTowTable(ComboBox cb, string tableName1, string tableName2, string primaryKeyColumnName, string foreignKeyColumnName,
                                                        OpType op, JoinType jtype, string displayMemberName, string valueMemberName)
                    {
                        table = new DataTable();
                        switch (jtype)
                        {
                            case JoinType.INNER: table.Load(ReadWithInnerJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.LEFT: table.Load(ReadWithLeftJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.RIGHT: table.Load(ReadWithRightJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.FULL: table.Load(ReadWithFullJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            default: table = null; break;
                        }
                        cb.DataSource = table;
                        CloseReading();
                        cb.ValueMember = valueMemberName;
                        cb.DisplayMember = displayMemberName;
                    }
                    #endregion
                }
                public class MySqlServerDeconnectionMode
                {
                    #region fields
                    SqlCodeGenerator code = SqlCodeGenerator.GetInstance();
                    private static MySqlServerDeconnectionMode instance = null;
                    private static object o = new object();
                    private static MySqlConnection cnx;
                    private MySqlCommand cmd;
                    private MySqlDataAdapter adapter;
                    private DataSet set;
                    private DataTable table;
                    private MySqlCommandBuilder build;
                    #endregion
                    #region Class methods
                    private MySqlServerDeconnectionMode() { }
                    public static MySqlServerDeconnectionMode GetInstance(string path)
                    {
                        if (instance == null)
                            instance = new MySqlServerDeconnectionMode();
                        string connectionString = getConnectionString(path);
                        cnx = new MySqlConnection(connectionString);
                        return instance;
                    }
                    private static string getConnectionString(string path)
                    {
                        return File.ReadAllLines(path).ToString();
                    }
                    #endregion
                    #region connection methods
                    private void Open()
                    {
                        if (cnx.State != ConnectionState.Open)
                        {
                            cnx.Open();
                        }
                    }
                    private void Close()
                    {
                        if (cnx.State != ConnectionState.Closed)
                        {
                            cnx.Close();
                        }
                    }
                    #endregion
                    #region methods to do some operation
                    public void FillDataSetByOneTable(DataSet ds, string TableName)
                    {
                        Open();
                        adapter = new MySqlDataAdapter(code.CreateSelect(TableName), cnx);
                        adapter.Fill(ds, TableName);
                        Close();
                    }
                    public void FillDataSetByTowTable(DataSet ds, string TableName, string TableName2, string primarykeyName, string foreignkeyName, OpType op)
                    {
                        Open();
                        adapter = new MySqlDataAdapter(code.SelectInnerJoin(TableName, TableName2, primarykeyName, foreignkeyName, op), cnx);
                        adapter.Fill(ds, TableName);
                        Close();
                    }
                    public void FillDataTabletByOneTable(DataTable dt, string TableName)
                    {
                        Open();
                        adapter = new MySqlDataAdapter(code.CreateSelect(TableName), cnx);
                        adapter.Fill(dt);
                        Close();
                    }
                    public void FillDataTableByTowTable(DataTable ds, string TableName, string TableName2, string primarykeyName, string foreignkeyName, OpType op)
                    {
                        Open();
                        adapter = new MySqlDataAdapter(code.SelectInnerJoin(TableName, TableName2, primarykeyName, foreignkeyName, op), cnx);
                        adapter.Fill(dt);
                        Close();
                    }
                    public void UpdateDataBase(DataSet ds)
                    {
                        adapter = new MySqlDataAdapter();
                        build = new MySqlCommandBuilder(adapter);
                        adapter.Update(ds);

                    }
                    public void UpdateDataBase(DataSet ds, string tableName)
                    {
                        adapter = new MySqlDataAdapter();
                        build = new MySqlCommandBuilder(adapter);
                        adapter.Update(ds, tableName);

                    }
                    public void UpdateDataBase(DataTable dt)
                    {
                        adapter = new MySqlDataAdapter();
                        build = new MySqlCommandBuilder(adapter);
                        adapter.Update(dt);

                    }
                    #endregion
                }

            }
            namespace OleDB
            {
                public class OleDBConnectionMode
                {
                    /// <summary>
                    /// fields
                    /// </summary>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    /// 
                    #region Fields
                    private static access Instance = null;
                    private SqlCodeGenerator Sql = SqlCodeGenerator.GetInstance();
                    private static object o=new object();
                    private static OleDbConnection cnx = null;
                    private static OleDbCommand cmd = null;
                    private static OleDbDataReader read = null;
                    private DataTable table = new DataTable();
                    #endregion
                    /// <summary>
                    /// class methods
                    /// </summary>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    /// 
                    #region class methods
                    private OleDBConnectionMode() { }
                    public static OleDBConnectionMode GetInstance(string path)
                    {
                        lock (o)
                        {
                            if (Instance == null)
                                Instance = new OleDBConnectionMode();
                            string connectionString = getConnectionString(path);
                            cnx = new OleDbConnection(connectionString);
                            return Instance;
                        }
                    }
                    
                    private static string getConnectionString(string path)
                    {
                        string connectionString = File.ReadAllText(path).ToString();
                        return connectionString;
                    }
                    #endregion
                    /// <summary>
                    /// connection methods
                    /// </summary>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    /// 
                    #region connection methods
                    public void Open()
                    {
                        if (cnx.State != ConnectionState.Open)
                        {
                            cnx.Open();
                        }
                    }
                    public void Close()
                    {
                        if (cnx.State != ConnectionState.Closed)
                        {
                            cnx.Close();
                        }
                    }
                    public void CloseReading()
                    {
                        read.Close();
                        cnx.Close();
                    }
                    #endregion
                    /// <summary>
                    /// Read From Data Base
                    /// </summary>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    /// 
                    #region methods to read from db
                    public OleDbDataReader ReadAll(string tableName)
                    {
                        cmd = new OleDbCommand(Sql.CreateSelect(tableName), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadAllLimeted(string tableName, int NbRecord)
                    {
                        cmd = new OleDbCommand(Sql.CreateSelect(tableName, NbRecord, Providers.SQLSERVER), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadAllLimetedAndNoRepete(string tableName, int NbRecord, bool NotRepetedRecord)
                    {
                        cmd = new OleDbCommand(Sql.CreateSelect(tableName, NbRecord, NotRepetedRecord, Providers.SQLSERVER), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadAllAndNoRepete(string tableName, bool NotRepetedRecords)
                    {
                        cmd = new OleDbCommand(Sql.CreateSelect(tableName, NotRepetedRecords), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader Read(string tableName, int nbRecord, Providers prov, List<column> ConditionColumns, OpType[] op)
                    {
                        cmd = new OleDbCommand(Sql.CreateLimtedSelect(tableName, nbRecord, Providers.SQLSERVER, ConditionColumns, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadWithInnerJoin(string tableName1, string tableName2, string primarykeyColumnName, string foreignKeyColumn, OpType op)
                    {
                        cmd = new OleDbCommand(Sql.SelectInnerJoin(tableName1, tableName2, primarykeyColumnName, foreignKeyColumn, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadWithInnerJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new OleDbCommand(Sql.SelectInnerJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadWithInnerJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op, List<column> columnofcondion, OpType[] ops, RelationName[] relations)
                    {
                        cmd = new OleDbCommand(Sql.SelectInnerJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op, columnofcondion, ops, relations), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadWithLeftJoin(string tableName1, string tableName2, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new OleDbCommand(Sql.SelectLeftJoin(tableName1, tableName2, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadWithLeftJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new OleDbCommand(Sql.SelectLeftJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadWithLeftJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op, List<column> columnofcondion, OpType[] ops, RelationName[] relations)
                    {
                        cmd = new OleDbCommand(Sql.SelectLeftJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op, columnofcondion, ops, relations), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadWithRightJoin(string tableName1, string tableName2, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new OleDbCommand(Sql.SelectRightJoin(tableName1, tableName2, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadWithRightJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new OleDbCommand(Sql.SelectRightJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadWithRightJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op, List<column> columnofcondion, OpType[] ops, RelationName[] relations)
                    {
                        cmd = new OleDbCommand(Sql.SelectRightJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op, columnofcondion, ops, relations), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadWithFullJoin(string tableName1, string tableName2, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new OleDbCommand(Sql.SelectFulltJoin(tableName1, tableName2, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadWithFullJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op)
                    {
                        cmd = new OleDbCommand(Sql.SelectFullJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadWithFullJoin(string tableName1, string tableName2, List<column> ColumnsName, string primarykeyColumnName, string foreignKeyColumnName, OpType op, List<column> columnofcondion, OpType[] ops, RelationName[] relations)
                    {
                        cmd = new OleDbCommand(Sql.SelectFullJoin(tableName1, tableName2, ColumnsName, primarykeyColumnName, foreignKeyColumnName, op, columnofcondion, ops, relations), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadWithGroupBy(string tableName, List<column> Columns, List<column> columnForGroupBy)
                    {
                        cmd = new OleDbCommand(Sql.SelectGroupBy(tableName, Columns, columnForGroupBy), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;

                    }
                    public OleDbDataReader ReadWithGroupBy(string[] tablesName, List<column> Columns, List<column> columnForGroupBy)
                    {
                        cmd = new OleDbCommand(Sql.SelectGroupBy(tablesName, Columns, columnForGroupBy), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;

                    }
                    public OleDbDataReader ReadWithGroupBy(string[] tablesName, List<column> Columns, List<column> columnForGroupBy, List<column> HavingconditionColumn, OpType[] ops)
                    {
                        cmd = new OleDbCommand(Sql.SelectGroupBy(tablesName, Columns, columnForGroupBy, HavingconditionColumn, ops), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;

                    }
                    public OleDbDataReader ReadWithGroupBy(string tableName, List<column> Columns, List<column> columnForGroupBy, List<column> HavingconditionColumn, OpType[] ops)
                    {
                        cmd = new OleDbCommand(Sql.SelectGroupBy(tableName, Columns, columnForGroupBy, HavingconditionColumn, ops), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadWithGroupBy(string[] tablesName, List<column> Columns, List<column> columnForGroupBy, List<column> HavingconditionColumn, OpType[] havingops, List<column> WhereConditionColumn, OpType[] whereops)
                    {
                        cmd = new OleDbCommand(Sql.SelectGroupBy(tablesName, Columns, columnForGroupBy, HavingconditionColumn, havingops, WhereConditionColumn, whereops), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadWithGroupBy(string tableName, List<column> Columns, List<column> columnForGroupBy, List<column> HavingconditionColumn, OpType[] havingops, List<column> WhereConditionColumn, OpType[] whereops)
                    {
                        cmd = new OleDbCommand(Sql.SelectGroupBy(tableName, Columns, columnForGroupBy, HavingconditionColumn, havingops, WhereConditionColumn, whereops), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader ReadWithOrderBy(string tableName, List<column> Columns, List<column> columnForGroupBy)
                    {
                        cmd = new OleDbCommand(Sql.SelectOrderBy(tableName, Columns, columnForGroupBy), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader Read(string[] tablesName, List<column> Columns, List<column> columnForOrderBy)
                    {
                        cmd = new OleDbCommand(Sql.SelectOrderBy(tablesName, Columns, columnForOrderBy), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader Read(string tableName, List<column> Columns, List<column> columnForOrderBy, GroupType grouptype)
                    {
                        cmd = new OleDbCommand(Sql.SelectOrderBy(tableName, Columns, columnForOrderBy, grouptype), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    public OleDbDataReader Read(string[] tablesName, List<column> Columns, List<column> columnForOrderBy, GroupType grouptype)
                    {
                        cmd = new OleDbCommand(Sql.SelectOrderBy(tablesName, Columns, columnForOrderBy, grouptype), cnx);
                        try
                        {
                            Open();
                            read = cmd.ExecuteReader();
                        }
                        catch { }
                        return read;
                    }
                    //Insert 
                    public void Insert(string tableName, List<column> col)
                    {
                        cmd = new OleDbCommand(Sql.Insert(tableName, col), cnx);
                        try
                        {
                            Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch { }

                    }
                    //Delete
                    public void Delete(string tableName, column column, OpType op)
                    {
                        cmd = new OleDbCommand(Sql.Delete(tableName, column, op), cnx);
                        try
                        {
                            Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch { }


                    }
                    public void Delete(string tableName, List<column> columns, OpType[] op, RelationName[] relation)
                    {
                        cmd = new OleDbCommand(Sql.Delete(tableName, columns, op, relation), cnx);
                        try
                        {
                            Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch { }
                    }
                    //Update
                    public void UpdateAll(string tableName, List<column> columns)
                    {
                        cmd = new OleDbCommand(Sql.UpdateALL(tableName, columns), cnx);
                        try
                        {
                            Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch { }
                    }
                    public void Update(string tableName, List<column> columns, OpType[] op, RelationName[] relation)
                    {
                        cmd = new OleDbCommand(Sql.Update(tableName, columns, op, relation), cnx);
                        try
                        {
                            Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch { }
                    }
                    #endregion
                    /// <summary>
                    ///   methods to fill some windows from controlrs liike datagridview and combox
                    /// </summary>
                    /// <param name="dt"></param>
                    /// <param name="tableName"></param>
                    /// <returns></returns>
                    /// 
                    #region methods to fill some windows from controlrs liike datagridview and combox
                    public void FillDataGridViewByOneTable(System.Windows.Forms.DataGridView dt, string tableName)
                    {
                        table = new DataTable();
                        table.Load(ReadAll(tableName));
                        dt.DataSource = table;
                        CloseReading();

                    }
                    public void FillDataGridViewByTowTable(DataGridView dt, string tableName1, string tableName2, string primaryKeyColumnName, string foreignKeyColumnName, OpType op, JoinType jtype)
                    {
                        table = new DataTable();
                        switch (jtype)
                        {
                            case JoinType.INNER: table.Load(ReadWithInnerJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.LEFT: table.Load(ReadWithLeftJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.RIGHT: table.Load(ReadWithRightJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.FULL: table.Load(ReadWithFullJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            default: table = null; break;
                        }

                        CloseReading();
                        dt.DataSource = table;
                    }

                    public void FillComboBoxByOneTable(System.Windows.Forms.ComboBox cb, string tableName)
                    {
                        table = new DataTable();
                        table.Load(ReadAll(tableName));
                        cb.DataSource = table;
                        CloseReading();

                    }
                    public void FillComboBoxByTowTable(ComboBox cb, string tableName1, string tableName2, string primaryKeyColumnName, string foreignKeyColumnName, OpType op, JoinType jtype)
                    {
                        table = new DataTable();
                        switch (jtype)
                        {
                            case JoinType.INNER: table.Load(ReadWithInnerJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.LEFT: table.Load(ReadWithLeftJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.RIGHT: table.Load(ReadWithRightJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.FULL: table.Load(ReadWithFullJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            default: table = null; break;
                        }

                        CloseReading();
                        cb.DataSource = table;
                    }

                    public void FillComboBoxByOneTable(System.Windows.Forms.ComboBox cb, string tableName, string displayMemberName, string valueMemberName)
                    {
                        table = new DataTable();
                        table.Load(ReadAll(tableName));
                        cb.DataSource = table;
                        CloseReading();
                        cb.ValueMember = valueMemberName;
                        cb.DisplayMember = displayMemberName;

                    }
                    public void FillComboBoxByTowTable(ComboBox cb, string tableName1, string tableName2, string primaryKeyColumnName, string foreignKeyColumnName, OpType op, JoinType jtype, string displayMemberName, string valueMemberName)
                    {
                        table = new DataTable();
                        switch (jtype)
                        {
                            case JoinType.INNER: table.Load(ReadWithInnerJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.LEFT: table.Load(ReadWithLeftJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.RIGHT: table.Load(ReadWithRightJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            case JoinType.FULL: table.Load(ReadWithFullJoin(tableName1, tableName2, primaryKeyColumnName, foreignKeyColumnName, op)); break;
                            default: table = null; break;
                        }
                        cb.DataSource = table;
                        CloseReading();
                        cb.ValueMember = valueMemberName;
                        cb.DisplayMember = displayMemberName;
                    }
                    #endregion
                }
                public class OleDBDeconnectionMode
                {
                    #region fields
                    SqlCodeGenerator code = SqlCodeGenerator.GetInstance();
                    private static OleDBDeconnectionMode instance = null;
                    private static object o = new object();
                    private static OleDbConnection cnx;
                    private OleDbCommand cmd;
                    private OleDbDataAdapter adapter;
                    private DataSet set;
                    private DataTable table;
                    private OleDbCommandBuilder build;
                    #endregion
                    #region Class methods
                    private OleDBDeconnectionMode() { }
                    public static OleDBDeconnectionMode GetInstance(string path)
                    {
                        if (instance == null)
                            instance = new OleDBDeconnectionMode();
                        string connectionString = getConnectionString(path);
                        cnx = new OleDbConnection(connectionString);
                        return instance;
                    }
                    private static string getConnectionString(string path)
                    {
                        return File.ReadAllLines(path).ToString();
                    }
                    #endregion
                    #region connection methods
                    private void Open()
                    {
                        if (cnx.State != ConnectionState.Open)
                        {
                            cnx.Open();
                        }
                    }
                    private void Close()
                    {
                        if (cnx.State != ConnectionState.Closed)
                        {
                            cnx.Close();
                        }
                    }
                    #endregion
                    #region methods to do some operation
                    public void FillDataSetByOneTable(DataSet ds, string TableName)
                    {
                        Open();
                        adapter = new OleDbDataAdapter(code.CreateSelect(TableName), cnx);
                        adapter.Fill(ds, TableName);
                        Close();
                    }
                    public void FillDataSetByTowTable(DataSet ds, string TableName, string TableName2, string primarykeyName, string foreignkeyName, OpType op)
                    {
                        Open();
                        adapter = new OleDbDataAdapter(code.SelectInnerJoin(TableName, TableName2, primarykeyName, foreignkeyName, op), cnx);
                        adapter.Fill(ds, TableName);
                        Close();
                    }
                    public void FillDataTabletByOneTable(DataTable dt, string TableName)
                    {
                        Open();
                        adapter = new OleDbDataAdapter(code.CreateSelect(TableName), cnx);
                        adapter.Fill(dt);
                        Close();
                    }
                    public void FillDataTableByTowTable(DataTable ds, string TableName, string TableName2, string primarykeyName, string foreignkeyName, OpType op)
                    {
                        Open();
                        adapter = new OleDbDataAdapter(code.SelectInnerJoin(TableName, TableName2, primarykeyName, foreignkeyName, op), cnx);
                        adapter.Fill(dt);
                        Close();
                    }
                    public void UpdateDataBase(DataSet ds)
                    {
                        adapter = new OleDbDataAdapter();
                        build = new OleDbCommandBuilder(adapter);
                        adapter.Update(ds);

                    }
                    public void UpdateDataBase(DataSet ds, string tableName)
                    {
                        adapter = new OleDbDataAdapter();
                        build = new OleDbCommandBuilder(adapter);
                        adapter.Update(ds, tableName);

                    }
                    public void UpdateDataBase(DataTable dt)
                    {
                        adapter = new OleDbDataAdapter();
                        build = new OleDbCommandBuilder(adapter);
                        adapter.Update(dt);

                    }
                    
                    #endregion
                }
            }
        }
}