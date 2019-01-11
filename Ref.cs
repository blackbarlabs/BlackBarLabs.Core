﻿using BlackBarLabs.Extensions;
using EastFive.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EastFive
{
    #region IRef(Obj)

    public interface IReferenceable
    {
        Guid id { get; }
    }

    public interface IRefBase : IReferenceable
    {
        Task ResolveAsync();

        bool resolved { get; }
    }

    public interface IRef<TType> : IRefBase
        where TType : struct
    {
        TType? value { get; }
    }

    public interface IRefObj<TType> : IRefBase
        where TType : class
    {
        Func<TType> value { get; }
    }

    #endregion

    #region IRef(Obj)Optional

    public interface IReferenceableOptional
    {
        Guid? id { get; }
    }

    public interface IRefOptionalBase : IReferenceableOptional
    {
        Task ResolveAsync();

        bool resolved { get; }

        bool HasValue { get; }
    }

    public interface IRefOptional<TType> : IRefOptionalBase
        where TType : struct
    {
        TType? value { get; }
    }

    public interface IRefObjOptional<TType> : IRefOptionalBase
        where TType : class
    {
        TType value { get; }
    }

    #endregion

    #region IRef plural

    public interface IReferences
    {
        Guid[] ids { get; }
    }

    public interface IRefs<TType> : IReferences
        where  TType : struct
    {
        IRef<TType>[] refs { get; }

        Linq.Async.IEnumerableAsync<TType> Values { get; }
    }

    #endregion

    public struct Ref<TType> : IRef<TType>
        where TType : struct
    {
        private Task<TType> valueTask;

        public Ref(Task<TType> valueTask)
        {
            this.id = default(Guid);
            this.value = default(TType?);
            this.valueTask = valueTask;
            this.resolved = false;
        }

        public Ref(Guid id) : this()
        {
            this.id = id;
            this.value = default(TType?);
            this.valueTask = default(Task<TType>);
            this.resolved = false;
        }

        public Guid id { get; set; }

        public TType? value { get; set; }
        
        public async Task ResolveAsync()
        {
            if (value.HasValue)
                return;
            
            this.value = await valueTask;
            this.resolved = true;
        }

        public bool resolved { get; set; }
        
        public static implicit operator Ref<TType>(Guid value)
        {
            return new Ref<TType>()
            {
                id = value,
            };
        }
        
        public static implicit operator Ref<TType>(TType value)
        {
            return value.IsDefault() ? 
                default(Ref<TType>) 
                :
                new Ref<TType>(value.AsTask());
        }
    }

    public struct RefObj<TType>
        where TType : class
    {
        public Guid id;
        public TType value;
        public Task<TType> valueAsync;
        public bool resolved;
    }

    public struct RefOptional<TType> : IRefOptional<TType>
        where TType : struct
    {
        private IRef<TType> baseRef;

        public static IRefOptional<TType> Empty()
        {
            return new RefOptional<TType>
            {
                HasValue = false,
                baseRef = default(IRef<TType>),
            };
        }

        public RefOptional(IRef<TType> baseRef)
        {
            this.HasValue = true;
            this.baseRef = baseRef;
        }

        public Guid? id
        {
            get
            {
                if (!this.HasValue)
                    return default(Guid?);
                return baseRef.id;
            }
        }

        public TType? value
        {
            get
            {
                if (!this.HasValue)
                    return default(TType?);
                return baseRef.value;
            }
        }

        public Task ResolveAsync()
        {
            if (!this.HasValue)
                throw new Exception("Attempt to resolve empty value");

            return this.baseRef.ResolveAsync();
        }

        [Newtonsoft.Json.JsonIgnore]
        public bool resolved
        {
            get
            {
                if (!this.HasValue)
                    throw new Exception("Attempt to check resolution of empty value");

                return this.baseRef.resolved;
            }
        }

        public bool HasValue { get; set; }
    }

    public struct RefObjOptional<TType> : IRefObjOptional<TType>
        where TType : class
    {
        private IRefObj<TType> baseRef;

        public static IRefObjOptional<TType> Empty()
        {
            return new RefObjOptional<TType>
            {
                HasValue = false,
                baseRef = default(IRefObj<TType>),
            };
        }

        public RefObjOptional(IRefObj<TType> baseRef)
        {
            this.HasValue = true;
            this.baseRef = baseRef;
        }

        public Guid? id
        {
            get
            {
                if (!this.HasValue)
                    return default(Guid?);
                return baseRef.id;
            }
        }

        public TType value
        {
            get
            {
                if (!this.HasValue)
                    return default(TType);
                return baseRef.value();
            }
        }

        public Task ResolveAsync()
        {
            if (!this.HasValue)
                throw new Exception("Attempt to resolve empty value");

            return this.baseRef.ResolveAsync();
        }

        public bool resolved
        {
            get
            {
                if (!this.HasValue)
                    throw new Exception("Attempt to check resolution of empty value");

                return this.baseRef.resolved;
            }
        }

        public bool HasValue { get; set; }
    }
}
