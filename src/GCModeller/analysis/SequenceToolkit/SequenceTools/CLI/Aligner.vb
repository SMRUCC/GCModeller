#Region "Microsoft.VisualBasic::154b6a40574a7f02938c77be4da6cfe2, ..\GCModeller\analysis\SequenceToolkit\SequenceTools\CLI\Aligner.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Analysis
Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.gwANI
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module Utilities

    ''' <summary>
    ''' <see cref="RunNeedlemanWunsch.RunAlign(FASTA.FastaToken, FASTA.FastaToken, Boolean, String)"/>
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/nw",
               Info:="RunNeedlemanWunsch",
               Usage:="/nw /query <query.fasta> /subject <subject.fasta> [/out <out.txt>]")>
    <ParameterInfo("/query", False, AcceptTypes:={GetType(FastaToken)})>
    <ParameterInfo("/subject", False, AcceptTypes:={GetType(FastaToken)})>
    <ParameterInfo("/out", True, AcceptTypes:={GetType(String)})>
    Public Function NW(args As CommandLine.CommandLine) As Integer
        Dim query As String = args("/query")
        Dim subject As String = args("/subject")
        Dim out As String = args.GetValue("/out", query.TrimFileExt & "-" & subject.BaseName & ".txt")
        Call RunNeedlemanWunsch.RunAlign(New FASTA.FastaToken(query), New FASTA.FastaToken(subject), False, out)
        Return 0
    End Function

    <ExportAPI("/align", Usage:="/align /query <query.fasta> /subject <subject.fasta> [/blosum <matrix.txt> /out <out.xml>]")>
    Public Function Align2(args As CommandLine.CommandLine) As Integer
        Dim query As String = args("/query")
        Dim subject As String = args("/subject")
        Dim blosum As String = args("/blosum")
        Dim out As String = args.GetValue("/out", query.TrimFileExt & "-" & IO.Path.GetFileNameWithoutExtension(subject) & ".xml")
        Dim queryFa As New FASTA.FastaToken(query)
        Dim subjectFa As New FASTA.FastaToken(subject)
        Dim mat = If(String.IsNullOrEmpty(blosum), Nothing, SequenceTools.Blosum.LoadMatrix(blosum))
        Dim sw As SequenceTools.SmithWaterman = SequenceTools.SmithWaterman.Align(queryFa, subjectFa, mat)
        Dim output As Output = sw.GetOutput(0.65, 6)
        Call output.__DEBUG_ECHO
        Return output.SaveAsXml(out).CLICode
    End Function

    <ExportAPI("--align", Usage:="--align /query <query.fasta> /subject <subject.fasta> [/out <out.DIR> /cost <0.7>]")>
    Public Function Align(args As CommandLine.CommandLine) As Integer
        Dim cost As Double = args.GetValue(Of Double)("/cost", 0.7)
        Dim query = FASTA.FastaFile.Read(args("/query"))
        Dim subject = FASTA.FastaFile.Read(args("/subject"))
        Dim outDIR As String = args.GetValue(
            "/out",
            $"{query.FilePath.ParentPath}/{query.FilePath.BaseName}-{subject.FilePath.BaseName}/")
        Dim resultSet = __alignCommon(query, subject, cost, outDIR)
        Return resultSet.GetXml.SaveTo(outDIR & "/AlignmentResult.xml").CLICode
    End Function

    Private Function __alignCommon(query As FASTA.FastaFile, subject As FASTA.FastaFile, cost As Double, outDIR As String) As AlignmentResult()
        Dim resultSet As New List(Of AlignmentResult)

        For Each queryToken As FASTA.FastaToken In query
            Dim queryCache As Integer() = queryToken.SequenceData.ToArray(Function(x) Asc(x))
            Dim alignSet As AlignmentResult() =
                LinqAPI.Exec(Of AlignmentResult) <= From subjectToken As FASTA.FastaToken
                                                    In subject.AsParallel
                                                    Let aln = AlignmentResult.SafeAlign(
                                                        queryToken.Title,
                                                        queryToken.SequenceData,
                                                        queryCache,
                                                        subjectToken,
                                                        cost)
                                                    Where Not aln Is Nothing
                                                    Select aln
            Call resultSet.Add(alignSet)

            For Each result As AlignmentResult In alignSet
                Dim path As String =
                    outDIR & $"/Views/{result.Reference.Split.First.NormalizePathString(False)}_vs_{result.Hypotheses.Split.First.NormalizePathString(False)}.html"
                Call result.Visualize.SaveTo(path)
            Next

            Call queryToken.Title.__DEBUG_ECHO
            Call FlushMemory()
        Next

        Return resultSet.ToArray
    End Function

    <ExportAPI("--align.Self", Usage:="--align.Self /query <query.fasta> /out <out.DIR> [/cost 0.75]")>
    Public Function AlignSelf(args As CommandLine.CommandLine) As Integer
        Dim query As New FASTA.FastaFile(args("/query"))
        Dim outDIR As String = args("/out")
        Dim cost As Double = args.GetValue("/cost", 0.75)
        Dim resultSet = __alignCommon(query, query, cost, outDIR)
        Return resultSet.ToArray.GetXml.SaveTo(outDIR & "/AlignmentResult.xml").CLICode
    End Function

    Public Class AlignmentResult : Inherits DistResult

        Public Property Query As String
        Public Property Subject As String

        Sub New()
        End Sub

        Sub New(query As FASTA.FastaToken, subject As FASTA.FastaToken, cost As Double)
            Call Me.New(query.Title, query.SequenceData, query.SequenceData.ToArray(AddressOf Asc), subject, cost)
        End Sub

        Sub New(queryTitle As String,
                query As String,
                queryArray As Integer(),
                subject As FASTA.FastaToken,
                cost As Double)
            Dim result = LevenshteinDistance.ComputeDistance(queryArray, subject.SequenceData, cost)

            Me.CSS = result.CSS
            Me.DistEdits = result.DistEdits
            Me.DistTable = result.TrimMatrix(1)
            Me.Hypotheses = subject.Title
            Me.Reference = queryTitle
            Me.Query = query
            Me.Subject = subject.SequenceData
            Me.Matches = result.Matches
        End Sub

        Public Shared Function SafeAlign(queryTitle As String,
                query As String,
                queryArray As Integer(),
                subject As FASTA.FastaToken,
                cost As Double) As AlignmentResult
            Try
                Return New AlignmentResult(queryTitle, query, queryArray, subject, cost)
            Catch ex As Exception  ' 无法比对出结果，放弃这次比对
                Return Nothing
            End Try
        End Function

        Protected Overrides Function __getReference() As String
            Return Query
        End Function

        Protected Overrides Function __getSubject() As String
            Return Subject
        End Function
    End Class

    ''' <summary>
    ''' 剪裁多重比对的结果文件，将两旁的大部分的非保守区去除掉
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Clustal.Cut", Usage:="/Clustal.Cut /in <in.fasta> [/left 0.1 /right 0.1 /out <out.fasta>]")>
    Public Function CutMlAlignment(args As CommandLine.CommandLine) As Integer
        Dim aln As New Patterns.Clustal.Clustal(args("/in"))
        Dim left As Double = args.GetValue("/left", 0.1)
        Dim right As Double = args.GetValue("/right", 0.1)
        Dim leftOffset, rightOffset As Integer

        For i As Integer = 0 To aln.Conservation.Length - 1
            If aln.Conservation(i).Frq >= left Then
                leftOffset = i
                Exit For
            End If
        Next
        For i As Integer = aln.Conservation.Length - 1 To 0 Step -1
            If aln.Conservation(i).Frq >= right Then
                rightOffset = aln.Conservation.Length - i
                Exit For
            End If
        Next

        Dim out = aln.Mid(leftOffset, rightOffset)
        Dim outFile As String =
            args.GetValue("/out", aln.FilePath.TrimFileExt & $"{leftOffset}-{rightOffset}.fasta")
        Return out.Save(-1, outFile, Encodings.ASCII).CLICode
    End Function

    <ExportAPI("/gwANI", Usage:="/gwANI /in <in.fasta> [/fast /out <out.Csv>]")>
    Public Function gwANI(args As CommandLine.CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ".gwANI.Csv")
        Dim fast As Boolean = args.GetBoolean("/fast")
        Dim result = gwANIExtensions.Evaluate([in], fast)
        Return result.SaveTo(out)
    End Function
End Module

