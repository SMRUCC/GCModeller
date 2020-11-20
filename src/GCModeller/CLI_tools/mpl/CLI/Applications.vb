#Region "Microsoft.VisualBasic::7f5a08dbcb230e55f170f47273001660, CLI_tools\mpl\CLI\Applications.vb"

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
    '     Function: __getReport, AlignFunction, FamilyClassified, MotifDensity, MplPPI
    '               StructureAlign
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports ProteinTools.Interactions.CLI
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.ProteinTools
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Data.Xfam
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString
Imports SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment
Imports xMPAlignment.Settings

Partial Module CLI

    <ExportAPI("/Motif.Density",
               Usage:="/Motif.Density /in <pfam-string.csv> [/out <out.csv>]")>
    Public Function MotifDensity(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-motifs-density.csv")
        Dim pfamString As PfamString() = (From x As PfamString In [in].LoadCsv(Of PfamString) Where Not x.PfamString.IsNullOrEmpty Select x).ToArray
        Dim n As Integer = pfamString.Length
        Dim LQuery = (From motif As ProteinModel.DomainObject
                      In (From x As PfamString
                          In pfamString
                          Let motifs As ProteinModel.DomainObject() = x.GetDomainData(False)
                          Select motifs).IteratesALL
                      Select motif.Name
                      Group Name By Name Into Count)
        Dim result = (From x In LQuery Select x.Name, density = x.Count / n Order By density Descending).ToArray
        Return result.SaveTo(out)
    End Function

    <ExportAPI("--align.Family",
               Usage:="--align.Family /query <pfam-string.csv> [/out <out.csv> /threshold 0.5 /mp 0.6 /Name <null>]",
               Info:="Protein family annotation by using MPAlignment algorithm.")>
    <ArgumentAttribute("/Name", True,
                   Description:="The database name of the aligned subject, if this value is empty or not exists in the source, then the entired Family database will be used.")>
    Public Function FamilyClassified(args As CommandLine) As Integer
        Dim Query = args("/query").LoadCsv(Of Pfam.PfamString.PfamString)
        Dim Threshold As Double = args.GetValue("/threshold", 0.5)
        Dim MpTh As Double = args.GetValue("/mp", 0.6)
        Dim Name As String = args("/Name")
        Dim settings As Programs.MPAlignment = Session.Initialize.GetMplParam
        Call settings.__DEBUG_ECHO

        Dim result As Family.API.AnnotationOut() = Family.FamilyAlign(Query, Threshold, MpTh, DbName:=Name, accept:=settings.FamilyAccept)
        Dim path As String = If(String.IsNullOrEmpty(Name),
            args("/query").TrimSuffix & ".Family.Csv",
            $"{args("/query").TrimSuffix}__vs.{Name}.Family.Csv")
        Dim out As String = args.GetValue("/out", path)
        Return result.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 这个是和KEGG标准数据库来做比较的
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--align.Function",
               Info:="Protein function annotation by using MPAlignment algorithm.")>
    Public Function AlignFunction(args As CommandLine) As Integer
        Throw New NotImplementedException
    End Function

    <ExportAPI("--align.PPI",
               Info:="Protein-Protein interaction network annotation by using MPAlignment algorithm.")>
    Public Function MplPPI(args As CommandLine) As Integer
        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' 这个操作是ppi比对操作的基础
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("--align.PPI_test", Usage:="--align.PPI_test /query <contacts.fasta> /db <ppi_signature.Xml> [/mp <cutoff:=0.9> /out <outDIR>]")>
    Public Function StructureAlign(args As CommandLine) As Integer
        Dim queryFile As String = args("/query")
        Dim DbXml As String = args("/db")
        Dim query = SequenceModel.FASTA.FastaSeq.Load(queryFile)
        Dim Db = DbXml.LoadXml(Of Category)
        Dim cutoff As Double = args.GetValue("/mp", 0.9)
        Dim score As Double = 0
        Dim alignOut = Align(query, Db, score, cutoff)
        Dim outDIR As String = args.GetValue("/out", queryFile.ParentPath & "/PPI_MPAlignment/")
        '    Return __getReport(Db, query, alignOut, score, outDIR)
        Throw New NotImplementedException
    End Function

    Private Function __getReport(Db As Category,
                                 query As SequenceModel.FASTA.FastaSeq,
                                 alignOut As AlignmentOutput,
                                 score As Double,
                                 outDIR As String) As Integer
        Dim htmlBuilder As New StringBuilder()

        Dim res As Image = ClustalVisual.InvokeDrawing(Db.GetSignatureFasta)
        Dim save As String = outDIR & "/DbClustalW.png"
        Call res.Save(save, ImageFormats.Png.GetFormat)
        Call htmlBuilder.AppendLine($"<img src=""DbClustalW.png"" />")
        Call htmlBuilder.AppendLine($"<pre>{alignOut.ToString}</pre>")

        Dim doc As New StringBuilder(My.Resources.index)
        Call doc.Replace("{TitleHere}", $"")
        Call doc.Replace("{ContentHere}", htmlBuilder.ToString)
        Call doc.SaveTo(outDIR & "/mpl.html")

        Call My.Resources.materialize.SaveTo(outDIR & "/assets/materialize.css")
        Call My.Resources.style.SaveTo(outDIR & "/assets/style.css")

        Return 0
    End Function
End Module
