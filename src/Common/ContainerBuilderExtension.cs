﻿using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad;

namespace Xarial.CadPlus.Common
{
    public static class ContainerBuilderExtension
    {
        public static void RegisterFromServiceProvider<TSvc>(this ContainerBuilder builder, IServiceProvider svcProv)
            where TSvc : class
            => builder.RegisterInstance((TSvc)svcProv.GetService(typeof(TSvc))).As<TSvc>();

        public static void Populate(this ContainerBuilder builder, IXServiceCollection svcColl) 
        {
            foreach (var svc in svcColl.Services)
            {
                builder.Register<object>(x => svc.Value.Invoke())
                    .As(svc.Key);
            }
        }
    }
}
