#Region "Microsoft.VisualBasic::5dfd58ddd462cc093be41a63dbb0b951, GCModeller\core\Bio.Assembly\Assembly\DOOR\Models\DOOR.vb"

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


    ' Code Statistics:

    '   Total Lines: 184
    '    Code Lines: 107
    ' Comment Lines: 53
    '   Blank Lines: 24
    '     File Size: 6.61 KB


    '     Class DOOR
    ' 
    '         Properties: DOOROperonView, Genes
    ' 
    '         Function: [Select], CreateEmpty, Export, GetEnumerator, GetOperon
    '                   HaveGene, HaveOperon, IEnumerable_GetEnumerator, IsOprPromoter, SameOperon
    '                   (+2 Overloads) Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text

Namespace Assembly.DOOR

    ''' <summary>
    ''' ###### DOOR: Database of prOkaryotic OpeRons.
    ''' 
    ''' Door operon prediction data.(Door操纵子预测数据)
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Public Class DOOR
        Implements IEnumerable(Of Operon)
        Implements ISaveHandle

        ''' <summary>
        ''' 在文件之中，是一个表格的形式来表示整个文件的，这个属性表示文件之中的所有行
        ''' </summary>
        ''' <returns></returns>
        Public Property Genes As OperonGene()
            Get
                Return _genes
            End Get
            Set(value As OperonGene())
                _genes = value
                _geneTable = _genes.ToDictionary(Function(gene) gene.Synonym)
            End Set
        End Property

        ''' <summary>
        ''' 按照操纵子编号进行分组得到的operon模型的集合
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property DOOROperonView As OperonView
            Get
                If view Is Nothing Then
                    view = CreateOperonView
                End If

                Return view
            End Get
        End Property

        Dim view As OperonView
        Dim _genes As OperonGene()
        Dim _geneTable As Dictionary(Of String, OperonGene)

        ''' <summary>
        ''' 查找不到目标基因对象则会返回空值
        ''' </summary>
        ''' <param name="locusId">基因编号</param>
        ''' <returns></returns>
        Default Public ReadOnly Property GetGene(locusId As String) As OperonGene
            Get
                If _geneTable.ContainsKey(locusId) Then
                    Return _geneTable(locusId)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' 查看目标基因是否是其所处的操纵子的第一个基因
        ''' </summary>
        ''' <param name="locus">目标基因的基因编号</param>
        ''' <returns></returns>
        Public Function IsOprPromoter(locus As String) As Boolean
            Dim g As OperonGene = _geneTable(locus)
            Dim opr As String = g.OperonID
            Dim DOOR As Operon = DOOROperonView.GetOperon(opr)

            Return String.Equals(DOOR.InitialX.Synonym, locus, StringComparison.OrdinalIgnoreCase)
        End Function

        ''' <summary>
        ''' 是否能够在存在的基因数据之中找得到目标基因对象？
        ''' </summary>
        ''' <param name="locus_tag"></param>
        ''' <returns></returns>
        Public Function HaveGene(locus_tag As String) As Boolean
            Return _geneTable.ContainsKey(locus_tag)
        End Function

        Public Function HaveOperon(DOOR_id$) As Boolean
            Return DOOROperonView._operonsTable.ContainsKey(DOOR_id)
        End Function

        ''' <summary>
        ''' 根据基因编号来判断这两个基因是否是处在相同的一个操纵子之中的
        ''' </summary>
        ''' <param name="locus_a"></param>
        ''' <param name="locus_b"></param>
        ''' <returns></returns>
        Public Function SameOperon(locus_a As String, locus_b As String) As Boolean
            Dim ga As OperonGene = Me(locus_a)
            Dim gb As OperonGene = Me(locus_b)

            If ga Is Nothing OrElse gb Is Nothing Then
                Return False
            End If

            Return String.Equals(ga.OperonID, gb.OperonID)
        End Function

        ''' <summary>
        ''' 根据操纵子编号来获取改操纵子之中的一系列的结构基因
        ''' </summary>
        ''' <param name="operonID"></param>
        ''' <returns></returns>
        Public Function [Select](operonID As String) As OperonGene()
            Dim LQuery = LinqAPI.Exec(Of OperonGene) <=
 _
                From obj As OperonGene
                In Genes
                Where String.Equals(operonID, obj.OperonID)
                Select obj
                Order By obj.Synonym Ascending

            Return LQuery
        End Function

        ''' <summary>
        ''' Gets the operon object where the gene is located. 
        ''' </summary>
        ''' <param name="locusId">The gene's locus_tag</param>
        ''' <returns></returns>
        Public Function GetOperon(locusId As String) As Operon
            Dim g As OperonGene = GetGene(locusId)
            Dim DOOR As String = g.OperonID
            Return DOOROperonView.GetOperon(DOOR)
        End Function

        Public Function Save(Path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Return SaveFile(Me.DOOROperonView.Operons, Path)
        End Function

        Public Overloads Shared Widening Operator CType(Path As String) As DOOR
            Return DOOR_API.Load(Path)
        End Operator

        Public Shared Function CreateEmpty() As DOOR
            Return New DOOR With {
                .Genes = New OperonGene() {},
                .view = New OperonView With {
                    .Operons = New Operon() {}
                }
            }
        End Function

        ''' <summary>
        ''' 导出为一个Csv格式的文件
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="trim">是否将仅有一个基因的Operon对象进行去除</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Export(path$, Optional trim As Boolean = False) As Boolean
            Try
                Return csv(trim).SaveTo(path, Encodings.ASCII.CodePage)
            Catch ex As Exception
                Call App.LogException(New Exception(path, ex))
                Return False
            End Try
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of Operon) Implements IEnumerable(Of Operon).GetEnumerator
            For Each Operon In Me.DOOROperonView.Operons
                Yield Operon
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, encoding.CodePage)
        End Function
    End Class
End Namespace
