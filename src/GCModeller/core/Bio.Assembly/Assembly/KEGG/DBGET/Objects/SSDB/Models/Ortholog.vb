#Region "Microsoft.VisualBasic::a3fa61f1589fbe61f8f409d33a6a53ec, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\SSDB\Models\Ortholog.vb"

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

    '   Total Lines: 62
    '    Code Lines: 47
    ' Comment Lines: 9
    '   Blank Lines: 6
    '     File Size: 2.38 KB


    '     Class Ortholog
    ' 
    '         Properties: bestAll, bits, Definition, hit_name, identity
    '                     KO, len, LocusId, margin, overlap
    '                     query_length, sp, SW
    ' 
    '         Function: createObject, CreateObjects, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

#If netcore5 = 0 Then
Imports System.Data.Linq.Mapping
#Else
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
#End If
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.KEGG.DBGET.bGetObject.SSDB

    ''' <summary>
    ''' ``*.csv``
    ''' 
    ''' > http://www.kegg.jp/ssdb-bin/ssdb_best?org_gene=sp:locus_tag
    ''' </summary>
    Public Class Ortholog

        Public Property sp As String
        ''' <summary>
        ''' query_name
        ''' </summary>
        ''' <returns></returns>
        Public Property LocusId As String
        <Column(Name:="KEGG_Entry")> Public Property hit_name As String
        Public Property Definition As String
        Public Property KO As String
        Public Property query_length As Integer
        <Column(Name:="Length")> Public Property len As Integer
        <Column(Name:="SW-score")> Public Property SW As Double
        <Column(Name:="Margin")> Public Property margin As Integer
        Public Property bits As Double
        Public Property identity As Double
        Public Property overlap As Double
        <Column(Name:="best(all)")> Public Property bestAll As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function CreateObjects(result As SSDB.OrthologREST) As Ortholog()
            Return result.Orthologs.Select(Function(x) createObject(result, x)).ToArray
        End Function

        Private Shared Function createObject(result As SSDB.OrthologREST, hit As SShit) As Ortholog
            Return New Ortholog With {
                .bestAll = $"{hit.Best.Key} {hit.Best.Value}",
                .bits = Val(hit.Bits),
                .Definition = hit.Entry.description,
                .hit_name = hit.Entry.locusID,
                .LocusId = result.KEGG_ID,
                .identity = Val(hit.Identity),
                .KO = hit.KO.Key,
                .len = Val(hit.Length),
                .margin = Val(hit.Margin),
                .overlap = Val(hit.Overlap),
                .sp = hit.Entry.speciesID,
                .SW = Val(hit.SWScore),
                .query_length = Strings.Len(result.Sequence)
            }
        End Function
    End Class
End Namespace
