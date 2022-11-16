Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

''' <summary>
''' 
''' </summary>
Public Class KEGGApi

    Const base As String = "https://rest.kegg.jp"

    ''' <summary>
    ''' This operation displays the database release information with statistics for the databases shown in Table 1. Except for kegg, genes and ligand, this operation also displays the list of linked databases that can be used in the link operation.
    ''' </summary>
    ''' <param name="database"></param>
    ''' <returns></returns>
    Public Function GetInformation(database As database) As String
        Return GetInformation(database.Description)
    End Function

    ''' <summary>
    ''' This operation displays the database release information with statistics for the databases shown in Table 1. Except for kegg, genes and ligand, this operation also displays the list of linked databases that can be used in the link operation.
    ''' </summary>
    ''' <param name="database"></param>
    ''' <returns></returns>
    Public Function GetInformation(database As String) As String
        Dim url As String = $"{base}/info/{database}"
        Dim html As String = url.GET
        Return html
    End Function

    ''' <summary>
    ''' This operation can be used to obtain a list of all entries in each database. The database names shown in Tables 1 and 2, excluding the composite database names of genes, ligand and kegg, may be given. The special database name "organism" is allowed only in this operation, which may be used to obtain a list of KEGG organisms with the three- or four-letter organism codes.
    ''' The option "xl" Is applicable only to the brite database for listing binary relation files, which are used to expand brite hierarchies by the Join Brite tool in KEGG Mapper.
    ''' When the organism code Is known, the second form can be used to obtain a list of organism-specific pathways Or modules.
    ''' The third form may be used To obtain a list Of definitions For a given Set Of database entry identifiers. The maximum number Of identifiers that can be given Is 10.
    ''' </summary>
    ''' <param name="database"></param>
    ''' <param name="org"></param>
    ''' <returns></returns>
    Public Function List(database As String, Optional org As String = Nothing) As Dictionary(Of String, String)
        Dim url As String = $"{base}/list/{database}"

        If Not org.StringEmpty Then
            url = $"{url}/{org}"
        End If

        Dim html As String = url.GET
        Dim lines As String() = html.LineTokens.Where(Function(str) Not str.StringEmpty).ToArray
        Dim dataset As New Dictionary(Of String, String)
        Dim tuple As NamedValue(Of String)

        For Each line As String In lines
            tuple = line.GetTagValue(vbTab, trim:=True)
            dataset.Add(tuple.Name, tuple.Value)
        Next

        Return dataset
    End Function

    ''' <summary>
    ''' This is a search operation. The first form searches entry identifier 
    ''' and associated fields shown below for matching keywords.
    '''
    ''' |Database	|Text search fields (see flat file format)|
    ''' |----------|-----------------------------------------|
    ''' |pathway|	ENTRY And NAME|
    ''' |module| ENTRY And NAME|
    ''' |ko	   |ENTRY, NAME And DEFINITION|
    ''' |genes (&lt;org>, vg, vp, ag)|	ENTRY, ORTHOLOGY, NAME And DEFINITION|
    ''' |genome	|ENTRY, NAME And DEFINITION|
    ''' |compound|	ENTRY And NAME|
    ''' |glycan	|ENTRY, NAME And COMPOSITION|
    ''' |reaction|	ENTRY, NAME And DEFINITION|
    ''' |rclass	|ENTRY And DEFINITION|
    ''' |enzyme	|ENTRY And NAME|
    ''' |network|	ENTRY And NAME|
    ''' |variant|	ENTRY And NAME|
    ''' |disease|	ENTRY And NAME|
    ''' |drug	|ENTRY And NAME|
    ''' |dgroup|	ENTRY And NAME|
    '''
    ''' Keyword search against brite Is Not supported. Use /list/brite To 
    ''' retrieve a Short list.
    '''
    ''' In the second form the chemical formula search Is a partial match 
    ''' irrespective of the order of atoms given. The exact mass (Or molecular 
    ''' weight) Is checked by rounding off to the same decimal place as the
    ''' query data. A range of values may also be specified with the minus(-) 
    ''' sign.
    ''' </summary>
    ''' <param name="database"></param>
    ''' <param name="query"></param>
    ''' <param name="[option]"></param>
    ''' <returns></returns>
    Public Function Find(database As String,
                         query As String,
                         Optional [option] As String = Nothing)

    End Function
End Class

Public Enum operation
    ''' <summary>
    ''' info – display database release information and linked db information
    ''' </summary>
    info
    list
    find
    [get]
    conv
    link
    ddi
End Enum

Public Enum database
    kegg
    pathway
    brite
    [module]
    ko
    genes
    vg
    vp
    ag
    genome
    ligand
    compound
    glycan
    reaction
    rclass
    enzyme
    network
    [variant]
    disease
    drug
    dgroup
End Enum