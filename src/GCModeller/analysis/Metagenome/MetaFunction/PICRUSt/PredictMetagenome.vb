Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Metagenomics

Namespace PICRUSt

    Public Module PredictMetagenome

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="OTUtable">
        ''' For metagenome prediction, PICRUSt takes an input OTU table 
        ''' that contains identifiers that match tips from the marker 
        ''' gene (e.g., Greengenes identifiers) with corresponding 
        ''' abundances for each of those OTUs across one or more samples.
        ''' </param>
        ''' <param name="precalculated">
        ''' ko_13_5_precalculated.tab, precalculated 
        ''' genome content 
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
        <Extension>
        Public Function PredictMetagenome(OTUtable As OTUData(Of Double)(), precalculated As MetaBinaryReader) As OTUData(Of Double)()
            ' normalize the OTU table at first
            Dim allSampleNames As String() = OTUtable _
                .Select(Function(OTU) OTU.data.Keys) _
                .IteratesALL _
                .Distinct _
                .ToArray
            Dim KOIds As String() = precalculated.GetAllFeatureIds
            Dim OTUdata = OTUtable _
                .Select(Function(OTU)
                            Dim tax As Taxonomy = BIOMTaxonomyParser.Parse(OTU.taxonomy)
                            Dim data = precalculated.findByTaxonomy(tax)

                            If data.IsNullOrEmpty Then
                                Return KOIds.ToDictionary(Function(id) id, Function(any) 0.0)
                            Else
                                Return data
                            End If
                        End Function) _
                .ToArray
            Dim KOResult As OTUData(Of Double)() = New OTUData(Of Double)(precalculated.featureSize - 1) {}
            Dim OTU_KO As OTUData(Of Dictionary(Of String, Double))() = New OTUData(Of Dictionary(Of String, Double))(OTUdata.Length - 1) {}

            For i As Integer = 0 To OTU_KO.Length - 1
                OTU_KO(i) = New OTUData(Of Dictionary(Of String, Double)) With {
                    .data = New Dictionary(Of String, Dictionary(Of String, Double)),
                    .OTU = i + 1,
                    .taxonomy = OTUtable(i).taxonomy
                }
            Next

            For Each name As String In allSampleNames
                Dim v As Double() = OTUtable _
                    .Select(Function(OTU) OTU.data.TryGetValue(name)) _
                    .ToArray
                Dim sum As Double = v.Sum

                For i As Integer = 0 To v.Length - 1
                    Dim norm = v(i) / sum
                    Dim idx As Integer = i
                    Dim ko_vec As Dictionary(Of String, Double) = KOIds _
                        .ToDictionary(Function(id) id,
                                      Function(id)
                                          Return norm * OTUdata(idx)(id)
                                      End Function)

                    Call OTU_KO(i).data.Add(name, ko_vec)
                Next
            Next


        End Function
    End Module
End Namespace