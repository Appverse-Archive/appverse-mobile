using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Core.AppLoader;

namespace Unity.Platform.Windows
{
    public class WindowsAppLoader : AbstractLoader
    {
        public override void DeleteModules(Module[] modules)
        {
            throw new NotImplementedException();
        }

        public override Module[] ListInstalledModules()
        {
            throw new NotImplementedException();
        }

        public override void LoadModule(Module module, ModuleParam[] moduleParams)
        {
            throw new NotImplementedException();
        }

        public override void LoadModule(Module module, ModuleParam[] moduleParams, bool autoUpdate)
        {
            throw new NotImplementedException();
        }

        public override void UpdateModule(Module module, string callbackId)
        {
            throw new NotImplementedException();
        }

        public override void UpdateModules(Module[] modules, string callbackId)
        {
            throw new NotImplementedException();
        }

        public override Unity.Core.Storage.FileSystem.IFileSystem GetFileSystemService()
        {
            throw new NotImplementedException();
        }

        public override Unity.Core.IO.IIo GetIOService()
        {
            throw new NotImplementedException();
        }

        public override Unity.Core.Notification.INotification GetNotificationService()
        {
            throw new NotImplementedException();
        }

        public override bool StoreModuleZipFile(Module module, string tempFile)
        {
            throw new NotImplementedException();
        }
    }
}
