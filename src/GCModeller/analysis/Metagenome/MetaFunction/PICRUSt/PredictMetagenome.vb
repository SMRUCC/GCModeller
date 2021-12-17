Imports SMRUCC.genomics.Metagenomics

Namespace PICRUSt

    Public Module PredictMetagenome

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="OTUtable"></param>
        ''' <param name="precalculated">
        ''' ko_13_5_precalculated.tab
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' For metagenome prediction, PICRUSt takes an input OTU table 
        ''' that contains identifiers that match tips from the marker 
        ''' gene (e.g., Greengenes identifiers) with corresponding 
        ''' abundances for each of those OTUs across one or more samples. 
        ''' First, PICRUSt normalizes the OTU table by the 16S copy 
        ''' number predictions so that OTU abundances more accurately 
        ''' reflect the true abundances of the underlying organisms. 
        ''' The metagenome is then predicted by looking up the precalculated 
        ''' genome content for each OTU, multiplying the normalized 
        ''' OTU abundance by each KO abundance in the genome and 
        ''' summing these KO abundances together per sample. The 
        ''' prediction yields a table of KO abundances for each 
        ''' metagenome sample in the OTU table. For optional organism
        ''' specific predictions, the per-organism abundances are 
        ''' retained and annotated for each KO.
        ''' </remarks>
        Public Function PredictMetagenome(OTUtable As OTUData(), precalculated As ko_13_5_precalculated) As OTUData()

        End Function
    End Module
End Namespace