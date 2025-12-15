using MiniTemplateEnginge.Common;
using MiniTemplateEnginge.Models;
using MiniTemplateEnginge.Parsers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MiniTemplateEnginge.Core
{
    public class HtmlTemplateRenderer : IHtmlTemplateRenderer
    {

        private static readonly Regex ParameterBlock = new Regex(
            @"\$\{(?<property>[\w\.]+)\}",
            RegexOptions.Compiled
        );

        public string RenderToFile(string inputFilePath, string outputFilePath, object dataModel)
        {
            var render = RenderFromFile(inputFilePath, dataModel);

            File.WriteAllText(render, outputFilePath, Encoding.UTF8);

            return render;
        }

        public string RenderFromFile(string filePath, object dataModel)
        {
            try
            {
                var htmlTemplate = File.ReadAllText(filePath);

                return RenderFromString(htmlTemplate, dataModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return string.Empty;
            }
        }

        public string RenderFromString(string htmlTemplate, object dataModel)
        {
            bool hasNext = TryGetNextBlock(htmlTemplate, out BlockType blockType, out var model);

            while (hasNext)
            {
                string replacement = string.Empty;

                switch (blockType)
                {
                    case BlockType.If:
                        replacement = HandleIfBlock(htmlTemplate, dataModel, (model as IfModel)!);
                        break;

                    case BlockType.Foreach:
                        replacement = HandleForBlock(htmlTemplate, dataModel, (model as ForeachModel)!);
                        break;

                    case BlockType.Data:
                        replacement = HandleDataBlock(htmlTemplate, dataModel, (model as DataModel)!);
                        break;
                }

                htmlTemplate = htmlTemplate.Remove(model.Start, model.Length);
                htmlTemplate = htmlTemplate.Insert(model.Start, replacement);


                hasNext = TryGetNextBlock(htmlTemplate, out blockType, out model);
            }

            return htmlTemplate;
        }

        public bool TryGetNextBlock(string input, out BlockType blockType, out BaseModel model)
        {

            blockType = BlockType.None;
            model = default;

            if (string.IsNullOrEmpty(input))
                return false;

            int i = 0;
            while (i < input.Length)
            {
                if (input[i] == '$')
                {
                    if (input.AsSpan(i).StartsWith("$if("))
                    {
                        model = IfParser.Parse(input, i);
                        blockType = BlockType.If;

                        return true;
                    }

                    else if (input.AsSpan(i).StartsWith("$foreach("))
                    {
                        model = ForeachParser.Parse(input, i);
                        blockType = BlockType.Foreach;

                        return true;
                    }

                    else if (input.AsSpan(i).StartsWith("${"))
                    {
                        var m = ParameterBlock.Match(input, i - 1);

                        if (m.Success)
                        {
                            model = new DataModel()
                            {
                                Content = m.Groups[1].Value,
                                Start = i,
                                Length = m.Length
                            };
                            blockType = BlockType.Data;

                            return true;
                        }

                        return false;
                    }
                }
                i++;
            }

            return false;
        }

        private string HandleIfBlock(string htmlTemplate, object dataModel, IfModel model)
        {
            string condition = model.Condition.PropertyPath;
            string ifContent = model.IfContent;
            string elseContent = model.ElseContent ?? string.Empty;

            bool conditionResult = Helper.IsBoolProperty(condition, dataModel);

            string selected = conditionResult ? ifContent : elseContent;

            string rendered = RenderFromString(selected, dataModel);

            return rendered;
        }

        private string HandleForBlock(string htmlTemplate, object dataModel, ForeachModel model)
        {
            string varName = model.IterationModel.PropertyName;
            string collectionPath = model.IterationModel.CollectionPath;

            var collectionObj = Helper.GetPropertyValue(dataModel, collectionPath);

            if (collectionObj is not IEnumerable enumerable)
                return string.Empty;

            string result = string.Empty;

            foreach (var item in enumerable)
            {
                var scoped = new ScopedModel(dataModel, varName, item);

                result += RenderFromString(model.Content, scoped);
            }

            return result;
        }

        private string HandleDataBlock(string htmlTemplate, object dataModel, DataModel model)
        {
            string propertyPath = model.Content;

            var value = Helper.GetPropertyValue(dataModel, propertyPath);

            return value?.ToString() ?? string.Empty;
        }
    }

}

    public enum BlockType
    {
        If,
        Foreach,
        Data,
        None
    }

