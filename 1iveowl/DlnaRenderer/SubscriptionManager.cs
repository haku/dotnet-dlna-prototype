namespace _1iveowl;

public class SubscriptionManager
{
    private List<Subscription> Subscriptions { get; } = [];

    public Subscription Add(string callbackUrl, int timeoutSeconds)
    {
        var sub = new Subscription
        {
            Sid = $"uuid:{Guid.NewGuid().ToString()}",
            CallbackUrl = callbackUrl,
            ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(timeoutSeconds),
        };

        Subscriptions.Add(sub);
        Console.WriteLine($"new subscription: {sub.Sid} {sub.ExpiresAt} {sub.CallbackUrl}");
        return sub;
    }

    public bool Renew(string sid, int timeoutSeconds)
    {
        var sub = Subscriptions.Find(s => s.Sid == sid);
        if (sub == null) return false;

        sub.Renew(timeoutSeconds);
        Console.WriteLine($"renew subscription: {sub.Sid} {sub.ExpiresAt} {sub.CallbackUrl}");
        return true;
    }

    public bool Remove(string sid)
    {
        return Subscriptions.RemoveAll(s => s.Sid == sid) > 0;
    }

    public void SendNotification()
    {
        Subscriptions.RemoveAll(s => s.IsExpired());

        // TODO Send
    }

}

public class Subscription
{
    public string Sid { get; init; }
    public string CallbackUrl { get; init; }
    public DateTimeOffset ExpiresAt { get; set; }

    public bool IsExpired()
    {
        return DateTimeOffset.UtcNow >= ExpiresAt;
    }

    public void Renew(int timeoutSeconds)
    {
        ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(timeoutSeconds);
    }
}