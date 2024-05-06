WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDeliveryApi()
    .AddComposers()
    .AddMemberCustomRemoteAuthentication()
    .Build();

WebApplication app = builder.Build();

await app.BootUmbracoAsync();

// Set security headers
app.UseHsts();
app.UseXfo(options => options.SameOrigin());
app.UseXContentTypeOptions();

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseInstallerEndpoints();
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
