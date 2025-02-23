﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Laraue.EfCoreTriggers.Common.Services.Impl.ExpressionVisitors;
using Laraue.EfCoreTriggers.Common.SqlGeneration;
using Laraue.EfCoreTriggers.Common.TriggerBuilders;

namespace Laraue.EfCoreTriggers.Common.Services.Impl.SetExpressionVisitors;

/// <inheritdoc />
public class SetNewExpressionVisitor : IMemberInfoVisitor<NewExpression>
{
    private readonly IExpressionVisitorFactory _factory;
    private readonly VisitingInfo _visitingInfo;

    /// <summary>
    /// Initializes a new instance of <see cref="SetNewExpressionVisitor"/>.
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="visitingInfo"></param>
    public SetNewExpressionVisitor(IExpressionVisitorFactory factory, VisitingInfo visitingInfo)
    {
        _factory = factory;
        _visitingInfo = visitingInfo;
    }

    /// <inheritdoc />
    public Dictionary<MemberInfo, SqlBuilder> Visit(NewExpression expression, ArgumentTypes argumentTypes, VisitedMembers visitedMembers)
    {
        return expression.Arguments.ToDictionary(
            argument => ((MemberExpression)argument).Member,
            argument =>
            {
                return _visitingInfo.ExecuteWithChangingMember(
                    ((MemberExpression)argument).Member,
                    () => _factory.Visit(argument, argumentTypes, visitedMembers));
            });
    }
}