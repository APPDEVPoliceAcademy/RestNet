﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;
using RestNet.Models;
using Swashbuckle.Swagger;


namespace RestNet.App_Start
{
    public class AuthTokenOperation : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            swaggerDoc.paths.Add("/token", new PathItem
            {
                post = new Operation
                {
                    tags = new List<string> { "Auth" },
                    consumes = new List<string>
                    {
                        "application/x-www-form-urlencoded"
                    },
                    parameters = new List<Parameter> {
                        new Parameter
                        {
                            type = "string",
                            name = "grant_type",
                            required = true,
                            @in = "formData"
                        },
                        new Parameter
                        {
                            type = "string",
                            name = "username",
                            required = false,
                            @in = "formData"
                        },
                        new Parameter
                        {
                            type = "string",
                            name = "password",
                            required = false,
                            @in = "formData"
                        }
                    },
                    responses = new Dictionary<string, Response>()
                    {
                        {"200" , new Response() {schema = schemaRegistry.GetOrRegister(typeof(TokenInfo))}}
                    }
                   
                }
            });
        }
    }
}