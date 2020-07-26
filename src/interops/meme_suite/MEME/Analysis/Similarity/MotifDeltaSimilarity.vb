#Region "Microsoft.VisualBasic::08b801499434c3613b3a1976da2a868e, meme_suite\MEME\Analysis\Similarity\MotifDeltaSimilarity.vb"

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

    '     Module MotifDeltaSimilarity
    ' 
    '         Function: __BIAS, (+2 Overloads) __counts, __createObject, DinucleotideBIAS, (+2 Overloads) MergeSubsamples
    '                   MotifDiff, (+3 Overloads) PWM, SaveCompareResult, (+2 Overloads) Sigma
    '         Class MotifCompares
    ' 
    '             Properties: Delta, Motif1, Motif2
    ' 
    '             Function: GetMotif
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.ListExtensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.LDM
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Analysis.Similarity

    <Package("Motif.Delta.Similarity",
                    Description:="Calculate the motif similarity using the nucleotide sequence delta similarity method.",
                    Publisher:="xie.guigang@gcmodeller.org")>
    Public Module MotifDeltaSimilarity

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="source">Subsample经过MEME计算所导出的文件夹</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Merge.SubSamples.MEME_Out")>
        Public Function MergeSubsamples(source As String, Optional delta_threshold As Double = 0.5) As Motif()
            Dim setValue = New SetValue(Of Motif)() <= NameOf(Motif.Id)
            Dim resources As List(Of Motif) =
                LinqAPI.MakeList(Of Motif) <= From path
                                              In FileIO.FileSystem.GetFiles(source, FileIO.SearchOption.SearchTopLevelOnly, "*.txt").LoadSourceEntryList.AsParallel
                                              Let MotifData = DocumentFormat.MEME.Text.Load(path.Value)
                                              Where Not MotifData.IsNullOrEmpty
                                              Select From i As SeqValue(Of Motif)
                                                     In MotifData.SeqIterator
                                                     Select setValue(i.value, path.Key & "." & i.i)

            Dim Grouped = (From item In resources Select item Group item By item.Signature Into Group).ToArray
            Dim LQuery = (From item In Grouped.AsParallel() Let InternalCreateObject As Motif = __createObject(item.Signature, item.Group.ToArray) Select InternalCreateObject
                          Order By InternalCreateObject.Sites.Count Descending).AsList
            Dim ChunkBuffer As List(Of Motif()) = New List(Of Motif())  '按照相似性分组

            Do While LQuery.Count > 0
                Dim Ref = LQuery.First
                Dim cl = (From n In LQuery.Skip(1).AsParallel Let delta As Double = MotifDeltaSimilarity.Sigma(Ref, n) Where delta <= delta_threshold Select delta, n).AsList
                Dim ls = (From item In cl Select item.n).AsList
                Call ls.Add(Ref)
                Call ChunkBuffer.Add(ls.ToArray)

                For Each item In ls
                    Call LQuery.Remove(item)
                Next
            Loop

            LQuery = (From item As Motif()
                  In ChunkBuffer.AsParallel
                      Select Motif = __createObject((From nn In item Select s = MotifPM.ToString(nn.PspMatrix) Order By Len(s) Ascending).First, item)
                      Order By Motif.Sites.Count Descending).AsList
            Return LQuery.ToArray
        End Function

        Private Function __createObject(Signature As String, dat As Motif()) As Motif
            Dim Motif As Motif = New Motif With {.Signature = Signature}
            Motif.Sites = (From m In dat Select m.Sites).ToArray.Unlist.Distinct.ToArray
            Motif.Width = dat.First.Width
            Motif.PspMatrix = MotifPM.CreateObject((From nn In dat Select nn.PspMatrix).ToArray)

            Return Motif
        End Function

        <ExportAPI("Motif.Clustering")>
        Public Function MergeSubsamples(source As IEnumerable(Of Motif), Optional delta_threshold As Double = 0.5) As Motif()
            Dim resources = source.AsList
            Dim Grouped = (From item In resources Select item Group item By item.Signature Into Group).ToArray
            Dim LQuery = (From item In Grouped.AsParallel() Let InternalCreateObject As Motif = __createObject(item.Signature, item.Group.ToArray) Select InternalCreateObject
                          Order By InternalCreateObject.Sites.Count Descending).AsList
            Dim ChunkBuffer As List(Of Motif()) = New List(Of Motif())  '按照相似性分组

            Do While LQuery.Count > 0
                Dim Ref = LQuery.First
                Dim cl = (From n In LQuery.Skip(1).AsParallel Let delta As Double = MotifDeltaSimilarity.Sigma(Ref, n) Where delta <= delta_threshold Select delta, n).AsList
                Dim ls = (From item In cl Select item.n).AsList
                Call ls.Add(Ref)
                Call ChunkBuffer.Add(ls.ToArray)

                For Each item In ls
                    Call LQuery.Remove(item)
                Next
            Loop

            LQuery = (From item As Motif()
                  In ChunkBuffer.AsParallel
                      Select Motif = __createObject((From nn In item Select s = MotifPM.ToString(nn.PspMatrix) Order By Len(s) Ascending).First, item)
                      Order By Motif.Sites.Count Descending).AsList
            Return LQuery.ToArray
        End Function

        <ExportAPI("PWM")>
        Public Function PWM(sequence As String) As MotifPM()
            Dim NT As New NucleicAcid(sequence)
            Return PWM(NT)
        End Function

        <ExportAPI("PWM")>
        Public Function PWM(sequence As NucleicAcid) As MotifPM()
            Dim LQuery = (From nt As DNA
                          In sequence.ToArray
                          Select MotifPM.CreateFromNtBase(nt)).ToArray
            Return LQuery
        End Function

        <ExportAPI("PWM")>
        Public Function PWM(nt As IEnumerable(Of DNA)) As MotifPM()
            Dim LQuery = (From ntbase In nt Select MotifPM.CreateFromNtBase(ntbase)).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' A measure of difference between two sequences f and g (from different organisms or from different regions of the same genome) 
        ''' is the average absolute dinucleotide relative abundance difference calculated as
        '''
        ''' sigma(f, g) = (1/16)*∑|pXY(f)-pXY(g)|
        ''' 
        ''' where the sum extends over all dinucleotides (abbreviated sigma-differences).
        ''' </summary>
        ''' <param name="f"></param>
        ''' <param name="g"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Motif.Delta", Info:="A measure of difference between two sequences f and g (from different organisms or from different regions of the same genome) 
is the average absolute dinucleotide relative abundance difference calculated as
<br /><br />
<li>sigma(f, g) = (1/16)*∑|pXY(f)-pXY(g)|
<br /><br />
where the sum extends over all dinucleotides (abbreviated sigma-differences).")>
        Public Function Sigma(f As Motif, g As Motif) As Double
            Return Sigma(f.PspMatrix, g.PspMatrix)
        End Function

        ''' <summary>
        ''' A measure of difference between two sequences f and g (from different organisms or from different regions of the same genome) 
        ''' is the average absolute dinucleotide relative abundance difference calculated as
        '''
        ''' sigma(f, g) = (1/16)*∑|pXY(f)-pXY(g)|
        ''' 
        ''' where the sum extends over all dinucleotides (abbreviated sigma-differences).
        ''' </summary>
        ''' <param name="f"></param>
        ''' <param name="g"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Motif.Delta", Info:="A measure of difference between two sequences f and g (from different organisms or from different regions of the same genome) 
is the average absolute dinucleotide relative abundance difference calculated as
<br /><br />
<li>sigma(f, g) = (1/16)*∑|pXY(f)-pXY(g)|
<br /><br />
where the sum extends over all dinucleotides (abbreviated sigma-differences).")>
        Public Function Sigma(f As MotifPM(), g As MotifPM()) As Double
            Dim sum As Double

            sum += __BIAS(f, g, DNA.dAMP, DNA.dAMP) +
                __BIAS(f, g, DNA.dAMP, DNA.dCMP) +
                __BIAS(f, g, DNA.dAMP, DNA.dGMP) +
                __BIAS(f, g, DNA.dAMP, DNA.dTMP)

            sum += __BIAS(f, g, DNA.dCMP, DNA.dAMP) +
                __BIAS(f, g, DNA.dCMP, DNA.dCMP) +
                __BIAS(f, g, DNA.dCMP, DNA.dGMP) +
                __BIAS(f, g, DNA.dCMP, DNA.dTMP)

            sum += __BIAS(f, g, DNA.dGMP, DNA.dAMP) +
                __BIAS(f, g, DNA.dGMP, DNA.dCMP) +
                __BIAS(f, g, DNA.dGMP, DNA.dGMP) +
                __BIAS(f, g, DNA.dGMP, DNA.dTMP)

            sum += __BIAS(f, g, DNA.dTMP, DNA.dAMP) +
                __BIAS(f, g, DNA.dTMP, DNA.dCMP) +
                __BIAS(f, g, DNA.dTMP, DNA.dGMP) +
                __BIAS(f, g, DNA.dTMP, DNA.dTMP)

            sum = sum / 16

            Return sum
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="a"><paramref name="a"></paramref>和<paramref name="b"></paramref>都是MEME的文本结果文件的文件路径</param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("MEME.Diff")> Public Function MotifDiff(<Parameter("Path.A")> a As String, <Parameter("Path.b")> b As String) As MotifCompares
            Dim Mtf = DocumentFormat.MEME.Text.Load(a), Mtg = DocumentFormat.MEME.Text.Load(b)
            Dim LQuery = (From Mta As Motif In Mtf.AsParallel Select (From Mtb In Mtg Select Motif1 = Mta, Motif2 = Mtb, delta = Sigma(Mta, Mtb))).ToVector
            Dim Result As MotifCompares = New MotifCompares With {
                .Motif1 = Mtf,
                .Motif2 = Mtg,
                .Delta = (From compares In LQuery
                          Select New NamedTuple(Of String) With {
                              .Name = compares.delta,
                              .Item1 = compares.Motif1.Id,
                              .Item2 = compares.Motif2.Id}).ToArray
            }
            Return Result
        End Function

        <ExportAPI("Write.Xml.Compares")>
        Public Function SaveCompareResult(result As MotifCompares, <Parameter("Path.Save")> SaveTo As String) As Boolean
            Return result.GetXml.SaveTo(SaveTo)
        End Function

        <XmlType("Motif.Delta.Compares", Namespace:="http://code.google.com/p/genome-in-code/meme/motif.matrix")>
        Public Class MotifCompares

            Public Property Motif1 As Motif()
            Public Property Motif2 As Motif()
            Public Property Delta As NamedTuple(Of String)()

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="Entry"><see cref="Delta"></see>之中的一个元素</param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function GetMotif(Entry As NamedTuple(Of String), ByRef Motif1 As Motif, ByRef Motif2 As Motif) As Boolean
                Motif1 = (From item In Me.Motif1 Where String.Equals(item.Id, Entry.Item1) Select item).First
                Motif2 = (From item In Me.Motif2 Where String.Equals(item.Id, Entry.Item2) Select item).First
                Return True
            End Function
        End Class

        Private Function __BIAS(f As MotifPM(), g As MotifPM(), X As DNA, Y As DNA) As Double
            Return Math.Abs(DinucleotideBIAS(f, X, Y) - DinucleotideBIAS(g, X, Y))
        End Function

        ''' <summary>
        ''' Dinucleotide relative abundance values (dinucleotide bias) are assessed through the odds ratio p(XY) = f(XY)/f(X)f(Y), 
        ''' where fX denotes the frequency of the nucleotide X and fXY is the frequency of the dinucleotide XY in the sequence under study.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Motif.Dinucleotide.BIAS", Info:="Dinucleotide relative abundance values (dinucleotide bias) are assessed through the odds ratio p(XY) = f(XY)/f(X)f(Y), 
where fX denotes the frequency of the nucleotide X and fXY is the frequency of the dinucleotide XY in the sequence under study.")>
        Public Function DinucleotideBIAS(Motif As MotifPM(), X As DNA, Y As DNA) As Double
            Dim Len As Integer = Motif.Length
            Dim diBias As Double = __counts(Motif, {X, Y}) / (Len - 1)
            Dim fx As Double = __counts(Motif, X) / Len, fy = __counts(Motif, Y) / Len
            Dim value As Double = diBias / (fx * fy)

            Return value
        End Function

        ''' <summary>
        ''' 计算某一种碱基在序列之中的出现频率
        ''' </summary>
        ''' <param name="X"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __counts(Motif As MotifPM(), X As DNA) As Double
            Dim GetValue As MotifPM.GetValue = MotifPM.GetValueMethods(X)
            Dim LQuery = (From n As MotifPM In Motif Select GetValue(n)).ToArray.Sum
            Return LQuery
        End Function

        ''' <summary>
        ''' 计算某些连续的碱基片段在序列之中的出现频率
        ''' </summary>
        ''' <param name="ns"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __counts(Motif As MotifPM(), ns As DNA()) As Double
            Dim nl As Integer = ns.Length
            Dim SlideWindows = Motif.CreateSlideWindows(nl)
            Dim LQuery = (From n In SlideWindows
                          Let compare = (From i As Integer In ns.Sequence
                                         Let nn = n.Items(i).MostProperly
                                         Where ns(i) = nn.Key
                                         Select nn.Value).ToArray
                          Where compare.Length = nl
                          Select compare.Sum).Sum
            Return LQuery
        End Function
    End Module

End Namespace
