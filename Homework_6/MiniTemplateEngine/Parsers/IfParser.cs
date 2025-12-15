using MiniTemplateEnginge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTemplateEnginge.Parsers
{
    public enum Content
    {
        If,
        Else
    }

    public class IfParser
    {
        public static IfModel Parse(string htmlTemplate, int startIndex)
        {
            Condition? condition = default(Condition);
            var ifContent = "";
            var elseContent = "";

            var depth = -1;
            int i = startIndex;
            var content = Content.If;

            while (i < htmlTemplate.Length)
            {
                var span = htmlTemplate.AsSpan(i);

                //Если самый внешний if (то есть который парсим), то парсим условие и передвигаем указатель
                if (span.StartsWith("$if(") && depth == -1)
                {
                    depth = 0;
                    condition = ParseCondition(htmlTemplate, i, out var continueIndex);
                    i = continueIndex;
                    continue;
                }

                //Если открывается вложенная конструкция добавляем глубину
                else if (span.StartsWith("$foreach(") || span.StartsWith("$if("))
                {
                    depth++;
                }

                //Если закрывается вложенная конструкция вычитаем глубину
                else if ((span.StartsWith("$endfor") || span.StartsWith("$endif")) && depth != 0)
                {
                    depth--;
                }

                //Если встретили else и глубина равна 0(то есть он относиться к нашему if) то начинаем записывать в elseContent
                else if (span.StartsWith("$else") && depth == 0)
                {
                    i += "$else".Length;
                    content = Content.Else;
                    continue;
                }

                //Если встретили Endif и глубина равна 0(то есть конец нашего if) то прекращаем цикл
                else if (span.StartsWith("$endif") && depth == 0)
                {
                    i += "$endif".Length;
                    break;
                }

                //Просто записываем контент в нужный блок
                if (content == Content.If) ifContent += htmlTemplate[i];
                else elseContent += htmlTemplate[i];
                i++;
            }

            return new IfModel()
            {
                Condition = condition,
                ElseContent = elseContent,
                IfContent = ifContent,
                Start = startIndex,
                Length = i - startIndex,
            };
        }

        private static Condition ParseCondition(string source, int start, out int continueIndex)
        {
            var conditionPath = "";
            var beginWrite = false;

            for (int i = start; i < source.Length; i++)
            {
                var c = source[i];

                if (c == '(')
                {
                    beginWrite = true;
                    continue;
                }

                else if (c == ')')
                {
                    continueIndex = i + 1;
                    return new Condition() { PropertyPath = conditionPath };
                }

                if (beginWrite) conditionPath += source[i];
            }

            continueIndex = start + 1;
            return new Condition() { PropertyPath = string.Empty };
        }
    }
}
