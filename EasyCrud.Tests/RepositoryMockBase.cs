﻿using EasyCrud.Logging;
using EasyCrud.Models;
using EasyCrud.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyCrud.Tests
{
    public class RepositoryMockBase<TRecordType, TIndexType> : RepositoryBase<TRecordType, TIndexType> where TRecordType : class , IModel<TIndexType>
    {
        public List<TRecordType> StoreList { get; set; }

        public RepositoryMockBase(ILogger logger)
            : base(logger)
        {
            StoreList = new List<TRecordType>();
        }

        public override void Dispose()
        {
        }

        //Needs to  be overridden by child clases if the model they handle has autogenerated keys
        protected virtual TIndexType GenerateIndex()
        {
            return default(TIndexType);
        }

        public override Task<TIndexType> CreateAsync(TRecordType input)
        {
            if (input.KeyIsAutogenerated())
            {
                var newKey = GenerateIndex();
                input.SetKey(newKey);
            }

            StoreList.Add(input);
            var output = input.GetKey();
            return Task.FromResult(output);
        }

        public override Task<TRecordType> ReadAsync(TIndexType id)
        {
            var output = StoreList.FirstOrDefault(d => d.KeyEqual(id));
            return Task.FromResult(output);
        }

        public override Task UpdateAsync(TRecordType input)
        {
            var target = ReadAsync(input.GetKey()).Result;
            if (target == null)
            {
                return Task.FromResult(false);
            }

            StoreList.Remove(target);
            StoreList.Add(input);
            return Task.FromResult(true);
        }

        public override Task DeleteAsync(TIndexType id)
        {
            var target = ReadAsync(id).Result;
            if (target == null)
            {
                return Task.FromResult(false);
            }

            StoreList.Remove(target);
            return Task.FromResult(true);
        }

        public override void ClearData()
        {
            StoreList.Clear();
        }

        public override IQueryable<TRecordType> GetQueryable()
        {
            return StoreList.AsQueryable();
        }
    }
}
