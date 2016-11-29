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

        protected override Parser Start()
        {
            return terms.End;
        }

        private Parser Addition()
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

        private Parser Factors()
        {
            return multiplication.Or(power);
        }

        private Parser Multiplication()
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

        private Parser Number()
        {
            return '-'.AsParser().Optional
                .Then(Digit.Plus)
                .Then('.'.AsParser().Then(Digit.Plus).Optional)
                .Flatten
                .Trim(Space)
                .Map<string, double>(value => double.Parse(value));
        }

        private Parser Parentheses()
        {
            return '('.AsParser().Trim(Space)
                .Then(terms)
                .Then(')'.AsParser().Trim(Space))
                .Map(nodes => nodes[1]);
        }

        private Parser Power()
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
                        subTotal = Math.Pow(next, subTotal);
                    }
                    return subTotal;
                });
        }

        private Parser Primary()
        {
            return number.Or(parentheses);
        }

        private Parser Terms()
        {
            return addition.Or(factors);
        }
    }
}
