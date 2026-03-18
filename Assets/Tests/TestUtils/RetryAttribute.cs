using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace UnityMobileQA.Tests.TestUtils
{
    /// <summary>
    /// Retries a failed test up to tryCount times. Use for flaky tests.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RetryAttribute : NUnitAttribute, IWrapSetUpTearDown
    {
        private readonly int _tryCount;

        public RetryAttribute(int tryCount = 3)
        {
            _tryCount = tryCount;
        }

        public TestCommand Wrap(TestCommand command)
        {
            return new RetryCommand(command, _tryCount);
        }

        private class RetryCommand : DelegatingTestCommand
        {
            private readonly int _tryCount;

            public RetryCommand(TestCommand innerCommand, int tryCount) : base(innerCommand)
            {
                _tryCount = tryCount;
            }

            public override TestResult Execute(ITestExecutionContext context)
            {
                int attempts = 0;
                while (true)
                {
                    attempts++;
                    context.CurrentResult = innerCommand.Execute(context);
                    if (context.CurrentResult.ResultState != ResultState.Failure)
                        return context.CurrentResult;
                    if (attempts >= _tryCount)
                        return context.CurrentResult;
                    context.CurrentResult = context.CurrentTest.MakeTestResult();
                }
            }
        }
    }
}
