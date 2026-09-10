Namespace Core

    ''' <summary>
    ''' an application level module contract for a dynamically loaded web app.
    ''' </summary>
    ''' <remarks>
    ''' The concrete implementation usually lives in a separate controller class
    ''' library that is loaded through reflection by a generic host (for example
    ''' the ``Fluteway /run --app &lt;app.dll&gt;`` command). The host instantiates
    ''' the module by its parameter-less constructor and then invokes
    ''' <see cref="Mount(HttpRouter, IReadOnlyDictionary(Of String, String))"/>
    ''' so that the module can register its http routes (via
    ''' <see cref="HttpRouter.RegisterController(Object)"/> and
    ''' <see cref="HttpRouter.Register(String, String, HttpSocket.AppHandler)"/>)
    ''' and read the host supplied configuration.
    ''' </remarks>
    Public Interface IHttpAppModule

        ''' <summary>
        ''' mount this module into the given <paramref name="router"/> using the
        ''' host supplied <paramref name="config"/> values.
        ''' </summary>
        ''' <param name="router">the router the controller routes are registered into.</param>
        ''' <param name="config">
        ''' the host supplied configuration values, for example the physical
        ''' ``wwwroot`` folder or the data storage directory. It is never
        ''' <c>Nothing</c>, though individual keys may be missing.
        ''' </param>
        Sub Mount(router As HttpRouter, config As IReadOnlyDictionary(Of String, String))

    End Interface
End Namespace
