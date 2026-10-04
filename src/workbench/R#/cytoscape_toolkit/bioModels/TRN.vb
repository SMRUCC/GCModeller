#Region "Microsoft.VisualBasic::b2f70964df426442b5b3b097b8a396dc, R#\cytoscape_toolkit\bioModels\TRN.vb"

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

'   Total Lines: 39
'    Code Lines: 29 (74.36%)
' Comment Lines: 4 (10.26%)
'    - Xml Docs: 75.00%
' 
'   Blank Lines: 6 (15.38%)
'     File Size: 1.62 KB


' Module TRN
' 
'     Function: edge_table, ExpressionConnections
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.IO.Compression
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.WGCNA
Imports SMRUCC.genomics.Model.Network.Regulons
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' Transcription Regulation Network Builder Tools
''' </summary>
''' 
<Package("TRN")>
Module TRN

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(Connection()), AddressOf edge_table)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function edge_table(edges As Connection(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}

        Call df.add("source", From e As Connection In edges Select e.gene1)
        Call df.add("target", From e As Connection In edges Select e.gene2)
        Call df.add("is_directly", From e As Connection In edges Select e.is_directly)
        Call df.add("cor", From e As Connection In edges Select e.cor)
        Call df.add("pval", From e As Connection In edges Select e.pval)
        Call df.add("interaction", From e As Connection In edges Select e.interaction)

        Return df
    End Function

    <ExportAPI("fpkm.connections")>
    Public Function ExpressionConnections(fpkm As DataSet(), Optional cutoff# = 0.65) As Connection()
        Return fpkm.CorrelationNetwork(cutoff).ToArray
    End Function

    <ExportAPI("write_bicor")>
    Public Function write_bicor(x As Matrix, file As Object, Optional type As CorrelationEncodings = CorrelationEncodings.Float32, Optional env As Environment = Nothing) As Object
        Dim is_filepath As Boolean = False
        Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env, is_filepath:=is_filepath)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Dim zip As New ZipArchive(s, ZipArchiveMode.Update)
        Dim matrix_item = zip.CreateEntry("bicor.dat", CompressionLevel.Fastest)
        Dim index_item = zip.CreateEntry("index.dat", CompressionLevel.Fastest)
        Dim matrix_s As Stream = matrix_item.Open
        Dim index_s As Stream = index_item.Open
        Dim geneIds As String() = x.rownames
        Dim nSample As Integer = x.sample_count

        ' ② 逐行 bicor 计算（模拟耗时的一次性矩阵计算），边算边写 corstore
        Using writer As New CorrelationMatrixWriter(matrix_s, index_s, geneIds, nSample, type, CompressionLevel.NoCompression)
            Dim bar As ProgressBar = Nothing

            For Each i As Integer In TqdmWrapper.Range(0, geneIds.Length, bar:=bar)
                Dim row(geneIds.Length - 1) As Single
                Dim vi As Double() = x(i).experiments

                For j As Integer = 0 To geneIds.Length - 1
                    row(j) = If(j = i, 1.0F, CSng(Bicor.BiweightMidcorrelation(vi, x(j).experiments)))
                Next

                Call bar.SetLabel(geneIds(i))
                Call writer.WriteRow(geneIds(i), row)
            Next

            Call writer.Complete()
        End Using

        Try
            Call matrix_s.Dispose()
            Call index_s.Dispose()
        Catch ex As Exception

        End Try

        If is_filepath Then
            Try
                Call s.TryCast(Of Stream).Dispose()
            Catch ex As Exception
                Call ex.Message.warning
                Call App.LogException(ex)
            End Try
        End If

        Return True
    End Function

    <ExportAPI("open_bicor")>
    <RApiReturn(GetType(CorrelationMatrixStore))>
    Public Function open_bicor(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim is_filepath As Boolean = False
        Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env, is_filepath:=is_filepath)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Dim zip As New ZipArchive(s, ZipArchiveMode.Read)
        Dim matrix_item = zip.GetEntry("bicor.dat")
        Dim index_item = zip.GetEntry("index.dat")
        Dim matrix_s As Stream = matrix_item.Open
        Dim index_s As Stream = index_item.Open
        Dim matrix = CorrelationMatrixStore.Open(matrix_s, index_s, cacheRows:=2048)

        Return matrix
    End Function
End Module
