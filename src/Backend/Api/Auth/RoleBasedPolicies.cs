namespace Api.Auth;

internal static class RoleBasedPolicies
{
    public const string Admin = "Administrator";
    public const string Hr = "HumanResources";
    public const string DecisionMaker = "DecisionMaker";
    public const string AdminAndHr = "AdministratorAndHumanResources";
    public const string AdminAndDecisionMaker = "AdministratorAndDecisionMaker";
    public const string Authenticated = "Authenticated";
}