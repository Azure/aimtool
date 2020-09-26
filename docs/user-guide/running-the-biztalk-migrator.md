# Running the BizTalk Migrator

This is the full list of command-line arguments currently supported by the BizTalk Migrator.

`aim [command] [options]`

#### Commands

| Command | Description |
| -------- | ------- |
| assess | Runs only the stages associated with assessment: Discover, Parse, Analyze, Report. |
| migrate | Runs all stages associated with migration: Discover, Parse, Analyze, Report, Convert, Verify. |
| convert | Runs only the stages associated with conversion: Convert, Verify. |
| verify | Runs only the stages associated with verification: Verify. |

#### Options

| Option | Description |
| -------- | ------- |
| -v, --verbose | Output verbose logging. |
| --verbose-level &lt;verbose-level&gt; | Specifies the level of verbose logging, if verbose logging is switched on using the -v option. Use '-' for minimum trace outputand '+' for full trace logging. Default is '-'. [default: -] |
| --no-abort | Don't abort on first exception encountered from a stage runner and allow subsequent stage runners to continue to run. Default is false. | 
| --abort-stage &lt;abort-stage&gt; | Abort at the end of a stage when any exception encountered from a stage runner. Default is not to abort at the end of a stage. This option, if used, must be used in conjunction with --no-abort. |
| -f, --find-path &lt;find-path&gt; | One or more paths to directories containing stage runners. Default find paths can be specified in the application configuration file (appsettings.json). This is an additive option, so paths specified in configuration file will be added to by paths specified on the command line. | 
| -p, --find-pattern &lt;find-pattern&gt; | Specify a search pattern for finding stage runner libraries. Use * (zero or more chars) and ? (exactly zero or one char) as wildcards. Default is "*.dll", however, this can be overridden in the application configuration file (appsettings.json). | 
| -a, --arg &lt;arg&gt; | Arbitrary arguments that are passed through to stage runners.  Format key=value. Multi-item values are accepted using a delimiter. Complex objects are supported by passing JSON string (use single quotes around the argument). | 
| --arg-delimiter &lt;arg-delimiter&gt; | Specifies a delimiter for a multi-value argument. Default is a pipe (\|) character. | 
| -w, --working-path &lt;working-path&gt; | Change the current working directory to a different path for the tool execution. Default is current directory. This option will affect any relative paths specified in other options. |
| -s, --state-path &lt;state-path&gt; | Path to store run state during execution of the tool. Default is the working directory. | 
| --save-state | Saves the runner execution state to disk before and after each stage runner. | 
| --save-stage-state | Saves the runner execution state to disk before and after each stage. | 
| --target &lt;Consumption\|Ise&gt; | Azure Integration Services target environment: Consumption or Ise (Integration Service Environment). | 
| --subscription-id &gt;subscription-id&gt; | Specifies the Azure Subscription ID to use for resource template rendering, if the templates require the subscription ID. | 
| --primary-region &lt;primary-region&gt; | Specifies the Azure Primary Region to use for resource template rendering, if the templates require the primary region. |
| --secondary-region &lt;secondary-region&gt; | Specifies the Azure Secondary Region to use for resource template rendering, if the templates require the secondary region as a paired region. |
| -e, --deployment-env &lt;deployment-env&gt; | Specifies the deployment environment to use for resource template rendering. Default is the 'dev' environment. |
| --unique-deployment-id &lt;unique-deployment-id&gt; | Specify a unique value to be used in the resource name when generating Azure resources. Set this to a short value, such as an abbreviated company name. Default is a random string. Max length is 5 characters. |
| --version | Show version information. |
| -?, -h, --help | Show help and usage information. |
| microsoft.biztalk.msidiscoverer.msifiles | One or more paths to MSI files, separated by the argument delimiter (default '\|' character). | 
| microsoft.biztalk.msidiscoverer.msidir | Path to a directory containing MSI files. |
| microsoft.biztalk.msidiscoverer.unpackdir | Path to a directory where the MSI files will be unpacked. |
| microsoft.biztalk.htmlreporter.reportfile | Path to a file where the HTML report will be written. |

### Example commands

Specify verbose logging:  
    `aim assess -v`

  Specify single MSI file:  
    `aim assess -a "microsoft.biztalk.msidiscoverer.msifiles=C:\msi\app.msi"`

  Specify multiple MSI files:  
    `aim assess -a "microsoft.biztalk.msidiscoverer.msifiles=C:\msi\app1.msi|C:\msi\app2.msi"`

  Specify directory containing MSI files:  
    `aim assess -a "microsoft.biztalk.msidiscoverer.msidir=C:\msi"`

  Specify directory containing MSI files with unpack directory:  
    `aim assess -a "microsoft.biztalk.msidiscoverer.msidir=C:\msi" -a "microsoft.biztalk.msidiscoverer.unpackdir=C:\temp"`

  Specify a custom report file name and location:  
    `aim assess -a "microsoft.biztalk.htmlreporter.reportfile=C:\reportoutput\report.html"`