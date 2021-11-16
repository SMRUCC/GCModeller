#Region "Microsoft.VisualBasic::43ce2b14ef0129a383fff2938098279d, R#\proteomics_toolkit\summary.vb"

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

    ' Module summary
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: asProfileTable, profileSelector, proteinsGOprofiles
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Annotation
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.DAG
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports csvfile = Microsoft.VisualBasic.Data.csv.IO.File
Imports RDataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe

<Package("summary")>
Module summary

    Sub New()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(CatalogProfiles), AddressOf asProfileTable)
    End Sub

    Private Function asProfileTable(profiles As CatalogProfiles, args As list, env As Environment) As RDataframe
        Dim type As String = args.getValue("type", env, [default]:="go")
        Dim table As New RDataframe With {.columns = New Dictionary(Of String, Array)}
        Dim file As New csvfile

        Select Case type
            Case "go"
                Dim i As i32 = Scan0

                For Each k As NamedValue(Of CatalogProfile) In profiles.GetProfiles()
                    For Each term As NamedValue(Of Double) In k.Value.AsEnumerable
                        ' {"namespace", "id", "name", "counts"} 
                        Call file.AppendLine(New String() {k.Name, term.Name, term.Description, term.Value})
                    Next
                Next

                table.columns.Add("namespace", file.Column(++i).ToArray)
                table.columns.Add("id", file.Column(++i).ToArray)
                table.columns.Add("name", file.Column(++i).ToArray)
                table.columns.Add("counts", file.Column(++i).Select(AddressOf Val).ToArray)
                table.rownames = table.columns!id

            Case "ko"

                Throw New NotImplementedException

            Case Else
                Throw New NotImplementedException
        End Select

        Return table
    End Function

    <ExportAPI("profileSelector")>
    Public Function profileSelector(profiles As CatalogProfiles, Optional selects$ = "Q3") As CatalogProfiles
        Dim newProfiles As New CatalogProfiles

        For Each cat As NamedValue(Of CatalogProfile) In profiles.GetProfiles
            Dim list As NamedValue(Of Double)() = cat.Value.AsEnumerable.ToArray
            Dim top As NamedValue(Of Double)() = list _
                .ApplySelector(Function(o) o.Value, selects) _
                .Select(Function(o)
                            Return New NamedValue(Of Double) With {
                                .Name = o.Description,
                                .Value = o.Value
                            }
                        End Function) _
                .ToArray

            newProfiles.catalogs(cat.Name) = New CatalogProfile(DirectCast(top, IEnumerable(Of NamedValue(Of Double))))
        Next

        Return newProfiles
    End Function

    <ExportAPI("proteins.GO")>
    <RApiReturn(GetType(CatalogProfiles))>
    Public Function proteinsGOprofiles(<RRawVectorArgument> annotations As Object, goDb As GO_OBO, Optional level% = -1, Optional env As Environment = Nothing) As Object
        Dim ptf As pipeline = pipeline.TryCreatePipeline(Of AnnotationTable)(annotations, env, suppress:=True)

        If ptf.isError Then
            ptf = pipeline.TryCreatePipeline(Of ProteinAnnotation)(annotations, env)

            If ptf.isError Then
                Return ptf.getError
            End If

            ptf = ptf.populates(Of ProteinAnnotation)(env) _
                .Select(AddressOf AnnotationTable.FromUnifyPtf) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        End If

        Dim goInfo As AnnotationTable() = ptf _
            .populates(Of AnnotationTable)(env) _
            .Where(Function(prot) Not prot.GO.IsNullOrEmpty) _
            .ToArray
        Dim goTerms As Dictionary(Of String, Term) = goDb.terms.ToDictionary(Function(x) x.id)
        Dim DAG As New Graph(goTerms.Values)
        Dim data = goInfo.CountStat(Function(prot) prot.GO, goTerms)

        If level > 0 Then
            data = data.LevelGOTerms(level, DAG)
        End If

        Return New CatalogProfiles(data)
    End Function
End Module
