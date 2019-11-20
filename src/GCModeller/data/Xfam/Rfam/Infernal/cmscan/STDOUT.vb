#Region "Microsoft.VisualBasic::af1ecb4bb2ba9f685f83b196f37476ce, data\Xfam\Rfam\Infernal\cmscan\STDOUT.vb"

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

    '     Class ScanSites
    ' 
    '         Properties: QueryHits
    ' 
    '     Class Query
    ' 
    '         Properties: Description, hits, Length, title, uncertainHits
    ' 
    '     Class Hit
    ' 
    '         Properties: bias, description, Evalue, gc, mdl
    '                     modelname, rank, score, trunc
    ' 
    '         Function: ToString
    ' 
    '     Class HitAlign
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Infernal.cmscan

    Public Class ScanSites : Inherits Infernal.STDOUT
        Public Property QueryHits As Query
    End Class

    Public Class Query
        Public Property title As String
        Public Property Length As Long
        Public Property Description As String
        Public Property hits As Hit()
        ''' <summary>
        ''' ------ inclusion threshold ------
        ''' </summary>
        ''' <returns></returns>
        Public Property uncertainHits As Hit()
    End Class

    Public Class Hit : Inherits IHit

        <XmlAttribute> Public Property rank As String
        <XmlAttribute> <Column("E-value")>
        Public Property Evalue As Double
        <XmlAttribute> Public Property score As Double
        <XmlAttribute> Public Property bias As Double
        <XmlAttribute> Public Property modelname As String
        <XmlAttribute> Public Property mdl As String
        <XmlAttribute> Public Property trunc As String
        <XmlAttribute> Public Property gc As Double
        <XmlAttribute> Public Property description As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class HitAlign


    End Class
End Namespace
