Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DataVisualization.Network
Imports Microsoft.VisualBasic.DataVisualization.Network.Canvas
Imports Microsoft.VisualBasic.DataVisualization.Network.Graph
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Mathematical
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports SMRUCC.genomics.Analysis.SSystem
Imports SMRUCC.genomics.Analysis.SSystem.Kernel
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels

Public Class Engine : Implements IDisposable

    Dim ssys As New Value(Of Kernel)
    Dim canvas As Canvas
    Dim nodeTbl As Dictionary(Of Node)

    Sub New(canvas As Canvas)
        Me.canvas = canvas
    End Sub

    Public Sub RunModel(model As Script.Model)

        If Not ssys.IsNothing Then
            Call CType(ssys, Kernel).Break()
        End If

        Dim nodes As Node() = LinqAPI.Exec(Of Node) <=
 _
            From x As var
            In (ssys = New Kernel(model, AddressOf __tickData)).Vars
            Select New Node With {
                .ID = x.UniqueId,
                .Data = New NodeData With {
                    .label = x.Title,
                    .mass = CSng(x.Value),
                    .Color = New SolidBrush(Color.Black)
                }
            }

        Dim nodeTbl As Dictionary(Of Node) =
            nodes.ToDictionary
        Dim net As Edge() = LinqAPI.Exec(Of Edge) <=
 _
            From x As Script.SEquation
            In model.sEquations
            Where nodeTbl & x.x
            Let tokens As List(Of Token(Of Tokens)) =
                ExpressionParser.GetTokens(x.Expression)
            Select __innerNET(x.x, tokens, nodeTbl)

        Me.canvas.DynamicsRadius = True
        Me.nodeTbl = nodeTbl
        Me.canvas.Graph(True) = New NetworkGraph() With {
            .edges = New List(Of Edge)(net),
            .nodes = New List(Of Node)(nodes)
        }

        Call RunTask(AddressOf CType(ssys, Kernel).Run)
    End Sub

    Private Sub __tickData(x As DataSet)
        For Each k As String In x.Properties.Keys
            nodeTbl(Regex.Replace(k, "\(.+\)", "")).Data.radius = CSng(x(k))
        Next

        Thread.Sleep(50)
    End Sub

    ''' <summary>
    ''' 方程左边的变量名称
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="tokens">右边的表达式</param>
    ''' <returns></returns>
    Private Shared Function __innerNET(x As String, tokens As List(Of Token(Of Tokens)), nodes As Dictionary(Of Node)) As Edge()
        Dim xlst As String() = LinqAPI.Exec(Of String) <=
 _
            From t As Token(Of Tokens)
            In tokens
            Where t.Type = Mathematical.Tokens.UNDEFINE
            Select t.TokenValue
            Distinct

        Return LinqAPI.Exec(Of Edge) <= From t As String
                                        In xlst
                                        Where nodes & t  ' 方程表达式里面有些是常数来的，故而会在nodes里面不存在
                                        Select New Edge With {
                                            .Source = nodes <= t,
                                            .Target = nodes <= x,
                                            .ID = $"{t}->{x}"
                                        }
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                Call CType(ssys, Kernel).Break()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
