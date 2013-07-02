using System;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Interop;

namespace SomethingBlue.Clipboard
{
    public class ClipboardHelper<T> :  IDisposable where T : class
    {
        /// <summary>
        /// Next clipboard viewer window 
        /// </summary>
        private IntPtr _hWndNextViewer = IntPtr.Zero;

        /// <summary>
        /// The <see cref="HwndSource"/> for this window.
        /// </summary>
        private HwndSource _hWndSource;

        private readonly string _clipBoardFormatToWatch;
#pragma warning disable 649
        private bool _hasSupportedClipboardFormat;
#pragma warning restore 649

        public ClipboardHelper(Window window = null)
        {
            _clipBoardFormatToWatch = typeof(T).Name;
            InitClipboardViewer(window);
            //MessageBus.Current.RegisterMessageSource(this.WhenAny(x => x.HasSupportedClipboardFormat, x => x.Value), Contracts.CanPasteFromClipboard);
        }

        public void InitClipboardViewer(Window window)
        {
            var wih = new WindowInteropHelper(window);
            _hWndSource = HwndSource.FromHwnd(wih.Handle);

            if (_hWndSource == null) return;

            _hWndSource.AddHook(WinProc);   // start processing window messages
            _hWndNextViewer = Win32.SetClipboardViewer(_hWndSource.Handle);   // set this window as a viewer
        }

        private void CloseClipboardViewer()
        {
            // remove this window from the clipboard viewer chain
            Win32.ChangeClipboardChain(_hWndSource.Handle, _hWndNextViewer);

            _hWndNextViewer = IntPtr.Zero;
            _hWndSource.RemoveHook(WinProc);
        }

        private IntPtr WinProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32.WM_CHANGECBCHAIN:
                    if (wParam == _hWndNextViewer)
                    {
                        // clipboard viewer chain changed, need to fix it.
                        _hWndNextViewer = lParam;
                    }
                    else if (_hWndNextViewer != IntPtr.Zero)
                    {
                        // pass the message to the next viewer.
                        Win32.SendMessage(_hWndNextViewer, msg, wParam, lParam);
                    }
                    break;

                case Win32.WM_DRAWCLIPBOARD:
                    // clipboard content changed
                    HasSupportedClipboardFormat = System.Windows.Clipboard.ContainsData(_clipBoardFormatToWatch);
                    // pass the message to the next viewer.
                    Win32.SendMessage(_hWndNextViewer, msg, wParam, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        public bool HasSupportedClipboardFormat
        {
            get { return _hasSupportedClipboardFormat; }
            set { _hasSupportedClipboardFormat = value; }
            //set { this.RaiseAndSetIfChanged(value); Debug.WriteLine("HasSupportedClipboardFormat {0}", value); }
        }

        public void SetClipboardData(T data)
        {
            var stream = new MemoryStream();
            var serializer = new DataContractSerializer(typeof(T));
            serializer.WriteObject(stream, data);
            var dataObj = new DataObject();
            dataObj.SetData(_clipBoardFormatToWatch, stream, autoConvert: false);
            System.Windows.Clipboard.SetDataObject(dataObj, copy: false);
        }

        public T GetClipboardData()
        {
            T data = null;
            var dataObj = System.Windows.Clipboard.GetDataObject();
            if (dataObj != null && dataObj.GetDataPresent(_clipBoardFormatToWatch))
            {
                var serializer = new DataContractSerializer(typeof(T));
                var ms = dataObj.GetData(_clipBoardFormatToWatch) as MemoryStream;
                if (ms != null) data = (T)serializer.ReadObject(ms);
            }

            return data;
        }

        public void Dispose()
        {
            CloseClipboardViewer();
        }

    }
}