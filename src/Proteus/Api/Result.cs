/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

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

    /// <summary>
    /// Describe un resultado de operación detallado.
    /// </summary>
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

        /// <summary>
        /// Valor de resultado de la operación.
        /// </summary>
        public Result Result { get; }

        /// <summary>
        /// Mensaje descriptivo sobre el resultado de la operación.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="DetailedResult"/>.
        /// </summary>
        public DetailedResult() : this(Result.Ok)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="DetailedResult"/> cuando la misma ha fallado.
        /// </summary>
        /// <param name="message">
        /// Mensaje que describe el motivo por el cual la operación falló.
        /// </param>
        public DetailedResult(string message)
        {
            Result = Result.Fail;
            Message = message;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="DetailedResult"/>.
        /// </summary>
        /// <param name="result">Resultado de la operación.</param>
        /// <param name="message">
        /// Mensaje descriptivo sobre el resultado de la operación.
        /// </param>
        public DetailedResult(Result result, string message)
        {
            Result = result;
            Message = message;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="DetailedResult"/>.
        /// </summary>
        /// <param name="result">Resultado de la operación.</param>
        public DetailedResult(Result result)
        {
            Result = result;
            Message = result switch
            {
                Result.Ok => "Operación completada correctamente.",
                Result.Fail => "Se ha producido un error realizando la operación.",
                Result.Forbidden => "Acceso denegado.",
                Result.Unreachable => "No ha sido posible contactar al servidor.",
                _ => "La operación ha finalizado debido a un problema desconocido.",
            };
        }

        /// <summary>
        /// Convierte implícitamente un <see cref="string"/> en un
        /// <see cref="DetailedResult"/>.
        /// </summary>
        /// <param name="message"><see cref="string"/> a convertir.</param>
        public static implicit operator DetailedResult(string message) => new DetailedResult(message);


        /// <summary>
        /// Convierte implícitamente un <see cref="DetailedResult"/> en un
        /// <see cref="string"/>.
        /// </summary>
        /// <param name="result"><see cref="DetailedResult"/> a convertir.</param>
        public static implicit operator string(DetailedResult result) => result.Message;


        /// <summary>
        /// Convierte implícitamente un <see cref="Result"/> en un
        /// <see cref="DetailedResult"/>.
        /// </summary>
        /// <param name="result"><see cref="Result"/> a convertir.</param>
        public static implicit operator DetailedResult(Result result) => new DetailedResult(result);


        /// <summary>
        /// Convierte implícitamente un <see cref="DetailedResult"/> en un
        /// <see cref="Result"/>.
        /// </summary>
        /// <param name="result"><see cref="DetailedResult"/> a convertir.</param>
        public static implicit operator Result(DetailedResult result) => result.Result;
    }
}
