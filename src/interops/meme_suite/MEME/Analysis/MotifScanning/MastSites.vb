#Region "Microsoft.VisualBasic::e1680416d0cc0e7e44cc2adc4251b39d, meme_suite\MEME\Analysis\MotifScanning\MastSites.vb"

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

    '     Class MastSites
    ' 
    '         Properties: ATGDist, evalue, Family, Gene, gStop
    '                     Length, match, pValue, Regulators, SequenceData
    '                     Sites, Start, Strand, StrandRaw, Trace
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: (+4 Overloads) __createObject, __getMappingLoci, __getRegulatorys, __getsVIMSSID, __toSites
    '                   (+2 Overloads) Compile, Copy, HasEmptyMappings, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.XmlOutput.MAST
Imports SMRUCC.genomics.SequenceModel

Namespace Analysis.MotifScans

    ''' <summary>
    ''' 使用某一个Motif的MEME模型扫描整个基因组的结果
    ''' </summary>
    Public Class MastSites : Inherits NucleotideModels.Contig
        Implements IPolymerSequenceModel
        Implements ISiteReader

        Public Property Start As Integer Implements ISiteReader.gStart
        <Ignored>
        Public ReadOnly Property Strand As Strands
        Public Property pValue As Double
        <Column("MEME.E-value")>
        Public Property evalue As Double
        Public Property match As String
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

        ''' <summary>
        ''' 在Regprecise之中的调控位点的记录，这个是通过meme模型来获取的，然后再根据这个就可以找到调控因子了，再结合bbh结果就可以计算出预测的调控关系了
        ''' </summary>
        ''' <returns></returns>
        Public Property Sites As Integer()
        ''' <summary>
        ''' 在Regprecise数据库之中的调控因子的基因编号
        ''' </summary>
        ''' <returns></returns>
        Public Property Regulators As Integer()
        Public Property Gene As String Implements ISiteReader.ORF
        Public Property ATGDist As Integer Implements ISiteReader.Distance
        Public Property Family As String Implements ISiteReader.Family

        ''' <summary>
        ''' Motif的来源，一般是meme的数据源
        ''' </summary>
        ''' <returns></returns>
        Public Property Trace As String

        Public ReadOnly Property Length As Integer
            Get
                Return Len(SequenceData)
            End Get
        End Property

        <Column("Strand")> Public Property StrandRaw As String Implements ISiteReader.Strand
            Get
                Return _StrandRaw
            End Get
            Set(value As String)
                _StrandRaw = value
                _Strand = GetStrand(value)
            End Set
        End Property

        Public ReadOnly Property gStop As Integer Implements ISiteReader.gStop
            Get
                If Strand = Strands.Forward Then
                    Return Start + Length
                Else
                    Return Start - Length
                End If
            End Get
        End Property

        Dim _StrandRaw As String

        Sub New()
        End Sub

        Sub New(site As MastSites)
            Me.ATGDist = site.ATGDist
            Me.Family = site.Family
            Me.Gene = site.Gene
            Me.match = site.match
            Me.pValue = site.pValue
            Me.Regulators = site.Regulators
            Me.SequenceData = site.SequenceData
            Me.Sites = site.Sites
            Me.Start = site.Start
            Me.StrandRaw = site.StrandRaw
            Me.Trace = site.Trace
            Me.evalue = site.evalue
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function Copy() As MastSites
            Return ShadowsCopy
        End Function

        ''' <summary>
        ''' 只要是<see cref="regulators"/>或者<see cref="sites"/>这两个指向Regprecise数据库之中的记录的任意一个属性为空，则本函数返回真
        ''' </summary>
        ''' <returns></returns>
        Public Function HasEmptyMappings() As Boolean
            Return Sites.IsNullOrEmpty OrElse Regulators.IsNullOrEmpty
        End Function

        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Return NucleotideLocation.CreateObject(Start, Length, Strand)
        End Function

        ''' <summary>
        ''' 这个函数是适用于Windows版本的&lt;meme>.txt来源的
        ''' </summary>
        ''' <param name="mast"></param>
        ''' <param name="PWMDir">从MEME文本文件所输出的pwm模型的结果的文件夹</param>
        ''' <param name="faDIR">生成meme模型的调控位点的fasta文件夹，
        ''' 由于调控关系是在生成位点的fasta文件的时候一同生成在fasta序列的标题之中的，这个过程是为了方便构建调控关系，所以在这里需要fasta文件来得到调控关系，
        '''
        ''' 假若所生成的模型文件之中不需要包含有调控数据，则可以将这个参数置为空值
        ''' </param>
        ''' <returns></returns>
        Public Shared Function Compile(mast As DocumentFormat.XmlOutput.MAST.MAST,
                                       PWMDir As String,
                                       Optional faDIR As String = "") As MastSites()
            Dim memePWM As String = mast.Motifs.memePWM
            Dim memePWMs = (From file As String In FileIO.FileSystem.GetFiles(PWMDir, FileIO.SearchOption.SearchTopLevelOnly, $"{memePWM}.*.xml")
                            Let pwm = file.LoadXml(Of AnnotationModel)
                            Let id = pwm.Uid.Split("."c).Last).ToDictionary(Function(obj) obj.id, Function(obj) obj.pwm)
            Dim pwmSites As Dictionary(Of String, Regprecise.FastaReaders.Site())

            If String.IsNullOrEmpty(faDIR) Then
                pwmSites = New Dictionary(Of String, Regprecise.FastaReaders.Site())
            Else
                pwmSites = __getRegulatorys(faDIR, memePWM)
            End If

            Dim mastSites = mast.Sequences.SequenceList(Scan0) _
                .Segments.Select(Function(loci) __createObject(loci, memePWMs, pwmSites, memePWM))
            Dim FamilyName As String = mast.Motifs.BriefName
            Dim setValue = New SetValue(Of Analysis.MotifScans.MastSites)() _
                .GetSet(NameOf(Analysis.MotifScans.MastSites.Family))
            Dim result As Analysis.MotifScans.MastSites() =
                LinqAPI.Exec(Of Analysis.MotifScans.MastSites) <= From site As Analysis.MotifScans.MastSites
                                                                  In mastSites.IteratesALL
                                                                  Where Not site Is Nothing  ' 由于过滤操作可能会出现空值，这些空值都是被过滤掉的位点，已经不需要了
                                                                  Select setValue(site, FamilyName)
            Return result
        End Function

        Private Shared Function __getRegulatorys(faDIR As String, memePWM As String) As Dictionary(Of String, Regprecise.FastaReaders.Site())
            Dim path As String = $"{faDIR}/{memePWM}.fasta"

            If Not path.FileExists Then
                Call $"{path.ToFileURL} is not available, regulation info will not be associated!".Warning
                Return New Dictionary(Of String, Regprecise.FastaReaders.Site())
            End If

            Dim lstSites = Regprecise.FastaReaders.Site.Load(path)
            Dim pwmFasta = (From site As Regprecise.FastaReaders.Site
                            In lstSites
                            Select guid = $"{site.geneLocusTag}:{site.position}",
                                site
                            Group By guid Into Group) _
                                .ToDictionary(Function(site) site.guid,
                                              Function(site) site.Group.Select(Function(obj) obj.site).ToArray)
            Return pwmFasta
        End Function

        Public Shared Function Compile(mast As DocumentFormat.XmlOutput.MAST.MAST, path$) As MastSites()
            Dim FamilyName As String = mast.Motifs.Directory
            If FamilyName Is Nothing Then
                FamilyName = path.BaseName
            End If
            If mast.Sequences Is Nothing OrElse
                mast.Sequences.SequenceList.IsNullOrEmpty Then
                Return New MastSites() {}
            End If
            Dim mastSites = mast.Sequences.SequenceList().Select(Function(x) __toSites(x, FamilyName))
            Dim setValue = New SetValue(Of MastSites) <=
                NameOf(Analysis.MotifScans.MastSites.Family)
            Dim result As MastSites() =
                LinqAPI.Exec(Of MastSites) <= From site As MastSites
                                              In mastSites.IteratesALL.IteratesALL
                                              Select setValue(site, FamilyName)
            Return result
        End Function

        Private Shared Function __toSites(seq As SequenceDescript, familyName As String) As MastSites()()
            Return seq.Segments.Select(Function(loci) __createObject(loci, familyName))
        End Function

        Private Shared Function __createObject(site As DocumentFormat.XmlOutput.MAST.Segment,
                                               pwms As Dictionary(Of String, AnnotationModel),
                                               pwmSites As Dictionary(Of String, Regprecise.FastaReaders.Site()),
                                               trace As String) As MastSites()
            Dim sequence As String = TrimNewLine(site.SegmentData, "").Replace(vbTab, "").Trim
            Dim sites As MastSites() = site.Hits.Select(Of MastSites)(
                Function(hit) __createObject(site.start, hit, sequence, pwms, pwmSites, offset:=5, trace:=trace))
            Return sites
        End Function

        Private Shared Function __createObject(site As DocumentFormat.XmlOutput.MAST.Segment, trace As String) As MastSites()
            Dim sequence As String = TrimNewLine(site.SegmentData, "").Replace(vbTab, "").Trim
            Dim sites As MastSites() = site.Hits.Select(Of MastSites)(
                Function(hit) __createObject(site.start, hit, sequence, OffSet:=5, trace:=trace))
            Return sites
        End Function

        Private Shared Function __getsVIMSSID(name As String, sites As Dictionary(Of String, Regprecise.FastaReaders.Site())) As Integer()
            If Not sites.ContainsKey(name) Then
                ' Call $"Motif source site {name} was unable to found in pwm source!".__DEBUG_ECHO
                Return Nothing
            Else
                Return sites(name).Select(Function(site) site.geneVIMSSId)
            End If
        End Function

        ''' <summary>
        ''' 不做任何筛选，直接导出数据
        ''' </summary>
        ''' <param name="start"></param>
        ''' <param name="hit"></param>
        ''' <param name="sequence"></param>
        ''' <param name="OffSet"></param>
        ''' <param name="trace"></param>
        ''' <returns></returns>
        Private Shared Function __createObject(start As Integer,
                                               hit As DocumentFormat.XmlOutput.MAST.HitResult,
                                               sequence As String,
                                               OffSet As Integer,
                                               trace As String) As MastSites
            Dim id As String = hit.GetId.Split("_"c).Last
            Dim length As Integer = Len(hit.match) + 2 * OffSet  '为了保证在进行分子生物学实验的时候能够得到完整的片段，在这里将位点的范围扩大了10个bp
            start = hit.pos - start
            start -= OffSet
            If start <= 0 Then
                start = 1
                'Call $"{hit.pos} - {start} is not enough".__DEBUG_ECHO
            End If

            Dim site As String = Mid(sequence, start, length)
            Dim strand As String = hit.GetStrand

            '需不需要将反向的序列互补为正向的？？？

            Dim mastSite As New MastSites With {              '  .gap = hit.gap,
                .Trace = $"{trace}::{id}",
                .match = hit.match,
                .pValue = hit.pvalue,
                .SequenceData = site,
                .Start = hit.pos,
                .StrandRaw = strand
            }
            Return mastSite
        End Function

        Private Shared Function __createObject(start As Integer,
                                               hit As DocumentFormat.XmlOutput.MAST.HitResult,
                                               sequence As String,
                                               pwms As Dictionary(Of String, AnnotationModel),
                                               pwmSites As Dictionary(Of String, Regprecise.FastaReaders.Site()),
                                               offset As Integer,
                                               trace As String) As MastSites
            Dim id As String = hit.motif.Split("_"c).Last

            If Not pwms.ContainsKey(id) Then
                ' 实际上这里是起着一个过滤的作用，可能会在建立LDM模型的时候重新选择了evalue进行过滤，所以有些motif消失了，在这里假若编号不存在的话，就不会生成数据到文件之中
                ' 从而实现过滤的操作
                Return Nothing
            End If

            Dim pwmSite As AnnotationModel = pwms(id)
            Dim sites As Integer() = (From siteId As Integer
                                      In pwmSite.Sites.Select(Function(siteRef) __getsVIMSSID(siteRef.Name, pwmSites)).Unlist
                                      Select siteId
                                      Distinct).ToArray
            Dim regulators As Integer()() = pwmSite.Sites.Select(Function(siteRef) siteRef.Regulators)
            Dim mastSite As MastSites = __createObject(start, hit, sequence, offset, trace)

            mastSite.evalue = pwmSite.Evalue
            mastSite.Sites = (From vsmID As Integer In sites
                              Select vsmID
                              Distinct
                              Order By vsmID Ascending).ToArray
            mastSite.Regulators = (From vsmID As Integer In regulators.Unlist
                                   Select vsmID
                                   Distinct
                                   Order By vsmID Ascending).ToArray
            Return mastSite
        End Function
    End Class
End Namespace
