Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.WebCloud.HTTPInternal.Platform

Public Module DebuggerAPI

    ''' <summary>
    ''' Run start the httpd web server.
    ''' </summary>
    ''' <param name="port%">The server port of this httpd web server to listen.</param>
    ''' <param name="wwwroot$">The website html root directory path.</param>
    ''' <param name="threads%">The number of threads of this web server its thread pool.</param>
    ''' <param name="cacheMode">Is this server running in file system cache mode? Not recommended for open.</param>
    ''' <returns></returns>
    <ExportAPI("/start",
               Info:="Run start the httpd web server.",
               Usage:="/start [/port 80 /wwwroot <wwwroot_DIR> /threads -1 /cache]")>
    <Argument("/port", True, CLITypes.Integer,
              AcceptTypes:={GetType(Integer)},
              Description:="The server port of this httpd web server to listen.")>
    <Argument("/wwwroot", True, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(String)},
              Description:="The website html root directory path.")>
    <Argument("/threads", True, CLITypes.Integer,
              AcceptTypes:={GetType(Integer)},
              Description:="The number of threads of this web server its thread pool.")>
    <Argument("/cache", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="Is this server running in file system cache mode? Not recommended for open.")>
    Public Function Start(Optional port% = 80, Optional wwwroot$ = "./wwwroot", Optional threads% = -1, Optional cacheMode As Boolean = False) As Integer
        Return New PlatformEngine(wwwroot, port, True, threads:=threads, cache:=cacheMode).Run
    End Function
End Module
