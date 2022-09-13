#Region "Microsoft.VisualBasic::388d78896bf5adbbe327024c5fcae55b, GCModeller\core\Bio.Assembly\Assembly\MiST2\DocArchive\Models\Transducin.vb"

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

    '   Total Lines: 71
    '    Code Lines: 45
    ' Comment Lines: 18
    '   Blank Lines: 8
    '     File Size: 2.73 KB


    '     Class Transducin
    ' 
    '         Properties: [Class], GeneName, ImageUrl, Inputs, Outputs
    ' 
    '         Function: __convert, GetDomainArchitecture, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports SMRUCC.genomics.ProteinModel
Imports SMRUCC.genomics.SequenceModel
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.MiST2

    ''' <summary>
    ''' Signal Transduction Protein
    ''' </summary>
    ''' <remarks></remarks>
    '''
    Public Class Transducin : Inherits Protein
        Implements INamedValue

        <XmlElement> Public Property Inputs As String()
        <XmlElement> Public Property Outputs As String()
        ''' <summary>
        ''' Image file url
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property ImageUrl As String
        <XmlAttribute> Public Property GeneName As String
        <XmlAttribute> Public Property [Class] As String

        Public Overrides Function ToString() As String
            Return ID
        End Function

        ''' <summary>
        ''' 从DomainImage字符串之中解析出结构域数据，并返回自身
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' http://mistdb.com/arch.php?l=646;seg(115|136)+seg(242|254)+seg(517|521)+coil(266|286)+PAS_4(145|244)+HisKA(284|350)+HK_CA:3(353|508)+RR(8|127)+RR(530|646)+
        ''' </remarks>
        Public Function GetDomainArchitecture() As Protein
            Dim strTemp As String = Mid(ImageUrl, InStr(ImageUrl, ";") + 1)
            Dim Domains As String() = strTemp.Split(CChar("+"))
            Dim LQuery = (From str As String
                          In Domains.Take(Domains.Count - 1)
                          Let dm = __convert(str)
                          Select dm
                          Order By dm.Position.Left Ascending).ToArray

            MyBase.Domains = LQuery

            Return Me
        End Function

        Private Function __convert(pfam As String) As DomainObject
            Dim p As Integer = InStr(pfam, "(")
            Dim TempTokens As String() = (From m As Match
                                          In Regex.Matches(Mid(pfam, p), "\d+")
                                          Select m.Value).ToArray
            Dim Domain As New DomainObject With {
                .Name = Mid(pfam, 1, p - 1)
            }
            Domain.CommonName = Domain.Name
            Domain.Position = New ComponentModel.Loci.Location With {
                .Left = Convert.ToInt64(TempTokens(0)),
                .Right = Convert.ToInt64(TempTokens(1))
            }
            Return Domain
        End Function
    End Class
End Namespace
