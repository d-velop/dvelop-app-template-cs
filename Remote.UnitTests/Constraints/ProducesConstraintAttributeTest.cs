using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Dvelop.Remote.Constraints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Remote.UnitTests.Constraints
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ProducesConstraintAttributeTest
    {
        
        [TestMethod]
        public void Test_IsValid_AcceptedHeader()
        {
            var constraintAttribute = new ProducesConstraintAttribute("application/json");
            var actionDescriptor = new ActionDescriptor
            {
                ActionConstraints = new List<IActionConstraintMetadata>
                {
                    constraintAttribute
                }
            };
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Headers["Accept"] = "application/json";
            var isValid = constraintAttribute.IsValidForRequest(new RouteContext(defaultHttpContext),actionDescriptor );
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Test_IsValid_EmptyValue()
        {
            var constraintAttribute = new ProducesConstraintAttribute("application/json");
            var actionDescriptor = new ActionDescriptor
            {
                ActionConstraints = new List<IActionConstraintMetadata>
                {
                    constraintAttribute
                }
            };
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Headers["Accept"] = "";
            var isValid = constraintAttribute.IsValidForRequest(new RouteContext(defaultHttpContext),actionDescriptor );
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void Test_IsValid_DifferentAccept()
        {
            var constraintAttribute = new ProducesConstraintAttribute("application/json");
            var actionDescriptor = new ActionDescriptor
            {
                ActionConstraints = new List<IActionConstraintMetadata>
                {
                    constraintAttribute
                }
            };
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Headers["Accept"] = "text/html";
            var isValid = constraintAttribute.IsValidForRequest(new RouteContext(defaultHttpContext),actionDescriptor );
            Assert.IsFalse(isValid);
        }
        
        [TestMethod]
        public void Test_IsValid_NoHeader()
        {
            var constraintAttribute = new ProducesConstraintAttribute("application/json");
            var actionDescriptor = new ActionDescriptor
            {
                ActionConstraints = new List<IActionConstraintMetadata>
                {
                    constraintAttribute
                }
            };
            var isValid = constraintAttribute.IsValidForRequest(new RouteContext(new DefaultHttpContext()),actionDescriptor );
            Assert.IsFalse(isValid);
        }
        
        [TestMethod]
        public void Test_IsValid_MultiHeader()
        {
            var constraintAttribute = new ProducesConstraintAttribute("application/json");
            var actionDescriptor = new ActionDescriptor
            {
                ActionConstraints = new List<IActionConstraintMetadata>
                {
                    constraintAttribute
                }
            };
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Headers["Accept"] = "text/html, application/json";
            var isValid = constraintAttribute.IsValidForRequest(new RouteContext(defaultHttpContext),actionDescriptor );
            Assert.IsTrue(isValid);
        }
    }
}