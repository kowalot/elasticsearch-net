﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Elasticsearch.Net.Connection;
using Nest.Resolvers.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Nest.Resolvers;

namespace Nest
{
	public partial class ElasticClient : Nest.IElasticClient
	{
		protected readonly IConnectionSettingsValues _connectionSettings;

		internal RawDispatch RawDispatch { get; set; }

		public IConnection Connection { get; protected set; }
		public INestSerializer Serializer { get; protected set; }
		public IElasticsearchClient Raw { get; protected set; }
		public ElasticInferrer Infer { get; protected set; }

		public ElasticClient(IConnectionSettingsValues settings, IConnection connection = null, INestSerializer serializer = null)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			this._connectionSettings = settings;
			this.Connection = connection ?? new HttpConnection(settings);

			this.Serializer = serializer ?? new NestSerializer(this._connectionSettings);
			var stringifier = new NestStringifier(settings);
			this.Raw = new ElasticsearchClient(
				this._connectionSettings, 
				this.Connection, 
				null, //default transport
				this.Serializer, 
				stringifier
			);
			this.RawDispatch = new RawDispatch(this.Raw);
			this.Infer = new ElasticInferrer(this._connectionSettings);

		}


		private R Dispatch<D, Q, R>(
			Func<D, D> selector
			, Func<ElasticsearchPathInfo<Q>, D, ElasticsearchResponse> dispatch
			, Func<ElasticsearchResponse, D, R> resultSelector = null
			, bool allow404 = false
			)
			where Q : FluentQueryString<Q>, new()
			where D : IPathInfo<Q>, new()
			where R : BaseResponse
		{
			selector.ThrowIfNull("selector");
			var descriptor = selector(new D());
			return Dispatch<D, Q, R>(descriptor, dispatch, resultSelector, allow404);
		}


		private R Dispatch<D, Q, R>(
			D descriptor
			, Func<ElasticsearchPathInfo<Q>, D, ElasticsearchResponse> dispatch
			, Func<ElasticsearchResponse, D, R> resultSelector = null
			, bool allow404 = false
			)
			where Q : FluentQueryString<Q>, new()
			where D : IPathInfo<Q>
			where R : BaseResponse
		{
			var pathInfo = descriptor.ToPathInfo(this._connectionSettings);
			resultSelector = resultSelector ?? ((c, d) => this.Serializer.ToParsedResponse<R>(c, allow404));
			var response = dispatch(pathInfo, descriptor);
			return resultSelector(response, descriptor);
		}

		internal Task<I> DispatchAsync<D, Q, R, I>(
			Func<D, D> selector
			, Func<ElasticsearchPathInfo<Q>, D, Task<ElasticsearchResponse>> dispatch
			, Func<ElasticsearchResponse, D, R> resultSelector = null
			, bool allow404 = false
			)
			where Q : FluentQueryString<Q>, new()
			where D : IPathInfo<Q>, new()
			where R : BaseResponse, I
			where I : IResponse
		{
			selector.ThrowIfNull("selector");
			var descriptor = selector(new D());
			return DispatchAsync<D, Q, R, I>(descriptor, dispatch, resultSelector, allow404);
		}

		private Task<I> DispatchAsync<D, Q, R, I>(
			D descriptor 
			, Func<ElasticsearchPathInfo<Q>, D, Task<ElasticsearchResponse>> dispatch
			, Func<ElasticsearchResponse, D, R> resultSelector = null
			, bool allow404 = false
			) 
			where Q : FluentQueryString<Q>, new()
			where D : IPathInfo<Q>
			where R : BaseResponse, I 
			where I : IResponse
		{
			var pathInfo = descriptor.ToPathInfo(this._connectionSettings);
			resultSelector = resultSelector ?? ((c, d) => this.Serializer.ToParsedResponse<R>(c, allow404));
			return dispatch(pathInfo, descriptor)
				.ContinueWith<I>(r => resultSelector(r.Result, descriptor));
		}


	}
}
