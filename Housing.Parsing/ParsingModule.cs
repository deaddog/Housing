using Autofac;
using System;
using System.Linq;

namespace Housing.Parsing
{
    public class ParsingModule : Module
    {
        private readonly System.Reflection.Assembly _interfaceAssembly;
        private readonly Type _interfaceType;

        public ParsingModule()
        {
            _interfaceType = typeof(IEstateAgentHouseParser);
            _interfaceAssembly = _interfaceType.Assembly;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EstateAgentHouseParser>().AsSelf();

            builder.RegisterAssemblyTypes(_interfaceAssembly).Where(IsEstateAgentImplementation).As<IEstateAgentHouseParser>();
            builder.RegisterAdapter<IEstateAgentHouseParser, IHouseParser>((container, parser) => container.Resolve<EstateAgentHouseParser.Factory>()(parser)).SingleInstance();

            base.Load(builder);
        }

        private bool IsEstateAgentImplementation(Type type)
        {
            if (!type.GetInterfaces().Contains(_interfaceType))
                return false;

            if (!type.IsClass)
                return false;

            if (type.IsAbstract)
                return false;

            return true;
        }
    }
}
