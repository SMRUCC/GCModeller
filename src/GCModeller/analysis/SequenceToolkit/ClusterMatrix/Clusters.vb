#Region "Microsoft.VisualBasic::f3593a054e1714d81c5267491912a88b, ..\GCModeller\analysis\SequenceToolkit\ClusterMatrix\Clusters.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Clusters

    ''' <summary>
    ''' Using first token in the fasta title as the sequence uid
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="delimiter$"></param>
    <Extension>
    Public Sub FirstTokenID(ByRef source As FastaFile, Optional delimiter$ = FastaToken.DefaultHeaderDelimiter)
        Dim tokens As Func(Of FastaToken, String())

        If delimiter = FastaToken.DefaultHeaderDelimiter Then
            tokens = Function(f) {
                f.Attributes(Scan0)
            }
        Else
            tokens = Function(f) {
                Strings.Split(f.Title, delimiter)(Scan0)
            }
        End If

        For Each f As FastaToken In source
            f.Attributes = tokens(f)
        Next
    End Sub

    <Extension>
    Public Function KMeans(data As IEnumerable(Of DataSet), Optional expected% = 20) As EntityClusterModel()
        Dim models As EntityClusterModel() = data.ToKMeansModels
        Dim clusters As EntityClusterModel() = models.Kmeans(expected:=expected)
        Return clusters
    End Function
End Module

