Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace EngineSystem.Services.DataAcquisition.Services

    Public Interface IDataAdapter
        Function DataSource() As DataSource()
        Function DefHandles() As HandleF()
        ReadOnly Property TableName As String
    End Interface

    Public Interface IDataSource
        ReadOnly Property DataSource As DataSource()
        Function Get_DataSerializerHandles() As HandleF()
    End Interface

    Public Interface IDataSourceEntity
        ReadOnly Property DataSource As DataSource
        ReadOnly Property SerialsHandle As HandleF
    End Interface

    Public Structure DataSource : Implements IAddressHandle

        Public Property Address As Integer Implements IAddressHandle.Address
        Public Property value As Double

        Sub New(raw As KeyValuePair(Of Long, Double))
            Address = raw.Key
            value = raw.Value
        End Sub

        Sub New(i As Long, value As Double)
            Me.Address = i
            Me.value = value
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
        End Sub
    End Structure
End Namespace