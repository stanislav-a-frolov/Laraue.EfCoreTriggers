﻿using System.Linq;
using System.Linq.Expressions;
using Laraue.EfCoreTriggers.Common.Services.Impl.SetExpressionVisitors;
using Laraue.EfCoreTriggers.Common.SqlGeneration;
using Laraue.EfCoreTriggers.Common.TriggerBuilders;

namespace Laraue.EfCoreTriggers.Common.Services.Impl.TriggerVisitors.Statements;

/// <inheritdoc />
public class UpdateExpressionVisitor : IUpdateExpressionVisitor
{
    private readonly IMemberInfoVisitorFactory _memberInfoVisitorFactory;
    private readonly ISqlGenerator _sqlGenerator;

    /// <summary>
    /// Initializes a new instance of <see cref="UpdateExpressionVisitor"/>.
    /// </summary>
    /// <param name="memberInfoVisitorFactory"></param>
    /// <param name="sqlGenerator"></param>
    public UpdateExpressionVisitor(
        IMemberInfoVisitorFactory memberInfoVisitorFactory,
        ISqlGenerator sqlGenerator)
    {
        _memberInfoVisitorFactory = memberInfoVisitorFactory;
        _sqlGenerator = sqlGenerator;
    }

    /// <inheritdoc />
    public SqlBuilder Visit(LambdaExpression expression, ArgumentTypes argumentTypes, VisitedMembers visitedMembers)
    {
        var updateType = expression.Body.Type;
        
        var assignmentParts = _memberInfoVisitorFactory.Visit(
            expression,
            argumentTypes,
            visitedMembers);
        
        var sqlResult = new SqlBuilder();
        
        var assignmentPartsSql = assignmentParts
            .Select(expressionPart => 
                $"{_sqlGenerator.GetColumnSql(updateType, expressionPart.Key, ArgumentType.None)} = {expressionPart.Value}")
            .ToArray();
        
        sqlResult.AppendJoin(", ", assignmentPartsSql);
        return sqlResult;
    }
}