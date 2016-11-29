using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitParser.Test
{
    class ArithmeticParser : CompositeParser
    {
        Parser terms = null;
        Parser addition = null;
        Parser factors = null;
        Parser multiplication = null;
        Parser power = null;
        Parser primary = null;
        Parser parentheses = null;
        Parser number = null;

        public override Parser Start()
        {
            return terms.End;
        }

        public Parser Addition()
        {
            return factors.SeparatedBy('+'.AsParser().Or('-'.AsParser()).Trim(Space))
                .Map(nodes =>
                {
                    double subTotal = (double)nodes[0];
                    for (int i = 1; i < nodes.Length; i += 2)
                    {
                        char operand = (char)nodes[i];
                        double next = (double)nodes[i + 1];
                        switch (operand)
                        {
                            case '+': subTotal += next; break;
                            case '-': subTotal -= next; break;
                            default: throw new Exception("Invalid operand");
                        }
                    }
                    return subTotal;
                });
        }

        public Parser Factors()
        {
            return multiplication.Or(power);
        }

        public Parser Multiplication()
        {
            return power.SeparatedBy('*'.AsParser().Or('/'.AsParser()).Trim(Space))
                .Map(nodes =>
                {
                    double subTotal = (double)nodes[0];
                    for (int i = 1; i < nodes.Length; i += 2)
                    {
                        char operand = (char)nodes[i];
                        double next = (double)nodes[i + 1];
                        switch (operand)
                        {
                            case '*': subTotal *= next; break;
                            case '/': subTotal /= next; break;
                            default: throw new Exception("Invalid operand");
                        }
                    }
                    return subTotal;
                });
        }

        public Parser Number()
        {
            return '-'.AsParser().Optional
                .Then(Digit.Plus)
                .Then('.'.AsParser().Then(Digit.Plus).Optional)
                .Flatten
                .Trim(Space)
                .Map<string, double>(value => double.Parse(value));
        }

        public Parser Parentheses()
        {
            return '('.AsParser().Trim(Space)
                .Then(terms)
                .Then(')'.AsParser().Trim(Space))
                .Map(nodes => nodes[1]);
        }

        public Parser Power()
        {
            return primary.SeparatedBy('^'.AsParser().Trim(Space))
                .Map(nodes =>
                {
                    nodes = nodes.Reverse().ToArray(); // Right to left
                    double subTotal = (double)nodes[0];
                    for (int i = 1; i < nodes.Length; i += 2)
                    {
                        char operand = (char)nodes[i];
                        double next = (double)nodes[i + 1];
                        subTotal = Math.Pow(subTotal, next);
                    }
                    return subTotal;
                });
        }

        public Parser Primary()
        {
            return number.Or(parentheses);
        }

        public Parser Terms()
        {
            return addition.Or(factors);
        }
    }
}
