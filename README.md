# csharp-driver-winform-test

Test project to reproduce issue https://datastax-oss.atlassian.net/browse/CSHARP-579 .

The issue happens when executing this code:

```
public void SimpleConnectTest()
{
    // Specify the minimum trace level you want to see
    Cassandra.Diagnostics.CassandraTraceSwitch.Level = TraceLevel.Verbose;
    // Add a standard .NET trace listener
    Trace.Listeners.Add(new ConsoleTraceListener());
    const string ip = "127.0.0.1";
    const string keyspace = "test";
    var queryOptions = new QueryOptions();
    queryOptions.SetConsistencyLevel(ConsistencyLevel.LocalOne);
    var socketOptions = new SocketOptions();
    socketOptions.SetConnectTimeoutMillis(3000);
    var cluster = Cluster.Builder()
        .AddContactPoints(ip)
        .WithQueryOptions(queryOptions)
        .WithSocketOptions(socketOptions)
        .Build();
    var cassandraSession = cluster.Connect(keyspace);
    var rs = cassandraSession.Execute("select * from table1;");
    foreach (var row in rs)
    {
        Console.WriteLine("{0} {1}", row["id"], row["name"]);
    }
    cluster.Dispose();
}
```

## Workaround
The issue was workarounded when connecting to cassandra using a different Task: 
```
Task.Run(() =>
    {
        //If ran in same task it Timeout
        SimpleConnectTest();
    });
```            
