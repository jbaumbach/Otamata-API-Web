<%@ Application Language="C#" %>

<script runat="server">

    void RegisterRoutes(System.Web.Routing.RouteCollection routes)
    {
        //
        // URL Rewriting for some stuff.  Cool way to do REST-style urls.
        //
        routes.MapPageRoute("mobileSoundPlayer", "player/{encryptedId}", "~/player.aspx");
        
    }
    
    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup
        
        //
        // WCF API endpoints
        //
        System.Web.Routing.RouteTable.Routes.Add(new System.ServiceModel.Activation.ServiceRoute("services/soundssummary", new System.ServiceModel.Activation.WebServiceHostFactory(), typeof(OttaMatta.Application.Services.Sounds)));
        System.Web.Routing.RouteTable.Routes.Add(new System.ServiceModel.Activation.ServiceRoute("services/sounddata", new System.ServiceModel.Activation.WebServiceHostFactory(), typeof(OttaMatta.Application.Services.SoundData)));
        System.Web.Routing.RouteTable.Routes.Add(new System.ServiceModel.Activation.ServiceRoute("services/soundicon", new System.ServiceModel.Activation.WebServiceHostFactory(), typeof(OttaMatta.Application.Services.SoundIcon)));
        System.Web.Routing.RouteTable.Routes.Add(new System.ServiceModel.Activation.ServiceRoute("services/ratesound", new System.ServiceModel.Activation.WebServiceHostFactory(), typeof(OttaMatta.Application.Services.RateSound)));
        System.Web.Routing.RouteTable.Routes.Add(new System.ServiceModel.Activation.ServiceRoute("services/markinappropriate", new System.ServiceModel.Activation.WebServiceHostFactory(), typeof(OttaMatta.Application.Services.MarkInappropriate)));
        System.Web.Routing.RouteTable.Routes.Add(new System.ServiceModel.Activation.ServiceRoute("services/purchase", new System.ServiceModel.Activation.WebServiceHostFactory(), typeof(OttaMatta.Application.Services.PurchaseSound)));
        System.Web.Routing.RouteTable.Routes.Add(new System.ServiceModel.Activation.ServiceRoute("services/recordpurchase", new System.ServiceModel.Activation.WebServiceHostFactory(), typeof(OttaMatta.Application.Services.RecordPurchase)));
    
        //
        // URL Rewriting
        //
        RegisterRoutes(System.Web.Routing.RouteTable.Routes);
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
       
</script>
