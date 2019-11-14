#Region "Microsoft.VisualBasic::69c70182ef50bba7c302b44922083c36, engine\GCModeller\GUID\Category.vb"

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

    ' Class Guid
    ' 
    ' 
    '     Structure CategoryItems
    ' 
    '         Function: ToString
    '         Operators: <>, =
    '         Structure Entity
    ' 
    ' 
    ' 
    '         Structure [Module]
    ' 
    ' 
    ' 
    '         Structure Feature
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Partial Class Guid

    ''' <summary>
    ''' 第二段小分类
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure CategoryItems

        ''' <summary>
        ''' Entity represents a bio-macromolecule instance.
        ''' ('实体'代表着一个生物大分子对象)
        ''' </summary>
        ''' <remarks></remarks>
        Public Structure Entity
            Public Shared ReadOnly Polypeptide As CategoryItems = New CategoryItems With {._description = "Polypeptide", ._hash = "bcf7-e99b", ._class = Guid.Classes.Entity}
            Public Shared ReadOnly mRNA As CategoryItems = New CategoryItems With {._description = "mRNA", ._hash = "e656-4558", ._class = Guid.Classes.Entity}
            Public Shared ReadOnly tRNA As CategoryItems = New CategoryItems With {._description = "tRNA", ._hash = "a3e1-0ee6", ._class = Guid.Classes.Entity}
            Public Shared ReadOnly rRNA As CategoryItems = New CategoryItems With {._description = "rRNA", ._hash = "489c-308b", ._class = Guid.Classes.Entity}
            Public Shared ReadOnly Terminator As CategoryItems = New CategoryItems With {._description = "Terminator", ._hash = "0cd5-1189", ._class = Guid.Classes.Entity}
            Public Shared ReadOnly Promoter As CategoryItems = New CategoryItems With {._description = "Promoter", ._hash = "c37c-e395", ._class = Guid.Classes.Entity}
            Public Shared ReadOnly Gene As CategoryItems = New CategoryItems With {._description = "Gene", ._hash = "653d-6d26", ._class = Guid.Classes.Entity}
            Public Shared ReadOnly DNA As CategoryItems = New CategoryItems With {._description = "DNA", ._hash = "c33f-9369", ._class = Guid.Classes.Entity}
            Public Shared ReadOnly Compound As CategoryItems = New CategoryItems With {._description = "Compound", ._hash = "f939-cc4d", ._class = Guid.Classes.Entity}
        End Structure

        ''' <summary>
        ''' Module represents a information organization form of the entity.
        ''' ('模块'代表着一些实体对象的信息组织形式，例如生化反应、代谢途径、
        ''' 调控网络、信号网络等) 
        ''' </summary>
        ''' <remarks></remarks>
        Public Structure [Module]
            Public Shared ReadOnly Reaction As CategoryItems = New CategoryItems With {._description = "Reaction", ._hash = "2d38-57c8", ._class = Guid.Classes.Module}
            Public Shared ReadOnly Pathway As CategoryItems = New CategoryItems With {._description = "Pathway", ._hash = "b875-27f8", ._class = Guid.Classes.Module}
            Public Shared ReadOnly ProteinComplexes As CategoryItems = New CategoryItems With {._description = "ProteinComplexes", ._hash = "1198-645f", ._class = Guid.Classes.Module}
            Public Shared ReadOnly Regulation As CategoryItems = New CategoryItems With {._description = "Regulation", ._hash = "361f-4163", ._class = Guid.Classes.Module}
            Public Shared ReadOnly Enzyme As CategoryItems = New CategoryItems With {._description = "Enzyme", ._hash = "5aa9-2229", ._class = Guid.Classes.Module}
            Public Shared ReadOnly Cell As CategoryItems = New CategoryItems With {._description = "Cell", ._hash = "0000-0000", ._class = Classes.Module}
        End Structure

        ''' <summary>
        ''' Element represents a functional region part in the 
        ''' bio-macromolecule entity.
        ''' ('元素'代表着生物大分子实体中的功能性位点、结构域)  
        ''' </summary>
        ''' <remarks></remarks>
        Public Structure Feature
            Public Shared ReadOnly DNABindSite As CategoryItems = New CategoryItems With {._description = "DNABindSite", ._hash = "234b-eaef", ._class = Guid.Classes.Feature}
            Public Shared ReadOnly ProteinFeature As CategoryItems = New CategoryItems With {._description = "ProteinFeature", ._hash = "070b-8702", ._class = Guid.Classes.Feature}
        End Structure

        Friend _description As String
        Friend _hash As String
        Friend _class As SMRUCC.genomics.GCModeller.ModellingEngine.Guid.Classes

        Public Overrides Function ToString() As String
            Return _description
        End Function

        Public Shared Widening Operator CType(hash As String) As CategoryItems
            Return New CategoryItems With {._hash = hash}
        End Operator

        Public Shared Operator =(e As CategoryItems, f As CategoryItems) As Boolean
            Return String.Equals(e._hash, f._hash)
        End Operator

        Public Shared Operator <>(e As CategoryItems, f As CategoryItems) As Boolean
            Return Not String.Equals(e._hash, f._hash)
        End Operator

        Public Shared Narrowing Operator CType(e As CategoryItems) As String
            Return e._hash
        End Operator
    End Structure
End Class
