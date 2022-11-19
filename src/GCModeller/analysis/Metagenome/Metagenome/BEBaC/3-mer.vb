#Region "Microsoft.VisualBasic::9fb4d55f9841e8d079ef9e5cbbe5d595, GCModeller\analysis\Metagenome\Metagenome\BEBaC\3-mer.vb"

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

    '   Total Lines: 187
    '    Code Lines: 129
    ' Comment Lines: 27
    '   Blank Lines: 31
    '     File Size: 4.91 KB


    '     Enum I3Mers
    ' 
    '         AAA, AAC, AAG, AAT, ACA
    '         ACC, ACG, ACT, AGA, AGC
    '         AGG, AGT, ATA, ATC, ATG
    '         ATT, CAA, CAC, CAG, CAT
    '         CCA, CCC, CCG, CCT, CGA
    '         CGC, CGG, CGT, CTA, CTC
    '         CTG, CTT, GAA, GAC, GAG
    '         GAT, GCA, GCC, GCG, GCT
    '         GGA, GGC, GGG, GGT, GTA
    '         GTC, GTG, GTT, TAA, TAC
    '         TAG, TAT, TCA, TCC, TCG
    '         TCT, TGA, TGC, TGG, TGT
    '         TTA, TTC, TTG, TTT
    ' 
    '  
    ' 
    ' 
    ' 
    '     Module VectorAPI
    ' 
    '         Properties: I3Mersx
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Count, GetVector, Transform
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
                Dim n As Integer = seq.SequenceData.Length - 2  ' 由于计算是和划窗类似的，所以总数目和序列的长度基本一致
                Dim vec As Dictionary(Of I3Mers, Integer) = seq.GetVector
                Dim f As Dictionary(Of I3Mers, Double) =
                    vec.ToDictionary(Function(o) o.Key,
                                     Function(o) o.Value / n)

                Yield New I3merVector With {
                    .Name = getTag(seq),
                    .Vector = vec,
                    .Frequency = f
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
        Public Function GetVector(seq As IPolymerSequenceModel) As Dictionary(Of I3Mers, Integer)
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
