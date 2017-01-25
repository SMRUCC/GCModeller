#Region "Microsoft.VisualBasic::6163d67cd40ed3595ec473181df0cf5a, ..\GCModeller\models\Networks\STRING\LDM\Network.vb"

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
Imports Microsoft.VisualBasic.Text

<XmlRoot("StrP_Network", Namespace:="http://code.google.com/p/genome-in-code/interaction_network/signal_transduction_network/")>
Public Class Network : Implements ISaveHandle

    <XmlElement> Public Property Pathway As Pathway()

    Public Function Save(Optional Path As String = "", Optional encoding As Text.Encoding = Nothing) As Boolean Implements ISaveHandle.Save
        Return Me.GetXml.SaveTo(Path, encoding)
    End Function

    Public Function Save(Optional Path As String = "", Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return Save(Path, encoding.GetEncodings)
    End Function
End Class
