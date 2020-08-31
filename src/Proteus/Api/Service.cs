/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TheXDS.MCART;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Component.Attributes;
using TheXDS.Proteus.Context;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;
using static TheXDS.Proteus.Proteus;
using Timer = System.Timers.Timer;
using St = TheXDS.Proteus.Resources.Strings;

namespace TheXDS.Proteus.Api
{
    public class CallbackRegistryEntry
    {
        public Type Type { get; }
        public Action<ModelBase> Action { get; }
        public CallbackRegistryEntry(Type type, Action<ModelBase> action)
        {
            Type = ModelBase.ResolveModelType(type);
            Action = action;
        }
        public bool IsFor(Type t)
        {
            return Type == ModelBase.ResolveModelType(t);
        }
    }

    /// <summary>
    /// Clase base para todos los servicios de Proteus.
    /// </summary>
    public abstract class Service : Plugin, IDisposable, IFullService
    {

        private static readonly IEnumerable<IModelPreprocessor> _preprocessors = Objects.FindAllObjects<IModelPreprocessor>().OrderBy(p => p.GetAttr<PriorityAttribute>()?.Value).ToList();

        private readonly HashSet<CallbackRegistryEntry> _saveCallbacks = new HashSet<CallbackRegistryEntry>();

        public IEnumerable<CallbackRegistryEntry> SaveCallbacks => _saveCallbacks;

        /// <summary>
        /// Enumera los nombres de las funciones expuestas por este
        /// servicio junto a un valor que indica las banderas de seguridad
        /// establecidas para los mismas.
        /// </summary>
        public IEnumerable<NamedObject<SecurityFlags>> FunctionNames
            => Functions.Select(p => new NamedObject<SecurityFlags>(p.Value, p.Key.FullName()));

        /// <summary>
        /// Enumera las referencias a los métodos expuestas por este
        /// servicio junto a un valor que indica las banderas de seguridad
        /// establecidas para los mismos.
        /// </summary>
        public IEnumerable<KeyValuePair<MethodInfo, SecurityFlags>> Functions
        {
            get
            {
                foreach (var j in GetType().GetMethods())
                {
                    if (!j.HasAttr(out MethodKindAttribute? kind)) continue;
                    yield return new KeyValuePair<MethodInfo, SecurityFlags>(j, kind!.Value);
                }
            }
        }

        private static readonly HashSet<Timer> _revokeTimers = new HashSet<Timer>();

        private static void Rollback(DbEntityEntry entry)
        {
            switch (entry?.State)
            {
                case EntityState.Modified:
                    entry.CurrentValues.SetValues(entry.OriginalValues);
                    entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case null:
                    break;
                default:
                    throw new TamperException();
            }
        }

        private IProteusUserCredential? _session;
        private IStatusReporter? _reporter;

        /// <summary>
        /// Obtiene una referencia al contexto de datos activo de este
        /// servicio.
        /// </summary>
        protected DbContext Context { get; }

        /// <summary>
        /// Obtiene una referencia a la sesión activa para este servicio.
        /// </summary>  
        public IProteusUserCredential? Session => _session ?? Proteus.Session;

        /// <summary>
        /// Obtiene un valor que indica si este servicio se encuentra
        /// actualmente en modo de permisos elevados.
        /// </summary>
        public bool IsElevated => !(_session is null);

        /// <summary>
        /// Obtiene o establece un objeto que permite reportar el progreso
        /// de una operación.
        /// </summary>
        public IStatusReporter? Reporter 
        {
            get => _reporter ?? CommonReporter;
            set => _reporter = value;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Service"/>.
        /// </summary>
        /// <param name="context">Contexto de datos a utilizar.</param>
        protected Service(DbContext context)
        {
            Context = context;
        }

        #region Operaciones CRUD
        
        /// <summary>
        /// Agrega un conjunto de nuevas entidades a la base de datos.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidades a agregar a la base de datos.
        /// </typeparam>
        /// <param name="newEntities">
        /// Colección de entidades a ser agregadas a la base de datos.
        /// </param>
        /// <returns>
        /// <see cref="Result.Ok"/> si la operación fue exitosa,
        /// <see cref="Result.Fail"/> si la operación falla,
        /// <see cref="Result.Forbidden"/> si no se han concedido los
        /// permisos necesarios para realizar la operación, y
        /// <see cref="Result.Unreachable"/> si no es posible contactar con
        /// el servidor de dase de datos.
        /// </returns>
        [MethodKind(SecurityFlags.New)]
        public Task<DetailedResult> AddAsync<TEntity>(IEnumerable<TEntity> newEntities) where TEntity : ModelBase
        {
            return Op(() => Context.Set<TEntity>().AddRange(newEntities));
        }

        /// <summary>
        /// Agrega una nueva entidad al contexto de datos sin ejecutar una operación de guardado.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidad a agregar a la base de datos.
        /// </typeparam>
        /// <param name="newEntity">
        /// Entidad a ser agregada a la base de datos.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si la operación se realizó
        /// correctamente, <see langword="false"/> en caso contrario.
        /// </returns>
        [MethodKind(SecurityFlags.New)]
        public bool Add<TEntity>(TEntity newEntity) where TEntity : ModelBase
        {
            return PerformElevated(() => { Context.Set<TEntity>().Add(newEntity); });
        }

        /// <summary>
        /// Agrega una nueva entidad al contexto de datos sin ejecutar una operación de guardado.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidad a agregar a la base de datos.
        /// </typeparam>
        /// <param name="newEntities">
        /// Entidad a ser agregada a la base de datos.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si la operación se realizó
        /// correctamente, <see langword="false"/> en caso contrario.
        /// </returns>
        [MethodKind(SecurityFlags.New)]
        public bool Add<TEntity>(IEnumerable<TEntity> newEntities) where TEntity : ModelBase
        {
            return PerformElevated(() => { Context.Set<TEntity>().AddRange(newEntities); });
        }

        /// <summary>
        /// Agrega una nueva entidad al contexto de datos sin ejecutar una operación de guardado.
        /// </summary>
        /// <param name="newEntity">
        /// Entidad a ser agregada a la base de datos.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si la operación se realizó
        /// correctamente, <see langword="false"/> en caso contrario.
        /// </returns>
        [MethodKind(SecurityFlags.New)]
        public bool Add(ModelBase newEntity)
        {
            return PerformElevated(() => { Context.Set(newEntity.GetType().ResolveToDefinedType())?.Add(newEntity); });
        }

        /// <summary>
        /// Agrega un conjunto de nuevas entidades a la base de datos.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidad a agregar a la base de datos.
        /// </typeparam>
        /// <param name="newEntity">
        /// Entidad a ser agregada a la base de datos.
        /// </param>
        /// <returns>
        /// <see cref="Result.Ok"/> si la operación fue exitosa,
        /// <see cref="Result.Fail"/> si la operación falla,
        /// <see cref="Result.Forbidden"/> si no se han concedido los
        /// permisos necesarios para realizar la operación, y
        /// <see cref="Result.Unreachable"/> si no es posible contactar con
        /// el servidor de dase de datos.
        /// </returns>
        [MethodKind(SecurityFlags.New)]
        public Task<DetailedResult> AddAsync<TEntity>(TEntity newEntity) where TEntity : ModelBase
        {
            return Op(() => Context.Set<TEntity>().Add(newEntity));
        }
        
        /// <summary>
        /// Agrega un conjunto de nuevas entidades a la base de datos.
        /// </summary>
        /// <param name="newEntity">
        /// Entidad a ser agregada a la base de datos.
        /// </param>
        /// <returns>
        /// <see cref="Result.Ok"/> si la operación fue exitosa,
        /// <see cref="Result.Fail"/> si la operación falla,
        /// <see cref="Result.Forbidden"/> si no se han concedido los
        /// permisos necesarios para realizar la operación, y
        /// <see cref="Result.Unreachable"/> si no es posible contactar con
        /// el servidor de dase de datos.
        /// </returns>
        [MethodKind(SecurityFlags.New)]
        public Task<DetailedResult> AddAsync(ModelBase newEntity)
        {
            return Op(() => Context.Set(newEntity.GetType().ResolveToDefinedType())?.Add(newEntity));
        }

        /// <summary>
        /// Guarda todos los cambios pendientes en la base de datos.
        /// </summary>
        /// <returns>
        /// <see cref="Result.Ok"/> si la operación fue exitosa,
        /// <see cref="Result.Fail"/> si la operación falla,
        /// <see cref="Result.Forbidden"/> si no se han concedido los
        /// permisos necesarios para realizar la operación, y
        /// <see cref="Result.Unreachable"/> si no es posible contactar con
        /// el servidor de dase de datos.
        /// </returns>
        [MethodKind(SecurityFlags.Edit)]
        public Task<DetailedResult> SaveAsync()
        {
            return Op(null);
        }

        /// <summary>
        /// Edita una entidad existente en la base de datos.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidad a editar en la base de datos.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// Tipo del campo llave de la entidad.
        /// </typeparam>
        /// <param name="entity">
        /// Objeto que contiene los nuevos valores de la entidad a ser
        /// editada en la base de datos.
        /// </param>
        /// <returns>
        /// <see cref="Result.Ok"/> si la operación fue exitosa,
        /// <see cref="Result.Fail"/> si la operación falla,
        /// <see cref="Result.Forbidden"/> si no se han concedido los
        /// permisos necesarios para realizar la operación, y
        /// <see cref="Result.Unreachable"/> si no es posible contactar con
        /// el servidor de dase de datos.
        /// </returns>
        [MethodKind(SecurityFlags.Edit)]
        public Task<DetailedResult> UpdateAsync<TEntity, TKey>(TEntity entity) where TEntity : ModelBase<TKey>, new() where TKey : IComparable<TKey>
        {
            return Op(() => Context.Set<TEntity>().Find(entity.Id)?.SetFrom(entity));
        }

        /// <summary>
        /// Marca una entidad como borrada.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidad a borrar de la base de datos.
        /// </typeparam>
        /// <param name="entity">
        /// Entidad a ser borrada de la base de datos.
        /// </param>
        /// <returns>
        /// <see cref="Result.Ok"/> si la operación fue exitosa,
        /// <see cref="Result.Fail"/> si la operación falla,
        /// <see cref="Result.Forbidden"/> si no se han concedido los
        /// permisos necesarios para realizar la operación, y
        /// <see cref="Result.Unreachable"/> si no es posible contactar con
        /// el servidor de dase de datos.
        /// </returns>
        [MethodKind(SecurityFlags.Delete)]
        public Task<DetailedResult> DeleteAsync<TEntity>(TEntity entity) where TEntity : ModelBase
        {
            return Op(() => entity.IsDeleted = true);
        }
        
        /// <summary>
        /// Marca una entidad como borrada.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidad a borrar de la base de datos.
        /// </typeparam>
        /// <param name="entities">
        /// Colección de entidades a ser borradas de la base de datos.
        /// </param>
        /// <returns>
        /// <see cref="Result.Ok"/> si la operación fue exitosa,
        /// <see cref="Result.Fail"/> si la operación falla,
        /// <see cref="Result.Forbidden"/> si no se han concedido los
        /// permisos necesarios para realizar la operación, y
        /// <see cref="Result.Unreachable"/> si no es posible contactar con
        /// el servidor de dase de datos.
        /// </returns>
        [MethodKind(SecurityFlags.Delete)]
        public Task<DetailedResult> DeleteAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : ModelBase
        {
            return Op(() =>
            {
                foreach (var j in entities)
                    j.IsDeleted = true;
            });
        }
        
        /// <summary>
        /// Elimina definitivamente una entidad de la base de datos.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidad a eliminar definitivamente de la base de datos.
        /// </typeparam>
        /// <param name="entity">
        /// Entidad a ser eliminada definitivamente de la base de datos.
        /// </param>
        /// <returns>
        /// <see cref="Result.Ok"/> si la operación fue exitosa,
        /// <see cref="Result.Fail"/> si la operación falla,
        /// <see cref="Result.Forbidden"/> si no se han concedido los
        /// permisos necesarios para realizar la operación, y
        /// <see cref="Result.Unreachable"/> si no es posible contactar con
        /// el servidor de dase de datos.
        /// </returns>
        [MethodKind(SecurityFlags.Purge)]
        public Task<DetailedResult> PurgeAsync<TEntity>(TEntity entity) where TEntity : ModelBase
        {
            return Op(() => Context.Set<TEntity>().Remove(entity));
        }

        /// <summary>
        /// Elimina definitivamente una entidad de la base de datos.
        /// </summary>
        /// <param name="entity">
        /// Entidad a ser eliminada definitivamente de la base de datos.
        /// </param>
        /// <returns>
        /// <see cref="Result.Ok"/> si la operación fue exitosa,
        /// <see cref="Result.Fail"/> si la operación falla,
        /// <see cref="Result.Forbidden"/> si no se han concedido los
        /// permisos necesarios para realizar la operación, y
        /// <see cref="Result.Unreachable"/> si no es posible contactar con
        /// el servidor de dase de datos.
        /// </returns>
        [MethodKind(SecurityFlags.Purge)]
        public Task<DetailedResult> PurgeAsync(ModelBase entity)
        {
            return Op(() => Context.Set(entity.GetType().ResolveToDefinedType()).Remove(entity));
        }

        /// <summary>
        /// Elimina definitivamente una entidad de la base de datos.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidad a eliminar definitivamente de la base de datos.
        /// </typeparam>
        /// <param name="entities">
        /// Colección de entidades a ser eliminadas definitivamente de la
        /// base de datos.
        /// </param>
        /// <returns>
        /// <see cref="Result.Ok"/> si la operación fue exitosa,
        /// <see cref="Result.Fail"/> si la operación falla,
        /// <see cref="Result.Forbidden"/> si no se han concedido los
        /// permisos necesarios para realizar la operación, y
        /// <see cref="Result.Unreachable"/> si no es posible contactar con
        /// el servidor de dase de datos.
        /// </returns>
        [MethodKind(SecurityFlags.Purge)]
        public Task<DetailedResult> PurgeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : ModelBase
        {
            return Op(() => Context.Set<TEntity>().RemoveRange(entities));
        }
        /// <summary>
        /// Obtiene todos los registros de una tabla.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de tabla de entidades a devolver.
        /// </typeparam>
        /// <returns>
        /// Un <see cref="T:System.Linq.IQueryable`1" /> que devolverá la tabla que contiene
        /// las entidades de tipo <typeparamref name="TEntity" />.
        /// </returns>
        [MethodKind(SecurityFlags.Read)]
        public IQueryable<TEntity> All<TEntity>() where TEntity : ModelBase, new()
        {
            return All<TEntity>(false);
        }

        /// <summary>
        /// Obtiene todos los registros de una tabla.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de tabla de entidades a devolver.
        /// </typeparam>
        /// <param name="showDeleted">
        /// Si se establece en <see langword="true"/>, se incluirán los
        /// elementos marcados como borrados de la base de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="T:System.Linq.IQueryable`1" /> que devolverá la tabla que contiene
        /// las entidades de tipo <typeparamref name="TEntity" />.
        /// </returns>
        [MethodKind(SecurityFlags.Read)]
        public IQueryable<TEntity> All<TEntity>(bool showDeleted) where TEntity : ModelBase, new()
        {
            return PerformElevated(() =>
                from i in Context.Set<TEntity>()
                where !i.IsDeleted || showDeleted
                select i);
        }

        /// <summary>
        /// Obtiene todos los registros de una tabla.
        /// </summary>
        /// <param name="model">
        /// Tipo de tabla de entidades a devolver.
        /// </param>
        /// <param name="showDeleted">
        /// Si se establece en <see langword="true"/>, se incluirán los
        /// elementos marcados como borrados de la base de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="T:System.Linq.IQueryable`1" /> que devolverá la tabla que contiene
        /// las entidades de tipo <paramref name="model" />.
        /// </returns>
        [MethodKind(SecurityFlags.Read)]
        public IQueryable<ModelBase> All(Type model, bool showDeleted)
        {
            try
            {
                return PerformElevated(() =>
                    from i in Set(model.ResolveCollectionType()!.ResolveToDefinedType()!)
                    where !i.IsDeleted || showDeleted
                    select i);
            }
            catch (Exception ex)
            {
                MessageTarget?.Critical(ex);                
                return Array.Empty<ModelBase>().AsQueryable();
            }
        }

        /// <summary>
        /// Obtiene todos los registros de una tabla.
        /// </summary>
        /// <param name="model">
        /// Tipo de tabla de entidades a devolver.
        /// </param>
        /// <returns>
        /// Un <see cref="T:System.Linq.IQueryable`1" /> que devolverá la tabla que contiene
        /// las entidades de tipo <paramref name="model" />.
        /// </returns>
        [MethodKind(SecurityFlags.Read)]
        public IQueryable<ModelBase> All(Type model) => All(model, false);

        /// <summary>
        /// Prepara un Query que obtendrá todos los registros de una tabla.
        /// </summary>
        /// <param name="period"></param>
        /// <param name="showDeleted"></param>
        /// <returns></returns>
        [MethodKind(SecurityFlags.Read)]
        public IQueryable<TEntity> All<TEntity>(Range<DateTime> period, bool showDeleted)
            where TEntity : ModelBase, ITimestamp, new()
        {
            return All<TEntity>(showDeleted)
                .Where(p => p.Timestamp >= period.Minimum && p.Timestamp <= period.Maximum);
        }

        /// <summary>
        /// Prepara un Query que obtendrá todos los registros de una tabla.
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        [MethodKind(SecurityFlags.Read)]
        public IQueryable<TEntity> All<TEntity>(Range<DateTime> period) where TEntity : ModelBase, ITimestamp, new()
        {
            return All<TEntity>(period, false);
        }

        /// <summary>
        /// Obtiene todas las entidades que tengan como clase base el tipo especificado.
        /// </summary>
        /// <typeparam name="TEntity">Tipo de entidad base.</typeparam>
        /// <param name="showDeleted">
        /// Si se establece en <see langword="true" />, se incluirán los
        /// elementos marcados como borrados.
        /// </param>
        /// <returns>
        /// Una enumeración con todas las entidades que tienen como clase
        /// base a <typeparamref name="TEntity" />.
        /// </returns>
        [MethodKind(SecurityFlags.Read)]
        public IEnumerable<TEntity> AllBase<TEntity>(bool showDeleted) where TEntity : ModelBase
        {
            foreach (var j in Context.GetType().GetProperties()
                .Where(p => typeof(TEntity).IsAssignableFrom(p.PropertyType.GenericTypeArguments.FirstOrDefault())))
            {
                if (!(j.GetMethod?.Invoke(Context, null) is IEnumerable dbSet)) continue;
                foreach (var k in dbSet.OfType<TEntity>().Where(p => !p.IsDeleted || showDeleted)) yield return k;
            }
        }

        /// <summary>
        /// Obtiene todas las entidades que tengan como clase base el tipo especificado.
        /// </summary>
        /// <param name="baseModelType">Tipo de entidad base.</param>
        /// <param name="showDeleted">
        /// Si se establece en <see langword="true" />, se incluirán los
        /// elementos marcados como borrados.
        /// </param>
        /// <returns>
        /// Una enumeración con todas las entidades que tienen como clase
        /// base a <paramref name="baseModelType" />.
        /// </returns>
        [MethodKind(SecurityFlags.Read)]
        public IQueryable<ModelBase> AllBase(Type baseModelType, bool showDeleted = false)
        {
            return Tables(baseModelType).SelectMany(j => j.Where(p => !p.IsDeleted || showDeleted)).AsQueryable();
        }

        /// <summary>
        /// Obtiene todas las entidades que tengan como clase base el tipo especificado.
        /// </summary>
        /// <typeparam name="TEntity">Tipo de entidad base.</typeparam>
        /// <returns>
        /// Una enumeración con todas las entidades que tienen como clase
        /// base a <typeparamref name="TEntity" />.
        /// </returns>
        [MethodKind(SecurityFlags.Read)]
        public IEnumerable<TEntity> AllBase<TEntity>() where TEntity : ModelBase
        {
            return AllBase<TEntity>(false);
        }

        /// <summary>
        /// Determina si cualquiera de las entidades de la base de datos cumple con una condición.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidades a analizar.
        /// </typeparam>
        /// <param name="predicate">Predicado de análisis.</param>
        /// <returns>
        /// <see langword="true" /> si por lo menos una entidad de la base
        /// de datos cumple con el predicado, <see langword="false" /> en
        /// caso contrario.
        /// </returns>
        [Sugar, MethodKind(SecurityFlags.Search)]
        public bool Any<TEntity>(Func<TEntity, bool> predicate) where TEntity : ModelBase, new()
        {
            return All<TEntity>().Any(predicate);
        }

        /// <summary>
        /// Devuelve a la primera entidad que cumpla con la condición.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidad a devolver.
        /// </typeparam>
        /// <param name="predicate">Predicado de análisis.</param>
        /// <returns>
        /// La primera entidad que cumple con el predicado.
        /// </returns>
        [Sugar, MethodKind(SecurityFlags.Search)]
        public TEntity First<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : ModelBase, new()
        {
            return All<TEntity>().First(predicate);
        }

        /// <summary>
        /// Devuelve a la primera entidad que cumpla con la condición.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidad a devolver.
        /// </typeparam>
        /// <param name="predicate">Predicado de análisis.</param>
        /// <returns>
        /// La primera entidad que cumple con el predicado.
        /// </returns>
        [Sugar, MethodKind(SecurityFlags.Search)]
        public TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : ModelBase, new()
        {
            return All<TEntity>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Determina si la entidad especificada existe en la base de datos
        /// controlada por este servicio.
        /// </summary>
        /// <param name="entity">
        /// Entidad a comprobar.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si el modelo existe en la base de datos,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        [MethodKind(SecurityFlags.None)]
        public bool Exists(ModelBase entity)
        {
            return Get(entity.GetType(), entity.StringId.OrNull() ?? entity.IdType.Default()?.ToString() ?? string.Empty)?.StringId == entity.StringId;
        }

        /// <summary>
        /// Determina si la entidad especificada existe en la base de datos
        /// controlada por este servicio.
        /// </summary>
        /// <param name="entity">
        /// Entidad a comprobar.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si el modelo existe en la base de datos,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        [MethodKind(SecurityFlags.None)]
        public async Task<bool> ExistsAsync(ModelBase entity)
        {
            return (await GetAsync(entity.GetType(), entity.StringId.OrNull() ?? entity.IdType.Default()?.ToString() ?? string.Empty)) == entity;
        }

        /// <summary>
        /// Determina si la entidad especificada existe en la base de datos
        /// controlada por este servicio.
        /// </summary>
        /// <param name="id">
        /// Entidad a comprobar.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si la entidad existe en la base de
        /// datos, <see langword="false"/> en caso contrario.
        /// </returns>
        [MethodKind(SecurityFlags.None)]
        public bool Exists<TEntity>(string id) where TEntity : ModelBase, new()
        {
            return (All<TEntity>() ?? AllBase<TEntity>())?.Any(p=> p.Id.ToString()!.Equals(id)) ?? false;
        }

        /// <summary>
        /// Determina si la entidad especificada existe en la base de datos
        /// controlada por este servicio.
        /// </summary>
        /// <param name="model">
        /// Tipo de entidad a buscar.
        /// </param>
        /// <param name="id">
        /// Entidad a comprobar.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si el modelo existe en la base de datos,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        [MethodKind(SecurityFlags.None)]
        public bool Exists(Type model, string id)
        {
            return (All(model) ?? AllBase(model))?.Any(p => p != null && p.Id != null && p.Id.ToString()!.Equals(id)) ?? false;
        }

        /// <summary>
        /// Determina si la entidad especificada existe en la base de datos
        /// controlada por este servicio.
        /// </summary>
        /// <param name="id">
        /// Entidad a comprobar.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si el modelo existe en la base de datos,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        [MethodKind(SecurityFlags.None)]
        public Task<bool> ExistsAsync<TEntity>(string id) where TEntity : ModelBase, new()
        {
            return ExistsAsync(typeof(TEntity), id);
        }

        /// <summary>
        /// Determina si la entidad especificada existe en la base de datos
        /// controlada por este servicio de forma asíncrona.
        /// </summary>
        /// <param name="model">
        /// Tipo de entidad a buscar.
        /// </param>
        /// <param name="id">
        /// Entidad a comprobar.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si el modelo existe en la base de datos,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        [MethodKind(SecurityFlags.None)]
        public Task<bool> ExistsAsync(Type model, string id)
        {
            return (All(model) ?? AllBase(model))?.AnyAsync(p => p.Id.ToString()!.Equals(id)) ?? Task.FromResult(false);
        }

        /// <summary>
        /// Determina si cualquiera de las entidades de la base de datos
        /// cumple con una condición, realizando la comprobación de forma
        /// asíncrona.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidades a analizar.
        /// </typeparam>
        /// <param name="predicate">Predicado de análisis.</param>
        /// <returns>
        /// <see langword="true" /> si por lo menos una entidad de la base
        /// de datos cumple con el predicado, <see langword="false" /> en
        /// caso contrario.
        /// </returns>
        [Sugar, MethodKind(SecurityFlags.Search)]
        public Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : ModelBase, new()
        {
            return All<TEntity>().AnyAsync(predicate);
        }

        /// <summary>
        /// Determina si la tabla de datos contiene al menos un elemento.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidades a analizar.
        /// </typeparam>
        /// <returns>
        /// <see langword="true" /> si existe por lo menos una entidad en
        /// la tabla especificada, <see langword="false" /> en caso
        /// contrario.
        /// </returns>
        [Sugar, MethodKind(SecurityFlags.Search)]
        public Task<bool> AnyAsync<TEntity>() where TEntity : ModelBase, new()
        {
            return All<TEntity>().AnyAsync();
        }

        /// <summary>
        /// Determina si todas las entidades de la base de datos cumplen
        /// con una condición, realizando la comprobación de forma
        /// asíncrona.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidades a analizar.
        /// </typeparam>
        /// <param name="predicate">Predicado de análisis.</param>
        /// <returns>
        /// <see langword="true" /> si todas las entidades de la base
        /// de datos cumple con el predicado, <see langword="false" /> en
        /// caso contrario.
        /// </returns>
        [Sugar, MethodKind(SecurityFlags.Search)]
        public Task<bool> AllAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : ModelBase, new()
        {
            return All<TEntity>().AllAsync(predicate);
        }

        /// <summary>
        /// Obtiene todos los registros de una tabla de forma asíncrona.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de tabla de entidades a devolver.
        /// </typeparam>
        /// <returns>
        /// Un <see cref="T:System.Linq.IQueryable`1" /> que devolverá la tabla que contiene
        /// las entidades de tipo <typeparamref name="T" />.
        /// </returns>
        public Task<List<T>> AllAsync<T>() where T : ModelBase, new()
        {
            return All<T>().ToListAsync();
        }

        /// <summary>
        /// Devuelve a la primera entidad que cumpla con la condición,
        /// realizando la comprobación de forma asíncrona.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidad a devolver.
        /// </typeparam>
        /// <param name="predicate">Predicado de análisis.</param>
        /// <returns>
        /// La primera entidad que cumple con el predicado.
        /// </returns>
        [Sugar, MethodKind(SecurityFlags.Search)]
        public Task<TEntity> FirstAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : ModelBase, new()
        {
            return All<TEntity>().FirstAsync(predicate);
        }

        /// <summary>
        /// Devuelve a la primera entidad que cumpla con la condición,
        /// realizando la comprobación de forma asíncrona.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidad a devolver.
        /// </typeparam>
        /// <param name="predicate">Predicado de análisis.</param>
        /// <returns>
        /// La primera entidad que cumple con el predicado.
        /// </returns>
        [Sugar, MethodKind(SecurityFlags.Search)]
        public Task<TEntity?> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : ModelBase, new()
        {
            return All<TEntity>().FirstOrDefaultAsync(predicate)!;
        }

        /// <summary>
        /// Obtiene una entidad del tipo especificado cuyo campo llave
        /// coincida con el valor buscado, realizando la búsqueda de forma
        /// asíncrona.
        /// </summary>
        /// <typeparam name="TEntity">Tipo de entidad a obtener.</typeparam>
        /// <typeparam name="TKey">
        /// Tipo del campo llave de la entidad.
        /// </typeparam>
        /// <param name="id">
        /// Valor de campo llave primario a buscar.
        /// </param>
        /// <returns>
        /// Una entidad del tipo especificado cuyo campo llave coincida con
        /// el valor buscado, o <see langword="null"/> si ninguna entidad
        /// coincide.
        /// </returns>
        [MethodKind(SecurityFlags.Search)]
        public Task<TEntity?> GetAsync<TEntity, TKey>(TKey id) where TEntity : ModelBase<TKey>, new()
            where TKey : IComparable<TKey>
        {
            //return FirstOrDefaultAsync<TEntity>(p => p.Id.Equals(id));
            return Context.Set<TEntity>().FindAsync(id)!;
        }

        /// <summary>
        /// Obtiene una entidad del tipo especificado cuyo campo llave
        /// coincida con el valor buscado.
        /// </summary>
        /// <typeparam name="TEntity">Tipo de entidad a obtener.</typeparam>
        /// <typeparam name="TKey">
        /// Tipo del campo llave de la entidad.
        /// </typeparam>
        /// <param name="id">
        /// Valor de campo llave primario a buscar.
        /// </param>
        /// <returns>
        /// Una entidad del tipo especificado cuyo campo llave coincida con
        /// el valor buscado, o <see langword="null"/> si ninguna entidad
        /// coincide.
        /// </returns>
        [MethodKind(SecurityFlags.Search)]
        public TEntity Get<TEntity, TKey>(TKey id) where TEntity : ModelBase<TKey>, new() where TKey : IComparable<TKey>, IEquatable<TKey>
        {
            return FirstOrDefault<TEntity>(p => p.Id.Equals(id));
        }

        /// <summary>
        /// Obtiene una entidad con el Id especificado.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidada a obtener.
        /// </typeparam>
        /// <param name="id">
        /// Id de la entidad a obtener.
        /// </param>
        /// <returns>
        /// La entidad con el Id especificado, o <see langword="null"/> si
        /// no existe ninguna entidad con el Id especificado.
        /// </returns>
        [MethodKind(SecurityFlags.Search)]
        public TEntity? Get<TEntity>(string id) where TEntity : ModelBase, new()
        {
            return FirstOrDefault<TEntity>(p => p.Id.ToString()!.Equals(id));
        }

        /// <summary>
        /// Obtiene una entidad con el Id especificado de forma asíncrona.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidada a obtener.
        /// </typeparam>
        /// <param name="id">
        /// Id de la entidad a obtener.
        /// </param>
        /// <returns>
        /// La entidad con el Id especificado, o <see langword="null"/> si
        /// no existe ninguna entidad con el Id especificado.
        /// </returns>
        [MethodKind(SecurityFlags.Search)]
        public Task<TEntity?> GetAsync<TEntity>(string id) where TEntity : ModelBase, new()
        {
            return FirstOrDefaultAsync<TEntity>(p => p.Id.ToString()!.Equals(id));
        }

        /// <summary>
        /// Obtiene una entidad con el Id especificado.
        /// </summary>
        /// <param name="model">
        /// Tipo de entidada a obtener.
        /// </param>
        /// <param name="id">
        /// Id de la entidad a obtener.
        /// </param>
        /// <returns>
        /// La entidad con el Id especificado, o <see langword="null"/> si
        /// no existe ninguna entidad con el Id especificado.
        /// </returns>
        [MethodKind(SecurityFlags.Search)]
        public ModelBase? Get(Type model, string id)
        {
            return All(model)?.FirstOrDefault(p => p.Id.ToString()!.Equals(id));
        }

        /// <summary>
        /// Obtiene una entidad con el Id especificado de forma asíncrona.
        /// </summary>
        /// <param name="model">
        /// Tipo de entidada a obtener.
        /// </param>
        /// <param name="id">
        /// Id de la entidad a obtener.
        /// </param>
        /// <returns>
        /// La entidad con el Id especificado, o <see langword="null"/> si
        /// no existe ninguna entidad con el Id especificado.
        /// </returns>
        [MethodKind(SecurityFlags.Search)]
        public Task<ModelBase?> GetAsync(Type model, string id)
        {
            return All(model).FirstOrDefaultAsync(p => p.Id.ToString()!.Equals(id))!;
        }

        #endregion

        #region Elevación y seguridad

        /// <summary>
        /// Obtiene o establece el comportamiento de las funciones de
        /// elevación del servicio.
        /// </summary>
        public ElevationBehavior ElevationBehavior { get; set; }

        /// <summary>
        /// Infiere las banderas de seguridad requeridas para ejecutar un
        /// método desde un elevador de permisos.
        /// </summary>
        /// <returns>
        /// Las banderas requeridas por el servicio que ha realizado una
        /// llamada al elevador de permisos, o
        /// <see cref="SecurityFlags.None"/> si no es posible determinar un
        /// método seguro de origen de llamada, o si el mismo no posee
        /// banderas de seguridad.
        /// </returns>
        /// <remarks>
        /// Esta función debe ejecutarse desde el método
        /// <see cref="IElevator.Elevate(ref IProteusUserCredential)"/> de una
        /// clase que implemente <see cref="IElevator"/> para obtener las
        /// banderas requeridas.
        /// </remarks>
        public static SecurityFlags InferFlags()
        {
            Infer(out _, out var m);
            return m;

        }

        /// <summary>
        /// Infiere al método llamante que contiene banderas de seguridad.
        /// </summary>
        /// <returns>
        /// El primer método encontrado en la pila de llamadas que contenga
        /// banderas de seguridad, o <see langword="null"/> si se alcanza
        /// el punto de entrada de la aplicación (el método Main())
        /// </returns>
        public static MethodInfo? InferMethod()
        {
            Infer(out var m, out _);
            return m;
        }
        
        /// <summary>
        /// Infiere al método llamante que contiene banderas de seguridad.
        /// </summary>
        /// <param name="method">
        /// Parámetro de salida. Primer método encontrado con banderas de
        /// seguridad.
        /// </param>
        /// <param name="flags">
        /// Parámetro de salida. Banderas de seguridad del método.
        /// </param>
        public static void Infer(out MethodInfo? method, out SecurityFlags flags)
        {
            var c = 1;
            MethodKindAttribute? mka;
            do
            {
                method = ReflectionHelpers.GetCallingMethod(c++) as MethodInfo;
                if (method is null)
                {
                    flags = SecurityFlags.None;
                    return;
                }
            } while (!method.HasAttr(out mka));
            flags = mka?.Value ?? default;
        }

        /// <summary>
        /// Ejecuta una auditoría de permisos para la credencial
        /// especificada sobre este servicio.
        /// </summary>
        /// <param name="credential">Credencial a auditar.</param>
        /// <returns>
        /// Una colección con los resultados de la auditoría para cada
        /// función de este servicio.
        /// </returns>
        public IEnumerable<KeyValuePair<MethodInfo, bool?>> Audit(IProteusHierachicalCredential? credential)
        {
            foreach (var j in Functions)
            {
                yield return new KeyValuePair<MethodInfo, bool?>(j.Key, CanRunService(j.Key, credential));
            }
        }

        /// <summary>
        /// Ejecuta una auditoría local de permisos para la credencial
        /// especificada sobre este servicio.
        /// </summary>
        /// <param name="credential">Credencial a auditar.</param>
        /// <returns>
        /// Una colección con los resultados de la auditoría local para
        /// cada función de este servicio.
        /// </returns>
        public IEnumerable<KeyValuePair<MethodInfo, bool?>> ShallowAudit(IProteusCredential credential)
        {
            foreach (var j in Functions)
            {
                yield return new KeyValuePair<MethodInfo, bool?>(j.Key, CanRunService(j.Key, credential));
            }
        }

        /// <summary>
        /// Ejecuta una auditoría de permisos para la credencial
        /// actual de sesión.
        /// </summary>
        /// <returns>
        /// Una colección con los resultados de la auditoría para cada
        /// función de este servicio.
        /// </returns>
        public IEnumerable<KeyValuePair<MethodInfo, bool?>> Audit() => Audit(Session);

        /// <summary>
        /// Comprueba los permisos de ejecución del servicio bajo el 
        /// contexto actual de funcionamiento.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> si la operación tiene permisos de
        /// ejecución, <see langword="false"/> si no los tiene, y
        /// <see langword="null"/> si no existe un descriptor que defina la
        /// posibilidad de ejecutar una operación sobre el servicio.
        /// </returns>
        public static bool? CanRunService() => InferMethod() is { } m ? CanRunService(m) : null;

        /// <summary>
        /// Comprueba los permisos de ejecución del servicio bajo el 
        /// contexto actual de funcionamiento.
        /// </summary>
        /// <param name="method">
        /// Método con banderas de permiso que se está intentando ejecutar.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si la operación tiene permisos de
        /// ejecución, <see langword="false"/> si no los tiene, y
        /// <see langword="null"/> si no existe un descriptor que defina la
        /// posibilidad de ejecutar una operación sobre el servicio.
        /// </returns>
        public static bool? CanRunService(MethodInfo method) => CanRunService(method, LogonService?.Session);

        /// <summary>
        /// Comprueba los permisos de ejecución del servicio bajo el 
        /// contexto actual de funcionamiento.
        /// </summary>
        /// <param name="method">
        /// Método con banderas de permiso que se está intentando ejecutar.
        /// </param>
        /// <param name="credential">
        /// Credencial específica contra la cual comprobar.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si la operación tiene permisos de
        /// ejecución, <see langword="false"/> si no los tiene, y
        /// <see langword="null"/> si no existe un descriptor que defina la
        /// posibilidad de ejecutar una operación sobre el servicio.
        /// </returns>
        public static bool? CanRunService(MethodInfo method, IProteusCredential? credential)
        {
            return CanRunService(method.FullName(), method.GetAttr<MethodKindAttribute>()?.Value ?? SecurityFlags.None, credential);
        }

        /// <summary>
        /// Comprueba los permisos de ejecución del servicio bajo el 
        /// contexto actual de funcionamiento.
        /// </summary>
        /// <param name="id">
        /// Id de la acción que se está intentando ejecutar.
        /// </param>
        /// <param name="flags">
        /// Banderas de permisos a comprobar.
        /// </param>
        /// <param name="credential">
        /// Credencial específica contra la cual comprobar.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si la operación tiene permisos de
        /// ejecución, <see langword="false"/> si no los tiene, y
        /// <see langword="null"/> si no existe un descriptor que defina la
        /// posibilidad de ejecutar una operación sobre el servicio.
        /// </returns>
        public static bool? CanRunService(string id, SecurityFlags flags, IProteusCredential? credential)
        {
            if (credential is null) return null;
            foreach (var j in credential.Descriptors.OfType<IServiceSecurityDescriptor>())
            {
                if (j.Id != id) continue;
                return !j.Revoked.HasFlag(flags) || j.Granted.HasFlag(flags);
            }
            if (credential.DefaultRevoked.HasFlag(flags)) return false;
            if (credential.DefaultGranted.HasFlag(flags)) return true;
            return null;
        }

        /// <summary>
        /// Comprueba los permisos de ejecución del servicio bajo el 
        /// contexto actual de funcionamiento.
        /// </summary>
        /// <param name="method">
        /// Método con banderas de permiso que se está intentando ejecutar.
        /// </param>
        /// <param name="credential">
        /// Credencial a analizar. Se comprobará ésta, además de cualquier
        /// credencial padre de la misma.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si la operación tiene permisos de
        /// ejecución, <see langword="false"/> si no los tiene, y
        /// <see langword="null"/> si no existe un descriptor que defina la
        /// posibilidad de ejecutar una operación sobre el servicio.
        /// </returns>
        public static bool? CanRunService(MethodInfo method, IProteusHierachicalCredential? credential)
        {
            return RecursiveCheck(CanRunService, method, credential);
        }

        public static bool? RecursiveCheck(Func<MethodInfo, IProteusCredential?, bool?> check, MethodInfo method, IProteusHierachicalCredential? credential)
        {
            if (check is null)
                throw new ArgumentNullException(nameof(check));
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            if (credential is null) return null;
            bool? r = check(method, credential);
            if (!(r is null)) return r;

            foreach (var j in credential.Roles)
            {
                r = check(method, j);
                if (r.HasValue) return r;
            }
            return check(method, credential.Parent);
        }

        public static bool? RecursiveCheck(Func<string, SecurityFlags, IProteusCredential, bool?> check, string method, SecurityFlags flags, IProteusHierachicalCredential? credential)
        {
            if (check is null)
                throw new ArgumentNullException(nameof(check));
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            if (credential is null) return null;
            bool? r = check(method, flags, credential);
            if (!(r is null)) return r;

            foreach (var j in credential.Roles)
            {
                r = check(method,flags, j);
                if (r.HasValue) return r;
            }
            return check(method,flags, credential.Parent);
        }

        public static bool? CanRunService(string id, SecurityFlags flags) => CanRunService(id, flags, LogonService?.Session);

        public static bool? CanRunService(string id, SecurityFlags flags, IProteusHierachicalCredential? credential)
        {
            if (credential is null) return null;
            bool? r = CanRunService(id, flags, (IProteusCredential)credential);
            if (!(r is null)) return r;

            foreach (var j in credential.Roles)
            {
                r = CanRunService(id, flags, j);
                if (r.HasValue) return r;
            }
            return CanRunService(id, flags, credential.Parent);
        }

        public bool? CanRunService(SecurityFlags flags) => CanRunService(flags, Session);

        public bool? CanRunService(SecurityFlags flags, IProteusHierachicalCredential? cred)
        {
            if (cred is null) return null;
            foreach (var j in cred.Descriptors.OfType<ServiceSecurityDescriptor>())
            {
                if (j.Id == GetType().FullName)
                {
                    if ((j.Revoked & flags) != SecurityFlags.None) return false;
                    if (j.Granted.HasFlag(flags)) return true;
                }
            }
            if ((cred.DefaultRevoked & flags) != SecurityFlags.None) return false;
            if (cred.DefaultGranted.HasFlag(flags)) return true;
            return CanRunService(flags, cred.Parent);
        }

        #endregion

        /// <summary>
        /// Realiza tareas adicionales de inicialización del servicio.
        /// </summary>
        /// <param name="reporter">
        /// <see cref="IStatusReporter"/> a utilizar para reportar
        /// visualmente el progreso de la operación.
        /// </param>
        /// <returns>
        /// Una tarea que puede utilizarse para monitorear la operación
        /// asíncrona.
        /// </returns>
        protected virtual Task AfterInitialization(IStatusReporter? reporter) => Task.CompletedTask;

        internal async Task AfterInitAsync()
        {
            await AfterInitialization(Reporter);
            _reporter?.Done();
        }

        /// <summary>
        /// Vuelve a ejecutar un Query sobre todos los elementos de la base de datos
        /// administrada por este servicio.
        /// </summary>
        /// <returns>
        /// Un objeto que describe detalladamente el resultado de la operación.
        /// </returns>
        public DetailedResult Reload()
        {
            lock (Context)
            {
                try
                {
                    return Context.ChangeTracker.Entries()
                        .Any(j => !j.ReloadAsync().Wait(Settings!.ServerTimeout))
                        ? Result.Unreachable
                        : Result.Ok;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        /// <summary>
        /// Vuelve a ejecutar un Query sobre todos los elementos de la base de datos
        /// administrada por este servicio.
        /// </summary>
        /// <param name="data">Tabla de datos a recargar.</param>
        /// <returns>
        /// Un objeto que describe detalladamente el resultado de la operación.
        /// </returns>
        public DetailedResult Reload(IQueryable<ModelBase> data)
        {
            lock (Context)
            {
                try
                {
                    return data.Select(p=>Context.Entry(p)).NotNull()
                        .Any(j => !j.ReloadAsync().Wait(Settings!.ServerTimeout))
                        ? Result.Unreachable
                        : Result.Ok;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        /// <summary>
        /// Vuelve a cargar desde la base de datos toda la información de la entidad
        /// especificada.
        /// </summary>
        /// <param name="entity">Entidad a recargar.</param>
        /// <returns>
        /// Un objeto que describe detalladamente el resultado de la operación.
        /// </returns>
        public DetailedResult Reload(ModelBase entity)
        {
            lock (Context)
            {
                try
                {
                    return
                        Context.ChangeTracker.Entries()
                            .FirstOrDefault(p => p.Entity.Is(entity))?.ReloadAsync()
                            .Wait(Settings!.ServerTimeout) ?? true ? Result.Ok : Result.Unreachable;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        /// <summary>
        /// Registra una llamada a función a ejecutar inmediatamente
        /// después de guardar una entidad.
        /// </summary>
        /// <typeparam name="T">
        /// Modelo de datos a observar. Se ejecutará
        /// <paramref name="callback"/> cuando se guarde una entidad de
        /// este tipo.
        /// </typeparam>
        /// <param name="callback">
        /// Función a ejecutar al guardar una entidad de tipo
        /// <typeparamref name="T"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Se produce si <paramref name="callback"/> es
        /// <see langword="null"/>.
        /// </exception>
        public void RegisterSaveCallback<T>(Action<T> callback) where T : ModelBase
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            _saveCallbacks.Add(new CallbackRegistryEntry(typeof(T), p => callback((T)p)));
        }

        /// <summary>
        /// Registra una llamada a función a ejecutar inmediatamente
        /// después de guardar una entidad.
        /// </summary>
        /// <param name="model">
        /// Modelo de datos a observar. Se ejecutará
        /// <paramref name="callback"/> cuando se guarde una entidad de
        /// este tipo.
        /// </param>
        /// <param name="callback">
        /// Función a ejecutar al guardar una entidad del tipo
        /// especificado.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Se produce si <paramref name="model"/> o
        /// <paramref name="callback"/> son <see langword="null"/>.
        /// </exception>
        public void RegisterSaveCallback(Type model, Action<ModelBase> callback)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            _saveCallbacks.Add(new CallbackRegistryEntry(model, callback));
        }

        /// <summary>
        /// Comprueba si el contexto de datos asociado a este <see cref="T:TheXDS.Proteus.API.Service" />
        /// permite alojar entidades del tipo especificado.
        /// </summary>
        /// <param name="tEntity">
        /// Tipo de entidad a comprobar.
        /// </param>
        /// <returns>
        /// <see langword="true" /> si el contexto contiene un <see cref="T:System.Data.Entity.DbSet" />
        /// compatible con el modelo especificado, <see langword="false" /> en
        /// caso contrario.
        /// </returns>
        public bool Hosts(Type tEntity)
        {
            var dbType = typeof(DbSet<>).MakeGenericType(tEntity);
            return Context.GetType().GetProperties().Any(p => dbType.IsAssignableFrom(p.PropertyType));
        }

        /// <summary>
        /// Comprueba si el contexto de datos asociado a este <see cref="T:TheXDS.Proteus.API.Service" />
        /// permite alojar entidades del tipo especificado.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> si el contexto contiene un <see cref="T:System.Data.Entity.DbSet" />
        /// compatible con el modelo especificado, <see langword="false" /> en
        /// caso contrario.
        /// </returns>
        /// <param name="model">Nombre del modelo a comprobar.</param>
        /// <param name="tModel">Parámetro de salida. Modelo de datos encontrado.</param>
        public bool Hosts(string model, out Type? tModel)
        {
            foreach (var j in Context.GetType().GetProperties())
            {
                if (!j.PropertyType.Implements(typeof(DbSet<>))) continue;

                tModel = (j.GetValue(Context) as IQueryable<ModelBase>)?.ElementType;
                if (tModel?.Name == model)
                {
                    return true;
                }
            }
            tModel = null;
            return false;
        }

        /// <summary>
        /// Comprueba si el contexto de datos asociado a este <see cref="T:TheXDS.Proteus.API.Service" />
        /// permite alojar entidades del tipo base especificado.
        /// </summary>
        /// <param name="tEntity">
        /// Tipo de entidad a comprobar.
        /// </param>
        /// <returns>
        /// <see langword="true" /> si el contexto contiene un <see cref="T:System.Data.Entity.DbSet" />
        /// compatible con el modelo especificado, <see langword="false" /> en
        /// caso contrario.
        /// </returns>
        public bool HostsBase(Type tEntity)
        {
            return Context.GetType().GetProperties().Any(p => IsTable(p,tEntity));
        }

        private bool IsTable(PropertyInfo p, Type model)
        {
            var t = p.PropertyType;
            return t.IsConstructedGenericType
                && t.GetGenericTypeDefinition() == typeof(DbSet<>)
                && model.IsAssignableFrom(t.GenericTypeArguments[0]);
        }

        private IEnumerable<IQueryable<ModelBase>> Tables(Type modelBase)
        {
            return Context.GetType().GetProperties()
                .Where(t => IsTable(t, modelBase))
                .Select(p => p.GetValue(Context) as IQueryable<ModelBase>).NotNull();
        }

        private IQueryable<ModelBase> Set(Type model)
        {
            return Tables(model).FirstOrDefault();
        }

        /// <summary>
        /// Comprueba si el contexto de datos asociado a este <see cref="Service" />
        /// permite alojar entidades del tipo especificado.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo de entidad a comprobar.
        /// </typeparam>
        /// <returns>
        /// <see langword="true" /> si el contexto contiene un <see cref="DbSet" />
        /// compatible con el modelo especificado, <see langword="false" /> en
        /// caso contrario.
        /// </returns>
        public bool Hosts<TEntity>() where TEntity : ModelBase, new()
        {
            return Hosts(typeof(TEntity));
        }

        /// <summary>
        /// Indica si existen cambios pendientes de guardar.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> si existen cambios pendientes de
        /// guardar, <see langword="false" /> en caso contrario.
        /// </returns>
        public bool ChangesPending()
        {
            return Context.ChangeTracker.Entries().Any(p => p.State != EntityState.Unchanged);
        }

        /// <summary>
        /// Indica si existen cambios pendientes de guardar.
        /// </summary>
        /// <param name="entity">
        /// Entidad a comprobar.
        /// </param>
        /// <returns>
        /// <see langword="true" /> si existen cambios pendientes de
        /// guardar, <see langword="false" /> en caso contrario.
        /// </returns>
        public bool ChangesPending(ModelBase entity)
        {
            return Context.ChangeTracker.Entries().Any(p => p.Entity == entity && p.State != EntityState.Unchanged);
        }

        /// <summary>
        /// Revoca la elevación activa.
        /// </summary>
        public void Revoke()
        {
            _session = null;
        }
        /// <summary>
        /// Revierte todas las operaciones pendientes de guardado.
        /// </summary>
        public void Rollback()
        {
            var changedEntries = Context.ChangeTracker.Entries()
                .Where(x => x.State != EntityState.Unchanged).ToList();

            foreach (var entry in changedEntries)
                Rollback(entry);
        }
        /// <summary>
        /// Revierte los cambios sin guardar realizados en la entidad especificada.
        /// </summary>
        /// <param name="entity">Entidad a revertir.</param>
        public void Rollback(ModelBase entity)
        {
            Rollback(GetEntry(entity));
        }

        private DbEntityEntry GetEntry(ModelBase entity)
        {
            return Context.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged).ToList().SingleOrDefault(p => p.Entity == entity);
        }

        /// <summary>
        /// Obtiene una  colección con los valores originales de una
        /// entidad modificada.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<PropertyInfo, object>> OldValues(ModelBase entity)
        {
            if (entity is null) yield break;
            var e = GetEntry(entity);
            foreach (var j in entity.GetType().ResolveToDefinedType()!.GetProperties())
            {
                if (e.OriginalValues.PropertyNames.Contains(j))
                    yield return new KeyValuePair<PropertyInfo, object>(
                        j, e.OriginalValues[j.Name]);
            }
        }
                
        private protected Task<DetailedResult> Op(Action? action)
        {
            return !PerformElevated(action)
                ? Task.FromResult(DetailedResult.Forbidden)
                : InternalSaveAsync();
        }

        /// <summary>
        /// Libera los recursos no utilizados de esta instancia antes de
        /// ser desechada.
        /// </summary>
        public void Dispose()
        {
            try
            {
                Context?.Dispose();
            }
            catch (ObjectDisposedException)
            {
                /* no hacer nada. */
            }
        }

        /// <summary>
        /// Obtiene un nombre amigable para el servicio.
        /// </summary>
        /// <returns>Un nombre amigable para el servicio.</returns>
        public override string ToString() => $"Servicio de {FriendlyName.ToLower()}";

        /// <summary>
        /// Obtiene un nombre amigable para el servicio.
        /// </summary>
        public string FriendlyName => GetType().NameOf()?.Without(GetType().Name).OrNull() ?? GetType().Name.ChopEnd(nameof(Service));

        private protected void AfterElevation()
        {
            switch (ElevationBehavior)
            {
                case ElevationBehavior.Once:
                    Revoke();
                    break;
                case ElevationBehavior.Keep:
                    break;
                case ElevationBehavior.Timeout:
                    ConfigureTimeout();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ConfigureTimeout()
        {
            var t = new Timer(TimeSpan.FromMinutes(10).TotalMilliseconds)
            {
                AutoReset = false,
                Enabled = true
            };
            t.Elapsed += ElevationTimer_Elapsed;
            _revokeTimers.Add(t);
        }

        /// <summary>
        /// Comprueba los permisos de ejecución de una función del
        /// servicio.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> si la función tiene permisos para
        /// ejecutarse, <see langword="false"/> en caso contrario.
        /// </returns>
        public bool Elevate()
        {
            return !Interactive || (Session ?? Proteus.Session)?.Id == "root" || (CanRunService() ?? Elevator?.Elevate(ref _session) ?? false);
        }

        public bool Elevate(SecurityFlags flags)
        {
            return Elevate() && (CanRunService(flags) ?? false);
        }

        /// <summary>
        /// Enumera los tipos de entidad hospedadas en el contexto
        /// administrado por este servicio.
        /// </summary>
        public IEnumerable<Type> Models()
        {
            foreach (var j in Context.GetType().GetProperties().Where(p => p.CanRead))
            {
                if (j.GetMethod!.Invoke(Context, Array.Empty<object>()) is IQueryable s)
                    yield return s.ElementType;
            }
        }

        public Task<DetailedResult> ForcefullySaveAsync() => InternalSaveAsync(true);

        /// <summary>
        /// Ejecuta una operación de guardado directamente, sin realizar
        /// verificaciones de permisos.
        /// </summary>
        /// <returns>
        /// <see cref="Result.Ok"/> si la operación fue exitosa,
        /// <see cref="Result.Fail"/> si la operación falla,
        /// <see cref="Result.Forbidden"/> si no se han concedido los
        /// permisos necesarios para realizar la operación, y
        /// <see cref="Result.Unreachable"/> si no es posible contactar con
        /// el servidor de dase de datos.
        /// </returns>
        protected async Task<DetailedResult> InternalSaveAsync(bool forcefullySave = false)
        {
            CancellationTokenSource? cs = null;
            try
            {
                if (!(ChangesPending() || forcefullySave)) return Result.Ok;

                var affectedEntities = Context.ChangeTracker.Entries()
                    .Where(p => p.State != EntityState.Unchanged).ToList();

                foreach (var j in affectedEntities)
                {
                    foreach (var k in _preprocessors)
                    {
                        if (!k.CanProcess(j)) continue;
                        k.Process(j);
                    }
                }

                var logResult = await (Logger?.Log(Context.ChangeTracker, Session) ?? Task.FromResult(Result.Ok));

                cs = new CancellationTokenSource(Settings?.ServerTimeout ?? 15000);
                await Context.SaveChangesAsync(cs.Token);
                if (cs.IsCancellationRequested) return Result.Unreachable | logResult;
                cs.Dispose();
                cs = null;
                if (_saveCallbacks.Any())
                {
                    foreach (var j in affectedEntities.Select(p => p.Entity as ModelBase).NotNull())
                    {
                        foreach(var k in _saveCallbacks.Where(p => p.IsFor(j.GetType())))
                        {
                            k.Action(j);
                        }
                    }
                }
                return Result.Ok | logResult;
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageTarget?.Critical(ex);
                if (System.Diagnostics.Debugger.Launch())
                    System.Diagnostics.Debugger.Break();
#endif
                cs?.Cancel(false);
                return ex.Message;                
            }
        }

        /// <summary>
        /// Ejecuta una operación bajo un contexto elevado de permisos.
        /// </summary>
        /// <param name="action">Acción a ajecutar.</param>
        /// <returns>
        /// <see langword="true"/> si la acción tiene permisos para
        /// ejecutarse, <see langword="false"/> en caso contrario.
        /// </returns>
        protected bool PerformElevated(Action? action)
        {
            if (!Elevate()) return false;
            action?.Invoke();
            AfterElevation();
            return true;
        }

        /// <summary>
        /// Ejecuta una operación bajo un contexto elevado de permisos.
        /// </summary>
        /// <param name="action">Acción a ajecutar.</param>
        /// <returns>
        /// <see langword="true"/> si la acción tiene permisos para
        /// ejecutarse, <see langword="false"/> en caso contrario.
        /// </returns>
        protected T PerformElevated<T>(Func<T> action)
        {
            return PerformElevated(action, out var result) ? result : default!;
        }

        /// <summary>
        /// Ejecuta una operación bajo un contexto elevado de permisos.
        /// </summary>
        /// <param name="action">Acción a ajecutar.</param>
        /// <param name="result">
        /// Parámetro de salida. Contiene el resultado de la acción.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si la acción tiene permisos para
        /// ejecutarse, <see langword="false"/> en caso contrario.
        /// </returns>
        protected bool PerformElevated<T>(Func<T> action, out T result)
        {
            if (!Elevate())
            {
                result = default!;
                return false;
            }
            try
            {
                result = action.Invoke();
                return true;
            }
            finally
            {
                AfterElevation();
            }
        }

        /// <summary>
        /// Realiza una operación de limpieza sobre el contexto de datos,
        /// eliminando todos los elementos marcados como tal.
        /// </summary>
        /// <returns>
        /// <see cref="Result.Ok"/> si la operación fue exitosa,
        /// <see cref="Result.Fail"/> si la operación falla,
        /// <see cref="Result.Forbidden"/> si no se han concedido los
        /// permisos necesarios para realizar la operación, y
        /// <see cref="Result.Unreachable"/> si no es posible contactar con
        /// el servidor de dase de datos.
        /// </returns>
        [MethodKind(SecurityFlags.Root)]
        public Task<DetailedResult> SanitizeAsync()
        {
            return Op(() =>
            {
                CommonReporter?.UpdateStatus(string.Format(St.SanitizingDb,FriendlyName.ToLower()));
                foreach (var j in Context.GetType().GetProperties()
                    .Where(p => typeof(DbSet<ISoftDeletable>).IsAssignableFrom(p.PropertyType))
                    .Select(q => q.GetMethod!.Invoke(Context, Array.Empty<object>()) as DbSet<ISoftDeletable>))
                    j?.RemoveRange(j.Where(p => p.IsDeleted));
            });
        }

        /// <summary>
        /// Ejecuta una comprobación de la base de datos.
        /// </summary>
        /// <returns>
        /// <see cref="Result.Ok"/> si la operación fue exitosa,
        /// <see cref="Result.Fail"/> si la operación falla,
        /// <see cref="Result.Forbidden"/> si no se han concedido los
        /// permisos necesarios para realizar la operación, y
        /// <see cref="Result.Unreachable"/> si no es posible contactar con
        /// el servidor de dase de datos.
        /// </returns>
        public virtual Task<DetailedResult> VerifyAsync() => Task.FromResult(DetailedResult.Ok);

        /// <summary>
        /// Comprueba la salud del servicio, comprobando si la base de
        /// datos existe y verificando su validez de forma asíncrona.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> si fue posible conectarse al servidor y
        /// comprobar que la base de datos existe, <see langword="false"/>
        /// en caso contrario.
        /// </returns>
        public Task<bool> IsHealthyAsync() => Task.Run(IsHealthy);

        /// <summary>
        /// Comprueba la salud del servicio, comprobando si la base de
        /// datos existe y verificando su validez.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> si fue posible conectarse al servidor y
        /// comprobar que la base de datos existe, <see langword="false"/>
        /// en caso contrario.
        /// </returns>
        public bool IsHealthy()
        {
            try
            {
                return Context.Database.Exists() && Context.Database.CompatibleWithModel(false);
            }
            catch
            {
                return false;
            }
        }

        private void ElevationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var t = sender as Timer ?? throw new TamperException();
            _revokeTimers.Remove(t);
            Revoke();
            t.Dispose();
        }

        internal Task<bool> InitializeDatabaseAsync()
        {
            return Task.Run(InitializeDatabase);
        }

#if DEBUG
        internal bool InitializeDatabase()
        {
            return InitializeDatabase(false);
        }

        internal bool InitializeDatabase(bool forcefully)
#else
        internal bool InitializeDatabase()
#endif
        {
            Reporter?.UpdateStatus(string.Format(St.CheckingDb, FriendlyName.ToLower()));
            try
            {
#if DEBUG
                if (forcefully) Context.Database.Delete();
#endif
                if (Context.Database.Exists())
                {
                    if (Context.Database.CompatibleWithModel(false) 
                        || (!InteractiveMt?.Ask(St.ReinitDb, string.Format(St.ReinitDbQuestion, FriendlyName.ToLower())) ?? true)) return false;
                    Reporter?.UpdateStatus($"{St.DamagedDb} {string.Format(St.CreatingDb, FriendlyName.ToLower())}");
                    Context.Database.Delete();
                }
                else
                {
                    Reporter?.UpdateStatus(string.Format(St.CreatingDb, FriendlyName.ToLower()));
                }
                Context.Database.Create();
                return true;
            }
            catch (Exception ex)
            {
                Reporter?.UpdateStatus(ex.Message);                
                return false;
            }
        }

        internal Result RunSeeders(bool runRegardless)
        {
            return RunSeedersAsync(runRegardless).GetAwaiter().GetResult();
        }

        internal Task<Result> RunSeedersAsync(bool runRegardless)
        {
            return RunSeedersAsync(Task.FromResult(runRegardless));
        }

        internal async Task<Result> RunSeedersAsync(Task<bool> runRegardless)
        {
            foreach (var j in
                GetType().GetAttrs<SeederAttribute>()
                    .Select(p => p.Value)
                    .Concat(FindSeeders())
                    .Distinct()
                    .OrderBy(p => p.GetAttr<OrderAttribute>()?.Value))
            {
                if (!j.IsInstantiable()) continue;
                var s = j.New<IAsyncDbSeeder>();
                if (await runRegardless || await s.ShouldRunAsync(this, Reporter))
                {
                    var r = await s.SeedAsync(this, Reporter);
                    if (r.Result != Result.Ok)
                    {
                        AlertTarget?.Alert(string.Format(St.ErrorInitDb, FriendlyName.ToLower()), r.Message);
                        return r.Result;
                    }
                }
            }
            await runRegardless;
            return await InternalSaveAsync();
        }

        private IEnumerable<Type> FindSeeders()
        {
            var pt = Objects.PublicTypes<IAsyncDbSeeder>();
            foreach (var j in pt)
            {
                var k = j.GetAttrs<SeederForAttribute>().Select(p => p.Value);
                if (k.Contains(GetType())) yield return j;
            }
        }

        /// <summary>
        /// Obtiene una entidad que referencia a este equipo como una 
        /// estación de trabajo representada en la base de datos.
        /// </summary>
        /// <typeparam name="TEstacion">
        /// Modelo de datos de la estación de trabajo.
        /// </typeparam>
        /// <param name="instance">
        /// Instancia del servicio desde la cual obtener información.
        /// </param>
        /// <returns>
        /// Una entidad que representa a este equipo como una estación de
        /// trabajo representada en la base de datos, o
        /// <see langword="null"/> si este equipo no ha sido registrado.
        /// </returns>
        protected static TEstacion? GetStation<TEstacion>(Service? instance) where TEstacion : EstacionBase, new()
        {
            return instance?.Get<TEstacion>(Environment.MachineName);
        }

        /// <summary>
        /// Obtiene una entidad que referencia a este equipo como una 
        /// estación de trabajo representada en la base de datos.
        /// </summary>
        /// <typeparam name="TEstacion">
        /// Modelo de datos de la estación de trabajo.
        /// </typeparam>
        /// <typeparam name="TService">
        /// Servicio desde el cual obtener información.
        /// </typeparam>
        /// <returns>
        /// Una entidad que representa a este equipo como una estación de
        /// trabajo representada en la base de datos, o
        /// <see langword="null"/> si este equipo no ha sido registrado.
        /// </returns>
        protected static TEstacion? GetStation<TEstacion, TService>() where TEstacion : EstacionBase, new() where TService : Service, new()
        {
            return GetStation<TEstacion>(Proteus.Service<TService>());
        }

        /// <summary>
        /// Obtiene una entidad que referencia a este equipo como una 
        /// estación de trabajo representada en la base de datos.
        /// </summary>
        /// <typeparam name="TEstacion">
        /// Modelo de datos de la estación de trabajo.
        /// </typeparam>
        /// <returns>
        /// Una entidad que representa a este equipo como una estación de
        /// trabajo representada en la base de datos, o
        /// <see langword="null"/> si este equipo no ha sido registrado.
        /// </returns>
        protected static TEstacion? GetStation<TEstacion>() where TEstacion : EstacionBase, new()
        {
            return GetStation<TEstacion>(Proteus.Infer(typeof(TEstacion)) 
                ?? throw new InvalidOperationException(string.Format(St.Svc4EntNotFound,typeof(TEstacion))));
        }

        /// <summary>
        /// Obtiene una entidad que referencia al usuario actual como un 
        /// operario específico representado en la base de datos.
        /// </summary>
        /// <typeparam name="TUser">
        /// Modelo de datos del usuario.
        /// </typeparam>
        /// <returns>
        /// Una entidad que referencia al usuario actual como un operario
        /// específico representado en la base de datos, o
        /// <see langword="null"/> si el usuario no ha sido registrado.
        /// </returns>
        protected static TUser? GetUser<TUser>() where TUser : ModelBase, IUserBase, new()
        {
            return GetUser<TUser>(Proteus.Infer(typeof(TUser))
                ?? throw new InvalidOperationException(string.Format(St.Svc4EntNotFound, typeof(TUser))));
        }

        /// <summary>
        /// Obtiene una entidad que referencia al usuario actual como un 
        /// operario específico representado en la base de datos.
        /// </summary>
        /// <typeparam name="TUser">
        /// Modelo de datos del usuario.
        /// </typeparam>
        /// <typeparam name="TService">
        /// Servicio desde el cual obtener información.
        /// </typeparam>
        /// <returns>
        /// Una entidad que referencia al usuario actual como un operario
        /// específico representado en la base de datos, o
        /// <see langword="null"/> si el usuario no ha sido registrado.
        /// </returns>
        protected static TUser? GetUser<TUser, TService>() where TUser : ModelBase, IUserBase, new() where TService : Service, new()
        {
            return GetUser<TUser>(Proteus.Service<TService>());
        }

        /// <summary>
        /// Obtiene una entidad que referencia al usuario actual como un 
        /// operario específico representado en la base de datos.
        /// </summary>
        /// <typeparam name="TUser">
        /// Modelo de datos del usuario.
        /// </typeparam>
        /// <param name="instance">
        /// Instancia del servicio desde la cual obtener información.
        /// </param>
        /// <returns>
        /// Una entidad que referencia al usuario actual como un operario
        /// específico representado en la base de datos, o
        /// <see langword="null"/> si el usuario no ha sido registrado.
        /// </returns>
        protected static TUser? GetUser<TUser>(Service? instance) where TUser : ModelBase, IUserBase, new()
        {
            if (Proteus.Session is null) return null;
            return instance?.FirstOrDefault<TUser>(p => p.UserId == Proteus.Session.Id);
        }        
    }

    /// <inheritdoc cref="Service" />
    /// <summary>
    /// Clase base para los servicios que pueden ser ofrecidos por Proteus,
    /// asociándoles un <see cref="ProteusContext" /> específico a utilizar.
    /// </summary>
    public abstract class Service<T> : Service where T : ProteusContext, new()
    {
        /// <inheritdoc />
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="Service{T}" />.
        /// </summary>
        protected Service() : base(new T()) { }

        /// <summary>
        /// Contexto de Entity Framework asociado a este servicio.
        /// </summary>
        protected new T Context => (T)base.Context;
    }

    /// <inheritdoc cref="Service" />
    /// <summary>
    /// Clase base para los servicios que pueden ser ofrecidos por Proteus,
    /// asociándoles un <see cref="ProteusContext" /> y un 
    /// <see cref="ISettingsRepository"/> específicos a utilizar.
    /// </summary>
    /// <typeparam name="TContext">
    /// Tipo específico del contexto de datos a controlar por esta
    /// instancia.
    /// </typeparam>
    /// <typeparam name="TSettings">
    /// Enumeración con los valores de configuración.
    /// </typeparam>
    public abstract class Service<TContext, TSettings> : Service<TContext>, ISettingsRepository where TContext : ProteusContext, new() where TSettings : class, ISettingsRepository, new()
    {
        /// <summary>
        /// Obtiene o establece un valor de configuración.
        /// </summary>
        /// <param name="customSetting"></param>
        /// <returns></returns>
        public Setting this[string customSetting]
        {
            get => SettingsRepo[customSetting];
            set => SettingsRepo[customSetting] = value;
        }

        /// <summary>
        /// Obtiene el repositorio de configuración asociado a este 
        /// servicio.
        /// </summary>
        public static TSettings SettingsRepo { get; } = Objects.FindSingleObject<TSettings>() ?? throw new MissingTypeException(typeof(TSettings));

        /// <summary>
        /// Enumera todos los valores de configuración de este servicio.
        /// </summary>
        public IEnumerable<Setting> Settings => SettingsRepo.Settings;

        /// <summary>
        /// Obtiene el <see cref="Guid"/> asociado al repositorio de configuración de este servicio.
        /// </summary>
        public Guid Guid => SettingsRepo.Guid;

        /// <summary>
        /// Ejecuta una operación de semillado de la base de datos de configuración asociada a este servicio.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="reporter"></param>
        /// <returns></returns>
        public Task<DetailedResult> SeedAsync(IFullService service, IStatusReporter? reporter)
        {
            return SettingsRepo.SeedAsync(service, reporter);
        }

        /// <summary>
        /// Obtiene un valor de forma asíncrona que determina si este servicio requiere de semillado de los valores de configuración.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="reporter"></param>
        /// <returns></returns>
        public Task<bool> ShouldRunAsync(IReadAsyncService service, IStatusReporter? reporter)
        {
            return SettingsRepo.ShouldRunAsync(service, reporter);
        }
    }
}