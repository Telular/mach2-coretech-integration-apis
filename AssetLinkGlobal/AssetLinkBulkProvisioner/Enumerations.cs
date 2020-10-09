using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetLinkBulkProvisioner
{
    enum ExitCode : int
    {
        Success = 0,
        Failure = 1,
        InvalidFilename = 2,
        InvalidCommandLineArgs = 3,
        NoMutexAvaliable = 4,
        InvalidCsvReader = 5,
        InvalidFileOperation = 6,
        MissingUrnAndOrSerialNumber = 7,
        CommonBusinesApiFailure = 8,
        IvalidCarrierName = 9,
        GeneralException = 11,
        UnknownError = 12
    }
}
