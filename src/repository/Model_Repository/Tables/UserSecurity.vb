#Region "Microsoft.VisualBasic::140f39df9a916e4784ad575e14109ce0, Model_Repository\Tables\UserSecurity.vb"

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

    '     Class UserSecurity
    ' 
    '         Properties: Group, Password, UserName
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Data.Linq.Mapping

Namespace Tables

    <Table(name:="user_security")>
    Public Class UserSecurity

        <Column(isprimarykey:=True, dbtype:="varchar(512)")> Public Property UserName As String
        <Column(dbtype:="varchar(512)")> Public Property Password As String
        <Column(name:="user_group")> Public Property Group As Integer

        Public Overrides Function ToString() As String
            Return String.Format("{0}//     {1}  | {2}", Group, UserName, Password)
        End Function
    End Class
End Namespace
