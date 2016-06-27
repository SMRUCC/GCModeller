Imports LANS.SystemsBiology.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic

Namespace Assembly.NCBI.COG

    Public Class COGFunc : Inherits ClassObject
        Implements sIdEnumerable

        Public Property Category As COGCategories
        Public Property COG As String Implements sIdEnumerable.Identifier
        Public Property Func As String
        Public Property locus As String()
        Public ReadOnly Property NumOfLocus As Integer
            Get
                Return locus.Length
            End Get
        End Property

        Private Shared Function __notAssigned() As COGFunc
            Return New COGFunc With {
                .Category = COGCategories.NotAssigned,
                .COG = "-",
                .Func = ""
            }
        End Function

        Public Shared Function GetClass(Of T As ICOGDigest)(source As IEnumerable(Of T), func As [Function]) As COGFunc()
            Dim hash = func.Categories.ToArray(
                Function(x) x.ToArray).MatrixAsIterator _
                        .ToDictionary(Function(x) x.COG.First,
                                      Function(x) New With {
                                        .fun = x,
                                        .count = New List(Of String)})
            Dim locus = source.ToArray(
                Function(x) New With {
                    x.Identifier,
                     .COG = Strings.UCase([Function].__trimCOGs(x.COG))})

            hash.Add("-", New With {.fun = __notAssigned(), .count = New List(Of String)})
            For Each x In locus
                For Each c As Char In x.COG
                    hash(c).count.Add(x.Identifier)
                Next
            Next

            Return hash.Values.ToArray(Function(x) x.fun.InvokeSet(NameOf(COGFunc.locus), x.count.ToArray))
        End Function
    End Class
End Namespace