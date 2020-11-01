Imports System
Imports System.Runtime.CompilerServices

Namespace Utilities
    ''' <summary>
    ''' Advanced, less usual extension methods for the R.NET REngine
    ''' </summary>
    Public Module REngineExtensionsAdvanced
        ''' <summary>
        ''' Checks the equality in native memory of a pointer against a pointer to the R 'NULL' value
        ''' </summary>
        ''' <param name="engine">R.NET Rengine</param>
        ''' <param name="pointer">Pointer to test</param>
        ''' <returns>True if the pointer and pointer to R NULL are equal</returns>
        <Extension()>
        Public Function EqualsRNilValue(ByVal engine As REngine, ByVal pointer As IntPtr) As Boolean
            Return engine.NilValue.DangerousGetHandle() = pointer
        End Function

        ''' <summary>
        ''' Checks the equality in native memory of a pointer against a pointer to the R 'R_UnboundValue',
        ''' i.e. whether a symbol exists (i.e. functional equivalent to "exists('varName')" in R)
        ''' </summary>
        ''' <param name="engine">R.NET Rengine</param>
        ''' <param name="pointer">Pointer to test</param>
        ''' <returns>True if the pointer is not bound to a value</returns>
        <Extension()>
        Public Function CheckUnbound(ByVal engine As REngine, ByVal pointer As IntPtr) As Boolean
            Return engine.UnboundValue.DangerousGetHandle() = pointer
        End Function
    End Module
End Namespace
