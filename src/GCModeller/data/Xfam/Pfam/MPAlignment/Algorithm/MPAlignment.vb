#Region "Microsoft.VisualBasic::0f0b0347db5c7a885abb087f121a6bca, data\Xfam\Pfam\MPAlignment\Algorithm\MPAlignment.vb"

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

    '     Module Algorithm
    ' 
    '         Function: __createDS, AltEquals, Edits, (+2 Overloads) PfamStringEquals, PositionEquals
    ' 
    '         Sub: __alignDomain
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ComponentModel.Loci
Imports sys = System.Math

Namespace ProteinDomainArchitecture.MPAlignment

    ''' <summary>
    ''' Tools for comparing the protein domain structure similarity.
    ''' </summary>
    <Package("MPAlignment.Algorithm",
                      Publisher:="xie.guigang@gmail.com",
                      Category:=APICategories.ResearchTools,
                      Description:="Tools for comparing the protein domain structure similarity.",
                      Cites:="Motif Parallel Alignment")>
    <Cite(Title:="Protein structure similarity from Principle Component Correlation analysis",
          Abstract:="BACKGROUND: Owing to rapid expansion of protein structure databases in recent years, methods of structure comparison are becoming increasingly effective and important in revealing novel information on functional properties of proteins and their roles in the grand scheme of evolutionary biology.
Currently, the structural similarity between two proteins is measured by the root-mean-square-deviation (RMSD) in their best-superimposed atomic coordinates.
RMSD is the golden rule of measuring structural similarity when the structures are nearly identical; it, however, fails to detect the higher order topological similarities in proteins evolved into different shapes.
We propose new algorithms for extracting geometrical invariants of proteins that can be effectively used to identify homologous protein structures or topologies in order to quantify both close and remote structural similarities.
<p><p>RESULTS: We measure structural similarity between proteins by correlating the principle components of their secondary structure interaction matrix.
In our approach, the Principle Component Correlation (PCC) analysis, a symmetric interaction matrix for a protein structure is constructed with relationship parameters between secondary elements that can take the form of distance, orientation, or other relevant structural invariants.
When using a distance-based construction in the presence or absence of encoded N to C terminal sense, there are strong correlations between the principle components of interaction matrices of structurally or topologically similar proteins.
<p><p>CONCLUSION: The PCC method is extensively tested for protein structures that belong to the same topological class but are significantly different by RMSD measure.
The PCC analysis can also differentiate proteins having similar shapes but different topological arrangements.
Additionally, we demonstrate that when using two independently defined interaction matrices, comparison of their maximum eigenvalues can be highly effective in clustering structurally or topologically similar proteins.
We believe that the PCC analysis of interaction matrix is highly flexible in adopting various structural parameters for protein structure comparison.",
          AuthorAddress:="Center for Neurodegeneration and Repair, Center for Bioinformatics, Harvard Medical School, Boston, MA 02215, USA. zhou@crystal.harvard.edu",
          Authors:="Zhou, X.
Chou, J.
Wong, S. T.",
          DOI:="10.1186/1471-2105-7-40",
          ISSN:="1471-2105 (Electronic)
1471-2105 (Linking)",
          Issue:="",
          Journal:="BMC bioinformatics",
          Keywords:="*Algorithms
Cluster Analysis
Computer Simulation
Crystallography/*methods
Databases, Protein
*Models, Chemical
*Models, Molecular
Models, Statistical
Pattern Recognition, Automated
Principal Component Analysis
Protein Conformation
Proteins/*chemistry/*classification/ultrastructure
Sequence Alignment/methods
Sequence Analysis, Protein/*methods
Statistics as Topic",
          Pages:="40",
          URL:="",
          Volume:=7,
          Year:=2006,
          PubMed:=16436213)>
    Public Module Algorithm

        <Extension> Public Function Edits(align As DomainAlignment()) As String
            Return New String(align.Select(Function(x) If(x.IsMatch, "m"c, "-"c)).ToArray)
        End Function

        ''' <summary>
        ''' 初略的判断蛋白质的结构上面的等价性，等价性越高，则返回的数值越接近于1，反之不相等则返回0甚至小于零 {得分， 相似性}
        ''' </summary>
        ''' <param name="Protein_1"></param>
        ''' <param name="Protein_2"></param>
        ''' <returns></returns>
        ''' <remarks>对于两个Domain之间的序列，其格式为[ABCT](start|ends)</remarks>
        '''
        <ExportAPI("PfamString.Equals", Info:="MPAlignment algorithm")>
        Public Function PfamStringEquals(Protein_1 As PfamString.PfamString, Protein_2 As PfamString.PfamString,
                                         highlyScoringThreshold As Double,
                                         Optional partEquals As Boolean = False) As LevAlign
            Return PfamStringEquals(Protein_1, Protein_2, New DomainEquals(highlyScoringThreshold, partEquals))
        End Function

        <ExportAPI("PfamString.Equals", Info:="MPAlignment algorithm")>
        Public Function PfamStringEquals(Protein_1 As PfamString.PfamString, Protein_2 As PfamString.PfamString, equals As DomainEquals) As LevAlign
            Dim result As LevAlign = New LevAlign(Protein_1, Protein_2, equals)
            Return result
        End Function

        ''' <summary>
        ''' 初略的判断蛋白质的结构上面的等价性，等价性越高，则返回的数值越接近于1，反之不相等则返回0甚至小于零 {得分， 相似性}
        ''' </summary>
        ''' <param name="Protein_1"></param>
        ''' <param name="Protein_2"></param>
        ''' <returns></returns>
        ''' <remarks>对于两个Domain之间的序列，其格式为[ABCT](start|ends)</remarks>
        ''' 
        '''
        <ExportAPI("PfamString.Equals", Info:="MPAlignment algorithm")>
        Public Function AltEquals(Protein_1 As PfamString.PfamString, Protein_2 As PfamString.PfamString, highlyScoringThreshold As Double) As AlignmentOutput
            If Protein_1.PfamString.IsNullOrEmpty OrElse Protein_2.PfamString.IsNullOrEmpty Then  '两个蛋白质没有任何的结构域，则无法做进一步判断是否到底相等
                Return New AlignmentOutput With {
                    .ProteinQuery = Protein_1,
                    .ProteinSbjct = Protein_2,
                    .Score = 0,
                    .FullScore = 1
                }
            End If

            Dim a = __createDS(Protein_1.GetDomainData(True), Protein_1.Length),
                b = __createDS(Protein_2.GetDomainData(True), Protein_2.Length)
            Dim score As Double = Protein_1.Domains.Length + Protein_2.Domains.Length
            Dim a_score As Double = score   '没有任何罚分的最佳结果
            Dim ps As Double() = New Double() {Protein_1.Length, Protein_2.Length}

            Call __alignDomain(a, b)  '经过对齐之后，数目已经完全相等了

            Dim ds As Double = a.Length ^ (1 / a.Length)
            Dim AlignmentOutput As AlignmentOutput = New AlignmentOutput With {
                .ProteinQuery = Protein_1,
                .ProteinSbjct = Protein_2,
                .DeltaScore = ds,
                .LengthDelta = ps.Min / ps.Max
            }
            Dim DomainAlignment As List(Of DomainAlignment) = New List(Of DomainAlignment)

            score = score * AlignmentOutput.LengthDelta

            For i As Integer = 0 To a.Length - 1
                Dim da As DomainDistribution = a(i), db As DomainDistribution = b(i)

                If Not String.Equals(da.DomainId, db.DomainId) OrElse
                    String.Equals(da.DomainId, DomainDistribution.EmptyId) OrElse
                    String.Equals(db.DomainId, DomainDistribution.EmptyId) Then  ' 在这里可能会遇到两个空的DomainId被比对上的情况，两个空的Domain算作没有被比对上

                    Dim dd As Double = -ds
                    score += dd 'Domain can not be aligned  当有结构域不一致的时候，罚分比较重，因为二者的生物学功能这个时候肯定会产生差异。
                    DomainAlignment += New DomainAlignment With {
                        .ProteinQueryDomainDs = da,
                        .ProteinSbjctDomainDs = db,
                        .Score = dd
                    }
                Else
                    Dim _ref_a_score As Double = 0
                    Dim n As DomainAlignment = PositionEquals(da, db, a_score:=_ref_a_score, high_Scoring_thresholds:=highlyScoringThreshold)
                    score += n.Score
                    a_score += _ref_a_score
                    DomainAlignment += n
                End If
            Next

            AlignmentOutput.Score = score
            AlignmentOutput.FullScore = a_score
            AlignmentOutput.AlignmentResult = DomainAlignment.ToArray

            If AlignmentOutput.Score < 0 Then
                AlignmentOutput.Score = 0
            End If

            Return AlignmentOutput
        End Function

        ''' <summary>
        ''' <paramref name="a"></paramref>和<paramref name="b"></paramref>都是已经经过排序的数据
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <remarks>这个比对不是实际的物理位置构成的比较</remarks>
        Private Sub __alignDomain(ByRef a As DomainDistribution(), ByRef b As DomainDistribution())
            Dim la As List(Of DomainDistribution) = a.AsList,
                lb As List(Of DomainDistribution) = b.AsList
            Dim p As Integer
            Dim n As List(Of DomainDistribution) = (From dms As List(Of DomainDistribution)
                                                    In {la, lb}
                                                    Select dms
                                                    Order By dms.Count Descending).First
            Do While p <= n.Count
                If la.Count = p Then
                    Call la.Add(DomainDistribution.EmptyDomain)
                    If la.Count = lb.Count Then  '后面已经没有再需要进行判断的元素了，必须要在这个时候退出循环，否则会死循环
                        Exit Do
                    End If
                End If
                If lb.Count = p Then
                    Call lb.Add(DomainDistribution.EmptyDomain)
                    If la.Count = lb.Count Then  '后面已经没有再需要进行判断的元素了，必须要在这个时候退出循环，否则会死循环
                        Exit Do
                    End If
                End If

                Dim cl = (From item In {la, lb} Select item Order By item.Count Descending).ToArray

                Dim lla = cl.First 'lla永远是元素数目最多的
                Dim llb = cl.Last

                Dim aa As DomainDistribution = lla(p)
                Dim bb As DomainDistribution = llb(p)

                If String.Equals(aa.DomainId, bb.DomainId) Then
                    p += 1
                    Continue Do
                End If

                '当不相等的时候，先假设前面的所有数据都是经过对齐之后相等的
                '则只需要插入新的数据就可以了
                Dim tmp = bb
                bb = (From item In llb.Skip(p) Where String.Equals(aa.DomainId, item.DomainId) Select item).ToArray.FirstOrDefault
                If bb Is Nothing Then 'b之中不存在，则插入一个假设的数据
                    Call llb.Insert(p, DomainDistribution.EmptyDomain)
                Else 'b之中存在，则互换位置
                    Call llb.SwapItem(tmp, bb)
                End If

                p += 1
            Loop

            a = la.ToArray
            b = lb.ToArray
        End Sub

        ''' <summary>
        ''' Domain position specific alignment
        ''' </summary>
        ''' <param name="ds1"></param>
        ''' <param name="ds2"></param>
        ''' <returns></returns>
        ''' <param name="high_Scoring_thresholds">高分比对的计算阈值</param>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Position.Equals")>
        Public Function PositionEquals(ds1 As DomainDistribution, ds2 As DomainDistribution, ByRef a_score As Double, high_Scoring_thresholds As Double) As DomainAlignment
            Dim score As Double
            Dim combinations As Position()() = {ds1.Distribution, ds2.Distribution}.AllCombinations.IteratesALL.ToArray
            Dim high_scores As Integer '高分比对的次数，假若完全匹配上高分比对的时候，则高分比对的次数很明显应该为ds1.count

            score -= Math.Abs(ds1.Distribution.Length - ds2.Distribution.Length) / 2  'Domain number is not equals, so protein function maybe not equals.

            For i As Integer = 0 To combinations.Length - 1
                Dim paired As Position() = combinations(i)
                Dim Line As Position() = {paired(0), paired(1)}
                Dim ps As Double()
                Dim left_highlyPaired As Boolean
                Dim pn As Double

                ps = {Line.First.left, Line.Last.left}
                pn = ps.Min / ps.Max  'Max is 1 (when min = max)
                score += (pn * 2)
                left_highlyPaired = pn >= high_Scoring_thresholds
                ps = {Line.First.right, Line.Last.right}
                pn = ps.Min / ps.Max  'Max is 1 (when min = max)
                score += (pn * 2)
                a_score += 4

                If pn >= high_Scoring_thresholds AndAlso left_highlyPaired Then
                    high_scores += 1
                End If
            Next

            Dim min As Integer = sys.Min(ds1.Distribution.Count, ds2.Distribution.Count)
            Dim max As Integer = Math.Max(ds1.Distribution.Count, ds2.Distribution.Count)

            score = (score / (combinations.Count * 2)) * min
            score += high_scores / max
            a_score = (a_score / (combinations.Count * 2)) * min
            a_score += high_scores / max

            Return New DomainAlignment With {
                 .ProteinQueryDomainDs = ds1,
                 .ProteinSbjctDomainDs = ds2,
                 .Score = score
             }
        End Function

        ''' <summary>
        ''' 这个函数会进行归一化处理
        ''' </summary>
        ''' <param name="domains"></param>
        ''' <param name="protLen"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Private Function __createDS(domains As ProteinModel.DomainObject(), protLen As Integer) As DomainDistribution()
            Dim Groups = (From x As ProteinModel.DomainObject
                          In domains
                          Select x
                          Group x By x.Name Into Group)
            Dim LQuery As IEnumerable(Of DomainDistribution) = From xGroup In Groups
                                                               Let dists As Position() = (From domain As ProteinModel.DomainObject
                                                                                          In xGroup.Group.ToArray
                                                                                          Select New Position(domain.Position, protLen)).ToArray
                                                               Select New DomainDistribution With {
                                                                   .DomainId = xGroup.Name,
                                                                   .Distribution = dists
                                                               }
            Return LQuery.ToArray
        End Function
    End Module
End Namespace
