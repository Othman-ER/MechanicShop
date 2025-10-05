using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Infrastructure.Identity;

namespace MechanicShop.tests.Common.Security;

public class TestCurrentUser : IUser
{
    private ApplicationUser? _currentUser;

    public void Returns(ApplicationUser currentUser) => _currentUser = currentUser;

    public string? Id => _currentUser!.Id ?? UserFactory.CreateUser().Id;
}
