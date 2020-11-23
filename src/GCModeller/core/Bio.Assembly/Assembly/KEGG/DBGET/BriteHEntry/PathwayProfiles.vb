#Region "Microsoft.VisualBasic::9450ae5ad1805b937954db63b639c2a5, core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\PathwayProfiles.vb"

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

    '     Module PathwayProfiles
    ' 
    '         Function: doProfiles, KEGGCategoryProfiles
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Public Module PathwayProfiles

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
                               .Description = p.entry.text
                           }
                       End Function) _
               .ToArray
        End Function

        <ExportAPI("kegg.category_profiles")>
        <Extension>
        Public Function KEGGCategoryProfiles(profiles As Dictionary(Of String, Double)) As Dictionary(Of String, NamedValue(Of Double)())
            Dim brite As Dictionary(Of String, BriteHEntry.Pathway()) = BriteHEntry.Pathway _
                .LoadDictionary _
                .GroupBy(Function(p) p.Value.class) _
                .ToDictionary(Function(p) p.Key,
                              Function(p)
                                  Return p.Values
                              End Function)

            profiles = profiles.ToDictionary(
                Function(a) a.Key.Match("\d+"),
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
