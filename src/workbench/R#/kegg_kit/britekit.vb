
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

<Package("kegg.brite")>
Module britekit

    <ExportAPI("brite.as.table")>
    Public Function BriteTable(htext As htext) As EntityObject()
        Return htext.GetEntryDictionary _
            .Values _
            .Select(Function(term)
                        Return New EntityObject With {
                            .ID = term.entryID,
                            .Properties = New Dictionary(Of String, String) From {
                                {NameOf(term.CategoryLevel), term.CategoryLevel},
                                {"numberOfSubCategoryChilds", term.categoryItems.SafeQuery.Count},
                                {NameOf(term.classLabel), term.classLabel},
                                {NameOf(term.level), term.level},
                                {NameOf(term.class), term.class},
                                {NameOf(term.description), term.description}
                            }
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' Do parse of the kegg brite text file.
    ''' </summary>
    ''' <param name="file">
    ''' The file text content or its file path
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("brite.parse")>
    Public Function ParseBriteTree(file As String) As htext
        Return htext.StreamParser(res:=file)
    End Function
End Module
