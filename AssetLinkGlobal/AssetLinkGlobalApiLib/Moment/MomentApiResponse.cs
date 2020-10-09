using System.Collections.Generic;

namespace AssetLinkGlobalApiLib.Moment
{
    using AssetLinkGlobalApiLib.Moment.Models;

    public class MomentApiResponse : BaseApiResponse
    {
        public List<Moment> Data { get; set; }
    }
}
