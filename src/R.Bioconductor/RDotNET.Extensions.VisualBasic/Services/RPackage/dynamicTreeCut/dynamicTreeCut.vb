Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder

Namespace dynamicTreeCut

    Public Module Func

        ''' <summary>
        ''' Passes all its arguments unchaged to the standard print function; after the execution of print it flushes the console, if possible.
        ''' </summary>
        ''' <param name="args">Arguments to be passed to the standard print function.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Passes all its arguments unchaged to the standard print function; after the execution of print it flushes the console, if possible.
        ''' </remarks>
        Public Function printFlush(ParamArray args As Object()) As String
            Dim x As String() = args.ToArray(Function(obj) Scripting.ToString(obj))
            Dim xs As String = String.Join(", ", x)
            Return $"printFlush({xs})"
        End Function
    End Module
End Namespace
