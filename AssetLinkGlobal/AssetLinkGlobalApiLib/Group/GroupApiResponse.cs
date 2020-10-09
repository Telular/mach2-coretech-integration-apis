using System.Collections.Generic;

namespace AssetLinkGlobalApiLib.Group
{
    using AssetLinkGlobalApiLib.Group.Models;

    public class GroupApiResponse : BaseApiResponse
    {
        public List<Group> Data { get; set; }
    }
}
