
using MiniHttpServerEgorFramework.Core;
using MiniHttpServerEgorFramework.Core.Attributes;
using MiniHttpServerEgorFramework.Core.HttpResponse;
using MyORMLibrary;


namespace MiniHttpServerEgor.Endpoints
{
    public class UserEndpoint : EndpointBase
    {
        private string _connectionString = "Host=localhost;Port=5432;Database=users_db;Username=egors;Password=197911";

        [HttpGet("users")]
        public IHttpResult GetUsers()
        {
            ORMContext context = new ORMContext(_connectionString);

            var users = context.ReadByAll();

            return Json(users); 
        }

        [HttpPost("users")]
        public IHttpResult PostUser(User user)
        {
            ORMContext context = new ORMContext(_connectionString);

            var createdUser = context.Create(user);

            return Json(createdUser); //return ...
        }

        [HttpDelete("users")]
        public IHttpResult DeleteByAgeUser(int age)
        {
            ORMContext context = new ORMContext(_connectionString);

            context.DeleteByAge(age);

            return null; 
        }

        [HttpPut("users")]
        public IHttpResult UpdatePassword(User user, string newPassword)
        {
            ORMContext context = new ORMContext(_connectionString);

            context.UpdatePassword(user, newPassword);

            return null; 
        }
    }
}
