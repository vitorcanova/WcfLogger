# WcfLogger
Event base WCF log to hookup straight to the config file.

You just need to add to your client config

	<system.serviceModel>
		<extensions>
			<behaviorExtensions>
				<add name="WcfMessageLogger" type="WcfLogger.WcfMessageLoggerExtension, WcfLogger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
			</behaviorExtensions>
		</extensions>
		<behaviors>
			<serviceBehaviors>
				<behavior name="">
					<serviceMetadata httpGetEnabled="true" />
					<serviceDebug includeExceptionDetailInFaults="false" />
					<WcfMessageLogger />
				</behavior>
			</serviceBehaviors>
		</behaviors>
		<serviceHostingEnvironment multipleSiteBindingsEnabled="true" />
	</system.serviceModel>

So in your Service class you can do like this:

		static Service1()
		{
			WcfLogger.WcfMessageLogger.MessageLogAvailable += LogDispatcher_MessageLogAvailable;
		}

		static void LogDispatcher_MessageLogAvailable(object sender, WcfLogger.LogEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(e.Message);
		}
		
In this case every message (even exception) will be logged to the Output windows in Visual Studio. But nothing stop you to send an email or save to database.
