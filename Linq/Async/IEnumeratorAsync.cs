﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EastFive.Linq.Async
{
    public interface IEnumeratorAsync<out T>
    {
        T Current { get; }

        Task<bool> MoveNextAsync();
    }
}
