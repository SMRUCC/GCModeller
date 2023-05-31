#Region "Microsoft.VisualBasic::cd67169cb1335e17930876c93911c883, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\PathwayProfiles.vb"

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

    '   Total Lines: 123
    '    Code Lines: 96
    ' Comment Lines: 18
    '   Blank Lines: 9
    '     File Size: 5.91 KB


    '     Module PathwayProfiles
    ' 
    '         Function: doProfiles, GetPathwayClass, GetProfileMapping, KEGGCategoryProfiles
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Public Module PathwayProfiles

        ''' <summary>
        ''' mapping kegg category to a given meta id list
        ''' </summary>
        ''' <param name="maps">a collection of kegg maps contains kegg id data</param>
        ''' <param name="metainfo">
        ''' a mapping of ``[keggId => metaID]``
        ''' </param>
        ''' <returns>
        ''' a mapping of ``[kegg_category => metaID()]``
        ''' </returns>
        ''' 
        <Extension>
        Public Function GetProfileMapping(maps As IEnumerable(Of Map), metainfo As Dictionary(Of String, String)) As Dictionary(Of String, String())
            ' get category data of the kegg pathway maps
            Dim brite As Dictionary(Of String, String()) = BriteHEntry.Pathway _
               .LoadDictionary _
               .GroupBy(Function(p) p.Value.class) _
               .ToDictionary(Function(p) p.Key,
                             Function(p)
                                 Return p _
                                    .Values _
                                    .Select(Function(d) d.EntryId.Match("\d+")) _
                                    .ToArray
                             End Function)
            Dim mapIndex As Dictionary(Of String, String()) = maps _
                .GroupBy(Function(m) m.EntryId.Match("\d+")) _
                .ToDictionary(Function(m) m.Key,
                              Function(m)
                                  Return m _
                                     .Select(Function(map) map.shapes) _
                                     .IteratesALL _
                                     .Select(Function(a) a.IDVector) _
                                     .IteratesALL _
                                     .Distinct _
                                     .ToArray
                              End Function)
            Dim getMapping = Function(c As KeyValuePair(Of String, String())) As String()
                                 Dim mapId As String() = c.Value
                                 Dim mapList As String() = mapId _
                                    .Where(Function(id) mapIndex.ContainsKey(id)) _
                                    .Select(Function(id) mapIndex(id)) _
                                    .IteratesALL _
                                    .Distinct _
                                    .ToArray
                                 Dim mapping As String() = mapList _
                                    .Where(Function(id) metainfo.ContainsKey(id)) _
                                    .Select(Function(id) metainfo(id)) _
                                    .Distinct _
                                    .ToArray

                                 Return mapping
                             End Function

            Return brite _
                .ToDictionary(Function(c) c.Key,
                              Function(c)
                                  Return getMapping(c)
                              End Function)
        End Function

        <Extension>
        Private Function doProfiles(group As Pathway(), profiles As Dictionary(Of String, Double)) As NamedValue(Of Double)()
            Return group _
               .Where(Function(p)
                          Return profiles.ContainsKey(p.EntryId) AndAlso profiles(p.EntryId) > 0
                      End Function) _
               .Select(Function(p As Pathway)
                           Return New NamedValue(Of Double) With {
                               .Name = p.entry.text,
                               .Value = profiles(p.EntryId),
                               .Description = p.category & ":" & p.EntryId
                           }
                       End Function) _
               .ToArray
        End Function

        ''' <summary>
        ''' load pathway category class information from the internal database resource
        ''' </summary>
        ''' <returns>
        ''' pathway entry id is integer number, zero padding may be exists
        ''' </returns>
        Public Function GetPathwayClass() As Dictionary(Of String, BriteHEntry.Pathway())
            Return BriteHEntry.Pathway _
                .LoadDictionary _
                .GroupBy(Function(p) p.Value.class) _
                .ToDictionary(Function(p) p.Key,
                              Function(p)
                                  Return p.Values
                              End Function)
        End Function

        <ExportAPI("kegg.category_profiles")>
        <Extension>
        Public Function KEGGCategoryProfiles(profiles As Dictionary(Of String, Double)) As Dictionary(Of String, NamedValue(Of Double)())
            Dim brite As Dictionary(Of String, BriteHEntry.Pathway()) = GetPathwayClass()

            profiles = profiles.ToDictionary(Function(a) a.Key.Match("\d+"),
                                             Function(a)
                                                 Return a.Value
                                             End Function)
            Return brite _
               .ToDictionary(Function(p) p.Key,
                             Function(group)
                                 Return group.Value _
                                    .doProfiles(profiles) _
                                    .OrderByDescending(Function(t) t.Value) _
                                    .ToArray
                             End Function)
        End Function
    End Module
End Namespace
