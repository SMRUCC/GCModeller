#Region "Microsoft.VisualBasic::347b9cd9164bc4c79d7f272f4906b7e4, ..\GCModeller\core\Bio.Assembly\Assembly\DOOR\DOOR.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
Imports SMRUCC.genomics.ComponentModel.Loci
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel

Namespace Assembly.DOOR

    ''' <summary>
    ''' DOOR: Database of prOkaryotic OpeRons.
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
        Public Property Genes As GeneBrief()
            Get
                Return _Genes
            End Get
            Set(value As GeneBrief())
                _Genes = value
                _geneHash = _Genes.ToDictionary(Function(gene) gene.Synonym)
            End Set
        End Property

        Public Property DOOROperonView As OperonView

        Dim _Genes As GeneBrief()
        Dim _geneHash As Dictionary(Of String, GeneBrief)

        ''' <summary>
        ''' 查找不到目标基因对象则会返回空值
        ''' </summary>
        ''' <param name="locusId"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property GetGene(locusId As String) As GeneBrief
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
        ''' <param name="locus"></param>
        ''' <returns></returns>
        Public Function IsOprPromoter(locus As String) As Boolean
            Dim g As GeneBrief = _geneHash(locus)
            Dim opr As String = g.OperonID
            Dim DOOR As Operon = Me.DOOROperonView.GetOperon(opr)
            Return String.Equals(DOOR.InitialX.Synonym, locus, StringComparison.OrdinalIgnoreCase)
        End Function

        Public Function ContainsGene(GeneID As String) As Boolean
            Return _geneHash.ContainsKey(GeneID)
        End Function

        Public Function SameOperon(a As String, b As String) As Boolean
            Dim ga = Me(a)
            Dim gb = Me(b)
            If ga Is Nothing OrElse gb Is Nothing Then
                Return False
            End If

            Return String.Equals(ga.OperonID, gb.OperonID)
        End Function

        Public Function [Select](OperonId As String) As GeneBrief()
            Dim LQuery = (From obj As GeneBrief In Genes
                          Where String.Equals(OperonId, obj.OperonID)
                          Select obj
                          Order By obj.Synonym Ascending).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' Gets the operon object where the gene is located. 
        ''' </summary>
        ''' <param name="locusId">The gene's locus_tag</param>
        ''' <returns></returns>
        Public Function GetOperon(locusId As String) As Operon
            Dim g As GeneBrief = GetGene(locusId)
            Dim DOOR As String = g.OperonID
            Return Me.DOOROperonView.GetOperon(DOOR)
        End Function

        Public Overrides Function Save(Optional Path As String = "", Optional encoding As Encoding = Nothing) As Boolean
            Return SaveFile(Me.DOOROperonView.Operons, getPath(Path))
        End Function

        Public Overloads Shared Widening Operator CType(Path As String) As DOOR
            Return DOOR_API.Load(Path)
        End Operator

        Private Function __generate(sId As String, trim As Boolean) As String
            Dim Genes As GeneBrief() = Me.[Select](OperonId:=sId)
            If trim Then
                If Genes.Count < 2 Then
                    Return ""
                End If
            End If
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Dim LocationBuilder As StringBuilder = New StringBuilder(128)

            For Each obj In Genes
                Call sBuilder.Append(obj.Synonym & "; ")
                Call LocationBuilder.Append(If(obj.Location.Strand = Strands.Forward, "+; ", "-; "))
            Next
            Call sBuilder.Remove(sBuilder.Length - 2, 2)
            Call LocationBuilder.Remove(LocationBuilder.Length - 2, 2)

            Dim strData As StringBuilder = New StringBuilder(1024)
            Call strData.Append(String.Format("{0},{1},", sId, Genes.Length))
            Call strData.Append(LocationBuilder.ToString & ",")
            Call strData.Append(sBuilder.ToString)

            Return strData.ToString
        End Function

        Public Shared Function CreateEmpty() As DOOR
            Return New DOOR With {
                .Genes = New GeneBrief() {},
                .DOOROperonView = New OperonView With {
                    .Operons = New Operon() {}
                }
            }
        End Function

        Public Shared Function DocParser(data As String(), path As String) As DOOR
            Dim LQuery As GeneBrief() = (From s_Line As String In data.Skip(1)
                                         Where Not String.IsNullOrEmpty(s_Line)
                                         Select GeneBrief.TryParse(s_Line)).ToArray
            Dim DOOR As DOOR = New DOOR With {
                .Genes = LQuery,
                .FilePath = path
            }
            DOOR.DOOROperonView = Assembly.DOOR.CreateOperonView(DOOR)
            Return DOOR
        End Function

        ''' <summary>
        ''' 导出为一个Csv格式的文件
        ''' </summary>
        ''' <param name="SavedPath"></param>
        ''' <param name="Trim">是否将仅有一个基因的Operon对象进行去除</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Export(SavedPath As String, Trim As Boolean) As Boolean
            Dim OperonIds As String() = (From GeneObj As Assembly.DOOR.GeneBrief
                                         In Genes
                                         Select GeneObj.OperonID
                                         Distinct
                                         Order By OperonID Ascending).ToArray
            Dim LQuery = (From OperonID As String In OperonIds
                          Let row As String = __generate(OperonID, Trim)
                          Where Not String.IsNullOrEmpty(row)
                          Select row).ToList
            Call LQuery.Insert(0, "OperonID,Counts,Direction,OperonGenes")

            Try
                Call IO.File.WriteAllLines(SavedPath, LQuery.ToArray)
            Catch ex As Exception
                Return False
            End Try

            Return True
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
