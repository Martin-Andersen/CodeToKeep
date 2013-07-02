namespace SomethingBlue.IoC
{
    /* Bootstrapper for wpf with SimpleInjector as Ioc
    public class AppBootstrapper : ReactiveObject
    {
        public AppBootstrapper(AccessInfo userInit)
        {
            SetupRxAppConfigureServiceLocatorForSimpleInjector(userInit);
        }

        private void SetupRxAppConfigureServiceLocatorForSimpleInjector(AccessInfo userInit)
        {
            RxApp.GetFieldNameForPropertyNameFunc = x =>
            {
                char[] arr = x.ToCharArray();
                arr[0] = char.ToLower(arr[0]);
                return '_' + new String(arr);
            };

            // Register types for this app
            var container = CreateContainer(userInit);

            // RxUI types
            var collectionRegistrations = new Dictionary<Type, List<Type>>();
            collectionRegistrations[typeof(ICreatesObservableForProperty)] = new List<Type>();
            collectionRegistrations[typeof(IDefaultPropertyBindingProvider)] = new List<Type>();
            collectionRegistrations[typeof(IBindingTypeConverter)] = new List<Type>();
            collectionRegistrations[typeof(IPropertyBindingHook)] = new List<Type>();
            collectionRegistrations[typeof(ICreatesCommandBinding)] = new List<Type>();

            // Set up NInject to do DI
            RxApp.ConfigureServiceLocator(
                getService: (serviceType, key) =>
                {
                    if (key != null) throw new NotSupportedException();
                    return container.GetInstance(serviceType);
                },
                getAllServices: (serviceType, key) =>
                {
                    if (key != null) throw new NotSupportedException();
                    return container.GetAllInstances(serviceType);
                },
                register: (implementationType, serviceType, key) =>
                {
                    if (key != null) throw new NotSupportedException();

                    // Rx expects the container to allow registering multiple implementations of the 
                    // same abstraction, to be resolved as collection later on.
                    if (collectionRegistrations.ContainsKey(serviceType))
                        collectionRegistrations[serviceType].Add(implementationType);
                    else
                        container.Register(serviceType, implementationType, Lifestyle.Transient);
                });

            // Register RxUI types
            foreach (var pair in collectionRegistrations)
                container.RegisterAll(pair.Key, pair.Value);

            container.Verify();
        }

        private Container CreateContainer(AccessInfo accessInfo)
        {
            var con = new Container();
            con.Options.AllowOverridingRegistrations = true;
#if FAKE
            con.RegisterSingle<IPropertiesRepository>(() => new PropertiesRepositoryFake(accessInfo));
#else
            con.RegisterSingle<IPropertiesRepository>(() => new PropertiesRepository(accessInfo));
#endif
            con.RegisterSingle<IDocumentViewModel, DocumentViewModel>();
            con.RegisterSingle<IPropertyNameValidator, PropertyNameValidator>();
            con.RegisterSingle<IToolbarViewModel, ToolbarViewModel>();
            con.RegisterSingle<IDocumentOutlineViewModel, DocumentOutlineViewModel>();
            con.RegisterSingle<IFormCanvasViewModel, FormCanvasViewModel>();
            con.RegisterSingle<IPropertyEditorViewModel, PropertyEditorViewModel>();
            con.RegisterSingle<OpenDocumentDlgViewModel>();
            con.RegisterSingle<IconViewModel>();
            con.RegisterSingle<RubberbandHelper>();
            con.RegisterSingle<IApplicationCommands, ApplicationCommands>();


            // Register all IValidators
            con.RegisterManyForOpenGeneric(typeof(IValidator<>), typeof(ButtonValidator).Assembly);
            // Null validator object result is always true, Implementation of the Null Object pattern.
            con.RegisterOpenGeneric(typeof(IValidator<>), typeof(AlwaysValidValidator<>));

            return con;
        }
    }
     */
}