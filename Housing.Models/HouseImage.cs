using System;

namespace Housing.Models
{
    public class HouseImage
    {
        public HouseImage(Uri url, ImageTypes type = ImageTypes.Unknown)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Type = type;
        }

        public Uri Url { get; }
        public ImageTypes Type { get; }
    }
}
