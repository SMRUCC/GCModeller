#Region "Microsoft.VisualBasic::9c0e63ffeb6a645c89501b4c021bc160, GCModeller\models\SBML\SBML\Level2\FluxModel\kineticLaw\Parameter.vb"

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

    '   Total Lines: 27
    '    Code Lines: 20
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 849 B


    '     Structure MathInfo
    ' 
    '         Function: ToString
    ' 
    '     Class parameter
    ' 
    '         Properties: id, units, value
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Level2.Elements

    <XmlType("math")> Public Structure MathInfo

        <XmlElement()> Dim ci As String()

        Public Overrides Function ToString() As String
            Return String.Join(vbCrLf, ci)
        End Function
    End Structure

    Public Class parameter : Implements INamedValue

        <XmlAttribute()> Public Property id As String Implements INamedValue.Key
        <XmlAttribute()> Public Property value As Double
        <XmlAttribute()> Public Property units As String

        Public Overrides Function ToString() As String
            Return String.Format("{0} = {1} ({2})", id, value, units)
        End Function
    End Class
End Namespace
