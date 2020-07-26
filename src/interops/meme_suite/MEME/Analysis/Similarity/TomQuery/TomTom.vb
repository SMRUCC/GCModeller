#Region "Microsoft.VisualBasic::36327762cfee3584d735de30b43d9e4c, meme_suite\MEME\Analysis\Similarity\TomQuery\TomTom.vb"

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

    '     Module TomTOm
    ' 
    '         Properties: Motifs
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ED, GetFamilyMotifs, KLD, PCC, SW
    '         Delegate Function
    ' 
    '             Function: (+2 Overloads) Compare, (+2 Overloads) CompareBest, CreateResult, GetMethod, ToChar
    '         Class __equals
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: BitsEquals, Equals
    ' 
    '         Class CompareResult
    ' 
    '             Properties: Consensus, Distance, Edits, Gaps, HitMotif
    '                         HitsLength, QueryLength, QueryMotif, Score, Similarity
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.SequenceModel

Namespace Analysis.Similarity.TOMQuery

    ''' <summary>
    ''' various motif column comparison functions and score combination methods
    ''' </summary>
    ''' 
    <Package("TOM.Query",
                      Category:=APICategories.ResearchTools,
                      Publisher:="xie.guigang@gcmodeller.org",
                      Description:="Various motif column comparison functions and score combination methods")>
    <Cite("TY  - JOUR
T1  - Constrained Binding Site Diversity within Families of Transcription Factors Enhances Pattern Discovery Bioinformatics
JO  - Journal of Molecular Biology
VL  - 338
IS  - 2
SP  - 207
EP  - 215
PY  - 2004/4/23/
T2  - 
AU  - Sandelin, Albin
AU  - Wasserman, Wyeth W.
SN  - 0022-2836
DO  - http://dx.doi.org/10.1016/j.jmb.2004.02.048
UR  - http://www.sciencedirect.com/science/article/pii/S0022283604002281
KW  - pattern finding
KW  - transcriptional regulation
KW  - biological constraints
KW  - familial binding profiles
KW  - pattern comparison
AB  - Diverse computational and experimental efforts are required to elucidate the control circuitry regulating the transcription of human genes. The fusion of gene-specific promoter analyses with large microarray studies and bioinformatics advances has produced optimism that significant progress can be made in unravelling this complex network. Within bioinformatics, past emphasis for improved pattern discovery has been placed upon “"phylogenetic footprinting"”, the identification of sequences conserved over moderate periods of evolution (e.g. human and mouse comparisons). We introduce a new direction in bioinformatics based on the constraints imposed by the structures of DNA-binding proteins. For most structurally related families of transcription factors, there are clear similarities in the sequences of the sites to which they bind. On the basis of this observation, we construct familial binding profiles for well-characterized transcription factor families. The profiles are shown to classify correctly the structural class of mediating transcription factors for novel motifs in 88% of cases. By incorporating the familial profiles into pattern discovery procedures, we demonstrate that functional binding sites can be found in genomic sequences of dramatically greater length than is possible otherwise. Thus, incorporating familial models can overcome the signal-to-noise challenge that has hindered the transition from microarray data to regulatory control sequences for human genes. Biochemically motivated constraints upon sequence diversity of binding sites will complement the genetically motivated constraints imposed in "“phylogenetic footprinting”" algorithms.
ER  - ")>
    <Cite(Title:="A Gibbs sampling method to detect overrepresented motifs in the upstream regions of coexpressed genes",
          Abstract:="Microarray experiments can reveal important information about transcriptional regulation. In our case, we look for potential promoter regulatory elements in the upstream region of coexpressed genes. Here we present two modifications of the original Gibbs sampling algorithm for motif finding (Lawrence et al., 1993). First, we introduce the use of a probability distribution to estimate the number of copies of the motif in a sequence. Second, we describe the technical aspects of the incorporation of a higher-order background model whose application we discussed in Thijs et al. (2001). Our implementation is referred to as the Motif Sampler. We successfully validate our algorithm on several data sets. First, we show results for three sets of upstream sequences containing known motifs: 1) the G-box light-response element in plants, 2) elements involved in methionine response in Saccharomyces cerevisiae, and 3) the FNR O(2)-responsive element in bacteria. We use these data sets to explain the influence of the parameters on the performance of our algorithm. Second, we show results for upstream sequences from four clusters of coexpressed genes identified in a microarray experiment on wounding in Arabidopsis thaliana. Several motifs could be matched to regulatory elements from plant defence pathways in our database of plant cis-acting regulatory elements (PlantCARE). Some other strong motifs do not have corresponding motifs in PlantCARE but are promising candidates for further analysis.",
          AuthorAddress:="ESAT-SCD, KULeuven, Kasteelpark Arenberg 10, 3001 Leuven, Belgium. GertThijs@esat.kuleuven.ac.be",
          Authors:="Thijs, G.
Marchal, K.
Lescot, M.
Rombauts, S.
De Moor, B.
Rouze, P.
Moreau, Y.",
          DOI:="10.1089/10665270252935566",
          ISSN:="1066-5277 (Print)
1066-5277 (Linking)",
          Issue:="2",
          Journal:="J Comput Biol",
          Keywords:="*Algorithms
Arabidopsis/genetics
Bacteria/genetics
Base Sequence
Computational Biology
DNA/genetics
Gene Expression Profiling/*statistics & numerical data
Models, Genetic
Oligonucleotide Array Sequence Analysis/statistics & numerical data
Saccharomyces cerevisiae/genetics",
          Notes:="Thijs, Gert
Marchal, Kathleen
Lescot, Magali
Rombauts, Stephane
De Moor, Bart
Rouze, Pierre
Moreau, Yves
eng
Research Support, Non-U.S. Gov't
Validation Studies
2002/05/23 10:00
J Comput Biol. 2002;9(2):447-64.",
          Pages:="447-64",
          PubMed:=12015892,
          StartPage:=0,
          URL:="",
          Volume:=9,
          Year:=2002)>
    <Cite(Title:="Computational detection of cis -regulatory modules",
          Year:=2003,
          Volume:=19,
          StartPage:=0,
          PubMed:=14534164,
          URL:="",
          Pages:="ii5-ii14",
          Notes:="Aerts, Stein
Van Loo, Peter
Thijs, Gert
Moreau, Yves
De Moor, Bart
eng
Research Support, Non-U.S. Gov't
England
Oxford, England
2003/10/10 05:00
Bioinformatics. 2003 Oct;19 Suppl 2:ii5-14.",
          Abstract:="MOTIVATION: The transcriptional regulation of a metazoan gene depends on the cooperative action of multiple transcription factors that bind to cis-regulatory modules (CRMs) located in the neighborhood of the gene. By integrating multiple signals, CRMs confer an organism specific spatial and temporal rate of transcription. RESULTS: Based on the hypothesis that genes that are needed in exactly the same conditions might share similar regulatory switches, we have developed a novel methodology to find CRMs in a set of coexpressed or coregulated genes. The ModuleSearcher algorithm finds for a given gene set the best scoring combination of transcription factor binding sites within a sequence window using an A(*)procedure for tree searching. To keep the level of noise low, we use DNA sequences that are most likely to contain functional cis-regulatory information, namely conserved regions between human and mouse orthologous genes. The ModuleScanner performs genomic searches with a predicted CRM or with a user-defined CRM known from the literature to find possible target genes. The validity of a set of putative targets is checked using Gene Ontology annotations. We demonstrate the use and effectiveness of the ModuleSearcher and ModuleScanner algorithms and test their specificity and sensitivity on semi-artificial data. Next, we search for a module in a cluster of gene expression profiles of human cell cycle genes. AVAILABILITY: The ModuleSearcher is available as a web service within the TOUCAN workbench for regulatory sequence analysis, which can be downloaded from http://www.esat.kuleuven.ac.be/~dna/BioI.",
          AuthorAddress:="Department of Electrical Engineering ESAT-SCD, Katholieke Universiteit Leuven, Kasteelpark Arenberg 10, Leuven, Belgium. stein.aerts@esat.kuleuven.ac.be",
          Authors:="Aerts, S.
Van Loo, P.
Thijs, G.
Moreau, Y.
De Moor, B.",
          DOI:="10.1093/bioinformatics/btg1052",
          ISSN:="1367-4803
1460-2059",
          Issue:="Suppl 2",
          Journal:="Bioinformatics",
          Keywords:="*Algorithms
Base Sequence
Binding Sites
Chromosome Mapping/*methods
Molecular Sequence Data
Protein Binding
Regulatory Elements, Transcriptional/*genetics
Sequence Analysis, DNA/*methods
*Software
Transcription Factors/*genetics
Transcription, Genetic/*genetics")>
    <Cite(Title:="T-Reg Comparator: an analysis tool for the comparison of position weight matrices",
          Keywords:="Binding Sites
*Gene Expression Regulation
Genomics/*methods
Internet
*Regulatory Sequences, Nucleic Acid
*Software
Transcription Factors/*metabolism
User-Computer Interface",
          Journal:="Nucleic Acids Res",
          Issue:="Web Server issue",
          ISSN:="1362-4962 (Electronic)
0305-1048 (Linking)",
          DOI:="10.1093/nar/gki590",
          Authors:="Roepcke, S.
Grossmann, S.
Rahmann, S.
Vingron, M.",
          AuthorAddress:="Max Planck Institute for Molecular Genetics, Ihnestrasse 73, 14195 Berlin, Germany. roepcke@molgen.mpg.de",
          Abstract:="T-Reg Comparator is a novel software tool designed to support research into transcriptional regulation. Sequence motifs representing transcription factor binding sites are usually encoded as position weight matrices. The user inputs a set of such weight matrices or binding site sequences and our program matches them against the T-Reg database, which is presently built on data from the Transfac [E. Wingender (2004) In Silico Biol., 4, 55-61] and Jaspar [A. Sandelin, W. Alkema, P. Engstrom, W. W. Wasserman and B. Lenhard (2004) Nucleic Acids Res., 32, D91-D94]. Our tool delivers a detailed report on similarities between user-supplied motifs and motifs in the database. Apart from simple one-to-one relationships, T-Reg Comparator is also able to detect similarities between submatrices. In addition, we provide a user interface to a program for sequence scanning with weight matrices. Typical areas of application for T-Reg Comparator are motif and regulatory module finding and annotation of regulatory genomic regions. T-Reg Comparator is available at http://treg.molgen.mpg.de.",
          Notes:="Roepcke, Stefan
Grossmann, Steffen
Rahmann, Sven
Vingron, Martin
eng
Evaluation Studies
Research Support, Non-U.S. Gov't
England
2005/06/28 09:00
Nucleic Acids Res. 2005 Jul 1;33(Web Server issue):W438-41.",
          Pages:="W438-41",
          PubMed:=15980506,
          StartPage:=0,
          URL:="",
          Volume:=33,
          Year:=2005)>
    Public Module TomTOm

        Public ReadOnly Property Motifs As IReadOnlyDictionary(Of String, AnnotationModel)

        <ExportAPI("Family.Motifs")>
        Public Function GetFamilyMotifs(name As String) As AnnotationModel()
            Dim LQuery = (From x In Motifs
                          Let fn As String = x.Key.Split("."c).First
                          Where String.Equals(name, fn, StringComparison.OrdinalIgnoreCase)
                          Select x.Value).ToArray
            Return LQuery
        End Function

        Sub New()

            On Error Resume Next

            Call Settings.Session.Initialize()
            Call $"Initialize scanning for the motifs in the GCModeller database....".__DEBUG_ECHO

            Dim MotifDIR As String = GCModeller.FileSystem.GetMotifLDM
            Dim Motifs = FileIO.FileSystem.GetFiles(MotifDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.xml")

            Call $"Load motifs @{MotifDIR.ToDIR_URL}...".__DEBUG_ECHO

            TomTOm.Motifs = (From xmlFile As String In Motifs.AsParallel
                             Let Id As String = BaseName(xmlFile)
                             Select Id,
                                 motif = xmlFile.LoadXml(Of MotifScans.AnnotationModel)) _
                                .ToDictionary(Function(x) x.Id, Function(x) x.motif)

            Call $"{Motifs.Count} motifs loaded!".__DEBUG_ECHO
        End Sub

        ''' <summary>
        ''' PCC, Pearson correlation coefficient;
        ''' </summary>
        ''' <param name="X"></param>
        ''' <param name="Y"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("PCC")>
        Public Function PCC(X As ResidueSite, Y As ResidueSite) As Double
            Dim value As Double = Correlations.GetPearson(X, Y)
            Return value
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="X"></param>
        ''' <param name="Y"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("ED")>
        Public Function ED(X As Analysis.MotifScans.ResidueSite, Y As Analysis.MotifScans.ResidueSite) As Double
            Dim d As Double = VBMath.EuclideanDistance(X.PWM, Y.PWM)
            Dim value As Double = 1 - d
            Return value
        End Function

        <ExportAPI("SW", Info:="Sandelin-Wasserman similarity function. Sandelin and Wasserman [15] introduced their own motif column
comparison function for the construction of familial binding profiles.")>
        Public Function SW(X As Analysis.MotifScans.ResidueSite, Y As Analysis.MotifScans.ResidueSite) As Double
            Dim value As Double = Correlations.SW(X, Y)
            value /= 2
            Return value
        End Function

        <ExportAPI("KLD", Info:="Kullback-Leibler divergence, The KLD has been used by several research groups to quantify similarity between motifs.")>
        Public Function KLD(X As Analysis.MotifScans.ResidueSite, Y As Analysis.MotifScans.ResidueSite) As Double
            Dim value As Double = Correlations.KLD(X, Y)
            Return value
        End Function

        Delegate Function ColumnCompare(X As Analysis.MotifScans.ResidueSite, Y As Analysis.MotifScans.ResidueSite) As Double

        ReadOnly _compares As IReadOnlyDictionary(Of String, TomTOm.ColumnCompare) =
            New Dictionary(Of String, TomTOm.ColumnCompare) From {
 _
                {"pcc", AddressOf TomTOm.PCC},
                {"ed", AddressOf TomTOm.ED},
                {"sw", AddressOf TomTOm.SW},
                {"kld", AddressOf TomTOm.KLD}
            }

        <ExportAPI("Compare")>
        Public Function Compare(query As Analysis.MotifScans.AnnotationModel,
                                LDM As Analysis.MotifScans.AnnotationModel,
                                Optional method As String = "pcc",
                                Optional cost As Double = 0.7,
                                Optional threshold As Double = 0.75,
                                Optional bitsLevel As Double = 1.5) As DistResult
            Dim compareInvoke As TomTOm.ColumnCompare = GetMethod(method)
            Dim param As New Parameters With {
                .Method = method,
                .LevCost = cost,
                .TOMThreshold = threshold,
                .BitsLevel = bitsLevel
            }
            Return Compare(query, LDM, compareInvoke, param)
        End Function

        <ExportAPI("GET.Compare.Method")>
        Public Function GetMethod(method As Value(Of String)) As TomTOm.ColumnCompare
            If Not _compares.ContainsKey(method = method.Value.ToLower) Then
                Call $"{NameOf(method)}:={method} is not available, using PCC method as default.".__DEBUG_ECHO
                method.Value = "pcc"
            End If

            Return _compares(method)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="LDM">GCModeller的数据库里面的Motif模型</param>
        ''' <param name="method"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Compare")>
        Public Function Compare(query As AnnotationModel,
                                LDM As AnnotationModel,
                                method As TomTOm.ColumnCompare,
                                param As Parameters) As DistResult
            Dim equals As New __equals(method, param.TOMThreshold, param.BitsLevel)
            Dim lev As DistResult = LevenshteinDistance.ComputeDistance(query.PWM, LDM.PWM, AddressOf equals.Equals, AddressOf ToChar, param.LevCost)

            If lev Is Nothing Then
                Dim rc = LDM.Complement.Uid = "*" & LDM.Uid
                Dim hit = rc
                lev = LevenshteinDistance.ComputeDistance(query.PWM, LDM.PWM, AddressOf equals.Equals, AddressOf ToChar, param.LevCost)
            End If

            Return lev
        End Function

        ''' <summary>
        ''' 大写字母表示概率很高的，小写字母表示概率比较低的，N表示任意碱基
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        <Extension> Public Function ToChar(x As MotifScans.ResidueSite) As Char
            Dim A = x.PWM(0), T = x.PWM(1), G = x.PWM(2), C = x.PWM(3)
            Dim nt As New MotifPM(A:=A, T:=T, G:=G, C:=C)
            Dim res = nt.MostProperly.Key
            Dim ch As Char = NucleotideModels.Conversion.ToChar(base:=res)
            Dim p As Double = nt.MostProperly.Value

            If p < 0.65 Then
                ch = Char.ToLower(ch)
            End If
            If p < 0.025 Then
                ch = "N"
            End If

            Return ch
        End Function

        ''' <summary>
        ''' 先对列比对，然后bits比对
        ''' </summary>
        Friend Class __equals

            ReadOnly TomQuery As TomTOm.ColumnCompare
            ReadOnly threshold As Double
            ReadOnly Bitslevel As Double = 2

            Sub New(query As TomTOm.ColumnCompare, threshold As Double, lv As Double)
                Me.TomQuery = query
                Me.Bitslevel = threshold / lv
                Me.threshold = threshold
            End Sub

            Public Overloads Function Equals(a As MotifScans.ResidueSite, b As MotifScans.ResidueSite) As Boolean
                Dim similarity As Double = TomQuery(a, b)  ' motif列的相似度
                Dim bits = (Math.Min(a.Bits, b.Bits) + 1) / (Math.Max(a.Bits, b.Bits) + 1) ' bits信息量的相似度
                Return similarity >= threshold AndAlso bits >= Bitslevel
            End Function

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="a"></param>
            ''' <param name="b"></param>
            ''' <returns>
            ''' Dim bits = (Math.Min(a.Bits, b.Bits) + 1) / (Math.Max(a.Bits, b.Bits) + 1) ' bits信息量的相似度
            ''' </returns>
            Public Shared Function BitsEquals(a As IEnumerable(Of ResidueSite), b As IEnumerable(Of ResidueSite)) As Double
                Dim ba As Double() = a.Select(Function(x) x.Bits)
                Dim bb As Double() = b.Select(Function(x) x.Bits)
                Return Correlations.GetPearson(ba, bb)
            End Function
        End Class

        ''' <summary>
        ''' 和数据库之中的Motif进行比较，已经在这里将列相似度和bits的相似度结合在一起了
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="method"></param>
        ''' <param name="cost"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Compare.Best")>
        Public Function CompareBest(query As Analysis.MotifScans.AnnotationModel,
                                    Optional method As String = "pcc",
                                    Optional cost As Double = 0.7,
                                    Optional threshold As Double = 0.75,
                                    Optional bitsLevel As Double = 2) As Dictionary(Of MotifScans.AnnotationModel, DistResult)

            Dim methodInvoke As TomTOm.ColumnCompare = GetMethod(method)
            Dim param As New Parameters With {
                .Method = method,
                .LevCost = cost,
                .TOMThreshold = threshold,
                .BitsLevel = bitsLevel
            }
            Dim LQuery = (From motif As KeyValuePair(Of String, AnnotationModel) In Motifs
                          Let distResult As DistResult = Compare(
                              query,
                              motif.Value,
                              methodInvoke,
                              param)
                          Where Not distResult Is Nothing' AndAlso distResult.MatchSimilarity >= threshold
                          Select score = distResult.MatchSimilarity,
                              distResult,
                              motifLDM = motif.Value
                          Order By score Descending).ToDictionary(Function(x) x.motifLDM,
                                                                  Function(x) x.distResult)
            Return LQuery
        End Function

        '<Extension> Public Function GetBitS(edits As DistResult) As Double
        '    Dim n As Double = Val(edits.Meta.TryGetValue(NameOf(ResidueSite.Bits), [default]:="0"))
        '    Return n
        'End Function

        Public Class CompareResult : Inherits I_BlastQueryHit

            <XmlElement> Public Property QueryMotif As String
            <XmlElement> Public Property HitMotif As String
            <XmlAttribute> Public Property Score As Double
            <XmlAttribute> Public Property Distance As Double
            <XmlAttribute> Public Property Similarity As Double
            <XmlElement> Public Property Edits As String
            <XmlElement> Public Property Consensus As String
            <XmlAttribute> Public Property Gaps As Integer

            <Column("Len.Qur")> Public ReadOnly Property QueryLength As Integer
                Get
                    Return Len(QueryMotif)
                End Get
            End Property
            <Column("len.Hit")> Public ReadOnly Property HitsLength As Integer
                Get
                    Return Len(HitMotif)
                End Get
            End Property

        End Class

        <ExportAPI("Result.Create")>
        Public Function CreateResult(query As MotifScans.AnnotationModel, subject As MotifScans.AnnotationModel, edits As DistResult) As CompareResult
            If edits Is Nothing Then
                Return New CompareResult With {
                    .QueryName = query.ToString,
                    .HitName = subject.ToString
                }
            End If

            Return New CompareResult With {
                .QueryName = query.ToString,
                .HitName = subject.ToString,
                .Distance = edits.Distance,
                .Edits = edits.DistEdits,
                .Gaps = edits.DistEdits.Length - edits.DistEdits.Count("m"c),
                .HitMotif = edits.Hypotheses,
                .QueryMotif = edits.Reference,
                .Score = Math.Round(edits.Score, 4),
                .Similarity = Math.Round(edits.MatchSimilarity, 4),
                .Consensus = edits.Matches', .BitS = edits.GetBitS
            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="query">query的MEME.txt</param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Compare.Best")>
        Public Function CompareBest(<Parameter("Query.MEME.Text", "The file path of the meme.txt for the tom query.")>
                                    query As String,
                                    Optional method As String = "pcc",
                                    Optional cost As Double = 0.7,
                                    Optional threshold As Double = 0.75,
                                    Optional bitsLevel As Double = 1.5) As Dictionary(Of AnnotationModel, Dictionary(Of AnnotationModel, DistResult))
            Dim Motifs = MotifScans.AnnotationModel.LoadDocument(query)
            Dim dict As New Dictionary(Of AnnotationModel, Dictionary(Of AnnotationModel, DistResult))

            For Each motif As MotifScans.AnnotationModel In Motifs
                Dim result = CompareBest(motif, method, cost, threshold, bitsLevel)
                Call dict.Add(motif, result)
            Next

            Return dict
        End Function
    End Module
End Namespace
