﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TheXDS.Proteus.Resources {
    using System;
    
    
    /// <summary>
    ///   Clase de recurso fuertemente tipado, para buscar cadenas traducidas, etc.
    /// </summary>
    // StronglyTypedResourceBuilder generó automáticamente esta clase
    // a través de una herramienta como ResGen o Visual Studio.
    // Para agregar o quitar un miembro, edite el archivo .ResX y, a continuación, vuelva a ejecutar ResGen
    // con la opción /str o recompile su proyecto de VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TheXDS.Proteus.Resources.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Reemplaza la propiedad CurrentUICulture del subproceso actual para todas las
        ///   búsquedas de recursos mediante esta clase de recurso fuertemente tipado.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Comprobando base de datos de {0}....
        /// </summary>
        internal static string CheckingDb {
            get {
                return ResourceManager.GetString("CheckingDb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Creando base de datos de {0}....
        /// </summary>
        internal static string CreatingDb {
            get {
                return ResourceManager.GetString("CreatingDb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Base de datos dañada..
        /// </summary>
        internal static string DamagedDb {
            get {
                return ResourceManager.GetString("DamagedDb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a No se pudo iniciar el escucha de red.
        /// </summary>
        internal static string ErrCannotInitListener {
            get {
                return ResourceManager.GetString("ErrCannotInitListener", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a El servicio de base de datos no está disponible. La aplicación debe ser reconfigurada..
        /// </summary>
        internal static string ErrDataSvcNotAvailable {
            get {
                return ResourceManager.GetString("ErrDataSvcNotAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Error inicializando la base de datos de {0}.
        /// </summary>
        internal static string ErrorInitDb {
            get {
                return ResourceManager.GetString("ErrorInitDb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Proteus ya ha sido inicializado..
        /// </summary>
        internal static string ErrProteusInited {
            get {
                return ResourceManager.GetString("ErrProteusInited", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a El puerto UDP {0} ya está en uso por otra aplicación..
        /// </summary>
        internal static string ErrUdpInUse {
            get {
                return ResourceManager.GetString("ErrUdpInUse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Se ha perdido la conectividad con el servidor de sesión de red. La conexión será restablecida automáticamente cuando el servidor vuelva a estar en línea..
        /// </summary>
        internal static string NwClientConnLost {
            get {
                return ResourceManager.GetString("NwClientConnLost", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Sesión de red.
        /// </summary>
        internal static string NwSvc {
            get {
                return ResourceManager.GetString("NwSvc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Reinicialización de base de datos.
        /// </summary>
        internal static string ReinitDb {
            get {
                return ResourceManager.GetString("ReinitDb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a La base de datos no coincide con los modelos de datos definidos en el servicio &apos;{0}&apos;. ¿Desea destruir la base de datos anterior y reinicializarla?.
        /// </summary>
        internal static string ReinitDbQuestion {
            get {
                return ResourceManager.GetString("ReinitDbQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Sanitizando la base de datos de {0}....
        /// </summary>
        internal static string SanitizingDb {
            get {
                return ResourceManager.GetString("SanitizingDb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a No fue posible encontrar un servicio que administre entidades de tipo {0}..
        /// </summary>
        internal static string Svc4EntNotFound {
            get {
                return ResourceManager.GetString("Svc4EntNotFound", resourceCulture);
            }
        }
    }
}
