using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    /// <summary>
    /// Implémente une FILO à capacité limitée. Une fois la capacité atteinte, les éléments les
    /// plus anciens sont oubliés.
    /// </summary>
    public class CLActionStack
    {
        public const int StackSize = 50;
        public int Count => ((beg - end) + StackSize) % StackSize;
        private CLAction[] stack;
        private int beg;
        private int end;

        public CLActionStack()
        {
            stack = new CLAction[StackSize];
            beg = 0;
            end = 0;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int i = beg;
            while (i != end)
            {
                i = (i + StackSize - 1) % StackSize;
                sb.AppendLine(stack[i].ToString());
            }
            return sb.ToString();
        }

        public void Push(CLAction theAction)
        {
            stack[beg] = theAction;
            beg = (beg + 1) % StackSize;
            if (beg == end)
            {
                end = (end + 1) % StackSize;
            }
        }

        /// <summary>
        /// Retire l'élément qui se trouve au sommet de la pile.
        /// </summary>
        /// <returns>L'élément enlevé ou <c>null</c> si la ile est vide.</returns>
        public CLAction Pop()
        {
            if (beg == end)
            {
                return null;
            } 
            else
            {
                beg = (beg + StackSize - 1) % StackSize;
                return stack[beg];
            }
        }

        /// <summary>
        /// Vide la pile.
        /// </summary>
        public void Clear()
        {
            beg = 0;
            end = 0;
        }
    }
}
