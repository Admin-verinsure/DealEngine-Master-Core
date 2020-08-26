using DealEngine.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DealEngine.Services.Impl
{
    public class SerializerationService : ISerializerationService
    {
        public SerializerationService()
        { }

        public async Task<object?> GetDeserializedObject(Type type, IFormCollection collection)
        {
            Dictionary<object, string> model = new Dictionary<object, string>();
            object obj = null;
            try
            {
                foreach (var Key in collection.Keys)
                {
                    var value = Key.Split(".").ToList().LastOrDefault();
                    var Field = type.GetProperty(value);
                    if (Field != null)
                    {
                        var fieldType = Field.PropertyType;
                        if (
                            (fieldType == typeof(string)) ||
                            (fieldType == typeof(int)) ||
                            (fieldType == typeof(decimal)) ||
                            (fieldType == typeof(bool)) ||
                            (fieldType == typeof(DateTime?)) ||
                            (fieldType == typeof(DateTime))
                            )
                        {
                            if (model.ContainsKey(value))
                            {
                                model[value] = collection[Key].ToString();
                            }
                            else
                            {
                                model.Add(value, collection[Key].ToString());
                            }
                        }
                    }
                }

                var JsonString = await GetSerializedObject(model);
                obj = JsonConvert.DeserializeObject(JsonString, type,
                    new JsonSerializerSettings()
                    {
                        MaxDepth = 1,
                        ObjectCreationHandling = ObjectCreationHandling.Auto,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        FloatFormatHandling = FloatFormatHandling.DefaultValue,
                        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
                    }); ;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return obj;
        }


        public async Task<string> GetSerializedObject(object model)
        {
            try
            {
                return JsonConvert.SerializeObject(model,
                    new JsonSerializerSettings()
                    {
                        MaxDepth = 2,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                        FloatFormatHandling = FloatFormatHandling.DefaultValue,
                        DateParseHandling = DateParseHandling.DateTime
                    });
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}


