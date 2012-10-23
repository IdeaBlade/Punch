<script language="C#" runat="server">

    /// <summary>
    /// Perform application start activities.  
    /// </summary>
    protected void Application_Start(Object sender, EventArgs e) {

      // To enable remote viewing of trace messages via the TraceViewer:
      // TODO:  Comment/uncomment the following line as appropriate for your installation
      //IdeaBlade.Core.TracePublisher.LocalInstance.MakeRemotable();     
      
      // Register a virtual path provider for *.svc files.  
      // You do not need to supply .svc files for the EntityService and EntityServer services
      // when the provider is registered.      
      System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(new IdeaBlade.EntityModel.Web.ServiceVirtualPathProvider());     
    }			
</script>
