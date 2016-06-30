Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language

Namespace DataStorage.FileModel

    Public Class ObjectHandle : Inherits ClassObject
        Implements IKeyValuePairObject(Of String, Integer)
        Implements IAddressHandle
        Implements sIdEnumerable

        Public Property Identifier As String Implements sIdEnumerable.Identifier, IKeyValuePairObject(Of String, Integer).Identifier
        Public Property Handle As Integer Implements IAddressHandle.Address, IKeyValuePairObject(Of String, Integer).Value

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace