#Region "Microsoft.VisualBasic::20e186d8f63ab77a9411ece29fd091a4, R#\kegg_kit\activatorUtils.vb"

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

    '   Total Lines: 74
    '    Code Lines: 67
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 2.85 KB


    ' Module activatorUtils
    ' 
    '     Function: GetDbLinks, GetGeneName, GetNameValues, GetPropertyValues, GetReference
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports any = Microsoft.VisualBasic.Scripting

Module activatorUtils

    <Extension>
    Public Function GetPropertyValues(data As dataframe) As [Property]()
        Return data.forEachRow({"id", "name", "link"}) _
            .Select(Function(r)
                        Return New [Property] With {
                            .name = any.ToString(r(0)),
                            .value = any.ToString(r(1)),
                            .comment = any.ToString(r(2))
                        }
                    End Function) _
            .ToArray
    End Function


    <Extension>
    Public Function GetNameValues(data As dataframe) As NamedValue()
        Return data.forEachRow({"id", "name"}) _
            .Select(Function(r)
                        Return New NamedValue With {
                            .name = any.ToString(r(0)),
                            .text = any.ToString(r(1))
                        }
                    End Function) _
            .ToArray
    End Function

    <Extension>
    Public Function GetGeneName(data As dataframe) As GeneName()
        Return data.forEachRow({"id", "name"}) _
            .Select(Function(r)
                        Return New GeneName With {
                            .geneId = any.ToString(r(0)),
                            .description = any.ToString(r(1))
                        }
                    End Function) _
            .ToArray
    End Function

    <Extension>
    Public Function GetDbLinks(data As dataframe) As DBLink()
        Return data.forEachRow({"db", "id", "link"}) _
            .Select(Function(r)
                        Return New DBLink With {
                            .DBName = any.ToString(r(0)),
                            .Entry = any.ToString(r(1)),
                            .link = any.ToString(r(2))
                        }
                    End Function) _
            .ToArray
    End Function

    <Extension>
    Public Function GetReference(data As dataframe) As Reference()
        Return data.forEachRow({"reference", "authors", "title", "journal"}) _
            .Select(Function(r)
                        Return New Reference With {
                            .Reference = any.ToString(r(Scan0)),
                            .Authors = any.ToString(r(1)).StringSplit(",\s+"),
                            .Title = any.ToString(r(2)),
                            .Journal = any.ToString(r(3))
                        }
                    End Function) _
            .ToArray
    End Function
End Module
