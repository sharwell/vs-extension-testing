﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for more information.

#nullable disable

namespace Xunit.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using Xunit.Abstractions;
    using Xunit.Harness;
    using Xunit.Sdk;

    public sealed class IdeTheoryTestCaseRunner : XunitTheoryTestCaseRunner
    {
        public IdeTheoryTestCaseRunner(
            WpfTestSharedData sharedData,
            VisualStudioVersion visualStudioVersion,
            IXunitTestCase testCase,
            string displayName,
            string skipReason,
            object[] constructorArguments,
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(testCase, displayName, skipReason, constructorArguments, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource)
        {
            SharedData = sharedData;
            VisualStudioVersion = visualStudioVersion;
        }

        public WpfTestSharedData SharedData
        {
            get;
        }

        public VisualStudioVersion VisualStudioVersion
        {
            get;
        }

        protected override XunitTestRunner CreateTestRunner(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, string skipReason, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            if (Process.GetCurrentProcess().ProcessName == "devenv")
            {
                // We are already running inside Visual Studio
                // TODO: Verify version under test
                return new InProcessIdeTestRunner(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, beforeAfterAttributes, aggregator, cancellationTokenSource);
            }
            else if (SharedData.Exception is not null)
            {
                return new ErrorReportingIdeTestRunner(SharedData.Exception, test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, beforeAfterAttributes, aggregator, cancellationTokenSource);
            }
            else
            {
                throw new NotSupportedException($"{nameof(IdeTheoryAttribute)} can only be used with the {nameof(IdeTestFramework)} test framework");
            }
        }
    }
}
