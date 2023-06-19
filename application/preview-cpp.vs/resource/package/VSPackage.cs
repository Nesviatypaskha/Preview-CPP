using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace resource.package
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(CONSTANT.GUID)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.FirstLaunchSetup_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class PreviewCPP : AsyncPackage
    {
        internal static class CONSTANT
        {
            public const string APPLICATION = "Visual Studio";
            public const string COMPANY = "Nesviatypaskha Oleksii";
            public const string COPYRIGHT = "Copyright (c) 2020 by Nesviatypaskha Oleksii. All rights reserved.";
            public const string DESCRIPTION = "Quick preview of C, CPP, H and HPP files";
            public const string GUID = "42533993-FCE3-42E5-85C5-F339A242EF10";
            public const string HOST = "MetaOutput";
            public const string NAME = "Preview-CPP";
            public const string VERSION = "0.8.7";
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            extension.AnyPreview.Connect(CONSTANT.APPLICATION, CONSTANT.NAME);
            extension.AnyPreview.Register(".C",   new preview.CPP());
            extension.AnyPreview.Register(".CPP", new preview.CPP());
            extension.AnyPreview.Register(".H",   new preview.CPP());
            extension.AnyPreview.Register(".HPP", new preview.CPP());
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }

        protected override int QueryClose(out bool canClose)
        {
            extension.AnyPreview.Disconnect();
            canClose = true;
            return VSConstants.S_OK;
        }
    }
}
