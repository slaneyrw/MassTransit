﻿// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit
{
	using System;
	using System.Collections.Generic;
	using Exceptions;

	public class ObjectConsumerFactory<TConsumer> :
		IConsumerFactory<TConsumer>
		where TConsumer : class
	{
		readonly Func<Type, object> _objectFactory;

		public ObjectConsumerFactory(Func<Type, object> objectFactory)
		{
			_objectFactory = objectFactory;
		}

		public IEnumerable<Action<TMessage>> GetConsumer<TMessage>(Func<TConsumer, Action<TMessage>> callback)
		{
			var consumer = (TConsumer) _objectFactory(typeof (TConsumer));
			if (consumer == null)
				throw new ConfigurationException(string.Format("Unable to resolve type '{0}' from container: ", typeof (TConsumer)));

			try
			{
				Action<TMessage> result = callback(consumer);
				if (result == null)
					yield break;

				yield return result;
			}
			finally
			{
				var disposable = consumer as IDisposable;
				if (disposable != null)
					disposable.Dispose();
			}
		}
	}
}