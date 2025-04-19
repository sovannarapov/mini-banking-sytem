using System.Reflection;
using Application.Abstractions.Messaging;
using Domain.Accounts;
using Infrastructure.Database;
using Web.Api;

namespace Tests;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(Account).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(ICommand).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
}
