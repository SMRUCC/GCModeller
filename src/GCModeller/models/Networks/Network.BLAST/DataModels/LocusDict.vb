#Region "Microsoft.VisualBasic::92a76aa07b99f7c81ddb36d994cb5ad9, Network.BLAST\DataModels\LocusDict.vb"

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

    '     Class LocusDict
    ' 
    '         Properties: genome, LocusId
    ' 
    '         Function: CreateDictionary, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace LDM

    Public Class LocusDict

        <XmlAttribute> Public Property genome As String
        <XmlAttribute> Public Property LocusId As String()

        Public Overrides Function ToString() As String
            Return genome
        End Function

        Public Shared Function CreateDictionary(lst As IEnumerable(Of LocusDict)) As Dictionary(Of String, String)
            Dim LQuery As Dictionary(Of String, String) =
                (From x As LocusDict In lst.AsParallel
                 Let lstLocus = x.LocusId.Distinct.ToArray
                 Select (From sId As String
                         In lstLocus
                         Select sId, x.genome).ToArray).IteratesALL _
                             .ToDictionary(Function(x) x.sId,
                                           Function(x) x.genome)
            Return LQuery
        End Function
    End Class
End Namespace
