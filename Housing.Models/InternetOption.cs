using System;

namespace Housing.Models
{
    public class InternetOption
    {
        public InternetOption(double downloadMbit, double uploadMbit, InternetConnectionTypes connectionType)
        {
            if (downloadMbit < 0)
                throw new ArgumentOutOfRangeException(nameof(downloadMbit), "Internet download speed cannot be negative.");
            if (uploadMbit < 0)
                throw new ArgumentOutOfRangeException(nameof(uploadMbit), "Internet upload speed cannot be negative.");

            DownloadMbit = downloadMbit;
            UploadMbit = uploadMbit;
            ConnectionType = connectionType;
        }

        public double DownloadMbit { get; }
        public double UploadMbit { get; }

        public InternetConnectionTypes ConnectionType { get; }
    }
}
