#Region "Microsoft.VisualBasic::2ae8f14586faff9ce5e9f78d9079f19f, ..\interops\visualize\Circos\Circos\TrackDatas\Adapter\Highlights\Name.vb"

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

Imports System.Drawing
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Namespace TrackDatas.Highlights

    Public Class Name

        <Column("locus_tag")>
        Public Property Locus As String
        Public Property Name As String
        Public Property Minimum As Integer
        Public Property Maximum As Integer

        Public Function ToMeta() As ValueTrackData
            Dim n As Integer() = {Minimum, Maximum}
            Dim s As New StringBuilder(Regex.Replace(Name, "\s+", "_"))

            Call s.Replace(",", "_")
            Call s.Replace(";", "_")
            Call s.Replace(".", "_")
            Call s.Replace("=", "_")

            Dim ss As String = s.ToString
            ss = Regex.Replace(ss, "[_]+", "_")

            Return New ValueTrackData With {
                .start = n.Min,
                .end = n.Max,
                .value = ss.ParseDouble
            }
        End Function

        Public Function Loci() As Location
            Return New Location(Minimum, Maximum)
        End Function

        Public Shared Iterator Function MatchLocus(source As IEnumerable(Of Name), PTT As PTT) As IEnumerable(Of Name)
            For Each x As Name In source.AsParallel
                Dim matched As GeneBrief = __matches(x, PTT)

                If matched Is Nothing Then
                    Continue For
                End If

                x.Locus = matched.Synonym

                Yield x
            Next
        End Function

        Private Shared Function __matches(loci As Name, PTT As PTT) As GeneBrief
            Dim lcl As Location = loci.Loci
            Dim LQuery As GeneBrief =
                LinqAPI.DefaultFirst(Of GeneBrief) <= From x As GeneBrief
                                                      In PTT.GeneObjects
                                                      Where lcl.Equals(x.Location, 10)
                                                      Select x
            Return LQuery
        End Function
    End Class
End Namespace
