using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;


namespace Elasticsearch.Net.Integration.Yaml.Delete6
{
	public partial class Delete6YamlTests
	{	


		[NCrunch.Framework.ExclusivelyUses("ElasticsearchYamlTests")]
		public class ParentWithRouting1Tests : YamlTestsBase
		{
			[Test]
			public void ParentWithRouting1Test()
			{	

				//do indices.create 
				_body = new {
					mappings= new {
						test= new {
							_parent= new {
								type= "foo"
							}
						}
					},
					settings= new {
						number_of_replicas= "0"
					}
				};
				this.Do(()=> _client.IndicesCreate("test_1", _body));

				//do cluster.health 
				this.Do(()=> _client.ClusterHealth(nv=>nv
					.Add("wait_for_status", @"green")
				));

				//do index 
				_body = new {
					foo= "bar"
				};
				this.Do(()=> _client.Index("test_1", "test", "1", _body, nv=>nv
					.Add("parent", 5)
					.Add("routing", 4)
				));

				//do delete 
				this.Do(()=> _client.Delete("test_1", "test", "1", nv=>nv
					.Add("parent", 5)
					.Add("routing", 1)
				), shouldCatch: @"missing");

				//do delete 
				this.Do(()=> _client.Delete("test_1", "test", "1", nv=>nv
					.Add("parent", 5)
					.Add("routing", 4)
				));

			}
		}
	}
}

