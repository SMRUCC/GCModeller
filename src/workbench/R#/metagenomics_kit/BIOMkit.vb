Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
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

''' <summary>
''' the BIOM file toolkit
''' </summary>
<Package("BIOM_kit")>
<RTypeExport("biom.matrix", GetType(BIOMDataSet(Of Double)))>
Public Module BIOMkit

    Sub New()
        Internal.Object.Converts.makeDataframe.addHandler(GetType(BIOMDataSet(Of Double)), AddressOf asDataFrame)
    End Sub

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
            Return Internal.debug.stop("the given file can not be nothing!", env)
        ElseIf TypeOf file Is String Then
            If DirectCast(file, String).FileExists Then
                Try
                    Return BIOM.ReadAuto(file, denseMatrix:=denseMatrix)
                Catch ex As Exception
                    If suppressErr Then
                        Call env.AddMessage(New Exception("file read error on " & file, ex), MSG_TYPES.WRN)
                        Return Nothing
                    Else
                        Throw ex
                    End If
                End Try
            Else
                Return Internal.debug.stop("the given file is not found on your filesystem!", env)
            End If
        Else
            Return Internal.debug.stop(Message.InCompatibleType(GetType(String), file.GetType, env), env)
        End If
    End Function

    <ExportAPI("biom.taxonomy")>
    <RApiReturn(GetType(Taxonomy))>
    Public Function getTaxonomy(biom As Object, Optional env As Environment = Nothing) As Object
        If biom Is Nothing Then
            Return Internal.debug.stop("the given biom matrix object can not be nothing!", env)
        ElseIf TypeOf biom Is BIOMDataSet(Of Double) Then
            Return DirectCast(biom, BIOMDataSet(Of Double)).rows _
                .Where(Function(r) r.hasMetaInfo) _
                .Select(Function(row) row.metadata.lineage) _
                .ToArray
        Else
            Return Internal.debug.stop(Message.InCompatibleType(GetType(BIOMDataSet(Of Double)), biom.GetType, env), env)
        End If
    End Function

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

    Public Function asDataFrame(x As Object, args As list, env As Environment) As RDataframe
        Dim biomTable As BIOMDataSet(Of Double) = DirectCast(x, BIOMDataSet(Of Double))
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
End Module
