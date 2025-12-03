#Region "Microsoft.VisualBasic::a1bdf21a5baf9d846431f586e5135917, analysis\Metagenome\MetaFunction\PICRUSt\PredictMetagenome.vb"

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

'   Total Lines: 149
'    Code Lines: 88 (59.06%)
' Comment Lines: 37 (24.83%)
'    - Xml Docs: 81.08%
' 
'   Blank Lines: 24 (16.11%)
'     File Size: 6.49 KB


'     Module PredictMetagenome
' 
'         Function: PredictMetagenome
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Metagenomics

Namespace PICRUSt

    Public Class PredictMetagenome

        ''' <summary>
        ''' binary database file reader for ``ko_13_5_precalculated.tab``, precalculated genome content 
        ''' </summary>
        ReadOnly precalculated As MetaBinaryReader
        ReadOnly println As Action(Of String, Boolean)
        ReadOnly KOIds As String()

        Sub New(ko_13_5_precalculated As MetaBinaryReader, Optional log As Action(Of String, Boolean) = Nothing)
            precalculated = ko_13_5_precalculated
            println = If(log, New Action(Of String, Boolean)(Sub(s, flag) Console.WriteLine(s)))
            KOIds = precalculated.GetAllFeatureIds
        End Sub

        Private Iterator Function LoadGenomeContent(OTUtable As OTUData(Of Double)()) As IEnumerable(Of Dictionary(Of String, Double))
            For i As Integer = 0 To OTUtable.Length - 1
                Dim OTU As OTUData(Of Double) = OTUtable(i)
                Dim tax As Taxonomy = BIOMTaxonomyParser.Parse(OTU.taxonomy)
                Dim data As Dictionary(Of String, Double) = precalculated.findByTaxonomy(tax)

                Call println($" -> {data.TryCount} hits, {OTU.taxonomy} [{i + 1}/{OTUtable.Length}]", True)

                If data.IsNullOrEmpty Then
                    Yield KOIds.ToDictionary(Function(id) id, Function(any) 0.0)
                Else
                    Yield data
                End If
            Next

            Call println(" done!", True)
        End Function

        Private Function SamplePredicts(allSampleNames As String(), OTUtable As OTUData(Of Double)(), OTUdata As Dictionary(Of String, Double)()) As OTUData(Of Dictionary(Of String, Double))()
            Dim OTU_KO As OTUData(Of Dictionary(Of String, Double))() = New OTUData(Of Dictionary(Of String, Double))(OTUdata.Length - 1) {}

            For i As Integer = 0 To OTU_KO.Length - 1
                OTU_KO(i) = New OTUData(Of Dictionary(Of String, Double)) With {
                    .data = New Dictionary(Of String, Dictionary(Of String, Double)),
                    .OTU = i + 1,
                    .taxonomy = OTUtable(i).taxonomy
                }
            Next

            Dim n As i32 = 1

            Call println("start processing samples:", True)

            For Each name As String In allSampleNames
                Dim v As Double() = OTUtable _
                    .Select(Function(OTU) OTU.data.TryGetValue(name)) _
                    .ToArray
                Dim sum As Double = v.Sum

                Call println($"[{++n}/{allSampleNames.Length}]Processing sample: {name}...", False)

                For i As Integer = 0 To v.Length - 1
                    Dim norm As Double = v(i) / sum
                    Dim PICRUSt As Dictionary(Of String, Double) = OTUdata(i)
                    Dim ko_vec As Dictionary(Of String, Double) = KOIds _
                        .ToDictionary(Function(id) id,
                                      Function(id)
                                          Return norm * PICRUSt(id)
                                      End Function)

                    Call OTU_KO(i).data.Add(name, ko_vec)
                Next

                Call println(" ~done!", True)
            Next

            Return OTU_KO
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="OTUtable">
        ''' For metagenome prediction, PICRUSt takes an input OTU table 
        ''' that contains identifiers that match tips from the marker 
        ''' gene (e.g., Greengenes identifiers) with corresponding 
        ''' abundances for each of those OTUs across one or more samples.
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' For metagenome prediction, PICRUSt takes an input OTU table 
        ''' that contains identifiers that match tips from the marker 
        ''' gene (e.g., Greengenes identifiers) with corresponding 
        ''' abundances for each of those OTUs across one or more samples. 
        ''' 
        ''' First, PICRUSt normalizes the OTU table by the 16S copy 
        ''' number predictions so that OTU abundances more accurately 
        ''' reflect the true abundances of the underlying organisms. 
        ''' 
        ''' The metagenome is then predicted by looking up the precalculated 
        ''' genome content for each OTU, multiplying the normalized 
        ''' OTU abundance by each KO abundance in the genome and 
        ''' summing these KO abundances together per sample. The 
        ''' prediction yields a table of KO abundances for each 
        ''' metagenome sample in the OTU table. For optional organism
        ''' specific predictions, the per-organism abundances are 
        ''' retained and annotated for each KO.
        ''' </remarks>
        ''' 
        Public Iterator Function PredictMetagenome(OTUtable As OTUData(Of Double)()) As IEnumerable(Of OTUData(Of Double))
            ' normalize the OTU table at first
            Dim allSampleNames As String() = OTUtable _
                .Select(Function(OTU) OTU.data.Keys) _
                .IteratesALL _
                .Distinct _
                .ToArray

            Call println($"Loading {KOIds.Length} KO id list, and", True)
            Call println($"   processing for {OTUtable.Length} OTU dataset!", True)
            Call println($"Loading taxonomy reference data...", True)

            ' load ko_13_5_precalculated genome content for each OTU
            Dim OTUdata As Dictionary(Of String, Double)() = LoadGenomeContent(OTUtable).ToArray
            Dim OTU_KO As OTUData(Of Dictionary(Of String, Double))() = SamplePredicts(allSampleNames, OTUtable, OTUdata)
            Dim featureSize As Integer = precalculated.featureSize

            ' column is sample names
            ' row is the KO terms
            For i As Integer = 0 To featureSize - 1
                Dim sampledata As New Dictionary(Of String, Double)

                For Each sample As String In allSampleNames
                    Dim sum As Double = 0
                    Dim id As String = KOIds(i)

                    For Each OTU In OTU_KO
                        sum += OTU.data(sample)(id)
                    Next

                    sampledata(sample) = sum
                Next

                Yield New OTUData(Of Double) With {
                    .OTU = KOIds(i),
                    .taxonomy = KOIds(i),
                    .data = sampledata
                }
            Next
        End Function
    End Class
End Namespace
