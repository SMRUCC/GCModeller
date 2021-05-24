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
