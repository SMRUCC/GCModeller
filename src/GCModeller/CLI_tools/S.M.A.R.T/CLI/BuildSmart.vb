#Region "Microsoft.VisualBasic::f45b57166f72f5903aa757f031921b97, CLI_tools\S.M.A.R.T\CLI\BuildSmart.vb"

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

    ' Module CLI
    ' 
    '     Function: BuildSmart, ExportRegpreciseDomains, FamilyClassify, FamilyDomains, FamilyStat
    '               ManualBuild
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Analysis.ProteinTools.Family.API
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.Xfam

Partial Module CLI

    <ExportAPI("-buildsmart")>
    Public Function BuildSmart(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Call Console.WriteLine("BuildSmart [version {0}]" & vbCrLf & "Program for build smart analysis database." & vbCrLf, My.Application.Info.Version.ToString)

        Call Console.WriteLine("Input the CDD source directory:")
        Call Console.Write("> ")
        Dim Dir As String

READ_CDD_DIR:
        Dir = Console.ReadLine
        If Not FileIO.FileSystem.DirectoryExists(Dir) Then
            Call Console.Write("The specific directory is not exists on the file system, try again:" & vbCrLf & "> ")
            GoTo READ_CDD_DIR
        End If

        Call Console.WriteLine("Input the CDD database output directory:")
        Call Console.Write("> ")

        Dim ExportDir As String = Console.ReadLine

        Call FileIO.FileSystem.CreateDirectory(Dir)
        Call NCBI.CDD.DbFile.BuildDb(Dir, ExportDir)

        Return 0
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--Export.Domains", Usage:="--Export.Domains /in <pfam-string.csv>")>
    Public Function ExportRegpreciseDomains(args As CommandLine) As Integer
        Dim input = args("/in").LoadCsv(Of Pfam.PfamString.PfamString)
        Dim Domains = (From x As Pfam.PfamString.PfamString
                       In input
                       Where Not x.Domains Is Nothing OrElse x.Domains.Length = 0
                       Select x.Domains.Select(Function(name) name.Split(":"c).First)).ToArray.Unlist.Distinct.ToArray

        Domains = (From name As String In Domains Select name Order By name Ascending).ToArray

        Dim Pfam = New SMRUCC.genomics.Assembly.NCBI.CDD.Database(GCModeller.FileSystem.RepositoryRoot & "/CDD/").DomainInfo.Pfam
        Dim DomainPn = Domains.Select(Function(x) Pfam(x))

        Return DomainPn.SaveTo(args("/in").TrimSuffix & "-Domain.Pn.csv")
    End Function

    ''' <summary>
    ''' 建立蛋白质家族数据库
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--Family.Domains", Usage:="--Family.Domains /regprecise <regulators.fasta> /pfam <pfam-string.csv>",
               Info:="Build the Family database for the protein family annotation by MPAlignment.")>
    Public Function FamilyDomains(args As CommandLine) As Integer
        Dim inFile As String = args("/regprecise")
        Dim regprecise = FastaReaders.Regulator.LoadDocument(inFile).ToDictionary(Function(x) x.KEGG)
        Dim pfam = args("/pfam").LoadCsv(Of Pfam.PfamString.PfamString)
        Dim FamilyDb = SMRUCC.genomics.Analysis.ProteinTools.Family.API.FamilyDomains(regprecise, pfam)
        Dim Name As String = inFile.BaseName
        Return SMRUCC.genomics.Analysis.ProteinTools.Family.SaveRepository(FamilyDb, Name).CLICode
    End Function

    ''' <summary>
    ''' 手工建立家族数据库
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <ExportAPI("--manual-Build", Usage:="--manual-Build /pfam-string <pfam-string.csv> /name <familyName>")>
    Public Function ManualBuild(args As CommandLine) As Integer
        Dim PfamString = args("/pfam-string").LoadCsv(Of Pfam.PfamString.PfamString)
        Dim Name As String = args("/name")
        Dim result = New SMRUCC.genomics.Analysis.ProteinTools.Family.FileSystem.Database().ManualAdd(Name, PfamString)
        Call $"Add new database {result.ToFileURL}...".__DEBUG_ECHO
        Return 0
    End Function

    ''' <summary>
    ''' 使用MPAlignment方法进行家族注释
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--Family.Align", Usage:="--Family.Align /query <pfam-string.csv> [/threshold 0.65 /mp 0.65 /Name <null>]",
               Info:="Family Annotation by MPAlignment")>
    <ArgumentAttribute("/Name", True,
                          Description:="The database name of the aligned subject, if this value is empty or not exists in the source, then the entired Family database will be used.")>
    Public Function FamilyClassify(args As CommandLine) As Integer
        Dim Query = args("/query").LoadCsv(Of Pfam.PfamString.PfamString)
        Dim Threshold As Double = args.GetValue("/threshold", 0.65)
        Dim MpTh As Double = args.GetValue("/mp", 0.65)
        Dim Name As String = args("/Name")
        Dim result = SMRUCC.genomics.Analysis.ProteinTools.Family.FamilyAlign(Query, Threshold, MpTh, DbName:=Name)
        Dim path As String = If(String.IsNullOrEmpty(Name),
            args("/query").TrimSuffix & ".Family.Csv",
            $"{args("/query").TrimSuffix}__vs.{Name}.Family.Csv")
        Return result.SaveTo(path).CLICode
    End Function

    <ExportAPI("--Family.Stat", Usage:="--Family.Stat /in <anno_out.csv>")>
    Public Function FamilyStat(args As CommandLine) As Integer
        Dim input As String = args("/in")
        Dim out = SMRUCC.genomics.Analysis.ProteinTools.Family.FamilyStat(input.LoadCsv(Of AnnotationOut))
        Return out.Save(input.TrimSuffix & ".FamilyStat.csv", System.Text.Encoding.ASCII)
    End Function
End Module
