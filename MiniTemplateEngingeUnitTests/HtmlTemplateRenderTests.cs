using MiniTemplateEnginge;
using MiniTemplateEnginge.Core;
using MiniTemplateEnginge.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTemplateEngingeUnitTests
{
    [TestClass]
    public class HtmlTemplateRenderTests
    {
        //render tests


        [TestMethod]
        public void RenderFromString_When_IfFalse_BlockRendersElseSection()
        {
            // arrange
            HtmlTemplateRenderer renderer = new HtmlTemplateRenderer();
            string template = "$if(user.IsActive)<p>User is active</p>$else<p>User is not active</p>$endif";
            var model = new { user = new { IsActive = false } };
            string expected = "<p>User is not active</p>";

            // act
            string result = renderer.RenderFromString(template, model);

            // assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void RenderFromString_When_IfTrue_BlockRendersIfSection()
        {
            // arrange
            HtmlTemplateRenderer renderer = new HtmlTemplateRenderer();
            string template = "$if(user.IsActive)<p>User is active</p>$else<p>User is not active</p>$endif";
            var model = new { user = new { IsActive = true } };
            string expected = "<p>User is active</p>";

            // act
            string result = renderer.RenderFromString(template, model);

            // assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RenderFromString_When_SimpleVariable_ReplacedCorrectly()
        {
            // arrange
            HtmlTemplateRenderer renderer = new HtmlTemplateRenderer();
            string template = "<h1>Привет, ${user.Name}!</h1>";
            var model = new { user = new { Name = "Дима" } };
            string expected = "<h1>Привет, Дима!</h1>";

            // act
            string result = renderer.RenderFromString(template, model);

            // assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RenderFromString_When_NestedForeachAndIf_RendersCorrectly()
        {
            // arrange
            HtmlTemplateRenderer renderer = new HtmlTemplateRenderer();
            string template = "<ul>$foreach(var item in user.Items)$if(item.IsAvailable)<li>${item.Name} - available</li>$else<li>${item.Name} - not available</li>$endif$endfor</ul>";
            var model = new
            {
                user = new
                {
                    Items = new[]
                    {
                        new { Name = "Apple", IsAvailable = true },
                        new { Name = "Banana", IsAvailable = false }
                    }
                }
            };
            string expected = "<ul><li>Apple - available</li><li>Banana - not available</li></ul>";

            // act
            string result = renderer.RenderFromString(template, model);

            // assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RenderFromString_When_Foreach_RendersAllItems()
        {
            // arrange
            HtmlTemplateRenderer renderer = new HtmlTemplateRenderer();
            string template = "<ul>$foreach(var item in user.Items)<li>${item.Name}</li>$endfor</ul>";
            var model = new
            {
                user = new
                {
                    Items = new[]
                    {
                        new { Name = "Apple" },
                        new { Name = "Banana" },
                        new { Name = "Orange" }
                    }
                }
            };
            string expected = "<ul><li>Apple</li><li>Banana</li><li>Orange</li></ul>";

            // act
            string result = renderer.RenderFromString(template, model);

            // assert
            Assert.AreEqual(expected, result);
        }


        //if tests

        [TestMethod]
        public void Parse_OnDoubleIfWhereInternalIfHasElse_ReturnsCorrect()
        {
            // Arrange
            var template = @"$if(user.IsActive)
    <p>User is active</p>
    $if(user.IsPremium)
        <p>Premium user</p>
    $else
        <p>Regular user</p>
    $endif
$endif";

            // Act
            var result = IfParser.Parse(template, 0);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("user.IsActive", result.Condition.PropertyPath);
            Assert.IsTrue(result.IfContent.Contains("$if(user.IsPremium)"));
            Assert.IsTrue(result.IfContent.Contains("$else"));
            Assert.IsTrue(result.IfContent.Contains("<p>Regular user</p>"));
            Assert.AreEqual("", result.ElseContent);
        }

        [TestMethod]
        public void Parse_OnDoubleIfWithoutElse_ReturnCorrect()
        {
            // Arrange
            var template = @"$if(user.IsActive)
    <p>User is active</p>
    $if(user.IsPremium)
        <p>Premium user</p>
    $endif
$endif";

            // Act
            var result = IfParser.Parse(template, 0);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("user.IsActive", result.Condition.PropertyPath);
            Assert.IsTrue(result.IfContent.Contains("$if(user.IsPremium)"));
            Assert.IsTrue(result.IfContent.Contains("<p>Premium user</p>"));
            Assert.AreEqual("", result.ElseContent);
        }

        [TestMethod]
        public void Parse_OnSingleIfWithoutElse_ReturnCorrect()
        {
            // Arrange
            var template = "$if(user.IsActive)<p>User is active</p>$endif";

            // Act
            var result = IfParser.Parse(template, 0);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("user.IsActive", result.Condition.PropertyPath);
            Assert.AreEqual("<p>User is active</p>", result.IfContent);
            Assert.AreEqual("", result.ElseContent);
            Assert.AreEqual(template.Length, result.Length);
        }


        [TestMethod]
        public void Parse_OnDoubleIfWhereBothHasElse_ReturnsCorrect()
        {
            // Arrange
            var template = @"$if(user.IsActive)
    <p>User is active</p>
    $if(user.IsPremium)
        <p>Premium user</p>
    $else
        <p>Regular user</p>
    $endif
$else
    <p>User is not active</p>
$endif";

            // Act
            var result = IfParser.Parse(template, 0);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("user.IsActive", result.Condition.PropertyPath);
            Assert.IsTrue(result.IfContent.Contains("$if(user.IsPremium)"));
            Assert.IsTrue(result.IfContent.Contains("$else"));
            Assert.IsTrue(result.IfContent.Contains("<p>Regular user</p>"));
            Assert.AreEqual("<p>User is not active</p>", result.ElseContent.Trim());
        }


        [TestMethod]
        public void Parse_OnDoubleIfWhereExternalIfHasElse_ReturnsCorrect()
        {
            // Arrange
            var template = @"$if(user.IsActive)
    <p>User is active</p>
    $if(user.IsPremium)
        <p>Premium user</p>
    $endif
$else
    <p>User is not active</p>
$endif";

            // Act
            var result = IfParser.Parse(template, 0);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("user.IsActive", result.Condition.PropertyPath);
            Assert.IsTrue(result.IfContent.Contains("$if(user.IsPremium)"));
            Assert.IsTrue(result.IfContent.Contains("<p>Premium user</p>"));
            Assert.AreEqual("<p>User is not active</p>", result.ElseContent.Trim());
        }


        //foreach tests



        [TestMethod]
        public void Parse_OnDoubleForeach_ReturnsCorrect()
        {
            // Arrange
            var template = @"$foreach(var user in users)
                                    <div>
                                        $foreach(var item in user.Items)
                                            <span>${item.Name}</span>
                                        $endfor
                                    </div>
                                $endfor";

            // Act
            var result = ForeachParser.Parse(template, 0);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.IterationModel);
            Assert.AreEqual("user", result.IterationModel.PropertyName);
            Assert.AreEqual("users", result.IterationModel.CollectionPath);
            Assert.IsTrue(result.Content.Contains("$foreach(var item in user.Items)"));
            Assert.IsTrue(result.Content.Contains("<span>${item.Name}</span>"));
            Assert.AreEqual(0, result.Start);
        }


        [TestMethod]
        public void Parse_OnForeachIfForeach_ReturnsCorrect()
        {
            // Arrange
            var template = @"$foreach(var user in users)
                                    $if(user.IsActive)
                                        $foreach(var item in user.Items)
                                            <p>${item.Name}</p>
                                        $endfor
                                    $endif
                                $endfor";

            // Act
            var result = ForeachParser.Parse(template, 0);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.IterationModel);
            Assert.AreEqual("user", result.IterationModel.PropertyName);
            Assert.AreEqual("users", result.IterationModel.CollectionPath);
            Assert.IsTrue(result.Content.Contains("$if(user.IsActive)"));
            Assert.IsTrue(result.Content.Contains("$foreach(var item in user.Items)"));
            Assert.IsTrue(result.Content.Contains("<p>${item.Name}</p>"));
            Assert.AreEqual(0, result.Start);
        }

        [TestMethod]
        public void Parse_OnForeachForeachIf_ReturnsCorrect()
        {
            // Arrange
            var template = @"$foreach(var user in users)
                                    $foreach(var order in user.Orders)
                                        $if(order.IsCompleted)
                                            <div>${order.Total}</div>
                                        $endif
                                    $endfor
                                $endfor";

            // Act
            var result = ForeachParser.Parse(template, 0);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.IterationModel);
            Assert.AreEqual("user", result.IterationModel.PropertyName);
            Assert.AreEqual("users", result.IterationModel.CollectionPath);
            Assert.IsTrue(result.Content.Contains("$foreach(var order in user.Orders)"));
            Assert.IsTrue(result.Content.Contains("$if(order.IsCompleted)"));
            Assert.IsTrue(result.Content.Contains("<div>${order.Total}</div>"));
            Assert.AreEqual(0, result.Start);
        }

        [TestMethod]
        public void Parse_OnSingleForeach_ReturnsCorrect()
        {
            // Arrange
            var template = @"$foreach(var item in user.Items)
                                <li>${item.Name}</li>
                            $endfor";

            // Act
            var result = ForeachParser.Parse(template, 0);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.IterationModel);
            Assert.AreEqual("item", result.IterationModel.PropertyName);
            Assert.AreEqual("user.Items", result.IterationModel.CollectionPath);
            Assert.AreEqual("<li>${item.Name}</li>", result.Content.Trim());
            Assert.AreEqual(0, result.Start);
        }


        [TestMethod]
        public void Parse_OnForeachWithComplexContent_ReturnsCorrect()
        {
            // Arrange
            var template = @"$foreach(var product in catalog.Products)
                                <div class='product'>
                                    <h3>${product.Name}</h3>
                                    <p>${product.Description}</p>
                                    <span>$${product.Price}</span>
                                    $if(product.InStock)
                                        <button>Add to Cart</button>
                                    $else
                                        <p>Out of Stock</p>
                                    $endif
                                </div>
                            $endfor";

            // Act
            var result = ForeachParser.Parse(template, 0);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.IterationModel);
            Assert.AreEqual("product", result.IterationModel.PropertyName);
            Assert.AreEqual("catalog.Products", result.IterationModel.CollectionPath);
            Assert.IsTrue(result.Content.Contains("$if(product.InStock)"));
            Assert.IsTrue(result.Content.Contains("<button>Add to Cart</button>"));
            Assert.IsTrue(result.Content.Contains("<p>Out of Stock</p>"));
            Assert.AreEqual(0, result.Start);
        }




    }
}

