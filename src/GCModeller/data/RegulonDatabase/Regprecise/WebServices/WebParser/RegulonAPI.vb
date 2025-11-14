#Region "Microsoft.VisualBasic::d906cc4804feb4bf6dfa54c29f73f58c, data\RegulonDatabase\Regprecise\WebServices\WebParser\RegulonAPI.vb"

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

    '   Total Lines: 247
    '    Code Lines: 201 (81.38%)
    ' Comment Lines: 28 (11.34%)
    '    - Xml Docs: 96.43%
    ' 
    '   Blank Lines: 18 (7.29%)
    '     File Size: 12.52 KB


    '     Module RegulonAPI
    ' 
    '         Function: __getOperons, BuildMapHash, Equals, (+5 Overloads) Reconstruct, TrimId
    '                   uid
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace Regprecise

    ''' <summary>
    ''' 根据直系同源的方法能够真的重建出Regulon么？
    ''' </summary>
    <Package("Regprecise.RegulonAPI",
                      Description:="Tools API for reconstruct the regulon data for your bacterial genome by uisng Regprecise regulon data.",
                      Category:=APICategories.ResearchTools,
                      Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Module RegulonAPI

        <ExportAPI("Regulon.Equals", Info:="Regulon equals?")>
        Public Function Equals(a As Regulator, b As Regulator, Optional allMisMatch As Integer = 0) As Boolean
            If Not String.Equals(a.LocusId, b.LocusId, StringComparison.OrdinalIgnoreCase) Then
                Return False
            End If
            Dim aList As String() = a.Regulates.Select(Function(x) x.locusId)
            Dim bList As String() = b.Regulates.Select(Function(x) x.locusId)
            Dim na, nb As Integer

            For Each sId As String In aList
                If Array.IndexOf(bList, sId) = -1 Then
                    na += 1
                    If na > allMisMatch Then
                        Return False
                    End If
                End If
            Next
            For Each sId As String In bList
                If Array.IndexOf(aList, sId) = -1 Then
                    nb += 1
                    If nb > allMisMatch Then
                        Return False
                    End If
                End If
            Next

            Return True
        End Function

        ''' <summary>
        ''' 这个函数主要是用来唯一标示meme的分析结果的
        ''' </summary>
        ''' <param name="regulator"></param>
        ''' <returns></returns>
        <Extension>
        Public Function uid(regulator As Regulator) As String
            Return $"{regulator.LocusId.NormalizePathString(True)}.{regulator.locus_tags.Values.JoinBy(",").NormalizePathString(True)}"
        End Function

        <ExportAPI("Regulon.Reconstruct")>
        Public Function Reconstruct(mappings As String, Regprecise As String, door As String) As BacteriaRegulome
            Dim DoorOperon As DOOR = DOOR_API.Load(door)
            Return Reconstruct(mappings, Regprecise, DoorOperon)
        End Function

        ''' <summary>
        ''' Regulon reconstruction
        ''' </summary>
        ''' <param name="mappings">Mappings result of the regulon gene from the bbh method.</param>
        ''' <param name="Regprecise">Regulon annotation data source genome data which is downloaded from the RegPrecise web server.</param>
        ''' <param name="door">Door operon database</param>
        ''' <returns></returns>
        <ExportAPI("Regulon.Reconstruct")>
        Public Function Reconstruct(mappings As String, Regprecise As String, DOOR As DOOR) As BacteriaRegulome
            Dim bbh = mappings.LoadCsv(Of BiDirectionalBesthit)
            Dim genomeRef = Regprecise.LoadXml(Of BacteriaRegulome)
            Dim regulators = bbh.Reconstruct(genomeRef, DOOR)
            Dim genomeGET As New BacteriaRegulome With {
                .genome = New JSON.genome With {
                    .name = BaseName(mappings)
                },
                .regulome = New Regulome With {
                    .regulators = regulators
                }
            }
            Return genomeGET
        End Function

        <ExportAPI("MapHash.Build"), Extension>
        Public Function BuildMapHash(mappings As IEnumerable(Of BiDirectionalBesthit)) As Dictionary(Of String, BiDirectionalBesthit())
            Dim mappingHash = (From x As BiDirectionalBesthit In mappings
                               Where x.isMatched
                               Select x
                               Group x By x.HitName.Split(":"c).Last Into Group) _
                                    .ToDictionary(Function(x) x.Last,
                                                  Function(x) x.Group.ToArray)
            Return mappingHash
        End Function

        <ExportAPI("Regulon.Reconstruct")>
        <Extension>
        Public Function Reconstruct(mappings As IEnumerable(Of BiDirectionalBesthit),
                                    Regprecise As BacteriaRegulome,
                                    Operons As DOOR) As Regulator()
            Dim mappingHash As Dictionary(Of String, BiDirectionalBesthit()) = mappings.BuildMapHash
            Dim reconstructs As Regulator() = mappingHash.Reconstruct(Regprecise, Operons)
            Return reconstructs
        End Function

        <ExportAPI("Regulon.Reconstruct")>
        <Extension>
        Public Function Reconstruct(mappings As Dictionary(Of String, BiDirectionalBesthit()),
                                    Regprecise As BacteriaRegulome,
                                    Operons As DOOR) As Regulator()
            Dim LQuery As Regulator() = (From x As Regulator
                                         In Regprecise.regulome.regulators
                                         Select mappings.Reconstruct(regulon:=x, DOOR:=Operons)).ToVector
            Return LQuery
        End Function

        ''' <summary>
        ''' 分别比较调控因子和被调控的基因，被调控的基因应该是在一个操纵子里面的
        ''' </summary>
        ''' <param name="mappings"></param>
        ''' <param name="regulon">RegPrecise数据库之中的数据</param>
        ''' <returns></returns>
        <ExportAPI("Regulon.Reconstruct")>
        <Extension>
        Public Function Reconstruct(mappings As Dictionary(Of String, BiDirectionalBesthit()),
                                    regulon As Regulator,
                                    DOOR As DOOR) As Regulator()
            If Not mappings.ContainsKey(regulon.LocusId) Then
                Return Nothing
            End If

            Dim mapReg As BiDirectionalBesthit() = mappings(regulon.LocusId)
            Dim mappedGenes As RegulatedGene() = LinqAPI.Exec(Of RegulatedGene) <=
                From gene As RegulatedGene
                In regulon.Regulates
                Where mappings.ContainsKey(gene.locusId)
                Let maps = mappings(gene.locusId)
                Select From x As BiDirectionalBesthit
                       In maps
                       Select New RegulatedGene With {
                           .description = x.description,
                           .locusId = x.QueryName,
                           .vimssId = x.HitName,
                           .name = x.term
                       }

            If mappedGenes.IsNullOrEmpty Then  ' 没有mapping得到共同的被调控的基因，则不敢太确定是不是成立的
                Return Nothing
            End If

            Dim mappedOperons As Operon() = __getOperons(mappedGenes, DOOR)
            Dim reRegulon As Regulator() =
                LinqAPI.Exec(Of Regulator) <= From mapped As BiDirectionalBesthit
                                              In mapReg
                                              Let locusId = New NamedValue With {
                                                  .name = mapped.QueryName,
                                                  .text = mapped.HitName
                                              }
                                              Select New Regulator With {
                                                  .regulatorySites = regulon.regulatorySites,
                                                  .regulator = regulon.regulator,
                                                  .locus_tags = {locusId},
                                                  .biological_process = regulon.biological_process,
                                                  .effector = regulon.effector,
                                                  .family = regulon.family,
                                                  .pathway = regulon.pathway,
                                                  .regulationMode = regulon.regulationMode,
                                                  .regulog = regulon.regulog,
                                                  .type = regulon.type,
                                                  .operons = mappedOperons,
                                                  .Regulates = mappedGenes
                                              }
            Return reRegulon
        End Function

        ''' <summary>
        ''' 从需要被注释的基因组之中获取得到被调控基因所在的操纵子
        ''' </summary>
        ''' <param name="mappings"></param>
        ''' <param name="DOOR"></param>
        ''' <returns></returns>
        Private Function __getOperons(mappings As RegulatedGene(), DOOR As DOOR) As Operon()
            Dim mapHash = (From x As RegulatedGene In mappings
                           Select x
                           Group x By x.locusId Into Group) _
                                .ToDictionary(Function(x) x.locusId,
                                              Function(x) x.Group.ToArray)
            Dim oprGenes As OperonGene()
            If DOOR.Genes.IsNullOrEmpty Then
                oprGenes =
                    LinqAPI.Exec(Of OperonGene) <= From x As RegulatedGene
                                                  In mappings
                                                   Select New OperonGene With {
                                                       .OperonID = "x",
                                                       .Synonym = x.locusId,
                                                       .Product = x.description,
                                                       .GI = x.vimssId
                                                   }
            Else
                oprGenes = mappings.Select(Function(x) DOOR.GetGene(x.locusId))
            End If
            Dim opr = (From x As OperonGene In oprGenes
                       Select x
                       Group x By x.OperonID Into Group) _
                            .Select(Function(x) New Operon With {
                                .ID = x.Group.First.OperonID,
                                .members = x.Group.Select(Function(name) mapHash(name.Synonym)).ToVector})
            ' 补齐基因的功能描述信息
            For Each gene As RegulatedGene In mappings
                If String.IsNullOrEmpty(gene.description) Then
                    If DOOR.HaveGene(gene.locusId) Then
                        gene.description = DOOR.GetGene(gene.locusId).Product
                    End If
                End If
                If String.IsNullOrEmpty(gene.name) Then
                    If DOOR.HaveGene(gene.locusId) Then
                        gene.name = DOOR.GetGene(gene.locusId).COG_number
                    End If
                End If
            Next

            Return opr
        End Function

        <ExportAPI("BBH.sId.Trim")>
        <Extension>
        Public Function TrimId(source As IEnumerable(Of BiDirectionalBesthit)) As BiDirectionalBesthit()
            Dim setValue As New SetValue(Of BiDirectionalBesthit)()
            Dim LQuery As BiDirectionalBesthit() =
                LinqAPI.Exec(Of BiDirectionalBesthit) <=
                    From x As BiDirectionalBesthit
                    In source.AsParallel
                    Let qid As String = x.QueryName.Split.First.Split("|"c).First
                    Let sid As String = x.HitName.Split.First.Split("|"c).First
                    Select setValue _
                        .InvokeSetValue(x, NameOf(x.QueryName), qid) _
                        .InvokeSet(NameOf(x.HitName), sid).obj
            Return LQuery
        End Function
    End Module
End Namespace
