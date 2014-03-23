﻿using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a comparison between a value and null.
    /// </summary>
    public class NullFilter : Filter
    {
        private readonly IFilterItem _item;

        /// <summary>
        /// Initializes a new instance of a NullFilter.
        /// </summary>
        /// <param name="item">The item to check whether or not is null.</param>
        public NullFilter(IFilterItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            _item = item;
        }

        /// <summary>
        /// Gets the item being compared to null.
        /// </summary>
        public IFilterItem LeftHand
        {
            get { return _item; }
        }

        /// <summary>
        /// Gets or sets whether to negate the comparison.
        /// </summary>
        public bool Not
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the filter text irrespective of the parentheses.
        /// </summary>
        /// <param name="options">The configuration to use when building the command.</param>
        /// <returns>A string representing the filter.</returns>
        protected override TokenStream GetInnerFilterTokens(CommandOptions options)
        {
            TokenStream stream = new TokenStream();
            stream.AddRange(_item.GetFilterTokens(options));
            stream.Add(new TokenResult(SqlTokenRegistry.Is, "IS"));
            if (Not)
            {
                stream.Add(new TokenResult(SqlTokenRegistry.Not, "NOT"));
            }
            stream.Add(new TokenResult(SqlTokenRegistry.Null, "NULL"));
            return stream;
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            visitor.VisitNullFilter(this);
        }
    }
}
