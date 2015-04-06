using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace WcfLogger
{
	/// <summary>
	/// Evento para logs do WCF.
	/// </summary>
	public class LogEventArgs : EventArgs
	{
		/// <summary>
		/// Construtor que preenche com a mensagem.
		/// </summary>
		/// <param name="message">Mensagem a ser preenchida no argumento do evento.</param>
		/// <param name="isInbound">Indica se é uma mensagem que está sendo recebida. Caso contrário é de saída ou OneWay.</param>
		public LogEventArgs(string message, bool isInbound)
		{
			this.Message = message;
			this.IsInbound = isInbound;
		}

		/// <summary>
		/// Mensagem que se quer logar.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Indica se o log é de uma mensagem de entrada ou de uma resposta.
		/// </summary>
		public bool IsInbound { get; set; }
	}

	/// <summary>
	/// Classe para utilizar em behavior e capturar tráfego de entrada e saída do WCF. Conforme exemplo:
	///<para>	&lt;system.serviceModel&gt;</para>
	///<para>		&lt;extensions&gt;</para>
	///<para>			&lt;behaviorExtensions&gt;</para>
	///<para>				&lt;add name=&quot;WcfMessageLogger&quot; type=&quot;WcfLogger.WcfMessageLoggerExtension, WcfLogger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&quot; /&gt;</para>
	///<para>			&lt;/behaviorExtensions&gt;</para>
	///<para>		&lt;/extensions&gt;</para>
	///<para>		&lt;behaviors&gt;</para>
	///<para>			&lt;serviceBehaviors&gt;</para>
	///<para>				&lt;behavior name=&quot;&quot;&gt;</para>
	///<para>					&lt;serviceMetadata httpGetEnabled=&quot;true&quot; /&gt;</para>
	///<para>					&lt;serviceDebug includeExceptionDetailInFaults=&quot;false&quot; /&gt;</para>
	///<para>					&lt;WcfMessageLogger /&gt;</para>
	///<para>				&lt;/behavior&gt;</para>
	///<para>			&lt;/serviceBehaviors&gt;</para>
	///<para>		&lt;/behaviors&gt;</para>
	///<para>		&lt;serviceHostingEnvironment multipleSiteBindingsEnabled=&quot;true&quot; /&gt;</para>
	///<para>	&lt;/system.serviceModel&gt;</para>
	/// </summary>
	public class WcfMessageLogger : IDispatchMessageInspector, IServiceBehavior
	{
		/// <summary>
		/// Evento disparado quando uma mensagem é enviada ou recebida.
		/// </summary>
		public static event EventHandler<LogEventArgs> MessageLogAvailable;

		/// <summary>
		/// Verifica e dispara o evento de log.
		/// </summary>
		/// <param name="message">mensagem que se quer logar.</param>
		///<param name="isInbound">Indica se é uma mensagem que está sendo recebida. Caso contrário é de saída ou OneWay.</param>
		private void onMessageLogAvailable(string message, bool isInbound)
		{
			var handler = WcfMessageLogger.MessageLogAvailable;

			if (handler != null)
			{
				WcfMessageLogger.MessageLogAvailable(this, new LogEventArgs(message, isInbound));
			}
		}

		#region IDispatchMessageInspector

		/// <summary>
		/// Chamado depois da mensagem ter sido recebida mas antes de ir para operação a que se destina.
		/// </summary>
		/// <param name="request">Mensagem da requisição.</param>
		/// <param name="channel">Canal de entrada.</param>
		/// <param name="instanceContext">Instância corrente do serviço.</param>
		/// <returns>Retorna o objeto para correlacionar o estado. O objeto será passado de volta ao método BeforeSendReply.</returns>
		public object AfterReceiveRequest(ref Message request, IClientChannel channel,
			InstanceContext instanceContext)
		{
			this.onMessageLogAvailable(request.ToString(), true);
			return null;
		}

		/// <summary>
		/// Chamando após a operação retornar mas antes da mensagem ser enviada.
		/// </summary>
		/// <param name="reply">Mensagem de resposta.</param>
		/// <param name="correlationState">Mensagem de correlação retornado pelo método AfterReceiveRequest.</param>
		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			this.onMessageLogAvailable(reply.ToString(), false);
		}

		#endregion

		#region IServiceBehavior

		/// <summary>
		/// Adiciona este behavior de rota para o específico host de serviço.
		/// </summary>
		/// <param name="serviceDescription">Descritor do serviço.</param>
		/// <param name="serviceHostBase">Host de serviço ao qual será adicionado.</param>
		public void ApplyDispatchBehavior(ServiceDescription serviceDescription,
			ServiceHostBase serviceHostBase)
		{
			foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
			{
				foreach (var endpoint in dispatcher.Endpoints)
				{
					endpoint.DispatchRuntime.MessageInspectors.Add(new WcfMessageLogger());
				}
			}
		}

		/// <summary>
		/// Implementação do método da interface. Não é utilizado.
		/// </summary>
		/// <param name="serviceDescription">Não é utilizado.</param>
		/// <param name="serviceHostBase">Não é utilizado.</param>
		/// <param name="endpoints">Não é utilizado.</param>
		/// <param name="bindingParameters">Não é utilizado.</param>
		public void AddBindingParameters(ServiceDescription serviceDescription,
			ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
			BindingParameterCollection bindingParameters)
		{
		}

		/// <summary>
		/// Implementação do método da interface. Não é utilizado.
		/// </summary>
		/// <param name="serviceDescription">Não é utilizado.</param>
		/// <param name="serviceHostBase">Não é utilizado.</param>
		public void Validate(ServiceDescription serviceDescription,
			ServiceHostBase serviceHostBase)
		{
		}

		#endregion
	}
}
