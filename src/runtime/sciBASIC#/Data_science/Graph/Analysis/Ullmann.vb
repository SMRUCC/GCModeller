Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Analysis

    Public Class Ullmann

        ''' <summary>
        ''' The target graph
        ''' </summary>
        Private ReadOnly T As Integer()()
        ''' <summary>
        ''' the query graph
        ''' </summary>
        Private ReadOnly Q As Integer()()

        Private ReadOnly m0Matrix As Integer()()


        Public Sub New(largeGraphMatrix As Integer()(), searchGraphMatrix As Integer()())
            checkMatrix(largeGraphMatrix, searchGraphMatrix)
            Me.T = largeGraphMatrix
            Me.Q = searchGraphMatrix

            m0Matrix = creatM0Matrix()
        End Sub

        Private Shared Sub checkMatrix(largeGraphMatrix As Integer()(), searchGraphMatrix As Integer()())
            If largeGraphMatrix Is Nothing OrElse searchGraphMatrix Is Nothing Then
                Throw New ArgumentException("参数不为空")
            End If
            If largeGraphMatrix.Length <> largeGraphMatrix(0).Length OrElse searchGraphMatrix.Length <> searchGraphMatrix(0).Length Then
                Throw New ArgumentException("矩阵不合法")
            End If
        End Sub

        Public Overridable Function cal() As IList(Of Integer()())
            Dim res As IList(Of Integer()()) = New List(Of Integer()())()
            Dim m1 = RectangularArray.Matrix(Of Integer)(m0Matrix.Length, m0Matrix(0).Length)
            dfs(m1, res, 0)
            Return res
        End Function

        Public Shared Iterator Function ExplainNodeMapping(ullmann As Integer()(), G As String(), H As String()) As IEnumerable(Of NamedValue(Of String))
            For Each gv As (map As Integer(), gid As String) In ullmann.Zip(join:=G)
                For i As Integer = 0 To H.Length - 1
                    If gv.map(i) > 0 Then
                        Yield New NamedValue(Of String)(gv.gid, H(i))
                    End If
                Next
            Next
        End Function

        Private Sub dfs(m1 As Integer()(), res As IList(Of Integer()()), d As Integer)
            For i = 0 To m1(d).Length - 1
                If m0Matrix(d)(i) = 1 Then
                    m1(d)(i) = 1
                    If d = m1.Length - 1 Then
                        If check(m1) Then
                            res.Add(copy(m1))
                        End If
                    Else
                        dfs(m1, res, d + 1)
                    End If
                    m1(d)(i) = 0
                End If
            Next
        End Sub

        Private Function copy(m1 As Integer()()) As Integer()()

            Dim res = RectangularArray.Matrix(Of Integer)(m1.Length, m1(0).Length)
            For i = 0 To res.Length - 1
                Array.Copy(m1(i), 0, res(i), 0, res(i).Length)
            Next
            Return res
        End Function

        Private Function check(m1 As Integer()()) As Boolean
            Dim mc = getMC(m1)
            For i = 0 To Q.Length - 1
                For j = 0 To Q(i).Length - 1
                    If Q(i)(j) = 1 AndAlso mc(i)(j) <> 1 Then
                        Return False
                    End If
                Next
            Next
            Return True
        End Function

        Private Function getMC(m1 As Integer()()) As Integer()()
            'M'⋅B

            Dim m1b = MatrixUtils.multiplication(m1, T)
            '（M'⋅B)T

            Dim m1bt = MatrixUtils.transpose(m1b)
            'MC=M'（M'⋅B)T
            Return MatrixUtils.multiplication(m1, m1bt)
        End Function

        ''' <summary>
        ''' 创建M0矩阵 M0 矩阵以小图的点为行，大图的点为列
        ''' <para>
        ''' 当大图的j点的度大于小图j点的度，m[i][j]=1,否则m[i][j]=0。
        ''' </para>
        ''' </summary>
        Private Function creatM0Matrix() As Integer()()
            Dim res = RectangularArray.Matrix(Of Integer)(Q.Length, T.Length)
            For i = 0 To res.Length - 1
                For j = 0 To res(i).Length - 1
                    Dim l = getDegree(j, T)
                    Dim s = getDegree(i, Q)
                    res(i)(j) = If(l >= s, 1, 0)
                Next
            Next
            Return res
        End Function

        ''' <summary>
        ''' 获取点的度
        ''' </summary>
        ''' <paramname="i">           点的编号 </param>
        ''' <paramname="graphMatrix"> 图的矩阵 </param>
        Private Function getDegree(i As Integer, graphMatrix As Integer()()) As Integer
            Dim res = 0
            For k = 0 To graphMatrix(i).Length - 1
                res += If(graphMatrix(i)(k) = 1, 1, 0)
            Next
            Return res
        End Function

    End Class

End Namespace
