﻿using System;
using System.Text;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents an inner join in a select statement.
    /// </summary>
    public class InnerJoin : FilteredJoin
    {
        /// <summary>
        /// Initializes a new instance of a InnerJoin.
        /// </summary>
        /// <param name="leftHand">The left hand item in the join.</param>
        /// <param name="rightHand">The right hand item in the join.</param>
        internal InnerJoin(Join leftHand, AliasedSource rightHand)
            : base(leftHand, rightHand)
        {
        }

        /// <summary>
        /// Gets the name of the join type.
        /// </summary>
        /// <param name="options">The configuration to use when building the command.</param>
        /// <returns>The name of the join type.</returns>
        protected override TokenResult GetJoinType(CommandOptions options)
        {
            StringBuilder result = new StringBuilder();
            if (options.VerboseInnerJoin)
            {
                result.Append("INNER ");
            }
            result.Append("JOIN");
            return new TokenResult(SqlTokenRegistry.InnerJoin, result.ToString());
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            visitor.VisitInnerJoin(this);
        }
    }
}
