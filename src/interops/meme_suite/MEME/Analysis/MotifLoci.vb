#Region "Microsoft.VisualBasic::09f30dfedce166bc1cb5af84acc82ac0, meme_suite\MEME\Analysis\MotifLoci.vb"

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

    '     Module MotifLoci
    ' 
    '         Function: __forwards, __getGene, __located, __reversed, CreateMotifModels
    '                   GetTraningSet, LoadMastSiteXml, (+2 Overloads) Located
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.LDM
Imports SMRUCC.genomics.SequenceModel

Imports Strands = SMRUCC.genomics.ComponentModel.Loci.Strands
Imports NucleotideLocation = SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Analysis.SequenceTools.MSA
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract.Probability

Namespace Analysis

    ''' <summary>
    ''' 对MEME text里面的位点在整个基因组上面的定位
    ''' </summary>
    ''' 
    <Package("Motif.Loci", Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Module MotifLoci

        ''' <summary>
        ''' This function is only works on the situation of the sequence was parsing from the upstream of the gene's ATG site.
        ''' (请注意，这个函数仅仅适用于从ATG往上游开始数的位置)
        ''' </summary>
        ''' <param name="PTT"></param>
        ''' <param name="Motifs"></param>
        ''' <returns></returns>
        Public Function Located(PTT As PTT,
                               <Parameter("Please make sure the object id in this motif list is the gene synonym.")>
                                Motifs As IEnumerable(Of Motif),
                                SiteLength As Dictionary(Of String, KeyValuePair(Of Double, Integer))) As MotifSite()
            Dim GeneObjects = PTT.ToDictionary
            Dim LQuery = (From motif In Motifs
                          Select motif, site_locis = (From site In motif.Sites
                                                      Let gene = __getGene(GeneObjects, site.Name)
                                                      Where Not gene Is Nothing
                                                      Select site, loci = __located(gene, site, SiteLength)).ToArray).ToArray
            Dim setValue As New SetValue(Of MotifSite)
            Dim sites As MotifSite() =
                LinqAPI.Exec(Of MotifSite) <= From motif
                                              In LQuery.AsParallel
                                              Select From site
                                                     In motif.site_locis
                                                     Let motifSite = DocumentFormat.MEME.Text.CopyObject(motif.motif, site.site)
                                                     Select setValue _
                                                         .InvokeSetValue(motifSite, NameOf(motifSite.gStart), site.loci.left) _
                                                         .InvokeSet(NameOf(motifSite.gStop), site.loci.right) _
                                                         .InvokeSet(NameOf(motifSite.Strand), site.loci.Strand.ToString).obj
            Return sites.ToArray
        End Function

        Private Function __getGene(GeneObject As Dictionary(Of String, GeneBrief), site As String) As GeneBrief
            If GeneObject.ContainsKey(site) Then
                Return GeneObject(site)
            Else
                Call $"Unable found gene information for {NameOf(site)}:={site}...".__DEBUG_ECHO
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="GeneObject"></param>
        ''' <param name="site">位点都描绘的是在所输入的<see cref="FASTA.FastaFile"/>文件之中的序列上面你的左端的起始位置</param>
        ''' <returns></returns>
        Private Function __located(GeneObject As GeneBrief, site As Site, Length As Dictionary(Of String, KeyValuePair(Of Double, Integer))) As NucleotideLocation
            Dim Len = Length(site.Name).Value
            Dim loci = If(GeneObject.Location.Strand = Strands.Forward,
                __forwards(GeneObject, site, Len),
                __reversed(GeneObject, site, Len))
            Return loci
        End Function

        ''' <summary>
        ''' 获取Motif位点在基因组上面的位置
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <param name="site"></param>
        ''' <param name="len">MEME序列的长度</param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Located")>
        <Extension> Public Function Located(gene As GeneBrief, site As Site, len As Integer) As NucleotideLocation
            Dim loci = If(gene.Location.Strand = Strands.Forward,
                __forwards(gene, site, len),
                __reversed(gene, site, len))
            Return loci
        End Function

        Private Function __forwards(GeneObject As GeneBrief, site As Site, Len As Integer) As NucleotideLocation
            Dim Loci = GeneObject.ATG - Len + site.Start
            Return New NucleotideLocation(Loci, GeneObject.ATG - site.Right, Strands.Forward)
        End Function

        Private Function __reversed(GeneObject As GeneBrief, site As Site, Len As Integer) As NucleotideLocation
            Dim Loci = GeneObject.ATG + Len - site.Start
            Return New NucleotideLocation(Loci, GeneObject.ATG + site.Right, Strands.Reverse)
        End Function

        <ExportAPI("DataSet.Training", Info:="Gets the data section in: <br /><br />
********************************************************************************<br />
TRAINING SET<br />
********************************************************************************")>
        Public Function GetTraningSet(path As String) As Dictionary(Of String, KeyValuePair(Of Double, Integer))
            Dim Lines = IO.File.ReadAllLines(path)
            Dim p As Integer = Lines.Located("TRAINING SET") + 6

            If p = -1 Then
                Return New Dictionary(Of String, KeyValuePair(Of Double, Integer))
            End If

            Dim str As New Value(Of String)
            Dim dict As New Dictionary(Of String, KeyValuePair(Of Double, Integer))

            Do While Not Regex.Match(str = Lines(p), "\*+").Success

                If String.IsNullOrEmpty(str) Then
                    p += 1
                    Continue Do
                End If

                Dim Tokens As String() = LinqAPI.Exec(Of String) <=
 _
                    From s As String
                    In str.Value.Split
                    Where Not String.IsNullOrEmpty(s)
                    Select s

                Call dict.Add(Tokens(Scan0), New KeyValuePair(Of Double, Integer)(Val(Tokens(1)), CInt(Val(Tokens(2)))))
                If Tokens.Length > 3 Then
                    Call dict.Add(Tokens(3), New KeyValuePair(Of Double, Integer)(Val(Tokens(4)), CInt(Val(Tokens(5)))))
                End If

                p += 1
            Loop

            Return dict
        End Function

        <ExportAPI("Load.Xml.MAST")>
        Public Function LoadMastSiteXml(Xml As String) As DocumentFormat.XmlOutput.MAST.MAST
            Dim doc As DocumentFormat.XmlOutput.MAST.MAST = Xml.LoadXml(Of DocumentFormat.XmlOutput.MAST.MAST)
            Return doc
        End Function

        <Extension>
        Public Iterator Function CreateMotifModels(motifs As IEnumerable(Of Motif)) As IEnumerable(Of SequenceMotif)
            Dim MSA_seed As MSAOutput
            Dim PWM As Residue()

            For Each data As Motif In motifs
                MSA_seed = New MSAOutput With {
                    .cost = data.Sites.Length,
                    .MSA = data.Sites.Select(Function(s) s.Site).ToArray,
                    .names = data.Sites.Select(Function(l) l.Name).ToArray
                }
                PWM = data.PspMatrix _
                    .Select(Function(b, i)
                                Return New Residue With {
                                    .index = i + 1,
                                    .frequency = New Dictionary(Of Char, Double) From {
                                        {"A"c, b.A},
                                        {"T"c, b.T},
                                        {"G"c, b.G},
                                        {"C"c, b.C}
                                    }
                                }
                            End Function) _
                    .ToArray

                Yield New SequenceMotif With {
                    .length = data.Width,
                    .pvalue = data.Evalue,
                    .seeds = MSA_seed,
                    .region = PWM
                }
            Next
        End Function

    End Module
End Namespace
