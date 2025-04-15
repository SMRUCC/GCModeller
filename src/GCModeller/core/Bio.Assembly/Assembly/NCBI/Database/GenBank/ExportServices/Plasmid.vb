#Region "Microsoft.VisualBasic::da96e3a5998d1d0275fbfb87c0eceecc, core\Bio.Assembly\Assembly\NCBI\Database\GenBank\ExportServices\Plasmid.vb"

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

    '   Total Lines: 30
    '    Code Lines: 22 (73.33%)
    ' Comment Lines: 3 (10.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (16.67%)
    '     File Size: 1.14 KB


    '     Class Plasmid
    ' 
    '         Properties: Country, Host, isolation_source, IsShortGun, PlasmidID
    '                     PlasmidType
    ' 
    '         Function: Build
    ' 
    ' 
    ' /********************************************************************************/

#End Region


Namespace Assembly.NCBI.GenBank.CsvExports

    ''' <summary>
    ''' tabular information about a plasmid replicate
    ''' </summary>
    Public Class Plasmid : Inherits gbEntryBrief

        Public Property PlasmidID As String
        Public Property PlasmidType As String
        Public Property isolation_source As String
        Public Property Country As String
        Public Property Host As String
        Public ReadOnly Property IsShortGun As Boolean
            Get
                Return InStr(Definition, "shotgun", CompareMethod.Text) > 0
            End Get
        End Property

        Public Overloads Shared Function Build(gbk As NCBI.GenBank.GBFF.File) As Plasmid
            Dim Plasmid As Plasmid = ConvertObject(Of Plasmid)(gbk)
            Plasmid.PlasmidID = gbk.Features.source.Query("plasmid")
            Plasmid.Host = gbk.Features.source.Query("host")
            Plasmid.Country = gbk.Features.source.Query("country")
            Plasmid.isolation_source = gbk.Features.source.Query("isolation_source")

            Return Plasmid
        End Function
    End Class
End Namespace
