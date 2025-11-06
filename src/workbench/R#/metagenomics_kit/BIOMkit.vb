#Region "Microsoft.VisualBasic::ccac4366411fb0dc3f3d83d915679889, R#\metagenomics_kit\BIOMkit.vb"

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

    '   Total Lines: 135
    '    Code Lines: 100 (74.07%)
    ' Comment Lines: 21 (15.56%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 14 (10.37%)
    '     File Size: 5.40 KB


    ' Module BIOMkit
    ' 
    '     Function: asDataFrame, getTaxonomy, readMatrix, unionBIOM
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome.BIOMExtensions
Imports SMRUCC.genomics.foundation
Imports SMRUCC.genomics.foundation.BIOM.v10
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports RDataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' the BIOM file toolkit
''' </summary>
<Package("BIOM_kit")>
<RTypeExport("biom.matrix", GetType(BIOMDataSet(Of Double)))>
Public Module BIOMkit

    Sub Main()
        RInternal.Object.Converts.makeDataframe.addHandler(GetType(BIOMDataSet(Of Double)), AddressOf asDataFrame)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Public Function asDataFrame(biomTable As BIOMDataSet(Of Double), args As list, env As Environment) As RDataframe
        Dim columns As New Dictionary(Of String, List(Of Double))
        Dim taxonomyNames As New List(Of String)

        For Each otu As NamedValue(Of [Property](Of Double)) In biomTable.PopulateRows
            For Each col In otu.Value
                If Not columns.ContainsKey(col.Name) Then
                    Call columns.Add(col.Name, New List(Of Double))
                End If

                Call columns(col.Name).Add(col.Value)
            Next

            Call taxonomyNames.Add(otu.Name)
        Next

        Return New RDataframe With {
            .columns = columns _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return DirectCast(a.Value.ToArray, Array)
                              End Function),
            .rownames = taxonomyNames.ToArray
        }
    End Function

    ''' <summary>
    ''' read matrix data from a given BIOM file.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("read.matrix")>
    <RApiReturn(GetType(BIOMDataSet(Of Double)))>
    Public Function readMatrix(file As Object,
                               Optional denseMatrix As Boolean = True,
                               Optional suppressErr As Boolean = False,
                               Optional env As Environment = Nothing) As Object

        If file Is Nothing Then
            Return RInternal.debug.stop("the given file can not be nothing!", env)
        ElseIf TypeOf file Is String Then
            If DirectCast(file, String).FileExists Then
                Try
                    Return BIOM.ReadAuto(file, denseMatrix:=denseMatrix)
                Catch ex As Exception When suppressErr
                    Call env.AddMessage(New Exception("file read error on " & file, ex), MSG_TYPES.WRN)
                    Return Nothing
                Catch ex As Exception
                    Throw
                End Try
            Else
                Return RInternal.debug.stop({"the given file is not found on your filesystem!", "file: " & file}, env)
            End If
        Else
            Return RInternal.debug.stop(Message.InCompatibleType(GetType(String), file.GetType, env), env)
        End If
    End Function

    ''' <summary>
    ''' get the taxonomy information from the BIOM matrix
    ''' </summary>
    ''' <param name="biom"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("biom.taxonomy")>
    <RApiReturn(GetType(Taxonomy))>
    Public Function getTaxonomy(biom As Object, Optional env As Environment = Nothing) As Object
        If biom Is Nothing Then
            Return RInternal.debug.stop("the given biom matrix object can not be nothing!", env)
        ElseIf TypeOf biom Is BIOMDataSet(Of Double) Then
            Return DirectCast(biom, BIOMDataSet(Of Double)).rows _
                .Where(Function(r) r.hasMetaInfo) _
                .Select(Function(row) row.metadata.lineage) _
                .ToArray
        Else
            Return RInternal.debug.stop(Message.InCompatibleType(GetType(BIOMDataSet(Of Double)), biom.GetType, env), env)
        End If
    End Function

    ''' <summary>
    ''' union merge multiple biom matrix
    ''' </summary>
    ''' <param name="tables"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("biom.union")>
    <RApiReturn(GetType(DataSet))>
    Public Function unionBIOM(tables As Object, Optional env As Environment = Nothing) As Object
        Dim raw As pipeline = pipeline.TryCreatePipeline(Of BIOMDataSet(Of Double))(tables, env)

        If raw.isError Then
            Return raw.getError
        End If

        Dim result As DataSet() = raw.populates(Of BIOMDataSet(Of Double))(env) _
            .Where(Function(tbl) Not tbl Is Nothing) _
            .Union _
            .ToArray

        If raw.isError Then
            Return raw.getError
        Else
            Return result
        End If
    End Function
End Module
