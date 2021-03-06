﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.SqlParser.SqlCodeGen
{
    public class MySqlCodeGenerator : SqlCodeGeneratorBase
    {
        public MySqlCodeGenerator()
        {
        }

        public override bool WriteColumnExpression(ColumnExpression node)
        {
            if (ResolveNames)
            {
                Expression ex = node.FindDescendant<Expression>();
                WriteNode(ex);
                if (!String.IsNullOrEmpty(node.ColumnReference.ColumnAlias))
                {
                    Writer.Write(" AS {0}", QuoteIdentifier(node.ColumnReference.ColumnAlias));
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool WriteColumnIdentifier(ColumnIdentifier node)
        {
            if (ResolveNames)
            {
                string res = String.Empty;

                if (node.TableReference != null)
                {
                    res += QuoteTableReferenceName(node.TableReference);
                }

                if (res != String.Empty) res += ".";

                if (node.ColumnReference.IsStar)
                {
                    res += "*";
                }
                else
                {
                    res += QuoteIdentifier(node.ColumnReference.ColumnName);
                }

                Writer.Write(res);

                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool WriteTableOrViewName(TableOrViewName node)
        {
            if (ResolveNames)
            {
                Writer.Write(QuoteTableReferenceName(node.TableReference));
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool WriteFunctionIdentifier(FunctionIdentifier node)
        {
            if (ResolveNames)
            {
                Writer.Write(QuoteFunctionReferenceName(node.FunctionReference));
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool WriteTableValuedFunctionCall(TableValuedFunctionCall node)
        {
            if (ResolveNames)
            {
                Writer.Write(QuoteTableReferenceName(node.TableReference));
                return false;
            }
            else
            {
                return true;
            }
        }

        protected override string QuoteIdentifier(string identifier)
        {
            return String.Format("`{0}`", UnquoteIdentifier(identifier));
        }

        private string QuoteTableReferenceName(TableReference tableReference)
        {
            string res = String.Empty;

            if (tableReference != null)
            {
                if (tableReference.Alias == null)
                {
                    if (tableReference.IsUdf || tableReference.IsSubquery || tableReference.IsComputed)
                    {
                        res = QuoteIdentifier(tableReference.Alias);
                    }
                    else
                    {
                        if (tableReference.DatabaseObject != null)
                        {
                            if (tableReference.DatabaseObject.ObjectName != null) res += QuoteIdentifier(tableReference.DatabaseObject.ObjectName);
                        }
                        else
                        {
                            //if (tableReference.DatabaseName != null) res += String.Format("`{0}`.", tableReference.DatabaseName);
                            if (tableReference.DatabaseObjectName != null) res += QuoteIdentifier(tableReference.DatabaseObjectName);
                        }
                    }
                }
                else
                {
                    res = QuoteIdentifier(tableReference.Alias);
                }
            }

            return res;
        }

        private string QuoteFunctionReferenceName(FunctionReference function)
        {
            string res = String.Empty;

            if (!function.IsUdf)
            {
                throw new NotImplementedException();
            }
            else
            {
                if (function.DatabaseObject != null)
                {
                    if (function.DatabaseObject.ObjectName != null) res += QuoteIdentifier(function.DatabaseObject.ObjectName);
                }
                else
                {
                    if (function.DatabaseObjectName != null) res += QuoteIdentifier(function.DatabaseObjectName);
                }
            }

            return res;
        }

        private string QuoteDatabaseObjectName(DatabaseObject dbobject)
        {
            return QuoteIdentifier(dbobject.ObjectName);
        }

        public override string GenerateSelectStarQuery(TableOrView tableOrView, int top)
        {
            string sql = "SELECT t.* FROM {1} AS t {0}";
            return String.Format(sql, GenerateTopExpression(top), QuoteDatabaseObjectName(tableOrView));
        }

        protected override string GenerateTopExpression(int top)
        {
            string topstr = String.Empty;
            if (top != 0)
            {
                topstr = String.Format("LIMIT {0}", top);
            }

            return topstr;
        }

        public override string GenerateMostRestrictiveTableQuery(TableReference table, bool includePrimaryKey, int top)
        {
            // Normalize search conditions and extract where clause
            var cn = new SearchConditionNormalizer();
            cn.NormalizeQuerySpecification(((TableSource)table.Node).QuerySpecification);
            var where = cn.GenerateWhereClauseSpecificToTable(table);

            // Build table specific query
            var sql = new StringWriter();

            sql.Write("SELECT ");

            // Now write the referenced columns
            int q = 0;
            foreach (var cr in table.ColumnReferences.Where(c => c.IsReferenced))
            {
                if (q != 0)
                {
                    sql.Write(", ");
                }

                if (cr.DataType.IsInteger)
                {
                    // Here a cast to a type that is accepted by SQL Server has to be made
                    sql.Write("CAST(`{0}` AS SIGNED) AS `{0}`", cr.ColumnName);
                }
                else
                {
                    sql.Write("`{0}`", cr.ColumnName);
                }
                q++;
            }

            // From cluse
            sql.Write(" FROM `{0}`", table.DatabaseObjectName);
            if (table.Alias != null)
            {
                sql.Write(" `{0}`", table.Alias);
            }
            sql.Write(" ");

            if (where != null)
            {
                Execute(sql, where);
            }

            if (top > 0)
            {
                sql.Write(" LIMIT {0} ", top);
            }

            return sql.ToString();
        }

#if false
        public override string GenerateMostRestrictiveTableQuery(SelectStatement selectStatement, TableReference table, int top)
        {
            // Function assumes that Condition Normalizer has already run on the query
            // *** TODO: check this using flags ^^^^

            var sql = new StringWriter();

            sql.Write("SELECT ");

            // Now write the referenced columns
            int q = 0;
            foreach (var cr in table.ColumnReferences.Where(c => c.IsReferenced))
            {
                if (q != 0)
                {
                    sql.Write(", ");
                }
                sql.Write("`{0}`", cr.ColumnName);
                q++;
            }

            // From cluse
            sql.Write(" FROM `{0}`", table.TableName);
            if (!String.IsNullOrWhiteSpace(table.Alias))
            {
                sql.Write(" `{0}`", table.Alias);
            }

            // Generate the table specific most restictive where clause
            var cnr = new SearchConditionNormalizer();
            cnr.Execute(selectStatement.EnumerateQuerySpecifications().First());        // TODO: what if more than one QS?
            var where = cnr.GenerateWhereClauseSpecificToTable(table);
            if (where != null)
            {
                Execute(sql, where);
            }

            if (top > 0)
            {
                sql.Write(" LIMIT {0} ", top);
            }

            return sql.ToString();
        }
#endif
    }
}
