
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.Rsharp.Runtime
Imports REnv = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' Toolkit for process the kegg brite text file
''' </summary>
<Package("kegg.brite")>
Module britekit

    ''' <summary>
    ''' Convert the kegg brite htext tree to plant table
    ''' </summary>
    ''' <param name="htext"></param>
    ''' <param name="entryIDPattern$"></param>
    ''' <returns></returns>
    <ExportAPI("brite.as.table")>
    Public Function BriteTable(htext As htext, Optional entryIDPattern$ = "[a-z]+\d+") As EntityObject()
        Return htext.Deflate(entryIDPattern) _
            .Select(Function(term)
                        Return New EntityObject With {
                            .ID = term.kegg_id,
                            .Properties = New Dictionary(Of String, String) From {
                                {NameOf(term.class), term.class},
                                {NameOf(term.category), term.category},
                                {NameOf(term.subcategory), term.subcategory},
                                {NameOf(term.order), term.order},
                                {NameOf(term.entry), term.entry.Key},
                                {"name", term.entry.Value}
                            }
                        }
                    End Function) _
            .GroupBy(Function(term) term.ID) _
            .Select(Function(terms) terms.First) _
            .ToArray
    End Function

    ''' <summary>
    ''' Do parse of the kegg brite text file.
    ''' </summary>
    ''' <param name="file">
    ''' The file text content, brite id or its file path
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("brite.parse")>
    Public Function ParseBriteTree(file$, Optional env As Environment = Nothing) As Object
        If file.IsPattern("[a-z]+\d+", RegexICSng) Then
            Select Case file.ToLower
                Case NameOf(htext.br08201) : Return htext.br08201
                Case NameOf(htext.br08204) : Return htext.br08204
                Case CompoundBrite.cpd_br08001,
                     CompoundBrite.cpd_br08002,
                     CompoundBrite.cpd_br08003,
                     CompoundBrite.cpd_br08005,
                     CompoundBrite.cpd_br08006,
                     CompoundBrite.cpd_br08007,
                     CompoundBrite.cpd_br08008,
                     CompoundBrite.cpd_br08009,
                     CompoundBrite.cpd_br08010,
                     CompoundBrite.cpd_br08021

                    Return htext.GetInternalResource(file)
                Case Else
                    Return REnv.debug.stop({$"Invalid brite id: {file}", $"brite id: {file}"}, env)
            End Select
        Else
            Return htext.StreamParser(res:=file)
        End If
    End Function

    ''' <summary>
    ''' Do parse of the kegg brite json file.
    ''' </summary>
    ''' <param name="file$"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("brite.parseJSON")>
    Public Function ParseBriteJson(file$, Optional env As Environment = Nothing) As Object

    End Function
End Module
