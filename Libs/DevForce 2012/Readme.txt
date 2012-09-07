Requirements
 - VS2012 RC
 - Only .NET 4.5, Silverlight 5 and Metro projects are supported. 
 - Windows 8 RP (if developing for Metro)


1 - Install license key to registry
    You can run the AddLicense.reg file as admin to install a temporary Universal license.  If running 32-bit, edit the file
    or manually add the key to the registry based on the info in the file.

2 - Run IdeaBlade.VisualStudio.OM.Designer.11.0.vsix to install the EDM Designer extension

3 - Optional - run IdeaBlade.VisualStudio.TemplateInstaller.vsix to install the DevForce project and item templates

4 - Optional - For Code First build time support
     - Run InstallTargetsForTesting.bat as admin ... if this doesn't work you can manually copy the necessary files, or not use Code First right now.

5 - Optional - To allow The "Add references" dialog to find the DF assemblies without you having to browse for them, edit registry:

    Add a key pointing to the folder holding the desktop assemblies under:
         HKEY_LOCAL_MACHINE\SOFTWARE\<Wow6432Node>\Microsoft\.NETFramework\v4.5.50501\AssemblyFoldersEx

    And for SL assemblies under:
         HKEY_LOCAL_MACHINE\SOFTWARE\<Wow6432Node>\Microsoft\Microsoft SDKs\Silverlight\v5.0\AssemblyFoldersEx

6 - You may get a security warning in Visual Studio telling you to unblock the dlls. To do this, right click each dll in Windows,
    select 'Properties', and hit the 'Unblock' button at the bottom.

- Note all assemblies are debug builds.

To upgrade a DF2010 model -
   - Re-target the project to .NET 4.5. (under project properties, "target framework")
   - Open the EDMX in the designer
   - Delete the edmx.tt file
   - Save the model - this will regenerate the code

Silverlight 5 -
  - The Async Targeting Pack for VS11 is required - http://nuget.org/packages/Microsoft.CompilerServices.AsyncTargetingPack
     "Install-Package Microsoft.CompilerServices.AsyncTargetingPack"

Metro -
  - MEF for Metro is required - http://nuget.org/packages/Microsoft.Composition/1.0.13-rc
    "Install-Package Microsoft.Composition -Pre"
  - We do have a C# template which will create a Metro client project and a web application project for the EntityServer.
    You'll need to manually add the DevForce Metro assembly references.  
  - Note that an app.config is used for IdeaBladeConfig info (like the server address), and must be marked as an embedded resource.
  - By default, the "Internet (Client)" capability is enabled, and this is all that's needed to test a DF app locally.
  - If you try to deploy a Metro client to talk to an EntityServer elsewhere on the network, also give the application the 
    "Home or Work Networking" capability (and check your firewall).
  - For Metro unit testing, you must also manually add a loopback exception:
     CheckNetIsolation.exe LoopbackExempt -a -n=<package-family-name>
     You can find the package family name in the manifest.
  - Metro apps can only be developed and deployed on Win 8.  You'll need a developer license too (just accept the prompts in VS).
  - Anonymous types are not currently supported.  Use a projection into a known type instead.
  - Code First is not currently supported. 

Code First -
  - The build time support is the same as in DF2010, and not robust in VS2012.  If you have problems, make
    sure only one instance of VS2012 is running, and build each model project individually.
  - You should reference the EF 5 RC - http://nuget.org/packages/EntityFramework/5.0.0-rc
    "Install-Package EntityFramework -Pre"
  - Remember to install PostSharp.
     

Changes and breaking changes

 - The biggest change is async/await support.  So queries and saves look like this:

    var products = await mgr.Products.ExecuteAsync();
    var saveResult = await mgr.SaveChangesAsync();

 - If you'd like additional information about a query result, both sync and async "ExecuteQueryForResult" are provided:
     var queryResult = await mgr.ExecuteQueryForResultAsync(mgr.Customers);
   The QueryResult has the information which we used to provide for async queries, eg WasFetched, ChangedEntities, etc.

 - If you want to use the old async api or need backward compatibility, add a reference to IdeaBlade.EntityModel.Compat 
   (or IdeaBlade.EntityModel.Compat.SL) and a using for IdeaBlade.EntityModel.Compat, and you'll get extension methods giving
   you the old api.  You'll also see Coroutine support here. (This is currently unavailable.)  

 - Most other breaking changes are due to the removal of anything marked obsolete in DF2010.  So there's no DefaultManager, no
   Login/Logout on the EntityManager, and more.  

 - Push is not available.  The IdeaBlade.Windows.SL assembly (for the EntityQueryPagedCollectioniew and ObjectDataSource) is also not available.
   You should be using the EntityQueryPager now, which has also been available in DF2010 for a while.




  




 