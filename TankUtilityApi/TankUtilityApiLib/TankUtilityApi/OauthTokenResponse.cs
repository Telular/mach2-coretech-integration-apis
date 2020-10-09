using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankUtilityApiLib.TankUtilityApi
{
    public class OauthTokenResponse
    {
        private DateTime? _expiresInDateTime = (DateTime?) null;

        public string access_token { get; set; }
        public string token_type { get; set; }
        public int? expires_in { get; set; }
        public int? expires_on { get; set; }
        public string refresh_token { get; set; }

        public DateTime? ExpiresOn
        {
            get
            {
                if ((expires_in != null) && (_expiresInDateTime == null))
                {
                    _expiresInDateTime = DateTime.UtcNow.AddSeconds((int)expires_in);
                }

                return _expiresInDateTime;

            }
        }
    }
}
