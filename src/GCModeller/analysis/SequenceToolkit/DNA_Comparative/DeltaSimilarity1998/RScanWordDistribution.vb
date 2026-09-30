Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Conversion

''' <summary>
''' R-SCAN: word spatial distribution significance test 
''' (Dembo-Karlin 1992; Karlin, Campbell &amp; Mrazek 1998 section 5).
''' 
''' While the ``rho*``/``tau*`` statistics only ask "how often does the word occur", 
''' the r-scan statistics ask "how are the occurrences distributed": whether a set of 
''' marked positions (e.g. the USS sites, the rare CTAG tetranucleotide) fits the 
''' random (Poisson) spacing expectation. Three kinds of deviations are detected:
''' 
''' 1. **clustering** - e.g. the CTAG clusters inside the ``E. coli`` rRNA genes;
''' 2. **overdispersion** - e.g. the USS sites in the phage Mu region of 
'''    ``H. influenzae`` 1.56-1.59 Mb;
''' 3. **even spacing** - e.g. the HIP1 element (GGCGATCGCC) of ``Synechocystis``.
''' 
''' The statistical core is provided by the sciBASIC# Math.Statistics module 
''' (``RScan.RScanStatistics``), this module only adds the word position scanning 
''' and the interpretation layer.
''' </summary>
<Package("RScan.Word.Distribution",
         Description:="r-scan statistics of the word spatial distribution (clustering / overdispersion / even spacing)")>
Public Module RScanWordDistribution

    ''' <summary>
    ''' Locate all occurrences of the given oligonucleotide word in the genome 
    ''' sequence (exact match, both strands are not merged: the r-scan statistics 
    ''' are evaluated on the forward strand positions).
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="word"></param>
    ''' <returns>0-based start positions of the word occurrences</returns>
    <ExportAPI("Word.Locate")>
    <Extension>
    Public Function LocateWord(genome As DeltaSimilarity1998.NucleicAcid, word As String) As Integer()
        If String.IsNullOrEmpty(word) Then
            Return New Integer() {}
        End If

        Dim w As Char() = word.ToUpper.Trim.ToArray
        Dim seq As DNA() = genome.nt
        Dim positions As New List(Of Integer)
        Dim n As Integer = seq.Length - w.Length

        For i As Integer = 0 To n
            Dim matched As Boolean = True

            For j As Integer = 0 To w.Length - 1
                If ToChar(seq(i + j)) <> w(j) Then
                    matched = False
                    Exit For
                End If
            Next

            If matched Then
                positions.Add(i)
            End If
        Next

        Return positions.ToArray
    End Function

    ''' <summary>
    ''' Run the r-scan significance test on the spatial distribution of the 
    ''' given word occurrences.
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="word">the oligonucleotide word under study</param>
    ''' <param name="options">r-scan options (RMax, gap model, Monte Carlo, ...)</param>
    ''' <returns></returns>
    <ExportAPI("Word.RScan")>
    <Extension>
    Public Function RScan(genome As DeltaSimilarity1998.NucleicAcid,
                          word As String,
                          Optional options As Microsoft.VisualBasic.Math.Statistics.RScan.RScanOptions = Nothing) As WordScanResult
        Dim positions As Integer() = genome.LocateWord(word)
        Dim result As Microsoft.VisualBasic.Math.Statistics.RScan.RScanResult =
            Microsoft.VisualBasic.Math.Statistics.RScan.RScanStatistics.Scan(
                positions.Select(Function(p) CDbl(p)),
                genome.Length,
                options)

        Return New WordScanResult With {
            .Word = word,
            .Occurrences = positions.Length,
            .GenomeLength = genome.Length,
            .Scan = result,
            .Pattern = InterpretScan(result)
        }
    End Function

    ''' <summary>
    ''' Interpretation of the r-scan result: clustering / overdispersion / even spacing 
    ''' / no significant deviation (p &gt; 0.05).
    ''' </summary>
    ''' <param name="scan"></param>
    ''' <returns></returns>
    Public Function InterpretScan(scan As Microsoft.VisualBasic.Math.Statistics.RScan.RScanResult) As String
        If scan Is Nothing OrElse scan.M < 3 Then
            Return "too few sites (m < 3) for the r-scan statistics"
        End If

        Dim pLeft As Double = scan.BestPLeftBonferroni
        Dim pRight As Double = If(scan.RightTable.Count > 0, scan.RightTable(0).PRight, 1.0)

        If pLeft < 0.05 AndAlso pRight < 0.05 Then
            Return "both clustering and overdispersion/even-spacing signals (check the gap table)"
        ElseIf pLeft < 0.05 Then
            Return $"clustering (best r={scan.BestR}, Bonferroni p={pLeft:G4})"
        ElseIf pRight < 0.05 Then
            Return $"overdispersion / even spacing (max gap={scan.MaxGap:G6}, p={pRight:G4})"
        Else
            Return "no significant deviation from the random (Poisson) spacing"
        End If
    End Function
End Module

''' <summary>
''' The word distribution r-scan result of one oligonucleotide word.
''' </summary>
Public Class WordScanResult

    ''' <summary>the word under study</summary>
    Public Property Word As String

    ''' <summary>number of occurrences found in the genome</summary>
    Public Property Occurrences As Integer

    ''' <summary>the genome sequence length in bp</summary>
    Public Property GenomeLength As Integer

    ''' <summary>the raw r-scan statistical result (left/right tail tables)</summary>
    Public Property Scan As Microsoft.VisualBasic.Math.Statistics.RScan.RScanResult

    ''' <summary>
    ''' the pattern interpretation: clustering / overdispersion / even spacing
    ''' </summary>
    Public Property Pattern As String

    Public Overrides Function ToString() As String
        Return $"{Word}: m={Occurrences}, L={GenomeLength} -> {Pattern}"
    End Function
End Class
