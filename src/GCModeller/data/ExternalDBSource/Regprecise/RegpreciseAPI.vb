#Region "Microsoft.VisualBasic::6b01be8899d6a58d829312845b604fc7, ExternalDBSource\Regprecise\RegpreciseAPI.vb"

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

    '     Module RegpreciseAPI
    ' 
    '         Function: Download, DownloadRegulatorSequence, Export, ExportBySpecies, (+2 Overloads) ExportMotifs
    '                   GetFastaCollection, GetTfFamilies, ReadXml, RegpreciseRegulatorMatch, WriteMatches
    '                   WriteRegprecise
    '         Class Matches
    ' 
    '             Properties: Matched, RegpreciseRegulator, RegulonSites
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Datavisualization.DocumentFormat.Extensions

Namespace Regprecise

    <[Namespace]("regprecise", Description:="[Regprecise database] [Collections of regulogs classified by transcription factors]")>
    Public Module RegpreciseAPI

        <Command("wget.regprecise", info:="Download the whole regprecise database fro each bacteria genome.")>
        Public Function Download(Optional Export As String = "") As TranscriptionFactors
            If String.IsNullOrEmpty(Export) Then
                Export = My.Computer.FileSystem.SpecialDirectories.Temp
            End If

            Dim Regprecise = TranscriptionFactors.Download(Export)
            Return Regprecise
        End Function

        <Command("wget.regprecise_regulators", info:="Download regprecise regulator protein sequence from kegg database.")>
        Public Function DownloadRegulatorSequence(Regprecise As Regprecise.TranscriptionFactors, Optional Export As String = "") As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile
            If String.IsNullOrEmpty(Export) Then
                Export = My.Computer.FileSystem.SpecialDirectories.Temp
            End If

            Return ExternalDBSource.Regprecise.TranscriptionFactors.DownloadRegulatorSequence(Regprecise, Export)
        End Function

        <Command("write.xml.regprecise")>
        Public Function WriteRegprecise(Regprecise As TranscriptionFactors, saveto As String) As Boolean
            Return Regprecise.GetXml.SaveTo(saveto)
        End Function

        Public Class Matches
            Public Property RegpreciseRegulator As String
            Public Property Matched As String()
            Public Property RegulonSites As String()
        End Class

        <Command("regprecise.matches_regulator")>
        Public Function RegpreciseRegulatorMatch(Regprecise As Regprecise.TranscriptionFactors, Bh As Generic.IEnumerable(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BidirBestHitBLAST.BiDireBesthit)) As Matches()
            Dim ChunkBuffer As List(Of Matches) = New List(Of Matches)

            For Each BacteriaGenome In Regprecise.BacteriaGenomes
                Dim LQuery = (From RegpreciseRegulator In BacteriaGenome.Regulons.Regulators
                              Let Regulator As String = RegpreciseRegulator.Regulator.Key
                              Select New Matches With
                                     {
                                         .RegpreciseRegulator = Regulator,
                                         .RegulonSites = (From item In RegpreciseRegulator.RegulatorySites Select String.Format("{0}:{1}", item.LocusTag, item.Position)).ToArray,
                                         .Matched = (From item In Bh Where String.Equals(item.HitName, Regulator) Select item.QueryName).ToArray})

                Call ChunkBuffer.AddRange(LQuery)
            Next

            Return ChunkBuffer.ToArray
        End Function

        <Command("write.csv.matches")>
        Public Function WriteMatches(data As Generic.IEnumerable(Of Matches), saveto As String) As Boolean
            Return data.SaveTo(saveto, False)
        End Function

        <Command("read.xml.regprecise")>
        Public Function ReadXml(path As String) As LANS.SystemsBiology.ExternalDBSource.Regprecise.TranscriptionFactors
            Return LANS.SystemsBiology.ExternalDBSource.Regprecise.TranscriptionFactors.Load(path)
        End Function

        <Command("regprecise.export_motifs.by_species")>
        Public Function ExportBySpecies(Regprecise As LANS.SystemsBiology.ExternalDBSource.Regprecise.TranscriptionFactors, dir As String) As Boolean
            Dim TFFamilies = GetTfFamilies(Regprecise)
            Dim TFSitesFasta = GetFastaCollection(Regprecise)
            Dim BriefInfo As Microsoft.VisualBasic.Datavisualization.DocumentFormat.Csv.File = New Datavisualization.DocumentFormat.Csv.File

            Call BriefInfo.Add(New String() {"Family", "Species", "Counts", "Locations"})

            For Each Family As String In TFFamilies
                Dim LQuery = (From item In TFSitesFasta.AsParallel Where String.Equals(item.Key, Family) Select New With {.Species = Split(item.Value, " - ").Last, .Fasta = item.DataObject}).ToArray
                Dim Species = (From item In LQuery Let SpeciesId As String = item.Species Select SpeciesId Distinct).ToArray
                Dim MergedSmalls As List(Of KeyValuePair(Of String, Regtransbase.WebServices.FastaObject)) = New List(Of KeyValuePair(Of String, Regtransbase.WebServices.FastaObject))

                Dim MergeAction =
                    Sub(Collection As Generic.IEnumerable(Of KeyValuePair(Of String, Regtransbase.WebServices.FastaObject)), SpeciesId As String)
                        Dim FastaFile = CType(ExportMotifs(Collection, Family), LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile)
                        Dim Path As String = String.Format("{0}/{1}.{2}.fasta", dir, Family.Replace("[", "").Replace("]", "").NormalizePathString, SpeciesId.NormalizePathString(True).Replace("-", "_").Replace(" ", "_"))
                        Dim Locations As String() = (From item In Collection Select String.Format("{0}{1}", item.Value.LocusTag, item.Value.Position)).ToArray

                        Call BriefInfo.AppendLine(New String() {Family, SpeciesId, Collection.Count, String.Join("; ", Locations)})
                        Call FastaFile.Save(Path, System.Text.Encoding.ASCII)
                    End Sub

                For Each SpeciesId As String In Species
                    Dim FastaCollection = (From item In LQuery Where String.Equals(item.Species, SpeciesId) Select New KeyValuePair(Of String, Regtransbase.WebServices.FastaObject)(SpeciesId, item.Fasta)).ToArray

                    If FastaCollection.Count < 2 Then
                        Call MergedSmalls.AddRange(FastaCollection)
                        Continue For
                    End If

                    Call MergeAction(FastaCollection, SpeciesId)
                Next

                If Not MergedSmalls.IsNullOrEmpty Then
                    Call MergeAction(MergedSmalls, "Other")
                End If
            Next

            Call BriefInfo.Save(String.Format("{0}/BriefInfo.csv", dir))

            Return True
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Collection"></param>
        ''' <param name="Family"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExportMotifs(Collection As Generic.IEnumerable(Of KeyValuePair(Of String, Regtransbase.WebServices.FastaObject)), Family As String) As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject()
            Dim LQuery = (From i As Integer In Collection.Sequence
                          Let FastaObject = Collection(i)
                          Select New LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject With {
                              .SequenceData = Regtransbase.WebServices.Regulator.SequenceTrimming(FastaObject.Value),
                              .Attributes = New String() {String.Format("{0}:{1}_lcl.{2} [family={3}] [regulog={4}]", FastaObject.Value.LocusTag, FastaObject.Value.Position, i, Family, String.Format("{0} - {1}", Family, FastaObject.Key))}}).ToArray
            Return LQuery
        End Function

        Private Function GetFastaCollection(Regprecise As LANS.SystemsBiology.ExternalDBSource.Regprecise.TranscriptionFactors) _
            As Microsoft.VisualBasic.ComponentModel.Collection.Generic.KeyValuePairData(Of Regtransbase.WebServices.FastaObject)()

            Dim ChunkBuffer As List(Of Microsoft.VisualBasic.ComponentModel.Collection.Generic.KeyValuePairData(Of Regtransbase.WebServices.FastaObject)) =
                New List(Of Microsoft.VisualBasic.ComponentModel.Collection.Generic.KeyValuePairData(Of Regtransbase.WebServices.FastaObject))

            For Each BacterialGenome In Regprecise.BacteriaGenomes
                For Each Regulon In BacterialGenome.Regulons.Regulators
                    If Regulon.Type = BacteriaGenome.Regulon.Regulator.Types.RNA Then
                        Continue For
                    End If

                    Call ChunkBuffer.AddRange((From item
                                               In Regulon.RegulatorySites
                                               Select New Microsoft.VisualBasic.ComponentModel.Collection.Generic.KeyValuePairData(Of Regtransbase.WebServices.FastaObject) With {
                                                   .Key = Regulon.Family,
                                                   .Value = Regulon.Regulog.Key,
                                                   .DataObject = item}).ToArray)
                Next
            Next

            Return ChunkBuffer.ToArray
        End Function

        <Command("regprecise.export_motifs_sequence")>
        Public Function ExportMotifs(Regprecise As LANS.SystemsBiology.ExternalDBSource.Regprecise.TranscriptionFactors) As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile
            Return ExternalDBSource.Regprecise.TranscriptionFactors.ExportTFBSInfo(Regprecise)
        End Function

        <Command("regprecise.export_motifs")>
        Public Function Export(Regprecise As LANS.SystemsBiology.ExternalDBSource.Regprecise.TranscriptionFactors, dir As String) As Boolean
            Dim TFFamilies = GetTfFamilies(Regprecise)
            Dim ChunkBuffer As Dictionary(Of String, LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile) =
                New Dictionary(Of String, LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile)

            For Each TFFamily As String In TFFamilies
                Call ChunkBuffer.Add(TFFamily, New LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile)
            Next

            For Each BacterialGenome In Regprecise.BacteriaGenomes
                For Each Regulon In BacterialGenome.Regulons.Regulators
                    If Regulon.Type = BacteriaGenome.Regulon.Regulator.Types.RNA Then
                        Continue For
                    End If

                    Dim FastaFile = ChunkBuffer(Regulon.Family)
                    Call FastaFile.AddRange(Regulon.ExportMotifs)
                Next
            Next

            For Each item In ChunkBuffer
                Dim FastaFile = item.Value
                For i As Integer = 0 To FastaFile.Count - 1
                    Dim FastaObject = FastaFile(i)
                    Dim attrs = FastaObject.Attributes.ToList
                    attrs(0) = String.Format("lcl_{0} ", i) & attrs.First
                    FastaObject.Attributes = attrs.ToArray
                Next
                Call FastaFile.Save(String.Format("{0}/{1}.fasta", dir, item.Key))
            Next

            Return True
        End Function

        Private Function GetTfFamilies(Regprecise As LANS.SystemsBiology.ExternalDBSource.Regprecise.TranscriptionFactors) As String()
            Dim Chunkbuffer As List(Of String) = New List(Of String)

            For Each BacterialGenome In Regprecise.BacteriaGenomes
                Call Chunkbuffer.AddRange((From item In BacterialGenome.Regulons.Regulators Where item.Type = BacteriaGenome.Regulon.Regulator.Types.TF Select item.Family).ToArray)
            Next

            Return (From strValue As String In Chunkbuffer Select strValue Distinct).ToArray
        End Function
    End Module
End Namespace
