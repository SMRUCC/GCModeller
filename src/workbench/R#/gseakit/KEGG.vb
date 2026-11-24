#Region "Microsoft.VisualBasic::2a4ae92283bc0f02a984d4090cf8b4a9, R#\gseakit\KEGG.vb"

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

    '   Total Lines: 76
    '    Code Lines: 53 (69.74%)
    ' Comment Lines: 12 (15.79%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 11 (14.47%)
    '     File Size: 3.20 KB


    ' Module KEGG
    ' 
    '     Function: compoundSet, kegg_category_annotation
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' the kegg background helper
''' </summary>
''' 
<Package("kegg")>
Module KEGG

    ''' <summary>
    ''' Get the kegg compound set from a kegg pathway map collection.
    ''' </summary>
    ''' <param name="maps">
    ''' a collection of the KEGG pathway <see cref="Map"/> object.
    ''' </param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' a ``list`` object, the name of each slot is the KEGG map id and the value is a named
    ''' list of the compound set (the name is the compound id and the value is the compound name)
    ''' of that map.
    ''' </returns>
    <ExportAPI("compound_set")>
    Public Function compoundSet(<RRawVectorArgument> maps As Object, Optional env As Environment = Nothing) As Object
        Dim coll = pipeline.TryCreatePipeline(Of Map)(maps, env)

        If coll.isError Then
            Return coll.getError
        End If

        Dim cpd_set As list = list.empty

        For Each map As Map In Tqdm.Wrap(coll.populates(Of Map)(env).ToArray)
            Call cpd_set.add(map.EntryId, map.GetCompoundSet.ToDictionary(Function(a) a.Name, Function(a) a.Value))
        Next

        Return cpd_set
    End Function

    ''' <summary>
    ''' Append the KEGG BRITE category annotation to a given dataframe by the KEGG id column.
    ''' </summary>
    ''' <param name="kegg">
    ''' the KEGG BRITE background model that is used for query the category information of
    ''' the KEGG id, for example the model that is created by ``KO.background``.
    ''' </param>
    ''' <param name="anno">
    ''' the target dataframe that contains the KEGG id column, the annotation columns will be
    ''' appended to this dataframe directly.
    ''' </param>
    ''' <param name="kegg_id">the column name of the KEGG id in the <paramref name="anno"/> dataframe. Default ``kegg_id``.</param>
    ''' <param name="class_field">the column name of the output KEGG class information. Default ``kegg_class``.</param>
    ''' <param name="category_field">the column name of the output KEGG category information. Default ``kegg_category``.</param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' the given dataframe with two new columns (``kegg_class`` and ``kegg_category``) appended,
    ''' or an error message when the required KEGG id column is missing.
    ''' </returns>
    ''' <remarks>
    ''' Only the first KEGG id will be used when the value of the ``kegg_id`` column contains
    ''' multiple ids that are separated by ``;``, ``/`` or other blank characters.
    ''' </remarks>
    <ExportAPI("kegg_category_annotation")>
    Public Function kegg_category_annotation(kegg As Background, anno As dataframe,
                                             Optional kegg_id As String = "kegg_id",
                                             Optional class_field As String = "kegg_class",
                                             Optional category_field As String = "kegg_category",
                                             Optional env As Environment = Nothing) As Object
        If Not anno.hasName(kegg_id) Then
            Return RInternal.debug.stop($"the required specific data field '{kegg_id}' is not existed inside the given dataframe!", env)
        Else
            ' we should sort the cluster with member size asc
            ' for avoid the global map always has hits
            kegg.clusters = kegg.clusters _
                .OrderBy(Function(c) c.members.TryCount) _
                .ToArray
        End If

        Dim ids As String() = CLRVector.asCharacter(anno(kegg_id)) _
            .Select(AddressOf Strings.Trim) _
            .Select(Function(str)
                        Return str _
                            .StringSplit("[;,\s]+") _
                            .Where(Function(si) Not si.StringEmpty(, True)) _
                            .FirstOrDefault
                    End Function) _
            .ToArray
        Dim maps = ids.Select(Function(id) kegg.GetClusterByMemberGeneId(id)).ToArray

        Call anno.add(class_field, maps.Select(Function(m) If(m Is Nothing, Nothing, m.class)))
        Call anno.add(category_field, maps.Select(Function(m) If(m Is Nothing, Nothing, m.category)))

        Return anno
    End Function

End Module
