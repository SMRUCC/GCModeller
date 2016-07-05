#Region "Microsoft.VisualBasic::c86951d57b985fd560cc60f5be4c644a, ..\GCModeller\data\RegulonDatabase\Regprecise\WebServices\WebParser\RegulonAPI.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace Regprecise

    ''' <summary>
    ''' 根据直系同源的方法能够真的重建出Regulon么？
    ''' </summary>
    <PackageNamespace("Regprecise.RegulonAPI",
                      Description:="Tools API for reconstruct the regulon data for your bacterial genome by uisng Regprecise regulon data.",
                      Category:=APICategories.ResearchTools,
                      Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Module RegulonAPI

        <ExportAPI("Regulon.Equals", Info:="Regulon equals?")>
        Public Function Equals(a As Regulator, b As Regulator, Optional allMisMatch As Integer = 0) As Boolean
            If Not String.Equals(a.LocusId, b.LocusId, StringComparison.OrdinalIgnoreCase) Then
                Return False
            End If
            Dim aList As String() = a.Regulates.ToArray(Function(x) x.LocusId)
            Dim bList As String() = b.Regulates.ToArray(Function(x) x.LocusId)
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
        <Extension> Public Function uid(regulator As Regulator) As String
            Return $"{regulator.LocusId.NormalizePathString(True)}.{regulator.LocusTag.Value.NormalizePathString(True)}"
        End Function

        <ExportAPI("Regulon.Reconstruct")>
        Public Function Reconstruct(mappings As String, Regprecise As String, door As String) As BacteriaGenome
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
        Public Function Reconstruct(mappings As String, Regprecise As String, DOOR As DOOR) As BacteriaGenome
            Dim bbh = mappings.LoadCsv(Of BiDirectionalBesthit)
            Dim genomeRef = Regprecise.LoadXml(Of BacteriaGenome)
            Dim regulators = bbh.Reconstruct(genomeRef, DOOR)
            Dim genomeGET As New BacteriaGenome With {
                .BacteriaGenome = New WebServices.JSONLDM.genome With {
                    .name = IO.Path.GetFileNameWithoutExtension(mappings)
                },
                .Regulons = New Regulon With {
                    .Regulators = regulators
                }
            }
            Return genomeGET
        End Function

        <ExportAPI("MapHash.Build"), Extension>
        Public Function BuildMapHash(mappings As IEnumerable(Of BiDirectionalBesthit)) As Dictionary(Of String, BiDirectionalBesthit())
            Dim mappingHash = (From x As BiDirectionalBesthit In mappings
                               Where x.Matched
                               Select x
                               Group x By x.HitName.Split(":"c).Last Into Group) _
                                    .ToDictionary(Function(x) x.Last,
                                                  Function(x) x.Group.ToArray)
            Return mappingHash
        End Function

        <ExportAPI("Regulon.Reconstruct")>
        <Extension>
        Public Function Reconstruct(mappings As IEnumerable(Of BiDirectionalBesthit),
                                    Regprecise As BacteriaGenome,
                                    Operons As DOOR) As Regulator()
            Dim mappingHash As Dictionary(Of String, BiDirectionalBesthit()) = mappings.BuildMapHash
            Dim reconstructs As Regulator() = mappingHash.Reconstruct(Regprecise, Operons)
            Return reconstructs
        End Function

        <ExportAPI("Regulon.Reconstruct")>
        <Extension>
        Public Function Reconstruct(mappings As Dictionary(Of String, BiDirectionalBesthit()),
                                    Regprecise As BacteriaGenome,
                                    Operons As DOOR) As Regulator()
            Dim LQuery As Regulator() = (From x As Regulator
                                         In Regprecise.Regulons.Regulators
                                         Select mappings.Reconstruct(regulon:=x, DOOR:=Operons)).MatrixToVector
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
                Where mappings.ContainsKey(gene.LocusId)
                Let maps = mappings(gene.LocusId)
                Select From x As BiDirectionalBesthit
                       In maps
                       Select New RegulatedGene With {
                           .Function = x.Description,
                           .LocusId = x.QueryName,
                           .vimssId = x.HitName,
                           .Name = x.COG
                       }

            If mappedGenes.IsNullOrEmpty Then  ' 没有mapping得到共同的被调控的基因，则不敢太确定是不是成立的
                Return Nothing
            End If

            Dim mappedOperons As Operon() = __getOperons(mappedGenes, DOOR)
            Dim reRegulon As Regulator() =
                LinqAPI.Exec(Of Regulator) <= From mapped As BiDirectionalBesthit
                                              In mapReg
                                              Let locusId = New KeyValuePair With {
                                                  .Key = mapped.QueryName,
                                                  .Value = mapped.HitName
                                              }
                                              Select New Regulator With {
                                                  .RegulatorySites = regulon.RegulatorySites,
                                                  .Regulator = regulon.Regulator,
                                                  .LocusTag = locusId,
                                                  .BiologicalProcess = regulon.BiologicalProcess,
                                                  .Effector = regulon.Effector,
                                                  .Family = regulon.Family,
                                                  .Pathway = regulon.Pathway,
                                                  .RegulationMode = regulon.RegulationMode,
                                                  .Regulog = regulon.Regulog,
                                                  .Type = regulon.Type,
                                                  .lstOperon = mappedOperons,
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
                           Group x By x.LocusId Into Group) _
                                .ToDictionary(Function(x) x.LocusId,
                                              Function(x) x.Group.ToArray)
            Dim oprGenes As GeneBrief()
            If DOOR.Genes.IsNullOrEmpty Then
                oprGenes =
                    LinqAPI.Exec(Of GeneBrief) <= From x As RegulatedGene
                                                  In mappings
                                                  Select New GeneBrief With {
                                                      .OperonID = "x",
                                                      .Synonym = x.LocusId,
                                                      .Product = x.Function,
                                                      .GI = x.vimssId
                                                  }
            Else
                oprGenes = mappings.ToArray(Function(x) DOOR.GetGene(x.LocusId))
            End If
            Dim opr = (From x As GeneBrief In oprGenes
                       Select x
                       Group x By x.OperonID Into Group) _
                            .ToArray(Function(x) New Operon With {
                                .sId = x.Group.First.OperonID,
                                .Members = x.Group.ToArray(Function(name) mapHash(name.Synonym)).MatrixToVector})
            ' 补齐基因的功能描述信息
            For Each gene As RegulatedGene In mappings
                If String.IsNullOrEmpty(gene.Function) Then
                    If DOOR.ContainsGene(gene.LocusId) Then
                        gene.Function = DOOR.GetGene(gene.LocusId).Product
                    End If
                End If
                If String.IsNullOrEmpty(gene.Name) Then
                    If DOOR.ContainsGene(gene.LocusId) Then
                        gene.Name = DOOR.GetGene(gene.LocusId).COG_number
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
