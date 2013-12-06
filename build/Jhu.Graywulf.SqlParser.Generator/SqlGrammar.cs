﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser.Generator
{
    [Grammar(Namespace = "Jhu.Graywulf.SqlParser", ParserName = "SqlParser",
        Comparer = "StringComparer.InvariantCultureIgnoreCase", RootToken = "SelectStatement")]
    public class SqlGrammar : Grammar
    {

        #region Symbols (matched by value)

        public static Expression<Symbol> Plus = () => @"+";
        public static Expression<Symbol> Minus = () => @"-";
        public static Expression<Symbol> Mul = () => @"*";
        public static Expression<Symbol> Div = () => @"/";
        public static Expression<Symbol> Mod = () => @"%";

        public static Expression<Symbol> BitwiseNot = () => @"~";
        public static Expression<Symbol> BitwiseAnd = () => @"&";
        public static Expression<Symbol> BitwiseOr = () => @"|";
        public static Expression<Symbol> BitwiseXor = () => @"^";

        public static Expression<Symbol> Equals = () => @"=";
        public static Expression<Symbol> Equals2 = () => @"==";
        public static Expression<Symbol> LessOrGreaterThan = () => @"<>";
        public static Expression<Symbol> NotEquals = () => @"!=";
        public static Expression<Symbol> NotLessThan = () => @"!<";
        public static Expression<Symbol> NotGreaterThan = () => @"!>";
        public static Expression<Symbol> LessOrEqualThan = () => @"<=";
        public static Expression<Symbol> GreaterOrEqualThan = () => @">=";
        public static Expression<Symbol> LessThan = () => @"<";
        public static Expression<Symbol> GreaterThan = () => @">";

        public static Expression<Symbol> DoubleColon = () => @"::";
        public static Expression<Symbol> Dot = () => @".";
        public static Expression<Symbol> Comma = () => @",";
        public static Expression<Symbol> Semicolon = () => @";";
        public static Expression<Symbol> Colon = () => @":";

        public static Expression<Symbol> BracketOpen = () => @"(";
        public static Expression<Symbol> BracketClose = () => @")";
        public static Expression<Symbol> VectorOpen = () => @"{";
        public static Expression<Symbol> VectorClose = () => @"}";


        #endregion
        #region Terminals (matched by regular expressions)

        public static Expression<Whitespace> Whitespace = () => @"\G\s+";
        public static Expression<Comment> SingleLineComment = () => @"\G--.*";
        public static Expression<Comment> MultiLineComment = () => @"\G(?sm)/\*.*?\*/";
        public static Expression<Terminal> Number = () => @"\G([0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)";
        public static Expression<Terminal> StringConstant = () => @"\G('([^']|'')*')";
        public static Expression<Terminal> Identifier = () => @"\G([a-zA-Z_]+[0-9a-zA-Z_]*|\[[^\]]+\])";
        public static Expression<Terminal> Variable = () => @"\G(@[$a-zA-Z_]+)";
        public static Expression<Terminal> SystemVariable = () => @"\G(@@[$a-zA-Z_]+)";

        #endregion
        #region Arithmetic operators used in expressions

        public static Expression<Rule> UnaryOperator = () =>
            Must(Plus, Minus, BitwiseNot);
        public static Expression<Rule> ArithmeticOperator = () =>
            Must(Plus, Minus, Mul, Div, Mod);
        public static Expression<Rule> BitwiseOperator = () =>
            Must(BitwiseAnd, BitwiseOr, BitwiseXor);
        public static Expression<Rule> ComparisonOperator = () =>
            Must(Equals2, Equals, LessOrGreaterThan, NotEquals,
            NotLessThan, NotGreaterThan, LessOrEqualThan, GreaterOrEqualThan,
            LessThan, GreaterThan);
        #endregion
        #region Logical operators used in search conditions

        public static Expression<Rule> LogicalNot = () => Keyword("NOT");
        public static Expression<Rule> LogicalOperator = () => Must(Keyword("AND"), Keyword("OR"));

        #endregion
        #region Identifiers

        public static Expression<Rule> DatasetName = () => Identifier;
        public static Expression<Rule> DatabaseName = () => Identifier;
        public static Expression<Rule> SchemaName = () => Identifier;
        public static Expression<Rule> TableName = () => Identifier;
        public static Expression<Rule> DerivedTable = () => Identifier;
        public static Expression<Rule> TableAlias = () => Identifier;
        public static Expression<Rule> FunctionName = () => Identifier;
        public static Expression<Rule> ColumnName = () => Identifier;
        public static Expression<Rule> ColumnAlias = () => Identifier;
        public static Expression<Rule> ColumnPosition = () => Number;
        public static Expression<Rule> UdtColumnName = () => Identifier;
        public static Expression<Rule> PropertyName = () => Identifier;
        public static Expression<Rule> SampleNumber = () => Number;
        public static Expression<Rule> RepeatSeed = () => Number;
        public static Expression<Rule> IndexValue = () => Identifier;

        #endregion
        #region Arithemtic expressions

        public static Expression<Rule> CommentOrWhitespace = () =>
            Sequence
            (
                Must(MultiLineComment, SingleLineComment, Whitespace),
                May(CommentOrWhitespace)
            );

        public static Expression<Rule> Expression = () =>
            Sequence
            (
                Must
                (
                    ExpressionBrackets,
                    FunctionCall,
                    Sequence(UnaryOperator, May(CommentOrWhitespace), Number),
                    Sequence(UnaryOperator, May(CommentOrWhitespace), AnyVariable),
                    Number,
                    AnyVariable,
                    StringConstant,
                    SimpleCaseExpression,
                    SearchedCaseExpression
                ),
                May
                (
                    Must
                    (
                        Sequence(May(CommentOrWhitespace), ArithmeticOperator, May(CommentOrWhitespace), Expression),
                        Sequence(May(CommentOrWhitespace), BitwiseOperator, May(CommentOrWhitespace), Expression)
                    )
                )
            );

        public static Expression<Rule> ExpressionBrackets = () =>
            Sequence(BracketOpen, May(CommentOrWhitespace), Expression, May(CommentOrWhitespace), BracketClose);

        public static Expression<Rule> AnyVariable = () =>
            Must(ColumnIdentifier, SystemVariable, Variable);

        #endregion
        #region Case-When constructs *** TODO: test, especially whitespaces

        public static Expression<Rule> SimpleCaseExpression = () =>
            Sequence
            (
                Keyword("CASE"),
                May(CommentOrWhitespace),
                Expression,
                May(CommentOrWhitespace),
                SimpleCaseWhenList,
                May(CaseElse),
                Keyword("END")
            );

        public static Expression<Rule> SimpleCaseWhenList = () =>
            Sequence(SimpleCaseWhen, May(SimpleCaseWhenList));

        // *** TODO: add whitespaces
        public static Expression<Rule> SimpleCaseWhen = () =>
            Sequence
            (
                Keyword("WHEN"),
                Expression,
                Keyword("THEN"),
                Expression
            );

        // *** TODO: add whitespaces
        public static Expression<Rule> SearchedCaseExpression = () =>
            Sequence
            (
                Keyword("CASE"),
                SearchedCaseWhenList,
                May(CaseElse),
                Keyword("END")
            );

        public static Expression<Rule> SearchedCaseWhenList = () =>
            Sequence(SearchedCaseWhen, May(SearchedCaseWhenList));

        // *** TODO: add whitespaces
        public static Expression<Rule> SearchedCaseWhen = () =>
            Sequence
            (
                Keyword("WHEN"),
                //SearchCondition,
                Keyword("THEN"),
                Expression
            );

        // *** TODO: add whitespaces
        public static Expression<Rule> CaseElse = () =>
            Sequence
            (
                Keyword("ELSE"),
                Expression
            );

        #endregion
        #region Table and column names

        public static Expression<Rule> TableOrViewName = () =>
            Sequence
            (
                // Dataset prefix
                May(Sequence(DatasetName, May(CommentOrWhitespace), Colon, May(CommentOrWhitespace))),
                // Standard table name
                Must
                (
                    Sequence(DatabaseName, May(CommentOrWhitespace), Dot, May(Sequence(May(CommentOrWhitespace), SchemaName)), May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), TableName),
                    Sequence(SchemaName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), TableName),
                    TableName
                )
            );

        public static Expression<Rule> ColumnIdentifier = () =>
            Must
            (
                Sequence
                (
                // Optional dataset prefix
                    May(Sequence(DatasetName, May(CommentOrWhitespace), Colon, May(CommentOrWhitespace))),
                // Original column name syntax
                    Must
                    (
                        Sequence(DatabaseName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), May(Sequence(SchemaName, May(CommentOrWhitespace))), Dot, May(CommentOrWhitespace), TableName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), Must(Mul, ColumnName)),
                        Sequence(SchemaName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), TableName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), Must(Mul, ColumnName)),
                        Sequence(TableName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), Must(Mul, ColumnName))
                    )
                ),
                Must(Mul, ColumnName)
            );

        #endregion
        #region Function call syntax

        public static Expression<Rule> FunctionIdentifier = () => Must(UdfIdentifier, FunctionName);

        // *** TODO: maybe add dataset support here to be able to call mydb functions?
        public static Expression<Rule> UdfIdentifier = () =>
            Sequence
            (
                May(Sequence(DatasetName, May(CommentOrWhitespace), Comma, May(CommentOrWhitespace))),
                Must
                (
                    Sequence(DatabaseName, May(CommentOrWhitespace), Dot, May(Sequence(May(CommentOrWhitespace), SchemaName)), May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), FunctionName),
                    Sequence(SchemaName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), FunctionName)
                )
            );
        //CHANGED, ADDED ::
        public static Expression<Rule> UdtFunctionIdentifier = () =>
            Sequence
            (
                Variable,
                May(CommentOrWhitespace),
                Must(Dot, DoubleColon),        // Need to add :: ?
                May(CommentOrWhitespace),
                FunctionName
            );

        public static Expression<Rule> Argument = () => Expression;

        public static Expression<Rule> ArgumentList = () =>
            Sequence
            (
                May(CommentOrWhitespace),
                Argument,
                May(Sequence(May(CommentOrWhitespace), Comma, ArgumentList))
            );

        public static Expression<Rule> FunctionCall = () =>
            Sequence
            (
                FunctionIdentifier,
                May(CommentOrWhitespace),
                BracketOpen,
                May(ArgumentList),
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> TableValuedFunctionCall = () =>
            Sequence
            (
                FunctionIdentifier,
                May(CommentOrWhitespace),
                BracketOpen,
                May(ArgumentList),
                May(CommentOrWhitespace),
                BracketClose
            );

        #endregion
        #region Select statement and query expressions (combinations of selects)

        public static Expression<Rule> SelectStatement = () =>
            Sequence
            (
                DeclareStatement
            );
        public static Expression<Rule> SelectStatement2 = () =>
            Sequence
            (
                May(CommentOrWhitespace),   // remove this when wrapped in generic statement grammar
                QueryExpression,
                May(CommentOrWhitespace),
                May(OrderByClause),
                May(CommentOrWhitespace)   // remove this when wrapped in generic statement grammar
            );

        public static Expression<Rule> QueryExpression = () =>
            Sequence
            (
                Must
                (
                    Sequence(BracketOpen, May(CommentOrWhitespace), QueryExpression, May(CommentOrWhitespace), BracketClose),
                    QuerySpecification
                ),
                May(Sequence(May(CommentOrWhitespace), QueryOperator, May(CommentOrWhitespace), QueryExpression))
            );

        public static Expression<Rule> QueryOperator = () =>
            Must
            (
                Sequence(Keyword("UNION"), May(Sequence(CommentOrWhitespace, Keyword("ALL")))),
                Keyword("EXCEPT"),
                Keyword("INTERSECT")
            );

        public static Expression<Rule> QuerySpecification = () =>
            Sequence
            (
                Keyword("SELECT"),
                May(Sequence(CommentOrWhitespace, Must(Keyword("ALL"), CommentOrWhitespace, Keyword("DISTINCT")))),
                May(Sequence(CommentOrWhitespace, TopExpression)),
                CommentOrWhitespace, SelectList,
                May(Sequence(CommentOrWhitespace, IntoClause)),
                May(Sequence(CommentOrWhitespace, FromClause)),
                May(Sequence(CommentOrWhitespace, WhereClause)),
                May(Sequence(CommentOrWhitespace, GroupByClause)),
                May(Sequence(CommentOrWhitespace, HavingClause))
            );

        #endregion
        #region Top expression

        public static Expression<Rule> TopExpression = () =>
            Sequence
            (
                Keyword("TOP"),
                CommentOrWhitespace,
                Expression,
                May(Sequence(CommentOrWhitespace, Keyword("PERCENT"))),
                May(Sequence(CommentOrWhitespace, Keyword("WITH"), CommentOrWhitespace, Keyword("TIES")))
            );

        #endregion
        #region Select list and column expression

        public static Expression<Rule> SelectList = () =>
            Sequence
            (
                ColumnExpression,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), SelectList))
            );

        public static Expression<Rule> ColumnExpression = () =>
            Must
            (
                Sequence(ColumnAlias, May(CommentOrWhitespace), Equals, May(CommentOrWhitespace), Expression),
                Sequence(Expression, May(Sequence(May(Sequence(CommentOrWhitespace, Keyword("AS"))), CommentOrWhitespace, ColumnAlias)))
            );

        #endregion
        #region Into clause

        public static Expression<Rule> IntoClause = () => Sequence(Keyword("INTO"), CommentOrWhitespace, TableOrViewName);

        #endregion
        #region From clause, table sources and joins

        public static Expression<Rule> FromClause = () =>
            Sequence(Keyword("FROM"), May(CommentOrWhitespace), TableSourceExpression);

        public static Expression<Rule> TableSourceExpression = () =>
            Sequence(TableSource, May(Sequence(May(CommentOrWhitespace), JoinedTable)));

        public static Expression<Rule> JoinedTable = () =>
            Sequence
            (
                Must
                (
                    Sequence(JoinType, May(CommentOrWhitespace), TableSource, May(CommentOrWhitespace), Keyword("ON"), May(CommentOrWhitespace), SearchCondition),
                    Sequence(Keyword("CROSS"), CommentOrWhitespace, Keyword("JOIN"), May(CommentOrWhitespace), TableSource),
                    Sequence(Comma, May(CommentOrWhitespace), TableSource),
                    Sequence(Must(Keyword("CROSS"), Keyword("OUTER")), CommentOrWhitespace, Keyword("APPLY"), May(CommentOrWhitespace), TableSource)
                ),
                May(Sequence(May(CommentOrWhitespace), JoinedTable))
            );

        public static Expression<Rule> JoinType = () =>
            Sequence
            (
                May
                (
                    Sequence
                    (
                        Must
                        (
                            Keyword("INNER"),
                            Sequence(Must(Keyword("LEFT"), Keyword("RIGHT"), Keyword("FULL")), May(Sequence(CommentOrWhitespace, Keyword("OUTER"))))
                        ),
                        May(CommentOrWhitespace),
                        May(JoinHint)
                    )
                ),
                May(CommentOrWhitespace),
                Keyword("JOIN")
            );

        public static Expression<Rule> JoinHint = () =>
            Must
            (
                Keyword("LOOP"), Keyword("HASH"), Keyword("MERGE"), Keyword("REMOTE")
            );

        public static Expression<Rule> TableSource = () =>
            Must
            (
                FunctionTableSource,
                SimpleTableSource,
                VariableTableSource,
                SubqueryTableSource
            );

        public static Expression<Rule> SimpleTableSource = () =>
            Sequence
            (
                TableOrViewName,
                May(Sequence(CommentOrWhitespace, May(Sequence(Keyword("AS"), CommentOrWhitespace)), TableAlias)),   // Optional
                May(Sequence(CommentOrWhitespace, TableSampleClause)),
                May(Sequence(CommentOrWhitespace, TableHintClause)),
                May(Sequence(CommentOrWhitespace, TablePartitionClause))
            );

        public static Expression<Rule> FunctionTableSource = () =>
            Sequence
            (
                TableValuedFunctionCall,
                May(CommentOrWhitespace),
                May(Sequence(Keyword("AS"), CommentOrWhitespace)),
                TableAlias,     // Required
                May(Sequence(May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace), ColumnAliasList, May(CommentOrWhitespace), BracketClose))
            );

        public static Expression<Rule> VariableTableSource = () =>
            Sequence
            (
                Variable,
                May(Sequence(CommentOrWhitespace, May(Sequence(Keyword("AS"), CommentOrWhitespace)), TableAlias))   // Optional
            );

        public static Expression<Rule> SubqueryTableSource = () =>
            Sequence
            (
                Subquery,
                May(CommentOrWhitespace),
                May(Sequence(Keyword("AS"), CommentOrWhitespace)),
                TableAlias     // Required
            );

        public static Expression<Rule> ColumnAliasList = () =>
            Sequence(May(CommentOrWhitespace), ColumnAlias, May(Sequence(May(CommentOrWhitespace), Comma, ColumnAliasList)));

        public static Expression<Rule> TableSampleClause = () =>
            Sequence
            (
                Keyword("TABLESAMPLE"),
                May(Sequence(CommentOrWhitespace, Keyword("SYSTEM"))),
                May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace),
                SampleNumber,
                May(Sequence(CommentOrWhitespace, May(Must(Keyword("PERCENT"), Keyword("ROWS"))))),
                May(CommentOrWhitespace), BracketClose,
                May(Sequence
                (
                    CommentOrWhitespace,
                    Keyword("REPEATABLE"),
                    May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace),
                    RepeatSeed,
                    May(CommentOrWhitespace), BracketClose)
                )
            );

        public static Expression<Rule> TableHintClause = () =>
            Sequence
            (
                Keyword("WITH"),
                May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace),
                May(Sequence(Keyword("NOEXPAND"), CommentOrWhitespace)),
                TableHintList,
                May(CommentOrWhitespace), BracketClose
            );

        public static Expression<Rule> TableHintList = () =>
            Sequence
            (
                TableHint,
                Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), TableHintList)
            );

        public static Expression<Rule> TableHint = () =>
            Must(
                Sequence(Keyword("INDEX"), May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace), IndexValueList, May(CommentOrWhitespace), BracketClose),
                Keyword("FASTFIRSTROW"),
                Keyword("HOLDLOCK"),
                Keyword("NOLOCK"),
                Keyword("NOWAIT"),
                Keyword("PAGLOCK"),
                Keyword("READCOMMITTED"),
                Keyword("READCOMMITTEDLOCK"),
                Keyword("READPAST"),
                Keyword("READUNCOMMITTED"),
                Keyword("REPEATABLEREAD"),
                Keyword("ROWLOCK"),
                Keyword("SERIALIZABLE"),
                Keyword("TABLOCK"),
                Keyword("TABLOCKX"),
                Keyword("UPDLOCK"),
                Keyword("XLOCK")
            );

        public static Expression<Rule> IndexValueList = () =>
            Sequence
            (
                IndexValue,
                Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), IndexValueList)
            );

        public static Expression<Rule> Subquery = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                SelectStatement,            // Order by is also allowed here with top!
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> TablePartitionClause = () =>
            Sequence
            (
                Keyword("PARTITION"), CommentOrWhitespace, Keyword("ON"),
                CommentOrWhitespace,
                ColumnIdentifier
            );

        #endregion
        #region Where clause, search conditions and predicates

        public static Expression<Rule> WhereClause = () =>
            Sequence
            (
                Keyword("WHERE"),
                May(CommentOrWhitespace),
                SearchCondition
            );

        public static Expression<Rule> SearchCondition = () =>
            Sequence
            (
                May(LogicalNot),
                May(CommentOrWhitespace),
                Must(Predicate, SearchConditionBrackets),
                May(Sequence(May(CommentOrWhitespace), LogicalOperator, May(CommentOrWhitespace), SearchCondition))
            );

        public static Expression<Rule> SearchConditionBrackets = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                SearchCondition,
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> Predicate = () =>
            Must
            (
                // Expression comparieson
                Sequence
                (
                    Expression,
                    May(CommentOrWhitespace),
                    ComparisonOperator,
                    May(CommentOrWhitespace),
                    Expression
                ),
                // Like
                Sequence
                (
                    Expression,
                    May(Keyword("NOT")),
                    May(CommentOrWhitespace),
                    Keyword("LIKE"),
                    May(CommentOrWhitespace),
                    Expression,
                    May(Sequence(May(CommentOrWhitespace), Keyword("ESCAPE"), May(CommentOrWhitespace), Expression))
                ),
                // Between
                Sequence
                (
                    Expression,
                    May(Sequence(May(CommentOrWhitespace), Keyword("NOT"))),
                    May(CommentOrWhitespace),
                    Keyword("BETWEEN"),
                    May(CommentOrWhitespace),
                    Expression,
                    May(CommentOrWhitespace),
                    Keyword("AND"),
                    May(CommentOrWhitespace),
                    Expression
                ),
                // IS NULL
                Sequence
                (
                    Expression,
                    May(CommentOrWhitespace),
                    Keyword("IS"),
                    May(Sequence(CommentOrWhitespace, Keyword("NOT"))),
                    CommentOrWhitespace,
                    Keyword("NULL")
                ),
                // IN - semi join
                Sequence
                (
                    Expression,
                    May(Keyword("NOT")),
                    May(CommentOrWhitespace),
                    Keyword("IN"),
                    May(CommentOrWhitespace),
                    Must
                    (
                        Subquery,
                        Sequence(BracketOpen, May(CommentOrWhitespace), ArgumentList, May(CommentOrWhitespace), BracketClose)
                    )
                ),
                // comparision semi join
                Sequence
                (
                    Expression,
                    May(CommentOrWhitespace),
                    ComparisonOperator,
                    May(CommentOrWhitespace),
                    Must(Keyword("ALL"), Keyword("SOME"), Keyword("ANY")),
                    May(CommentOrWhitespace),
                    Subquery
                ),
                // EXISTS
                Sequence
                (
                    Keyword("EXISTS"),
                    May(CommentOrWhitespace),
                    Subquery
                ),
                Sequence
                (
                    Keyword("FREETEXT"),
                    May(CommentOrWhitespace),
                    BracketOpen,
                    May(CommentOrWhitespace),
                    Must
                    (
                        ColumnName,
                        Sequence(May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace), ColumnNameList, May(CommentOrWhitespace), BracketClose),
                        Mul
                    ),
                    May(CommentOrWhitespace),
                    Comma,
                    May(CommentOrWhitespace),
                    Must(StringConstant, Variable),
                    May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), Keyword("LANGUAGE"), CommentOrWhitespace, Must(Number, StringConstant))),
                    May(CommentOrWhitespace),
                    BracketClose
                )
                // *** TODO: add string constructs (contains, freetext etc.)
            );

        #endregion
        #region Group by clause

        public static Expression<Rule> GroupByClause = () =>
            Sequence
            (
                Keyword("GROUP"),
                CommentOrWhitespace,
                Keyword("BY"),
                Must
                (
                    Sequence(CommentOrWhitespace, Keyword("ALL")),
                    Sequence(May(CommentOrWhitespace), GroupByList)
                )
            );

        public static Expression<Rule> GroupByList = () =>
            Sequence
            (
                Expression,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), GroupByList))
            );

        #endregion
        #region Having clause

        public static Expression<Rule> HavingClause = () =>
            Sequence
            (
                Keyword("HAVING"),
                May(CommentOrWhitespace),
                SearchCondition
            );

        #endregion
        #region Order by clause

        public static Expression<Rule> OrderByClause = () =>
            Sequence
            (
                Keyword("ORDER"),
                CommentOrWhitespace,
                Keyword("BY"),
                May(CommentOrWhitespace),
                OrderByList
            );

        public static Expression<Rule> OrderByList = () =>
            Sequence
            (
                Must
                (
                    Expression,
                    Sequence(ColumnPosition, May(Sequence(May(CommentOrWhitespace), Must(Keyword("ASC"), Keyword("DESC")))))
                ),
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), OrderByList))
            );

        #endregion


        #region Set statement

        public static Expression<Symbol> AddAssignment = () => @"+=";
        public static Expression<Symbol> SubAssignment = () => @"-=";
        public static Expression<Symbol> MulAssignment = () => @"*=";
        public static Expression<Symbol> DivAssignment = () => @"/=";
        public static Expression<Symbol> ModAssignment = () => @"%=";
        public static Expression<Symbol> AndAssignment = () => @"&=";
        public static Expression<Symbol> OrAssignment = () => @"|=";
        public static Expression<Symbol> XorAssignment = () => @"^=";

        public static Expression<Rule> CompoundOperator = () =>
            Must(AddAssignment, SubAssignment, MulAssignment, DivAssignment,
                 ModAssignment, AndAssignment, OrAssignment, OrAssignment);

        public static Expression<Rule> SetStatement = () =>
            Sequence
            (
                May(CommentOrWhitespace),
                Keyword("SET"),
                CommentOrWhitespace,
                Must
                (
                    Sequence(Variable, May(VariableProperty), May(CommentOrWhitespace), Equals, May(CommentOrWhitespace), Must(UdtFunctionIdentifier, Expression, QueryExpression)),
                    Sequence(Variable, May(CommentOrWhitespace), CompoundOperator, May(CommentOrWhitespace), Expression),
                    Sequence(Must(Variable, UdtFunctionIdentifier), May(CommentOrWhitespace))
                // @SQLCLR_local_variable.mutator_method        
                )
            );
        public static Expression<Rule> VariableProperty = () =>
            Must(
                Sequence(May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), Identifier)                           // nem tokeletes mivel ha a jobb oldalon is @p.X van akkor nem ismeri fel expressionkent
            );
        #endregion
        #region Delete statement
        public static Expression<Rule> DeleteStatement = () =>
            Sequence
            (
                May(Sequence(May(CommentOrWhitespace), WithStatement)),
                May(CommentOrWhitespace),
                Keyword("DELETE"),
                May(Sequence(CommentOrWhitespace, TopExpression)),
                May(Sequence(CommentOrWhitespace, Keyword("FROM"))),
                Must(Sequence(CommentOrWhitespace, Must(TableOrViewName, TableAlias))),             //object or rowset_function_limited  [ WITH ( table_hint_limited [ ...n ] ) ]
                May(Sequence(CommentOrWhitespace, FromClause)),
                May(Sequence(CommentOrWhitespace, WhereClause)),
                May(Sequence(CommentOrWhitespace, TableHint))
            );
        #endregion
        #region Update statement
        public static Expression<Rule> UpdateStatement = () =>
            Sequence
            (
                May(Sequence(May(CommentOrWhitespace), WithStatement)),
                May(CommentOrWhitespace),
                Keyword("UPDATE"),
                May(Sequence(CommentOrWhitespace, TopExpression)),
                Must(Sequence(CommentOrWhitespace, Must(TableOrViewName, TableAlias), May(Sequence(May(CommentOrWhitespace), WithStatement)))),    // or rowset function limited or table_variable
                CommentOrWhitespace, Keyword("SET"),
                SetExpression,
                May(Sequence(CommentOrWhitespace, FromClause)),
                May(Sequence(CommentOrWhitespace, WhereClause))
            );
        public static Expression<Rule> SetExpression = () =>
            Sequence(
                 Must(
                    SetExpression1,
                    SetExpression2,
                    SetExpression3,
                    SetExpression4,
                    SetExpression5,
                    SetExpression6
                ),
                May(
                    Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), SetExpression)
                )
            );
        public static Expression<Rule> SetExpression1 = () =>
            Sequence
            (
                May(CommentOrWhitespace), ColumnIdentifier, May(CommentOrWhitespace), Equals, May(CommentOrWhitespace), Must(Expression, Keyword("DEFAULT"), Keyword("NULL"))
            );
        public static Expression<Rule> SetExpression2 = () =>
            Sequence
            (
                May(CommentOrWhitespace),
                UdtColumnNameArgument,
                Must
                (
                    Sequence(May(CommentOrWhitespace), Equals, Expression),
                    Sequence(May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace), ArgumentList, May(CommentOrWhitespace), BracketClose)
                )
            );
        public static Expression<Rule> UdtColumnNameArgument = () =>
            Sequence(UdtColumnName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), Identifier);

        public static Expression<Rule> SetExpression3 = () =>
            Sequence
            (
                May(CommentOrWhitespace), ColumnIdentifier, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), Keyword("WRITE"), May(CommentOrWhitespace),
                BracketOpen, May(CommentOrWhitespace), WriteArgumentList, May(CommentOrWhitespace), BracketClose
            );
        public static Expression<Rule> WriteArgumentList = () =>
            Sequence
            (
                May(CommentOrWhitespace), Argument, May(CommentOrWhitespace), Comma,
                May(CommentOrWhitespace), Argument, May(CommentOrWhitespace), Comma,
                May(CommentOrWhitespace), Argument
            );

        public static Expression<Rule> SetExpression4 = () =>
            Sequence
            (
                May(CommentOrWhitespace), Variable, May(CommentOrWhitespace), Equals, May(CommentOrWhitespace), Expression
            );
        public static Expression<Rule> SetExpression5 = () =>
            Sequence
            (
                May(CommentOrWhitespace), Variable, May(CommentOrWhitespace), Equals,
                May(CommentOrWhitespace), ColumnIdentifier, May(CommentOrWhitespace), Must(Equals, CompoundOperator), May(CommentOrWhitespace), Expression
            );
        public static Expression<Rule> SetExpression6 = () =>
            Sequence
            (
                May(CommentOrWhitespace), Must(Variable, ColumnIdentifier), May(CommentOrWhitespace), CompoundOperator, May(CommentOrWhitespace), Expression
            );

        #endregion
        #region With statement

        public static Expression<Rule> WithStatement = () =>
            Sequence
            (
                May(CommentOrWhitespace),
                Keyword("WITH"),
                CommentOrWhitespace,
                CommonTableExpression
            );

        public static Expression<Rule> CommonTableExpression = () =>
            Sequence
            (
                CommonTableExpressionName,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), CommonTableExpression))

            );
        public static Expression<Rule> CommonTableExpressionName = () =>                                                                //maybe need to rename this rule because its not concrete
           Sequence
           (
               TableAlias,//expression_name
               May(Sequence(May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace), ColumnAliasList, May(CommentOrWhitespace), BracketClose)),
               CommentOrWhitespace,
               Keyword("AS"),
               May(CommentOrWhitespace),
               BracketOpen,
               May(CommentOrWhitespace),
               SelectStatement2,
               //Sequence(QuerySpecification,May(Sequence(May(CommentOrWhitespace), QueryOperator, May(CommentOrWhitespace), QueryExpression))),
               May(CommentOrWhitespace),
               BracketClose
           );
        #endregion
        #region Insert statement

        public static Expression<Rule> InsertStatement = () =>
            Sequence
            (
                May(CommentOrWhitespace),
                May(Sequence((WithStatement), CommentOrWhitespace)),
                Keyword("INSERT"),
                CommentOrWhitespace,
                May(TopExpression),
                May(Sequence(Keyword("INTO"), CommentOrWhitespace)),

                Sequence(TableOrViewName, May(CommentOrWhitespace), May(WithStatement)),

                May
                (
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        BracketOpen,
                        May(CommentOrWhitespace),
                        ColumnAliasList,
                        May(CommentOrWhitespace),
                        BracketClose
                    )
                ),


                May(CommentOrWhitespace),
                Must(
                    Sequence(Keyword("DEFAULT"), CommentOrWhitespace, Keyword("VALUES")),
                    Sequence(Keyword("VALUES"), ListColumnValueList),
                    SelectStatement2                                                            //NEED TO MODIFY TO SELECTSTATMENT FROM SELECTSTATEMENT2
                )
            );

        public static Expression<Rule> ColumnValueList = () =>
            Sequence(
                May(CommentOrWhitespace),
                Must(Expression, Keyword("NULL"), Keyword("DEFAULT")),
                May(Sequence(May(CommentOrWhitespace), Comma, ColumnValueList))
            );
        public static Expression<Rule> ListColumnValueList = () =>
            Sequence(
                Sequence(May(CommentOrWhitespace), BracketOpen, ColumnValueList, May(CommentOrWhitespace), BracketClose),
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), ListColumnValueList))
            );

        #endregion
        #region Declare statement



        public static Expression<Rule> DataTypeIdentifier = () => Identifier;
        public static Expression<Rule> ScalarDataTypeIdentifier = () => Identifier;
        public static Expression<Rule> VariableAlias = () => Identifier;
        public static Expression<Rule> VariableValue = () => Identifier;

        public static Expression<Rule> DeclareStatement = () =>
            Sequence
            (
                Keyword("DECLARE"),
                CommentOrWhitespace,
                Must
                (
                    LocalVariableSource,
                    Sequence(VariableTableSource, CommentOrWhitespace, TableTypeDefinition)
                )
            );

        public static Expression<Rule> LocalVariableSource = () =>
            Sequence
            (
                May(CommentOrWhitespace),
                Variable,
                May(Sequence(May(CommentOrWhitespace), Keyword("AS"), CommentOrWhitespace, VariableAlias)),
                CommentOrWhitespace,
                DataTypeIdentifier,
                May(Sequence(May(CommentOrWhitespace), Equals, May(CommentOrWhitespace), VariableValue)),
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), LocalVariableSource))
            );
        public static Expression<Rule> TableTypeDefinition = () =>
            Sequence
            (
                Keyword("TABLE"),
                May(CommentOrWhitespace),
                BracketOpen,
                May(CommentOrWhitespace),
                TableTypeDefinitionType,
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> TableTypeDefinitionType = () =>
            Sequence
            (
                Must
                (
                    ColumnDefinition,
                    TableConstraint
                ),
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), TableTypeDefinitionType))
            );
        public static Expression<Rule> TableConstraint = () =>
            Sequence
            (
                Must
                (
                    Sequence
                    (
                        Must
                        (
                            Sequence(May(CommentOrWhitespace), Keyword("PRIMARY"), CommentOrWhitespace, Keyword("KEY")),
                            Sequence(May(CommentOrWhitespace), Keyword("UNIQUE"))
                        ),
                        May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace), ColumnAliasList, May(CommentOrWhitespace), BracketClose
                    ),
                    Sequence(May(CommentOrWhitespace), Keyword("CHECK"), May(CommentOrWhitespace), SearchConditionBrackets)                                 //logical expression not implemented yet
                )
            );
        public static Expression<Rule> ColumnDefinition = () =>
            Sequence
            (
                ColumnName,
                CommentOrWhitespace,
                ScalarDataTypeIdentifier,                                                                                                           //may AS computed column expression
                //May(Sequence(Keyword("COLLATE"),CommentOrWhitespace,CollationDefinition))                                                         //  is this important????
                May
                (
                    Must
                    (
                        Sequence(May(CommentOrWhitespace), Keyword("DEFAULT"), CommentOrWhitespace, ConstantExpression),
                        Sequence
                        (
                            May(CommentOrWhitespace),
                            Keyword("IDENTITY"),
                            May(Sequence(May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace), Number, May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), Number, May(CommentOrWhitespace), BracketClose))
                        )
                    )
                ),
                May(Sequence(May(CommentOrWhitespace), Keyword("ROWGUIDCOL"))),
                May(Sequence(May(CommentOrWhitespace), ColumnConstraint))
            );

        public static Expression<Rule> ConstantExpression = () =>
            Sequence
            (
                Must
                (
                    Number,
                    StringConstant
                )
            );
        public static Expression<Rule> ColumnConstraint = () =>
            Sequence
            (
                Must
                (
                    Sequence(May(CommentOrWhitespace), May(Sequence(Keyword("NOT"), CommentOrWhitespace)), Keyword("NULL")),
                    Must
                    (
                        Sequence(May(CommentOrWhitespace), Keyword("PRIMARY"), CommentOrWhitespace, Keyword("KEY")),
                        Sequence(May(CommentOrWhitespace), Keyword("UNIQUE"))
                    ),
                    Sequence(May(CommentOrWhitespace), Keyword("CHECK"), May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace), LogicalExpression, May(CommentOrWhitespace), BracketClose),
                    May(Sequence(CommentOrWhitespace, ColumnConstraint))
                )
            );
        public static Expression<Rule> LogicalExpression = () =>                                                                                //  add rule
            Sequence
            (
                May(CommentOrWhitespace),
                Keyword("MEGKELLCSINAL")
            );
        /*
 <syntax>         ::= <rule> | <rule> <syntax>
 <rule>           ::= <opt-whitespace> "<" <rule-name> ">" <opt-whitespace> "::=" <opt-whitespace> <expression> <line-end>
 <opt-whitespace> ::= " " <opt-whitespace> | ""
 <expression>     ::= <list> | <list> "|" <expression>
 <line-end>       ::= <opt-whitespace> <EOL> | <line-end> <line-end>
 <list>           ::= <term> | <term> <opt-whitespace> <list>
 <term>           ::= <literal> | "<" <rule-name> ">"
 <literal>        ::= '"' <text> '"' | "'" <text> "'"
         */
        #endregion

        #region Over Clause
        //perfect
        public static Expression<Rule> OverClause = () =>
            Sequence
            (
                Keyword("OVER"),
                May(CommentOrWhitespace),
                BracketOpen,
                May(Sequence(May(CommentOrWhitespace), PartitionByClause)),
                May(Sequence(May(CommentOrWhitespace), OrderByClause)),
                May(Sequence(May(CommentOrWhitespace), RowOrRangeClause)),
                May(CommentOrWhitespace),
                BracketClose
            );


        #endregion
        #region Partition By Clause
        //perfect
        public static Expression<Rule> PartitionByClause = () =>
            Sequence
            (
                Keyword("PARTITION"),
                CommentOrWhitespace,
                Keyword("BY"),
                CommentOrWhitespace,
                ValueExpression
            );
        public static Expression<Rule> ValueExpression = () =>                  //need to be scalar function,scalar subquery, user-defined variable,column expression
            Sequence
            (
                Must(ColumnName, Number, StringConstant, Subquery, Variable, FunctionCall),
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), ValueExpression))
            );

        #endregion
        #region Row Or Range Clause
        //works
        public static Expression<Rule> RowOrRangeClause = () =>
            Sequence
            (
                Must(Keyword("ROWS"), Keyword("RANGE")),
                CommentOrWhitespace,
                WindowFrameExtent
            );
        public static Expression<Rule> WindowFrameExtent = () =>
            Must
            (
                WindowFramePreceding,
                WindowFrameBetween
            );
        public static Expression<Rule> WindowFrameBetween = () =>
            Sequence
            (
                Keyword("BETWEEN"),
                CommentOrWhitespace,
                WindowFrameBound,
                CommentOrWhitespace,
                Keyword("AND"),
                CommentOrWhitespace,
                WindowFrameBound

            );
        public static Expression<Rule> WindowFrameBound = () =>
            Must(WindowFramePreceding, WindowFrameFollowing);

        public static Expression<Rule> WindowFramePreceding = () =>
            Must(
                Sequence(Keyword("UNBOUNDED"), CommentOrWhitespace, Keyword("PRECEDING")),
                Sequence(Number, CommentOrWhitespace, Keyword("PRECEDING")),
                Sequence(Keyword("CURRENT"), May(CommentOrWhitespace), Keyword("ROW"))
            );
        public static Expression<Rule> WindowFrameFollowing = () =>
            Must
            (
                Sequence(Keyword("UNBOUNDED"), CommentOrWhitespace, Keyword("FOLLOWING")),
                Sequence(Number, CommentOrWhitespace, Keyword("FOLLOWING")),
                Sequence(Keyword("CURRENT"), CommentOrWhitespace, Keyword("ROW"))
            );
        #endregion
        #region Create View statement
        //works
        public static Expression<Rule> CreateViewStatement = () =>
            Sequence
            (
                Keyword("CREATE"),
                CommentOrWhitespace,
                Keyword("VIEW"),
                CommentOrWhitespace,
                TableOrViewName,
                May(Sequence(May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace), ColumnNameList, May(CommentOrWhitespace), BracketClose)),
                May(Sequence(May(CommentOrWhitespace), Keyword("WITH"), ViewAttribute)),
                CommentOrWhitespace,
                Keyword("AS"),
                CommentOrWhitespace,
                SelectStatement2,                                                                                                                               //  need to change to SelectStatement from SelectStatement2 
                May(Sequence(May(CommentOrWhitespace), Keyword("WITH"), CommentOrWhitespace, Keyword("CHECK"), CommentOrWhitespace, Keyword("OPTION")))
            );
        public static Expression<Rule> ViewAttribute = () =>
            Sequence
            (
                May(CommentOrWhitespace),
                Must(Keyword("ENCRYPTION"), Keyword("SCHEMABINDING"), Keyword("VIEW_METADATA")),
                May(Sequence(May(CommentOrWhitespace), Comma, ViewAttribute))
            );
        public static Expression<Rule> ColumnNameList = () =>
            Sequence(May(CommentOrWhitespace), ColumnName, May(Sequence(May(CommentOrWhitespace), Comma, ColumnNameList)));
        #endregion
        #region Drop View statement

        public static Expression<Rule> DropViewStatement = () =>
        Sequence
        (
            Keyword("DROP"),
            CommentOrWhitespace,
            Keyword("VIEW"),
            CommentOrWhitespace,
            ViewNameList
        );

        public static Expression<Rule> ViewNameList = () =>
            Sequence(
                May(CommentOrWhitespace),
                TableOrViewName,
                May(Sequence(May(CommentOrWhitespace), Comma, ViewNameList))
            );
        #endregion
        #region Drop Table statement

        public static Expression<Rule> DropTableStatement = () =>
        Sequence
        (
            Keyword("DROP"),
            CommentOrWhitespace,
            Keyword("TABLE"),
            CommentOrWhitespace,
            TableNameList
        );

        public static Expression<Rule> TableNameList = () =>
            Sequence(
                May(CommentOrWhitespace),
                TableOrViewName,
                May(Sequence(May(CommentOrWhitespace), Comma, TableNameList))
            );
        #endregion
        #region Truncate table

        public static Expression<Rule> TruncateTableStatement = () =>
        Sequence
        (
            Keyword("TRUNCATE"),
            CommentOrWhitespace,
            Keyword("TABLE"),
            CommentOrWhitespace,
            TableOrViewName
        );
        #endregion
        #region +predicates
        public static Expression<Rule> ContainsFreetext = () =>
        Sequence
        (
            Sequence
            (
                Keyword("CONTAINS"),
                May(CommentOrWhitespace),
                BracketOpen,
                May(CommentOrWhitespace),
                May
                (
                    Must
                    (
                        ColumnName,
                        Sequence(May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace), ColumnNameList, May(CommentOrWhitespace), BracketClose)
                    )
                ),
                May(CommentOrWhitespace),
                Comma,
                May(CommentOrWhitespace),
                ContainsSearchConditionList,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), Keyword("LANGUAGE"), CommentOrWhitespace, Must(Number, StringConstant))),
                May(CommentOrWhitespace),
                BracketClose
            )
        );
        public static Expression<Rule> ContainsSearchConditionList = () =>
        Sequence(
            ContainsSearchCondition,
            May(Sequence(CommentOrWhitespace, Must(LogicalOperator, Sequence(Keyword("AND"), CommentOrWhitespace, Keyword("NOT"))), CommentOrWhitespace, ContainsSearchConditionList))
        );

        public static Expression<Rule> ContainsSearchCondition = () =>
        Sequence(
            Must
            (
                Variable,
                SimpleTerm,
                PrefixTerm,
                GenerationTerm,
                GenericProximityTerm,
                CustonProximityTerm,
                WeightedTerm
            )
        );
        public static Expression<Rule> SimpleTerm = () =>
        Sequence
        (

        );
        public static Expression<Rule> PrefixTerm = () =>
        Sequence
        (

        );
        public static Expression<Rule> GenerationTerm = () =>
        Sequence
        (

        );
        public static Expression<Rule> GenericProximityTerm = () =>
        Sequence
        (

        );
        public static Expression<Rule> CustonProximityTerm = () =>
        Sequence
        (

        );
        public static Expression<Rule> WeightedTerm = () =>
        Sequence
        (

        );

        #endregion
        /*
        #region Delete statement
        public static Expression<Rule> DeleteStatement = () =>
            Sequence
            (
                May(CommentOrWhitespace),
                May(WithClause),
                Keyword("DELETE"),
                May(TopExpression),
                Keyword("FROM"),
                CommentOrWhitespace,
                TableAlias,
                May(CommentOrWhitespace),
                BracketOpen,
                May(CommentOrWhitespace),
                ColumnAliasList,
                May(CommentOrWhitespace),
                BracketClose,
                May(CommentOrWhitespace),
                Keyword("VALUES"),
                May(CommentOrWhitespace),
                BracketOpen,
                ColumnValueList,
                BracketClose
            );
        #endregion
        #region Update statement
        
        public static Expression<Rule> UpdateClause = () =>
            Sequence
            (
                May(WithClause),
                Keyword("UPDATE"),
                May(Sequence(CommentOrWhitespace, TopExpression)),
                CommentOrWhitespace,
                TableOrViewName,// or object or rowset_function_limited 
                May(CommentOrWhitespace),
                Keyword("SET"),
            );

        public static Expression<Rule> ColumnAliasExpressionList = () =>
            Sequence(May(CommentOrWhitespace), ColumnAlias, May(Sequence(May(CommentOrWhitespace), Comma, ColumnAliasList)));
        #endregion
         */
#if false




	



	

    

! <common_table_expression> ::=
!    Identifier [ '(' <ColumnList> ')' ]
!    "AS" '(' <QueryExpression> ')' ;
    
<ColumnList> ::= <ColumnName> [ ',' <ColumnList> ];
    

	
! <compute_clause> ::= "COMPUTE" { <function_call_list> } [ "BY" <ExpressionList> ];
	
! <function_call_list> ::= <FunctionCall> [ ',' <FunctionCall> ];

<ExpressionList> ::= <Expression> [ ',' <Expression> ];
    


! <query_hint > ::= 
!	{ { "HASH" | "ORDER" } "GROUP"
!    | { "CONCAT" | "HASH" | "MERGE" } "UNION"
!    | { "LOOP" | "MERGE" | "HASH" } "JOIN"
!    | "FAST" number_rows 
!    | "FORCE" "ORDER"
!    | "MAXDOP" number_of_processors 
!    | "OPTIMIZE" "FOR" ( @variable_name = literal_constant [ , ...n ] ) 
!    | "PARAMETERIZATION" { "SIMPLE" | "FORCED" }
!    | "RECOMPILE"
!    | "ROBUST" "PLAN"
!    | "KEEP" "PLAN"
!    | "KEEPFIXED" "PLAN"
!    | "EXPAND" "VIEWS"
!    | "MAXRECURSION" Number
!    | "USE" "PLAN" N'xml_plan'
!    } ;
    
! <query_hint_list> ::= <query_hint> [ ',' <query_hint> ]    ;
        
   

! <NewTable> ::= Identifier;








! -----------------------------------------------------------------------------------------------------





<TableExpression> ::=	{ <TableOrViewName>	| <TableAlias> };
    
<UdtColumnExpression> ::=	<UdtColumnName> [ <UdtMemberExpression> ];
                               
<UdtMemberExpression> ::=	{ '.' | '::' } { <PropertyName> '(' <ArgumentList> ')' | <PropertyName> };
	
        public static Expression<Rule> ColumnExpression = () =>
            Must
            (
                Sequence(ColumnAlias, May(CommentOrWhitespace), Equals, May(CommentOrWhitespace), Expression),
                Sequence(Expression, May(Sequence(May(Sequence(CommentOrWhitespace, Keyword("AS"))), CommentOrWhitespace, ColumnAlias)))
            );
    




<TableHintLimited> ::=
	{ "KEEPIDENTITY"
	| "KEEPDEFAULTS"
	| "FASTFIRSTROW"
	| "HOLDLOCK"
	| "IGNORE_CONSTRAINTS"
	| "IGNORE_TRIGGERS"
	| "NOWAIT"
	| "PAGLOCK"
	| "READCOMMITTED"
	| "READCOMMITTEDLOCK"
	| "READPAST"
	| "REPEATABLEREAD"
	| "ROWLOCK"
	| "SERIALIZABLE"
	| "TABLOCK"
	| "TABLOCKX"
	| "UPDLOCK"
	| "XLOCK"
	} ;
	

       
<TableSource> ::=
     { <FunctionCall> [ "AS" ] <TableAlias> [ '(' <ColumnAliasList> ')' ] 
     | <TableOrViewName> [ [ "AS" ] <TableAlias> ] [ <TablesampleClause> ] [ "WITH" '(' <TableHint> ')' ] [ <TablePartitionClause> ]
!    | "OPENXML" <openxml_clause> 
     | <DerivedTable> [ "AS" ] <TableAlias> [ '(' <ColumnAliasList> ')' ] 
!    | <pivoted_table> 
!    | <unpivoted_table>
     | <Variable> [ [ "AS" ] <TableAlias> ]
     | <Variable> '.' <FunctionCall> [ [ "AS" ] <TableAlias> ] [ '(' <ColumnAliasList> ')' ]
     | <Subquery> [ [ "AS" ] <TableAlias> ]
     };
     


<JoinedTable> ::= 
     { <JoinType> <TableSource> "ON" <SearchCondition> 
     | "CROSS" "JOIN" <TableSource>
     | { "CROSS" | "OUTER" } "APPLY" <TableSource>
	 | ',' <TableSource>
	 }
     [ <JoinedTable> ];


!<SearchCondition> ::= 
!    { <Predicate> | <LogicalNot> <SearchCondition> | <SearchConditionBrackets> } 
!    [ <LogicalOperator> <SearchCondition> ]
!    [ ',' <SearchCondition> ];

<SearchCondition> ::= 
    [ <LogicalNot> ] { <Predicate> | <SearchConditionBrackets> }
    [ <LogicalOperator> <SearchCondition> ];

<SearchConditionBrackets> ::= '(' <SearchCondition> ')';

! <SearchConditionTerms> ::= { "AND" | "OR" } [ "NOT" ] { <Predicate> | '(' <SearchCondition> ')' } [ <SearchConditionTerms> ];

<Predicate> ::= 
    { <Expression> <ComparisonOperator> <Expression>
    | <Expression> [ "NOT" ] "LIKE" <Expression> [ "ESCAPE" <Expression> ]
    | <Expression> [ "NOT" ] "BETWEEN" <Expression> "AND" <Expression>
    | <Expression> "IS" [ "NOT" ] "NULL" 
!    | "CONTAINS" '(' { <ColumnName> | '*' } ',' <contains_search_condition> ')' 
!    | "FREETEXT" '(' { <ColumnName> | '*' } ',' <freetext_string> ')'
    | <Expression> [ "NOT" ] "IN" { <Subquery> | '(' <ArgumentList> ')' }
    | <Expression> <ComparisonOperator> { "ALL" | "SOME" | "ANY"} '(' <Subquery> ')'
    | "EXISTS" <Subquery>
    };

	
! <contains_search_condition> ::= <StringExpression>;

! <freetext_string> ::= <StringExpression>;

#endif
    }
}
