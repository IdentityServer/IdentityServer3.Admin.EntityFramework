namespace IdentityServer3.Admin.EntityFramework.Interfaces
{
    // Summary:
    //     Minimal interface for a client
    //
    // Type parameters:
    //   TKey:
    public interface IClient<out TKey>
    {
        // Summary:
        //     Unique key for the client
        TKey Id { get; }
        //
        // Summary:
        //     Unique ClientId
        string ClientId { get; set; }

        //
        // Summary:
        //     Client name
        string ClientName { get; set; }
    }
}