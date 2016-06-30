#Region "Microsoft.VisualBasic::51ca6a913a04c82964946784adfe0b67, ..\Bio.Assembly\Assembly\EBI.ChEBI\Database\IO.StreamProviders\Tables.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Namespace Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables

    Public Class BaseElements
        Public Property ID As String
        Public Property COMPOUND_ID As String
        Public Property TYPE As String
        Public Property SOURCE As String

        Public Overrides Function ToString() As String
            Return String.Format("{0} {1} {2}", TYPE, SOURCE, ID)
        End Function
    End Class

    Public Class Names : Inherits BaseElements

        Public Property NAME As String
        Public Property ADAPTED As String
        Public Property LANGUAGE As String

        Public Overrides Function ToString() As String
            Return String.Format("({0}) [{1}]{2}", ID, LANGUAGE, NAME)
        End Function
    End Class

    Public Class DatabaseAccession : Inherits BaseElements
        Public Property ACCESSION_NUMBER As String

        Public Overrides Function ToString() As String
            Return ACCESSION_NUMBER
        End Function
    End Class
End Namespace
