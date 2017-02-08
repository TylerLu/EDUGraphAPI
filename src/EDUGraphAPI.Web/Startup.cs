using Owin;

namespace EDUGraphAPI.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureIdentityAuth(app);
            ConfigureAADAuth(app);
            ConfigureIoC(app);
        }
    }
}
