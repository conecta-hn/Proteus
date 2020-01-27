using System;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    /// Enumera los posibles resultados de una operación provista por un servicio.
    /// </summary>
    [Flags]
    public enum Result
    {
        /// <summary>
        /// Operación completada correctamente.
        /// </summary>
        [Name("Operación completada exitosamente.")]
        Ok,

        /// <summary>
        /// La operación falló debido a un error de validación.
        /// </summary>
        [Name("Ha ocurrido un problema durante la operación.")]
        Fail,

        /// <summary>
        /// La tarea no tuvo permisos para ejecutarse.
        /// </summary>
        [Name("La acción requiere permisos adicionales.")]
        Forbidden,

        /// <summary>
        /// No fue posible contactar con el servidor.
        /// </summary>
        [Name("No ha sido posible contactar al servidor de datos.")]
        Unreachable = 4
    }
    public class DetailedResult
    {
        /// <summary>
        /// Obtiene un resultado exitoso. Este campo es de solo lectura.
        /// </summary>
        public static readonly DetailedResult Ok = new DetailedResult();
        /// <summary>
        /// Obtiene un resultado que ha fallado. Este campo es de solo
        /// lectura.
        /// </summary>
        public static readonly DetailedResult Fail = new DetailedResult(Result.Fail);
        /// <summary>
        /// Obtiene un resultado que ocurre cuando no se tienen los
        /// permisos necesarios para la operación. Este campo es de solo
        /// lectura.
        /// </summary>
        public static readonly DetailedResult Forbidden = new DetailedResult(Result.Forbidden);
        /// <summary>
        /// Obtiene un resultado que ocurre cuando no es posible alcanzar
        /// el servidor.
        /// </summary>
        public static readonly DetailedResult Unreachable = new DetailedResult(Result.Unreachable);

        public Result Result { get; }
        public string Message { get; }
        public DetailedResult() : this(Result.Ok)
        {
        }
        public DetailedResult(string message)
        {
            Result = Result.Fail;
            Message = message;
        }
        public DetailedResult(Result result, string message)
        {
            Result = result;
            Message = message;
        }
        public DetailedResult(Result result)
        {
            Result = result;
            switch (result)
            {
                case Result.Ok:
                    Message = "Operación completada correctamente.";
                    break;
                case Result.Fail:
                    Message = "Se ha producido un error realizando la operación.";
                    break;
                case Result.Forbidden:
                    Message = "Acceso denegado.";
                    break;
                case Result.Unreachable:
                    Message = "No ha sido posible contactar al servidor.";
                    break;
            }

        }
        public static implicit operator DetailedResult(string message) => new DetailedResult(message);
        public static implicit operator string(DetailedResult result) => result.Message;
        public static implicit operator DetailedResult(Result result) => new DetailedResult(result);
        public static implicit operator Result(DetailedResult result) => result.Result;
    }
}
