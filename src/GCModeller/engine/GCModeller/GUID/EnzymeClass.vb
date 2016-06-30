Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace EngineSystem.ObjectModels.PoolMappings

    Public Structure EnzymeClass
        Implements IAddressHandle
        Implements IReadOnlyId
        Implements IPoolHandle

        Public Property Handle As Integer Implements IAddressHandle.Address

        Public ReadOnly Property ECNumber As String Implements IReadOnlyId.Identity, IPoolHandle.locusId

        Sub New([Class] As String)
            ECNumber = [Class]
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", Handle, Me.ECNumber)
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Sub Dispose(disposing As Boolean)
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

    End Structure

    Public Interface IMappingEdge(Of T As IPoolHandle, TMappingType As Feature.MappingFeature(Of T)) : Inherits IAddressHandle
        ReadOnly Property MappingHandler As T

        Sub set_Nodes(MappingNodes As TMappingType())
    End Interface

    Public Interface IPoolHandle : Inherits IAddressHandle

        ReadOnly Property locusId As String
    End Interface

    Public Structure MotifClass : Implements IPoolHandle

        Public ReadOnly Property MotifId As String Implements IPoolHandle.locusId
            Get
                Throw New NotImplementedException
            End Get
        End Property

        Public Property Handle As Integer Implements IAddressHandle.Address

        Public Sub Dispose() Implements IDisposable.Dispose

        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace