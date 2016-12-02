using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetitParser.Results;
using PetitParser.Utils;
using System.Reflection;

namespace PetitParser
{
    public abstract class CompositeParser : Parser
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public CompositeParser()
        {
            InitializeChildren();
        }

        protected abstract Parser Start();

        protected virtual void InitializeChildren()
        {
            ParserFieldsDo(field => field.SetValue(this, new DelegateParser()));
            ParserFieldsDo(Define);
        }

        private void ParserFieldsDo(Action<FieldInfo> action)
        {
            ParserFieldsDo(GetType(), action);
        }

        private void ParserFieldsDo(Type type, Action<FieldInfo> action)
        {
            foreach (FieldInfo field in type.GetFields(flags))
            {
                if (field.FieldType == typeof(Parser) ||
                    field.FieldType.IsSubclassOf(typeof(Parser)))
                {
                    action(field);
                }
            }
            if (type == typeof(CompositeParser)) return;
            ParserFieldsDo(type.BaseType, action);
        }

        private void Define(FieldInfo field)
        {
            DelegateParser undefined = (DelegateParser)field.GetValue(this);
            string methodName = GetMethodNameFor(field);
            MethodInfo method = GetType().GetMethod(methodName, flags);
            if (method == null)
            {
                throw new Exception(string.Format("{0} should have implemented the \"{1}\" method", this, methodName));
            }
            Parser actual = (Parser)method.Invoke(this, null);
            undefined.Define(actual);
        }

        private string GetMethodNameFor(FieldInfo field)
        {
            return char.ToUpper(field.Name[0]).ToString() + field.Name.Substring(1);
        }

        public override ParseResult ParseOn(Stream stream)
        {
            return Start().ParseOn(stream);
        }
    }
}
