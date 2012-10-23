<script language="C#" runat="server">

    /// <summary>
    /// Perform application start activities.  
    /// </summary>
    protected void Application_Start(Object sender, EventArgs e) {

      // To enable remote viewing of trace messages via the TraceViewer:
      // TODO:  Uncomment following line if you want to provide this feature.  
      //IdeaBlade.Core.TracePublisher.LocalInstance.MakeRemotable();        

      // If you don't want to create a .svc file for every EntityServer service, you
      // can instead register a "virtual path provider".       
      // You do not need to supply .svc files for the EntityService and EntityServer services
      // when the provider is registered.      
        System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(new IdeaBlade.EntityModel.Web.ServiceVirtualPathProvider());
  
    }			
</script>
