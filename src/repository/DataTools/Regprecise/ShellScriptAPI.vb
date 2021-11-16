#Region "Microsoft.VisualBasic::05673ac41c6621bc0fb24dfd361dcdea, DataTools\Regprecise\ShellScriptAPI.vb"

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

    '     Module RegpreciseShellScriptAPI
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: __assignRegulations, BuildPWM, CompileAnnotations, ParalogsFromSelfOverview
    ' 
    '         Sub: BuildPWM
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.Views

Namespace RegpreciseRegulations

    <Package("Annotations.Regprecise",
                        Category:=APICategories.ResearchTools,
                        Publisher:="xie.guigang@gcmodeller.org")>
    Public Module RegpreciseShellScriptAPI

        '<ExportAPI("Regprecise.Regulators.Install",
        '           Info:="Parameter Regulators is the fasta file path string of the regprecise regulators fasta sequence file.")>
        'Public Function InstallDatabase(Regulators As String) As Model_Repository.Regprecise()
        '    Return Regprecise.Install(Regulators, Settings.SettingsFile.RepositoryRoot)
        'End Function

        Sub New()
            If Not Settings.Session.Initialized Then
                Call Settings.Initialize()
            End If
        End Sub

        '<ExportAPI("Session._Init()")>
        'Public Function InitializeSession(Optional blastbin As String = "") As Regprecise
        '    Dim BlastHandle As InteropService = NCBILocalBlast.CreateSession(blastbin)
        '    Return New Regprecise(BlastHandle)
        'End Function

        '<ExportAPI("invoke_annotations")>
        'Public Function InvokeRegpreciseAnnotations(Handle As Regprecise, Fasta As String, Optional Parallel As Boolean = False) As GenomeAnnotations
        '    Return Handle.InvokeAnnotation(Fasta, Parallel)
        'End Function

        '<ExportAPI("invoke_annotations")>
        'Public Function InvokeAnnotation(Handle As Regprecise,
        '                                 Orthologous As BiDirectionalBesthit(),
        '                                 Paralogs As BestHit(),
        '                                 <Parameter("Proteins.Fasta")> ProteinsFasta As String) As Reports.GenomeAnnotations
        '    Return Handle.InvokeAnnotation(Orthologous, Paralogs, ProteinsFasta)
        'End Function

        '<ExportAPI("Table.Export.From.Fasta")>
        'Public Function CreateTableData(DBFile As String) As Model_Repository.Regprecise()
        '    Return Regprecise.CreateDatabaseTable(DBFile, Settings.TEMP)
        'End Function

        '<ExportAPI("Orthologous")>
        'Public Function Orthologous(<Parameter("Path.QvS")> qvsPath As String,
        '                            <Parameter("Path.SvQ")> svqPath As String) _
        '                            As BiDirectionalBesthit()
        '    Return Regprecise.Orthologous(qvsPath, svqPath)
        'End Function

        '''' <summary>
        '''' 从blastp日志数据之中导出regprecise数据库的注释结果
        '''' </summary>
        '''' <param name="qvsPath"><see cref="Overview.LoadExcel"></see> 物种_vs_regprecise</param>
        '''' <param name="svqPath">regprecise_vs_物种</param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        '''' 
        '<ExportAPI("Orthologous.From.Overview",
        '           Info:="qvs is the data of the annotated species blast data vs regprecise database, the svq is the contrast of the qvs.")>
        'Public Function OrthologousFromOverview(<Parameter("Path.QvS")> qvsPath As String,
        '                                        <Parameter("Path.SvQ")> svqPath As String) As BiDirectionalBesthit()
        '    Dim QvsBesthit = Overview.LoadExcel(qvsPath).ExportAllBestHist
        '    Dim SvQBesthit = Overview.LoadExcel(svqPath).ExportAllBestHist
        '    Dim bgs = From besthit As BestHit
        '              In SvQBesthit
        '              Select spcode = besthit.QueryName.Split(":"c).First,
        '                  bh = besthit
        '              Group By spcode Into Group
        '    Dim LQuery = (From bbbh
        '                  In bgs
        '                  Select bbbh.spcode,
        '                      bh = (From item In bbbh.Group Select item.bh).ToArray) _
        '                        .ToDictionary(Function(x) x.spcode,
        '                                      Function(x) x.bh)  '对subject进行按照物种分组
        '    Dim bg = From besthit As BestHit
        '             In QvsBesthit
        '             Select bh = besthit,
        '                 spcode = besthit.HitName.Split(CChar(":")).First
        '             Group By spcode Into Group
        '    Dim QvsBHData = From bhhh
        '                    In bg
        '                    Select spcode = bhhh.spcode,
        '                        bh = (From item In bhhh.Group Select item.bh).ToArray
        '    Dim BBH As BiDirectionalBesthit() = (From species
        '                                         In QvsBHData.AsParallel  ' 分别构建出bbh
        '                                         Let bbhData As BiDirectionalBesthit() = Regprecise.InternalCreateBBH(
        '                                             species.spcode,
        '                                             Qvs:=species.bh,
        '                                             SvqDict:=LQuery)
        '                                         Where Not bbhData.IsNullOrEmpty
        '                                         Select bbhData) _
        '                                            .IteratesALL _
        '                                            .Where(Function(item) item.Matched) _
        '                                            .ToArray
        '    Return BBH
        'End Function

        <ExportAPI("Paralogs.From.Overview")>
        Public Function ParalogsFromSelfOverview(<Parameter("Overview.Csv")> overViewCsv As String) As BestHit()
            Dim Oview = Overview.LoadExcel(overViewCsv)
            Dim Paralogs = Oview.ExportParalogs
            Return Paralogs
        End Function

        <ExportAPI("Compile.Regprecise.Annotations")>
        Public Function CompileAnnotations(orthologous As IEnumerable(Of BiDirectionalBesthit),
                                           paralogs As IEnumerable(Of BestHit),
                                           <Parameter("Query.Fasta")> QueryFasta As String,
                                           meta As IEnumerable(Of Model_Repository.Regprecise)) As Reports.GenomeAnnotations
            Dim OrthologousDict = (From item In orthologous
                                   Select spcode = item.HitName.Split(":"c).First,
                                        bbh = item Group By spcode Into Group).ToArray.ToDictionary(Function(item) item.spcode, Function(item) (From bh In item.Group Select bh.bbh).ToArray)
            Dim SourceMeta = AnnotationTool.CreateAnnotationSourceMeta(meta)
            Return Reports.GenomeAnnotations.CompileResult(OrthologousDict, paralogs.ToArray, QueryFasta, SourceMeta)
        End Function

        Const MEME As String =
"Imports Tools.Tmod

Call _init()
Call MEME.Invoke_Batch {Source}, {Export}, 0.001, 100, zoops, -dna"

        ''' <summary>
        ''' Build the pwm matrix model for the regulations sites in the regprecise database.
        ''' (构建meme的pwm模型并且保存于GCModeller的数据库之中)
        ''' </summary>
        ''' <param name="rebuildMatrix"></param>
        <ExportAPI("PWM.Build", Info:="Build the pwm matrix model for the regulations sites in the regprecise database.")>
        Public Sub BuildPWM(<Parameter("Matrix.ReBuild")> Optional rebuildMatrix As Boolean = True)
            Dim sites As String = Settings.Session.Initialize().RepositoryRoot & "/Regprecise/MEME/pwm/"   'MEME分析的fasta来源
            Dim Export As String = Settings.Session.SettingsFile.RepositoryRoot & "/Regprecise/MEME/Matrix/"  'MEME 程序输出的文件夹，里面应该包含着meme.txt文件

            If rebuildMatrix Then
                Dim Script As String = MEME.Replace("{Source}", sites.CLIPath).Replace("{Export}", Export)
                Call Settings.Session.FolkShoalThread(Script, Settings.Session.SettingsFile.RepositoryRoot & "/Regprecise/MEME/meme.log")
            End If

            Dim regulationsXml As String = $"{Settings.Session.SettingsFile.RepositoryRoot}/Regprecise/MEME/regulations.xml"
            Dim Regulations As Regulations = regulationsXml.LoadXml(Of Regulations)
            Dim LQuery = (From file As String
                          In FileIO.FileSystem.GetFiles(Export, FileIO.SearchOption.SearchTopLevelOnly, "*.txt").AsParallel
                          Select AnnotationModel.LoadDocument(file)).Unlist
            Dim setValue = New SetValue(Of AnnotationModel) <= NameOf(AnnotationModel.Sites)
            Dim assignRegulations As AnnotationModel() =
                LinqAPI.Exec(Of AnnotationModel) <= From motif As AnnotationModel
                                                    In LQuery.AsParallel
                                                    Let values As Site() =
                                                        motif.Sites.Select(Function(site) __assignRegulations(site, Regulations))
                                                    Select setValue(motif, values)
            Export = Settings.Session.SettingsFile.RepositoryRoot & "/Regprecise/MEME/MAST_LDM/"  ' 在这里导出模型
            Dim Saved = (From Motif As AnnotationModel
                         In assignRegulations.AsParallel
                         Let path As String = $"{Export}/{Motif.Uid}.xml"
                         Select Motif.Uid, b = Motif.GetXml.SaveTo(path)).ToArray
            Call $"{Saved.Select(Function(obj) obj.Uid.Split("."c).First).RemoveDuplicates(Function(obj) obj).Length} regulatory sites family...".__DEBUG_ECHO
        End Sub

        <ExportAPI("PWM.Build.Custom")>
        Public Function BuildPWM(sourceDIR As String,
                                 Optional Export As String = "./MAST_LDM",
                                 <Parameter("E-value", "You can using this cutoff value to makes the further filtering of the meme analysised motif.")>
                                 Optional evalue As Double = 0.001) As AnnotationModel()
            Dim setValue = New SetValue(Of AnnotationModel) <=
                NameOf(AnnotationModel.Uid)
            Dim LQuery As AnnotationModel() =
                LinqAPI.Exec(Of AnnotationModel) <= From file As String
                                                    In (ls - l - r - wildcards("*.txt") <= sourceDIR).AsParallel
                                                    Let Name As String = basename(FileIO.FileSystem.GetFileInfo(file).Directory.Name)
                                                    Let models = AnnotationModel.LoadDocument(file)
                                                    Let assigned = (From x As AnnotationModel
                                                                    In models
                                                                    Select setValue(x, $"{Name}.{x.Uid.Split("."c).Last}"))
                                                    Select assigned
            LQuery = (From model As AnnotationModel
                      In LQuery
                      Where model.Evalue <= evalue
                      Let path As String = $"{Export}/{model.Uid}.xml"
                      Let saveFile As Boolean = model.GetXml.SaveTo(path)
                      Select model).ToArray
            Return LQuery
        End Function

        Private Function __assignRegulations(site As Site, regulations As Regulations) As Site
            Dim uid As String = site.Name
            Dim regulateSite = regulations.GetSite(uid)
            Dim regulationList = regulations.GetRegulations(regulateSite)

            If Not regulationList Is Nothing Then
                site.Regulators = regulationList.Select(Function(regulator) regulator.Regulator)
            Else
                ' Call $"{uid} unable to found any record!".__DEBUG_ECHO
            End If

            Return site
        End Function
    End Module
End Namespace
