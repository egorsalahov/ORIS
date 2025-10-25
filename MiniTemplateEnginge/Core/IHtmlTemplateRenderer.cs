﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTemplateEnginge.Core
{
    public interface IHtmlTemplateRenderer
    {
        string RenderFromString(string htmlTemplate, object dataModel);

        string RenderFromFile(string filePath, object dataModel);

        string RenderToFile(string inputFilePath, string outputFilePath, object dataModel);
    }
}
