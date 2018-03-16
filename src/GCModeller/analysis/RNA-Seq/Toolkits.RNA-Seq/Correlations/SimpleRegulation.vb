#Region "Microsoft.VisualBasic::5abf0b62e0ce35482cf225d98f3894a2, analysis\RNA-Seq\Toolkits.RNA-Seq\Correlations\SimpleRegulation.vb"

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

    ' Class SimpleRegulation
    ' 
    '     Properties: Operon, PccValue, Regulator
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Public Class SimpleRegulation
    <XmlAttribute> Public Property Regulator As String
    <XmlAttribute> Public Property Operon As String
    <XmlAttribute> Public Property PccValue As Double
End Class
