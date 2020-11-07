#Region "Microsoft.VisualBasic::b774bd0043f2a36e185fa60f452f622c, CLI_tools\mpl\CLI\Database.vb"

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
    '     Constructor: (+1 Overloads) Sub New
    '     Function: BuildCOG, BuildFamily, BuildKO, BuildOrthologDb, BuildPPIDb
    '               BuildSignature, InstallCDD, KEGGFamilys, ManualBuild
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports ProteinTools.Interactions.CLI
Imports SMRUCC.genomics.Analysis.ProteinTools
Imports SMRUCC.genomics.Assembly.KEGG.Archives
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString
Imports SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/Build.Db.Family",
               Usage:="/Build.Db.Family /source <source.KEGG.fasta> /pfam <pfam-string.csv>",
               Info:="Build protein family database from KEGG database dump data and using for the protein family annotation by MPAlignment.")>
    Public Function BuildFamily(args As CommandLine) As Integer
        Dim source As New FastaFile(args("/source"))
        Dim pfam = args("/pfam").LoadCsv(Of PfamString)
        Dim FamilyDb = Family.KEGG.FamilyDomains(KEGG:=source, Pfam:=pfam)
        Dim Name As String = basename(args("/source"))
        Return Family.SaveRepository(FamilyDb, Name).CLICode
    End Function

    ''' <summary>
    ''' 手工建立家族数据库
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <ExportAPI("/Build.Db.Family.Manual-Build", Usage:="/Build.Db.Family.Manual-Build /pfam-string <pfam-string.csv> /name <familyName>")>
    Public Function ManualBuild(args As CommandLine) As Integer
        Dim PfamString = args("/pfam-string").LoadCsv(Of PfamString)
        Dim Name As String = args("/name")
        Dim result = New Family.FileSystem.Database().ManualAdd(Name, PfamString)
        Call $"Add new database {result.ToFileURL}...".__DEBUG_ECHO
        Return 0
    End Function

    <ExportAPI("/Build.Db.PPI",
               Info:="Build protein interaction seeds database from string-db.")>
    Public Function BuildPPIDb(args As CommandLine) As Integer

    End Function

    <ExportAPI("/Build.PPI.Signature", Usage:="/Build.PPI.Signature /in <clustalW.fasta> [/level <5> /out <out.xml>]")>
    <ArgumentAttribute("/level", True,
                   Description:="It is not recommended to modify this value. The greater of this value, the more strict of the interaction scoring. level 5 is enough.")>
    Public Function BuildSignature(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim aln As New FASTA.FastaFile(inFile)
        Dim level As Integer = args.GetValue("/level", 5)
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".Signature.Xml")
        Dim ppiCategory = Category.FromAlign(aln, level:=level, cutoff:=1,
                                                       name:=basename(inFile))
        Call ppiCategory.GetXml.SaveTo(out)
        Call ppiCategory.GetSignatureFasta.Save(out.TrimSuffix & ".fasta")
        Return 0
    End Function

    ''' <summary>
    ''' 构建功能注释的KEGG标准库
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Build.Db.Ortholog",
               Usage:="/Build.Db.Ortholog [/COG <cogDIR> /KO]",
               Info:="Build protein functional orthology database from KEGG orthology or NCBI COG database.")>
    Public Function BuildOrthologDb(args As CommandLine) As Integer
        If args.GetBoolean("/ko") Then
            Return BuildKO()
        Else
            Return BuildCOG(args("/cog"))
        End If
    End Function

    Private Function BuildKO() As Integer

    End Function

    Private Function BuildCOG(cogDIR As String) As Integer

    End Function

    Sub New()
        Call Settings.Session.Initialize()
    End Sub

    <ExportAPI("/Build.Db.CDD",
               Info:="Install NCBI CDD database into the GCModeller repository database for the MPAlignment analysis.",
               Usage:="/Build.Db.CDD /source <source.DIR>")>
    Public Function InstallCDD(args As CommandLine) As Integer


    End Function

    <ExportAPI("/KEGG.Family", Usage:="/KEGG.Family /in <inDIR> /pfam <pfam-string.csv> [/out <out.csv>]")>
    Public Function KEGGFamilys(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim pfam As String = args("/pfam")
        Dim out As String = args.GetValue("/out", pfam.TrimSuffix & ".Family.Csv")
        Dim pfamHash = pfam.LoadCsv(Of PfamString).ToDictionary(Function(x) x.ProteinId.Split(":"c).Last)
        Dim orthologs = FileIO.FileSystem.GetFiles(inDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
        Dim Familys = (From file As String In orthologs.AsParallel
                       Let x As SSDB.OrthologREST = file.LoadXml(Of SSDB.OrthologREST)
                       Let sid As String = x.KEGG_ID.Split(":"c).Last
                       Where pfamHash.ContainsKey(sid)
                       Let Family As String = SequenceDump.KEGGFamily(x.Definition,)
                       Let pfamString As PfamString = pfamHash(sid)
                       Select New Family.AnnotationOut With {
                           .Family = Family.Split("/"c),
                           .PfamString = pfamString.PfamString,
                           .LocusId = sid}).ToArray
        Familys = (From x As Family.AnnotationOut
                   In Familys
                   Where Not x.PfamString.IsNullOrEmpty AndAlso
                       Not x.Family.IsNullOrEmpty
                   Select x).ToArray
        Return Familys.SaveTo(out).CLICode
    End Function
End Module
