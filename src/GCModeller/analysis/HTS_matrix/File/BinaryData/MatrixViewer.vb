#Region "Microsoft.VisualBasic::b0f559e24a774c901dc3584f11343612, analysis\HTS_matrix\File\BinaryData\MatrixViewer.vb"

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

    '   Total Lines: 58
    '    Code Lines: 26 (44.83%)
    ' Comment Lines: 23 (39.66%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (15.52%)
    '     File Size: 2.10 KB


    ' Class MatrixViewer
    ' 
    '     Properties: Size, tag
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

''' <summary>
''' the abstract viewer model of the in-memory matrix object and the
''' huge binary matrix file
''' </summary>
Public MustInherit Class MatrixViewer

    Public ReadOnly Property tag As String
        Get
            Return tagString
        End Get
    End Property

    Protected tagString As String

    Public MustOverride ReadOnly Property SampleIDs As IEnumerable(Of String)

    ''' <summary>
    ''' get gene row features name
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride ReadOnly Property FeatureIDs As IEnumerable(Of String)

    Public Overridable ReadOnly Property Size As (nsample As Integer, nfeature As Integer)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return (SampleIDs.Count, FeatureIDs.Count)
        End Get
    End Property

    Public MustOverride Function GetSampleOrdinal(sampleID As String) As Integer
    ''' <summary>
    ''' get gene expression across all sample data
    ''' </summary>
    ''' <param name="geneID">a specific gene target</param>
    ''' <returns>
    ''' expression data is aligned with the <see cref="SampleIDs"/>
    ''' </returns>
    Public MustOverride Function GetGeneExpression(geneID As String) As Double()
    ''' <summary>
    ''' get a set of gene expression across a specific sample data
    ''' </summary>
    ''' <param name="geneID">a set of target gene</param>
    ''' <param name="sampleOrdinal">the order index of the specific sample data</param>
    ''' <returns>
    ''' expression data is aligned with the <paramref name="geneID"/> set.
    ''' </returns>
    Public MustOverride Function GetGeneExpression(geneID As String(), sampleOrdinal As Integer) As Double()
    Public MustOverride Sub SetNewGeneIDs(geneIDs As String())
    Public MustOverride Sub SetNewSampleIDs(sampleIDs As String())

    Public Overrides Function ToString() As String
        Dim size = Me.Size
        Return $"{tag}({size.nsample}x{size.nfeature})"
    End Function

End Class
