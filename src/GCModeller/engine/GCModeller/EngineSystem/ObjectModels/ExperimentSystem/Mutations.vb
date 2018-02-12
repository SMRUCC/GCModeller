#Region "Microsoft.VisualBasic::4877d478c683a82745b991925abe4dc7, engine\GCModeller\EngineSystem\ObjectModels\ExperimentSystem\Mutations.vb"

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

    '     Class Mutations
    ' 
    '         Properties: SystemLogging
    ' 
    '         Function: ToString, TryParse
    ' 
    '         Sub: ApplyMutation, (+2 Overloads) Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging

Namespace EngineSystem.ObjectModels.ExperimentSystem

    ''' <summary>
    ''' -gene_mutations "gene|factor; gene|factor"
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Mutations : Inherits SubSystem.SystemObject
        Implements IDisposable

        ''' <summary>
        ''' {key:=UniqueId, value:=Factor/BasalValue}()
        ''' </summary>
        ''' <remarks></remarks>
        Dim MutationList As KeyValuePair(Of String, Double)()

        Public Sub ApplyMutation(Cell As SubSystem.CellSystem)
            Dim ExpressionRegulationNetwork = Cell.ExpressionRegulationNetwork
            For Each item In MutationList
                Call ExpressionRegulationNetwork.SetupMutation(item.Key, item.Value)
                '假如是缺失突变的话，则必须要将相对应的BasalExpression进行屏蔽
                If item.Value = 0.0R Then
                    Dim Id As String = item.Key & "-transcript"
                    Dim LQuery = (From BasalExpression In Cell.ExpressionRegulationNetwork.BasalExpression.BasalExpressionFluxes Where String.Equals(Id, BasalExpression.Product.Identifier, StringComparison.OrdinalIgnoreCase) Select BasalExpression).ToArray
                    For i As Integer = 0 To LQuery.Count - 1
                        LQuery(i).DeletedMutation = True
                    Next
                End If
            Next
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ARGV">GeneId1|factor1; GeneId2|factor2; ...</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function TryParse(ARGV As String) As Mutations
            Dim Tokens As String() = Strings.Split(ARGV, "; ")
            Dim LQuery = (From strData As String In Tokens Let strTokens As String() = strData.Split(CChar("|")) Select New KeyValuePair(Of String, Double)(strTokens(0), Val(strTokens.Last))).ToArray
            Return New Mutations With {.MutationList = LQuery}
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("There is {0} gene was mutation.", MutationList.Count)
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  释放托管状态(托管对象)。
                End If

                ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO:  将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO:  仅当上面的 Dispose( disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose( disposing As Boolean)中。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' Visual Basic 添加此代码是为了正确实现可处置模式。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

        Protected Friend Overrides ReadOnly Property SystemLogging As LogFile
            Get
                Return Nothing
            End Get
        End Property
    End Class
End Namespace
