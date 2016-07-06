
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.SequenceModel

Namespace BEBaC

    ''' <summary>
    ''' Where a ``3-mer`` means 3 consecutive DNA bases ranging from ``AAA`` to ``TTT``. 
    ''' Each element of the ``1 x 64`` vector ``yi=(yi1, yi2, ... , yij, ... yi64)`` 
    ''' represents the count Of its corresponding ``3-mer`` in the given sequence xi. 
    ''' </summary>
    Public Enum I3Mers As Byte
        AAA
        AAG
        AAC
        AAT

        ACA
        ACG
        ACC
        ACT

        AGA
        AGG
        AGC
        AGT

        ATA
        ATT
        ATG
        ATC

        GAA
        GAG
        GAT
        GAC

        GGA
        GGG
        GGT
        GGC

        GTA
        GTT
        GTG
        GTC

        GCA
        GCG
        GCT
        GCC

        CAA
        CAG
        CAT
        CAC

        CGA
        CGG
        CGT
        CGC

        CCA
        CCG
        CCT
        CCC

        CTA
        CTG
        CTC
        CTT

        TAA
        TAG
        TAC
        TAT

        TGA
        TGG
        TGC
        TGT

        TCA
        TCG
        TCT
        TCC

        TTA
        TTG
        TTC
        TTT
    End Enum

    Public Module VectorAPI

        ReadOnly __all3Mer As I3Mers()

        Public ReadOnly Property I3Mersx As I3Mers()
            Get
                Return __all3Mer
            End Get
        End Property

        Sub New()
            __all3Mer = Enums(Of I3Mers)()
        End Sub

        ''' <summary>
        ''' We then transform each sequence ``xi`` To a ``3-mer count vector`` **yi**, 
        ''' Where a ``3-mer`` means 3 consecutive DNA bases ranging from ``AAA'' to ``TTT``. 
        ''' Each element of the ``1 x 64`` vector ``yi=(yi1, yi2, ... , yij, ... yi64)``
        ''' represents the count of its corresponding ``3-mer`` in the given sequence ``xi``. 
        ''' Hence the sequence set ``x(N)`` Is transformed to a ``3-mer`` count set 
        ''' ``y(N)={y1, y2, ... yn}``.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="x"></param>
        ''' <param name="getTag"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function Transform(Of T As ISequenceModel)(
                                          x As IEnumerable(Of T),
                            Optional getTag As Func(Of T, String) = Nothing) _
                                            As IEnumerable(Of I3merVector)

            If getTag Is Nothing Then
                getTag = Function(seq) seq.ToString
            End If

            For Each seq As T In x  ' seq = xi
                Yield New I3merVector With {
                    .Name = getTag(seq),
                    .Vector = seq.GetVector
                }
            Next
        End Function

        ''' <summary>
        ''' We then transform each sequence ``xi`` To a ``3-mer count vector`` **yi**, 
        ''' Where a ``3-mer`` means 3 consecutive DNA bases ranging from ``AAA'' to ``TTT``. 
        ''' Each element of the ``1 x 64`` vector ``yi=(yi1, yi2, ... , yij, ... yi64)``
        ''' represents the count of its corresponding ``3-mer`` in the given sequence ``xi``. 
        ''' Hence the sequence set ``x(N)`` Is transformed to a ``3-mer`` count set 
        ''' ``y(N)={y1, y2, ... yn}``.
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetVector(seq As I_PolymerSequenceModel) As Dictionary(Of I3Mers, Integer)
            Dim hash As New Dictionary(Of I3Mers, Integer)
            Dim nt As String = seq.SequenceData.ToUpper
            Dim n As Integer

            For Each token As I3Mers In __all3Mer
                n = token.Count(nt)
                Call hash.Add(token, n)
            Next

            Return hash
        End Function

        <Extension>
        Public Function Count(base As I3Mers, seq As String) As Integer
            Dim p As Integer
            Dim n As Integer
            Dim t As String = base.ToString

            Do While p >= 0
                p = seq.IndexOf(t, startIndex:=p) + 1
                If p <> 0 Then
                    n += 1
                Else
                    Exit Do
                End If
            Loop

            Return n
        End Function
    End Module
End Namespace