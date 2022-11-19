#Region "Microsoft.VisualBasic::360eef50f12c6b2fbe694507eb8162d5, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\Bacterial_GENOME\TranscriptUnit.vb"

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

    '   Total Lines: 182
    '    Code Lines: 90
    ' Comment Lines: 70
    '   Blank Lines: 22
    '     File Size: 7.82 KB


    '     Class TranscriptUnit
    ' 
    '         Properties: BasalLevel, GeneCluster, MaxLevel, Name, PromoterGene
    '                     RegulatedMotifs
    ' 
    '         Function: _add_Regulator, ContainsGene, CreateObject, get_Regulators, Link
    '                   (+2 Overloads) RemoveRegulator, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels

Namespace GCML_Documents.XmlElements.Bacterial_GENOME

    ''' <summary>
    ''' 在进行模型计算的时候，转录过程从这里开始，基因对象仅仅只是模板数据的承载者，
    ''' 对于一个转录单元而言，其只有一个基因的时候就仅表示该基因对象，当拥有多个基
    ''' 因的时候，则表示为一个操纵元。
    ''' </summary>
    ''' <remarks>
    ''' 在形成调控网络的时候，调控因子对启动子的调控作用被转换为对转录单元的调控作用
    ''' 在这里认为一个转录单元仅有一个启动子单元
    ''' </remarks>
    Public Class TranscriptUnit : Inherits T_MetaCycEntity(Of Slots.TransUnit)
        Implements I_BiologicalProcess_EventHandle

        ''' <summary>
        ''' 相当于<see cref="MetaCyc">MetaCyc数据库</see>中的<see cref="MetaCyc.File.DataFiles.Slots.TransUnit">转录单元</see>对象之中的<see cref="MetaCyc.File.DataFiles.Slots.TransUnit.CommonName">通用名称</see>属性
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Name As String

        ''' <summary>
        ''' An object handle collection of the gene object that defines in the genome namespace of the model 
        ''' file.
        ''' (指向模型文件中的基因列表的基因对象的句柄值的集合, {MetaCycId, NCBI AccessionId})
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' 本属性对应于MetaCyc.TransUnit中的Components属性值
        ''' 
        ''' The Components slot of a transcription unit lists the DNA segments within the transcription unit, 
        ''' including transcription start sites (class Promoters), Terminators, DNA-Binding-Sites, and Genes.
        ''' </remarks>
        <XmlElement("GeneCluster")> Public Property GeneCluster As KeyValuePair()

        ''' <summary>
        ''' 该转录单元的最大表达水平
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property MaxLevel As Double()
        ''' <summary>
        ''' 本底表达水平(表示在没有任何调控因子的作用下的转录水平)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property BasalLevel As Double = 1

        ''' <summary>
        ''' 本转录单元的调控因子以及调控位点
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RegulatedMotifs As List(Of MotifSite)

        ''' <summary>
        ''' Promoter gene unique-id, point to the item <see cref="GeneObject.Identifier"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PromoterGene As GeneObject

        Public Overrides Function ToString() As String
            If GeneCluster.IsNullOrEmpty Then
                Return Identifier
            Else
                Dim sBuilder As StringBuilder = New StringBuilder(10 * 1024)
                For Each Id In GeneCluster
                    Call sBuilder.Append(Id.ToString & ", ")
                Next
                Call sBuilder.Remove(sBuilder.Length - 2, 2)

                Return String.Format("{0}, Gene Cluster:= {1}", Identifier, sBuilder.ToString)
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="AccessionId">NCBI Accession Id</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ContainsGene(AccessionId As String) As Integer
            If GeneCluster.IsNullOrEmpty Then
                Return -1
            End If
            For i As Integer = 0 To GeneCluster.Count - 1
                If String.Equals(AccessionId, _GeneCluster(i).Value) Then
                    Return i
                End If
            Next

            Return -1
        End Function

        ''' <summary>
        ''' 使用MetaCyc数据库中的基因数据，对转录单元中的GeneCluster属性进行赋值
        ''' </summary>
        ''' <param name="Genes"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Link(Genes As Generic.IEnumerable(Of GeneObject)) As TranscriptUnit
            Dim LQuery = From cpId As String
                         In BaseType.Components
                         Let Find = (From Gene In Genes.AsParallel Where String.Equals(Gene.Identifier, cpId) Select Gene).ToArray
                         Where Find.Count > 0
                         Let Item = Find.First
                         Select New KeyValuePair() With {.Value = Item.AccessionId}  ' 
            GeneCluster = LQuery.ToArray
            Return Me
        End Function

        Public Shared Function CreateObject(TransUnit As Slots.TransUnit) As TranscriptUnit
            Return New TranscriptUnit With {
                .BaseType = TransUnit,
                .Identifier = TransUnit.Identifier,
                .Name = TransUnit.CommonName
            }
        End Function

        Public Function get_Regulators() As SignalTransductions.Regulator() Implements I_BiologicalProcess_EventHandle.get_Regulators
            If RegulatedMotifs.IsNullOrEmpty Then
                Return Nothing
            End If

            Return (From item In RegulatedMotifs Let value = item.Regulators Select value).ToArray.ToVector
        End Function

        Public Function _add_Regulator(motifId As String, Regulator As SignalTransductions.Regulator) As Boolean Implements I_BiologicalProcess_EventHandle._add_Regulator
            If String.IsNullOrEmpty(motifId) Then
                motifId = Regulator.Regulates
            End If

            Dim MotifSites = (From item In Me.RegulatedMotifs Where String.Equals(item.MotifName, motifId) Select item).ToArray
            If MotifSites.IsNullOrEmpty Then
                Call Console.WriteLine("Regulator data of {0}{1} can not find the regulated motif, data may be broken!", Regulator.Identifier, Regulator.CommonName)
                Return False
            End If

            Dim Site As MotifSite = MotifSites.First
            Site.Regulators = {Site.Regulators, New SignalTransductions.Regulator() {Regulator}.AsList}.Unlist
            Return True
        End Function

        ''' <summary>
        ''' 所有具有这个ID编号的调控因子都会被移除
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RemoveRegulator(ID As String) As Integer
            For Each motif In Me.RegulatedMotifs
                Dim LQuery = (From item In motif.Regulators Where Not String.Equals(item.Identifier, ID) Select item).AsList
                motif.Regulators = LQuery
            Next

            Return 0
        End Function

        Public Function RemoveRegulator(Regulator As SignalTransductions.Regulator) As Boolean
            For Each motif In Me.RegulatedMotifs
                Dim LQuery = (From item In motif.Regulators Where Not item.Equals(Regulator) Select item).AsList
                motif.Regulators = LQuery
            Next

            Return True
        End Function
    End Class
End Namespace
