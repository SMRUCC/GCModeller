#Region "Microsoft.VisualBasic::34f014fd87f2214aac4acda0ae9a528d, R#\seqtoolkit\hmmer.vb"

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

    '   Total Lines: 78
    '    Code Lines: 57 (73.08%)
    ' Comment Lines: 6 (7.69%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 15 (19.23%)
    '     File Size: 2.68 KB


    ' Module hmmer
    ' 
    '     Function: hmmer_search, load_hmmer, load_interprodb, parse_hmmer_model, parse_kofamscan
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.IO.HDF5.struct
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.HMMER
Imports SMRUCC.genomics.Analysis.SequenceTools.HMMER.InterPro.Xml
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("hmmer")>
Module hmmer

    <ExportAPI("load_interprodb")>
    <RApiReturn(GetType(Interpro))>
    Public Function load_interprodb(file As String) As Object
        Return pipeline.CreateFromPopulator(interprodb.ReadTerms(file))
    End Function

    <ExportAPI("parse_hmmer_model")>
    Public Function parse_hmmer_model(x As String) As ProfileHMM
        Return HMMER3Parser.ParseContent(x.SolveStream)
    End Function

    <ExportAPI("load_hmmer")>
    Public Function load_hmmer(<RRawVectorArgument> x As Object) As ProteinAnnotator
        Dim list = CLRVector.asCharacter(x)

        If list.IsNullOrEmpty Then
            Return Nothing
        End If

        Dim hmmer As New ProteinAnnotator

        If list.Length = 1 AndAlso list(0).DirectoryExists Then
            Call hmmer.LoadModelsFromDirectory(list(0))
        Else
            For Each file As String In list
                Call hmmer.LoadModel(file)
            Next
        End If

        Return hmmer
    End Function

    <ExportAPI("hmmer_search")>
    <RApiReturn(GetType(AnnotationResult))>
    Public Function hmmer_search(hmmer As ProteinAnnotator, <RRawVectorArgument> x As Object, Optional env As Environment = Nothing) As Object
        Dim seqs = GetFastaSeq(x, env)

        If seqs Is Nothing Then
            Return Nothing
        End If

        Return pipeline.CreateFromPopulator(seqs.Select(Function(fa) hmmer.Annotate(fa)).IteratesALL)
    End Function

    ''' <summary>
    ''' Parse the kofamscan table output
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("parse_kofamscan")>
    <RApiReturn(GetType(KOFamScan))>
    Public Function parse_kofamscan(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim s = SMRUCC.Rsharp.GetFileStream(file, IO.FileAccess.Read, env)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Return pipeline.CreateFromPopulator(KOFamScan.ParseTable(s.TryCast(Of Stream)))
    End Function

End Module

