#Region "Microsoft.VisualBasic::ef0afec39436b92510027c154b3888d7, kegg_kit\profiles.vb"

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

    ' Module profiles
    ' 
    '     Function: CompoundPathwayIndex, CompoundPathwayProfiles, KEGGCategoryProfiles, KOpathwayProfiles
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Visualize.CatalogProfiling
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("profiles")>
Module profiles

    <ExportAPI("compounds.pathway.index")>
    Public Function CompoundPathwayIndex(pathways As PathwayMap()) As Dictionary(Of String, Index(Of String))
        Return pathways _
            .GroupBy(Function(p) p.briteID) _
            .ToDictionary(Function(p)
                              Return p.Key
                          End Function,
                          Function(p)
                              Return p.First.KEGGCompound _
                                  .SafeQuery _
                                  .Keys _
                                  .Indexing
                          End Function)
    End Function

    ''' <summary>
    ''' Do statistics of the KEGG pathway profiles based on the given kegg id
    ''' </summary>
    ''' <param name="pathways">The pathway compound reference index data</param>
    ''' <param name="compounds">The kegg compound id</param>
    ''' <returns></returns>
    <ExportAPI("compounds.pathway.profiles")>
    Public Function CompoundPathwayProfiles(pathways As Dictionary(Of String, Index(Of String)), compounds As String()) As Dictionary(Of String, Integer)
        Return pathways _
            .ToDictionary(Function(p) p.Key,
                          Function(p)
                              Return p.Value _
                                  .Intersect(collection:=compounds) _
                                  .Distinct _
                                  .Count
                          End Function)
    End Function

    ''' <summary>
    ''' create KEGG map prfiles via a given KO id list.
    ''' </summary>
    ''' <param name="KO">a character vector of KO id list.</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("KO.map.profiles")>
    <RApiReturn(GetType(Dictionary(Of String, Double)))>
    Public Function KOpathwayProfiles(<RRawVectorArgument> KO As Object, Optional env As Environment = Nothing) As Object
        If KO Is Nothing Then
            Return Nothing
        ElseIf TypeOf KO Is String() Then
            KO = DirectCast(KO, String()).Select(Function(id) New NamedValue(Of String)(id, id)).ToArray
        End If

        If Not TypeOf KO Is NamedValue(Of String)() Then
            Return Internal.debug.stop({
                $"invalid data type for KO mapping statices",
                $"data type: {KO.GetType.FullName}"
            }, env)
        End If

        Dim profiles = DirectCast(KO, NamedValue(Of String)()).LevelAKOStatics.AsDouble
        Return profiles
    End Function

    ''' <summary>
    ''' create kegg catalog profiles data table
    ''' </summary>
    ''' <param name="profiles"></param>
    ''' <returns></returns>
    <ExportAPI("kegg.category_profiles")>
    Public Function KEGGCategoryProfiles(profiles As Dictionary(Of String, Integer)) As EntityObject()
        Return profiles _
            .AsNumeric _
            .KEGGCategoryProfiles _
            .Select(Function(category)
                        Return category.Value _
                            .Select(Function(term)
                                        Return New EntityObject With {
                                            .ID = term.Name,
                                            .Properties = New Dictionary(Of String, String) From {
                                                {"class", category.Key},
                                                {"count", term.Value}
                                            }
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray
    End Function
End Module
