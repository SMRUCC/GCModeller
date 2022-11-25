#Region "Microsoft.VisualBasic::4e18c56bcc92c9fd3567655ee7c65eb3, GCModeller\data\RegulonDatabase\Regprecise\Compiler.vb"

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

    '   Total Lines: 266
    '    Code Lines: 213
    ' Comment Lines: 21
    '   Blank Lines: 32
    '     File Size: 15.22 KB


    '     Module Compiler
    ' 
    '         Function: __predicate_genomeId, __predicate_name, __predicate_taxonomyId, GetGenome, LoadRegulon
    '                   LoadRegulons
    '         Delegate Function
    ' 
    '             Function: (+2 Overloads) __createRegulations, (+2 Overloads) Compile, CompileRegulations, FetchRegulators, FetchSites
    ' 
    '             Sub: __export, __genomePartitions, SitesFamilyCategory
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Regprecise

    <Package("Regprecise.Compiler", Publisher:="xie.guigang@gcmodeller.org")>
    Public Module Compiler

        <ExportAPI("Load.Xml.Regulon")>
        Public Function LoadRegulon(path As String) As JSON.regulon
            Return path.LoadXml(Of JSON.regulon)
        End Function

        <ExportAPI("Load.Xml.Regulons")>
        Public Function LoadRegulons(path As String) As JSON.regulon()
            Return path.LoadXml(Of JSON.regulon())
        End Function

        <ExportAPI("Xml.Get.Genome")>
        Public Function GetGenome(path As String, query As String,
                                  <Parameter("Query.Key", "name, genomeId, taxonomyId")>
                                  Optional key As String = "name") As JSON.genome

            Dim genomes As JSON.genome() = path.LoadXml(Of JSON.genome())
            Dim predicate As predicate
            Dim queryKey As Object

            Select Case key.ToLower.Trim
                Case NameOf(WebServices.JSON.genome.genomeId).ToLower
                    predicate = AddressOf __predicate_genomeId
                    queryKey = Scripting.InputHandler.CTypeDynamic(Of Integer)(query)
                Case NameOf(WebServices.JSON.genome.name).ToLower
                    predicate = AddressOf __predicate_name
                    queryKey = query
                Case NameOf(WebServices.JSON.genome.taxonomyId).ToLower
                    predicate = AddressOf __predicate_taxonomyId
                    queryKey = Scripting.InputHandler.CTypeDynamic(Of Integer)(query)
                Case Else
                    Call $"Query type '{key}' is not a valid field, using genomeId as default!".__DEBUG_ECHO
                    predicate = AddressOf __predicate_genomeId
                    queryKey = Scripting.InputHandler.CTypeDynamic(Of Integer)(query)
            End Select

            Dim LQuery = (From genome As JSON.genome
                          In genomes.AsParallel
                          Where predicate(genome, queryKey)
                          Select genome).FirstOrDefault
            Return LQuery
        End Function

        Private Function __predicate_genomeId(genome As WebServices.JSON.genome, query As Object) As Boolean
            Return genome.genomeId = DirectCast(query, Integer)
        End Function

        Private Function __predicate_name(genome As WebServices.JSON.genome, query As Object) As Boolean
            Return String.Equals(genome.name, DirectCast(query, String), StringComparison.OrdinalIgnoreCase)
        End Function

        Private Function __predicate_taxonomyId(genome As WebServices.JSON.genome, query As Object) As Boolean
            Return genome.taxonomyId = DirectCast(query, Integer)
        End Function

        Private Delegate Function predicate(genome As WebServices.JSON.genome, query As Object) As Boolean

        ''' <summary>
        ''' 请在下载完了整个数据库之后再使用这个函数来进行编译
        ''' </summary>
        ''' <param name="Regprecise"></param>
        ''' <param name="repository"></param>
        ''' <returns></returns>
        <ExportAPI("Regprecise.Compile")>
        Public Function Compile(Regprecise As IEnumerable(Of JSON.genome), repository As String) As Regprecise.WebServices.Regulations
            Dim MEME As String = repository & "/MEME"
            Dim LQuery = (From genome As JSON.genome
                              In Regprecise.AsParallel
                          Let fasta As SMRUCC.genomics.SequenceModel.FASTA.FastaFile = FetchRegulators(genome, repository)
                          Where Not fasta.IsNullOrEmpty
                          Select fasta.Save($"{MEME}/bbh/{genome.name.NormalizePathString.Replace(" ", "_")}.fasta")).ToArray  ' 调控因子的序列按照基因组保存，以方便建立bbh分析
            'sites则是按照调控因子的家族分类，以方便meme建模
            Call SitesFamilyCategory(repository)
            Dim regulations = (From genome As JSON.genome
                               In Regprecise.AsParallel
                               Select CompileRegulations(genome, repository)).ToArray.ToVector
            Return New WebServices.Regulations With {
                .Regulations = regulations,
                .Regulators = (From file As String
                                   In FileIO.FileSystem.GetFiles($"{repository}/regulators/", FileIO.SearchOption.SearchAllSubDirectories, "*.xml").AsParallel
                               Select file.LoadXml(Of JSON.regulator())).ToArray.ToVector.TrimNull,
                .Sites = (From file As String
                              In FileIO.FileSystem.GetFiles($"{repository}/sites/", FileIO.SearchOption.SearchAllSubDirectories, "*.xml").AsParallel
                          Select file.LoadXml(Of JSON.site())).ToArray.ToVector.TrimNull
            }
        End Function

        <ExportAPI("Regulations.Compiler")>
        Public Function CompileRegulations(genome As JSON.genome, repository As String) As Regprecise.WebServices.Regulation()
            Dim regulators = (From file As String
                              In FileIO.FileSystem.GetFiles($"{repository}/regulators/{genome.name.NormalizePathString}/", FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
                              Select id = BaseName(file),
                                  regulator = file.LoadXml(Of JSON.regulator())).ToDictionary(Function(obj) obj.id, elementSelector:=Function(obj) obj.regulator)
            Dim sites = (From file As String
                         In FileIO.FileSystem.GetFiles($"{repository}/sites/{genome.name.NormalizePathString}/", FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
                         Select id = BaseName(file),
                             site = file.LoadXml(Of JSON.site())).ToDictionary(Function(obj) obj.id, elementSelector:=Function(obj) obj.site)
            Dim regulons = $"{repository}/regulons/{genome.genomeId}.{genome.name.NormalizePathString}.xml".LoadXml(Of JSON.regulon()).ToDictionary(Function(obj) obj.regulonId)
            Dim regulations = (From regulator In regulators
                               Where Not regulator.Value.IsNullOrEmpty AndAlso sites.ContainsKey(regulator.Key)
                               Let sitesValue = sites(regulator.Key)
                               Select __createRegulations(regulator.Value, regulons, sitesValue)).ToArray.Unlist.Unlist.ToArray
            Return regulations
        End Function

        Private Function __createRegulations(regulators As JSON.regulator(),
                                             regulons As Dictionary(Of Integer, JSON.regulon),
                                             sites As JSON.site()) As Regprecise.WebServices.Regulation()()
            Return (From factor As JSON.regulator
                    In regulators
                    Select __createRegulations(factor, regulons, sites)).ToArray
        End Function

        Private Function __createRegulations(factor As JSON.regulator, regulons As Dictionary(Of Integer, JSON.regulon), sites As JSON.site()) As Regulation()
            If factor Is Nothing Then
                Return Nothing
            End If

            Dim regulon As JSON.regulon = regulons(factor.regulonId)
            Dim LQuery = (From site As JSON.site
                          In sites
                          Let regulation = New Regprecise.WebServices.Regulation With {
                              .Family = factor.regulatorFamily,
                              .Regulator = factor.vimssId,
                              .Site = $"{site.geneLocusTag}:{site.position}",'site.geneVIMSSId,
                              .Regulon = factor.regulonId,
                              .Type = ToType(regulon.regulationType)
                          }
                          Select regulation).ToArray
            Return LQuery
        End Function

        <ExportAPI("Fetch.Regulators")>
        Public Function FetchRegulators(genome As JSON.genome, repository As String) As FastaFile
            Dim DIR As String = $"{repository}/Fasta/regulators/{genome.name.NormalizePathString}/"
            If Not DIR.DirectoryExists Then
                Return Nothing
            End If
            Dim Fasta = (From file As String
                         In FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, "*.fasta", "*.fa", "*.fsa")
                         Select FastaReaders.Regulator.LoadDocument(FastaSeq.Load(file))).ToArray
            Return CType(Fasta.Select(Function(obj) New SequenceModel.FASTA.FastaSeq With {
                                           .Headers = New String() {obj.KEGG},
                                           .SequenceData = obj.SequenceData}), FastaFile)
        End Function

        <ExportAPI("Regprecise.Compile")>
        Public Function Compile(repository As String) As Regprecise.WebServices.Regulations
            Dim genomes As JSON.genome() = $"{repository}/genomes.xml".LoadXml(Of JSON.genome())
            Return Compile(genomes, repository)
        End Function

#Region "Regulator types"

        Public Const TF As String = "TF"
        Public Const RNA As String = "RNA"
#End Region

        ''' <summary>
        ''' 生成meme计算所需要的调控位点的fasta文件（按照家族分类）
        ''' </summary>
        ''' <param name="repositoryDIR">为了保持简洁性，没有引用配置项目。。。需要手动设定数据源</param>
        ''' <param name="genomePartitioning">当一个家族里面的序列数太多的时候是否需要按照基因组进行分组，默认不分组</param>
        ''' 
        <ExportAPI("Sites.Category.ByFamily", Info:="Category export the motif fasta sites into file system.")>
        Public Sub SitesFamilyCategory(repositoryDIR As String, Optional genomePartitioning As Boolean = False)
            Dim Regulons = (From xmlFile As String
                            In FileIO.FileSystem.GetFiles(repositoryDIR & "/regulons/", FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
                            Select xmlFile.LoadXml(Of JSON.regulon())).ToArray.Unlist  ' 读取数据源
            Dim Familys = (From regulon As JSON.regulon
                           In Regulons.AsParallel
                           Where String.Equals(TF, regulon.regulationType, StringComparison.OrdinalIgnoreCase)
                           Select regulon
                           Group regulon By Family = Strings.LCase(regulon.regulatorFamily.Trim) Into Group).ToArray  ' 将位点数据按照家族进行分类
            ' 从数据源之中读取site数据，然后导出fasta序列
            Dim FamilyFasta = (From family In Familys.AsParallel
                               Let sites As List(Of FastaReaders.Site) = (From regulon As JSON.regulon
                                                                          In family.Group
                                                                          Select FetchSites(regulon, repositoryDIR)).ToArray.Unlist
                               Select family, sites).ToArray

            For Each family In FamilyFasta
                If family.sites.Count > 500 AndAlso genomePartitioning Then
                    Call __genomePartitions(family.sites, repositoryDIR, family.family.Group.ToArray)
                Else
                    Call __export(family.family.Group.First.regulatorFamily, family.sites, repositoryDIR)
                End If
            Next
        End Sub

        Private Sub __export(Family As String, sites As List(Of FastaReaders.Site), repositoryDIR As String)
            Dim path As String = $"{repositoryDIR}/MEME/pwm/{Family}.fasta"
            Dim siteFa = sites.Select(Function(site) DirectCast(site, SMRUCC.genomics.SequenceModel.FASTA.FastaSeq))
            Dim Fasta As SMRUCC.genomics.SequenceModel.FASTA.FastaFile
            Fasta = CType(siteFa, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
            Call Console.Write("+")
            Call Fasta.Save(path)
        End Sub

        ''' <summary>
        ''' 检查，OK
        ''' </summary>
        ''' <param name="sites"></param>
        ''' <param name="repository"></param>
        ''' <param name="familyGroup"></param>
        Private Sub __genomePartitions(sites As List(Of FastaReaders.Site), repository As String, familyGroup As JSON.regulon())
            Dim i As Integer
            Dim temp As New List(Of FastaReaders.Site)
            ' 按照基因组分类
            Dim genomes = (From site As FastaReaders.Site
                           In sites
                           Select site
                           Group site By site.Bacteria Into Group).AsList

            Do While genomes.Count > 0
                i += 1

                Do While temp.Count < 500 AndAlso genomes.Count > 0
                    Call temp.AddRange(genomes(Scan0).Group.ToArray)
                    Call genomes.RemoveAt(Scan0)
                Loop

                If temp.Count < 6 Then  ' MEME的序列的数量的最低要求是6条
                    If i = 1 Then
                        GoTo SAVE
                    End If

                    Dim path As String = $"{repository}/MEME/pwm/{familyGroup.First.regulatorFamily}.part{i - 1}.fasta"  ' 和前面的合并
                    Dim Fasta = SMRUCC.genomics.SequenceModel.FASTA.FastaFile.Read(path)
                    Call Fasta.AddRange(temp)
                    Call Fasta.Save(path)
                Else
SAVE:               Dim path As String = $"{repository}/MEME/pwm/{familyGroup.First.regulatorFamily}.part{i}.fasta"
                    Dim siteFa = temp.Select(Function(site) DirectCast(site, SMRUCC.genomics.SequenceModel.FASTA.FastaSeq))
                    Dim Fasta = CType(siteFa, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
                    Call Fasta.Save(path)
                    Call temp.Clear()
                End If

                Call Console.Write(".")
            Loop
        End Sub

        <ExportAPI("Fetch.Sites")>
        Public Function FetchSites(regulon As JSON.regulon, repository As String) As FastaReaders.Site()
            Dim path As String = $"{repository}/sites/{regulon.genomeName.NormalizePathString}/{regulon.regulogId}.xml"
            Dim sites = path.LoadXml(Of JSON.site())
            Dim Fasta = (From site As JSON.site
                             In sites
                         Let siteFasta = FastaReaders.Site.CreateFrom(site, regulon.genomeName)
                         Select siteFasta).ToArray
            Return Fasta
        End Function

    End Module
End Namespace
