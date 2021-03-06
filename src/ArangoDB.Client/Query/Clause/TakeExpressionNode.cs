﻿using Remotion.Linq;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Query.Clause
{
    public class TakeExpressionNode : MethodCallExpressionNodeBase
    {
        public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               LinqUtility.GetSupportedMethod (() => Queryable.Take<object> (null,0)),
                                                               LinqUtility.GetSupportedMethod (() => Enumerable.Take<object> (null,0))
                                                           };

        public TakeExpressionNode(MethodCallExpressionParseInfo parseInfo, Expression count)
            : base(parseInfo)
        {
            Count = count;
        }

        public Expression Count { get; private set; }


        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            LinqUtility.CheckNotNull("inputParameter", inputParameter);
            LinqUtility.CheckNotNull("expressionToBeResolved", expressionToBeResolved);

            return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
        }

        protected override void ApplyNodeSpecificSemantics(QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
        {
            LinqUtility.CheckNotNull("queryModel", queryModel);

            var lastClause = queryModel.BodyClauses.LastOrDefault();
            SkipTakeClause skipTakeClause = lastClause as SkipTakeClause;
            if (skipTakeClause != null)
                skipTakeClause.TakeCount = Count;
            else
                queryModel.BodyClauses.Add(new SkipTakeClause(null, Count));
        }
    }
}
