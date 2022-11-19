#Region "Microsoft.VisualBasic::d97b5940b4166577786ed4d48a0481fb, GCModeller\models\SBML\SBML\Components\Specie.vb"

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

    '   Total Lines: 24
    '    Code Lines: 19
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 785 B


    '     Class Specie
    ' 
    '         Properties: boundaryCondition, compartmentId, id
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Model.SBML.FLuxBalanceModel
Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

Namespace Components

    Public Class Specie : Inherits IPartsBase
        Implements INamedValue

        <Escaped> <XmlAttribute>
        Public Overrides Property id As String Implements INamedValue.Key

        <Escaped>
        <XmlAttribute("compartment")>
        Public Overridable Property compartmentId As String
        <XmlAttribute()>
        Public Property boundaryCondition As Boolean

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]{1} [{2}]", id, name, compartmentId)
        End Function
    End Class
End Namespace
