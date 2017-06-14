using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cassandra;

namespace SampleForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Task.Run(() =>
            {
                //If ran in same task it Timeout
                SimpleConnectTest();
            });
        }
        
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
    }
}
