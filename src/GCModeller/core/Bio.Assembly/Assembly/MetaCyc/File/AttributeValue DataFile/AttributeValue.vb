#Region "Microsoft.VisualBasic::a1d8f597538f79c52f8f60f0a7629a8a, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\AttributeValue.vb"

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

    '   Total Lines: 79
    '    Code Lines: 28
    ' Comment Lines: 44
    '   Blank Lines: 7
    '     File Size: 3.59 KB


    '     Class AttributeValue
    ' 
    '         Properties: DbProperty, Objects
    ' 
    '         Function: GetEnumerator, GetEnumerator1, LoadDoc
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language

Namespace Assembly.MetaCyc.File

    ''' <summary>
    ''' Attribute-Value: Each attribute-value file contains data for one class of objects,
    ''' such as genes or proteins. A file is divided into entries, where one entry describes
    ''' one database object.
    ''' </summary>
    ''' <remarks>
    ''' An entry consists of a set of attribute-value pairs, •which describe properties of
    ''' the object, and relationships of the object to other object. Each attribute-value
    ''' pair typically resides on a single line of the file, although in some cases for
    ''' values that are long strings, the value will reside on multiple lines. An attribute-
    ''' value pair consists of an attribute name, followed by the string " - " and a value,
    ''' for example:
    '''
    ''' LEFT - NADP
    '''
    ''' A value that requires more than one line is continued by a newline followed by a /.
    ''' Thus, literal slashes at the beginning of a line must be escaped as //. A line that
    ''' contains only // separates objects. Comment lines can be anywhere in the file and
    ''' must begin with the following symbol:
    '''
    ''' #
    '''
    ''' Starting in version 6.5 of Pathway Tools, attribute-value files can also contain
    ''' annotation-value pairs. Annotations are a mechanism for attaching labeled values
    ''' to specific attribute values. For example, we might want to specify a coefficient
    ''' for a reactant in a chemical reaction. An annotations refers to the attribute value
    ''' that immediately precedes the annotation.
    ''' An annotation-value pair consists of a caret symbol "^" that points upward to indicate
    ''' that the annotation annotates the preceding attribute value, followed by the annotation
    ''' label, followed by the string " - ", followed by a value. The same attribute name or
    ''' annotation label with different values can appear any number of times in an object.
    ''' An example annotation-value pair that refers to the preceding attribute-value pair is:
    '''
    ''' LEFT - NADP
    ''' ^COEFFICIENT - 1
    ''' </remarks>
    Public Class AttributeValue
        Implements IEnumerable(Of ObjectModel)

        ''' <summary>
        ''' The database property in the head section.
        ''' </summary>
        ''' <returns></returns>
        Public Property DbProperty As [Property]
        ''' <summary>
        ''' Slots objects reader model.
        ''' </summary>
        ''' <returns></returns>
        Public Property Objects As ObjectModel()

        Public Shared Function LoadDoc(path As String) As AttributeValue
            Dim file As ObjectModel() = Nothing
            Dim prop As [Property] = Nothing
            Dim message As String = FileReader.TryParse(path, prop, file)

            Return New AttributeValue With {
                .Objects = file.ToArray,
                .DbProperty = prop
            }
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of ObjectModel) Implements IEnumerable(Of ObjectModel).GetEnumerator
            For i As Integer = 0 To Objects.Count - 1
                Yield Objects(i)
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
