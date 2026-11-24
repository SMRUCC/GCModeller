Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Conversion

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' The six-level significance grading of the symmetrized dinucleotide relative 
    ''' abundance values ``rho*`` described in Table 1 of the paper 
    ''' (Karlin, Campbell &amp; Mrazek, 1998).
    ''' 
    ''' For sequences longer than 50kb, ``rho* &lt;= 0.78`` or ``rho* &gt;= 1.23`` is 
    ''' regarded as significantly under-represented / over-represented 
    ''' (the probability for a random sequence to reach such extremes is &lt;= 0.001).
    ''' </summary>
    Public Enum SignatureLevels As Integer

        ''' <summary>rho* &lt; 0.50, strongly under-represented (symbol ``---``)</summary>
        UnderRepresented3
        ''' <summary>0.50 &lt;= rho* &lt; 0.70, under-represented (symbol ``--``)</summary>
        UnderRepresented2
        ''' <summary>0.70 &lt;= rho* &lt; 0.78, mildly under-represented (symbol ``-``)</summary>
        UnderRepresented1
        ''' <summary>0.78 &lt;= rho* &lt;= 1.23, within the random expectation range</summary>
        Neutral
        ''' <summary>1.23 &lt; rho* &lt;= 1.30, mildly over-represented (symbol ``+``)</summary>
        OverRepresented1
        ''' <summary>1.30 &lt; rho* &lt;= 1.50, over-represented (symbol ``++``)</summary>
        OverRepresented2
        ''' <summary>rho* &gt; 1.50, strongly over-represented (symbol ``+++``)</summary>
        OverRepresented3
    End Enum

    ''' <summary>
    ''' GENOME SIGNATURES (Karlin, Campbell &amp; Mrazek, 1998)
    ''' 
    ''' Dinucleotide relative abundance values (dinucleotide bias) are assessed through 
    ''' the odds ratio ``rho(XY) = f(XY)/f(X)f(Y)``, where ``f(X)`` denotes the frequency 
    ''' of the nucleotide ``X`` and ``f(XY)`` is the frequency of the dinucleotide ``XY`` 
    ''' in the sequence under study.
    ''' 
    ''' Because DNA is double-stranded, the paper symmetrizes the counts by concatenating 
    ''' the sequence with its reverse complement, which yields the genome signature 
    ''' ``rho*`` (see <see cref="NucleicAcid.RelativeAbundance"/>). 
    ''' The overall G+C content effect is removed by the odds ratio, so genomes of 
    ''' different G+C content can be compared directly.
    ''' </summary>
    ''' <remarks>
    ''' All calculations are based on the count-matrix cache type 
    ''' <see cref="NucleicAcid"/> (single pass O(n) integer counting).
    ''' </remarks>
    <Package("Genome.Signatures")>
    Public Module GenomeSignatures

        ''' <summary>
        ''' The raw (non-symmetrized) odds ratio ``rho(XY) = f(XY)/f(X)f(Y)`` of 
        ''' the forward strand.
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <param name="X"></param>
        ''' <param name="Y"></param>
        ''' <returns></returns>
        <Extension>
        Public Function OddsRatio(nt As NucleicAcid, X As DNA, Y As DNA) As Double
            Return nt.OddsRatio(X, Y)
        End Function

        ''' <summary>
        ''' The symmetrized dinucleotide relative abundance ``rho*XY``: 
        ''' the genome signature of the double-stranded DNA. 
        ''' (compatible rename of the legacy ``DinucleotideBIAS`` API)
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <param name="X"></param>
        ''' <param name="Y"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GenomeSignature(nt As NucleicAcid, X As DNA, Y As DNA) As Double
            Return nt.RelativeAbundance(X, Y)
        End Function

        ''' <summary>
        ''' The full 16-dim symmetrized genome signature vector ``{rho*XY}``.
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GenomeSignature(nt As NucleicAcid) As Double()
            Return nt.SignatureVector()
        End Function

        ''' <summary>
        ''' Six-level significance grading of the given ``rho*`` value, 
        ''' see <see cref="SignatureLevels"/>.
        ''' </summary>
        ''' <param name="rhoStar"></param>
        ''' <returns></returns>
        Public Function SignatureLevel(rhoStar As Double) As SignatureLevels
            If rhoStar < 0.5 Then
                Return SignatureLevels.UnderRepresented3
            ElseIf rhoStar < 0.7 Then
                Return SignatureLevels.UnderRepresented2
            ElseIf rhoStar < 0.78 Then
                Return SignatureLevels.UnderRepresented1
            ElseIf rhoStar <= 1.23 Then
                Return SignatureLevels.Neutral
            ElseIf rhoStar <= 1.3 Then
                Return SignatureLevels.OverRepresented1
            ElseIf rhoStar <= 1.5 Then
                Return SignatureLevels.OverRepresented2
            Else
                Return SignatureLevels.OverRepresented3
            End If
        End Function

        ''' <summary>
        ''' Six-level significance grading of the genome signature ``rho*XY``.
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <param name="X"></param>
        ''' <param name="Y"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SignatureLevel(nt As NucleicAcid, X As DNA, Y As DNA) As SignatureLevels
            Return SignatureLevel(nt.RelativeAbundance(X, Y))
        End Function

        ''' <summary>
        ''' The six-level significance symbol string of the given ``rho*`` value 
        ''' (Table 1 legend of the paper): 
        ''' ``---`` / ``--`` / ``-`` / (empty for neutral) / ``+`` / ``++`` / ``+++``.
        ''' </summary>
        ''' <param name="rhoStar"></param>
        ''' <returns></returns>
        Public Function SignificanceSymbol(rhoStar As Double) As String
            Select Case SignatureLevel(rhoStar)
                Case SignatureLevels.UnderRepresented3 : Return "---"
                Case SignatureLevels.UnderRepresented2 : Return "--"
                Case SignatureLevels.UnderRepresented1 : Return "-"
                Case SignatureLevels.OverRepresented1 : Return "+"
                Case SignatureLevels.OverRepresented2 : Return "++"
                Case SignatureLevels.OverRepresented3 : Return "+++"
                Case Else : Return ""
            End Select
        End Function

        ''' <summary>
        ''' The six-level significance symbol of the genome signature ``rho*XY``.
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <param name="X"></param>
        ''' <param name="Y"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SignificanceSymbol(nt As NucleicAcid, X As DNA, Y As DNA) As String
            Return nt.SignificanceSymbol(X, Y)
        End Function

        ''' <summary>
        ''' Build the ``{rho*XY, symbol}`` signature profile table of the 16 dinucleotides.
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SignatureProfile(nt As NucleicAcid) As NamedValue(Of String)()
            Dim order As (X As DNA, Y As DNA)() = NucleicAcid.DimerOrder
            Dim profile As New List(Of NamedValue(Of String))

            For Each dimer As (X As DNA, Y As DNA) In order
                Dim rhoStar As Double = nt.RelativeAbundance(dimer.X, dimer.Y)

                profile.Add(New NamedValue(Of String) With {
                    .Name = $"{ToChar(dimer.X)}{ToChar(dimer.Y)}",
                    .Value = $"{rhoStar.ToString("F4")} [{GenomeSignatures.SignificanceSymbol(rhoStar)}]",
                    .Description = rhoStar.ToString
                })
            Next

            Return profile.ToArray
        End Function
    End Module
End Namespace
