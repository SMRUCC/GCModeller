Imports Microsoft.VisualBasic.Serialization.JSON

Namespace CytoscapeGraphView.Cyjs.style

    Public Class Selector

        Public ReadOnly Property Expression As String
        ''' <summary>
        ''' node or edge?
        ''' </summary>
        ''' <returns></returns>
        Public Property Type As String
        Public Property Key As String
        Public Property value As String

        Const regexp$ = "^[a-z]+\[.+?='.+'\]$"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ctor$">
        ''' 字典条件
        ''' ```
        ''' type[key = 'value']
        ''' ```
        ''' </param>
        Sub New(ctor$)
            Expression = ctor

            If ctor.IsPattern(regexp, RegexICMul) Then
                Dim t = ctor.GetTagValue("[", trim:=True)

                ctor = t.Value
                Type = t.Name
                t = ctor.GetTagValue("=", trim:=True)
                Key = t.Name
                value = t.Value
            Else
                ' 所有的对象都符合
                Type = ctor
            End If
        End Sub

        Public Function Test(obj As Dictionary(Of String, String)) As Boolean
            If String.IsNullOrEmpty(Key) Then
                Return True ' 类似于 selector: 'node' ， 即所有的节点都符合条件 
            End If

            If Not obj.ContainsKey(Key) Then
                Return False ' 不存在目标属性，则这个节点当然不符合条件
            End If

            Dim value As String = obj(Key)
            Return value = Me.value
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace