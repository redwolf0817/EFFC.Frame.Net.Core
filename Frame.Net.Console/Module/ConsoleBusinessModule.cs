using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Business.Logic;
using EFFC.Frame.Net.Business.Logic.WebForm;
using EFFC.Frame.Net.Business.Module;
using EFFC.Frame.Net.Console.Module;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Web.Module
{
    public class ConsoleBusinessModule : AssemblyLoadBusinessModule<ConsoleParameter, DataCollection, ConsoleLogic>
    {
        public override string LogicAssemblyPath
        {
            get { return GlobalCommon.ConsoleCommon.LogicAssemblyPath; }
        }

        public override string LogicName
        {
            get { return this.Parameter.Logic; }
        }

        public override string Description
        {
            get { return "業務處理模塊"; }
        }

        public override string Name
        {
            get { return "ConsoleBusiness"; }
        }

        public override string Version
        {
            get { return "0.0.1"; }
        }

        protected override void OnError(Exception ex, ConsoleParameter p, DataCollection d)
        {
            throw ex;
        }
    }

    public class ConsoleBusinessSingletonModule : AssemblyLoadBusinessSingletonModule<ConsoleParameter, DataCollection, ConsoleLogic>
    {

        public override string LogicAssemblyPath
        {
            get { return GlobalCommon.ConsoleCommon.LogicAssemblyPath; }
        }

        public override string LogicName
        {
            get { return this.Parameter.Logic; }
        }

        protected override void OnError(Exception ex, ConsoleParameter p, DataCollection d)
        {
            throw ex;
        }

        public override string Name
        {
            get { return "ConsoleBusinessSingleton"; }
        }

        public override string Version
        {
            get { return "0.0.1"; }
        }

        public override string Description
        {
            get { return "業務處理模塊-单态"; }
        }
    }
}
