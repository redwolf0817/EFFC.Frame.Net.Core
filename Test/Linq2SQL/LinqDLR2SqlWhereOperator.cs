using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Linq2SQL
{
    public class LinqDLR2SqlWhereOperator : MyDynamicMetaProvider
    {
        string initOperatorResultString = "";
        public LinqDLR2SqlWhereOperator(string operatorstring, Dictionary<string, object> conditionsParameters)
        {
            initOperatorResultString = operatorstring;
            Result = initOperatorResultString;
            ConditionValues = new Dictionary<string, object>();
            AddConditions(conditionsParameters);
            OperatorTreeLevel = 0;
            AndFlag = "AND";
            OrFlag = "OR";
        }

        private void AddConditions(Dictionary<string, object> conditionsParameters)
        {
            if (conditionsParameters != null)
            {
                foreach (var item in conditionsParameters)
                {
                    if (!ConditionValues.ContainsKey(item.Key))
                    {
                        ConditionValues.Add(item.Key, item.Value);
                    }
                }
            }
        }
        public string AndFlag
        {
            get;
            set;
        }
        public string OrFlag
        {
            get;
            set;
        }
        public string LastOperator
        {
            get;
            private set;
        }
        public int OperatorTreeLevel
        {
            get;
            private set;
        }
        public string Result
        {
            get;
            private set;
        }
        public Dictionary<string, object> ConditionValues
        {
            get;
            private set;
        }
        protected override object GetMetaValue(string key)
        {
            return null;
        }

        protected override object InvokeMe(string methodInfo, params object[] args)
        {
            return this;
        }

        protected override object SetMetaValue(string key, object value)
        {
            return this;
        }

        protected override object MetaAndAlso(object v)
        {
            return MetaAnd(v);
        }
        protected override object MetaOrElse(object v)
        {
            return MetaOr(v);
        }
        protected override object MetaAnd(object v)
        {
            if (v is LinqDLR2SqlWhereOperator)
            {
               
                var rigthTarget = (LinqDLR2SqlWhereOperator)v;
                //如果右侧表达式上次操作与本次操作相同
                if (rigthTarget.LastOperator == "AND")
                {
                    Result += $" {AndFlag} {rigthTarget.Result}";
                }
                else
                {
                    //自身的操作等级为0
                    if(OperatorTreeLevel <= 0)
                    {
                        //如果右侧的操作等级也为0
                        if(rigthTarget.OperatorTreeLevel <= 0)
                        {
                            Result += $" {AndFlag} {rigthTarget.Result}";
                        }
                        else
                        {
                            Result += $" {AndFlag} ({rigthTarget.Result})";
                        }
                    }
                    else
                    {
                        //如果右侧的操作等级也为0
                        if (rigthTarget.OperatorTreeLevel <= 0)
                        {
                            //如果上次操作也为AND
                            if(LastOperator == "AND")
                            {
                                Result += $" {AndFlag} {rigthTarget.Result}";
                            }
                            else
                            {
                                Result = $"({Result}) {AndFlag} {rigthTarget.Result}";
                            }
                        }
                        else
                        {
                            //如果上次操作也为AND
                            if (LastOperator == "AND")
                            {
                                Result += $" {AndFlag} ({rigthTarget.Result})";
                            }
                            else
                            {
                                Result = $"({Result}) {AndFlag} ({rigthTarget.Result})";
                            }
                        }
                    }
                }

                LastOperator = "AND";

                AddConditions(((LinqDLR2SqlWhereOperator)v).ConditionValues);
                OperatorTreeLevel++;
                //Console.WriteLine($"{Result}");
            }
            return this;
        }
        protected override object MetaOr(object v)
        {
            if (v is LinqDLR2SqlWhereOperator)
            {
                var rigthTarget = (LinqDLR2SqlWhereOperator)v;

                //自身的操作等级为0
                if (OperatorTreeLevel <= 0)
                {
                    if (rigthTarget.OperatorTreeLevel <= 0)
                    {
                        Result += $" {OrFlag} {rigthTarget.Result}";
                    }
                    else
                    {
                        Result += $" {OrFlag} ({rigthTarget.Result})";
                    }
                }
                else
                {
                    if (rigthTarget.OperatorTreeLevel <= 0)
                    {
                        Result += $" {OrFlag} {rigthTarget.Result}";
                    }
                    else
                    {
                        if(LastOperator == "OR")
                        {
                            Result = $"{Result} {OrFlag} ({rigthTarget.Result})";
                        }
                        else
                        {
                            Result = $"({Result}) {OrFlag} ({rigthTarget.Result})";
                        }
                       
                    }
                }

                LastOperator = "OR";

                AddConditions(((LinqDLR2SqlWhereOperator)v).ConditionValues);
                OperatorTreeLevel++;
                //Console.WriteLine($"{Result}");
            }
            return this;
        }

        protected override bool MetaIsTrue()
        {
            return false;
        }
        protected override bool MetaIsFalse()
        {
            return false;
        }
    }


}
