using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text;

namespace HL7FuseWebApi
{
    public class RawStringInputFormatter : TextInputFormatter
    {
        public RawStringInputFormatter()
        {
            SupportedMediaTypes.Add("text/plain");
            SupportedMediaTypes.Add("application/hl7-v2");
            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanReadType(Type type) => type == typeof(string);

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            using var reader = new StreamReader(context.HttpContext.Request.Body, encoding);
            var content = await reader.ReadToEndAsync();
            return await InputFormatterResult.SuccessAsync(content);
        }
    }
}