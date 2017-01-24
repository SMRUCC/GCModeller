#Region "Microsoft.VisualBasic::d5ad3152235f0f884b42e32bf647bc69, ..\GCModeller\models\Networks\STRING\TCS\TCS.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace TCS

    <XmlType("TCS", Namespace:="http://code.google.com/p/genome-in-code/interaction_network/tcs")> Public Class TCS
        <XmlAttribute> Public Property Chemotaxis As String

        <XmlAttribute> Public Property HK As String
        <XmlAttribute> Public Property RR As String

        <XmlAttribute("Chemotaxis_HK_Confidence")> Public Property ChemotaxisHKConfidence As Double
        <XmlAttribute("HK_RR_Confidence")> Public Property HKRRConfidence As Double
        <XmlAttribute("RR_TF_Confidence")> Public Property RRTFConfidence As Double

        Public Overrides Function ToString() As String
            Return String.Format("{0} -> {1} -> {2}", Chemotaxis, HK, RR)
        End Function

        Public Function Exists(Id As String) As Boolean
            Return String.Equals(Id, Me.Chemotaxis) OrElse String.Equals(Id, Me.HK) OrElse String.Equals(Id, Me.RR)
        End Function
    End Class
End Namespace
