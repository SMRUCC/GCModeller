#Region "Microsoft.VisualBasic::ba028b49d2ec594e1c216c667fa2ebcb, modules\Knowledge_base\ncbi_kb\MeSH\DescriptorRecordSet.vb"

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

    '   Total Lines: 39
    '    Code Lines: 16 (41.03%)
    ' Comment Lines: 16 (41.03%)
    '    - Xml Docs: 87.50%
    ' 
    '   Blank Lines: 7 (17.95%)
    '     File Size: 1.60 KB


    '     Class DescriptorRecordSet
    ' 
    '         Properties: DescriptorRecord, LanguageCode
    ' 
    '         Function: ReadTerms, TreeTermIndex
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace MeSH

    ''' <summary>
    ''' the mesh Descriptor Record Set xml file
    ''' </summary>
    ''' <remarks>
    ''' which could be download from the ncbi ftp website: 
    ''' 
    ''' https://nlmpubs.nlm.nih.gov/projects/mesh/MESH_FILES/xmlmesh/
    ''' https://nlmpubs.nlm.nih.gov/projects/mesh/MESH_FILES/xmlmesh/desc2024.zip
    ''' 
    ''' https://nlmpubs.nlm.nih.gov/projects/mesh/MESH_FILES/xmlmesh/?_gl=1*jikpoo*_ga*MTQ4NzExODI0OS4xNjg3NDAyOTQ4*_ga_7147EPK006*MTcxMTE2MDE3Ny4xLjEuMTcxMTE2MDQzNC4wLjAuMA..*_ga_P1FPTH9PL4*MTcxMTE2MDE3Ny4xLjEuMTcxMTE2MDQzNC4wLjAuMA..
    ''' </remarks>
    Public Class DescriptorRecordSet

        <XmlAttribute> Public Property LanguageCode As String

        <XmlElement>
        Public Property DescriptorRecord As DescriptorRecord()

        Public Shared Function ReadTerms(file As String, Optional tqdm As Boolean = True) As IEnumerable(Of DescriptorRecord)
            Return file.LoadUltraLargeXMLDataSet(Of DescriptorRecord)(tqdm:=tqdm)
        End Function

        ''' <summary>
        ''' create term index by name
        ''' </summary>
        ''' <param name="terms"></param>
        ''' <returns></returns>
        Public Shared Function TreeTermIndex(terms As IEnumerable(Of DescriptorRecord)) As Dictionary(Of String, DescriptorRecord)
            Return terms.SafeQuery.ToDictionary(Function(term) term.DescriptorName.String)
        End Function

    End Class
End Namespace
