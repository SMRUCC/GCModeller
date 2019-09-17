#Region "Microsoft.VisualBasic::dee925b0a98be0252515d0834a295703, Bioconductor\Bioconductor\Web\pkgInfo\Details\Details.vb"

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

    '     Class Details
    ' 
    '         Properties: [Imports], biocViews, Depends, DependsOnMe, Enhances
    '                     ImportsMe, License, LinkingTo, Since, Suggests
    '                     SuggestsMe, SystemRequirements, URL, Version
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace Web.Packages

    Public Class Details

        <Column("biocViews")> Public Property biocViews As String
        <Column("Version")> Public Property Version As String
        <Column("In Bioconductor since")>
        Public Property Since As String
        <Column("License")> Public Property License As String
        <Column("Depends")> Public Property Depends As String
        <Column("[Imports]")> Public Property [Imports] As String
        <Column("LinkingTo")> Public Property LinkingTo As String
        <Column("Suggests")> Public Property Suggests As String
        <Column("SystemRequirements")> Public Property SystemRequirements As String
        <Column("Enhances")> Public Property Enhances As String
        <Column("URL")> Public Property URL As String
        <Column("Depends On Me")> Public Property DependsOnMe As String
        <Column("Imports Me")> Public Property ImportsMe As String
        <Column("Suggests Me")> Public Property SuggestsMe As String

    End Class
End Namespace
