#Region "Microsoft.VisualBasic::846174117b6d8a3b3c034bb4b9fcf718, RDotNET.Extensions.VisualBasic\ScriptBuilder\Extensions.vb"

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

    '     Module RScripts
    ' 
    '         Properties: NA
    ' 
    '         Function: [dim], (+5 Overloads) c, commandArgs, getOption, library
    '                   list, median, names, par, Rbool
    '                   rep, (+2 Overloads) Rstring, t, UnixPath
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace SymbolBuilder

    Public Module RScripts

        ''' <summary>
        ''' "NA" 字符串，而不是NA空值常量
        ''' </summary>
        Public ReadOnly Property NA As New RExpression("NA")

        Public Const [TRUE] As String = "TRUE"
        Public Const [FALSE] As String = "FALSE"

        ''' <summary>
        ''' 将VB.NET之中逻辑值<see cref="System.Boolean"/>转换为R语言之中的逻辑值
        ''' </summary>
        ''' <param name="bool"></param>
        ''' <returns></returns>
        <Extension>
        Public Function λ(bool As Boolean) As String
            Return If(bool, [TRUE], [FALSE])
        End Function

        ''' <summary>
        ''' Retrieve or set the dimension of an object.
        ''' </summary>
        ''' <param name="x">
        ''' an R object, for example a matrix, array or data frame.
        ''' For the default method, either NULL or a numeric vector, which is coerced to integer (by truncation).
        ''' </param>
        ''' <returns>
        ''' For an array (And hence in particular, for a matrix) dim retrieves the dim attribute of the object. It Is NULL Or a vector of mode integer.
        ''' The replacement method changes the "dim" attribute (provided the New value Is compatible) And removes any "dimnames" And "names" attributes.
        ''' </returns>
        ''' <remarks>
        ''' Details
        '''
        ''' The functions Dim And Dim&lt;- are internal generic primitive functions.
        ''' Dim has a method For data.frames, which returns the lengths Of the row.names attribute Of x And Of x (As the numbers Of rows And columns respectively).
        ''' </remarks>
        Public Function [dim](x As String) As String
            Return $"dim({x})"
        End Function

        ''' <summary>
        ''' Given a matrix or data.frame x, t returns the transpose of x.
        ''' </summary>
        ''' <param name="x">a matrix or data frame, typically.</param>
        ''' <returns>A matrix, with dim and dimnames constructed appropriately from those of x, and other attributes except names copied across.</returns>
        ''' <remarks>
        ''' This is a generic function for which methods can be written. The description here applies to the default and "data.frame" methods.
        ''' A data frame Is first coerced To a matrix: see as.matrix. When x Is a vector, it Is treated as a column, i.e., the result Is a 1-row matrix.
        ''' </remarks>
        Public Function t(x As String) As String
            Return $"t({x})"
        End Function

        ''' <summary>
        ''' Normalize the file path as the URL format in Unix system.
        ''' (这个函数只会将目标路径转换为unix格式的文件路径，而不会自动添加双引号)
        ''' </summary>
        ''' <param name="file">The file path string</param>
        ''' <param name="extendsFull">是否转换为全路径？默认不转换</param>
        ''' <returns></returns>
        <Extension>
        Public Function UnixPath(file As String, Optional extendsFull As Boolean = False) As String
            If String.IsNullOrEmpty(file) Then
                Return ""
            End If
            If extendsFull Then
                file = FileIO.FileSystem.GetFileInfo(file).FullName
            End If
            Return file.Replace("\"c, "/"c)
        End Function

        ''' <summary>
        ''' 这个函数仅用于生成脚本
        ''' </summary>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Function c(ParamArray b As Boolean()) As String
            Dim cx As String = String.Join(", ", b.Select(AddressOf λ).ToArray)
            Return $"c({cx})"
        End Function

        ''' <summary>
        ''' c(....).(请注意，这个会为每一个字符串元素添加双引号)
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Function c(ParamArray x As String()) As String
            Dim cx As String = String.Join(", ", x.Select(Function(s) $"""{s}""").ToArray)
            Return $"c({cx})"
        End Function

        ''' <summary>
        ''' c(....).(这个不会添加双引号，这个函数只是用于生成R脚本，并没有在运行时环境之中被执行)
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function c(Of T)(ParamArray x As T()) As String
            Dim cx As String = (From o In x Select Scripting.ToString(o, NULL)).JoinBy(", ")
            Return $"c({cx})"
        End Function

        Public Function c(vector As Array) As String
            Dim cx$ = (From o In vector Select Scripting.ToString(o, NULL)).JoinBy(", ")
            Return $"c({cx})"
        End Function

        ''' <summary>
        ''' c(....).(这个不会添加双引号)
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function c(Of T)(x As IEnumerable(Of T)) As String
            Return c(x.ToArray)
        End Function

        ''' <summary>
        ''' ##### Options Settings
        ''' 
        ''' Allow the user to set and examine a variety of global options which affect the way 
        ''' in which R computes and displays its results.
        ''' </summary>
        ''' <param name="x">a character string holding an option name.</param>
        ''' <param name="default">
        ''' if the specified option is not set in the options list, this value is returned. 
        ''' This facilitates retrieving an option and checking whether it is set and setting 
        ''' it separately if not.
        ''' </param>
        ''' <returns>
        ''' Invoking options() with no arguments returns a list with the current values of the options. 
        ''' Note that not all options listed below are set initially. To access the value of a single 
        ''' option, one should use, e.g., getOption("width") rather than options("width") which is a 
        ''' list of length one.
        ''' </returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function getOption(x$, Optional [default] As Object = NULL) As String
            Return $"getOption(""{x}"", default = {Scripting.ToString([default], NULL)})"
        End Function

        ''' <summary>
        '''  Treat the target input string a the string vector in the R language.
        ''' </summary>
        ''' <param name="s">Input value</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension> Public Function Rstring(s As String) As String
            Select Case s
                Case """", "\"""
                    Return """\"""""
                Case "\n", "\t"
                    Return $"""{s}"""
                Case Else
                    Return $"""{s.R_Escaping}"""
            End Select
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Rstring(Of T As {Structure})([enum] As T) As String
            Return [enum].ToString.Rstring
        End Function

        ''' <summary>
        '''  Escaping the boolean value in VisualBasic as the bool value in R language
        ''' </summary>
        ''' <param name="b"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Rbool(b As Boolean) As String
            Return b.ToString.ToUpper
        End Function

        Public Function par(x As String) As String
            Return $"par(""{x}"")"
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="x">是一个对象，不是字符串</param>
        ''' <returns></returns>
        Public Function median(x As String) As String
            Return $"media({x})"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function library([lib] As String) As String
            Return $"library({[lib]})"
        End Function

        ''' <summary>
        ''' Functions to get or set the names of an object.
        ''' </summary>
        ''' <param name="x">an R object.</param>
        ''' <returns>
        ''' a character vector of up to the same length as x, or NULL.
        '''
        ''' Value
        ''' For names, NULL Or a character vector of the same length as x. (NULL Is given if the object has no names, including for objects of types which cannot have names.)
        ''' For an environment, the length Is the number of objects in the environment but the order of the names Is arbitrary.
        ''' For names&lt;-, the updated object. (Note that the value of names(x) &lt;- value Is that of the assignment, value, Not the return value from the left-hand side.)
        '''
        ''' Note
        ''' For vectors, the names are one of the attributes with restrictions on the possible values. For pairlists, the names are the tags And converted To And From a character vector.
        ''' For a one-dimensional array the names attribute really Is dimnames[[1]].
        ''' Formally classed aka “S4” objects typically have slotNames() (And no names()).
        ''' </returns>
        ''' <remarks>
        ''' names is a generic accessor function, and names&lt;- is a generic replacement function. The default methods get and set the "names" attribute of a vector (including a list) or pairlist.
        ''' For an environment env, names(env) gives the names of the corresponding list, i.e., names(as.list(env, all.names = TRUE)) which are also given by ls(env, all.names = TRUE, sorted = FALSE). If the environment Is used as a hash table, names(env) are its “keys”.
        ''' If value Is shorter than x, it Is extended by character NAs To the length Of x.
        ''' It Is possible to update just part of the names attribute via the general rules: see the examples. This works because the expression there Is evaluated As z &lt;- "names&lt;-"(z, "[&lt;-"(names(z), 3, "c2")).
        ''' The name "" Is special: it Is used to indicate that there Is no name associated with an element of a (atomic Or generic) vector. Subscripting by "" will match nothing (Not even elements which have no name).
        ''' A name can be character NA, but such a name will never be matched And Is likely To lead To confusion.
        ''' Both are primitive functions.
        ''' </remarks>
        Public Function names(x As String) As RExpression
            Return New RExpression($"names({x})")
        End Function

        ''' <summary>
        ''' Provides access to a copy of the command line arguments supplied when this R session was invoked.
        ''' </summary>
        ''' <param name="trailingOnly">logical. Should only arguments after --args be returned?</param>
        ''' <returns>
        ''' A character vector containing the name of the executable and the user-supplied command line arguments.
        ''' The first element is the name of the executable by which R was invoked.
        ''' The exact form of this element is platform dependent: it may be the fully qualified name, or simply the last component (or basename) of the application, or for an embedded R it can be anything the programmer supplied.
        ''' If trailingOnly = True, a character vector Of those arguments (If any) supplied after --args.
        ''' </returns>
        ''' <remarks>
        ''' These arguments are captured before the standard R command line processing takes place. This means that they are the unmodified values.
        ''' This is especially useful with the --args command-line flag to R, as all of the command line after that flag is skipped.
        ''' </remarks>
        Public Function commandArgs(Optional trailingOnly As Boolean = False) As String
            Return $"commandArgs(trailingOnly = {New RBoolean(trailingOnly)})"
        End Function

        ''' <summary>
        ''' rep replicates the values in x. It is a generic function, and the (internal) default method is described here.
        ''' </summary>
        ''' <param name="x">a vector (of any mode including a list) or a factor or (for rep only) a POSIXct or POSIXlt or Date object; or an S4 object containing such an object.</param>
        ''' <returns></returns>
        Public Function rep(ParamArray x As String()) As String
            Return $"rep({String.Join(", ", x)})"
        End Function

        ''' <summary>
        ''' Functions to construct, coerce and check for both kinds of R lists.
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function list(ParamArray x As String()) As String
            Return $"list({String.Join(", ", x)})"
        End Function
    End Module
End Namespace
