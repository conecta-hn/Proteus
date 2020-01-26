/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component.Attributes;
using TheXDS.Proteus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.Types;

namespace TheXDS.Proteus.Component
{
    public abstract class SettingsRepository<T> : SettingsRepository where T : Enum
    {
        public Setting this[T setting]
        {
            get => this[setting.ToString()];
            set => this[setting.ToString()] = value;
        }
        protected override IEnumerable<KeyValuePair<string, string>> Defaults()
        {
            foreach (T j in typeof(T).GetEnumValues())
            {
                if (j.HasAttr<DefaultAttribute>(out var v))
                {
                    yield return new KeyValuePair<string, string>(j.ToString(), v.Value);
                }
            }
        }
    }

    [SeederFor(typeof(UserService))]
    public abstract class SettingsRepository : ISettingsRepository
    {
        private readonly IExposeGuid _implementor;
        public SettingsRepository()
        {
            _implementor = new ExposeGuidImplementor(this);
        }

        protected ConfigRepository Repo => Proteus.Service<UserService>().Get<ConfigRepository, Guid>(Guid);

        public IEnumerable<Setting> Settings => Repo.Settings;

        public Guid Guid => _implementor.Guid;

        public string Name => GetType().NameOf();

        public Setting this[string customSetting]
        {
            get
            {
                return Repo.Settings.FirstOrDefault(p => p.Id == customSetting);
            }
            set
            {
                if (this[customSetting] is null)
                    Repo.Settings.Add(new Setting { Id = customSetting, Value = value.Value });
                else
                    this[customSetting].Value = value.Value;
                Proteus.Service<UserService>().SaveAsync();
            }
        }

        public Task<DetailedResult> SeedAsync(IFullService service, IStatusReporter reporter)
        {
            reporter?.UpdateStatus($"Creando repositorio de configuración {Guid}");
            var r = new ConfigRepository { Id = Guid };
            foreach (var j in Defaults()) r.Settings.Add(j);
            return service.AddAsync(r);
        }

        protected virtual IEnumerable<KeyValuePair<string, string>> Defaults()
        {
            yield break;
        }

        public async Task<bool> ShouldRunAsync(IReadAsyncService service, IStatusReporter reporter)
        {
            reporter?.UpdateStatus($"Comprobando repositorio de configuración {Guid}...");
            return await service.GetAsync<ConfigRepository, Guid>(Guid) is null;
        }
    }
}