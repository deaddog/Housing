using Autofac;
using Housing.Services.Boligsiden;
using Housing.Services.Dawa;
using Housing.Services.eTilbudsavis;
using Housing.Services.GoogleDirections;
using Housing.Services.TjekDitNet;
using System;

namespace Housing.Services
{
    public class ServicesModule : Module
    {
        private readonly string _googleApiKey;
        private readonly string _eTilbudApiKey;
        private readonly string _eTilbudSecretKey;

        public ServicesModule(string googleApiKey, string eTilbudApiKey, string eTilbudSecretKey)
        {
            _googleApiKey = googleApiKey ?? throw new ArgumentNullException(nameof(googleApiKey));
            _eTilbudApiKey = eTilbudApiKey ?? throw new ArgumentNullException(nameof(eTilbudApiKey));
            _eTilbudSecretKey = eTilbudSecretKey ?? throw new ArgumentNullException(nameof(eTilbudSecretKey));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BoligsidenRepository>().As<IBoligsidenRepository>().SingleInstance();
            builder.Register(_ => new GoogleDirectionsRepository(_googleApiKey)).As<IGoogleDirectionsRepository>().SingleInstance();
            builder.Register(_ => new eTilbudsavisRepository(_eTilbudApiKey, _eTilbudSecretKey)).As<IeTilbudsavisRepository>().SingleInstance();
            builder.RegisterType<DawaRepository>().As<IDawaRepository>().SingleInstance();
            builder.RegisterType<TjekDitNetRepository>().As<ITjekDitNetRepository>().SingleInstance();

            base.Load(builder);
        }
    }
}
