using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;


namespace Elasticsearch.Net.Integration.Yaml.ClusterPutSettings1
{
	public partial class ClusterPutSettings1YamlTests
	{	


		[NCrunch.Framework.ExclusivelyUses("ElasticsearchYamlTests")]
		public class TestPutSettings1Tests : YamlTestsBase
		{
			[Test]
			public void TestPutSettings1Test()
			{	

				//do cluster.put_settings 
				_body = new {
					transient= new Dictionary<string, object> {
						 { "discovery.zen.minimum_master_nodes",  "1" }
					}
				};
				this.Do(()=> _client.ClusterPutSettings(_body, nv=>nv
					.Add("flat_settings", @"true")
				));

				//match _response.transient: 
				this.IsMatch(_response.transient, new Dictionary<string, object> {
					{ @"discovery.zen.minimum_master_nodes", @"1" }
				});

				//do cluster.get_settings 
				this.Do(()=> _client.ClusterGetSettings(nv=>nv
					.Add("flat_settings", @"true")
				));

				//match _response.transient: 
				this.IsMatch(_response.transient, new Dictionary<string, object> {
					{ @"discovery.zen.minimum_master_nodes", @"1" }
				});

			}
		}
	}
}

