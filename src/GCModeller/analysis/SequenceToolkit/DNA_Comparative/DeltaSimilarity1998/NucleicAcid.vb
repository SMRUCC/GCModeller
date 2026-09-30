Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' Genome signature calculation cache based on the Karlin, Campbell &amp; Mrazek (1998) 
    ''' ``Comparative DNA Analysis Across Diverse Genomes`` paper.
    ''' 
    ''' In contrast to the legacy implementation, this class no longer creates one 
    ''' ``SlideWindow(Of DNA)`` object for each adjacent nucleotide position (which 
    ''' generates millions of heap objects on whole genome scale data). Instead, 
    ''' a single pass O(n) scan fills the integer count matrix:
    ''' 
    ''' 1. ``monomer(4)``     - counts of A/G/C/T
    ''' 2. ``dimer(4,4)``     - counts of the adjacent dinucleotides XY
    ''' 
    ''' The symmetrized (double-stranded) counts which is required by the paper's 
    ''' genome signature ``rho*`` are derived on the fly: since the reverse complement 
    ''' of the sequence contributes the dinucleotide pair ``comp(Y)comp(X)`` for each 
    ''' forward pair ``XY``, we have:
    ''' 
    ''' ```
    '''    symCount(X, Y) = count(X, Y) + count(comp(Y), comp(X))
    '''    symMono(X)     = count(X) + count(comp(X))
    ''' ```
    ''' 
    ''' Ambiguous bases (R, Y, N, ...) are excluded from the frequency denominators 
    ''' to avoid div/0 and statistical pollution.
    ''' </summary>
    ''' <remarks>
    ''' SIMD applicability analysis (Math/SIMD module): the hot path of this class is a 
    ''' branch-less integer single pass word counting loop over the sequence array, which 
    ''' cannot benefit from the ``System.Numerics.Vector(Of Double)`` oriented SIMD module 
    ''' in the sciBASIC# runtime. SIMD acceleration is applied at the higher level 16-dim 
    ''' ``rho*`` vector operations (see <see cref="DeltaStarDistance"/>).
    ''' </remarks>
    Public Class NucleicAcid

        Friend ReadOnly nt As DNA()

        ''' <summary>
        ''' The fasta header title of the sequence (or user assigned tag).
        ''' </summary>
        Public ReadOnly Property UserTag As String

        ''' <summary>
        ''' the size of current nt sequence.
        ''' </summary>
        Public ReadOnly Property Length As Integer
            Get
                Return nt.Length
            End Get
        End Property

        '   A  G  C  T  => 0  1  2  3   (DNA enum: dAMP=1, dGMP=2, dCMP=3, dTMP=4)
        Friend monomer As Integer() = New Integer(3) {}
        Friend dimer As Integer(,) = New Integer(3, 3) {}
        Friend validBaseCount As Integer
        Friend validPairCount As Integer

        ' lazy cache of the 16-dim symmetrized rho* signature vector
        Friend signatureCache As Double()

#Region "Constructors"

        Sub New(nt As DNA())
            Me.nt = nt
            Me.validBaseCount = CountPass(Me)
            Me.UserTag = ""
        End Sub

        ''' <summary>
        ''' Fasta序列会自动使用<see cref="FastaSeq.Title"/>来作为序列的<see cref="UserTag"/>
        ''' </summary>
        Sub New(nt As FastaSeq)
            Call Me.New(New NucleotideModels.NucleicAcid(nt, strict:=False).ToArray)
            Me.UserTag = nt.Title
        End Sub

        Sub New(nt As String)
            Call Me.New(New NucleotideModels.NucleicAcid(nt).ToArray)
        End Sub

        Sub New(nt As NucleotideModels.NucleicAcid)
            Call Me.New(nt.ToArray)
            Me.UserTag = nt.UserTag
        End Sub

        Sub New(nt As IEnumerable(Of DNA))
            Call Me.New(nt.ToArray)
        End Sub
#End Region

        ''' <summary>
        ''' Single pass O(n) integer counting. No heap allocation at all inside the loop.
        ''' </summary>
        Private Shared Function CountPass(model As NucleicAcid) As Integer
            Dim seq As DNA() = model.nt
            Dim n As Integer = seq.Length
            Dim monomer As Integer() = model.monomer
            Dim dimer As Integer(,) = model.dimer
            Dim validBases As Integer = 0
            Dim validPairs As Integer = 0
            Dim prev As Integer = -1

            For i As Integer = 0 To n - 1
                Dim xi As Integer = BaseIndex(seq(i))

                If xi >= 0 Then
                    monomer(xi) += 1
                    validBases += 1

                    If prev >= 0 Then
                        dimer(prev, xi) += 1
                        validPairs += 1
                    End If

                    prev = xi
                Else
                    ' ambiguous base: break the adjacent dimer chain
                    prev = -1
                End If
            Next

            model.validPairCount = validPairs
            Return validBases
        End Function

        ''' <summary>
        ''' Map the DNA base enum to the index inside the count matrix.
        ''' Only the four canonical bases are counted, ambiguous/degenerated bases
        ''' returns -1. (``A=0, G=1, C=2, T=3``)
        ''' </summary>
        Public Shared Function BaseIndex(base As DNA) As Integer
            Select Case base
                Case DNA.dAMP : Return 0
                Case DNA.dGMP : Return 1
                Case DNA.dCMP : Return 2
                Case DNA.dTMP : Return 3
                Case Else : Return -1
            End Select
        End Function

        ''' <summary>
        ''' Watson-Crick complement base index. (A&lt;-&gt;T, G&lt;-&gt;C)
        ''' </summary>
        Public Shared Function ComplementIndex(index As Integer) As Integer
            Return 3 - index
        End Function

        ''' <summary>
        ''' Watson-Crick complement base. (A&lt;-&gt;T, G&lt;-&gt;C)
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Complement(base As DNA) As DNA
            Select Case base
                Case DNA.dAMP : Return DNA.dTMP
                Case DNA.dTMP : Return DNA.dAMP
                Case DNA.dGMP : Return DNA.dCMP
                Case DNA.dCMP : Return DNA.dGMP
                Case Else : Return base
            End Select
        End Function

        ''' <summary>
        ''' The 16 dinucleotides in the canonical order of the signature vector. 
        ''' (row: X in A/G/C/T, column: Y in A/G/C/T)
        ''' </summary>
        Public Shared ReadOnly Property DimerOrder As (X As DNA, Y As DNA)()
            Get
                Static dimer As (DNA, DNA)() = {
                    (DNA.dAMP, DNA.dAMP), (DNA.dAMP, DNA.dGMP), (DNA.dAMP, DNA.dCMP), (DNA.dAMP, DNA.dTMP),
                    (DNA.dGMP, DNA.dAMP), (DNA.dGMP, DNA.dGMP), (DNA.dGMP, DNA.dCMP), (DNA.dGMP, DNA.dTMP),
                    (DNA.dCMP, DNA.dAMP), (DNA.dCMP, DNA.dGMP), (DNA.dCMP, DNA.dCMP), (DNA.dCMP, DNA.dTMP),
                    (DNA.dTMP, DNA.dAMP), (DNA.dTMP, DNA.dGMP), (DNA.dTMP, DNA.dCMP), (DNA.dTMP, DNA.dTMP)
                }
                Return dimer
            End Get
        End Property

        ''' <summary>
        ''' Reverse complement of the given nucleotide sequence.
        ''' </summary>
        Public Shared Function ReverseComplement(seq As DNA()) As DNA()
            Dim rc As DNA() = New DNA(seq.Length - 1) {}

            For i As Integer = 0 To seq.Length - 1
                rc(i) = Complement(seq(seq.Length - 1 - i))
            Next

            Return rc
        End Function

#Region "Counts"

        ''' <summary>
        ''' The forward strand monomer count of the given canonical base.
        ''' </summary>
        Public Function MonomerCount(base As DNA) As Integer
            Dim i As Integer = BaseIndex(base)

            If i < 0 Then
                Return 0
            Else
                Return monomer(i)
            End If
        End Function

        ''' <summary>
        ''' The forward strand adjacent dinucleotide count XY.
        ''' </summary>
        Public Function DimerCount(X As DNA, Y As DNA) As Integer
            Dim i As Integer = BaseIndex(X)
            Dim j As Integer = BaseIndex(Y)

            If i < 0 OrElse j < 0 Then
                Return 0
            Else
                Return dimer(i, j)
            End If
        End Function

        ''' <summary>
        ''' Number of canonical bases (A/G/C/T) inside the sequence.
        ''' Ambiguous bases are not counted.
        ''' </summary>
        Public ReadOnly Property ValidBases As Integer
            Get
                Return validBaseCount
            End Get
        End Property

        ''' <summary>
        ''' Number of adjacent canonical dinucleotide pairs.
        ''' </summary>
        Public ReadOnly Property ValidPairs As Integer
            Get
                Return validPairCount
            End Get
        End Property
#End Region

#Region "Odds ratio: rho and rho*"

        ''' <summary>
        ''' Dinucleotide relative abundance odds ratio of a single strand:
        ''' 
        ''' ```
        '''    rho(XY) = f(XY) / (f(X) * f(Y))
        ''' ```
        ''' 
        ''' where ``fX`` denotes the frequency of the nucleotide X and ``fXY`` is 
        ''' the frequency of the dinucleotide XY in the sequence under study.
        ''' </summary>
        ''' <remarks>
        ''' This is the non-symmetrized odds ratio. For cross-genome comparisons 
        ''' the paper's genome signature <see cref="RelativeAbundance"/> (``rho*``, 
        ''' double-strand symmetrized) should be used instead.
        ''' </remarks>
        Public Function OddsRatio(X As DNA, Y As DNA) As Double
            Dim i As Integer = BaseIndex(X)
            Dim j As Integer = BaseIndex(Y)

            If i < 0 OrElse j < 0 OrElse validBaseCount < 2 OrElse validPairCount < 1 Then
                Return 0
            End If

            ' f(XY) = count / validPairCount
            ' f(X)  = count / validBaseCount
            ' rho   = f(XY) / (f(X) * f(Y))
            Dim fxy As Double = dimer(i, j) / CDbl(validPairCount)
            Dim fx As Double = monomer(i) / CDbl(validBaseCount)
            Dim fy As Double = monomer(j) / CDbl(validBaseCount)

            If fx = 0 OrElse fy = 0 Then
                Return 0
            End If

            Return fxy / (fx * fy)
        End Function

        ''' <summary>
        ''' Double-strand symmetrized dinucleotide relative abundance (the paper's 
        ''' genome signature ``rho*``). The DNA double helix means the sequence 
        ''' should be evaluated together with its reverse complement, which is 
        ''' equivalent to the symmetrized counts:
        ''' 
        ''' ```
        '''    symCount(X, Y) = count(X, Y) + count(comp(Y), comp(X))
        '''    symMono(X)     = count(X) + count(comp(X))
        ''' ```
        ''' </summary>
        ''' <remarks>
        ''' ``rho*`` removes the overall G+C content effect and therefore remains 
        ''' the purely context-dependent signal that can be compared between 
        ''' genomes of different G+C content.
        ''' </remarks>
        Public Function RelativeAbundance(X As DNA, Y As DNA) As Double
            Dim i As Integer = BaseIndex(X)
            Dim j As Integer = BaseIndex(Y)

            If i < 0 OrElse j < 0 OrElse validBaseCount < 2 OrElse validPairCount < 1 Then
                Return 0
            End If

            ' complement index: A(0)<->T(3), G(1)<->C(2)
            Dim ci As Integer = ComplementIndex(i)
            Dim cj As Integer = ComplementIndex(j)
            Dim symPair As Double = dimer(i, j) + dimer(cj, ci)
            Dim symX As Double = monomer(i) + monomer(ci)
            Dim symY As Double = monomer(j) + monomer(cj)

            ' symMono sums to 2 * validBaseCount and symPair sums to 2 * validPairCount,
            ' the common factor 2 cancels out in the odds ratio.
            If symX = 0 OrElse symY = 0 Then
                Return 0
            End If

            Dim fxy As Double = symPair / CDbl(validPairCount)
            Dim fx As Double = symX / CDbl(validBaseCount)
            Dim fy As Double = symY / CDbl(validBaseCount)

            Return fxy / (fx * fy)
        End Function

        ''' <summary>
        ''' Get value by using a paired of base. 
        ''' (returns the symmetrized genome signature ``rho*``, 
        ''' compat wrapper of <see cref="RelativeAbundance"/>)
        ''' </summary>
        ''' <param name="X"></param>
        ''' <param name="Y"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetValue(X As DNA, Y As DNA) As Double
            Return RelativeAbundance(X, Y)
        End Function

        ''' <summary>
        ''' The 16-dim symmetrized genome signature vector ``{rho*XY}`` in the 
        ''' canonical <see cref="DimerOrder"/>.
        ''' </summary>
        Public Function SignatureVector() As Double()
            If signatureCache Is Nothing Then
                Dim vec As Double() = New Double(15) {}
                Dim order As (X As DNA, Y As DNA)() = DimerOrder

                For i As Integer = 0 To 15
                    vec(i) = RelativeAbundance(order(i).X, order(i).Y)
                Next

                signatureCache = vec
            End If

            Return signatureCache
        End Function

        ''' <summary>
        ''' The six-level significance symbol of the genome signature rho*XY:
        ''' "---" (rho* &lt; 0.50) / "--" (0.50-0.70) / "-" (0.70-0.78) / 
        ''' "+" (1.23-1.30) / "++" (1.30-1.50) / "+++" (rho* &gt; 1.50), 
        ''' empty string for the non-significant neutral range [0.78, 1.23].
        ''' </summary>
        Public Function SignificanceSymbol(X As DNA, Y As DNA) As String
            Return GenomeSignatures.SignificanceSymbol(RelativeAbundance(X, Y))
        End Function
#End Region

        ''' <summary>
        ''' Create slide windows on the given sequence data
        ''' </summary>
        ''' <param name="winSize%"></param>
        ''' <param name="step%"></param>
        ''' <returns></returns>
        Public Iterator Function CreateFragments(winSize%, step%) As IEnumerable(Of NucleicAcid)
            For Each region As SlideWindow(Of DNA) In nt.SlideWindows(winSize, offset:=[step])
                Yield New NucleicAcid(region.Items)
            Next
        End Function

        Public Overrides Function ToString() As String
            Return $"{UserTag} [{Length} nt, {validBaseCount} valid bases]"
        End Function
    End Class
End Namespace
