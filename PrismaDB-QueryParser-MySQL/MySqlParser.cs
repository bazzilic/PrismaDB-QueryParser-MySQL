﻿using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using PrismaDB.Commons;
using PrismaDB.QueryAST;
using PrismaDB.QueryParser.MySQL.AntlrGrammer;
using System;
using System.Collections.Generic;

namespace PrismaDB.QueryParser.MySQL
{
    public static class MySqlQueryParser
    {
        public static List<Query> ParseToAst(String input)
        {
            try
            {
                var inputStream = new AntlrInputStream(input);
                var sqlLexer = new MySqlLexer(new CaseChangingCharStream(inputStream, true));
                var tokens = new CommonTokenStream(sqlLexer);
                var sqlParser = new MySqlParser(tokens);

                var visitor = new MySqlVisitor();
                var res = (List<Query>)visitor.Visit(sqlParser.root());
                return res;
            }
            catch (Exception ex) when (!(ex is PrismaParserException))
            {
                throw new PrismaParserException("Error occurred while parsing query.", ex);
            }
        }
    }

    public partial class MySqlVisitor : MySqlParserBaseVisitor<object>
    {
        public override object VisitRoot([NotNull] MySqlParser.RootContext context)
        {
            if (context.sqlStatements() == null)
                return new List<Query>();
            return Visit(context.sqlStatements());
        }

        public override object VisitSqlStatements([NotNull] MySqlParser.SqlStatementsContext context)
        {
            var queries = new List<Query>();
            foreach (var stmt in context.sqlStatement())
                queries.Add((Query)Visit(stmt));
            return queries;
        }
    }
}
