﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shuttle.Core.Threading {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Shuttle.Core.Threading.Resources", typeof(Resources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot remove or replace key &apos;{0}&apos;..
        /// </summary>
        public static string ImmutableKeyException {
            get {
                return ResourceManager.GetString("ImmutableKeyException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [processor thread executing] : managed thread id = {0} / processor type = {1}.
        /// </summary>
        public static string ProcessorExecuting {
            get {
                return ResourceManager.GetString("ProcessorExecuting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [processor thread active] : managed thread id = {0} / processor type = {1}.
        /// </summary>
        public static string ProcessorThreadActive {
            get {
                return ResourceManager.GetString("ProcessorThreadActive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Processor thread did not join within the required time..
        /// </summary>
        public static string ProcessorThreadJoinTimeoutException {
            get {
                return ResourceManager.GetString("ProcessorThreadJoinTimeoutException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The processor thread has not yet been started..
        /// </summary>
        public static string ProcessorThreadNotStartedException {
            get {
                return ResourceManager.GetString("ProcessorThreadNotStartedException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [processor thread starting] : managed thread id = {0} / processor type = {1}.
        /// </summary>
        public static string ProcessorThreadStarting {
            get {
                return ResourceManager.GetString("ProcessorThreadStarting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [processor thread stopped] : managed thread id = {0} / processor type = {1}.
        /// </summary>
        public static string ProcessorThreadStopped {
            get {
                return ResourceManager.GetString("ProcessorThreadStopped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [processor thread stopping] : managed thread id = {0} / processor type = {1}.
        /// </summary>
        public static string ProcessorThreadStopping {
            get {
                return ResourceManager.GetString("ProcessorThreadStopping", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No syncronization item with name &apos;{0}&apos; has been registered..
        /// </summary>
        public static string SynchronizationNameException {
            get {
                return ResourceManager.GetString("SynchronizationNameException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The queue handler configuration requires thread count of at least 1.  The input queue can not be processed..
        /// </summary>
        public static string ThreadCountZeroException {
            get {
                return ResourceManager.GetString("ThreadCountZeroException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Thread pool &apos;{0}&apos; has {1} successfully..
        /// </summary>
        public static string ThreadPoolStatusChange {
            get {
                return ResourceManager.GetString("ThreadPoolStatusChange", resourceCulture);
            }
        }
    }
}
