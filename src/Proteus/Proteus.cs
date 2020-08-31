/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security;
using System.Threading.Tasks;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Plugins;
using TheXDS.Proteus.Protocols;
using static TheXDS.MCART.Objects;
using St = TheXDS.Proteus.Resources.Strings;

[assembly: Name("Proteus Core Library")]
[assembly: LicenseFile("License.txt")]

namespace TheXDS.Proteus
{
    /// <summary>
    /// Librería principal de control, gestión y acceso a bases de datos.
    /// </summary>
    public static class Proteus
    {
        private static readonly HashSet<ISettingsRepository> _externalRepositories = new HashSet<ISettingsRepository>(new TypeComparer());

        /// <summary>
        /// Enumera los repositorios de configuración definidos en los servicios.
        /// </summary>
        public static IEnumerable<ISettingsRepository>? SettingsRepositories => Services?.OfType<ISettingsRepository>().Concat(_externalRepositories);

        /// <summary>
        /// Registra un repositorio externo de configuración.
        /// </summary>
        /// <param name="repo">
        /// Repositorio a registrar.
        /// </param>
        public static void RegisterExternalSettingsRepo(ISettingsRepository repo)
        {
            _externalRepositories.Add(repo);
        }

        /// <summary>
        /// Inicializa la clase <see cref="Proteus"/>
        /// </summary>
        static Proteus()
        {
            NwClient.ConnectionLost += NwClient_ConnectionLost;
        }

        /// <summary>
        /// Obtiene una colección de semilladores de base de datos.
        /// </summary>
        public static IEnumerable<IAsyncDbSeeder> Seeders => FindAllObjects<IAsyncDbSeeder>();

        private static void NwClient_ConnectionLost(object? sender, EventArgs e)
        {
            if (AlertTarget is null)
            {
                MessageTarget?.Warning(St.NwClientConnLost);
            }
            else
            {
                AlertTarget.Alert(St.NwSvc, St.NwClientConnLost);
            }
        }

        /// <summary>
        /// Cliente de red de Proteus.
        /// </summary>
        public static SessionClient NwClient { get; private set; } = new SessionClient();

        /// <summary>
        /// Obtiene un valor que indica si el servicio de gestión de sesión
        /// se encuentra actualmente conectado.
        /// </summary>
        public static bool IsNwClientAlive => NwClient.IsAlive;

        /// <summary>
        /// Obtiene un valor que indica si Proteus ha sido inicializado.
        /// </summary>
        public static bool Initialized => !(Settings is null);

        /// <summary>
        /// Enumera los posibles modos de inicialización de Proteus.
        /// </summary>
        [Flags]
        public enum InitMode : byte
        {
            /// <summary>
            /// Únicamente cargar los servicios.
            /// </summary>
            [Name("Únicamente cargar")] LoadOnly,
            /// <summary>
            /// Cargar servicios, y realizar una comprobación de base de
            /// datos.
            /// </summary>
            [Name("Comprobar modelo de datos")] Check,
            /// <summary>
            /// Cargar servicios, y ejecutar semillas para las bases de
            /// datos.
            /// </summary>
            [Name("Cargar y ejecutar semillas")] Seed,
            /// <summary>
            /// Cargar servicios, y ejecutar sanidad sobre las bases de datos.
            /// </summary>
            [Name("Cargar y ejecutar sanidad")] Sanitize = 4,
            /// <summary>
            /// Cargar y ejecutar  funciones de verificación integradas.
            /// </summary>
            [Name("Cargar y ejecutar funciones de verificación integradas")] Verify = 8,
            /// <summary>
            /// Inicialización completa, incluyendo carga, comprobación y
            /// ejecución de semillas.
            /// </summary>
            [Name("Inicialización completa")] Full = byte.MaxValue
        }

        private static bool _interactive;

        private class ILoginSourceComparer : IEqualityComparer<ILoginSource>
        {
            public bool Equals(ILoginSource x, ILoginSource y)
            {
                return x.GetType() == y.GetType();
            }

            public int GetHashCode(ILoginSource obj)
            {
                return obj.GetType().GetHashCode();
            }
        }

        internal static readonly HashSet<ILoginSource> _loginSources = new HashSet<ILoginSource>(new ILoginSourceComparer());

        internal static HashSet<ExternalSeeder>? ExternalSeeders { get; private set; }

        /// <summary>
        /// Obtiene una colección de orígenes de inicio de sesión
        /// registrados en Proteus.
        /// </summary>
        public static IEnumerable<ILoginSource> LoginSources => _loginSources;

        /// <summary>
        /// Enumera los servicios cargados en el sistema.
        /// </summary>
        public static HashSet<Service>? Services { get; private set; }

        /// <summary>
        /// Repositorio de configuración utilizado para inciar y configurar
        /// Proteus al iniciar la aplicación.
        /// </summary>
        public static ISettings? Settings { get; private set; }

        /// <summary>
        /// Objeto que administrará las elevaciones de permisos en el
        /// sistema. Determinado de forma automática mediante escaneo de
        /// ensamblados.
        /// </summary>
        public static IElevator? Elevator { get; } = FindSingleObject<IElevator>();
        
        /// <summary>
        /// Obtiene o establece el objetivo de mensajes predeterminado de
        /// la aplicación.
        /// </summary>
        public static IMessageTarget? MessageTarget { get; set; }

        public static IInteractiveMessageTarget? InteractiveMt => MessageTarget as IInteractiveMessageTarget;

        /// <summary>
        /// Obtiene el objeto registrado como objetivo de alertas de la
        /// aplicación.
        /// </summary>
        public static IAlertTarget? AlertTarget { get; set; }

        /// <summary>
        /// Objeto que administrará las entradas de bitácora sobre los
        /// cambios ocurridos en la base de datos. Determinado de forma
        /// automática mediante escaneo de ensamblados.
        /// </summary>
        public static IDbLogger? Logger { get; private set; }

        /// <summary>
        /// Referencia a las credenciales que han iniciado sesión en Proteus.
        /// </summary>
        public static IProteusUserCredential? Session { get; private set; }

        /// <summary>
        /// Obtiene o establece el objeto que administrará el reporte de
        /// estados cuando se realicen operaciones de larga duración.
        /// </summary>
        public static IStatusReporter? CommonReporter { get; set; }

        /// <summary>
        /// Obtiene una referencia al servicio utilizado para controlar los
        /// modelos básicos de datos de Proteus.
        /// </summary>
        public static UserService? LogonService { get; private set; }

        /// <summary>
        /// Obtiene o establece un valor que determina si Proteus funcionará
        /// en un entorno interactivo o no.
        /// </summary>
        public static bool Interactive
        {
            get => _interactive;
            set
            {
                if (value || Session?.DefaultGranted == SecurityFlags.Root) _interactive = value;
            }
        }

        /// <summary>
        /// Obtiene una ruta predeterminada para la ubicación de un archivo
        /// de inicio de sesión por medio de token.
        /// </summary>
        public static string DefaultTokenFilePath => Path.Combine(new[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ProteusLogin.dat"
        });

        /// <summary>
        /// Registra un origen de inicio de sesión.
        /// </summary>
        /// <param name="source">
        /// Origen de inicio de sesión a registrar.
        /// </param>
        public static void RegisterLoginSource(ILoginSource source)
        {
            _loginSources.Add(source);
        }

        /// <summary>
        /// Inicializa Proteus buscando automáticamente la configuración a
        /// utilizar.
        /// </summary>
        /// <returns></returns>
        public static Task Init() => Init(FindFirstObject<ISettings>()!);
        
        /// <summary>
        /// Inicializa Proteus.
        /// </summary>
        /// <param name="settings">
        /// Configuración a utilizar para inicializar Proteus.
        /// </param>
        public static async Task Init(ISettings settings)
        {
            if (settings is null) throw new ArgumentNullException(nameof(settings));
            if (!(Settings is null)) throw new InvalidOperationException(St.ErrProteusInited);

            Settings = settings;
            var pl = new PluginLoader();
            Services = new HashSet<Service>(Load<Service>(pl));
            ExternalSeeders = new HashSet<ExternalSeeder>(Load<ExternalSeeder>(pl));
            Logger = Services.FirstOf<IDbLogger>() ?? FindSingleObject<IDbLogger>();
            LogonService = Services.FirstOf<UserService>() ?? FindSingleObject<UserService>()
                ?? throw new MissingTypeException(typeof(UserService));

            if (!Services.Contains(LogonService)) Services.Add(LogonService);
            await ServicesInitialization(settings.InitMode);
            try
            {
                if (Settings.UseNetworkServer) NwClient?.SetupListener();
            }
            catch (SocketException)
            {
                AlertTarget?.Alert(St.ErrCannotInitListener, string.Format(St.ErrUdpInUse,Settings.NetworkServerPort));
            }
            catch (Exception ex)
            {
                AlertTarget?.Alert(St.ErrCannotInitListener, ex.Message);
            }
        }

        private static async Task ServicesInitialization(InitMode mode)
        {
            bool[] seedRequired = await MkTask<bool>(mode, InitMode.Check, p => p.InitializeDatabaseAsync);

            if (mode.HasFlag(InitMode.Seed))
            {
                var l = new List<Task<Result>>();
                foreach (var (svc, seed) in Services.OrderBy(p => p.GetAttr<PriorityAttribute>()?.Value).Zip(seedRequired))
                {
                    l.Add(svc.RunSeedersAsync(seed));
                }
                if ((await Task.WhenAll(l)).Where(p => p != Result.Ok).Any())
                {
                    return;
                }
            }

            await MkTask(mode, InitMode.Sanitize, p => p.SanitizeAsync);
            await MkTask(mode, InitMode.Verify, p => p.VerifyAsync);
            await Task.WhenAll(Services.Select(j => j.AfterInitAsync()));
        }

        private static Task<T[]> MkTask<T>(InitMode mode, InitMode flag, Func<Service, Func<Task<T>>> action)
        {
            return mode.HasFlag(flag) ? Task.WhenAll(Services.Select(p => action(p).Invoke())) : Task.FromResult(new T[Services!.Count]);
        }
        private static Task MkTask(InitMode mode, InitMode flag, Func<Service, Func<Task>> action)
        {
            return mode.HasFlag(flag) ? Task.WhenAll(Services.Select(p => action(p).Invoke())) : Task.CompletedTask;
        }

        /// <summary>
        /// Enecuta una inicialización mínima segura de Proteus.
        /// </summary>
        /// <param name="settings">
        /// Set de configuraciones a utilizar para inicializar Proteus.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si Proteus se ha inicializado correctamente,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        public static async Task<bool> SafeInit(ISettings? settings)
        {
            if (settings is null) return false;

            DisposeSettings();
            LogonService = FindSingleObject<UserService>() ?? throw new MissingTypeException(typeof(UserService));
            Services = new HashSet<Service>(new[] { LogonService });
            try
            {
                await LogonService.RunSeedersAsync(LogonService.InitializeDatabaseAsync()).Throwable();
                await LogonService.SanitizeAsync().Throwable();
                await LogonService.VerifyAsync().Throwable();
                await LogonService.AfterInitAsync();
            }
            catch
            {
                try
                {
                    if (DbConfig._forceLocalDb) throw;
                    DbConfig._forceLocalDb = true;
                    await LogonService.RunSeedersAsync(LogonService.InitializeDatabaseAsync()).Throwable();
                    await LogonService.SanitizeAsync().Throwable();
                    await LogonService.VerifyAsync().Throwable();
                    await LogonService.AfterInitAsync();
                }
                catch
                {
                    MessageTarget?.Critical(St.ErrDataSvcNotAvailable);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Vuelve a recargar la configuración.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        [Beta]
        public static Task ReloadSettings(ISettings settings)
        {
            DisposeSettings();
            return Init(settings);
        }

        /// <summary>
        /// Deshace las inicializaciones de Proteus.
        /// </summary>
        [Beta]
        public static void DisposeSettings()
        {
            NwClient?.Logout();
            Session = null;
            _interactive = false;
            Settings = null;
            Services?.Clear();
            Services = null;
            ExternalSeeders?.Clear();
            ExternalSeeders = null;
            Logger = null;
            LogonService = null;
        }

        /// <summary>
        /// Inicia sesión en Proteus.
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <param name="password">Contraseña.</param>
        /// <returns>
        /// Un objeto <see cref="LoginResult"/> que describe el resultado
        /// de la operación de inicio de sesión.
        /// </returns>
        public static Task<LoginResult> Login(string user, SecureString password)
        {
            return Connect(LogonService?.Login(user, password) ?? throw new InvalidOperationException());
        }

        internal static async Task<LoginResult> Connect(Task<LoginResult> task)
        {
            if (Settings is null) throw new InvalidOperationException();

            var r = await task;
            if (r.Success)
            {
                if (!(r.Logon is IProteusUserCredential u)) return "El gestor de inicio de sesión ha devuelto un objeto que no puede ser utilizado para el inicio de sesión.";
                if (Settings.UseNetworkServer)
                {
                    if (!await NwClient.ConnectAsync(Settings.NetworkServerAddress, Settings.NetworkServerPort) && Settings.RequireNetworkServerSuccess)
                    {
                        return "El servidor de control de red no está disponible.";
                    }
                    switch (NwClient.TryLogin(u))
                    {
                        case Response.Acknowledged:
                            break;
                        case Response.Failure:
                            return "El servidor de control de red denegó el inicio de sesión al usuario.";
                        case Response.Forbidden:
                            return "Ya existe una sesión activa para el usuario en otro equipo.";
                        default:
                            return "El servidor ha devuelto un estado inesperado.";
                    }
                }
                Session = u;
            }
            return r;
        }

        /// <summary>
        /// Inicia sesión en Proteus.
        /// </summary>
        /// <param name="token">
        /// Ruta del archivo que contiene una credencial de inicio de
        /// sesión por medio de un token.
        /// </param>
        /// <returns>
        /// Un objeto <see cref="LoginResult"/> que describe el resultado
        /// de la operación de inicio de sesión.
        /// </returns>
        public static Task<LoginResult> Login(string token)
        {
            return Connect(TryLogin(token));
        }

        /// <summary>
        /// Inicia sesión en Proteus.
        /// </summary>
        /// <param name="token">
        /// Archivo local que contiene el token de seguridad.
        /// </param>
        /// <param name="checkProteusMode">
        /// Si se establece en <see langword="true"/>, se comprobará el
        /// modo de interactividad de Proteus contra la configuración de
        /// la misma sobre el usuario que inicia sesión.
        /// </param>
        /// <returns>
        /// Un objeto <see cref="LoginResult"/> que describe el resultado
        /// de la operación.
        /// </returns>
        public static async Task<LoginResult> TryLogin(string token, bool checkProteusMode = true)
        {
            try
            {
                var t = await (LogonService?.GetAsync<LoginToken, string>(token) ?? throw new InvalidOperationException());
                if (t is null || (!t.ComputerName.IsEmpty() && t.ComputerName.ToLower() != Environment.MachineName.ToLower()))
                    return LoginResultCode.InvalidToken;
                if (t.Void < DateTime.Now || t.IsDeleted)
                    return LoginResultCode.ExpiredToken;
                if (t.Login is null) return LoginResultCode.UnknownUser;
                if (checkProteusMode && t.Login.Interactive != Interactive)
                    return Interactive ? LoginResultCode.NotInteractive : LoginResultCode.NotSvcUser;
                if (!t.Login.Enabled) return LoginResultCode.DisabledUser;
                var u = t.Login;
                var intStatus = _interactive;
                _interactive = false;
                await LogonService.ConsumeToken(t);
                _interactive = intStatus;
                return new LoginResult(u);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Permite registrar acciones adicionales a ejecutar al realizar
        /// un cierre de sesión.
        /// </summary>
        public static HashSet<Action> LogoutActions { get; } = new HashSet<Action>();

        /// <summary>
        /// Cierra la sesión en Proteus.
        /// </summary>
        /// <param name="invokeActions">
        /// Determina si se deben ejecutar las acciones personalizadas de
        /// cierre de sesión.
        /// </param>
        public static void Logout(bool invokeActions)
        {
            if (invokeActions)
            {
                foreach (var j in LogoutActions) j?.Invoke();
            }
            NwClient?.Logout();
            Session = null;
        }

        /// <summary>
        /// Cierra la sesión en Proteus.
        /// </summary>
        public static void Logout() => Logout(true);

        /// <summary>
        /// Obtiene un servicio del tipo especificado.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de servicio a obtener.
        /// </typeparam>
        /// <returns>
        /// Una instancia del servicio solicitado, o <see langword="null"/>
        /// si dicho servicio no pudo ser cargado.
        /// </returns>
        public static T? Service<T>() where T : Service => Services?.OfType<T>().SingleOrDefault();

        /// <summary>
        /// Infiere un servicio que pueda manejar entidades del tipo
        /// especificado.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Modelo de datos con el cual trabajar.
        /// </typeparam>
        /// <returns>
        /// Un servicio que pueda administrar y almacenar entidades del
        /// tipo especificado, o <see langword="null"/> si ningún servicio
        /// puede manejar el modelo.
        /// </returns>
        public static Service InferService<TEntity>() where TEntity : ModelBase, new()
        {
            return Services.FirstOrDefault(p => p.Hosts<TEntity>());
        }

        /// <summary>
        /// Infiere un servicio que pueda manejar entidades del tipo
        /// especificado.
        /// </summary>
        /// <param name="tEntity">
        /// Modelo de datos con el cual trabajar.
        /// </param>
        /// <returns>
        /// Un servicio que pueda administrar y almacenar entidades del
        /// tipo especificado, o <see langword="null"/> si ningún servicio
        /// puede manejar el modelo.
        /// </returns>
        public static Service InferService(Type tEntity)
        {
            return Services.FirstOrDefault(p => p.Hosts(tEntity));
        }

        /// <summary>
        /// Infiere un servicio que pueda manejar entidades del tipo base
        /// especificado.
        /// </summary>
        /// <param name="tEntity">
        /// Modelo de datos con el cual trabajar.
        /// </param>
        /// <returns>
        /// Un servicio que pueda administrar y almacenar entidades del
        /// tipo especificado, o <see langword="null"/> si ningún servicio
        /// puede manejar el tipo básico de modelos.
        /// </returns>
        public static Service InferBaseService(Type tEntity)
        {
            return Services.FirstOrDefault(p => p.HostsBase(tEntity));
        }

        /// <summary>
        /// Infiere un servicio que pueda manejar entidades del tipo
        /// especificado.
        /// </summary>
        /// <param name="entity">
        /// Entidad para la cual se necesita un servicio.
        /// </param>
        /// <returns>
        /// Un servicio que pueda administrar y almacenar a la entidad
        /// especificada, o <see langword="null"/> si ningún servicio puede
        /// manejar a la entidad.
        /// </returns>
        public static Service? InferService(ModelBase entity)
        {
            return Services?.FirstOrDefault(p => p.Hosts(entity.GetType()));
        }

        /// <summary>
        /// Infiere un servicio que pueda manejar entidades del tipo
        /// especificado.
        /// </summary>
        /// <param name="model">
        /// Modelo de datos con el cual trabajar.
        /// </param>
        /// <returns>
        /// Un servicio que pueda administrar y almacenar entidades del
        /// tipo especificado, o <see langword="null"/> si ningún servicio
        /// puede manejar al modelo.
        /// </returns>
        public static Service? Infer(Type? model)
        {
            if (model is null) return null;
            model = model.ResolveToDefinedType()!;
            return InferService(model) ?? InferBaseService(model);
        }

        /// <summary>
        /// Durante las fases de inicialización temprana, permite
        /// reemplazar al cliente de red para conectarse al servidor por
        /// medio de una implementación personalizada de la clase
        /// <see cref="SessionClient"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de cliente de red a instanciar.
        /// </typeparam>
        public static void ReplaceClient<T>() where T: SessionClient, new()
        {
            if (!(Settings is null)) throw new InvalidOperationException("El cliente de red debe ser reemplazado antes de inicializar Proteus.");
            if (NwClient.IsAlive) throw new InvalidOperationException("El cliente de red debe ser reemplazado antes de iniciar la conexión remota.");
            if (Interactive) throw new InvalidOperationException("Por seguridad, esta acción está prohibida para sesiones interactivas.");
            NwClient = new T();
        }

        /// <summary>
        /// Resuelve un objeto vinculado por medio de un Id.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de objeto a resolver.
        /// </typeparam>
        /// <param name="id">
        /// Id del objeto a obtener.
        /// </param>
        /// <returns>
        /// La entidad resuelta, o <see langword="null"/> si no se encontró
        /// una entidad que coincida con el Id especificado.
        /// </returns>
        public static T? ResolveLink<T>(object id) where T : ModelBase, new()
        {
            return Infer(typeof(T))?.Get<T>(id?.ToString() ?? string.Empty) ?? default!;
        }

        /// <summary>
        /// Resuelve un objeto vinculado por medio de un Id.
        /// </summary>
        /// <param name="model">
        /// Tipo de objeto a resolver.
        /// </param>
        /// <param name="id">
        /// Id del objeto a obtener.
        /// </param>
        /// <returns>
        /// La entidad resuelta, o <see langword="null"/> si no se encontró
        /// una entidad que coincida con el Id especificado.
        /// </returns>
        public static ModelBase? ResolveLink(Type model, object id)
        {
            return Infer(model)?.Get(model, id?.ToString() ?? string.Empty);
        }

        private static IEnumerable<T> Load<T>(PluginLoader pl) where T : class, IPlugin
        {
            try
            {
                return pl.LoadEverything<T>(Settings!.PluginsDir).OrNull() ?? GetTypes<T>(true).Select(p => p.New<T>());
            }
            catch (Exception ex)
            {
                MessageTarget?.Critical(ex);
                return Array.Empty<T>();
            }
        }
    }

    internal class TypeComparer : IEqualityComparer<ISettingsRepository>
    {
        public bool Equals(ISettingsRepository x, ISettingsRepository y)
        {
            return x.GetType() == y.GetType();
        }

        public int GetHashCode(ISettingsRepository obj)
        {
            return obj.GetType().GetHashCode();
        }
    }
}