namespace IdentityServer3.Admin.EntityFramework.Interfaces
{

    // Summary:
    //     Minimal interface for a scope
    //
    // Type parameters:
    //   TKey:
    public interface IScope<out TKey>
    {
        // Summary:
        //     Unique key for the user
        TKey Id { get; }
        //
        // Summary:
        //     Name
        string Name { get; set; }
    }
}