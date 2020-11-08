#Region "Microsoft.VisualBasic::93bedf8123b55822c9ae23b43a7200a8, proteomics_toolkit\ptfKit.vb"

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

' Module ptfKit
' 
'     Function: filterBykey, loadPtf, NCBITaxonomy, savePtf, split
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' toolkit for handle ptf annotation data set
''' </summary>
''' 
<Package("ptfKit")>
Module ptfKit

    <ExportAPI("uniprot.ptf")>
    Public Function fromUniProt(<RRawVectorArgument> uniprot As Object, Optional includesNCBITaxonomy As Boolean = False, Optional env As Environment = Nothing) As Object
        Dim source = getUniprotData(uniprot, env)

        If source Like GetType(Message) Then
            Return source.TryCast(Of Message)
        End If

        Return source _
            .TryCast(Of IEnumerable(Of entry)) _
            .Select(Function(protein) protein.toPtf(includesNCBITaxonomy)) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    <ExportAPI("load.ptf")>
    Public Function loadPtf(file As Object, Optional env As Environment = Nothing) As pipeline
        Dim stream = GetFileStream(file, FileAccess.Read, env)

        If stream Like GetType(Message) Then
            Return stream.TryCast(Of Message)
        End If

        Dim tryClose = Sub()
                           If TypeOf file Is String Then
                               Try
                                   Call stream.TryCast(Of Stream).Close()
                               Catch ex As Exception

                               End Try
                           End If
                       End Sub

        Return PtfFile _
            .ReadAnnotations(stream.TryCast(Of Stream)) _
            .DoCall(Function(anno)
                        Return pipeline.CreateFromPopulator(
                            upstream:=anno,
                            finalize:=tryClose
                        )
                    End Function)
    End Function

    <ExportAPI("filter")>
    Public Function filterBykey(<RRawVectorArgument> ptf As Object, key$, Optional env As Environment = Nothing) As pipeline
        Dim upstream As pipeline = pipeline.TryCreatePipeline(Of ProteinAnnotation)(ptf, env)

        If upstream.isError Then
            Return upstream
        End If

        Return upstream _
            .populates(Of ProteinAnnotation)(env) _
            .Where(Function(protein)
                       Return protein.attributes.ContainsKey(key)
                   End Function) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    <ExportAPI("save.ptf")>
    <RApiReturn(GetType(Boolean))>
    Public Function savePtf(<RRawVectorArgument> ptf As Object, file As Object, Optional env As Environment = Nothing) As Object
        Dim stream = GetFileStream(file, FileAccess.Write, env)
        Dim anno As pipeline = pipeline.TryCreatePipeline(Of ProteinAnnotation)(ptf, env)

        If anno.isError Then
            Return anno.getError
        End If
        If stream Like GetType(Message) Then
            Return stream.TryCast(Of Message)
        End If

        Using writer As New StreamWriter(stream) With {.NewLine = vbLf}
            Call PtfFile.WriteStream(
                annotation:=anno.populates(Of ProteinAnnotation)(env),
                file:=writer
            )
        End Using

        Return True
    End Function

    <ExportAPI("extract.taxonomy")>
    Public Function NCBITaxonomy(<RRawVectorArgument> ptf As Object, Optional env As Environment = Nothing) As Object
        Dim anno As pipeline = pipeline.TryCreatePipeline(Of ProteinAnnotation)(ptf, env)

        If anno.isError Then
            Return anno.getError
        End If

        Return anno.populates(Of ProteinAnnotation)(env).Where(Function(protein) protein.attributes.ContainsKey(""))
    End Function

    <ExportAPI("ptf.split")>
    Public Function split(<RRawVectorArgument> ptf As Object, key$, outputdir$, Optional env As Environment = Nothing) As Object
        Dim anno As pipeline = pipeline.TryCreatePipeline(Of ProteinAnnotation)(ptf, env)

        If anno.isError Then
            Return anno.getError
        End If

        Call anno.populates(Of ProteinAnnotation)(env).SplitAnnotations(key, outputdir)

        Return True
    End Function
End Module
