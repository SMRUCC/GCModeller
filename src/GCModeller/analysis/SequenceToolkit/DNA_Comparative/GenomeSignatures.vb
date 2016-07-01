#Region "Microsoft.VisualBasic::8d7a0d4ce9861bbb6a923d0eb58369b8, ..\GCModeller\analysis\SequenceToolkit\DNA_Comparative\GenomeSignatures.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' 在本模块之中，所有的计算过程都是基于<see cref="NucleotideModels.NucleicAcid"></see>核酸对象的
''' </summary>
''' <remarks></remarks>
''' 
<PackageNamespace("Genome.Signatures")>
Public Module GenomeSignatures

    ''' <summary>
    ''' Dinucleotide relative abundance values (dinucleotide bias) are assessed through the odds ratio ``p(XY) = f(XY)/f(X)f(Y)``, 
    ''' where ``fX`` denotes the frequency of the nucleotide ``X`` and ``fXY`` is the frequency of the dinucleotide ``XY`` in the 
    ''' sequence under study.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Dinucleotide.BIAS",
               Info:="Dinucleotide relative abundance values (dinucleotide bias) are assessed through the odds ratio p(XY) = f(XY)/f(X)f(Y), 
where fX denotes the frequency of the nucleotide X and fXY is the frequency of the dinucleotide XY in the sequence under study.")>
    Public Function DinucleotideBIAS(Sequence As NucleotideModels.NucleicAcid, X As DNA, Y As DNA) As Double
        Dim Len As Integer = Sequence.Length
        Dim dibias As Double = __counts(Sequence, {X, Y}) / (Len - 1)
        Dim fx As Double = __counts(Sequence, X) / Len, fy = __counts(Sequence, Y) / Len
        Dim value As Double = dibias / (fx * fy)

        Return value
    End Function

    ''' <summary>
    ''' 计算某一种碱基在序列之中的出现频率
    ''' </summary>
    ''' <param name="Sequence"></param>
    ''' <param name="X"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function __counts(Sequence As NucleotideModels.NucleicAcid, X As DNA) As Integer
        Dim LQuery As Integer = (From n As DNA In Sequence Where n = X Select 1).Count
        Return LQuery
    End Function

    ''' <summary>
    ''' 计算某些连续的碱基片段在序列之中的出现频率
    ''' </summary>
    ''' <param name="Sequence"></param>
    ''' <param name="ns"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function __counts(Sequence As NucleotideModels.NucleicAcid, ns As DNA()) As Integer
        Dim nl As Integer = ns.Length
        Dim SlideWindows = Sequence.CreateSlideWindows(slideWindowSize:=nl)
        Dim LQuery As Integer = (From n As SlideWindowHandle(Of DNA)
                                 In SlideWindows
                                 Where (From i As Integer
                                        In ns.Sequence
                                        Where ns(i) = n.Elements(i)
                                        Select 1).Count = nl
                                 Select 1).Count
        Return LQuery
    End Function

    ''' <summary>
    ''' Dinucleotide relative abundance values (dinucleotide bias) are assessed through the 
    ''' odds ratio ``p(XY) = f(XY)/f(X)f(Y)``, where fX denotes the frequency of 
    ''' the nucleotide X and fXY is the frequency of the dinucleotide XY in the 
    ''' sequence under study.(并行版本)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Dinucleotide.BIAS.Parallel",
               Info:="Dinucleotide relative abundance values (dinucleotide bias) are assessed through the odds ratio p(XY) = f(XY)/f(X)f(Y), 
where fX denotes the frequency of the nucleotide X and fXY is the frequency of the dinucleotide XY in the sequence under study.")>
    Public Function DinucleotideBIAS_p(Sequence As NucleotideModels.NucleicAcid, X As DNA, Y As DNA) As Double
        Dim Len As Integer = Sequence.Length
        Dim dibias As Double = __counts_p(Sequence, {X, Y}) / (Len - 1)
        Dim fx As Double = __counts(Sequence, X) / Len, fy = __counts(Sequence, Y) / Len
        Dim value As Double = dibias / (fx * fy)

        Return value
    End Function

    ''' <summary>
    ''' 计算某些连续的碱基片段在序列之中的出现频率(并行版本)
    ''' </summary>
    ''' <param name="Sequence"></param>
    ''' <param name="ns"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function __counts_p(Sequence As NucleotideModels.NucleicAcid, ns As DNA()) As Integer
#Const DEBUG = 1

#If DEBUG Then
        Dim nnn As Integer

        Try
#End If
            Dim nl As Integer = ns.Length
            Dim SlideWindows = Sequence.CreateSlideWindows(slideWindowSize:=nl)
#If DEBUG Then
            nnn = SlideWindows.Last.Elements.Length
#End If
            Dim LQuery As Integer = (From n As SlideWindowHandle(Of DNA)
                                     In SlideWindows.AsParallel
                                     Where (From i As Integer
                                            In ns.Sequence
                                            Where ns(i) = n.Elements(i)
                                            Select 1).Count = nl
                                     Select 1).Count
            Return LQuery
#If DEBUG Then
        Catch ex As Exception
            Dim msg As String = $"ns_length is {ns.Length}, The last_slidewindow_length is {nnn}, sequence_length for create the slidewindows is {Sequence.ToArray.Length}"
            ex = New Exception(msg, ex)
            Throw ex
        End Try
#End If
    End Function

    ''' <summary>
    ''' CODON SIGNATURE
    ''' 
    ''' For a given collection of genes, let fX(1); fY(2); fZ(3) denote frequencies of the indicated nucleotide at codon sites 1, 2, and 3, respectively, 
    ''' and let f(XYZ) indicate codon frequency. The embedded dinucleotide frequencies are denoted fXY(1, 2); fYZ(2, 3); and fXZ(1, 3). Dinucleotide 
    ''' contrasts are assessed through the odds ratio pXY = f(XY)/f(X)f(Y). 
    ''' In the context of codons, we define
    ''' 
    ''' ```
    '''    pXY(1, 2) = fXY(1, 2)/fX(1)fY(2)
    '''    pYZ(2, 3) = fYZ(2, 3)/fY(2)fZ(3)
    '''    pXZ(1, 3) = fXZ(1, 3)/fX(1)fZ(3)
    ''' ```
    ''' 
    ''' We refer to the profiles {pXY(1, 2)}; {pXZ(1, 3)}; {pYZ(2, 3)}, and also {pZW(3, 4)}, where 4(=1) is the first position of the next codon, as the 
    ''' codon signature to be distinguished from the global genome signature
    ''' </summary>
    ''' <param name="Sequence"></param>
    ''' <param name="Codon"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Codon.Signature",
             Info:="CODON SIGNATURE
             <br />
 <p>For a given collection of genes, let fX(1); fY(2); fZ(3) denote frequencies of the indicated nucleotide at codon sites 1, 2, and 3, respectively, 
 and let f(XYZ) indicate codon frequency. The embedded dinucleotide frequencies are denoted fXY(1, 2); fYZ(2, 3); and fXZ(1, 3). Dinucleotide 
 contrasts are assessed through the odds ratio pXY = f(XY)/f(X)f(Y). 
 In the context of codons, we define
 
<li>    pXY(1, 2) = fXY(1, 2)/fX(1)fY(2)
<li>    pYZ(2, 3) = fYZ(2, 3)/fY(2)fZ(3)
<li>    pXZ(1, 3) = fXZ(1, 3)/fX(1)fZ(3)
 
<p> We refer to the profiles {pXY(1, 2)}; {pXZ(1, 3)}; {pYZ(2, 3)}, and also {pZW(3, 4)}, where 4(=1) is the first position of the next codon, as the 
 codon signature to be distinguished from the global genome signature")>
    Public Function CodonSignature(Sequence As NucleotideModels.NucleicAcid, Codon As Codon) As TripleKeyValuesPair(Of Double)
        Dim Value As New TripleKeyValuesPair(Of Double) With {
            .Value1 = GenomeSignatures.DinucleotideBIAS(Sequence, X:=Codon.X, Y:=Codon.Y),
            .Value2 = GenomeSignatures.DinucleotideBIAS(Sequence, X:=Codon.Y, Y:=Codon.Z),
            .Value3 = GenomeSignatures.DinucleotideBIAS(Sequence, X:=Codon.X, Y:=Codon.Z)
        }
        Return Value
    End Function
End Module

