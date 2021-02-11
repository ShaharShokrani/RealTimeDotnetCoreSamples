using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealTimeDotnetCoreSamples.Models
{
    public interface IUserService
    {
        Task<ResultHandler<IEnumerable<UserModel>>> GetUsers(UsersRequest request);
    }
}
