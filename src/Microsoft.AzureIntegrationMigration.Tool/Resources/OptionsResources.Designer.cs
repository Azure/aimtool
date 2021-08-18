﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.AzureIntegrationMigration.Tool.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class OptionsResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal OptionsResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.AzureIntegrationMigration.Tool.Resources.OptionsResources", typeof(OptionsResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Abort at the end of a stage when any exception encountered from a stage runner.  Default is not to abort at the end of a stage.  This option, if used, must be used in conjunction with --no-abort..
        /// </summary>
        internal static string AbortStageOptionDescription {
            get {
                return ResourceManager.GetString("AbortStageOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --abort-stage.
        /// </summary>
        internal static string AbortStageOptionLong {
            get {
                return ResourceManager.GetString("AbortStageOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specifies a delimiter for a multi-value argument.  Default is a pipe (|) character..
        /// </summary>
        internal static string ArgumentDelimiterOptionDescription {
            get {
                return ResourceManager.GetString("ArgumentDelimiterOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --arg-delimiter.
        /// </summary>
        internal static string ArgumentDelimiterOptionLong {
            get {
                return ResourceManager.GetString("ArgumentDelimiterOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Arbitrary arguments that are passed through to stage runners.  Format key=value.  Multi-item values are accepted using a delimiter.  Complex objects are supported by passing JSON string (use single quotes around the argument)..
        /// </summary>
        internal static string ArgumentsOptionDescription {
            get {
                return ResourceManager.GetString("ArgumentsOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --arg.
        /// </summary>
        internal static string ArgumentsOptionLong {
            get {
                return ResourceManager.GetString("ArgumentsOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -a.
        /// </summary>
        internal static string ArgumentsOptionShort {
            get {
                return ResourceManager.GetString("ArgumentsOptionShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to assess.
        /// </summary>
        internal static string AssessVerb {
            get {
                return ResourceManager.GetString("AssessVerb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Runs only the stages associated with assessment: Discover, Parse, Analyze, Report..
        /// </summary>
        internal static string AssessVerbDescription {
            get {
                return ResourceManager.GetString("AssessVerbDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specifies the Azure Primary Region to use for resource template rendering, if the templates require the primary region..
        /// </summary>
        internal static string AzurePrimaryRegionOptionDescription {
            get {
                return ResourceManager.GetString("AzurePrimaryRegionOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --primary-region.
        /// </summary>
        internal static string AzurePrimaryRegionOptionLong {
            get {
                return ResourceManager.GetString("AzurePrimaryRegionOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specifies the Azure Secondary Region to use for resource template rendering, if the templates require the secondary region as a paired region..
        /// </summary>
        internal static string AzureSecondaryRegionOptionDescription {
            get {
                return ResourceManager.GetString("AzureSecondaryRegionOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --secondary-region.
        /// </summary>
        internal static string AzureSecondaryRegionOptionLong {
            get {
                return ResourceManager.GetString("AzureSecondaryRegionOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specifies the Azure Subscription ID to use for resource template rendering, if the templates require the subscription ID..
        /// </summary>
        internal static string AzureSubscriptionIdOptionDescription {
            get {
                return ResourceManager.GetString("AzureSubscriptionIdOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --subscription-id.
        /// </summary>
        internal static string AzureSubscriptionIdOptionLong {
            get {
                return ResourceManager.GetString("AzureSubscriptionIdOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Azure Integration Services target environment: Consumption or Standard..
        /// </summary>
        internal static string AzureTargetOptionDescription {
            get {
                return ResourceManager.GetString("AzureTargetOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --target.
        /// </summary>
        internal static string AzureTargetOptionLong {
            get {
                return ResourceManager.GetString("AzureTargetOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Path to store the converted output.  Default is the working directory..
        /// </summary>
        internal static string ConversionPathOptionDescription {
            get {
                return ResourceManager.GetString("ConversionPathOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --conversion-path.
        /// </summary>
        internal static string ConversionPathOptionLong {
            get {
                return ResourceManager.GetString("ConversionPathOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -c.
        /// </summary>
        internal static string ConversionPathOptionShort {
            get {
                return ResourceManager.GetString("ConversionPathOptionShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to convert.
        /// </summary>
        internal static string ConvertVerb {
            get {
                return ResourceManager.GetString("ConvertVerb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Runs only the stages associated with conversion: Convert, Verify..
        /// </summary>
        internal static string ConvertVerbDescription {
            get {
                return ResourceManager.GetString("ConvertVerbDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specifies the deployment environment to use for resource template rendering.  Default is the &apos;dev&apos; environment..
        /// </summary>
        internal static string DeploymentEnvOptionDescription {
            get {
                return ResourceManager.GetString("DeploymentEnvOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --deployment-env.
        /// </summary>
        internal static string DeploymentEnvOptionLong {
            get {
                return ResourceManager.GetString("DeploymentEnvOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -e.
        /// </summary>
        internal static string DeploymentEnvOptionShort {
            get {
                return ResourceManager.GetString("DeploymentEnvOptionShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to One or more paths to directories containing stage runners.  Default find paths can be specified in the application configuration file (appsettings.json).  This is an additive option, so paths specified in configuration file will be added to by paths specified on the command line..
        /// </summary>
        internal static string FindPathsOptionDescription {
            get {
                return ResourceManager.GetString("FindPathsOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --find-path.
        /// </summary>
        internal static string FindPathsOptionLong {
            get {
                return ResourceManager.GetString("FindPathsOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -f.
        /// </summary>
        internal static string FindPathsOptionShort {
            get {
                return ResourceManager.GetString("FindPathsOptionShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specify a search pattern for finding stage runner libraries.  Use * (zero or more chars) and ? (exactly zero or one char) as wildcards.  Default is &quot;*.dll&quot;, however, this can be overridden in the application configuration file (appsettings.json)..
        /// </summary>
        internal static string FindPatternOptionDescription {
            get {
                return ResourceManager.GetString("FindPatternOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --find-pattern.
        /// </summary>
        internal static string FindPatternOptionLong {
            get {
                return ResourceManager.GetString("FindPatternOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -p.
        /// </summary>
        internal static string FindPatternOptionShort {
            get {
                return ResourceManager.GetString("FindPatternOptionShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to migrate.
        /// </summary>
        internal static string MigrateVerb {
            get {
                return ResourceManager.GetString("MigrateVerb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Runs all stages associated with migration: Discover, Parse, Analyze, Report, Convert, Verify..
        /// </summary>
        internal static string MigrateVerbDescription {
            get {
                return ResourceManager.GetString("MigrateVerbDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Path to an exported model from a previous assessment..
        /// </summary>
        internal static string ModelPathOptionDescription {
            get {
                return ResourceManager.GetString("ModelPathOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --model.
        /// </summary>
        internal static string ModelPathOptionLong {
            get {
                return ResourceManager.GetString("ModelPathOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -m.
        /// </summary>
        internal static string ModelPathOptionShort {
            get {
                return ResourceManager.GetString("ModelPathOptionShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Don&apos;t abort on first exception encountered from a stage runner and allow subsequent stage runners to continue to run.  Default is false..
        /// </summary>
        internal static string NoAbortOptionDescription {
            get {
                return ResourceManager.GetString("NoAbortOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --no-abort.
        /// </summary>
        internal static string NoAbortOptionLong {
            get {
                return ResourceManager.GetString("NoAbortOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Path to a file that will be created with the saved model.  If the file already exists it will be overwritten..
        /// </summary>
        internal static string OutputModelPathDescription {
            get {
                return ResourceManager.GetString("OutputModelPathDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --output-model.
        /// </summary>
        internal static string OutputModelPathLong {
            get {
                return ResourceManager.GetString("OutputModelPathLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -o.
        /// </summary>
        internal static string OutputModelPathShort {
            get {
                return ResourceManager.GetString("OutputModelPathShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Saves the runner execution state to disk before and after each stage runner..
        /// </summary>
        internal static string SaveStageRunnerStateOptionDescription {
            get {
                return ResourceManager.GetString("SaveStageRunnerStateOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --save-state.
        /// </summary>
        internal static string SaveStageRunnerStateOptionLong {
            get {
                return ResourceManager.GetString("SaveStageRunnerStateOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Saves the runner execution state to disk before and after each stage..
        /// </summary>
        internal static string SaveStageStateOptionDescription {
            get {
                return ResourceManager.GetString("SaveStageStateOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --save-stage-state.
        /// </summary>
        internal static string SaveStageStateOptionLong {
            get {
                return ResourceManager.GetString("SaveStageStateOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Path to store run state during execution of the tool.  Default is the working directory..
        /// </summary>
        internal static string StatePathOptionDescription {
            get {
                return ResourceManager.GetString("StatePathOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --state-path.
        /// </summary>
        internal static string StatePathOptionLong {
            get {
                return ResourceManager.GetString("StatePathOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -s.
        /// </summary>
        internal static string StatePathOptionShort {
            get {
                return ResourceManager.GetString("StatePathOptionShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Path to a directory containing the resource template configuration files.  The configuration files are stored as Liquid templates and are rendered by the tool into YAML configuration files.  Default is the working directory unless overridden by a value specified in the application configuration file (appsettings.json)..
        /// </summary>
        internal static string TemplateConfigPathOptionDescription {
            get {
                return ResourceManager.GetString("TemplateConfigPathOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --template-config-path.
        /// </summary>
        internal static string TemplateConfigPathOptionLong {
            get {
                return ResourceManager.GetString("TemplateConfigPathOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -n.
        /// </summary>
        internal static string TemplateConfigPathOptionShort {
            get {
                return ResourceManager.GetString("TemplateConfigPathOptionShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to One or more paths to directories containing the resource template files.  The files are stored as Liquid templates and they represent individual artifacts used to build an AIS solution from the source application.  The Liquid syntax is converted by the tool and the rendered output is stored in the conversion path.  Default is the working directory unless overridden by a value specified in the application configuration file (appsettings.json)..
        /// </summary>
        internal static string TemplatePathOptionDescription {
            get {
                return ResourceManager.GetString("TemplatePathOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --template-path.
        /// </summary>
        internal static string TemplatePathOptionLong {
            get {
                return ResourceManager.GetString("TemplatePathOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -t.
        /// </summary>
        internal static string TemplatePathOptionShort {
            get {
                return ResourceManager.GetString("TemplatePathOptionShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specify a unique value to be used in the resource name when generating Azure resources.  Set this to a short value, such as an abbreviated company name.  Default is a random string.  Max length is {0} characters..
        /// </summary>
        internal static string UniqueDeploymentIdOptionDescription {
            get {
                return ResourceManager.GetString("UniqueDeploymentIdOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --unique-deployment-id.
        /// </summary>
        internal static string UniqueDeploymentIdOptionLong {
            get {
                return ResourceManager.GetString("UniqueDeploymentIdOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specifies the level of verbose logging, if verbose logging is switched on using the -v option.  Use &apos;-&apos; for minimum trace output and &apos;+&apos; for full trace logging.  Default is &apos;-&apos;..
        /// </summary>
        internal static string VerboseLevelOptionDescription {
            get {
                return ResourceManager.GetString("VerboseLevelOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --verbose-level.
        /// </summary>
        internal static string VerboseLevelOptionLong {
            get {
                return ResourceManager.GetString("VerboseLevelOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Output verbose logging..
        /// </summary>
        internal static string VerboseOptionDescription {
            get {
                return ResourceManager.GetString("VerboseOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --verbose.
        /// </summary>
        internal static string VerboseOptionLong {
            get {
                return ResourceManager.GetString("VerboseOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -v.
        /// </summary>
        internal static string VerboseOptionShort {
            get {
                return ResourceManager.GetString("VerboseOptionShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to verify.
        /// </summary>
        internal static string VerifyVerb {
            get {
                return ResourceManager.GetString("VerifyVerb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Runs only the stages associated with verification: Verify..
        /// </summary>
        internal static string VerifyVerbDescription {
            get {
                return ResourceManager.GetString("VerifyVerbDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Change the current working directory to a different path for the tool execution.  Default is current directory.  This option will affect any relative paths specified in other options..
        /// </summary>
        internal static string WorkingPathOptionDescription {
            get {
                return ResourceManager.GetString("WorkingPathOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --working-path.
        /// </summary>
        internal static string WorkingPathOptionLong {
            get {
                return ResourceManager.GetString("WorkingPathOptionLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -w.
        /// </summary>
        internal static string WorkingPathOptionShort {
            get {
                return ResourceManager.GetString("WorkingPathOptionShort", resourceCulture);
            }
        }
    }
}
