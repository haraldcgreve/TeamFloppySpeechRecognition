using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SpeechRecognition.Startup))]
namespace SpeechRecognition
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
