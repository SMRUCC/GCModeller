Imports LANS.SystemsBiology.Assembly
Imports LANS.SystemsBiology.Assembly.NCBI.CDD
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ProteinModel

    Public Class DomainModel
        Implements sIdEnumerable, IKeyValuePairObject(Of String, Location)

        Public Property DomainId As String Implements sIdEnumerable.Identifier, IKeyValuePairObject(Of String, Location).Identifier
        Public Property Location As Location Implements IKeyValuePairObject(Of String, Location).Value

        Sub New(DomainId As String, Location As Location)
            Me.DomainId = DomainId
            Me.Location = Location
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", DomainId, Location.ToString)
        End Function
    End Class
End Namespace