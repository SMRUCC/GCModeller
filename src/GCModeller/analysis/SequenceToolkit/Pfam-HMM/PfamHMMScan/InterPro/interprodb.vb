#Region "Microsoft.VisualBasic::45f6f5488935e16281dfcc9f21ccaf00, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\InterPro\interprodb.vb"

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

    '   Total Lines: 42
    '    Code Lines: 29 (69.05%)
    ' Comment Lines: 3 (7.14%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (23.81%)
    '     File Size: 1.40 KB


    '     Class interprodb
    ' 
    '         Properties: interpro, release
    ' 
    '         Function: ReadTerms, Save
    ' 
    '     Class dbinfo
    ' 
    '         Properties: dbname, entry_count, file_date, version
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Linq.Data

Namespace InterPro.Xml

    ''' <summary>
    ''' ftp://ftp.ebi.ac.uk/pub/databases/interpro/current_release/
    ''' </summary>
    <XmlType("interprodb")>
    Public Class interprodb

        Public Property release As dbinfo()

        <XmlElement>
        Public Property interpro As Interpro()

        Public Function Save(FilePath As String, Optional Encoding As Encoding = Nothing) As Boolean
            Return Me.GetXml.SaveTo(FilePath, Encoding)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ReadTerms(file As String) As IEnumerable(Of Interpro)
            Return LoadUltraLargeXMLDataSet(Of Interpro)(file, "interpro", preprocess:=AddressOf abstract.CleanText, tqdm:=True)
        End Function

    End Class

    <XmlType("dbinfo")>
    Public Class dbinfo

        <XmlAttribute> Public Property dbname As String
        <XmlAttribute> Public Property entry_count As Integer
        <XmlAttribute> Public Property file_date As String
        <XmlAttribute> Public Property version As String

        Public Overrides Function ToString() As String
            Return $"[{version}] {dbname}"
        End Function
    End Class
End Namespace
