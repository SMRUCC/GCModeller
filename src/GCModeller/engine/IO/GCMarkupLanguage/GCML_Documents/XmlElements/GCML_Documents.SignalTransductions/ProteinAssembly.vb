#Region "Microsoft.VisualBasic::e3be1c938af37d8d3ebae9482c5c3d9b, engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\GCML_Documents.SignalTransductions\ProteinAssembly.vb"

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

    '     Class ProteinAssembly
    ' 
    '         Properties: ComplexComponents
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace GCML_Documents.XmlElements.SignalTransductions

    ''' <summary>
    ''' 蛋白质相互做用的规则，与化学反应遵守同样的动力学原理
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ProteinAssembly : Inherits Reaction
        Implements INamedValue

#Region "Public Properties"
        ''' <summary>
        ''' 指向代谢组中的底物列表的所生成的蛋白质复合物的UniqueId
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ComplexComponents As String()
#End Region
    End Class
End Namespace
