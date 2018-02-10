Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Data
Imports Microsoft.VisualBasic.Language.UnixBash

Public Module ModelLoader

    ''' <summary>
    ''' Load kegg organism model
    ''' </summary>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function LoadKEGGModels(repo As String) As Dictionary(Of String, OrganismModel)
        Return (ls - l - r - "*.Xml" <= repo) _
            .Select(AddressOf LoadXml(Of OrganismModel)) _
            .ToDictionary(Function(org)
                              Return org _
                                  .organism _
                                  .Taxonomy _
                                  .GetTagValue(":", trim:=True) _
                                  .Value
                          End Function)
    End Function
End Module
