#Region "Microsoft.VisualBasic::357ab6b74de1319da70e138eb99fe0c8, Bio.Repository\KEGG\Web\KEGGApi.vb"

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

    '   Total Lines: 160
    '    Code Lines: 82
    ' Comment Lines: 60
    '   Blank Lines: 18
    '     File Size: 5.85 KB


    ' Class KEGGApi
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Find, (+2 Overloads) GetInformation, GetObject, getUrl, List
    ' 
    ' Enum operation
    ' 
    '     [get], conv, ddi, find, info
    '     link, list
    ' 
    '  
    ' 
    ' 
    ' 
    ' Enum database
    ' 
    '     [module], [variant], ag, brite, compound
    '     dgroup, disease, drug, enzyme, genes
    '     genome, glycan, kegg, ko, ligand
    '     network, pathway, rclass, reaction, vg
    '     vp
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Net.Http

''' <summary>
''' 
''' </summary>
Public Class KEGGApi

    Const base As String = "https://rest.kegg.jp"

    ReadOnly proxy As IHttpGet

    Sub New(Optional proxy As IHttpGet = Nothing)
        Me.proxy = proxy
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function getUrl(url As String) As String
        If proxy Is Nothing Then
            Return url.GET
        Else
            Return proxy.GetText(url)
        End If
    End Function

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
        Dim html As String = getUrl(url)
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

        Dim html As String = getUrl(url)
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

    Public Function GetObject(id As String) As String
        Dim url As String = $"{base}/get/{id}"
        Dim html As String = getUrl(url)

        Return html
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
