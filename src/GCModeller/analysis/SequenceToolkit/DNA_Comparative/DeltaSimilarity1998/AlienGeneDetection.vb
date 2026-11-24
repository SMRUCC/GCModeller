Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' The two thresholds of the 2D alien gene decision rule (the paper's 
    ''' ``B. subtilis`` values):
    ''' 
    ''' condition 1: B(g|all) &gt; 0.42 and B(g|RP) &gt; 0.45 = alien gene (laterally transferred);
    ''' condition 2: B(g|all) &gt; 0.42 but B(g|RP) &lt; 0.45 = highly expressed host gene.
    ''' </summary>
    Public Class AlienGeneThresholds

        ''' <summary>
        ''' the B(g|all) threshold: distance from the average host gene (default 0.42)
        ''' </summary>
        Public Property BiasAll As Double = 0.42

        ''' <summary>
        ''' the B(g|RP) threshold: distance from the ribosomal protein genes (default 0.45)
        ''' </summary>
        Public Property BiasRP As Double = 0.45

        ''' <summary>
        ''' only long genes (default 200 codons) are classified, 
        ''' short ORFs lack the codon usage statistics
        ''' </summary>
        Public Property MinCodons As Integer = 200
    End Class

    ''' <summary>
    ''' The decision class of one gene under the 2D threshold rule.
    ''' </summary>
    Public Enum AlienGeneClasses As Integer

        ''' <summary>
        ''' normal host gene: B(g|all) below the threshold
        ''' </summary>
        Native
        ''' <summary>
        ''' B(g|all) above the threshold but the codon usage is still close to the 
        ''' ribosomal protein genes: a highly expressed host gene 
        ''' (EF-G/EF-Tu, glycolysis enzymes, DnaK/GroEL, ...)
        ''' </summary>
        HighlyExpressed
        ''' <summary>
        ''' unlike both the average host gene and the ribosomal protein genes: 
        ''' likely a laterally transferred (alien) gene
        ''' </summary>
        Alien
    End Enum

    ''' <summary>
    ''' One gene prediction row of the alien gene 2D classification.
    ''' </summary>
    Public Class AlienGenePrediction

        ''' <summary>
        ''' the gene sequence title
        ''' </summary>
        Public Property Gene As String

        ''' <summary>
        ''' number of (sense) codons used for the statistics
        ''' </summary>
        Public Property Codons As Integer

        ''' <summary>
        ''' B(g|all): distance from the average host gene
        ''' </summary>
        Public Property BiasAll As Double

        ''' <summary>
        ''' B(g|RP): distance from the ribosomal protein gene set
        ''' </summary>
        Public Property BiasRP As Double

        ''' <summary>
        ''' the 2D threshold decision result
        ''' </summary>
        Public Property Category As AlienGeneClasses

        Public Overrides Function ToString() As String
            Return $"{Gene} [B(g|all)={BiasAll:F3}, B(g|RP)={BiasRP:F3}] -> {Category}"
        End Function
    End Class

    ''' <summary>
    ''' IDENTIFICATION OF ALIEN (LATERALLY TRANSFERRED) GENES - 
    ''' the 2D threshold method (Karlin, Campbell &amp; Mrazek, 1998, section 7).
    ''' 
    ''' Each long gene (&gt;= 200 codons) is placed on the plane:
    ''' horizontal axis: B(g|RP) - codon bias difference from the ribosomal proteins;
    ''' vertical axis: B(g|all) - codon bias difference from the average host gene.
    ''' 
    ''' Decision rule (B. subtilis thresholds):
    ''' B(g|all) &gt; 0.42 and B(g|RP) &gt; 0.45: alien gene - unlike both the 
    ''' average host gene and the host highly expressed genes;
    ''' B(g|all) &gt; 0.42 but B(g|RP) &lt; 0.45: highly expressed gene - 
    ''' the codon usage agrees with the ribosomal protein pattern.
    ''' </summary>
    <Package("Alien.Gene.Detection",
             Description:="2D threshold method for the identification of alien (laterally transferred) genes (Karlin 1998)")>
    Public Module AlienGeneDetection

        ''' <summary>
        ''' Select the ribosomal protein genes from a genome gene collection by 
        ''' matching the product name pattern in the fasta header.
        ''' </summary>
        ''' <param name="genes"></param>
        ''' <param name="pattern">default matches the ``ribosomal protein`` product names</param>
        ''' <returns></returns>
        <ExportAPI("RibosomalProtein.Genes")>
        Public Function RibosomalProteinGenes(genes As FastaFile,
                                              Optional pattern As String = "ribosomal protein") As FastaSeq()
            Dim regex As New System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase)

            Return genes _
                .Where(Function(gene) regex.IsMatch(gene.Title)) _
                .ToArray
        End Function

        ''' <summary>
        ''' Run the 2D threshold alien gene classification over the genome gene set.
        ''' </summary>
        ''' <param name="genes">all CDS gene sequences of the genome under study</param>
        ''' <param name="referenceAll">the codon usage profile of the average host gene 
        ''' (usually built from all genome genes)</param>
        ''' <param name="referenceRP">the codon usage profile of the ribosomal protein 
        ''' gene set (see <see cref="RibosomalProteinGenes"/>); 
        ''' pass Nothing to skip the B(g|RP) axis</param>
        ''' <param name="thresholds">the decision thresholds, default = paper values</param>
        ''' <returns></returns>
        <ExportAPI("Alien.Genes.Detect")>
        Public Function DetectAlienGenes(genes As FastaFile,
                                         referenceAll As CodonUsageProfile,
                                         referenceRP As CodonUsageProfile,
                                         Optional thresholds As AlienGeneThresholds = Nothing) As AlienGenePrediction()
            If thresholds Is Nothing Then
                thresholds = New AlienGeneThresholds
            End If

            Dim result As AlienGenePrediction() = genes _
                .AsParallel() _
                .Select(Function(gene)
                            Dim usage As CodonUsageProfile = CodonUsageProfile.FromGenes(
                                {gene}, referenceAll.code, name:=gene.Title)
                            Dim bAll As Double = CodonBiasMeasure.BiasDifference(usage, referenceAll)
                            Dim bRP As Double = If(referenceRP Is Nothing, 0, CodonBiasMeasure.BiasDifference(usage, referenceRP))
                            Dim category As AlienGeneClasses = Classify(bAll, bRP, thresholds, referenceRP Is Nothing)

                            Return New AlienGenePrediction With {
                                .Gene = gene.Title,
                                .Codons = usage.TotalCodons,
                                .BiasAll = bAll,
                                .BiasRP = bRP,
                                .Category = category
                            }
                        End Function) _
                .Where(Function(p) p.Codons >= thresholds.MinCodons) _
                .ToArray

            Return result
        End Function

        ''' <summary>
        ''' The 2D threshold decision rule.
        ''' </summary>
        ''' <param name="bAll">B(g|all)</param>
        ''' <param name="bRP">B(g|RP)</param>
        ''' <param name="thresholds"></param>
        ''' <param name="skipRPAxis">true when no ribosomal protein reference was provided</param>
        ''' <returns></returns>
        Public Function Classify(bAll As Double,
                                 bRP As Double,
                                 thresholds As AlienGeneThresholds,
                                 Optional skipRPAxis As Boolean = False) As AlienGeneClasses
            If bAll <= thresholds.BiasAll Then
                Return AlienGeneClasses.Native
            ElseIf skipRPAxis Then
                ' without the B(g|RP) axis, a high B(g|all) gene is only 
                ' reported as a candidate (alien or highly expressed)
                Return AlienGeneClasses.Alien
            ElseIf bRP > thresholds.BiasRP Then
                Return AlienGeneClasses.Alien
            Else
                Return AlienGeneClasses.HighlyExpressed
            End If
        End Function

        ''' <summary>
        ''' Summary report of the detection result.
        ''' </summary>
        ''' <param name="predictions"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Summary(predictions As IEnumerable(Of AlienGenePrediction)) As String
            Dim all As AlienGenePrediction() = predictions.ToArray
            Dim alien = all.Where(Function(p) p.Category = AlienGeneClasses.Alien).ToArray
            Dim hea = all.Where(Function(p) p.Category = AlienGeneClasses.HighlyExpressed).ToArray

            Return $"{all.Length} genes classified: {alien.Length} alien, {hea.Length} highly expressed."
        End Function
    End Module
End Namespace
