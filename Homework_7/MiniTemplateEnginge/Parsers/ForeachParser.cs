using MiniTemplateEnginge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTemplateEnginge.Parsers
{
    public class ForeachParser
    {
        public static ForeachModel Parse(string template, int startIdnex = 0)
        {
            string content = string.Empty;
            int start = startIdnex;
            IterationModel? iterationModel = default(IterationModel);

            int depth = -1;
            int i = startIdnex;
            while (i < template.Length)
            {
                var span = template.AsSpan(i);

                //Если самый внешний foreach то парсим данные о цикле и передвигаем указатель
                if (span.StartsWith("$foreach(") && depth == -1)
                {
                    iterationModel = ParseIterationModel(template, i, out var continueIndex);
                    i = continueIndex;
                    depth++;
                    continue;
                }

                //Если открывается вложенная конструкция добавляем глубину
                else if (span.StartsWith("$foreach(") || span.StartsWith("$if("))
                {
                    depth++;
                }

                //Если закрывается аложенная конструкция то вычитаем глубину
                else if ((span.StartsWith("$endfor") || span.StartsWith("$endif")) && depth != 0)
                {
                    depth--;
                }

                //Если нашли endfor и depth == 0(то есть наш foreach закрывается) то завершаем цикл
                else if (span.StartsWith("$endfor") && depth == 0)
                {
                    i += "$endfor".Length;
                    break;
                }

                content += template[i];
                i++;
            }

            return new ForeachModel()
            {
                IterationModel = iterationModel,
                Length = i - startIdnex,
                Content = content,
                Start = startIdnex
            };
        }

        private static IterationModel ParseIterationModel(string htmlTemplate, int startIndex, out int continueIndex)
        {
            var cycleString = string.Empty;
            continueIndex = startIndex;

            var beginWrite = false;
            for (int i = startIndex; i < htmlTemplate.Length; i++)
            {
                if (htmlTemplate[i] == '(')
                {
                    beginWrite = true;
                    continue;
                }

                else if (htmlTemplate[i] == ')')
                {
                    continueIndex = i + 1;
                    break;
                }

                if (beginWrite) cycleString += htmlTemplate[i];
            }

            var cycleParameters = cycleString.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (cycleParameters.Length != 4) throw new Exception();

            if (cycleParameters[0] != "var") throw new Exception();

            if (cycleParameters[2] != "in") throw new Exception();

            return new IterationModel()
            {
                CollectionPath = cycleParameters[3],
                PropertyName = cycleParameters[1],
            };
        }
    }
}
