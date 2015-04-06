using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;

namespace WcfLogger
{
	/// <summary>
	/// Extensão para o config suportar o behavior WcfMessageLogger.
	/// </summary>
	public class WcfMessageLoggerExtension : BehaviorExtensionElement
	{
		/// <summary>
		/// Retorna o behavior novo suportado pelo config.
		/// </summary>
		/// <returns>Novo behavior.</returns>
		protected override object CreateBehavior()
		{
			return new WcfMessageLogger();
		}

		/// <summary>
		/// Tipo do novo behavior.
		/// </summary>
		public override Type BehaviorType
		{
			get
			{
				return typeof(WcfMessageLogger);
			}
		}
	}
}
