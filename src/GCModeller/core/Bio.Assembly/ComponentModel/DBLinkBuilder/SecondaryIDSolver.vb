Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel.DBLinkBuilder

    ''' <summary>
    ''' 例如chebi和hmdb数据库，都存在有次级编号和主编号，
    ''' 虽然这些编号不一样，但是他们都是同一个对象，则这个模块就是专门解决这种映射问题的
    ''' </summary>
    Public Class SecondaryIDSolver

#Region "这里需要特别的注意一下：都是小写的字符串"
        Dim mainID As Index(Of String)
        Dim secondaryIDs As Dictionary(Of String, String)

        Public ReadOnly Property ALL As String()
            Get
                Return mainID.Objects _
                    .JoinIterates(secondaryIDs.Keys) _
                    .Distinct _
                    .ToArray
            End Get
        End Property
#End Region

        Sub New()
            mainID = New Index(Of String)
            secondaryIDs = New Dictionary(Of String, String)
        End Sub

        Default Public ReadOnly Property SolveIDMapping(id$) As String
            Get
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
            End Get
        End Property

        ''' <summary>
        ''' Add new 2nd to main mapping.(编号都被转换为小写形式了)
        ''' </summary>
        ''' <param name="main$"></param>
        ''' <param name="secondary"></param>
        Public Sub Add(main$, secondary As IEnumerable(Of String))
            main = main.ToLower

            If mainID.IndexOf(main) = -1 Then
                mainID.Add(main)
            End If

            For Each id As String In secondary _
                .SafeQuery _
                .Select(Function(s) s.ToLower)

                If Not secondaryIDs.ContainsKey(id) Then
                    Call secondaryIDs.Add(id, main)
                End If
            Next
        End Sub

        Public Overrides Function ToString() As String
            Return $"Has {mainID.Count} main IDs, ALL {ALL.Length} in total."
        End Function

        Public Delegate Function GetKey(Of T)(o As T) As String
        Public Delegate Function GetAllKeys(Of T)(o As T) As String()

        Public Shared Function Create(Of T)(source As IEnumerable(Of T), mainID As GetKey(Of T), secondaryID As GetAllKeys(Of T)) As SecondaryIDSolver
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