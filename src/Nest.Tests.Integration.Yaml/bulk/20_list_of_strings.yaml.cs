using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using NUnit.Framework;


namespace Nest.Tests.Integration.Yaml.Bulk
{
	public partial class Bulk20ListOfStringsYaml20Tests
	{
		
		public class ListOfStrings20Tests
		{
			private readonly RawElasticClient _client;
			private object _body;
		
			public ListOfStrings20Tests()
			{
				var uri = new Uri("http:localhost:9200");
				var settings = new ConnectionSettings(uri, "nest-default-index");
				_client = new RawElasticClient(settings);
			}

			[Test]
			public void ListOfStringsTests()
			{

				//do bulk 
				_body = @"""{\""index\"": {\""_index\"": \""test_index\"", \""_type\"": \""test_type\"", \""_id\"": \""test_id\""}}""
""{\""f1\"": \""v1\"", \""f2\"": 42}""
""{\""index\"": {\""_index\"": \""test_index\"", \""_type\"": \""test_type\"", \""_id\"": \""test_id2\""}}""
""{\""f1\"": \""v2\"", \""f2\"": 47}""";
				this._client.BulkPost(_body, nv=>nv);

				//do count 
				
				this._client.CountGet("test_index", nv=>nv);
			}
		}
	}
}