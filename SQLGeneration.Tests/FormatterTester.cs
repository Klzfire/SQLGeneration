﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLGeneration.Builders;
using SQLGeneration.Generators;

namespace SQLGeneration.Tests
{
    /// <summary>
    /// Tests that we can generate SQL properly from our builder object model.
    /// </summary>
    [TestClass]
    public class FormatterTester
    {
        #region Select

        /// <summary>
        /// This removes previously added source and projection from SelectBuilder.
        /// </summary>
        [TestMethod]
        public void TestSelect_RemoveSource()
        {
            var builder = new SelectBuilder();
            var table1 = builder.AddTable(new Table("Table1"));
            var table2 = builder.AddTable(new Table("Table2"));
            builder.AddProjection(table1.Column("Column"));
            var table2Column = builder.AddProjection(table2.Column("Column"));
            builder.RemoveSource(table2);
            builder.RemoveProjection(table2Column);
            var formatter = new Formatter();
            var commandText = formatter.GetCommandText(builder);
            var expected = "SELECT Table1.Column FROM Table1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select a column from a table.
        /// </summary>
        [TestMethod]
        public void TestSelect_Column()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select a column from a table and sort by it.
        /// </summary>
        [TestMethod]
        public void TestSelect_Column_OrderByColumn()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddOrderBy(new OrderBy(table.Column("Column")));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table ORDER BY Table.Column";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can combine two select statements.
        /// </summary>
        [TestMethod]
        public void TestSelect_UnionAll()
        {
            SelectBuilder builder = new SelectBuilder();
            builder.AddProjection(new NumericLiteral(1));
            Union union = new Union(builder, builder) { Distinct = DistinctQualifier.All };
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(union);
            string expected = "SELECT 1 UNION ALL SELECT 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can combine two select statements.
        /// </summary>
        [TestMethod]
        public void TestSelect_Union()
        {
            SelectBuilder builder = new SelectBuilder();
            builder.AddProjection(new NumericLiteral(1));
            Union union = new Union(builder, builder);
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(union);
            string expected = "SELECT 1 UNION SELECT 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can combine two select statements.
        /// </summary>
        [TestMethod]
        public void TestSelect_Intersect()
        {
            SelectBuilder builder = new SelectBuilder();
            builder.AddProjection(new NumericLiteral(1));
            Intersect union = new Intersect(builder, builder);
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(union);
            string expected = "SELECT 1 INTERSECT SELECT 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can combine two select statements.
        /// </summary>
        [TestMethod]
        public void TestSelect_Except()
        {
            SelectBuilder builder = new SelectBuilder();
            builder.AddProjection(new NumericLiteral(1));
            Except union = new Except(builder, builder);
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(union);
            string expected = "SELECT 1 EXCEPT SELECT 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can combine two select statements.
        /// </summary>
        [TestMethod]
        public void TestSelect_Minus()
        {
            SelectBuilder builder = new SelectBuilder();
            builder.AddProjection(new NumericLiteral(1));
            Minus union = new Minus(builder, builder);
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(union);
            string expected = "SELECT 1 MINUS SELECT 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can ask for distinct items.
        /// </summary>
        [TestMethod]
        public void TestSelect_All()
        {
            SelectBuilder builder = new SelectBuilder();
            builder.Distinct = DistinctQualifier.All;
            builder.AddProjection(new NumericLiteral(1));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT ALL 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can ask for distinct items.
        /// </summary>
        [TestMethod]
        public void TestSelect_Distinct()
        {
            SelectBuilder builder = new SelectBuilder();
            builder.Distinct = DistinctQualifier.Distinct;
            builder.AddProjection(new NumericLiteral(1));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT DISTINCT 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select the top 5 items.
        /// </summary>
        [TestMethod]
        public void TestSelect_Top()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.Top = new Top(new NumericLiteral(5));
            builder.AddProjection(table.Column("Column"));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT TOP 5 Table.Column FROM Table";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select the top 5 percent of items.
        /// </summary>
        [TestMethod]
        public void TestSelect_TopPercent()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.Top = new Top(new NumericLiteral(5)) { IsPercent = true };
            builder.AddProjection(table.Column("Column"));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT TOP 5 PERCENT Table.Column FROM Table";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select the top 5 items, along with any ties.
        /// </summary>
        [TestMethod]
        public void TestSelect_TopWithTies()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.Top = new Top(new NumericLiteral(5)) { WithTies = true };
            builder.AddProjection(table.Column("Column"));
            builder.AddOrderBy(new OrderBy(table.Column("Column")));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT TOP 5 WITH TIES Table.Column FROM Table ORDER BY Table.Column";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select the top 5 percent of items, along with any ties.
        /// </summary>
        [TestMethod]
        public void TestSelect_TopPercentWithTies()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.Top = new Top(new NumericLiteral(5)) { IsPercent = true, WithTies = true };
            builder.AddProjection(table.Column("Column"));
            builder.AddOrderBy(new OrderBy(table.Column("Column")));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT TOP 5 PERCENT WITH TIES Table.Column FROM Table ORDER BY Table.Column";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select with a simple WHERE clause.
        /// </summary>
        [TestMethod]
        public void TestSelect_Where()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddWhere(new EqualToFilter(table.Column("Column"), new NumericLiteral(1)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column = 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select with a GROUP BY clause.
        /// </summary>
        [TestMethod]
        public void TestSelect_GroupBy()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(new Function("COUNT", new NumericLiteral(1)));
            builder.AddGroupBy(table.Column("Column"));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT COUNT(1) FROM Table GROUP BY Table.Column";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select with a HAVING clause.
        /// </summary>
        [TestMethod]
        public void TestSelect_Having()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddHaving(new EqualToFilter(table.Column("Column"), new NumericLiteral(1)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table HAVING Table.Column = 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select multiple values.
        /// </summary>
        [TestMethod]
        public void TestSelect_MultipleProjections()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(new NumericLiteral(1));
            builder.AddProjection(new StringLiteral("Hello"));
            builder.AddProjection(new NullLiteral());
            builder.AddProjection(new Function("IsNull", table.Column("Column"), new NumericLiteral(123)));
            builder.AddProjection(table.Column("Column"));
            SelectBuilder inner = new SelectBuilder();
            inner.AddProjection(new NumericLiteral(1234));
            builder.AddProjection(inner);
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT 1, 'Hello', NULL, IsNull(Table.Column, 123), Table.Column, (SELECT 1234) FROM Table";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can give a projection item an alias.
        /// </summary>
        [TestMethod]
        public void TestSelect_ProjectionWithAlias()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"), "c");
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column AS c FROM Table";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can project all of the columns in a table.
        /// </summary>
        [TestMethod]
        public void TestSelect_ProjectStar()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(new AllColumns());
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT * FROM Table";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can project all of the columns in a table.
        /// </summary>
        [TestMethod]
        public void TestSelect_ProjectTableQualifiedStar()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(new AllColumns(table));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.* FROM Table";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select from multiple tables.
        /// </summary>
        [TestMethod]
        public void TestSelect_MultipleJoinItems()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table1 = builder.AddTable(new Table("Table1"));
            AliasedSource table2 = builder.AddTable(new Table("Table2"));
            builder.AddProjection(table1.Column("Column"));
            builder.AddProjection(table2.Column("Column"));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table1.Column, Table2.Column FROM Table1, Table2";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can wrap join items in parenthesis. Some odd
        /// databases (like MS Access) like parenthesis.
        /// </summary>
        [TestMethod]
        public void TestSelect_WrappedTable()
        {
            SelectBuilder builder = new SelectBuilder();
            Join join = Join.From(new Table("Table1"))
                .InnerJoin(new Table("Table2"))
                .On(j => new EqualToFilter(j.Sources["Table1"].Column("Column"), j.Sources["Table2"].Column("Column")));
            join.WrapInParentheses = true;
            builder.AddJoin(join);
            builder.AddProjection(builder.Sources["Table1"].Column("Column"));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table1.Column FROM (Table1 INNER JOIN Table2 ON Table1.Column = Table2.Column)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select from the results of a function call.
        /// </summary>
        [TestMethod]
        public void TestSelect_FromFunction()
        {
            SelectBuilder builder = new SelectBuilder();
            builder.AddFunction(new Function("GetData"), "F");
            builder.AddProjection(builder.Sources["F"].Column("Column"));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT F.Column FROM GetData() F";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select from the results of another select.
        /// </summary>
        [TestMethod]
        public void TestSelect_FromSelect()
        {
            SelectBuilder builder = new SelectBuilder();
            SelectBuilder inner = new SelectBuilder();
            inner.AddProjection(new NumericLiteral(1), "Column");
            builder.AddSelect(inner);
            builder.AddProjection(new Column("Column"));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Column FROM (SELECT 1 AS Column)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we the AS keyword can be used to alias tables.
        /// </summary>
        [TestMethod]
        public void TestSelect_AliasTableUsingAs()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"), "t");
            builder.AddProjection(new AllColumns());
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder, new CommandOptions() { AliasColumnSourcesUsingAs = true });
            string expected = "SELECT * FROM Table AS t";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can perform a cross join between tables.
        /// </summary>
        [TestMethod]
        public void TestSelect_CrossJoin()
        {
            SelectBuilder builder = new SelectBuilder();
            Join join = Join.From(new Table("Table1"))
                .CrossJoin(new Table("Table2"));
            builder.AddJoin(join);
            builder.AddProjection(new AllColumns());
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT * FROM Table1 CROSS JOIN Table2";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can perform a left outer join between tables.
        /// </summary>
        [TestMethod]
        public void TestSelect_LeftOuterJoin()
        {
            SelectBuilder builder = new SelectBuilder();
            Join join = Join.From(new Table("Table1"))
                .LeftOuterJoin(new Table("Table2"))
                .On(j => new EqualToFilter(j.Sources["Table1"].Column("Column"), j.Sources["Table2"].Column("Column")));
            builder.AddJoin(join);
            builder.AddProjection(new AllColumns());
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT * FROM Table1 LEFT OUTER JOIN Table2 ON Table1.Column = Table2.Column";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can perform a right outer join between tables.
        /// </summary>
        [TestMethod]
        public void TestSelect_RightOuterJoin()
        {
            SelectBuilder builder = new SelectBuilder();
            Join join = Join.From(new Table("Table1"))
                .RightOuterJoin(new Table("Table2"))
                .On(j => new EqualToFilter(j.Sources["Table1"].Column("Column"), j.Sources["Table2"].Column("Column")));
            builder.AddJoin(join);
            builder.AddProjection(new AllColumns());
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT * FROM Table1 RIGHT OUTER JOIN Table2 ON Table1.Column = Table2.Column";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can perform a full outer join between tables.
        /// </summary>
        [TestMethod]
        public void TestSelect_FullOuterJoin()
        {
            SelectBuilder builder = new SelectBuilder();
            Join join = Join.From(new Table("Table1"))
                .FullOuterJoin(new Table("Table2"))
                .On(j => new EqualToFilter(j.Sources["Table1"].Column("Column"), j.Sources["Table2"].Column("Column")));
            builder.AddJoin(join);
            builder.AddProjection(new AllColumns());
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT * FROM Table1 FULL OUTER JOIN Table2 ON Table1.Column = Table2.Column";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can group by multiple items.
        /// </summary>
        [TestMethod]
        public void TestSelect_MultipleGroupByItems()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column1"));
            builder.AddProjection(table.Column("Column2"));
            builder.AddProjection(new Function("SUM", table.Column("Column3")));
            builder.AddGroupBy(table.Column("Column1"));
            builder.AddGroupBy(table.Column("Column2"));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column1, Table.Column2, SUM(Table.Column3) FROM Table GROUP BY Table.Column1, Table.Column2";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can order by multiple items.
        /// </summary>
        [TestMethod]
        public void TestSelect_MultipleOrderByItems()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column1"));
            builder.AddProjection(table.Column("Column2"));
            builder.AddOrderBy(new OrderBy(table.Column("Column1")));
            builder.AddOrderBy(new OrderBy(table.Column("Column2")));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column1, Table.Column2 FROM Table ORDER BY Table.Column1, Table.Column2";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can order in descending order.
        /// </summary>
        [TestMethod]
        public void TestSelect_OrderDescending()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddOrderBy(new OrderBy(table.Column("Column"), Order.Descending));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table ORDER BY Table.Column DESC";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can specify where null values appear.
        /// </summary>
        [TestMethod]
        public void TestSelect_OrderNullsFirst()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddOrderBy(new OrderBy(table.Column("Column"), nullPlacement: NullPlacement.First));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table ORDER BY Table.Column NULLS FIRST";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can specify order direction and where null values appear.
        /// </summary>
        [TestMethod]
        public void TestSelect_OrderAscendingNullsLast()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddOrderBy(new OrderBy(table.Column("Column"), Order.Ascending, NullPlacement.Last));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table ORDER BY Table.Column ASC NULLS LAST";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can wrap multiple filters.
        /// </summary>
        [TestMethod]
        public void TestSelect_WrappedFilters()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            FilterGroup group = new FilterGroup() { WrapInParentheses = true };
            group.AddFilter(new EqualToFilter(new NumericLiteral(1), new NumericLiteral(1)));
            group.AddFilter(new LikeFilter(table.Column("Column"), new StringLiteral("%ABC")));
            builder.AddWhere(group);
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE (1 = 1 AND Table.Column LIKE '%ABC')";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can negate filters.
        /// </summary>
        [TestMethod]
        public void TestSelect_NegatedFilters()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            FilterGroup group = new FilterGroup();
            group.AddFilter(new EqualToFilter(new NumericLiteral(1), new NumericLiteral(1)));
            group.AddFilter(new LikeFilter(table.Column("Column"), new StringLiteral("%ABC")));
            builder.AddWhere(new NotFilter(group));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE NOT (1 = 1 AND Table.Column LIKE '%ABC')";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can filter by two filters OR'd together.
        /// </summary>
        [TestMethod]
        public void TestSelect_DisjunctionFilter()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            FilterGroup orGroup = new FilterGroup(Conjunction.Or);
            orGroup.AddFilter(new EqualToFilter(new NumericLiteral(1), new NumericLiteral(1)));
            orGroup.AddFilter(new NullFilter(table.Column("Column")));
            builder.AddWhere(orGroup);
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE 1 = 1 OR Table.Column IS NULL";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can wrap a single filter.
        /// </summary>
        [TestMethod]
        public void TestSelect_WrapFilter()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddWhere(new EqualToFilter(new NumericLiteral(1), new NumericLiteral(1)) { WrapInParentheses = true });
            builder.AddWhere(new EqualToFilter(new NumericLiteral(1), new NumericLiteral(1)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE (1 = 1) AND 1 = 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can negate a single filter.
        /// </summary>
        [TestMethod]
        public void TestSelect_NegateFilter()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddWhere(new NotFilter(new EqualToFilter(new NumericLiteral(1), new NumericLiteral(1))));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE NOT 1 = 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can use a between filter.
        /// </summary>
        [TestMethod]
        public void TestSelect_BetweenFilter()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddWhere(new BetweenFilter(table.Column("Column"), new NumericLiteral(0), new NumericLiteral(100)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column BETWEEN 0 AND 100";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can use a between filter, using its internal NOT keyword.
        /// </summary>
        [TestMethod]
        public void TestSelect_BetweenFilter_Negated()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddWhere(new BetweenFilter(table.Column("Column"), new NumericLiteral(0), new NumericLiteral(100)) { Not = true });
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column NOT BETWEEN 0 AND 100";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can use a like filter, using its internal NOT keyword.
        /// </summary>
        [TestMethod]
        public void TestSelect_LikeFilter_Negated()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddWhere(new LikeFilter(table.Column("Column"), new StringLiteral("%Bob%")) { Not = true });
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column NOT LIKE '%Bob%'";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can use an IS NULL filter, using its internal NOT keyword.
        /// </summary>
        [TestMethod]
        public void TestSelect_NullFilter_Negated()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddWhere(new NullFilter(table.Column("Column")) { Not = true });
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column IS NOT NULL";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can use an IN filter.
        /// </summary>
        [TestMethod]
        public void TestSelect_InFilter()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddWhere(new InFilter(table.Column("Column"), new ValueList(new NumericLiteral(1), new NumericLiteral(2), new NumericLiteral(3))));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column IN (1, 2, 3)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can use an IN filter, using its internal NOT.
        /// </summary>
        [TestMethod]
        public void TestSelect_InFilter_Negated()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddWhere(new InFilter(table.Column("Column"), new ValueList(new NumericLiteral(1), new NumericLiteral(2), new NumericLiteral(3))) { Not = true });
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column NOT IN (1, 2, 3)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can use an IN filter, using a select statement for the value source.
        /// </summary>
        [TestMethod]
        public void TestSelect_InFilter_SelectSource()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            SelectBuilder inner = new SelectBuilder();
            inner.AddProjection(new NumericLiteral(1));
            builder.AddWhere(new InFilter(table.Column("Column"), inner));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column IN (SELECT 1)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can use an IN filter, using a function call for the value source.
        /// </summary>
        [TestMethod]
        public void TestSelect_InFilter_FunctionSource()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddWhere(new InFilter(table.Column("Column"), new Function("GetData")));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column IN GetData()";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can use an EXISTS filter.
        /// </summary>
        [TestMethod]
        public void TestSelect_ExistsFilter()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            SelectBuilder inner = new SelectBuilder();
            inner.AddProjection(new NumericLiteral(1));
            ExistsFilter filter = new ExistsFilter(inner);
            builder.AddWhere(filter);
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE EXISTS(SELECT 1)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can use an equality quantifier.
        /// </summary>
        [TestMethod]
        public void TestSelect_All_Select()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            SelectBuilder inner = new SelectBuilder();
            inner.AddProjection(new NumericLiteral(1));
            EqualToQuantifierFilter filter = new EqualToQuantifierFilter(table.Column("Column"), Quantifier.All, inner);
            builder.AddWhere(filter);
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column = ALL (SELECT 1)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can use a inequality quantifier with a value list.
        /// </summary>
        [TestMethod]
        public void TestSelect_Any_ValueList()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            ValueList values = new ValueList(new NumericLiteral(1), new NumericLiteral(2), new NumericLiteral(3));
            NotEqualToQuantifierFilter filter = new NotEqualToQuantifierFilter(table.Column("Column"), Quantifier.Any, values);
            builder.AddWhere(filter);
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column <> ANY (1, 2, 3)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can use a inequality quantifier with a value list.
        /// </summary>
        [TestMethod]
        public void TestSelect_Some()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            ValueList values = new ValueList(new NumericLiteral(1), new NumericLiteral(2), new NumericLiteral(3));
            LessThanQuantifierFilter filter = new LessThanQuantifierFilter(table.Column("Column"), Quantifier.Some, values);
            builder.AddWhere(filter);
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column < SOME (1, 2, 3)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can filter using inequality.
        /// </summary>
        [TestMethod]
        public void TestSelect_NotEqualToFilter()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddWhere(new NotEqualToFilter(table.Column("Column"), new NumericLiteral(1)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column <> 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can filter using less than or equal to.
        /// </summary>
        [TestMethod]
        public void TestSelect_LessThanEqualTo()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddWhere(new LessThanEqualToFilter(table.Column("Column"), new NumericLiteral(1)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column <= 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can filter using greater than or equal to.
        /// </summary>
        [TestMethod]
        public void TestSelect_GreaterThanEqualTo()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddWhere(new GreaterThanEqualToFilter(table.Column("Column"), new NumericLiteral(1)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column >= 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can filter using less than.
        /// </summary>
        [TestMethod]
        public void TestSelect_LessThan()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddWhere(new LessThanFilter(table.Column("Column"), new NumericLiteral(1)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column < 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can filter using greater than.
        /// </summary>
        [TestMethod]
        public void TestSelect_GreaterThan()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Table"));
            builder.AddProjection(table.Column("Column"));
            builder.AddWhere(new GreaterThanFilter(table.Column("Column"), new NumericLiteral(1)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT Table.Column FROM Table WHERE Table.Column > 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select 1 + 1.
        /// </summary>
        [TestMethod]
        public void TestSelect_Addition()
        {
            SelectBuilder builder = new SelectBuilder();
            builder.AddProjection(new Addition(new NumericLiteral(1), new NumericLiteral(1)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT (1 + 1)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select 1 - 1.
        /// </summary>
        [TestMethod]
        public void TestSelect_Subtraction()
        {
            SelectBuilder builder = new SelectBuilder();
            builder.AddProjection(new Subtraction(new NumericLiteral(1), new NumericLiteral(1)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT (1 - 1)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select 1 * 1.
        /// </summary>
        [TestMethod]
        public void TestSelect_Multiplication()
        {
            SelectBuilder builder = new SelectBuilder();
            builder.AddProjection(new Multiplication(new NumericLiteral(1), new NumericLiteral(1)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT (1 * 1)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select 1 / 1.
        /// </summary>
        [TestMethod]
        public void TestSelect_Division()
        {
            SelectBuilder builder = new SelectBuilder();
            builder.AddProjection(new Division(new NumericLiteral(1), new NumericLiteral(1)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT (1 / 1)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can select 1 % 1.
        /// </summary>
        [TestMethod]
        public void TestSelect_Modulus()
        {
            SelectBuilder builder = new SelectBuilder();
            builder.AddProjection(new Modulus(new NumericLiteral(1), new NumericLiteral(1)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "SELECT (1 % 1)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// We should be able to build a function that applies ROW_NUMBER to each row to
        /// return a window.
        /// </summary>
        [TestMethod]
        public void TestSelect_FunctionWithOrderingWindow()
        {
            SelectBuilder innerBuilder = new SelectBuilder();

            AliasedSource table = innerBuilder.AddTable(new Table("Table"));

            Column column1 = table.Column("Column1");
            Column column2 = table.Column("Column2");
            Column column3 = table.Column("Column3");
            innerBuilder.AddProjection(column1, "c1");

            Function function = new Function("ROW_NUMBER");
            FunctionWindow window = new FunctionWindow();
            window.AddOrderBy(new OrderBy(column2));
            window.AddOrderBy(new OrderBy(column3));
            function.FunctionWindow = window;
            innerBuilder.AddProjection(function, "rn");

            SelectBuilder builder = new SelectBuilder();
            AliasedSource inner = builder.AddSelect(innerBuilder, "inner");
            builder.AddProjection(inner.Column("c1"));
            builder.AddWhere(new BetweenFilter(inner.Column("rn"), new NumericLiteral(11), new NumericLiteral(20)));

            Formatter formatter = new Formatter();
            string actual = formatter.GetCommandText(builder);
            string expected = "SELECT inner.c1 FROM (SELECT Table.Column1 AS c1, ROW_NUMBER() OVER (ORDER BY Table.Column2, Table.Column3) AS rn FROM Table) inner WHERE inner.rn BETWEEN 11 AND 20";
            Assert.AreEqual(expected, actual, "The wrong SQL was generated.");
        }

        /// <summary>
        /// We should be able to apply a function to a partition.
        /// </summary>
        [TestMethod]
        public void TestSelect_FunctionWithPartitioning()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource table = builder.AddTable(new Table("Employee"));
            AliasedProjection employeeType = builder.AddProjection(table.Column("Type"), "Type");

            Function function = new Function("COUNT", new NumericLiteral(1));
            FunctionWindow window = new FunctionWindow();
            window.AddPartition(employeeType);
            function.FunctionWindow = window;
            builder.AddProjection(function, "Count");

            Formatter formatter = new Formatter();
            string actual = formatter.GetCommandText(builder);
            string expected = "SELECT Employee.Type AS Type, COUNT(1) OVER (PARTITION BY Employee.Type) AS Count FROM Employee";
            Assert.AreEqual(expected, actual, "The wrong SQL was generated.");
        }

        /// <summary>
        /// We should be able to apply a function to a partitioned, ordered frame.
        /// </summary>
        [TestMethod]
        public void TestSelect_FunctionWithBetweenFraming()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource sale = builder.AddTable(new Table("sale"));
            AliasedProjection productId = builder.AddProjection(sale.Column("prod_id"));
            AliasedProjection monthNumber = builder.AddProjection(sale.Column("month_num"));
            AliasedProjection sales = builder.AddProjection(sale.Column("sales"));

            Function function = new Function("SUM", sales.ProjectionItem);
            FunctionWindow window = new FunctionWindow();
            window.AddPartition(productId);
            window.AddOrderBy(new OrderBy(monthNumber));
            window.Frame = new BetweenWindowFrame(new PrecedingUnboundFrame(), new FollowingUnboundFrame());
            function.FunctionWindow = window;
            builder.AddProjection(function);

            Formatter formatter = new Formatter();
            string actual = formatter.GetCommandText(builder);
            string expected = "SELECT sale.prod_id, sale.month_num, sale.sales, SUM(sale.sales) OVER (PARTITION BY sale.prod_id ORDER BY sale.month_num ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING) FROM sale";
            Assert.AreEqual(expected, actual, "The wrong SQL was generated.");
        }

        /// <summary>
        /// We should be able to apply a function to a partitioned, ordered frame.
        /// </summary>
        [TestMethod]
        public void TestSelect_FunctionWithStartFraming()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource sale = builder.AddTable(new Table("sale"));
            AliasedProjection productId = builder.AddProjection(sale.Column("prod_id"));
            AliasedProjection monthNumber = builder.AddProjection(sale.Column("month_num"));
            AliasedProjection sales = builder.AddProjection(sale.Column("sales"));

            Function function = new Function("SUM", sales.ProjectionItem);
            FunctionWindow window = new FunctionWindow();
            window.AddPartition(productId);
            window.AddOrderBy(new OrderBy(monthNumber));
            window.Frame = new PrecedingOnlyWindowFrame(new PrecedingBoundFrame(12));
            function.FunctionWindow = window;
            builder.AddProjection(function);

            Formatter formatter = new Formatter();
            string actual = formatter.GetCommandText(builder);
            string expected = "SELECT sale.prod_id, sale.month_num, sale.sales, SUM(sale.sales) OVER (PARTITION BY sale.prod_id ORDER BY sale.month_num ROWS 12 PRECEDING) FROM sale";
            Assert.AreEqual(expected, actual, "The wrong SQL was generated.");
        }

        /// <summary>
        /// We should be able to specify that a frame window starts and stops with the current row.
        /// This is pointless, but nonetheless possible.
        /// </summary>
        [TestMethod]
        public void TestSelect_FunctionWithBetweenFramingCurrentRow()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource sale = builder.AddTable(new Table("sale"));
            AliasedProjection productId = builder.AddProjection(sale.Column("prod_id"));
            AliasedProjection monthNumber = builder.AddProjection(sale.Column("month_num"));
            AliasedProjection sales = builder.AddProjection(sale.Column("sales"));

            Function function = new Function("SUM", sales.ProjectionItem);
            FunctionWindow window = new FunctionWindow();
            window.AddPartition(productId);
            window.AddOrderBy(new OrderBy(monthNumber));
            window.Frame = new BetweenWindowFrame(new CurrentRowFrame(), new CurrentRowFrame());
            function.FunctionWindow = window;
            builder.AddProjection(function);

            Formatter formatter = new Formatter();
            string actual = formatter.GetCommandText(builder);
            string expected = "SELECT sale.prod_id, sale.month_num, sale.sales, SUM(sale.sales) OVER (PARTITION BY sale.prod_id ORDER BY sale.month_num ROWS BETWEEN CURRENT ROW AND CURRENT ROW) FROM sale";
            Assert.AreEqual(expected, actual, "The wrong SQL was generated.");
        }

        /// <summary>
        /// We can create a CASE expression with just one case.
        /// </summary>
        [TestMethod]
        public void TestSelect_MatchCase_MultipleCases()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource source = builder.AddTable(new Table("Table"));
            Column column = source.Column("Column");
            MatchCase options = new MatchCase(column);
            options.AddBranch(new NumericLiteral(0), new StringLiteral("Sunday"));
            options.AddBranch(new NumericLiteral(1), new StringLiteral("Monday"));
            options.AddBranch(new NumericLiteral(2), new StringLiteral("Tuesday"));
            options.AddBranch(new NumericLiteral(3), new StringLiteral("Wednesday"));
            options.AddBranch(new NumericLiteral(4), new StringLiteral("Thursday"));
            options.AddBranch(new NumericLiteral(5), new StringLiteral("Friday"));
            options.AddBranch(new NumericLiteral(6), new StringLiteral("Saturday"));
            builder.AddProjection(options);

            Formatter formatter = new Formatter();
            string actual = formatter.GetCommandText(builder);
            string expected = "SELECT CASE Table.Column WHEN 0 THEN 'Sunday' WHEN 1 THEN 'Monday' WHEN 2 THEN 'Tuesday' WHEN 3 THEN 'Wednesday' WHEN 4 THEN 'Thursday' WHEN 5 THEN 'Friday' WHEN 6 THEN 'Saturday' END FROM Table";
            Assert.AreEqual(expected, actual, "The wrong SQL was generated.");
        }

        /// <summary>
        /// We can create a CASE expression with an ELSE branch.
        /// </summary>
        [TestMethod]
        public void TestSelect_MatchCase_Else()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource source = builder.AddTable(new Table("Table"));
            Column column = source.Column("Column");
            MatchCase options = new MatchCase(column);
            options.AddBranch(new StringLiteral("Admin"), new StringLiteral("Administrator"));
            options.Default = new StringLiteral("User");
            builder.AddProjection(options);

            Formatter formatter = new Formatter();
            string actual = formatter.GetCommandText(builder);
            string expected = "SELECT CASE Table.Column WHEN 'Admin' THEN 'Administrator' ELSE 'User' END FROM Table";
            Assert.AreEqual(expected, actual, "The wrong SQL was generated.");
        }

        /// <summary>
        /// We can create a CASE expression with just one case.
        /// </summary>
        [TestMethod]
        public void TestSelect_ConditionalCase_MultipleCases()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource source = builder.AddTable(new Table("Table"));
            Column column = source.Column("Column");
            ConditionalCase options = new ConditionalCase();
            options.AddBranch(new EqualToFilter(column, new NumericLiteral(0)), new StringLiteral("Sunday"));
            options.AddBranch(new EqualToFilter(column, new NumericLiteral(1)), new StringLiteral("Monday"));
            options.AddBranch(new EqualToFilter(column, new NumericLiteral(2)), new StringLiteral("Tuesday"));
            options.AddBranch(new EqualToFilter(column, new NumericLiteral(3)), new StringLiteral("Wednesday"));
            options.AddBranch(new EqualToFilter(column, new NumericLiteral(4)), new StringLiteral("Thursday"));
            options.AddBranch(new EqualToFilter(column, new NumericLiteral(5)), new StringLiteral("Friday"));
            options.AddBranch(new EqualToFilter(column, new NumericLiteral(6)), new StringLiteral("Saturday"));
            builder.AddProjection(options);

            Formatter formatter = new Formatter();
            string actual = formatter.GetCommandText(builder);
            string expected = "SELECT CASE WHEN Table.Column = 0 THEN 'Sunday' WHEN Table.Column = 1 THEN 'Monday' WHEN Table.Column = 2 THEN 'Tuesday' WHEN Table.Column = 3 THEN 'Wednesday' WHEN Table.Column = 4 THEN 'Thursday' WHEN Table.Column = 5 THEN 'Friday' WHEN Table.Column = 6 THEN 'Saturday' END FROM Table";
            Assert.AreEqual(expected, actual, "The wrong SQL was generated.");
        }

        /// <summary>
        /// We can create a CASE expression with an ELSE branch.
        /// </summary>
        [TestMethod]
        public void TestSelect_ConditionalCase_Else()
        {
            SelectBuilder builder = new SelectBuilder();
            AliasedSource source = builder.AddTable(new Table("Table"));
            Column column = source.Column("Column");
            ConditionalCase options = new ConditionalCase();
            options.AddBranch(new EqualToFilter(column, new StringLiteral("Admin")), new StringLiteral("Administrator"));
            options.Default = new StringLiteral("User");
            builder.AddProjection(options);

            Formatter formatter = new Formatter();
            string actual = formatter.GetCommandText(builder);
            string expected = "SELECT CASE WHEN Table.Column = 'Admin' THEN 'Administrator' ELSE 'User' END FROM Table";
            Assert.AreEqual(expected, actual, "The wrong SQL was generated.");
        }

        #endregion

        #region Insert

        /// <summary>
        /// This sees whether we can generate a simple insert statement.
        /// </summary>
        [TestMethod]
        public void TestInsert_NoColumns_NoValues()
        {
            Table table = new Table("Table");
            ValueList values = new ValueList();
            InsertBuilder builder = new InsertBuilder(table, values);
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "INSERT INTO Table VALUES()";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can alias a table in an insert statement.
        /// </summary>
        [TestMethod]
        public void TestInsert_AliasedTable()
        {
            Table table = new Table("Table");
            ValueList values = new ValueList();
            InsertBuilder builder = new InsertBuilder(table, values, "t");
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "INSERT INTO Table t VALUES()";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can alias a table in an insert statement using the AS keyword.
        /// </summary>
        [TestMethod]
        public void TestInsert_AliasedTable_WithAsKeyword()
        {
            Table table = new Table("Table");
            ValueList values = new ValueList();
            InsertBuilder builder = new InsertBuilder(table, values, "t");
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder, new CommandOptions() { AliasColumnSourcesUsingAs = true });
            string expected = "INSERT INTO Table AS t VALUES()";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can specify a column to insert into.
        /// </summary>
        [TestMethod]
        public void TestInsert_Column()
        {
            Table table = new Table("Table");
            ValueList values = new ValueList();
            InsertBuilder builder = new InsertBuilder(table, values);
            builder.AddColumn(builder.Table.Column("Column"));
            values.AddValue(new NumericLiteral(1));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder, new CommandOptions() { AliasColumnSourcesUsingAs = true });
            string expected = "INSERT INTO Table (Column) VALUES(1)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can use a select statement as the source of the values.
        /// </summary>
        [TestMethod]
        public void TestInsert_SelectAsSource()
        {
            Table table = new Table("Table");
            SelectBuilder select = new SelectBuilder();
            InsertBuilder builder = new InsertBuilder(table, select);
            builder.AddColumn(builder.Table.Column("Column"));
            select.AddProjection(new NumericLiteral(1));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder, new CommandOptions() { AliasColumnSourcesUsingAs = true });
            string expected = "INSERT INTO Table (Column) (SELECT 1)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can insert into multiple values.
        /// </summary>
        [TestMethod]
        public void TestInsert_MultipleColumns()
        {
            Table table = new Table("Table");
            ValueList values = new ValueList();
            InsertBuilder builder = new InsertBuilder(table, values);
            builder.AddColumn(builder.Table.Column("Column1"));
            builder.AddColumn(builder.Table.Column("Column2"));
            values.AddValue(new NumericLiteral(1));
            values.AddValue(new NullLiteral());
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder, new CommandOptions() { AliasColumnSourcesUsingAs = true });
            string expected = "INSERT INTO Table (Column1, Column2) VALUES(1, NULL)";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        #endregion

        #region Update

        /// <summary>
        /// This sees whether we can create an update statement that only updates a single column.
        /// </summary>
        [TestMethod]
        public void TestUpdate_OneSetter()
        {
            Table table = new Table("Table");
            UpdateBuilder builder = new UpdateBuilder(table);
            builder.AddSetter(new Setter(builder.Table.Column("Column"), new NumericLiteral(1)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "UPDATE Table SET Column = 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can create an update statement with a where clause.
        /// </summary>
        [TestMethod]
        public void TestUpdate_Where()
        {
            Table table = new Table("Table");
            UpdateBuilder builder = new UpdateBuilder(table);
            builder.AddSetter(new Setter(builder.Table.Column("Column"), new NumericLiteral(1)));
            builder.AddWhere(new EqualToFilter(builder.Table.Column("Column"), new NumericLiteral(2)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "UPDATE Table SET Column = 1 WHERE Column = 2";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can create an update statement with multiple setters.
        /// </summary>
        [TestMethod]
        public void TestUpdate_MultipleSetters()
        {
            Table table = new Table("Table");
            UpdateBuilder builder = new UpdateBuilder(table);
            builder.AddSetter(new Setter(builder.Table.Column("Column1"), new NumericLiteral(1)));
            builder.AddSetter(new Setter(builder.Table.Column("Column2"), new StringLiteral("Hello")));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "UPDATE Table SET Column1 = 1, Column2 = 'Hello'";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can create an update statement with an aliased table.
        /// </summary>
        [TestMethod]
        public void TestUpdate_AliasedTable()
        {
            Table table = new Table("Table");
            UpdateBuilder builder = new UpdateBuilder(table, "t");
            builder.AddSetter(new Setter(builder.Table.Column("Column1"), new NumericLiteral(1)));
            builder.AddSetter(new Setter(builder.Table.Column("Column2"), new StringLiteral("Hello")));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "UPDATE Table t SET Column1 = 1, Column2 = 'Hello'";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can create an update statement with an aliased table, forcing
        /// the AS keyword to appear.
        /// </summary>
        [TestMethod]
        public void TestUpdate_AliasedTable_WithAs()
        {
            Table table = new Table("Table");
            UpdateBuilder builder = new UpdateBuilder(table, "t");
            builder.AddSetter(new Setter(builder.Table.Column("Column1"), new NumericLiteral(1)));
            builder.AddSetter(new Setter(builder.Table.Column("Column2"), new StringLiteral("Hello")));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder, new CommandOptions() { AliasColumnSourcesUsingAs = true });
            string expected = "UPDATE Table AS t SET Column1 = 1, Column2 = 'Hello'";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        #endregion

        #region Delete

        /// <summary>
        /// This sees whether we can create an delete statement with no where clause.
        /// </summary>
        [TestMethod]
        public void TestDelete()
        {
            Table table = new Table("Table");
            DeleteBuilder builder = new DeleteBuilder(table);
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "DELETE FROM Table";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can create an delete statement that aliases its table.
        /// </summary>
        [TestMethod]
        public void TestDelete_AliasedTable()
        {
            Table table = new Table("Table");
            DeleteBuilder builder = new DeleteBuilder(table, "t");
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "DELETE FROM Table t";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can create an delete statement that aliases its table,
        /// forcing the use of the AS keyword.
        /// </summary>
        [TestMethod]
        public void TestDelete_AliasedTable_WithAs()
        {
            Table table = new Table("Table");
            DeleteBuilder builder = new DeleteBuilder(table, "t");
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder, new CommandOptions() { AliasColumnSourcesUsingAs = true });
            string expected = "DELETE FROM Table AS t";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        /// <summary>
        /// This sees whether we can create an delete statement with a WHERE clause.
        /// </summary>
        [TestMethod]
        public void TestDelete_Where()
        {
            Table table = new Table("Table");
            DeleteBuilder builder = new DeleteBuilder(table);
            builder.AddWhere(new EqualToFilter(builder.Table.Column("Column"), new NumericLiteral(1)));
            Formatter formatter = new Formatter();
            string commandText = formatter.GetCommandText(builder);
            string expected = "DELETE FROM Table WHERE Column = 1";
            Assert.AreEqual(expected, commandText, "The wrong SQL was generated.");
        }

        #endregion
    }
}
