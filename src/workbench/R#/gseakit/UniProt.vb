#Region "Microsoft.VisualBasic::526c669b2b106372d1f36af1e529f4a4, R#\gseakit\UniProt.vb"

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

    '   Total Lines: 115
    '    Code Lines: 79 (68.70%)
    ' Comment Lines: 24 (20.87%)
    '    - Xml Docs: 95.83%
    ' 
    '   Blank Lines: 12 (10.43%)
    '     File Size: 4.42 KB


    ' Module UniProt
    ' 
    '     Function: keywordsProfiles, SubcellularLocation, uniprotKeywords
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports std = System.Math

''' <summary>
''' The uniprot background model handler
''' </summary>
''' 
<Package("UniProt")>
Module UniProt

    ''' <summary>
    ''' extract the sub-cellular location information as background model
    ''' </summary>
    ''' <param name="uniprot"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("subcellular_location")>
    <RApiReturn(GetType(Background))>
    Public Function SubcellularLocation(<RRawVectorArgument> uniprot As Object,
                                        Optional db_xref As String = Nothing,
                                        Optional env As Environment = Nothing) As Object

        Dim base = pipeline.TryCreatePipeline(Of entry)(uniprot, env)

        If base.isError Then
            Return base.getError
        Else
            Return base.populates(Of entry)(env).SubcellularLocation(db_xref)
        End If
    End Function

    ''' <summary>
    ''' Create a gsea background model for uniprot keywords based
    ''' on the given uniprot database assembly data
    ''' </summary>
    ''' <param name="uniprot"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("uniprot_keywords")>
    <RApiReturn(GetType(Background))>
    Public Function uniprotKeywords(<RRawVectorArgument> uniprot As Object,
                                    Optional db_xref As String = Nothing,
                                    Optional env As Environment = Nothing) As Object

        Dim base = pipeline.TryCreatePipeline(Of entry)(uniprot, env)

        If base.isError Then
            Return base.getError
        Else
            Return base.populates(Of entry)(env).UniprotKeywordsModel(db_xref)
        End If
    End Function

    ''' <summary>
    ''' create uniprot keyword ontology result
    ''' </summary>
    ''' <param name="enrichment"></param>
    ''' <param name="keywords"></param>
    ''' <param name="top"></param>
    ''' <returns></returns>
    <ExportAPI("keyword_profiles")>
    Public Function keywordsProfiles(enrichment As EnrichmentResult(),
                                     keywords As dataframe,
                                     Optional top As Integer = 4) As CatalogProfiles

        Dim catalogs As New Dictionary(Of String, CatalogProfile)
        Dim id As String() = keywords("Keyword ID")
        Dim Category As String() = keywords("Category")
        Dim groups = Category _
            .Select(Function(tag, i) (tag, id(i))) _
            .GroupBy(Function(tag) tag.tag) _
            .ToArray
        Dim enrich = enrichment.ToDictionary(Function(a) a.term)
        Dim profile As NamedValue(Of Double)()

        For Each group In groups
            id = group.Select(Function(i) i.Item2).ToArray
            profile = id _
                .Where(Function(a) enrich.ContainsKey(a)) _
                .OrderBy(Function(a) enrich(a).pvalue) _
                .Take(top) _
                .Select(Function(a)
                            Return New NamedValue(Of Double) With {
                                .Name = $"{a}: {enrich(a).name}",
                                .Value = -std.Log10(enrich(a).pvalue),
                                .Description = enrich(a).name
                            }
                        End Function) _
                .ToArray

            If profile.Length > 0 Then
                Dim max = profile.Select(Function(a) a.Value).Where(Function(pi) Not pi.IsNaNImaginary).ToArray

                If max.Length = 0 Then
                    max = {100}
                Else
                    max = {max.Max}
                End If

                profile = profile _
                    .Select(Function(a)
                                Return New NamedValue(Of Double)(a.Name, If(a.Value.IsNaNImaginary, max(0), a.Value))
                            End Function) _
                    .ToArray
                catalogs.Add(group.Key, New CatalogProfile(data:=profile))
            End If
        Next

        Return New CatalogProfiles With {
            .catalogs = catalogs
        }
    End Function
End Module
