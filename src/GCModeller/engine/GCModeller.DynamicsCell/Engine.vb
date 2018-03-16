#Region "Microsoft.VisualBasic::28b5a98ac85dd53aac802b6c0aab1737, engine\GCModeller.DynamicsCell\Engine.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Class Engine
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: __innerNET
    ' 
    '     Sub: __tickData, (+2 Overloads) Dispose, RunModel
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Canvas
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.Scripting
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
            Let tokens As List(Of Token(Of ExpressionTokens)) =
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
    Private Shared Function __innerNET(x As String, tokens As List(Of Token(Of ExpressionTokens)), nodes As Dictionary(Of Node)) As Edge()
        Dim xlst$() = LinqAPI.Exec(Of String) _
 _
            () <= From t As Token(Of ExpressionTokens)
                  In tokens
                  Where t.Type = ExpressionTokens.UNDEFINE
                  Select t.Value
                  Distinct

        Return LinqAPI.Exec(Of Edge) <= From t As String
                                        In xlst
                                        Where nodes & t  ' 方程表达式里面有些是常数来的，故而会在nodes里面不存在
                                        Select New Edge With {
                                            .U = nodes <= t,
                                            .V = nodes <= x,
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
