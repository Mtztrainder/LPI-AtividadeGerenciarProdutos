using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtividadeGerenciarProdutos.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        public CustomControllerBase() {
            Messages = new List<Message>();
        }

        public enum TypeMessage 
        {
            Success = 1,
            InvalidField = 2,
            Created = 3,
            Error = 4,
            NotFound = 5,
            BadRequest = 6,
            NotAcceptable = 7
        }

        public struct Message
        {
            public string Text { get; set; }
            public TypeMessage Type { get; set; }
            public string TypeText { get; set; }
        }


        public List<Message> Messages { get; set; }

        public void AddSuccessMessage(string text)
        {
            Messages.Add(new Message (){
                Text = text, 
                Type = TypeMessage.Success,
                TypeText = TypeMessage.Success.ToString()
            });
        }

        public void AddInvalidMessage(string text) 
        {
            Messages.Add(new Message (){ 
                Text = text, 
                Type = TypeMessage.InvalidField,
                TypeText = TypeMessage.InvalidField.ToString()
            });
        }

        public void AddErrorMessage(string text)
        {
            Messages.Add(new Message()
            {
                Text = text,
                Type = TypeMessage.Error,
                TypeText = TypeMessage.Error.ToString()
            });
        }

        public void AddNotFoundMessage(string text)
        {
            Messages.Add(new Message()
            {
                Text = text,
                Type = TypeMessage.NotFound,
                TypeText = TypeMessage.NotFound.ToString()
            });
        }

        public void AddNotAcceptableMessage(string text)
        {
            Messages.Add(new Message()
            {
                Text = text,
                Type = TypeMessage.NotAcceptable,
                TypeText = TypeMessage.NotAcceptable.ToString()
            });
        }

        public IActionResult CustomResponse(System.Net.HttpStatusCode statusCode,
            bool success, object data = null)
        {
            var response = new
            {
                success = success,
                messages = Messages,
                data = data
            };

            return StatusCode((int)statusCode, response);
        }
    }
}
