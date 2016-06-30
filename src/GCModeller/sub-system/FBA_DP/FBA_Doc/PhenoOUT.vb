Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace Models.rFBA

    ''' <summary>
    ''' 基本类型之中的flux是野生型的数据
    ''' </summary>
    Public Class PhenoOUT : Inherits FBA_OUTPUT.TabularOUT
        Implements IDynamicMeta(Of Double)
        Implements IPhenoOUT

        Dim _props As Dictionary(Of String, Double)

        <Meta(GetType(Double))>
        Public Property Properties As Dictionary(Of String, Double) Implements IDynamicMeta(Of Double).Properties
            Get
                If _props Is Nothing Then
                    _props = New Dictionary(Of String, Double)
                End If
                Return _props
            End Get
            Set(value As Dictionary(Of String, Double))
                _props = value
            End Set
        End Property
    End Class

    Public Interface IPhenoOUT : Inherits sIdEnumerable, IDynamicMeta(Of Double)
    End Interface
End Namespace