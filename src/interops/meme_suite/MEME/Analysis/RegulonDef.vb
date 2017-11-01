#Region "Microsoft.VisualBasic::dfe8a5e15f126d95c70b8b43cbceb1ce, ..\interops\meme_suite\MEME\Analysis\RegulonDef.vb"

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

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans

Namespace Analysis

    ''' <summary>
    ''' 基因能够被比对得上，并且motif位点也能够恰好被比对上，就认为是一个成功预测的Regulon？？
    ''' </summary>
    ''' 
    <Package("MEME.Regulon.Def", Category:=APICategories.ResearchTools)>
    Public Module RegulonDef

        Public ReadOnly Property MAST_LDM As Dictionary(Of String, AnnotationModel)

        Sub New()
            Call Settings.Session.Initialize()

            Dim source = GCModeller.FileSystem.FileSystem.GetMotifLDM.LoadSourceEntryList("*.xml")
            MAST_LDM = source.ToDictionary(Function(x) x.Key, Function(x) x.Value.LoadXml(Of AnnotationModel))
        End Sub

        Public Function IsRegulonCorrect(regulon As Regprecise.Regulator) As Boolean

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="regulonBBH">ref.regulon.bbh.xml</param>
        ''' <param name="tomOUT">tom_out.csv.DIR</param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Export")>
        Public Function Export(regulonBBH As String, tomOUT As String) As Regulon()

            VBDebugger.Mute = True

            Dim regulons = FileIO.FileSystem.GetFiles(regulonBBH, FileIO.SearchOption.SearchTopLevelOnly, "*.xml") _
                .Select(Function(x) x.LoadXml(Of Regprecise.BacteriaGenome))
            Dim tomOUTs = FileIO.FileSystem.GetFiles(tomOUT, FileIO.SearchOption.SearchAllSubDirectories, "*.csv") _
                .Select(Function(x) x.LoadCsv(Of Analysis.Similarity.TOMQuery.CompareResult)).ToVector
            Dim tomHash = (From x As Similarity.TOMQuery.CompareResult
                           In tomOUTs
                           Select uid = basename(x.QueryName),
                               x
                           Group By uid Into Group) _
                                .ToDictionary(Function(x) x.uid,
                                              Function(x) x.Group.Select(Function(xx) xx.x))
            Dim regulonHash = (From x In regulons
                               Where Not x.Regulons Is Nothing
                               Select x.Regulons.Regulators.Select(Function(xx) New With {.uid = uid(xx), .regulon = xx})).ToVector
            Dim regulonResults = (From x In regulonHash Where tomHash.ContainsKey(x.uid) Select x.regulon.__creates(tomHash(x.uid))).ToVector
            Return regulonResults
        End Function

        Private Function uid(regulon As Regprecise.Regulator) As String
            Return $"{regulon.LocusTag.Key}.{regulon.LocusTag.Value}".Replace(":", "_")
        End Function

        <Extension> Private Function __creates(regulon As Regprecise.Regulator, query As Similarity.TOMQuery.CompareResult()) As Regulon()
            Return query.Select(Function(x) regulon.__creates(x))
        End Function

        <Extension> Private Function __creates(regulonRef As Regprecise.Regulator, query As Similarity.TOMQuery.CompareResult) As Regulon
            Dim regulon As New Regulon With {
                .BiologicalProcess = regulonRef.BiologicalProcess,
                .Family = regulonRef.Family,
                .Hit = query.HitName,
                .HitMotif = query.HitMotif,
                .Mode = regulonRef.RegulationMode,
                .Motif = query.QueryMotif,
                .Pathway = regulonRef.Pathway,
                .refLocus = regulonRef.LocusTag.Value,
                .Regulates = regulonRef.Regulates.Select(Function(x) x.LocusId),
                .Regulator = regulonRef.LocusTag.Key,
                .Similarity = query.Similarity,
                .Consensus = query.Consensus,
                .Distance = query.Distance,
                .Edits = query.Edits,
                .Gaps = query.Gaps,
                .ref = regulonRef.Regulog.Key,
                .Score = query.Score,
                .Trace = "",
                .Effector = regulonRef.Effector
            }
            Return regulon
        End Function
    End Module

    Public Class Regulon
        Public Property ref As String
        <Column("ref.Locus_tag")> Public Property refLocus As String
        Public Property Regulator As String
        Public Property Regulates As String()
        Public Property Family As String
        Public Property Hit As String
        Public Property Motif As String
        <Column("hit.Motif")> Public Property HitMotif As String
        <XmlAttribute> Public Property Score As Double
        <XmlAttribute> Public Property Distance As Double
        <XmlAttribute> Public Property Similarity As Double
        <XmlElement> Public Property Edits As String
        <XmlElement> Public Property Consensus As String
        <XmlAttribute> Public Property Gaps As Integer
        Public Property Trace As String
        Public Property Mode As String
        Public Property Pathway As String
        Public Property BiologicalProcess As String
        Public Property Effector As String

    End Class
End Namespace
