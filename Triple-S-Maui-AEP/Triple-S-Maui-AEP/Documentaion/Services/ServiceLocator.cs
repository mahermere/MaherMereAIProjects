using System;
using TripleS.SOA.AEP.UI.Services;
using TripleS.SOA.AEP.UI.Platforms;


namespace TripleS.SOA.AEP.UI.Services
{
    public static class ServiceLocator
    {
        static ServiceLocator() =>
#if ANDROID
            PdfOpener = new TripleS.SOA.AEP.UI.Platforms.PdfOpener();
#elif WINDOWS
            PdfOpener = new TripleS.SOA.AEP.UI.Platforms.PdfOpener();
#else
            PdfOpener = new TripleS.SOA.AEP.UI.Platforms.PdfOpener();
#endif

        public static IPdfOpener PdfOpener { get; set; }
    }
}
