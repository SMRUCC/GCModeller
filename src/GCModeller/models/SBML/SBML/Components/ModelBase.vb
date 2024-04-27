﻿#Region "Microsoft.VisualBasic::67344733743e528d2956322c6d27285a, G:/GCModeller/src/GCModeller/models/SBML/SBML//Components/ModelBase.vb"

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
    '    Code Lines: 17
    ' Comment Lines: 4
    '   Blank Lines: 6
    '     File Size: 813 B


    '     Class ModelBase
    ' 
    '         Function: ToString
    ' 
    '     Class IPartsBase
    ' 
    '         Properties: id, name
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

Namespace Components

    Public MustInherit Class ModelBase : Inherits IPartsBase

        Public Overrides Function ToString() As String
            Return String.Format("<model id=""{0}"" name=""{1}"">", id, name)
        End Function
    End Class

    ''' <summary>
    ''' this object is one of a model element and
    ''' contains id and name attribute value
    ''' </summary>
    Public MustInherit Class IPartsBase

        <Escaped>
        <XmlAttribute> Public Overridable Property id As String
        <XmlAttribute> Public Overridable Property name As String

        Public Overrides Function ToString() As String
            Return id
        End Function
    End Class
End Namespace
