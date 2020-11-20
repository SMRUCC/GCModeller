Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object

Module pipHelper

    Public Function getUniprotData(uniprot As Object, env As Environment) As [Variant](Of IEnumerable(Of entry), Message)
        If uniprot Is Nothing Then
            Return DirectCast(New entry() {}, IEnumerable(Of entry))
        End If

        If TypeOf uniprot Is entry() OrElse TypeOf uniprot Is IEnumerable(Of entry) Then
            Return New [Variant](Of IEnumerable(Of entry), Message)(DirectCast(uniprot, IEnumerable(Of entry)))
        ElseIf TypeOf uniprot Is pipeline AndAlso DirectCast(uniprot, pipeline).elementType Like GetType(entry) Then
            Return New [Variant](Of IEnumerable(Of entry), Message)(DirectCast(uniprot, pipeline).populates(Of entry)(env))
        ElseIf TypeOf uniprot Is vector AndAlso DirectCast(uniprot, vector).elementType Like GetType(entry) Then
            Return New [Variant](Of IEnumerable(Of entry), Message)(DirectCast(uniprot, vector).data.AsObjectEnumerator(Of entry))
        Else
            Return Internal.debug.stop($"invalid data source input: {uniprot.GetType.FullName}!", env)
        End If
    End Function
End Module
