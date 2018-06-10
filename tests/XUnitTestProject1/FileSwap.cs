using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTestProject1
{
    public class FileSwap : IDisposable
    {
        readonly string _backup;
        readonly string _source;
        public FileSwap(string source, string target)
        {
            _source = source;
            _backup = Path.GetFileName(source) + ".bak";
            if (File.Exists(_backup)) File.Delete(_backup);
            File.Replace(target, source, _backup);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                File.Delete(_source);
                File.Move(_backup, _source);

                disposedValue = true;
            }
        }
        
        ~FileSwap()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
