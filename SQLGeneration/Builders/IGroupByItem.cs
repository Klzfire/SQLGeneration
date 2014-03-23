﻿using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents an item that can appear in a group by clause in a select statement.
    /// </summary>
    public interface IGroupByItem : IVisitableBuilder
    {
        /// <summary>
        /// Gets a string representation of the group by.
        /// </summary>
        /// <param name="options">The configuration to use when building the command.</param>
        /// <returns>The generated text.</returns>
        TokenStream GetGroupByTokens(CommandOptions options);
    }
}
