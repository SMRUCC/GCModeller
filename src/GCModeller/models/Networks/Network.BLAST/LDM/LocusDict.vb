Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace LDM

    Public Class LocusDict

        <XmlAttribute> Public Property genome As String
        <XmlAttribute> Public Property LocusId As String()

        Public Overrides Function ToString() As String
            Return genome
        End Function

        Public Shared Function CreateDictionary(lst As IEnumerable(Of LocusDict)) As Dictionary(Of String, String)
            Dim LQuery As Dictionary(Of String, String) =
                (From x As LocusDict In lst.AsParallel
                 Let lstLocus = x.LocusId.Distinct.ToArray
                 Select (From sId As String
                         In lstLocus
                         Select sId, x.genome).ToArray).MatrixAsIterator _
                             .ToDictionary(Function(x) x.sId,
                                           Function(x) x.genome)
            Return LQuery
        End Function
    End Class
End Namespace