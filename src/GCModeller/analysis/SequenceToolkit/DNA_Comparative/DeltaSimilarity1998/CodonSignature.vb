Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' The four embedded dinucleotide position combinations of the codon signature 
    ''' (Karlin, Campbell &amp; Mrazek, 1998):
    ''' 
    ''' ```
    '''    rhoXY(1, 2) = fXY(1, 2)/fX(1)fY(2)
    '''    rhoYZ(2, 3) = fYZ(2, 3)/fY(2)fZ(3)
    '''    rhoXZ(1, 3) = fXZ(1, 3)/fX(1)fZ(3)
    '''    rhoZW(3, 4) = ... (4 = 1 is the first position of the next codon)
    ''' ```
    ''' </summary>
    Public Enum CodonSites As Integer

        ''' <summary>the 1st and 2nd position inside the codon</summary>
        Site12
        ''' <summary>the 2nd and 3rd position inside the codon</summary>
        Site23
        ''' <summary>the 1st and 3rd position inside the codon (non-adjacent)</summary>
        Site13
        ''' <summary>
        ''' the junction between two successive codons: position 3 of codon i 
        ''' and position 1 (``4 = 1``) of codon i + 1
        ''' </summary>
        Site34
    End Enum

    ''' <summary>
    ''' CODON SIGNATURE
    ''' 
    ''' For a given collection of genes, let fX(1), fY(2), fZ(3) denote frequencies of 
    ''' the indicated nucleotide at codon sites 1, 2, and 3, respectively, and let 
    ''' f(XYZ) indicate codon frequency. The embedded dinucleotide frequencies are denoted 
    ''' fXY(1, 2); fYZ(2, 3); and fXZ(1, 3). Dinucleotide contrasts are assessed through 
    ''' the odds ratio:
    ''' 
    ''' ```
    '''    rhoXY(1, 2) = fXY(1, 2)/fX(1)fY(2)
    '''    rhoYZ(2, 3) = fYZ(2, 3)/fY(2)fZ(3)
    '''    rhoXZ(1, 3) = fXZ(1, 3)/fX(1)fZ(3)
    ''' ```
    ''' 
    ''' We refer to the profiles {rhoXY(1, 2)}, {rhoXZ(1, 3)}, {rhoYZ(2, 3)}, and also 
    ''' {rhoZW(3, 4)}, where 4(=1) is the first position of the next codon, as the 
    ''' codon signature to be distinguished from the global genome signature.
    ''' 
    ''' This is the correct (paper-conform) site-specific implementation which replaces 
    ''' the legacy ``GenomeSignatures.CodonSignature`` function that mistakenly reused 
    ''' the global genome dinucleotide odds ratio.
    ''' </summary>
    Public Module CodonSignature

        ''' <summary>
        ''' Compute the codon signature profile from a collection of CDS gene sequences.
        ''' 
        ''' Codons that contain ambiguous bases (R/Y/N/...) are skipped. 
        ''' Only complete codons (sequence length being a multiple of 3, tail remainder 
        ''' is ignored) are taken into account.
        ''' </summary>
        ''' <param name="genes"></param>
        ''' <param name="name"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CodonSignatureProfile(genes As IEnumerable(Of FastaSeq),
                                              Optional name As String = Nothing) As CodonSignatureProfile
            Dim profile As New CodonSignatureProfile With {.Name = If(name, "codon-signature")}

            For Each gene As FastaSeq In genes
                Call profile.AddGene(New NucleotideModels.NucleicAcid(gene, strict:=False).ToArray)
            Next

            Call profile.Freeze()
            Return profile
        End Function

        ''' <summary>
        ''' Compute the codon signature profile from a single gene collection 
        ''' of nucleotide sequences.
        ''' </summary>
        ''' <param name="genes"></param>
        ''' <param name="name"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CodonSignatureProfile(genes As IEnumerable(Of DNA()),
                                              Optional name As String = Nothing) As CodonSignatureProfile
            Dim profile As New CodonSignatureProfile With {.Name = If(name, "codon-signature")}

            For Each gene As DNA() In genes
                Call profile.AddGene(gene)
            Next

            Call profile.Freeze()
            Return profile
        End Function
    End Module

    ''' <summary>
    ''' The codon signature profile of a gene collection: per-site nucleotide counts, 
    ''' site-specific dinucleotide counts, codon counts and the derived codon signature 
    ''' odds ratios {rhoXY(1,2)}, {rhoYZ(2,3)}, {rhoXZ(1,3)}, {rhoZW(3,4)}.
    ''' </summary>
    Public Class CodonSignatureProfile

        ''' <summary>
        ''' the display name of the gene collection
        ''' </summary>
        Public Property Name As String

        Friend site1(3) As Integer
        Friend site2(3) As Integer
        Friend site3(3) As Integer
        Friend dimer12(3, 3) As Integer
        Friend dimer23(3, 3) As Integer
        Friend dimer13(3, 3) As Integer
        Friend dimer34(3, 3) As Integer
        ''' <summary>
        ''' codon counts indexed by the packed base index ``((b1*4 + b2)*4 + b3)``
        ''' </summary>
        Friend codonCounts(63) As Integer
        Friend codonTotal As Integer
        Friend junctionTotal As Integer

        Friend signatureCache As Double()() = New Double(3)() {}

        ''' <summary>
        ''' Add one CDS gene sequence into the counters. 
        ''' (call <see cref="Freeze"/> after all genes have been added)
        ''' </summary>
        ''' <param name="gene"></param>
        Friend Sub AddGene(gene As DNA())
            Dim n As Integer = gene.Length - gene.Length Mod 3
            Dim prevCodon As Boolean = False
            Dim p3 As Integer = -1

            For i As Integer = 0 To n - 1 Step 3
                Dim b1 As Integer = NucleicAcid.BaseIndex(gene(i))
                Dim b2 As Integer = NucleicAcid.BaseIndex(gene(i + 1))
                Dim b3 As Integer = NucleicAcid.BaseIndex(gene(i + 2))

                If b1 < 0 OrElse b2 < 0 OrElse b3 < 0 Then
                    ' ambiguous codon: skip and break the (3,4) junction chain
                    prevCodon = False
                    Continue For
                End If

                site1(b1) += 1
                site2(b2) += 1
                site3(b3) += 1
                dimer12(b1, b2) += 1
                dimer23(b2, b3) += 1
                dimer13(b1, b3) += 1
                codonCounts((b1 * 4 + b2) * 4 + b3) += 1
                codonTotal += 1

                If prevCodon Then
                    ' junction (3, 4=1): position 3 of the previous codon and 
                    ' position 1 of the current codon
                    dimer34(p3, b1) += 1
                    junctionTotal += 1
                End If

                prevCodon = True
                p3 = b3
            Next
        End Sub

        Friend Sub Freeze()
            ' nothing to do: all derived values are computed on demand
        End Sub

        ''' <summary>
        ''' Total number of valid codons (no ambiguous base) counted in the profile.
        ''' </summary>
        Public ReadOnly Property Codons As Integer
            Get
                Return codonTotal
            End Get
        End Property

        ''' <summary>
        ''' Total number of valid codon junctions (3, 4) counted in the profile.
        ''' </summary>
        Public ReadOnly Property Junctions As Integer
            Get
                Return junctionTotal
            End Get
        End Property

        ''' <summary>
        ''' The relative frequency f(XYZ) of the given codon within this gene collection.
        ''' </summary>
        ''' <param name="codon"></param>
        ''' <returns></returns>
        Public Function CodonFrequency(codon As Codon) As Double
            If codonTotal = 0 Then
                Return 0
            End If

            Dim b1 As Integer = NucleicAcid.BaseIndex(codon.X)
            Dim b2 As Integer = NucleicAcid.BaseIndex(codon.Y)
            Dim b3 As Integer = NucleicAcid.BaseIndex(codon.Z)

            If b1 < 0 OrElse b2 < 0 OrElse b3 < 0 Then
                Return 0
            End If

            Return codonCounts((b1 * 4 + b2) * 4 + b3) / codonTotal
        End Function

        ''' <summary>
        ''' The absolute codon count f(XYZ) of the given codon within this gene collection.
        ''' </summary>
        ''' <param name="codon"></param>
        ''' <returns></returns>
        Public Function CodonCount(codon As Codon) As Integer
            Dim b1 As Integer = NucleicAcid.BaseIndex(codon.X)
            Dim b2 As Integer = NucleicAcid.BaseIndex(codon.Y)
            Dim b3 As Integer = NucleicAcid.BaseIndex(codon.Z)

            If b1 < 0 OrElse b2 < 0 OrElse b3 < 0 Then
                Return 0
            End If

            Return codonCounts((b1 * 4 + b2) * 4 + b3)
        End Function

        ''' <summary>
        ''' The site-specific codon signature odds ratio:
        ''' 
        ''' ```
        '''    rhoXY(i, j) = fXY(i, j) / (fX(i) * fY(j))
        ''' ```
        ''' </summary>
        ''' <param name="site">which dinucleotide position combination, see <see cref="CodonSites"/></param>
        ''' <param name="X"></param>
        ''' <param name="Y"></param>
        ''' <returns></returns>
        Public Function Signature(site As CodonSites, X As DNA, Y As DNA) As Double
            Dim i As Integer = NucleicAcid.BaseIndex(X)
            Dim j As Integer = NucleicAcid.BaseIndex(Y)

            If i < 0 OrElse j < 0 Then
                Return 0
            End If

            Select Case site
                Case CodonSites.Site12
                    If codonTotal < 2 Then Return 0
                    Return (dimer12(i, j) / codonTotal) / ((site1(i) / codonTotal) * (site2(j) / codonTotal))
                Case CodonSites.Site23
                    If codonTotal < 2 Then Return 0
                    Return (dimer23(i, j) / codonTotal) / ((site2(i) / codonTotal) * (site3(j) / codonTotal))
                Case CodonSites.Site13
                    If codonTotal < 2 Then Return 0
                    Return (dimer13(i, j) / codonTotal) / ((site1(i) / codonTotal) * (site3(j) / codonTotal))
                Case CodonSites.Site34
                    If junctionTotal < 2 Then Return 0
                    Return (dimer34(i, j) / junctionTotal) / ((site3(i) / codonTotal) * (site1(j) / codonTotal))
                Case Else
                    Return 0
            End Select
        End Function

        ''' <summary>
        ''' The 16-dim codon signature vector ``{rhoXY(i, j)}`` in the canonical 
        ''' <see cref="NucleicAcid.DimerOrder"/>.
        ''' </summary>
        ''' <param name="site"></param>
        ''' <returns></returns>
        Public Function SignatureVector(site As CodonSites) As Double()
            Dim cache As Double() = signatureCache(CInt(site))

            If cache Is Nothing Then
                Dim vec As Double() = New Double(15) {}
                Dim order As (X As DNA, Y As DNA)() = NucleicAcid.DimerOrder

                For k As Integer = 0 To 15
                    vec(k) = Signature(site, order(k).X, order(k).Y)
                Next

                signatureCache(CInt(site)) = vec
                cache = vec
            End If

            Return cache
        End Function

        ''' <summary>
        ''' The per-codon signature triple {rhoXY(1,2), rhoYZ(2,3), rhoXZ(1,3)} 
        ''' of the given codon.
        ''' </summary>
        ''' <param name="codon"></param>
        ''' <returns></returns>
        Public Function CodonBiasVector(codon As Codon) As CodonBiasVector
            Return New CodonBiasVector With {
                .Codon = codon.CodonValue,
                .XY = Signature(CodonSites.Site12, codon.X, codon.Y),
                .YZ = Signature(CodonSites.Site23, codon.Y, codon.Z),
                .XZ = Signature(CodonSites.Site13, codon.X, codon.Z)
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"{Name}: {codonTotal} codons, {junctionTotal} junctions"
        End Function
    End Class

    ''' <summary>
    ''' The codon usage profile of a gene collection (the relative codon frequencies 
    ''' and amino acid frequencies). This is the ``c(x, y, z)`` reference class data 
    ''' used by the B(F|C) bias measure (see <see cref="CodonBiasMeasure"/>) and the 
    ''' ``f^H(codon)`` reference set of the CAI (see <see cref="CAI.CodonWeightTable"/>).
    ''' </summary>
    Public Class CodonUsageProfile

        ''' <summary>
        ''' the display name of the gene collection
        ''' </summary>
        Public Property Name As String

        Friend ReadOnly counts As Double() = New Double(63) {}
        Friend ReadOnly code As GeneticCodes
        Friend ReadOnly codonHash As Codon() = Codon.CreateHashTable
        Friend totalCodonCount As Integer

        Sub New(Optional code As GeneticCodes = GeneticCodes.StandardCode,
                Optional name As String = Nothing)
            Me.code = code
            Me.Name = If(name, "codon-usage")
        End Sub

        ''' <summary>
        ''' Total number of counted codons.
        ''' </summary>
        Public ReadOnly Property TotalCodons As Integer
            Get
                Return totalCodonCount
            End Get
        End Property

        ''' <summary>
        ''' Add all complete codons of the given CDS gene sequence into the profile.
        ''' Stop codons can be excluded (default, the paper counts sense codons).
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <param name="excludeStopCodons"></param>
        Public Sub AddGene(gene As DNA(), Optional excludeStopCodons As Boolean = True)
            Dim stopCodons As Integer() = If(excludeStopCodons, TranslTable.GetTable(code).StopCodons, Nothing)
            Dim n As Integer = gene.Length - gene.Length Mod 3

            For i As Integer = 0 To n - 1 Step 3
                Dim b1 As Integer = NucleicAcid.BaseIndex(gene(i))
                Dim b2 As Integer = NucleicAcid.BaseIndex(gene(i + 1))
                Dim b3 As Integer = NucleicAcid.BaseIndex(gene(i + 2))

                If b1 < 0 OrElse b2 < 0 OrElse b3 < 0 Then
                    Continue For
                End If

                Dim idx As Integer = (b1 * 4 + b2) * 4 + b3

                If stopCodons IsNot Nothing Then
                    ' codon hash code: X * 1000 + Y * 100 + Z * 10000 
                    ' (DNA enum values: dAMP=1, dGMP=2, dCMP=3, dTMP=4)
                    Dim hashCode As Integer = (b1 + 1) * 1000 + (b2 + 1) * 100 + (b3 + 1) * 10000

                    If Array.IndexOf(stopCodons, hashCode) > -1 Then
                        Continue For
                    End If
                End If

                counts(idx) += 1
                totalCodonCount += 1
            Next
        End Sub

        ''' <summary>
        ''' The packed codon index of the given codon object 
        ''' (``((b1*4 + b2)*4 + b3)`` with base index A=0, G=1, C=2, T=3).
        ''' </summary>
        Public Shared Function CodonIndex(codon As Codon) As Integer
            Dim b1 As Integer = NucleicAcid.BaseIndex(codon.X)
            Dim b2 As Integer = NucleicAcid.BaseIndex(codon.Y)
            Dim b3 As Integer = NucleicAcid.BaseIndex(codon.Z)

            If b1 < 0 OrElse b2 < 0 OrElse b3 < 0 Then
                Return -1
            Else
                Return (b1 * 4 + b2) * 4 + b3
            End If
        End Function

        ''' <summary>
        ''' The relative codon frequency c(x, y, z) = count / total.
        ''' </summary>
        ''' <param name="codon"></param>
        ''' <returns></returns>
        Public Function Relative(codon As Codon) As Double
            Dim idx As Integer = CodonIndex(codon)

            If idx < 0 OrElse totalCodonCount = 0 Then
                Return 0
            Else
                Return counts(idx) / totalCodonCount
            End If
        End Function

        ''' <summary>
        ''' The absolute codon count.
        ''' </summary>
        ''' <param name="codon"></param>
        ''' <returns></returns>
        Public Function Count(codon As Codon) As Double
            Dim idx As Integer = CodonIndex(codon)

            If idx < 0 Then
                Return 0
            Else
                Return counts(idx)
            End If
        End Function

        ''' <summary>
        ''' The amino acid frequency p_a = (count of codons encoding a) / totalCodons.
        ''' Stop codons are excluded from the total when the profile was built 
        ''' with ``excludeStopCodons:=True``.
        ''' </summary>
        ''' <param name="aminoAcid"></param>
        ''' <returns></returns>
        Public Function AminoAcidFrequency(aminoAcid As Char) As Double
            If totalCodonCount = 0 Then
                Return 0
            End If

            Dim table As TranslTable = TranslTable.GetTable(code)
            Dim sum As Double = 0

            For Each codon As Codon In codonHash
                If Array.IndexOf(table.StopCodons, codon.TranslHashCode) = -1 Then
                    If TranslationTable.Translate(codon, code) = aminoAcid Then
                        Dim idx As Integer = CodonIndex(codon)

                        If idx >= 0 Then
                            sum += counts(idx)
                        End If
                    End If
                End If
            Next

            Return sum / totalCodonCount
        End Function

        ''' <summary>
        ''' All sense codons that encode the given amino acid under the genetic code 
        ''' of this profile.
        ''' </summary>
        ''' <param name="aminoAcid"></param>
        ''' <returns></returns>
        Public Function CodonsForAA(aminoAcid As Char) As Codon()
            Dim table As TranslTable = TranslTable.GetTable(code)

            Return codonHash _
                .Where(Function(c)
                           Return Array.IndexOf(table.StopCodons, c.TranslHashCode) = -1 AndAlso
                               TranslationTable.Translate(c, code) = aminoAcid
                       End Function) _
                .ToArray
        End Function

        ''' <summary>
        ''' Build the codon usage profile from a collection of CDS gene sequences.
        ''' </summary>
        ''' <param name="genes"></param>
        ''' <param name="code"></param>
        ''' <param name="name"></param>
        ''' <param name="excludeStopCodons"></param>
        ''' <returns></returns>
        Public Shared Function FromGenes(genes As IEnumerable(Of FastaSeq),
                                         Optional code As GeneticCodes = GeneticCodes.StandardCode,
                                         Optional name As String = Nothing,
                                         Optional excludeStopCodons As Boolean = True) As CodonUsageProfile
            Dim profile As New CodonUsageProfile(code, name)

            For Each gene As FastaSeq In genes
                Call profile.AddGene(New NucleotideModels.NucleicAcid(gene, strict:=False).ToArray, excludeStopCodons)
            Next

            Return profile
        End Function

        Public Overrides Function ToString() As String
            Return $"{Name}: {totalCodonCount} codons"
        End Function
    End Class
End Namespace
