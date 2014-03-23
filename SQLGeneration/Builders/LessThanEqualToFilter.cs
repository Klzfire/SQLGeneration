﻿using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a comparison where the left hand item is less than or equal to the right hand item.
    /// </summary>
    public class LessThanEqualToFilter : OrderFilter
    {
        /// <summary>
        /// Initializes a new instance of a LessThanEqualToFilter.
        /// </summary>
        /// <param name="leftHand">The left hand item.</param>
        /// <param name="rightHand">The right hand item.</param>
        public LessThanEqualToFilter(IFilterItem leftHand, IFilterItem rightHand)
            : base(leftHand, rightHand)
        {
        }

        /// <summary>
        /// Gets the operator that will compare the left and right hand values.
        /// </summary>
        /// <param name="options">The configuration to use when building the command.</param>
        /// <returns>A string containing the name of the operation that compares the left and right hand sides.</returns>
        protected override TokenStream GetComparisonOperator(CommandOptions options)
        {
            TokenStream stream = new TokenStream();
            TokenResult token = new TokenResult(SqlTokenRegistry.LessThanEqualTo, "<=");
            stream.Add(token);
            return stream;
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            visitor.VisitLessThanEqualToFilter(this);
        }
    }
}
