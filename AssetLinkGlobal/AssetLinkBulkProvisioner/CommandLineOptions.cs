using CommandLine;

namespace AssetLinkBulkProvisioner
{
    public sealed class CommandLineOptions
    {

        // NOTE: Omitting the "long name" on cmd line options, defaults the "long name" to the name of property.  
        // I don't do that on either of these properties.

        [Option('f', "filename", Required = true,
          HelpText = "Name of input file.")]
        public string InputFilename { get; set; }        

    }
}
