﻿using DomainTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public interface IStorageRepository
    {
        void add(Storage storage);
        
    }
}