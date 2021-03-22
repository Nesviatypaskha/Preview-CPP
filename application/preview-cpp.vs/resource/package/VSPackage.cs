
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace resource.package
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(CONSTANT.NAME, CONSTANT.DESCRIPTION, CONSTANT.VERSION)]
    [Guid(CONSTANT.GUID)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.FirstLaunchSetup_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class PreviewCSPackage : AsyncPackage
    {
        internal static class CONSTANT
        {
            public const string COPYRIGHT = "Copyright (c) 2020 by Nesviatypaskha Oleksii. All rights reserved.";
            public const string DESCRIPTION = "Quick preview of CPP files";
            public const string EXTENSION1 = ".C";
            public const string EXTENSION2 = ".CPP";
            public const string EXTENSION3 = ".H";
            public const string EXTENSION4 = ".HPP";
            public const string GUID = "42533993-FCE3-42E5-85C5-F339A242EF10";
            public const string NAME = "Preview-CPP";
            public const string VERSION = "0.5.1";
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            {
                cartridge.AnyPreview.Connect();
                cartridge.AnyPreview.Register(cartridge.AnyPreview.MODE.PREVIEW, CONSTANT.EXTENSION1, new preview.CPP());
                cartridge.AnyPreview.Register(cartridge.AnyPreview.MODE.PREVIEW, CONSTANT.EXTENSION2, new preview.CPP());
                cartridge.AnyPreview.Register(cartridge.AnyPreview.MODE.PREVIEW, CONSTANT.EXTENSION3, new preview.CPP());
                cartridge.AnyPreview.Register(cartridge.AnyPreview.MODE.PREVIEW, CONSTANT.EXTENSION4, new preview.CPP());
            }
        }

        protected override int QueryClose(out bool canClose)
        {
            {
                cartridge.AnyPreview.Disconnect();
                canClose = true;
            }
            return VSConstants.S_OK;
        }
    }
}
