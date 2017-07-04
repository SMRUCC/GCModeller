Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language

Namespace ComponentModel.DBLinkBuilder

    ''' <summary>
    ''' 例如chebi和hmdb数据库，都存在有次级编号和主编号，
    ''' 虽然这些编号不一样，但是他们都是同一个对象，则这个模块就是专门解决这种映射问题的
    ''' </summary>
    Public Class SecondaryIDSolver

        Dim mainID As Index(Of String)
        Dim secondaryIDs As Dictionary(Of String, String)

        Private Sub New()
        End Sub

        Public Function SolveIDMapping(id$) As String
            With id.ToLower
                If mainID.IndexOf(.ref) > -1 Then
                    ' 这个id是主编号，直接返回原来的值
                    Return id
                End If
                If secondaryIDs.ContainsKey(.ref) Then
                    Return secondaryIDs(.ref)
                Else
                    Return Nothing  ' 在数据库之中没有记录，确认一下是否是数据出错了？
                End If
            End With
        End Function

        Public Overrides Function ToString() As String
            Return $"Have {mainID.Count} main IDs"
        End Function

        Public Shared Function Create(Of T)(source As IEnumerable(Of T),
                                            mainID As Func(Of T, String),
                                            secondaryID As Func(Of T, String())) As SecondaryIDSolver

            Dim mainIDs As New List(Of String)
            Dim secondaryIDs As New Dictionary(Of String, String)  ' 2nd -> main

            For Each x As T In source
                Dim accession$ = mainID(x).ToLower
                Dim list2nd$() = secondaryID(x)

                mainIDs.Add(accession)
                list2nd.DoEach(Sub(id)
                                   Call secondaryIDs.Add(id.ToLower, accession)
                               End Sub)
            Next

            Return New SecondaryIDSolver With {
                .mainID = mainIDs.Indexing,
                .secondaryIDs = secondaryIDs
            }
        End Function
    End Class
End Namespace