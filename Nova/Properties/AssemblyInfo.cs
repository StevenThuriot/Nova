#region License
// 
//  Copyright 2012 Steven Thuriot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
#endregion
using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: InternalsVisibleTo("Nova.Shell, PublicKey=\"0024000004800000940000000602000000240000525341310004000001000100556792329e7d6cd241381073e9a2272a3fd6692a5812eda3f0306b37f9a46719982184a50bcc909efe39da614ce5a1a359cdfd5177bd8b6c9ee67cae3e2cc77e5c00defa3de0c684c840d7175c676ecb29daa94b09f3994bb4fc0ec4bd81df17eb10ab247ec7e08379e16ecbfef9479b87c262695d69623fc2fe43d2465ab5bb\", PublicKeyToken=\"03252df91086a919\"")]

[assembly: AssemblyTitle("Nova")]
[assembly: AssemblyDescription("Small graphical framework to quickly start developing your apps without having to worry too much about controls and looks.")]
[assembly: AssemblyCompany("Steven Thuriot")]
[assembly: AssemblyProduct("Nova")]
[assembly: AssemblyCopyright("Thuriot.be")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
	//(used if a resource is not found in the page, 
	// or application resource dictionaries)
	ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
	//(used if a resource is not found in the page, 
	// app, or any theme specific resource dictionaries)
	)]


// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

[assembly: AssemblyVersion("0.5.0.0")]
[assembly: AssemblyInformationalVersion("0.5.0.0")]
[assembly: AssemblyFileVersion("0.5.0.0")]

[assembly: NeutralResourcesLanguage("en")]
[assembly: Guid("BA5AA402-F9A9-11E0-B218-E9C04824019B")]
