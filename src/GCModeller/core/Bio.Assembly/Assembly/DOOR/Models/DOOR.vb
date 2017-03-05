#Region "Microsoft.VisualBasic::86b06a7b94ebb1d510e86dabbf7f5eb8, ..\GCModeller\core\Bio.Assembly\Assembly\DOOR\DOOR.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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
    Public Class DOOR : Inherits ITextFile
        Implements IEnumerable(Of Operon)

        ''' <summary>
        ''' 在文件之中，是一个表格的形式来表示整个文件的，这个属性表示文件之中的所有行
        ''' </summary>
        ''' <returns></returns>
        Public Property Genes As OperonGene()
            Get
                Return _Genes
            End Get
            Set(value As OperonGene())
                _Genes = value
                _geneHash = _Genes.ToDictionary(Function(gene) gene.Synonym)
            End Set
        End Property

        Public Property DOOROperonView As OperonView

        Dim _Genes As OperonGene()
        Dim _geneHash As Dictionary(Of String, OperonGene)

        ''' <summary>
        ''' 查找不到目标基因对象则会返回空值
        ''' </summary>
        ''' <param name="locusId">基因编号</param>
        ''' <returns></returns>
        Default Public ReadOnly Property GetGene(locusId As String) As OperonGene
            Get
                If _geneHash.ContainsKey(locusId) Then
                    Return _geneHash(locusId)
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
            Dim g As OperonGene = _geneHash(locus)
            Dim opr As String = g.OperonID
            Dim DOOR As Operon = Me.DOOROperonView.GetOperon(opr)

            Return String.Equals(DOOR.InitialX.Synonym, locus, StringComparison.OrdinalIgnoreCase)
        End Function

        ''' <summary>
        ''' 是否能够在存在的基因数据之中找得到目标基因对象？
        ''' </summary>
        ''' <param name="GeneID"></param>
        ''' <returns></returns>
        Public Function ContainsGene(GeneID As String) As Boolean
            Return _geneHash.ContainsKey(GeneID)
        End Function

        ''' <summary>
        ''' 根据基因编号来判断这两个基因是否是处在相同的一个操纵子之中的
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Function SameOperon(a As String, b As String) As Boolean
            Dim ga As OperonGene = Me(a)
            Dim gb As OperonGene = Me(b)

            If ga Is Nothing OrElse gb Is Nothing Then
                Return False
            End If

            Return String.Equals(ga.OperonID, gb.OperonID)
        End Function

        ''' <summary>
        ''' 根据操纵子编号来获取改操纵子之中的一系列的结构基因
        ''' </summary>
        ''' <param name="OperonId"></param>
        ''' <returns></returns>
        Public Function [Select](OperonId As String) As OperonGene()
            Dim LQuery = LinqAPI.Exec(Of OperonGene) <=
 _
                From obj As OperonGene
                In Genes
                Where String.Equals(OperonId, obj.OperonID)
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
            Return Me.DOOROperonView.GetOperon(DOOR)
        End Function

        Public Overrides Function Save(Optional Path As String = "", Optional encoding As Encoding = Nothing) As Boolean
            Return SaveFile(Me.DOOROperonView.Operons, getPath(Path))
        End Function

        Public Overloads Shared Widening Operator CType(Path As String) As DOOR
            Return DOOR_API.Load(Path)
        End Operator

        Public Shared Function CreateEmpty() As DOOR
            Return New DOOR With {
                .Genes = New OperonGene() {},
                .DOOROperonView = New OperonView With {
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
    End Class
End Namespace
