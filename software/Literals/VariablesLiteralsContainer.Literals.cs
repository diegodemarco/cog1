#pragma warning disable 1591

namespace cog1.Literals
{

    public static class VariablesLiterals
    {

        public class Variable : LiteralConstant
        {
            public override string EN => "Variable";
            public override string ES => "Variable";
        }

        public class Variables : LiteralConstant
        {
            public override string EN => "Variables";
            public override string ES => "Variables";
        }

        public class Binary : LiteralConstant
        {
            public override string EN => "Binary";
            public override string ES => "Binario";
        }

        public class Integer : LiteralConstant
        {
            public override string EN => "Integer";
            public override string ES => "Entero";
        }

        public class FloatingPoint : LiteralConstant
        {
            public override string EN => "Floating point";
            public override string ES => "Punto flotante";
        }

    }

}

#pragma warning restore 1591