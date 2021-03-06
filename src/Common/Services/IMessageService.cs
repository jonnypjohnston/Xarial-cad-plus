﻿//*********************************************************************
//CAD+ Toolset
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://cadplus.xarial.com
//License: https://cadplus.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xarial.XToolkit.Reporting;

namespace Xarial.CadPlus.Common.Services
{
    public interface IMessageService
    {
        void ShowError(string error);
        void ShowInformation(string msg);
        bool? ShowQuestion(string question);
    }

    public static class IMessageServiceExtension 
    {
        public static void ShowError(this IMessageService msgSvc, Exception ex, string baseMsg)
        {
            var err = ex.ParseUserError(out _, baseMsg);
            msgSvc.ShowError(err);
        }
    }
}
