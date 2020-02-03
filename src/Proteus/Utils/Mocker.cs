/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Utils
{
    public static class Mocker
    {
        public static readonly HashSet<IMockFiller> RegisteredFillers = new HashSet<IMockFiller>();

        public static readonly IEnumerable<IMockFiller> DefaultFillers = new IMockFiller[]
        {
            new StringFiller(),
            new FloatFiller(), 
            new DoubleFiller(), 
            new IntFiller(), 
            new ByteFiller(), 
            new ShortFiller(), 
            new DecimalFiller(), 
            new DateTimeFiller(), 
            new LongFiller()
        };

        static Mocker()
        {
            foreach (var j in MCART.Objects.PublicTypes<IMockFiller>().Where(p=>p.IsInstantiable()))
            {
                if (DefaultFillers.Select(p=>p.GetType()).Contains(j)) continue;
                RegisteredFillers.Add(j.New<IMockFiller>());
            }
        }

        public static IEnumerable<T> Create<T>(int count) where T : class, new()
        {
            for (var j = 0; j < count; j++) yield return Create<T>();
        }
        public static T Create<T>() where T : class, new()
        {
            var x = new T();

            foreach (var j in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanWrite))
            {
                (RegisteredFillers.FirstOrDefault(p => p.CanFill(j.PropertyType)) ??
                 DefaultFillers.FirstOrDefault(p => p.CanFill(j.PropertyType)))?.Fill(j,x);
            }
            return x;
        }

        public static object Create(Type type)
        {
            return Create(type, null);
        }

        public static object Create(Type type, IEnumerable<IMockFiller> fillers)
        {
            if (!type.IsInstantiable()) return null;
            var x = type.New();
            var mockFillers = fillers?.ToList();
            foreach (var j in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanWrite))
            {
                (mockFillers?.FirstOrDefault(p => p.CanFill(j.PropertyType)) ??
                RegisteredFillers.FirstOrDefault(p => p.CanFill(j.PropertyType)) ??
                DefaultFillers.FirstOrDefault(p => p.CanFill(j.PropertyType)))?.Fill(j, x);
            }
            return x;
        }
    }

    public interface IMockFiller
    {
        bool CanFill(Type type);
        void Fill(PropertyInfo property, object instance);
    }

    public class StringFiller : Filler<string>
    {
        public override string GetValue(PropertyInfo property, object instance)
        {
            return property.Name;
        }
    }
    public class FloatFiller : Filler<float>
    {
        public override float GetValue(PropertyInfo property, object instance)
        {
            return 123.4f;
        }
    }
    public class DoubleFiller : Filler<double>
    {
        public override double GetValue(PropertyInfo property, object instance)
        {
            return 123.456;
        }
    }
    public class IntFiller : Filler<int>
    {
        public override int GetValue(PropertyInfo property, object instance)
        {
            return 123456;
        }
    }
    public class ByteFiller : Filler<byte>
    {
        public override byte GetValue(PropertyInfo property, object instance)
        {
            return 123;
        }
    }
    public class ShortFiller : Filler<short>
    {
        public override short GetValue(PropertyInfo property, object instance)
        {
            return 1234;
        }
    }
    public class DateTimeFiller : Filler<DateTime>
    {
        public override DateTime GetValue(PropertyInfo property, object instance)
        {
            return DateTime.Now;
        }
    }
    public class LongFiller : Filler<long>
    {
        public override long GetValue(PropertyInfo property, object instance)
        {
            return 123456789;
        }
    }
    public class DecimalFiller : Filler<decimal>
    {
        public override decimal GetValue(PropertyInfo property, object instance)
        {
            return 1234567.89m;
        }
    }
    public abstract class Filler<T> : IMockFiller
    {
        public bool CanFill(Type type)
        {
            return type == typeof(T);
        }

        public void Fill(PropertyInfo property, object instance)
        {
            property.SetMethod.Invoke(instance, new object[] { GetValue(property,instance) });
        }

        public abstract T GetValue(PropertyInfo property, object instance);
    }
}
