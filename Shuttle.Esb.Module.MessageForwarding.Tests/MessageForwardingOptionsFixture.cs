using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Shuttle.Esb.Module.MessageForwarding.Tests
{
	[TestFixture]
	public class MessageForwardingOptionsFixture
	{
		protected MessageForwardingOptions GetOptions()
		{
			var result = new MessageForwardingOptions();

			new ConfigurationBuilder()
				.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @".\appsettings.json")).Build()
				.GetRequiredSection($"{MessageForwardingOptions.SectionName}").Bind(result);

			return result;
		}

		[Test]
		public void Should_be_able_to_load_the_configuration()
		{
			var options = GetOptions();

			Assert.IsNotNull(options);
			Assert.AreEqual(2, options.ForwardingRoutes.Count);

			foreach (var messageRouteOptions in options.ForwardingRoutes)
			{
				Console.WriteLine(messageRouteOptions.Uri);

				foreach (var specification in messageRouteOptions.Specifications)
				{
					Console.WriteLine($@"-> {specification.Name} - {specification.Value}");
				}

				Console.WriteLine();
			}
		}
	}
}
